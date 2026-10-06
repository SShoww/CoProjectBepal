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

    static Texture2D? _sky, _clouds, _hills, _mid, _near, _fg;

    public static void Load(ContentManager content)
    {
        _sky = content.Load<Texture2D>("Sprites/bg_parallax_sky");
        _clouds = content.Load<Texture2D>("Sprites/bg_parallax_clouds");
        _hills = content.Load<Texture2D>("Sprites/bg_parallax_hills");
        _mid = content.Load<Texture2D>("Sprites/bg_parallax_mid");
        _near = content.Load<Texture2D>("Sprites/bg_parallax_near");
        _fg = content.Load<Texture2D>("Sprites/fg_jungle");
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
