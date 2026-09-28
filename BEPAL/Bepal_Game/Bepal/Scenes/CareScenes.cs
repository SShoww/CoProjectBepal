using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bepal;

public enum Care { Train, Feed, Clean, Heal }

/// <summary>Figma Scene 4 — needle spins over 4 coloured choices; Space on a choice opens that QTE (costs 1 Energy).</summary>
public class CareSelectScene : Scene
{
    readonly GameState _gs;
    readonly Pet _pet;
    public Wheel Wheel => _wheel;
    readonly Wheel _wheel = new(new Vector2(Gfx.W / 2f, 330), 170) { Speed = 2.2f };
    readonly Popups _popups = new();
    float _time;
    float _openDelay = -1;
    Care _chosen;

    public override bool Overlay => true;

    public CareSelectScene(GameState gs, Pet pet)
    {
        _gs = gs;
        _pet = pet;
        (Care care, Color col)[] choices =
        {
            (Care.Train, Palette.Train), (Care.Feed, Palette.Feed), (Care.Clean, Palette.Clean), (Care.Heal, Palette.Heal),
        };
        for (int i = 0; i < 4; i++)
            _wheel.Zones.Add(new Zone
            {
                Center = i * MathHelper.PiOver2, Perfect = 0.62f, Great = 0, Color = choices[i].col, Label = choices[i].care.ToString(),
            });
    }

    public override void Update(float dt)
    {
        _time += dt;
        _popups.Update(dt);
        if (_openDelay >= 0)
        {
            _openDelay -= dt;
            if (_openDelay < 0)
            {
                M.Remove(this);
                M.Push(new QteScene(_gs, _pet, _chosen));
            }
            return;
        }

        _wheel.Update(dt);
        if (Input.Back)
        {
            M.Remove(this);
            return;
        }
        if (!Input.Confirm) return;

        if (_gs.Energy <= 0)
        {
            _popups.Add("No energy left!", Palette.Danger, new Vector2(Gfx.W / 2f, 120));
            return;
        }
        var (hit, zone) = _wheel.Evaluate();
        if (hit == Hit.Miss || zone == null) return;   // Figma: pressing a gap does nothing

        _chosen = Enum.Parse<Care>(zone.Label!);
        _gs.Energy--;
        Gfx.Shake(10, 0.3f);
        _popups.Add(zone.Label!, zone.Color, new Vector2(Gfx.W / 2f, 120));
        _openDelay = 0.4f;
    }

    public override void Draw(SpriteBatch sb)
    {
        Ui.Dim(sb, 0.65f);
        Gfx.Text(sb, Gfx.Font, $"Care for {_pet.Name}", new Vector2(Gfx.W / 2f, 40), Palette.Warm, 0.5f);
        Art.Pet(sb, _pet.Species, _pet.Color, new Vector2(_wheel.Center.X, _wheel.Center.Y + 115), 0.8f, _time,
            Gfx.OnWheel(_wheel.Center, 200, _wheel.Needle));
        _wheel.Draw(sb);
        PetStats(sb, _pet, new Rectangle(40, 200, 250, 210));
        Gfx.Text(sb, Gfx.Font, $"Energy {_gs.Energy}/{_gs.MaxEnergy}", new Vector2(Gfx.W - 60, 210), Palette.Warm, 1f);
        Gfx.Text(sb, Gfx.Small, "Each care costs 1 Energy", new Vector2(Gfx.W - 60, 244), Palette.Dim, 1f);
        _popups.Draw(sb);
        Ui.Hint(sb, "SPACE  Pick when the needle is on a choice   |   ESC  Back");
    }

    public static void PetStats(SpriteBatch sb, Pet p, Rectangle r)
    {
        Ui.Panel(sb, r, 0.9f);
        Gfx.Text(sb, Gfx.Font, $"{p.Name}  LV {p.Level}", new Vector2(r.X + 14, r.Y + 10), Palette.Text);
        Gfx.Text(sb, Gfx.Small, $"ATK {p.Atk}   {p.Element}", new Vector2(r.X + 14, r.Y + 40), Palette.Dim);
        int y = r.Y + 68, w = r.Width - 28;
        Ui.Bar(sb, new Rectangle(r.X + 14, y, w, 22), p.Hp, p.MaxHp, Palette.Hp, $"HP {p.HpShown}/{p.MaxHp}");
        Ui.Bar(sb, new Rectangle(r.X + 14, y + 28, w, 22), p.Stomach, 100, Palette.Stomach, $"Stomach {(int)p.Stomach}");
        Ui.Bar(sb, new Rectangle(r.X + 14, y + 56, w, 22), p.Clean, 100, Palette.CleanBar, $"Clean {(int)p.Clean}");
        Ui.Bar(sb, new Rectangle(r.X + 14, y + 84, w, 22), p.Progress, p.MaxProgress, Palette.Progress,
            $"Progress {(int)p.Progress}/{p.MaxProgress}");
    }
}

