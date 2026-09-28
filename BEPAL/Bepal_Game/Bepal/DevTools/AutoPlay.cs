using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Input;

namespace Bepal;

/// <summary>
/// Dev tool: <c>--autoplay [--refuse]</c> drives the whole slice (menu -> Day 5 -> To be continued) with injected
/// input and writes <c>%TEMP%/bepal_autoplay.log</c>, to catch soft-locks and crashes. Picks option 1 in every prompt
/// (Tame, Sell) unless <c>--refuse</c> is given, which refuses the merchant and visits the shop instead.
/// </summary>
public class AutoPlay
{
    readonly SceneManager _m;
    readonly bool _refuse;
    int _frame;
    int _cooldown;
    string _lastTop = "";
    int _day;
    readonly HashSet<int> _doctorVisited = new();

    public readonly List<string> Log = new();
    public bool Finished;
    public bool Failed;

    public AutoPlay(SceneManager m, bool refuseMerchant)
    {
        _m = m;
        _refuse = refuseMerchant;
    }

    public void Step()
    {
        _frame++;
        var top = _m.Top;
        var name = top?.GetType().Name ?? "none";
        if (name != _lastTop)
        {
            string extra = top is BaseScene b
                ? $" day={b.State.Day} coin={b.State.Coin} energy={b.State.Energy} lv={b.State.PlayerLevel} pts={b.State.Points} pets=" +
                  string.Join(", ", b.State.Pets.Select(p => $"{p.Name}(hp{p.HpShown}/{p.MaxHp} s{(int)p.Stomach} c{(int)p.Clean} L{p.Level})"))
                : "";
            Log.Add($"[{_frame / 60f,7:F1}s] {name}{extra}");
            _lastTop = name;
        }
        if (top is ToBeContinuedScene && !Finished)
        {
            Log.Add("RESULT: reached To be continued");
            Finished = true;
        }
        if (top is GameOverScene && !Finished)
        {
            Log.Add("RESULT: game over");
            Finished = true;
        }
        if (_frame > 60 * 60 * 30 && !Finished)
        {
            Log.Add($"RESULT: TIMEOUT (soft lock?) top={name}");
            Failed = true;
            Finished = true;
        }

        var keys = new List<Keys>();
        if (_cooldown > 0)
        {
            _cooldown--;
            Input.Inject(Array.Empty<Keys>());
            return;
        }

        switch (top)
        {
            case MainMenuScene or DialogueScene or ToBeContinuedScene or GameOverScene:
                keys.Add(Keys.Space);
                _cooldown = 3;
                break;
            case ChoiceScene c:
                bool second = c.Prompt.StartsWith("Toothless is at") || (_refuse && c.Prompt.StartsWith("Sell"));
                keys.Add(second ? Keys.D2 : Keys.D1);
                _cooldown = 3;
                break;
            case PetPickScene:
                keys.AddRange(new[] { Keys.D1, Keys.D2, Keys.D3, Keys.D4 });
                _cooldown = 3;
                break;
            case ShopScene:
                keys.Add(Keys.Escape);
                _cooldown = 3;
                break;
            case DoctorScene:
                keys.Add(Keys.Escape);
                _cooldown = 3;
                break;
            case CareSelectScene cs when cs.Wheel.Evaluate().hit != Hit.Miss:
                keys.Add(Keys.Space);
                _cooldown = 3;
                break;
            case QteScene q when q.Wheel.Evaluate().hit == Hit.Perfect:
            case FightScene f when f.Wheel.Evaluate().hit != Hit.Miss:
                keys.Add(Keys.Space);
                _cooldown = 2;
                break;
            case BaseScene b:
                BaseBrain(b, keys);
                break;
        }
        Input.Inject(keys.ToArray());
    }

    void BaseBrain(BaseScene b, List<Keys> keys)
    {
        var gs = b.State;
        _day = gs.Day;
        var pet = gs.Alive.FirstOrDefault();
        float target;
        BaseScene.Kind want;
        if (gs.DoorEventPending) (target, want) = (BaseScene.DoorX, BaseScene.Kind.Door);
        else if (gs.Energy > 0 && pet != null) (target, want) = (b.PetX(pet), BaseScene.Kind.Pet);
        else if (gs.Pets.Any(p => p.Dead) && !_doctorVisited.Contains(gs.Day))
        {
            (target, want) = (BaseScene.DoctorX, BaseScene.Kind.Doctor);
            if (b.Near?.Kind == want) _doctorVisited.Add(gs.Day);
        }
        else (target, want) = (BaseScene.BedX, BaseScene.Kind.Bed);

        if (b.Near is { } near && near.Kind == want && (want != BaseScene.Kind.Pet || near.Pet == pet))
        {
            keys.Add(Keys.Space);
            _cooldown = 4;
        }
        else keys.Add(target > b.PlayerX ? Keys.D : Keys.A);
    }
}
