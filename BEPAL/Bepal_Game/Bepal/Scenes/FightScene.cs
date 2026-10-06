using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bepal;

/// <summary>
/// Figma Scene 18. Dodge zone = Perfect only and keeps shrinking; when it vanishes (or you press on nothing)
/// your pet gets hit. Attack zone = Perfect 100% / Great 75% of ATK. A fallen pet is swapped out and
/// the enemy keeps its damage. All pets down = defeat.
/// </summary>
public class FightScene : Scene
{
    readonly GameState _gs;
    readonly Enemy _enemy;
    readonly Action<bool> _onEnd;
    Pet _pet;
    public Wheel Wheel => _wheel;
    readonly Wheel _wheel = new(new Vector2(Gfx.W / 2f, 440), 150);
    readonly Popups _popups = new();
    Zone _dodge = null!;
    Zone _attack = null!;
    readonly float _dodgeStart;
    float _time;
    float _enemyFlash, _petFlash, _lunge;
    float _end = -1;
    bool _won;
    bool _waitingForSwap;
    bool _poisoned;
    int _combo;   // Nibbleclaw: consecutive Attack hits

    public FightScene(GameState gs, Enemy enemy, Pet pet, Action<bool> onEnd)
    {
        _gs = gs;
        _enemy = enemy;
        _pet = pet;
        _onEnd = onEnd;
        _wheel.Speed = enemy.NeedleSpeed;
        _dodgeStart = gs.SeaTea ? 0.38f : 0.32f;   // Sea Tea: Dodge zone +20% for this fight
        gs.SeaTea = false;
        if (enemy.IsBoss) Audio.Play(Sfx.FightBossRoar);
        _attack = new Zone { Perfect = 0.08f, Great = 0.19f, Color = Palette.Heal, Label = "Attack" };
        _wheel.Zones.Add(_attack);
        _attack.Center = _wheel.FreeAngle(1.0f);
        NewDodge();
    }

    void NewDodge()
    {
        if (_dodge != null) _wheel.Zones.Remove(_dodge);
        _dodge = new Zone { Perfect = _dodgeStart, Great = 0, Color = Palette.Feed, Label = "Dodge" };
        _dodge.Center = _wheel.FreeAngle(1.2f);
        _wheel.Zones.Add(_dodge);
    }

    void MoveAttack()
    {
        _wheel.Zones.Remove(_attack);
        _attack.Center = _wheel.FreeAngle(0.8f);
        _wheel.Zones.Add(_attack);
    }

    void EnemyHits()
    {
        int dmg = (int)MathF.Round(_enemy.Atk * (_poisoned ? Balance.PoisonDealtMul : 1f));
        _pet.Hp -= dmg;
        _pet.ClampStats();
        _petFlash = 1;
        _lunge = 1;
        Audio.Play(Sfx.FightPetHurt);
        Gfx.Shake(14, 0.35f);
        _popups.Add($"{_enemy.AttackName}! -{dmg}", Palette.Danger, new Vector2(300, 200));
        NewDodge();
        if (_pet.Dead) PetFell();
    }

    void PetFell()
    {
        var alive = _gs.Alive.ToList();
        if (alive.Count == 0)
        {
            _end = 1.5f;
            _won = false;
            return;
        }
        _waitingForSwap = true;
        M.Push(new PetPickScene($"{_pet.Name} has fallen! Send another pet", alive, p =>
        {
            _pet = p;
            _combo = 0;
            _waitingForSwap = false;
            NewDodge();
        }));
    }

