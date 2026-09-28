using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bepal;

/// <summary>Figma Scene 1.</summary>
public class MainMenuScene : Scene
{
    readonly Button _play = new(new Rectangle(Gfx.W / 2 - 120, 380, 240, 56), "Play");
    readonly Button _quit = new(new Rectangle(Gfx.W / 2 - 120, 450, 240, 56), "Quit");
    float _time;

    public override void Update(float dt)
    {
        _time += dt;
        if (_play.Update() || Input.Confirm) M.Reset(new ChooseStarterScene());
        else if (_quit.Update()) Game1.Instance.Exit();
    }

    public override void Draw(SpriteBatch sb)
    {
        BaseScene.SharedBackdrop.Draw(sb, _time * 25, 0.55f, _time);
        Gfx.Rect(sb, 0, 600, Gfx.W, 120, new Color(30, 22, 26));
        Gfx.Glow(sb, new Vector2(Gfx.W / 2f, 230), 420, 150, Color.Black * 0.5f);
        Gfx.Text(sb, Gfx.Big, "BePal", new Vector2(Gfx.W / 2f, 170), Palette.Warm, 0.5f);
        Gfx.Text(sb, Gfx.Font, "a sanctuary at the edge of the universe", new Vector2(Gfx.W / 2f, 240), Palette.Text * 0.8f, 0.5f);
        Art.Pet(sb, Species.Blinkbun, Pet.Create(Species.Blinkbun).Color, new Vector2(200, 640), 1f, _time, new Vector2(640, 300));
        Art.Pet(sb, Species.Mossling, Pet.Create(Species.Mossling).Color, new Vector2(1080, 640), 1f, _time + 1, new Vector2(640, 300));
        _play.Draw(sb);
        _quit.Draw(sb);
        if (MathF.Sin(_time * 4) > -0.3f)
            Gfx.Text(sb, Gfx.Font, "[ Space ] to Enter", new Vector2(Gfx.W / 2f, 612), Palette.Text, 0.5f);
        Gfx.Text(sb, Gfx.Small, "A / D  Walk      SPACE  Interact / QTE      Mouse  UI", new Vector2(Gfx.W / 2f, 670), Palette.Dim, 0.5f);
    }
}

/// <summary>Figma Scene 2.</summary>
public class ChooseStarterScene : Scene
{
    float _time;
    bool _picked;

    public override void Enter()
    {
        var pets = new[] { Pet.Create(Species.Mossling), Pet.Create(Species.Nibbleclaw), Pet.Create(Species.Blinkbun) };
        M.Push(new PetPickScene("Choose your pet", pets, OnPick, subtitle: p => $"{p.Element}   HP {p.MaxHp}   ATK {p.Atk}"));
    }

    void OnPick(Pet pet)
    {
        _picked = true;
        var gs = new GameState(pet.Species);
        var baseScene = new BaseScene(gs);
        M.Reset(baseScene, DialogueScene.Say("", null,
            "At the edge of the universe, on a planet no one visits, there is a sanctuary. You are its only keeper.",
            $"Today a small creature arrived: {pet.Name}. It looks normal... mostly. Its eyes follow you everywhere.",
            "Walk with A / D. Press SPACE near things to use them. Walk up to your pet and press SPACE to care for it.",
            "Caring costs Energy. When you're done for the day, sleep in your bed on the far left."));
    }

    public override void Update(float dt) => _time += dt;

    public override void Draw(SpriteBatch sb)
    {
        BaseScene.SharedBackdrop.Draw(sb, 900, 0.3f, _time);
        if (_picked) Ui.Dim(sb, 0.7f);
    }
}

/// <summary>Figma Scene 18: all pets down -> Game Over -> back to Scene 2.</summary>
public class GameOverScene : Scene
{
    readonly string _reason;
    float _time;

    public GameOverScene(string reason) => _reason = reason;

