using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics;

namespace Bepal;

public static class Palette
{
    public static readonly Color Panel = new(34, 26, 32);
    public static readonly Color PanelEdge = new(214, 150, 88);
    public static readonly Color Warm = new(255, 196, 120);
    public static readonly Color Text = new(246, 234, 214);
    public static readonly Color Dim = new(160, 146, 140);
    public static readonly Color Danger = new(232, 64, 72);
    public static readonly Color Coin = new(255, 206, 84);

    public static readonly Color Train = new(232, 72, 72);
    public static readonly Color Feed = new(246, 206, 64);
    public static readonly Color Clean = new(72, 170, 240);
    public static readonly Color Heal = new(88, 204, 116);

    public static readonly Color Hp = new(226, 72, 80);
    public static readonly Color Stomach = new(240, 150, 64);
    public static readonly Color CleanBar = new(84, 172, 236);
    public static readonly Color Progress = new(176, 120, 232);
}

public class Button
{
    public Rectangle Rect;
    public string Label;
    public bool Enabled = true;
    public Color Accent = Palette.PanelEdge;

    public Button(Rectangle rect, string label)
    {
        Rect = rect;
        Label = label;
    }

    float _hover;   // 0..1, eased toward the hover state in Draw

    public bool Hover => Enabled && Rect.Contains(Input.Mouse);

    /// <summary>Returns true when clicked this frame.</summary>
    public bool Update()
    {
        if (!Hover || !Input.Click) return false;
        Audio.Play(Sfx.UiClick);
        return true;
    }

    public void Draw(SpriteBatch sb)
    {
        _hover = Ease.Damp(_hover, Hover ? 1 : 0, Balance.ButtonHoverK, Ui.Dt);
        var r = Rect;
        r.Y -= (int)MathF.Round(Balance.ButtonLiftPx * _hover);
        var bg = !Enabled ? new Color(40, 36, 40) : Color.Lerp(Palette.Panel, Accent, 0.35f * _hover);
        Ui.Frame(sb, r, bg * 0.95f, Enabled ? Color.Lerp(Accent, Palette.Warm, 0.4f * _hover) : new Color(80, 70, 70));
        float room = r.Width - Ui.Pad;
        var font = Label.Length == 1 ? Gfx.Big                                   // glyph buttons ("+") need the big size to read
            : Gfx.Font.MeasureString(Label).X > room ? Gfx.Small : Gfx.Font;   // long label: drop to the small size first
        var label = Gfx.Fit(font, Label, room);
        Gfx.Text(sb, font, label, new Vector2(r.Center.X, r.Center.Y - font.LineSpacing / 2f), Enabled ? Palette.Text : Palette.Dim, 0.5f);
    }
}

/// <summary>Horizontal 0..1 slider: click or drag on the track; <see cref="Changed"/>/<see cref="Released"/> report this frame's events.</summary>
public class Slider
{
    public Rectangle Rect;
    public float Value;
    public bool Changed, Released;
    bool _drag;

    public Slider(Rectangle rect, float value)
    {
        Rect = rect;
        Value = value;
    }

    Rectangle Hit => new(Rect.X - 12, Rect.Y - 14, Rect.Width + 24, Rect.Height + 28);

    public void Update()
    {
        Changed = Released = false;
        if (Input.Click && Hit.Contains(Input.Mouse)) _drag = true;
        if (!_drag) return;
        if (Input.MouseHeld)
        {
            float v = MathHelper.Clamp((Input.Mouse.X - Rect.X) / (float)Rect.Width, 0, 1);
            if (v != Value) { Value = v; Changed = true; }
        }
        else
        {
            _drag = false;
            Released = true;
        }
    }

    public void Nudge(float delta)
    {
        float v = MathHelper.Clamp(MathF.Round((Value + delta) * 100f) / 100f, 0, 1);
        if (v == Value) return;
        Value = v;
        Changed = Released = true;
    }

    public void Draw(SpriteBatch sb, bool selected)
    {
        Gfx.Rect(sb, Rect.X, Rect.Center.Y - 4, Rect.Width, 8, new Color(60, 50, 56));
        Gfx.Rect(sb, Rect.X, Rect.Center.Y - 4, (int)(Rect.Width * Value), 8, Palette.Warm);
        var knob = new Vector2(Rect.X + Rect.Width * Value, Rect.Center.Y);
        Gfx.Circle(sb, knob, selected || _drag ? 13 : 10, selected || _drag ? Palette.Text : Palette.Warm);
    }
}

public static class Ui
{
    /// <summary>Frame step of the current update, set once per frame by SceneManager (Button tween reads it).</summary>
    public static float Dt = 1 / 60f;
    /// <summary>0..1 fade-in of the overlay currently being drawn (1 = fully open); scales <see cref="Dim"/>.</summary>
    public static float OpenAlpha = 1f;

    /// <summary>Layout rhythm shared by the screens: safe margin from the screen edge, inner padding of a panel, gap under a title.</summary>
    public const int Margin = 32, Pad = 24, TitleGap = 16;

    public static void Panel(SpriteBatch sb, Rectangle r, float alpha = 0.95f)
    {
        Frame(sb, r, Palette.Panel * alpha, Palette.PanelEdge);
    }

    /// <summary>Rounded box: <paramref name="fill"/> under a 2px <paramref name="edge"/> border (MonoGame.Extended nine-patch).</summary>
    public static void Frame(SpriteBatch sb, Rectangle r, Color fill, Color edge)
    {
        sb.Draw(Gfx.PanelFill, r, fill);
        sb.Draw(Gfx.PanelRing, r, edge);
    }

    public static void Bar(SpriteBatch sb, Rectangle r, float value, float max, Color fill, string? label = null)
    {
        Gfx.Rect(sb, r, new Color(20, 16, 20));
        float t = max <= 0 ? 0 : MathHelper.Clamp(value / max, 0, 1);
        Gfx.Rect(sb, r.X + 2, r.Y + 2, (r.Width - 4) * t, r.Height - 4, fill);
        if (label != null)
            Gfx.Text(sb, Gfx.Small, Gfx.Fit(Gfx.Small, label, r.Width - 12), new Vector2(r.X + 6, r.Y + (r.Height - Gfx.Small.LineSpacing) / 2f), Palette.Text);
    }

    // Oversized so it still covers the screen while an overlay is scaled down during its open animation
    public static void Dim(SpriteBatch sb, float a = 0.6f) => Gfx.Rect(sb, -100, -100, Gfx.W + 200, Gfx.H + 200, Color.Black * (a * OpenAlpha));

    public static void Hint(SpriteBatch sb, string text) =>
        Gfx.Text(sb, Gfx.Small, text, new Vector2(Gfx.W / 2f, Gfx.H - 30), Palette.Dim, 0.5f);
}