    public override void Update(float dt)
    {
        _time += dt;
        _popups.Update(dt);
        _enemyFlash = MathF.Max(0, _enemyFlash - dt * 4);
        _petFlash = MathF.Max(0, _petFlash - dt * 4);
        _lunge = MathF.Max(0, _lunge - dt * 3);

        if (_end >= 0)
        {
            _end -= dt;
            if (_end < 0)
            {
                M.Remove(this);
                _onEnd(_won);
            }
            return;
        }
        if (_waitingForSwap) return;

        _wheel.Update(dt);
        _dodge.Perfect -= _enemy.DodgeShrink * dt;
        if (_dodge.Perfect <= 0.012f)
        {
            _popups.Add("Too slow!", Palette.Danger, new Vector2(Gfx.W / 2f, 250));
            EnemyHits();
            return;
        }

        if (!Input.Confirm) return;
        var (hit, zone) = _wheel.Evaluate();
        if (zone == _dodge)
        {
            Audio.Play(Sfx.FightDodge);
            _popups.Add("Dodge!", Palette.Feed, new Vector2(300, 200));
            NewDodge();
        }
        else if (zone == _attack)
        {
            float dmg = _pet.Atk * (hit == Hit.Perfect ? 1f : 0.75f) * (_poisoned ? Balance.PoisonTakenMul : 1f);
            if (_pet.Has(Species.Nibbleclaw))
            {
                dmg *= 1 + _combo * Balance.NibbleComboStep;
                _combo++;
                if (_combo > 1) _popups.Add($"Combo x{_combo}", Palette.Coin, new Vector2(300, 260));
            }
            _enemy.Hp = MathF.Max(0, _enemy.Hp - dmg);
            _enemyFlash = 1;
            Audio.Play(Sfx.FightPlayerAttack);
            Gfx.Shake(8, 0.2f);
            _popups.Add(hit, new Vector2(Gfx.W / 2f, 250));
            _popups.Add($"-{(int)dmg}", Palette.Text, new Vector2(1000, 150));
            if (_pet.Has(Species.Blinkbun)) _wheel.Needle = 0;
            MoveAttack();
            if (_pet.Has(Species.Mossling))
            {
                int heal = Math.Max(1, (int)MathF.Round(_pet.MaxHp * Balance.MosslingHealPct));
                _pet.Hp += heal;
                _pet.ClampStats();
                _popups.Add($"+{heal}", Palette.Heal, new Vector2(300, 260));
            }
            if (_pet.Has(Species.Toothless) && !_poisoned)
            {
                _poisoned = true;
                _popups.Add("Poisoned!", new Color(170, 90, 220), new Vector2(1000, 220));
            }
            if (_enemy.Hp <= 0)
            {
                _won = true;
                _end = 1.5f;
            }
        }
        else
        {
            _popups.Add("Miss", Palette.Danger, new Vector2(Gfx.W / 2f, 250));
            _combo = 0;
            EnemyHits();
        }
    }

    public override void Draw(SpriteBatch sb)
    {
        // Arena: the base at night, seen through the open red door
        var bd = BaseScene.SharedBackdrop;
        bd.Draw(sb, 3000, 0.85f, _time, 560);
        Gfx.Rect(sb, 0, 560, Gfx.W, 160, new Color(30, 22, 24));
        Gfx.Rect(sb, 0, 560, Gfx.W, 4, new Color(80, 50, 44));
        Gfx.Rect(sb, 0, 0, Gfx.W, Gfx.H, new Color(60, 0, 10) * 0.18f);

        var petFeet = new Vector2(250 + _lunge * 30, 560);
        var enemyFeet = new Vector2(1010 - _lunge * 60, 560);
        Art.Pet(sb, _pet.Species, Color.Lerp(_pet.Color, Color.Red, _petFlash), petFeet, 1.4f, _time, enemyFeet - new Vector2(0, 80),
            1f, _pet.Dead);
        Art.Enemy(sb, _enemy, enemyFeet, _time, petFeet, MathF.Max(_enemyFlash, _poisoned ? 0.25f : 0));

        // Pet HP (small bar over its head) and enemy HP (top right), per Figma
        Ui.Bar(sb, new Rectangle((int)petFeet.X - 70, 380, 140, 16), _pet.Hp, _pet.MaxHp, Palette.Hp);
        Gfx.Text(sb, Gfx.Small, $"{_pet.Name}  {_pet.HpShown}/{_pet.MaxHp}  ATK {_pet.Atk}", new Vector2(petFeet.X, 354), Palette.Text, 0.5f);
        Gfx.Text(sb, Gfx.Font, _enemy.Name, new Vector2(Gfx.W - 40, 24), Palette.Danger, 1f);
        Ui.Bar(sb, new Rectangle(Gfx.W - 440, 60, 400, 24), _enemy.Hp, _enemy.MaxHp, Palette.Hp,
            $"Health {(int)_enemy.Hp}/{(int)_enemy.MaxHp}");

        _wheel.Draw(sb, 26);
        Gfx.Text(sb, Gfx.Small, "Hit Dodge before it shrinks away. If you're too slow, your pet gets hit!",
            new Vector2(Gfx.W / 2f, 250), Palette.Dim, 0.5f);
        _popups.Draw(sb);

        if (_end >= 0)
        {
            Ui.Dim(sb, 0.4f);
            Gfx.Text(sb, Gfx.Big, _won ? "VICTORY" : "DEFEATED", new Vector2(Gfx.W / 2f, 300), _won ? Palette.Coin : Palette.Danger, 0.5f);
        }
        Ui.Hint(sb, "SPACE  on Dodge (yellow) to evade   |   SPACE  on Attack (green) to strike");
    }
}
