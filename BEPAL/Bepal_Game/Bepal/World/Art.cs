using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Bepal;

/// <summary>Placeholder characters built from shapes. "Cute, but something is wrong": eyes always track the player.</summary>
public static class Art
{
    static void Eye(SpriteBatch sb, Vector2 c, float r, Vector2 look, float alpha)
    {
        Gfx.Circle(sb, c, r, Color.White * alpha);
        Gfx.Circle(sb, c + look * r * 0.45f, r * 0.55f, new Color(14, 10, 16) * alpha);
    }

    /// <summary>Draws a pet standing on <paramref name="feet"/>. <paramref name="lookAt"/> is a world/screen point the eyes follow.</summary>
    public static void Pet(SpriteBatch sb, Species s, Color col, Vector2 feet, float scale, float time, Vector2 lookAt,
        float alpha = 1f, bool dead = false, bool highlight = false)
    {
        float bob = dead ? 0 : MathF.Sin(time * 3f) * 3f * scale;
        var body = feet + new Vector2(0, -40 * scale + bob);
        var c = (dead ? Color.Lerp(col, Color.Gray, 0.7f) : col) * alpha;
        var dark = Color.Lerp(c, Color.Black, 0.35f);
        var look = lookAt - body;
        look = look.LengthSquared() > 1 ? Vector2.Normalize(look) : Vector2.Zero;

        Gfx.Ellipse(sb, feet + new Vector2(0, -2), 40 * scale, 8 * scale, Color.Black * 0.3f * alpha);
        if (highlight) Gfx.Glow(sb, body, 90 * scale, 90 * scale, Palette.Warm * 0.55f);

        switch (s)
        {
            case Species.Mossling:
                Gfx.Ellipse(sb, body, 42 * scale, 36 * scale, c);
                Gfx.Ellipse(sb, body + new Vector2(-18, 20) * scale, 12 * scale, 10 * scale, dark);
                Gfx.Ellipse(sb, body + new Vector2(18, 20) * scale, 12 * scale, 10 * scale, dark);
                Gfx.Ellipse(sb, body + new Vector2(6, -40) * scale, 10 * scale, 14 * scale, new Color(120, 190, 90) * alpha);
                Gfx.Circle(sb, body + new Vector2(6, -50) * scale, 7 * scale, new Color(255, 140, 170) * alpha);
                if (!dead)
                {
                    Eye(sb, body + new Vector2(-14, -6) * scale, 11 * scale, look, alpha);
                    Eye(sb, body + new Vector2(14, -6) * scale, 11 * scale, look, alpha);
                }
                break;
            case Species.Nibbleclaw:
                Gfx.Ellipse(sb, body + new Vector2(0, 6) * scale, 46 * scale, 30 * scale, c);
                Gfx.Ellipse(sb, body + new Vector2(30, -14) * scale, 22 * scale, 18 * scale, c);
                Gfx.Ellipse(sb, body + new Vector2(50, -8) * scale, 16 * scale, 6 * scale, dark);        // long snout
                Gfx.Ellipse(sb, body + new Vector2(22, -32) * scale, 6 * scale, 12 * scale, dark);        // ear
                for (int i = 0; i < 3; i++)                                                               // claws
                    Gfx.Rect(sb, body.X + (-30 + i * 6) * scale, body.Y + 30 * scale, 3 * scale, 12 * scale, Color.White * 0.9f * alpha);
                if (!dead) Eye(sb, body + new Vector2(32, -18) * scale, 8 * scale, look, alpha);
                break;
            case Species.Blinkbun:
                Gfx.Ellipse(sb, body + new Vector2(-14, -52) * scale, 8 * scale, 26 * scale, c);
                Gfx.Ellipse(sb, body + new Vector2(14, -52) * scale, 8 * scale, 26 * scale, c);
                Gfx.Ellipse(sb, body, 36 * scale, 38 * scale, c);
                Gfx.Ellipse(sb, body + new Vector2(0, 16) * scale, 22 * scale, 18 * scale, Color.Lerp(c, Color.White, 0.25f));
                if (!dead)
                {
                    Eye(sb, body + new Vector2(-13, -8) * scale, 8 * scale, look, alpha);
                    Eye(sb, body + new Vector2(13, -8) * scale, 8 * scale, look, alpha);
                    Eye(sb, body + new Vector2(0, -26) * scale, 7 * scale, look, alpha);                // third eye
                }
                break;
            case Species.Toothless:
                Gfx.Ellipse(sb, body + new Vector2(0, 8) * scale, 52 * scale, 28 * scale, c);
                Gfx.Ellipse(sb, body + new Vector2(-40, 18) * scale, 26 * scale, 7 * scale, c);           // tail
                Gfx.Ellipse(sb, body + new Vector2(34, -16) * scale, 26 * scale, 20 * scale, c);
                Gfx.Glow(sb, body + new Vector2(26, 4) * scale, 30 * scale, 26 * scale, new Color(190, 70, 255) * 0.9f * alpha);
                Gfx.Ellipse(sb, body + new Vector2(26, 4) * scale, 14 * scale, 12 * scale, new Color(170, 80, 230) * alpha); // acid sac
                Gfx.Ellipse(sb, body + new Vector2(48, -8) * scale, 12 * scale, 3 * scale, Color.Black * alpha);          // toothless mouth
                if (!dead) Eye(sb, body + new Vector2(38, -24) * scale, 7 * scale, look, alpha);
                break;
        }
    }

