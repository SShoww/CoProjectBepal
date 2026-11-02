using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bepal;

/// <summary>
/// Day 4 rain VFX. Drops are a pure function of time (no per-frame allocation); <see cref="Tick"/> runs from
/// <see cref="SceneManager.Update"/> so the rain keeps falling while dialogue scenes sit on top of the base.
/// </summary>
public static class Rain
{
    const int MaxDrops = 280;
    const float Wind = 0.28f;            // horizontal px per vertical px
    static readonly float[] X01 = new float[MaxDrops], Y01 = new float[MaxDrops], Speed = new float[MaxDrops], Len = new float[MaxDrops];

    static double _t;
    static float _intensity;             // 0..1, eased towards Target
    static float _flash, _flashDelay, _nextBolt = 4f;
    static readonly Random Rng = new();

    /// <summary>Desired rain strength (0 = dry, 1 = full storm). Set by the scene that draws the sky.</summary>
    public static float Target;

    static Rain()
    {
        var r = new Random(4);
        for (int i = 0; i < MaxDrops; i++)
        {
            X01[i] = (float)r.NextDouble();
            Y01[i] = (float)r.NextDouble();
            bool near = i % 3 == 0;
            Speed[i] = near ? 1500 + (float)r.NextDouble() * 300 : 950 + (float)r.NextDouble() * 250;
            Len[i] = near ? 26 + (float)r.NextDouble() * 10 : 12 + (float)r.NextDouble() * 8;
        }
    }

    public static bool Active => _intensity > 0.01f;

    /// <summary>Lightning: a bright flash followed by a weaker echo.</summary>
    public static void Flash()
    {
        _flash = 1f;
        _flashDelay = 0.14f;
    }

    public static void Tick(float dt)
    {
        _t += dt;
        _intensity = MathHelper.Lerp(Target, _intensity, MathF.Exp(-dt * 1.5f));
        if (_flashDelay > 0 && (_flashDelay -= dt) <= 0) _flash = MathF.Max(_flash, 0.6f);
        _flash = MathF.Max(0, _flash - dt * 3.5f);
        if (Target >= 0.9f && (_nextBolt -= dt) <= 0)
        {
            Flash();
            _nextBolt = 4f + (float)Rng.NextDouble() * 5f;
        }
    }

    /// <summary>Overcast tint, drops and a sky flash. Draw after the backdrop, before the walls, so it shows through the windows.</summary>
    public static void DrawSky(SpriteBatch sb)
    {
        if (!Active) return;
        Gfx.Rect(sb, 0, 0, Gfx.W, Gfx.H, new Color(30, 42, 72) * (0.62f * _intensity));
        const int cw = Gfx.W + 400, ch = Gfx.H + 80;
        float t = (float)(_t % 1000);
        int count = (int)(MaxDrops * _intensity);
        for (int i = 0; i < count; i++)
        {
            float fall = Y01[i] * ch + t * Speed[i];
            float y = fall % ch - 40;
            float x = (X01[i] * cw + fall * Wind) % cw - 200;
            bool near = i % 3 == 0;
            var c = new Color(176, 206, 240) * ((near ? 0.9f : 0.6f) * _intensity);
            Gfx.Line(sb, new Vector2(x, y), new Vector2(x - Wind * Len[i], y - Len[i]), c, near ? 3f : 2f);
        }
        if (_flash > 0)
        {
            Gfx.BeginAdditive(sb);
            Gfx.Rect(sb, 0, 0, Gfx.W, Gfx.H, new Color(190, 205, 255) * (0.55f * _flash));
            Gfx.EndAdditive(sb);
        }
    }

    /// <summary>Faint whole-screen flash so the room lights up too. Draw last, under the HUD.</summary>
    public static void DrawFlash(SpriteBatch sb)
    {
        if (_flash > 0) Gfx.Rect(sb, 0, 0, Gfx.W, Gfx.H, new Color(200, 210, 255) * (0.16f * _flash));
    }
}
