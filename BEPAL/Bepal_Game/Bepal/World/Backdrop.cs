using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Bepal;

/// <summary>
/// 5-layer sprite parallax (320x180 pixel art drawn at 4x). Back to front: sky, clouds, hills, mid, near.
/// <paramref name="night"/> 0 = day, 1 = deep night (sprites are tinted).
/// </summary>
public class Backdrop
{
    const int Scale = 4;
    const int TileW = 320 * Scale;   // 1280
    const int TileH = 180 * Scale;   // 720

    // Parallax factors (fraction of camera movement) — tune by eye.
    const float SkyFactor = 0.03f;
    const float CloudFactor = 0.10f;
    const float CloudDrift = 6f;     // px/s of slow drift
    const float HillsFactor = 0.28f;
    const float MidFactor = 0.48f;
    const float NearFactor = 0.70f;

    // Foreground (in front of everything, incl. characters): 320x180 art at 2x, moves faster than the camera.
    const int FgScale = 2;
    const float FgFactor = 1.3f;
    const float FgSink = 60f;        // px the art is pushed below the screen bottom (crops the dark base)

    /// <summary>How many px the sprite bottom sits below groundY (hides it behind the ground fill).</summary>
    const float GroundOverlap = 0f;

    static readonly Color NightTint = new(60, 70, 130);

    static Texture2D? _sky, _clouds, _hills, _mid, _near, _fg, _base;

    /// <summary>True when the painted base interior (<c>bg_base.png</c>, 3840x720 = BaseScene.WorldW) is loaded.</summary>
    public static bool HasBaseInterior => _base != null;

    public static void Load(ContentManager content)
    {
        _sky = content.Load<Texture2D>("Sprites/bg_parallax_sky");
        _clouds = content.Load<Texture2D>("Sprites/bg_parallax_clouds");
        _hills = content.Load<Texture2D>("Sprites/bg_parallax_hills");
        _mid = content.Load<Texture2D>("Sprites/bg_parallax_mid");
        _near = content.Load<Texture2D>("Sprites/bg_parallax_near");
        _fg = content.Load<Texture2D>("Sprites/fg_jungle");
        try { _base = content.Load<Texture2D>("Sprites/bg_base"); }
        catch (ContentLoadException) { }   // no art: BaseScene draws the primitive wall
    }

    const int BaseFloorRow = 596;                         // y of the floor line in bg_base.png
    static readonly Color BaseFloorFill = new(133, 74, 45); // bg_base.png bottom rows, continues the floor below the art

    /// <summary>Painted base interior, scaled uniformly so the wall (image rows 0..<see cref="BaseFloorRow"/>) spans <paramref name="top"/>..<paramref name="floorY"/>
    /// (the old primitive wall's height) and tiled from <paramref name="left"/> to <paramref name="right"/> (screen px), the first tile starting <paramref name="phase"/> px early
    /// (moves the windows clear of the props). Its window is transparent, so draw it over <see cref="Draw"/>.</summary>
    public void DrawBaseInterior(SpriteBatch sb, float left, float right, float top, float floorY, float phase, float night)
    {
        if (_base == null) return;
        var tint = Color.Lerp(Color.White, NightTint, MathHelper.Clamp(night, 0, 1) * 0.7f);
        float s = (floorY - top) / BaseFloorRow;
        Gfx.Rect(sb, 0, floorY, Gfx.W, Gfx.H - floorY, Color.Lerp(BaseFloorFill, Color.Black, MathHelper.Clamp(night, 0, 1) * 0.6f));
        int k = 0;
        for (float x = left - phase; x < right; x += _base.Width * s, k++)
        {
            float x0 = MathF.Max(x, left), x1 = MathF.Min(x + _base.Width * s, right);   // visible part of this tile
            if (x1 <= x0 || x1 < 0 || x0 > Gfx.W) continue;
            int a = (int)((x0 - x) / s), b = (int)((x1 - x) / s);                          // its columns in the (unflipped) tile
            bool flip = (k & 1) == 1;   // mirror every other tile so the edges meet seamlessly
            sb.Draw(_base, new Rectangle((int)MathF.Round(x0), (int)top, (int)MathF.Round(x1 - x0), (int)MathF.Round(_base.Height * s)),
                new Rectangle(flip ? _base.Width - b : a, 0, b - a, _base.Height), tint, 0, Vector2.Zero,
                flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
        }
    }

    public void Draw(SpriteBatch sb, float camX, float night, float time, float groundY = 600)
    {
        var tint = Color.Lerp(Color.White, NightTint, MathHelper.Clamp(night, 0, 1));
        int y = (int)MathF.Round(groundY + GroundOverlap) - TileH;

        // Own point-sampled batch (same shake matrix); then restore the standard batch.
        sb.End();
        Gfx.Begin(sb, null, SamplerState.PointClamp);
        Layer(sb, _sky, camX * SkyFactor, y, tint, false, TileW, TileH);
        Layer(sb, _clouds, camX * CloudFactor + time * CloudDrift, y, tint, false, TileW, TileH);
        Layer(sb, _hills, camX * HillsFactor, y, tint, true, TileW, TileH);
        Layer(sb, _mid, camX * MidFactor, y, tint, true, TileW, TileH);
        Layer(sb, _near, camX * NearFactor, y, tint, false, TileW, TileH);
        sb.End();
        Gfx.Begin(sb);
    }

    /// <summary>Foreground strip along the bottom of the screen, drawn after the ground and characters.</summary>
    public void DrawForeground(SpriteBatch sb, float camX, float night)
    {
        if (_fg == null) return;
        var tint = Color.Lerp(Color.White, NightTint, MathHelper.Clamp(night, 0, 1));
        int w = _fg.Width * FgScale, h = _fg.Height * FgScale;
        sb.End();
        Gfx.Begin(sb, null, SamplerState.PointClamp);
        Layer(sb, _fg, camX * FgFactor, Gfx.H - h + (int)FgSink, tint, true, w, h);
        sb.End();
        Gfx.Begin(sb);
    }

    static void Layer(SpriteBatch sb, Texture2D? tex, float scroll, int y, Color tint, bool mirror, int tileW, int tileH)
    {
        if (tex == null) return;
        int s = (int)MathF.Round(scroll);
        int period = mirror ? tileW * 2 : tileW;
        int o = ((s % period) + period) % period;
        int first = o / tileW;          // index of the leftmost visible tile
        int x = -(o - first * tileW);
        for (int k = 0; x < Gfx.W + 8; k++, x += tileW)
        {
            bool flip = mirror && ((first + k) & 1) == 1;
            sb.Draw(tex, new Rectangle(x, y, tileW, tileH), null, tint, 0, Vector2.Zero,
                flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
        }
    }
}
