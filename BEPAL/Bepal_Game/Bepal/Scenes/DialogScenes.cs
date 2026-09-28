using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Bepal;

public record Line(string Speaker, string Text);

/// <summary>Typewriter dialogue box. Space / click: finish line, then advance.</summary>
public class DialogueScene : Scene
{
    readonly List<Line> _lines;
    readonly Action? _onDone;
    readonly Action<SpriteBatch, float>? _portrait;
    int _index;
    float _chars;
    float _time;

    public override bool Overlay => true;

    public DialogueScene(IEnumerable<Line> lines, Action? onDone = null, Action<SpriteBatch, float>? portrait = null)
    {
        _lines = lines.ToList();
        _onDone = onDone;
        _portrait = portrait;
    }

    public static DialogueScene Say(string speaker, Action? onDone, params string[] texts) =>
        new(texts.Select(t => new Line(speaker, t)), onDone);

    string Current => Gfx.Wrap(Gfx.Font, _lines[_index].Text, 1060);

    public override void Update(float dt)
    {
        _time += dt;
        _chars += dt * 55f;
        if (Input.Confirm || Input.Click)
        {
            if (_chars < Current.Length) _chars = Current.Length;
            else if (++_index < _lines.Count) _chars = 0;
            else
            {
                M.Remove(this);
                _onDone?.Invoke();
            }
        }
    }

    public override void Draw(SpriteBatch sb)
    {
        Ui.Dim(sb, 0.35f);
        _portrait?.Invoke(sb, _time);
        var box = new Rectangle(80, 500, 1120, 180);
        Ui.Panel(sb, box);
        var line = _lines[Math.Min(_index, _lines.Count - 1)];
        if (line.Speaker.Length > 0)
        {
            var tag = new Rectangle(100, 478, (int)Gfx.Font.MeasureString(line.Speaker).X + 30, 38);
            Ui.Panel(sb, tag);
            Gfx.Text(sb, Gfx.Font, line.Speaker, new Vector2(tag.X + 15, tag.Y + 6), Palette.Warm);
        }
        var text = Current;
        Gfx.Text(sb, Gfx.Font, text[..Math.Min(text.Length, (int)_chars)], new Vector2(110, 530), Palette.Text);
        if (_chars >= text.Length && MathF.Sin(_time * 6) > 0)
            Gfx.Text(sb, Gfx.Small, "Space >", new Vector2(1180, 650), Palette.Warm, 1f);
    }
}

public record Option(string Label, Action Action, bool Enabled = true);

/// <summary>A prompt with mouse buttons (keys 1..n also work).</summary>
public class ChoiceScene : Scene
{
    readonly string _prompt;
    public string Prompt => _prompt;
    readonly List<Option> _options;
    readonly List<Button> _buttons = new();
    readonly bool _cancellable;

    public override bool Overlay => true;

    public ChoiceScene(string prompt, IEnumerable<Option> options, bool cancellable = false)
    {
        _prompt = prompt;
        _options = options.ToList();
        _cancellable = cancellable;
        int w = 220, gap = 30;
        int total = _options.Count * w + (_options.Count - 1) * gap;
        for (int i = 0; i < _options.Count; i++)
            _buttons.Add(new Button(new Rectangle(Gfx.W / 2 - total / 2 + i * (w + gap), 400, w, 60), _options[i].Label)
                { Enabled = _options[i].Enabled });
    }

    public override void Update(float dt)
    {
        for (int i = 0; i < _buttons.Count; i++)
        {
            if (_buttons[i].Update() || (_buttons[i].Enabled && Input.Pressed(Keys.D1 + i)))
            {
                M.Remove(this);
                _options[i].Action();
                return;
            }
        }
        if (_cancellable && Input.Back) M.Remove(this);
    }

