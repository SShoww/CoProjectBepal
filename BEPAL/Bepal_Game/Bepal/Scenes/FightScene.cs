using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bepal;

/// <summary>
/// Figma Scene 18. Two alternating phases on the wheel: Attack (Perfect 100% / Great 75% of ATK) first; a landed
/// Attack that doesn't kill swaps it for a Dodge (Perfect only, keeps shrinking) — when it vanishes (or you press
/// on nothing) your pet gets hit. A missed Attack also gets your pet hit. A fallen pet is swapped out and
/// the enemy keeps its damage. All pets down = defeat.
/// </summary>
public class FightScene : Scene
{
    readonly GameState _gs;
    readonly Enemy _enemy;
    readonly Action<bool> _onEnd;
    Pet _pet;
    public Wheel Wheel => _wheel;
    readonly Wheel _wheel = new(new Vector2(Gfx.W / 2f, Balance.FightWheelY), Balance.FightWheelRadius);
    readonly Popups _popups = new();
    static Vector2 CenterPopup => new(Gfx.W / 2f, Balance.FightPopupCenterY);
    Zone? _dodge;   // non-null only during the Dodge phase
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
        _dodgeStart = gs.SeaTea ? Balance.DodgeStartSeaTea : Balance.DodgeStart;   // Sea Tea: Dodge zone +20% for this fight
        gs.SeaTea = false;
        if (enemy.IsBoss) Audio.Play(Sfx.FightBossRoar);
        _attack = new Zone { Color = Palette.Heal, Label = "Attack" };
        StartAttack();
    }

    void StartDodge()
    {
        _wheel.Zones.Remove(_attack);
        _dodge = new Zone { Perfect = _dodgeStart, Great = 0, Color = Palette.Feed, Label = "Dodge" };
        _dodge.Center = _wheel.FreeAngle(Balance.DodgeSpawnGap);
        _wheel.Zones.Add(_dodge);
    }

    void StartAttack()
    {
        if (_dodge != null) _wheel.Zones.Remove(_dodge);
        _dodge = null;
        _wheel.Zones.Remove(_attack);
        _attack.Perfect = _dodgeStart - Balance.AttackGreatBand;
        _attack.Great = _dodgeStart;
        _attack.Center = _wheel.FreeAngle(Balance.AttackSpawnGap);
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
        Gfx.Shake(Balance.FightHurtShake, Balance.FightHurtShakeTime);
        _popups.Add($"{_enemy.AttackName}! -{dmg}", Palette.Danger, Balance.FightPopupPet);
        StartAttack();
        if (_pet.Dead) PetFell();
    }

    void PetFell()
    {
        var alive = _gs.Alive.ToList();
        if (alive.Count == 0)
        {
            _end = Balance.FightEndDelay;
            _won = false;
            return;
        }
        _waitingForSwap = true;
        M.Push(new PetPickScene($"{_pet.Name} has fallen! Send another pet", alive, p =>
        {
            _pet = p;
            _combo = 0;
            _waitingForSwap = false;
            StartAttack();
        }));
    }

    public override void Update(float dt)
    {
        _time += dt;
        _popups.Update(dt);
        _enemyFlash = MathF.Max(0, _enemyFlash - dt * Balance.FightFlashFade);
        _petFlash = MathF.Max(0, _petFlash - dt * Balance.FightFlashFade);
        _lunge = MathF.Max(0, _lunge - dt * Balance.FightLungeFade);

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
        if (_dodge == null)
        {
            _attack.Perfect = MathF.Max(Balance.AttackMinPerfect, _attack.Perfect - _enemy.DodgeShrink * Balance.AttackShrinkMul * dt);
            _attack.Great = _attack.Perfect + Balance.AttackGreatBand;
        }
        else
        {
            _dodge.Perfect -= _enemy.DodgeShrink * dt;
            if (_dodge.Perfect <= Balance.DodgeVanish)
            {
                _popups.Add("Too slow!", Palette.Danger, CenterPopup);
                EnemyHits();
                return;
            }
        }

        if (!Input.Confirm) return;
        var (hit, zone) = _wheel.Evaluate();
        if (zone != null && zone == _dodge)
        {
            Audio.Play(Sfx.FightDodge);
            _popups.Add("Dodge!", Palette.Feed, Balance.FightPopupPet);
            StartAttack();
        }
        else if (zone == _attack)
        {
            float dmg = _pet.Atk * (hit == Hit.Perfect ? 1f : Balance.AttackGreatDmgMul) * (_poisoned ? Balance.PoisonTakenMul : 1f);
            if (_pet.Has(Species.Nibbleclaw))
            {
                dmg *= 1 + _combo * Balance.NibbleComboStep;
                _combo++;
                if (_combo > 1) _popups.Add($"Combo x{_combo}", Palette.Coin, Balance.FightPopupPetSub);
            }
            _enemy.Hp = MathF.Max(0, _enemy.Hp - dmg);
            _enemyFlash = 1;
            Audio.Play(Sfx.FightPlayerAttack);
            Gfx.Shake(Balance.FightHitShake, Balance.FightHitShakeTime);
            _popups.Add(hit, CenterPopup);
            _popups.Add($"-{(int)dmg}", Palette.Text, Balance.FightPopupEnemyDmg);
            if (_pet.Has(Species.Blinkbun)) _wheel.Needle = 0;
            if (_pet.Has(Species.Mossling))
            {
                int heal = Math.Max(1, (int)MathF.Round(_pet.MaxHp * Balance.MosslingHealPct));
                _pet.Hp += heal;
                _pet.ClampStats();
                _popups.Add($"+{heal}", Palette.Heal, Balance.FightPopupPetSub);
            }
            if (_pet.Has(Species.Toothless) && !_poisoned)
            {
                _poisoned = true;
                _popups.Add("Poisoned!", Balance.FightPoisonColor, Balance.FightPopupEnemyStatus);
            }
            if (_enemy.Hp <= 0)
            {
                _won = true;
                _end = Balance.FightEndDelay;
            }
            else StartDodge();
        }
        else
        {
            _popups.Add("Miss", Palette.Danger, CenterPopup);
            _combo = 0;
            EnemyHits();
        }
    }

    public override void Draw(SpriteBatch sb)
    {
        // Arena: the base at night, seen through the open red door
        var bd = BaseScene.SharedBackdrop;
        bd.Draw(sb, Balance.FightBackdropX, Balance.FightBackdropLight, _time, Balance.FightFloorY);
        Gfx.Rect(sb, 0, Balance.FightFloorY, Gfx.W, Gfx.H - Balance.FightFloorY, Balance.FightFloor);
        Gfx.Rect(sb, 0, Balance.FightFloorY, Gfx.W, 4, Balance.FightFloorEdge);
        Gfx.Rect(sb, 0, 0, Gfx.W, Gfx.H, Balance.FightRedTint * Balance.FightRedTintAlpha);

        var petFeet = new Vector2(Balance.FightPetX + _lunge * Balance.FightPetLunge, Balance.FightFloorY);
        var enemyFeet = new Vector2(Balance.FightEnemyX - _lunge * Balance.FightEnemyLunge, Balance.FightFloorY);
        Art.Pet(sb, _pet.Species, Color.Lerp(_pet.Color, Color.Red, _petFlash), petFeet, Balance.FightPetScale, _time, enemyFeet - new Vector2(0, 80),
            1f, _pet.Dead);
        Art.Enemy(sb, _enemy, enemyFeet, _time, petFeet, MathF.Max(_enemyFlash, _poisoned ? Balance.FightPoisonTint : 0));

        // Pet HP (small bar over its head) and enemy HP (top right), per Figma
        Ui.Bar(sb, new Rectangle((int)petFeet.X - Balance.FightPetBarW / 2, Balance.FightPetBarY, Balance.FightPetBarW, Balance.FightPetBarH), _pet.Hp, _pet.MaxHp, Palette.Hp);
        Gfx.Text(sb, Gfx.Small, $"{_pet.Name}  {_pet.HpShown}/{_pet.MaxHp}  ATK {_pet.Atk}", new Vector2(petFeet.X, Balance.FightPetLabelY), Palette.Text, 0.5f);
        Gfx.Text(sb, Gfx.Font, _enemy.Name, new Vector2(Gfx.W - Balance.FightEnemyMargin, Balance.FightEnemyNameY), Palette.Danger, 1f);
        Ui.Bar(sb, new Rectangle(Gfx.W - Balance.FightEnemyBarW - Balance.FightEnemyMargin, Balance.FightEnemyBarY, Balance.FightEnemyBarW, Balance.FightEnemyBarH), _enemy.Hp, _enemy.MaxHp, Palette.Hp,
            $"Health {(int)_enemy.Hp}/{(int)_enemy.MaxHp}");

        _wheel.Draw(sb, Balance.FightWheelThickness);
        Gfx.Text(sb, Gfx.Small, _dodge != null
                ? "Hit Dodge before it shrinks away. If you're too slow, your pet gets hit!"
                : "Land an Attack to strike, then get ready to Dodge!",
            new Vector2(Gfx.W / 2f, Balance.FightTipY), Palette.Dim, 0.5f);
        _popups.Draw(sb);

        if (_end >= 0)
        {
            Ui.Dim(sb, Balance.FightEndDim);
            Gfx.Text(sb, Gfx.Big, _won ? "VICTORY" : "DEFEATED", new Vector2(Gfx.W / 2f, Balance.FightEndTextY), _won ? Palette.Coin : Palette.Danger, 0.5f);
        }
        Ui.Hint(sb, _dodge != null ? "SPACE  on Dodge (yellow) to evade" : "SPACE  on Attack (green) to strike");
    }
}