/// <summary>Figma Scene 5-8 — 10 Spacebar presses, then back to the base.</summary>
public class QteScene : Scene
{
    readonly GameState _gs;
    readonly Pet _pet;
    readonly Care _care;
    public Wheel Wheel => _wheel;
    readonly Wheel _wheel = new(new Vector2(Gfx.W / 2f, 340), 180);
    readonly Popups _popups = new();
    int _attempts = Balance.Attempts;
    float _time;
    float _finish = -1;
    int _perfects, _greats, _levelUps;
    bool _playerLevelUp;
    const float BaseSpeed = 2.4f;
    const float TrainSpeed = 3.6f;
    const float CleanZoneSpeed = 0.6f;   // Clean: fixed run-away speed, always below the needle's BaseSpeed
    const float HealFadeRate = 1.5f;     // Heal: needle fade cycle (lower = slower, easier to read)
    const float HealMinAlpha = 0.3f;     // Heal: needle never fades below this
    const float HealShrink = 0.5f;       // Heal: zone shrink rate multiplier (lower = dot lives longer)

    public override bool Overlay => true;

    Color Col => _care switch
    {
        Care.Train => Palette.Train,
        Care.Feed => Palette.Feed,
        Care.Clean => Palette.Clean,
        _ => Palette.Heal,
    };

    public QteScene(GameState gs, Pet pet, Care care)
    {
        _gs = gs;
        _pet = pet;
        _care = care;
        _wheel.Speed = care == Care.Train ? TrainSpeed : BaseSpeed;

        // Metabolic burn (GDD 03 §2.2): Train -10 Stomach, Clean/Heal -5.
        if (care == Care.Train) _pet.Stomach -= 10;
        if (care is Care.Clean or Care.Heal) _pet.Stomach -= 5;
        _pet.ClampStats();

        if (care == Care.Train)
            for (int i = 0; i < 3; i++) SpawnZone();
        else SpawnZone();
    }

    /// <summary>Train dots shrink as the pet levels (stops at LV 3); the QTE upgrade widens them.</summary>
    float TrainScale => _gs.QteZoneMultiplier / (1f + 0.15f * (Math.Min(_pet.Level, 3) - 1));

    void SpawnZone(Zone? replace = null)
    {
        if (replace != null) _wheel.Zones.Remove(replace);
        var z = _care == Care.Train
            ? new Zone { Perfect = 0.07f * TrainScale, Great = 0.16f * TrainScale }
            : new Zone { Perfect = 0.12f, Great = 0.27f };
        z.Color = Col;
        z.Center = _wheel.FreeAngle(_care == Care.Train ? 0.4f : 0.9f);
        _wheel.Zones.Add(z);
    }

    public override void Update(float dt)
    {
        _time += dt;
        _popups.Update(dt);
        _wheel.Update(dt);

        switch (_care)
        {
            case Care.Clean:   // zone runs away in the needle's direction, slower than the needle
                foreach (var z in _wheel.Zones) z.Center += _wheel.Dir * CleanZoneSpeed * dt;
                break;
            case Care.Heal:    // needle fades in and out (never fully gone); zone slowly shrinks, respawns when gone
                float f = MathF.Cos(_time * HealFadeRate);
                _wheel.NeedleAlpha = MathHelper.Clamp(0.6f + f * 0.7f, HealMinAlpha, 1);
                foreach (var z in _wheel.Zones.ToArray())
                {
                    z.Perfect -= 0.035f * HealShrink * dt;
                    z.Great -= 0.07f * HealShrink * dt;
                    if (z.Perfect <= 0.02f) SpawnZone(z);
                }
                break;
        }

        if (_finish >= 0)
        {
            _finish -= dt;
            if (_finish < 0) Close();
            return;
        }
        if (!Input.Confirm || _attempts <= 0) return;

        _attempts--;
        var (hit, zone) = _wheel.Evaluate();
        _popups.Add(hit, new Vector2(Gfx.W / 2f, 150));
        Apply(hit, zone);
        if (_attempts == 0) _finish = 0.9f;
    }