    public override void Draw(SpriteBatch sb)
    {
        Ui.Dim(sb, 0.55f);
        var box = new Rectangle(240, 240, 800, 260);
        Ui.Panel(sb, box);
        Gfx.Text(sb, Gfx.Font, Gfx.Wrap(Gfx.Font, _prompt, 740), new Vector2(Gfx.W / 2f, 280), Palette.Text, 0.5f);
        foreach (var b in _buttons) b.Draw(sb);
    }
}

/// <summary>Pick one pet from cards with the mouse (hover shows a glow outline, Figma Scene 2/17).</summary>
public class PetPickScene : Scene
{
    readonly string _title;
    readonly List<Pet> _pets;
    readonly Action<Pet> _onPick;
    readonly Action? _onCancel;
    readonly Func<Pet, bool> _enabled;
    readonly Func<Pet, string>? _subtitle;
    float _time;

    public override bool Overlay => true;

    public PetPickScene(string title, IEnumerable<Pet> pets, Action<Pet> onPick, Action? onCancel = null,
        Func<Pet, bool>? enabled = null, Func<Pet, string>? subtitle = null)
    {
        _title = title;
        _pets = pets.ToList();
        _onPick = onPick;
        _onCancel = onCancel;
        _enabled = enabled ?? (p => !p.Dead);
        _subtitle = subtitle;
    }

    Rectangle Card(int i)
    {
        int w = 250, gap = 30;
        int total = _pets.Count * w + (_pets.Count - 1) * gap;
        return new Rectangle(Gfx.W / 2 - total / 2 + i * (w + gap), 190, w, 340);
    }

    public override void Update(float dt)
    {
        _time += dt;
        for (int i = 0; i < _pets.Count; i++)
        {
            if (((Input.Click && Card(i).Contains(Input.Mouse)) || Input.Pressed(Keys.D1 + i)) && _enabled(_pets[i]))
            {
                M.Remove(this);
                _onPick(_pets[i]);
                return;
            }
        }
        if (_onCancel != null && Input.Back)
        {
            M.Remove(this);
            _onCancel();
        }
    }

    public override void Draw(SpriteBatch sb)
    {
        Ui.Dim(sb, 0.7f);
        Gfx.Text(sb, Gfx.Big, _title, new Vector2(Gfx.W / 2f, 90), Palette.Text, 0.5f);
        for (int i = 0; i < _pets.Count; i++)
        {
            var p = _pets[i];
            var r = Card(i);
            bool ok = _enabled(p);
            bool hover = ok && r.Contains(Input.Mouse);
            Ui.Panel(sb, r, ok ? 0.95f : 0.6f);
            if (hover) Gfx.Outline(sb, r, Palette.Warm, 4);
            Art.Pet(sb, p.Species, p.Color, new Vector2(r.Center.X, r.Y + 190), 1.3f, _time, Input.Mouse.ToVector2(),
                ok ? 1f : 0.5f, p.Dead, hover);
            Gfx.Text(sb, Gfx.Font, p.Name, new Vector2(r.Center.X, r.Y + 210), ok ? Palette.Text : Palette.Dim, 0.5f);
            var sub = _subtitle?.Invoke(p) ?? $"LV {p.Level}  HP {p.HpShown}/{p.MaxHp}  ATK {p.Atk}";
            Gfx.Text(sb, Gfx.Small, sub, new Vector2(r.Center.X, r.Y + 246), Palette.Dim, 0.5f);
            Ui.Bar(sb, new Rectangle(r.X + 20, r.Y + 280, r.Width - 40, 18), p.Hp, p.MaxHp, Palette.Hp);
            if (p.Dead) Gfx.Text(sb, Gfx.Font, "FALLEN", new Vector2(r.Center.X, r.Y + 120), Palette.Danger, 0.5f);
        }
        if (_onCancel != null) Ui.Hint(sb, "Click a pet (or 1-4)   |   ESC  Back");
        else Ui.Hint(sb, "Click a pet (or 1-4)");
    }
}