    public static void Enemy(SpriteBatch sb, Enemy e, Vector2 feet, float time, Vector2 lookAt, float flash)
    {
        if (e.Name == "Toothless")
        {
            Pet(sb, Species.Toothless, Color.Lerp(e.Color, Color.White, flash), feet, 1.6f, time, lookAt);
            return;
        }
        // Big Z: a looming mass with a crown of spikes and too many eyes
        float s = e.Size;
        var c = Color.Lerp(e.Color, Color.White, flash);
        var body = feet + new Vector2(0, -90 * s + MathF.Sin(time * 1.3f) * 4);
        Gfx.Ellipse(sb, feet, 110 * s, 16 * s, Color.Black * 0.4f);
        Gfx.Glow(sb, body, 200 * s, 200 * s, new Color(255, 40, 60) * 0.25f);
        Gfx.Ellipse(sb, body, 90 * s, 96 * s, c);
        for (int i = 0; i < 5; i++)
        {
            var tip = body + new Vector2((-60 + i * 30) * s, -96 * s - (i % 2 == 0 ? 36 : 20) * s);
            Gfx.Line(sb, body + new Vector2((-60 + i * 30) * s, -70 * s), tip, Color.Lerp(c, Color.Black, 0.4f), 10 * s);
        }
        var look = lookAt - body;
        look = look.LengthSquared() > 1 ? Vector2.Normalize(look) : Vector2.Zero;
        Vector2[] eyes = { new(-34, -20), new(30, -24), new(0, -50), new(-8, 4), new(44, 10), new(-50, 18) };
        foreach (var o in eyes) Eye(sb, body + o * s, 10 * s, look, 1f);
        Gfx.Ellipse(sb, body + new Vector2(0, 50) * s, 44 * s, 10 * s, Color.Black);
    }

    /// <summary>The player's still image (null = draw the primitive character instead). Origin = bottom centre of <see cref="PlayerSpriteSrc"/>.</summary>
    public static Texture2D? PlayerSprite;
    /// <summary>Opaque area inside player.png (the file has transparent padding); drawn at <see cref="PlayerSpriteHeight"/> px tall (3x = crisp pixels).</summary>
    public static readonly Rectangle PlayerSpriteSrc = new(21, 9, 17, 33);
    public const float PlayerSpriteHeight = 99;
    /// <summary>Half of the drawn sprite width (px), so a wall can stop the sprite's edge instead of its centre.</summary>
    public static readonly float PlayerHalfWidth = PlayerSpriteSrc.Width * PlayerSpriteHeight / PlayerSpriteSrc.Height / 2;

    public static void LoadPlayer(ContentManager content) => PlayerSprite = content.Load<Texture2D>("Sprites/player");

    public static void PlayerImage(SpriteBatch sb, Vector2 feet, Pose pose, float face)
    {
        Gfx.Ellipse(sb, feet + new Vector2(0, -2), 26, 6, Color.Black * 0.35f);
        SpriteMotion.Draw(sb, PlayerSprite!, PlayerSpriteSrc, feet, PlayerSpriteHeight, pose, face);
    }

