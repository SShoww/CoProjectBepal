using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Bepal;

/// <summary>Primitive drawing helpers — everything in the prototype is drawn from shapes (placeholder art).</summary>
public static class Gfx
{
    public const int W = 1280;
    public const int H = 720;

    public static Texture2D Pixel = null!;
    public static Texture2D Disc = null!;   // hard-edged circle
    public static Texture2D Soft = null!;   // radial gradient, for glows
    public static SpriteFont Font = null!;
    public static SpriteFont Big = null!;
    public static SpriteFont Small = null!;

    static readonly Random Rng = new();
    static float _shakeTime, _shakeAmount;
    static Vector2 _shakeOffset;

    public static void Init(GraphicsDevice gd, ContentManager content)
    {
        Pixel = new Texture2D(gd, 1, 1);
        Pixel.SetData(new[] { Color.White });
        Disc = MakeCircle(gd, 128, soft: false);
        Soft = MakeCircle(gd, 128, soft: true);
        Font = content.Load<SpriteFont>("Fonts/Main");
        Big = content.Load<SpriteFont>("Fonts/Big");
        Small = content.Load<SpriteFont>("Fonts/Small");
        Backdrop.Load(content);
        Art.LoadPlayer(content);
    }

    static Texture2D MakeCircle(GraphicsDevice gd, int size, bool soft)
    {
        var tex = new Texture2D(gd, size, size);
        var data = new Color[size * size];
        float r = size / 2f;
        for (int y = 0; y < size; y++)
        for (int x = 0; x < size; x++)
        {
            float d = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), new Vector2(r, r)) / r;
            float a = soft ? MathF.Pow(MathHelper.Clamp(1 - d, 0, 1), 2f) : MathHelper.Clamp((1 - d) * r, 0, 1);
            data[y * size + x] = Color.White * a;
        }
        tex.SetData(data);
        return tex;
    }

    // ---------- screen shake ----------
    public static void Shake(float amount = 8f, float time = 0.25f)
    {
        _shakeAmount = amount;
        _shakeTime = time;
    }

    public static void UpdateShake(float dt)
    {
        if (_shakeTime > 0)
        {
            _shakeTime -= dt;
            float a = _shakeAmount * MathHelper.Clamp(_shakeTime / 0.25f, 0, 1);
            _shakeOffset = new Vector2((float)(Rng.NextDouble() * 2 - 1) * a, (float)(Rng.NextDouble() * 2 - 1) * a);
        }
        else _shakeOffset = Vector2.Zero;
    }

    /// <summary>Extra view transform applied under the shake (set by SceneManager for overlay open animations).</summary>
    public static Matrix ViewExtra = Matrix.Identity;

    public static void Begin(SpriteBatch sb, BlendState? blend = null, SamplerState? sampler = null) =>
        sb.Begin(SpriteSortMode.Deferred, blend ?? BlendState.AlphaBlend, sampler ?? SamplerState.LinearClamp, null, null, null,
            ViewExtra * Matrix.CreateTranslation(_shakeOffset.X, _shakeOffset.Y, 0));

    /// <summary>Switch to additive blending (for light), then call <see cref="EndAdditive"/>.</summary>
    public static void BeginAdditive(SpriteBatch sb)
    {
        sb.End();
        Begin(sb, BlendState.Additive);
    }

    public static void EndAdditive(SpriteBatch sb)
    {
        sb.End();
        Begin(sb);
    }

    // ---------- shapes ----------
    public static void Rect(SpriteBatch sb, float x, float y, float w, float h, Color c) =>
        sb.Draw(Pixel, new Vector2(x, y), null, c, 0, Vector2.Zero, new Vector2(w, h), SpriteEffects.None, 0);

    public static void Rect(SpriteBatch sb, Rectangle r, Color c) => Rect(sb, r.X, r.Y, r.Width, r.Height, c);

    public static void Outline(SpriteBatch sb, Rectangle r, Color c, int t = 2)
    {
        Rect(sb, r.X, r.Y, r.Width, t, c);
        Rect(sb, r.X, r.Bottom - t, r.Width, t, c);
        Rect(sb, r.X, r.Y, t, r.Height, c);
        Rect(sb, r.Right - t, r.Y, t, r.Height, c);
    }

    /// <summary>Filled ellipse centred on <paramref name="c"/>.</summary>
    public static void Ellipse(SpriteBatch sb, Vector2 c, float rx, float ry, Color col) =>
        sb.Draw(Disc, c, null, col, 0, new Vector2(64, 64), new Vector2(rx / 64f, ry / 64f), SpriteEffects.None, 0);

    public static void Circle(SpriteBatch sb, Vector2 c, float r, Color col) => Ellipse(sb, c, r, r, col);

    public static void Glow(SpriteBatch sb, Vector2 c, float rx, float ry, Color col) =>
        sb.Draw(Soft, c, null, col, 0, new Vector2(64, 64), new Vector2(rx / 64f, ry / 64f), SpriteEffects.None, 0);

    public static void Line(SpriteBatch sb, Vector2 a, Vector2 b, Color c, float thickness = 2f)
    {
        var d = b - a;
        sb.Draw(Pixel, a, null, c, MathF.Atan2(d.Y, d.X), new Vector2(0, 0.5f), new Vector2(d.Length(), thickness),
            SpriteEffects.None, 0);
    }

    /// <summary>Point on a wheel. Angle 0 = top, increasing clockwise.</summary>
    public static Vector2 OnWheel(Vector2 center, float radius, float angle) =>
        center + new Vector2(MathF.Sin(angle), -MathF.Cos(angle)) * radius;

    /// <summary>Thick arc using the wheel convention (0 = top, clockwise).</summary>
    public static void Arc(SpriteBatch sb, Vector2 center, float radius, float thickness, float a0, float a1, Color c)
    {
        if (a1 < a0) (a0, a1) = (a1, a0);
        float step = MathF.Max(0.012f, 3f / radius);
        for (float a = a0; a < a1; a += step)
        {
            float seg = MathF.Min(step, a1 - a);
            float mid = a + seg / 2;
            sb.Draw(Pixel, OnWheel(center, radius, mid), null, c, mid, new Vector2(0.5f, 0.5f),
                new Vector2(radius * seg + 1.5f, thickness), SpriteEffects.None, 0);
        }
    }

    // ---------- text ----------
    /// <param name="align">0 = left, 0.5 = centre, 1 = right.</param>
    public static void Text(SpriteBatch sb, SpriteFont font, string s, Vector2 pos, Color c, float align = 0f, bool shadow = true)
    {
        var size = font.MeasureString(s);
        var p = new Vector2(MathF.Round(pos.X - size.X * align), MathF.Round(pos.Y));
        if (shadow) sb.DrawString(font, s, p + new Vector2(2, 2), Color.Black * 0.5f * (c.A / 255f));
        sb.DrawString(font, s, p, c);
    }

    /// <summary>Greedy word wrap.</summary>
    public static string Wrap(SpriteFont font, string text, float width)
    {
        var words = text.Split(' ');
        var line = "";
        var result = "";
        foreach (var w in words)
        {
            var test = line.Length == 0 ? w : line + " " + w;
            if (font.MeasureString(test).X > width && line.Length > 0)
            {
                result += line + "\n";
                line = w;
            }
            else line = test;
        }
        return result + line;
    }

    public static Color Lerp(Color a, Color b, float t) => Color.Lerp(a, b, MathHelper.Clamp(t, 0, 1));

    /// <summary>Absolute angular distance in radians, wrapped to [0, PI].</summary>
    public static float AngDist(float a, float b)
    {
        float d = (a - b) % MathHelper.TwoPi;
        if (d < -MathHelper.Pi) d += MathHelper.TwoPi;
        if (d > MathHelper.Pi) d -= MathHelper.TwoPi;
        return MathF.Abs(d);
    }
}