    void Apply(Hit hit, Zone? zone)
    {
        if (hit == Hit.Miss)
        {
            if (_care == Care.Feed) _wheel.Speed = BaseSpeed;
            return;
        }

        bool perfect = hit == Hit.Perfect;
        if (perfect) _perfects++; else _greats++;
        if (_gs.AddExp(1)) _playerLevelUp = true;   // player EXP: every successful press

        switch (_care)
        {
            case Care.Train:
                _levelUps += _pet.AddProgress((perfect ? 10 : 5) * _gs.TrainGainMultiplier);
                break;
            case Care.Feed:
                _pet.Stomach += perfect ? 6 : 3;
                _wheel.Dir = -_wheel.Dir;                                   // hit flips the spin
                if (perfect) _wheel.Speed = MathF.Min(_wheel.Speed * 1.15f, 6f);
                break;
            case Care.Clean:
                _pet.Clean += perfect ? 6 : 3;
                break;
            case Care.Heal:
                _pet.Hp += perfect ? 6 : 3;
                break;
        }
        _pet.ClampStats();
        Gfx.Shake(perfect ? 6 : 3, 0.15f);
        SpawnZone(zone);
    }

    void Close()
    {
        M.Remove(this);
        var lines = new System.Collections.Generic.List<string>
        {
            $"{_care} finished: {_perfects} Perfect, {_greats} Great, {Balance.Attempts - _perfects - _greats} Miss.",
        };
        if (_levelUps > 0) lines.Add($"{_pet.Name} grew to LV {_pet.Level}! Stronger and tougher.");
        if (_pet.Level >= 3 && _levelUps > 0 && _pet.Level - _levelUps < 3)
            lines.Add($"{_pet.Name} learned a new move! (coming soon)");
        if (_playerLevelUp) lines.Add($"You reached Player LV {_gs.PlayerLevel}! +1 Point for upgrades.");
        M.Push(DialogueScene.Say("", null, lines.ToArray()));
    }

    public override void Draw(SpriteBatch sb)
    {
        Ui.Dim(sb, 0.72f);
        Gfx.Text(sb, Gfx.Big, _care.ToString().ToUpper(), new Vector2(Gfx.W / 2f, 24), Col, 0.5f);
        Art.Pet(sb, _pet.Species, _pet.Color, new Vector2(_wheel.Center.X, _wheel.Center.Y + 115), 0.8f, _time,
            Gfx.OnWheel(_wheel.Center, 200, _wheel.Needle));
        _wheel.Draw(sb);
        CareSelectScene.PetStats(sb, _pet, new Rectangle(40, 200, 250, 210));

        var ar = new Rectangle(Gfx.W - 290, 200, 250, 120);
        Ui.Panel(sb, ar, 0.9f);
        Gfx.Text(sb, Gfx.Font, $"Attempt : {_attempts}", new Vector2(ar.X + 16, ar.Y + 12), Palette.Text);
        for (int i = 0; i < Balance.Attempts; i++)
            Gfx.Rect(sb, ar.X + 16 + i * 22, ar.Y + 52, 16, 16, i < _attempts ? Col : new Color(60, 50, 56));
        Gfx.Text(sb, Gfx.Small, $"Player LV {_gs.PlayerLevel}  EXP {_gs.PlayerExp}/{_gs.PlayerMaxExp}",
            new Vector2(ar.X + 16, ar.Y + 84), Palette.Dim);

        string tip = _care switch
        {
            Care.Train => "Small dots, fast needle.",
            Care.Feed => "Each hit reverses the spin. Perfect speeds it up.",
            Care.Clean => "The dot runs away from the needle!",
            _ => "The needle fades... trust your rhythm.",
        };
        Gfx.Text(sb, Gfx.Small, tip, new Vector2(Gfx.W / 2f, 600), Palette.Dim, 0.5f);
        _popups.Draw(sb);
        Ui.Hint(sb, "SPACE  Hit when the needle is on a dot (Perfect = centre)");
    }
}