    /// <summary>The Lone Sanctuarist — a quiet humanoid in a hooded cloak.</summary>
    /// <param name="lean">Horizontal sway (px) of the head relative to the feet; lower parts shift proportionally.</param>
    /// <param name="bob">Upper body lift (px) while walking; the legs stay planted.</param>
    /// <param name="faceScale">Smoothed facing in -1..1 (defaults to <paramref name="facing"/>) so the face slides through a turn.</param>
    /// <param name="breath">Idle breathing, +-px on the cloak height.</param>
    public static void Player(SpriteBatch sb, Vector2 feet, int facing, float walkT,
        float lean = 0, float bob = 0, float? faceScale = null, float breath = 0)
    {
        float fx = faceScale ?? facing;
        float step = MathF.Sin(walkT * 12f);
        Vector2 U(float h) => feet + new Vector2(lean * h / 92f, -h - bob);   // point on the upper body at height h
        Gfx.Ellipse(sb, feet + new Vector2(0, -2), 26, 6, Color.Black * 0.35f);
        Gfx.Rect(sb, feet.X - 10 + step * 4, feet.Y - 26, 7, 26, new Color(40, 30, 36));
        Gfx.Rect(sb, feet.X + 3 - step * 4, feet.Y - 26, 7, 26, new Color(40, 30, 36));
        Gfx.Ellipse(sb, U(52), 20, 32 + breath, new Color(92, 110, 130));                       // cloak
        Gfx.Circle(sb, U(92 + breath * 0.5f), 16, new Color(92, 110, 130));                     // hood
        Gfx.Circle(sb, U(90 + breath * 0.5f) + new Vector2(5 * fx, 0), 10, new Color(20, 18, 26));   // face shadow
        Gfx.Circle(sb, U(92 + breath * 0.5f) + new Vector2(9 * fx, 0), 2.5f, Palette.Warm);           // glowing eye
        var scarf = U(60);
        Gfx.Rect(sb, scarf.X - 14, scarf.Y, 28, 5, new Color(214, 150, 88));                    // scarf
    }

    public static void Doctor(SpriteBatch sb, Vector2 feet, float time)
    {
        Gfx.Ellipse(sb, feet + new Vector2(0, -2), 26, 6, Color.Black * 0.35f);
        Gfx.Ellipse(sb, feet + new Vector2(0, -50), 24, 50, new Color(230, 230, 236));
        Gfx.Circle(sb, feet + new Vector2(0, -108), 18, new Color(196, 170, 150));
        Gfx.Rect(sb, feet.X - 14, feet.Y - 116, 28, 6, new Color(230, 230, 236));
        Gfx.Rect(sb, feet.X - 3, feet.Y - 70, 6, 18, Palette.Danger);
        Gfx.Rect(sb, feet.X - 9, feet.Y - 64, 18, 6, Palette.Danger);
        Gfx.Circle(sb, feet + new Vector2(-6, -110 + MathF.Sin(time) * 0.5f), 4, Color.White);   // spectacles
        Gfx.Circle(sb, feet + new Vector2(6, -110 + MathF.Sin(time) * 0.5f), 4, Color.White);
    }

    public static void Merchant(SpriteBatch sb, Vector2 feet, float time)
    {
        Gfx.Ellipse(sb, feet + new Vector2(0, -2), 40, 8, Color.Black * 0.35f);
        Gfx.Ellipse(sb, feet + new Vector2(0, -80), 40, 80, new Color(34, 28, 40));
        Gfx.Circle(sb, feet + new Vector2(0, -160), 30, new Color(20, 16, 24));                    // shadowed face
        Gfx.Ellipse(sb, feet + new Vector2(0, -170), 64, 10, new Color(24, 20, 28));             // wide hat brim
        Gfx.Rect(sb, feet.X - 28, feet.Y - 214, 56, 44, new Color(24, 20, 28));
        Gfx.Rect(sb, feet.X - 24, feet.Y - 152, 48, 6, Palette.Coin);                            // gold grin
        Gfx.Circle(sb, feet + new Vector2(-12, -166), 4, Palette.Warm * (0.6f + 0.4f * MathF.Sin(time * 3)));
        Gfx.Circle(sb, feet + new Vector2(12, -166), 4, Palette.Warm * (0.6f + 0.4f * MathF.Sin(time * 3)));
    }
}
