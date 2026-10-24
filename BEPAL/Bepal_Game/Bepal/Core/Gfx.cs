using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics;

namespace Bepal;

/// <summary>Primitive drawing helpers — everything in the prototype is drawn from shapes (placeholder art).</summary>
public static class Gfx
{
    public const int W = 1280;
    public const int H = 720;

    public static Texture2D Pixel = null!;
    public static Texture2D Disc = null!;   // hard-edged circle
    public static Texture2D Soft = null!;   // radial gradient, for glows
    public static NinePatch PanelFill = null!, PanelRing = null!;   // rounded frame as white masks (tinted per use), stretched by MonoGame.Extended
    public static SpriteFont Font = null!;
    public static SpriteFont Big = null!;
    public static SpriteFont Title = null!;   // game logo only
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
        PanelFill = MakePanelPatch(gd, 32, 8, 2, ring: false);
        PanelRing = MakePanelPatch(gd, 32, 8, 2, ring: true);
        Font = content.Load<SpriteFont>("Fonts/Main");
        Big = content.Load<SpriteFont>("Fonts/Big");
        Title = content.Load<SpriteFont>("Fonts/Title");
        Small = content.Load<SpriteFont>("Fonts/Small");
        Backdrop.Load(content);
        Art.LoadPlayer(content);
        Art.LoadOptional(content);
        Ui.LoadOptional(content);
    }

    /// <summary>Rounded rectangle (whole shape, or just its border) as an anti-aliased white mask cut into a nine-patch; the corners keep their size when the panel is stretched.</summary>
    static NinePatch MakePanelPatch(GraphicsDevice gd, int size, int radius, int border, bool ring)
    {
        var tex = new Texture2D(gd, size, size);
        var data = new Color[size * size];
        float half = size / 2f;
        for (int y = 0; y < size; y++)
        for (int x = 0; x < size; x++)
        {
            var q = new Vector2(MathF.Abs(x + 0.5f - half), MathF.Abs(y + 0.5f - half)) - new Vector2(half - radius);
            float d = Vector2.Max(q, Vector2.Zero).Length() + MathF.Min(MathF.Max(q.X, q.Y), 0) - radius;   // < 0 inside
            float a = MathHelper.Clamp(0.5f - d, 0, 1);
            if (ring) a *= MathHelper.Clamp(d + border + 0.5f, 0, 1);   // only the outermost <border> px
            data[y * size + x] = Color.White * a;
        }
        tex.SetData(data);
        var cells = new Texture2DRegion[9];
        int[] xs = { 0, radius, size - radius, size };
        for (int row = 0; row < 3; row++)
        for (int col = 0; col < 3; col++)
            cells[row * 3 + col] = new Texture2DRegion(tex, xs[col], xs[row], xs[col + 1] - xs[col], xs[row + 1] - xs[row]);
        return new NinePatch(cells);
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

    /// <summary>View transform of the current frame: overlay scale (<see cref="ViewExtra"/>) under the screen shake.</summary>
    static Matrix ViewMatrix => ViewExtra * Matrix.CreateTranslation(_shakeOffset.X, _shakeOffset.Y, 0);

    public static void Begin(SpriteBatch sb, BlendState? blend = null, SamplerState? sampler = null) =>
        sb.Begin(SpriteSortMode.Deferred, blend ?? BlendState.AlphaBlend, sampler ?? SamplerState.LinearClamp, null, null, null,
            ViewMatrix);

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

    /// <summary>Width of the transparent fringe that anti-aliases the arc's inner and outer edge.</summary>
    const float ArcFeather = 1.5f;
    static BasicEffect? _meshFx;
    static VertexPositionColor[] _arcVerts = new VertexPositionColor[4096];

    /// <summary>
    /// Thick arc using the wheel convention (0 = top, clockwise). Drawn as one mesh of non-overlapping quads
    /// (so a translucent colour never double-blends into stripes) with feathered edges instead of MSAA.
    /// </summary>
    public static void Arc(SpriteBatch sb, Vector2 center, float radius, float thickness, float a0, float a1, Color c)
    {
        if (a1 < a0) (a0, a1) = (a1, a0);
        if (a1 - a0 < 1e-4f) return;
        var gd = sb.GraphicsDevice;
        int n = Math.Max(1, (int)MathF.Ceiling((a1 - a0) / 0.025f));
        int need = n * 18;
        if (_arcVerts.Length < need) _arcVerts = new VertexPositionColor[need];

        // four rim radii: faded-out, solid, solid, faded-out
        float half = thickness / 2f;
        Span<float> rr = stackalloc float[] { radius - half - ArcFeather, radius - half, radius + half, radius + half + ArcFeather };
        Span<Color> cc = stackalloc Color[] { Color.Transparent, c, c, Color.Transparent };
        int v = 0;
        for (int i = 0; i < n; i++)
        {
            float s0 = a0 + (a1 - a0) * i / n, s1 = a0 + (a1 - a0) * (i + 1) / n;
            for (int k = 0; k < 3; k++)
            {
                var p00 = new VertexPositionColor(new Vector3(OnWheel(center, rr[k], s0), 0), cc[k]);
                var p01 = new VertexPositionColor(new Vector3(OnWheel(center, rr[k], s1), 0), cc[k]);
                var p10 = new VertexPositionColor(new Vector3(OnWheel(center, rr[k + 1], s0), 0), cc[k + 1]);
                var p11 = new VertexPositionColor(new Vector3(OnWheel(center, rr[k + 1], s1), 0), cc[k + 1]);
                _arcVerts[v++] = p00; _arcVerts[v++] = p01; _arcVerts[v++] = p10;
                _arcVerts[v++] = p01; _arcVerts[v++] = p11; _arcVerts[v++] = p10;
            }
        }

        var vp = gd.Viewport;
        _meshFx ??= new BasicEffect(gd) { VertexColorEnabled = true, TextureEnabled = false, LightingEnabled = false };
        _meshFx.World = ViewMatrix;
        _meshFx.Projection = Matrix.CreateOrthographicOffCenter(0, vp.Width, vp.Height, 0, 0, 1);

        sb.End();
        var prevBlend = gd.BlendState;
        var prevRaster = gd.RasterizerState;
        gd.BlendState = BlendState.AlphaBlend;
        gd.RasterizerState = RasterizerState.CullNone;
        foreach (var pass in _meshFx.CurrentTechnique.Passes)
        {
            pass.Apply();
            gd.DrawUserPrimitives(PrimitiveType.TriangleList, _arcVerts, 0, v / 3);
        }
        gd.BlendState = prevBlend;
        gd.RasterizerState = prevRaster;
        Begin(sb);
    }

    // ---------- text ----------
    /// <param name="align">0 = left, 0.5 = centre, 1 = right. Multi-line text is aligned line by line.</param>
    public static void Text(SpriteBatch sb, SpriteFont font, string s, Vector2 pos, Color c, float align = 0f, bool shadow = true)
    {
        if (s.Contains('\n'))
        {
            float y = pos.Y;
            foreach (var line in s.Split('\n'))
            {
                Text(sb, font, line, new Vector2(pos.X, y), c, align, shadow);
                y += font.LineSpacing;
            }
            return;
        }
        var size = font.MeasureString(s);
        var p = new Vector2(MathF.Round(pos.X - size.X * align), MathF.Round(pos.Y));
        if (shadow) sb.DrawString(font, s, p + new Vector2(2, 2), Color.Black * 0.5f * (c.A / 255f));
        sb.DrawString(font, s, p, c);
    }

    /// <summary>Shortens <paramref name="s"/> with "..." until it fits <paramref name="maxWidth"/>.</summary>
    public static string Fit(SpriteFont font, string s, float maxWidth)
    {
        if (font.MeasureString(s).X <= maxWidth) return s;
        while (s.Length > 1 && font.MeasureString(s + "...").X > maxWidth) s = s[..^1];
        return s + "...";
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
