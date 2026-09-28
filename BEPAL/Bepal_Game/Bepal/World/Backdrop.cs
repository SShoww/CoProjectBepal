using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bepal;

/// <summary>
/// Kingdom-style layered parallax: sky gradient, stars, a big planet, far mountains, near hills.
/// <paramref name="night"/> 0 = dusk day palette, 1 = deep night.
/// </summary>
public class Backdrop
{
    const int Col = 6;
    readonly float[] _far;
    readonly float[] _near;
    readonly Vector3[] _stars;   // x (0..1 of layer), y, phase

    public Backdrop(int seed = 7)
    {
        var rng = new Random(seed);
        _far = Heights(900, 250, 90, rng.Next(1000));
        _near = Heights(1400, 170, 60, rng.Next(1000));
        _stars = new Vector3[140];
        for (int i = 0; i < _stars.Length; i++)
            _stars[i] = new Vector3((float)rng.NextDouble(), (float)rng.NextDouble() * 380, (float)rng.NextDouble() * 6);
    }

    static float[] Heights(int count, float baseH, float amp, int phase)
    {
        var h = new float[count];
        for (int i = 0; i < count; i++)
        {
            float x = (i + phase) * Col;
            h[i] = baseH
                   + MathF.Sin(x * 0.004f) * amp
                   + MathF.Sin(x * 0.011f + 1.3f) * amp * 0.45f
                   + MathF.Sin(x * 0.031f + 0.2f) * amp * 0.15f;
        }
        return h;
    }

    public void Draw(SpriteBatch sb, float camX, float night, float time, float groundY = 600)
    {
        // Sky gradient
        var dayTop = new Color(48, 40, 88);
        var dayBottom = new Color(234, 146, 104);
        var nightTop = new Color(6, 6, 20);
        var nightBottom = new Color(38, 28, 66);
        var top = Gfx.Lerp(dayTop, nightTop, night);
        var bottom = Gfx.Lerp(dayBottom, nightBottom, night);
        const int bands = 40;
        for (int i = 0; i < bands; i++)
        {
            float t = i / (float)(bands - 1);
            Gfx.Rect(sb, -20, -20 + i * (groundY + 40) / bands, Gfx.W + 40, (groundY + 40) / bands + 1, Color.Lerp(top, bottom, t * t));
        }

        // Stars (parallax 0.05)
        float starAlpha = 0.15f + 0.85f * night;
        foreach (var s in _stars)
        {
            float x = ((s.X * 1600 - camX * 0.05f) % 1600 + 1600) % 1600 - 160;
            float tw = 0.6f + 0.4f * MathF.Sin(time * 1.7f + s.Z);
            Gfx.Rect(sb, x, s.Y, 2, 2, Color.White * starAlpha * tw);
        }

        // Planet (parallax 0.1)
        var planet = new Vector2(980 - camX * 0.1f, 170);
        Gfx.Glow(sb, planet, 190, 190, Gfx.Lerp(new Color(255, 190, 150), new Color(140, 120, 220), night) * 0.25f);
        Gfx.Circle(sb, planet, 92, Gfx.Lerp(new Color(250, 214, 176), new Color(170, 160, 220), night));
        Gfx.Circle(sb, planet + new Vector2(22, -14), 18, Gfx.Lerp(new Color(232, 188, 150), new Color(146, 136, 200), night));
        Gfx.Circle(sb, planet + new Vector2(-30, 26), 11, Gfx.Lerp(new Color(232, 188, 150), new Color(146, 136, 200), night));

        // Far mountains (0.25) and near hills (0.5)
        Ridge(sb, _far, camX * 0.25f, groundY, Gfx.Lerp(new Color(104, 70, 110), new Color(30, 24, 52), night));
        Ridge(sb, _near, camX * 0.5f, groundY, Gfx.Lerp(new Color(70, 46, 70), new Color(20, 16, 36), night));
    }

    static void Ridge(SpriteBatch sb, float[] heights, float offset, float groundY, Color c)
    {
        int first = (int)(offset / Col);
        float frac = offset - first * Col;
        for (int i = 0; i <= Gfx.W / Col + 2; i++)
        {
            int idx = ((first + i) % heights.Length + heights.Length) % heights.Length;
            float h = heights[idx];
            Gfx.Rect(sb, i * Col - frac, groundY - h, Col + 1, h + 1, c);
        }
    }
}
