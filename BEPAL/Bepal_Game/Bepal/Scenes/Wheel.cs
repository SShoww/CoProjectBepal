using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bepal;

public enum Hit { Miss, Great, Perfect }

public class Zone
{
    public float Center;
    public float Perfect;   // half-width in radians
    public float Great;     // half-width in radians (>= Perfect, or 0 for perfect-only zones)
    public Color Color;
    public string? Label;

    public Hit Test(float angle)
    {
        float d = Gfx.AngDist(angle, Center);
        if (d <= Perfect) return Hit.Perfect;
        if (d <= Great) return Hit.Great;
        return Hit.Miss;
    }
}

/// <summary>Shared rotating-needle wheel. Angle 0 = top, clockwise.</summary>
public class Wheel
{
    public Vector2 Center;
    public float Radius;
    public float Needle;
    public float Speed = 2.4f;
    public int Dir = 1;
    public float NeedleAlpha = 1f;
    public readonly List<Zone> Zones = new();

    static readonly Random Rng = new();

    public Wheel(Vector2 center, float radius)
    {
        Center = center;
        Radius = radius;
    }

    public void Update(float dt) => Needle = (Needle + Dir * Speed * dt) % MathHelper.TwoPi;

    /// <summary>Best hit among zones, and the zone that was hit.</summary>
    public (Hit hit, Zone? zone) Evaluate()
    {
        Hit best = Hit.Miss;
        Zone? zone = null;
        foreach (var z in Zones)
        {
            var h = z.Test(Needle);
            if (h > best)
            {
                best = h;
                zone = z;
            }
        }
        return (best, zone);
    }

    /// <summary>Random angle at least <paramref name="gap"/> from the needle and from every other zone.</summary>
    public float FreeAngle(float gap = 0.6f, Zone? ignore = null)
    {
        for (int tries = 0; tries < 40; tries++)
        {
            float a = (float)(Rng.NextDouble() * MathHelper.TwoPi);
            if (Gfx.AngDist(a, Needle) < gap) continue;
            bool clear = true;
            foreach (var z in Zones)
                if (z != ignore && Gfx.AngDist(a, z.Center) < MathF.Max(z.Great, z.Perfect) + gap * 0.5f) clear = false;
            if (clear) return a;
        }
        return (Needle + MathHelper.Pi) % MathHelper.TwoPi;
    }

    public void Draw(SpriteBatch sb, float thickness = 30)
    {
        Gfx.Glow(sb, Center, Radius * 1.6f, Radius * 1.6f, Color.Black * 0.5f);
        Gfx.Arc(sb, Center, Radius, thickness + 8, 0, MathHelper.TwoPi, new Color(20, 14, 20));
        Gfx.Arc(sb, Center, Radius, thickness, 0, MathHelper.TwoPi, new Color(70, 58, 66));
        foreach (var z in Zones)
        {
            if (z.Great > z.Perfect)
                Gfx.Arc(sb, Center, Radius, thickness, z.Center - z.Great, z.Center + z.Great, Color.Lerp(z.Color, Color.White, 0.1f) * 0.45f);
            Gfx.Arc(sb, Center, Radius, thickness, z.Center - z.Perfect, z.Center + z.Perfect, z.Color);
            if (z.Label != null)
                Gfx.Text(sb, Gfx.Font, z.Label, Gfx.OnWheel(Center, Radius + 44, z.Center) - new Vector2(0, 12), Palette.Text, 0.5f);
        }
        var tip = Gfx.OnWheel(Center, Radius + 22, Needle);
        Gfx.Line(sb, Center, tip, Color.White * NeedleAlpha, 5);
        Gfx.Circle(sb, tip, 7, Color.White * NeedleAlpha);
        Gfx.Circle(sb, Center, 16, new Color(240, 230, 220));
        Gfx.Circle(sb, Center, 8, new Color(40, 30, 36));
    }
}

/// <summary>Floating "Perfect / Great / Miss" text.</summary>
public class Popups
{
    readonly List<(string text, Color color, Vector2 pos, float t)> _items = new();

    public void Add(string text, Color color, Vector2 pos) => _items.Add((text, color, pos, 0));

    public void Add(Hit hit, Vector2 pos) => Add(hit switch
    {
        Hit.Perfect => "Perfect!",
        Hit.Great => "Great",
        _ => "Miss",
    }, hit switch
    {
        Hit.Perfect => Palette.Coin,
        Hit.Great => new Color(150, 220, 255),
        _ => Palette.Danger,
    }, pos);

    public void Update(float dt)
    {
        for (int i = _items.Count - 1; i >= 0; i--)
        {
            var it = _items[i];
            it.t += dt;
            if (it.t > 0.9f) _items.RemoveAt(i);
            else _items[i] = it;
        }
    }

    public void Draw(SpriteBatch sb)
    {
        foreach (var (text, color, pos, t) in _items)
            Gfx.Text(sb, Gfx.Big, text, pos - new Vector2(0, t * 50), color * (1 - t / 0.9f), 0.5f);
    }
}