    public override void Update(float dt)
    {
        _time += dt;
        if (_time > 1f && (Input.Confirm || Input.Click)) M.Reset(new ChooseStarterScene());
    }

    public override void Draw(SpriteBatch sb)
    {
        Gfx.Rect(sb, 0, 0, Gfx.W, Gfx.H, new Color(16, 6, 10));
        Gfx.Text(sb, Gfx.Big, "GAME OVER", new Vector2(Gfx.W / 2f, 260), Palette.Danger, 0.5f);
        Gfx.Text(sb, Gfx.Font, _reason, new Vector2(Gfx.W / 2f, 340), Palette.Text, 0.5f);
        if (_time > 1f) Gfx.Text(sb, Gfx.Small, "Space  Choose a new pet", new Vector2(Gfx.W / 2f, 440), Palette.Dim, 0.5f);
    }
}

/// <summary>End of the vertical slice (Day 5, after Big Z).</summary>
public class ToBeContinuedScene : Scene
{
    readonly int _days;
    float _time;

    public ToBeContinuedScene(int days) => _days = days;

    public override void Update(float dt)
    {
        _time += dt;
        if (_time > 2f && (Input.Confirm || Input.Click)) M.Reset(new MainMenuScene());
    }

    public override void Draw(SpriteBatch sb)
    {
        Gfx.Rect(sb, 0, 0, Gfx.W, Gfx.H, Color.Black);
        float a = MathHelper.Clamp(_time / 1.5f, 0, 1);
        Gfx.Text(sb, Gfx.Font, "The sanctuary fell silent.", new Vector2(Gfx.W / 2f, 230), Palette.Text * a, 0.5f);
        Gfx.Text(sb, Gfx.Big, "To be continued...", new Vector2(Gfx.W / 2f, 290), Palette.Warm * a, 0.5f);
        Gfx.Text(sb, Gfx.Small, $"You kept the sanctuary alive for {_days} days.", new Vector2(Gfx.W / 2f, 380), Palette.Dim * a, 0.5f);
        if (_time > 2f) Gfx.Text(sb, Gfx.Small, "Space  Main Menu", new Vector2(Gfx.W / 2f, 470), Palette.Dim, 0.5f);
    }
}

/// <summary>Sky darkens, the day is processed, morning comes back with a report.</summary>
public class NightScene : Scene
{
    readonly BaseScene _base;
    float _t;
    bool _advanced;
    System.Collections.Generic.List<string> _report = new();

    public override bool Overlay => true;

    public NightScene(BaseScene b) => _base = b;

    public override void Update(float dt)
    {
        _t += dt;
        if (_t < 1.5f) _base.Night = _t / 1.5f;
        else if (!_advanced)
        {
            _advanced = true;
            _report = _base.State.AdvanceDay();
        }
        else if (_t > 3.0f && _t < 4.5f) _base.Night = 1 - (_t - 3f) / 1.5f;
        else if (_t >= 4.5f)
        {
            _base.Night = 0;
            M.Remove(this);
            var gs = _base.State;
            if (!gs.Alive.Any() && gs.Coin < Balance.ReviveCost)
                M.Push(DialogueScene.Say("", () => M.Reset(new GameOverScene("No pets left, and no coin to revive them.")), _report.ToArray()));
            else M.Push(DialogueScene.Say("Morning", null, _report.ToArray()));
        }
    }

    public override void Draw(SpriteBatch sb)
    {
        float a = _t < 1.5f ? _t / 1.5f : _t < 3f ? 1 : MathHelper.Clamp(1 - (_t - 3f) / 1.5f, 0, 1);
        Ui.Dim(sb, a * 0.55f);
        if (_t > 0.8f && _t < 3.6f)
            Gfx.Text(sb, Gfx.Big, _advanced ? $"Day {_base.State.Day}" : $"Day {_base.State.Day} is over...",
                new Vector2(Gfx.W / 2f, 280), Palette.Text * MathHelper.Clamp(a * 1.5f, 0, 1), 0.5f);
    }
}
