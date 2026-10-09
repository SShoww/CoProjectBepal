using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Bepal;

/// <summary>Tuning numbers — see BEPAL/Docs/GDD/06-vertical-slice.md.</summary>
public static class Balance
{
    public const int LastDay = 5;
    /// <summary>Seconds between door knocks while a door event is pending (clip is 3 s + pause).</summary>
    public const float KnockInterval = 5f;
    /// <summary>Dialogue typewriter speed, characters per second.</summary>
    public const float TextSpeed = 55f;
    /// <summary>Pitch shift of the QTE Perfect chime: -1..1, 1.0 = one octave (0 = as synthesized, G5 -> D6).</summary>
    public const float PerfectPitch = 0f;
    /// <summary>Volume of the QTE Perfect chime, 0..1.</summary>
    public const float PerfectVolume = 0.8f;

    // ---- Movement feel (presentation only; no gameplay impact) ----
    public const float PlayerMaxSpeed = 330, PlayerAccel = 2200, PlayerDecel = 2800, PlayerTurnAccel = 4500;
    /// <summary>Below this speed (px/s) the footstep loop stays silent.</summary>
    public const float FootstepMinSpeed = 40;
    /// <summary>Smoothing rate of the visual facing (x-scale) after a turn.</summary>
    public const float FaceDamp = 18;
    public const float WalkBobPx = 4;
    /// <summary>Hold Shift to run: top speed (px/s); accel/decel are shared with walking.</summary>
    public const float PlayerRunSpeed = 560;
    /// <summary>SpriteMotion (single-image character): stride rate (rad/s at walk speed), lean/sway (rad), squash/stretch amounts.</summary>
    public const float StrideRate = 12, WalkLeanRad = 0.06f, RunLeanRad = 0.12f, StrideSwayRad = 0.03f;
    public const float BreathScale = 0.02f, StepSquash = 0.04f, StartStopSquash = 0.08f;
    public const int DustMax = 32;
    public const float DustLife = 0.35f, DustMinSpeed = 0.4f;
    /// <summary>Camera: look-ahead = clamp(vel * Scale, +-Max) smoothed by LookK; deadzone half-width; follow rate.</summary>
    public const float CamLookScale = 0.35f, CamLookMax = 110, CamLookK = 4, CamDeadzone = 60, CamFollowK = 5;
    public const float PetWalkSpeed = 55, PetEaseMul = 1.6f, PetHopPx = 5, PetHopLen = 45;
    /// <summary>Display smoothing rate for HUD / pet bars.</summary>
    public const float BarSmoothK = 8;
    public const float PromptFadeTime = 0.15f;
    /// <summary>Longest frame step used by scenes in normal play (guards against hitches).</summary>
    public const float MaxDt = 1 / 20f;
    public const float FadeSpeed = 2.5f;
    public const float OverlayOpenTime = 0.18f, OverlayOpenScale = 0.94f;
    public const float ButtonHoverK = 18, ButtonLiftPx = 2;
    public const int StartCoin = 150;
    public const int DailyCoin = 100;
    public const int ToothlessReward = 200;
    public const int MerchantOffer = 5000;
    public const int ReviveCost = 250;
    public const int BaseEnergy = 3;
    public const int MaxEnergyCap = 6;

    public const int DailyStomachDecay = 20;
    public const int DailyCleanDecay = 15;
    public const int StarveDamage = 20;
    public const int StormCleanLoss = 50;

    public const int HpPerLevel = 10;
    public const int AtkPerLevel = 5;

    public const int Attempts = 10;

    // Pet passives (first pass, to be balanced)
    public const int PassiveLevel = 2;
    public const float MosslingHealPct = 0.01f;
    /// <summary>Nibbleclaw combo: each consecutive Attack hit adds this fraction of base damage (10, 15, 20...).</summary>
    public const float NibbleComboStep = 0.5f;
    /// <summary>Toothless poison: enemy takes +25% damage and deals -25% damage.</summary>
    public const float PoisonTakenMul = 1.25f, PoisonDealtMul = 0.75f;

    // ================= FIGHT (Scenes/FightScene.cs) =================
    // Zone sizes are half-widths in radians (full circle = 6.28); speeds are rad/s.

    // --- Fight: wheel zones ---
    /// <summary>Dodge Perfect half-width when it appears; the Attack zone starts at the same size.</summary>
    public const float DodgeStart = 0.32f;
    /// <summary>Same, for the next fight after drinking Sea Tea (+20%).</summary>
    public const float DodgeStartSeaTea = 0.38f;
    /// <summary>Dodge counts as "Too slow" (enemy hits) once it shrinks to this half-width.</summary>
    public const float DodgeVanish = 0.012f;
    /// <summary>Attack shrinks at this fraction of the enemy's DodgeShrink.</summary>
    public const float AttackShrinkMul = 0.5f;
    /// <summary>Attack Perfect never shrinks below this half-width.</summary>
    public const float AttackMinPerfect = 0.06f;
    /// <summary>Attack Great half-width = Perfect + this.</summary>
    public const float AttackGreatBand = 0.16f;
    /// <summary>Min distance (rad) from the needle / other zones when a new zone is placed.</summary>
    public const float DodgeSpawnGap = 1.2f, AttackSpawnGap = 0.8f;

    // --- Fight: damage ---
    /// <summary>Share of ATK dealt by a Great hit (Perfect = 100%).</summary>
    public const float AttackGreatDmgMul = 0.75f;

    // --- Fight: enemies (Model/Pet.cs). DodgeShrink = rad/s the Dodge zone shrinks; NeedleSpeed = rad/s of the needle ---
    public const int ToothlessHp = 120, ToothlessAtk = 15;
    public const float ToothlessDodgeShrink = 0.1f, ToothlessNeedleSpeed = 2.4f;
    public const int BigZHp = 9999, BigZAtk = 20;
    public const float BigZDodgeShrink = 0.26f, BigZNeedleSpeed = 3.4f;

    // --- Fight visual: layout (pixels, 1280x720) ---
    public const float FightWheelY = 440f, FightWheelRadius = 150f, FightWheelThickness = 26f;
    public const int FightFloorY = 560;
    public const float FightPetX = 250f, FightEnemyX = 1010f, FightPetScale = 1.4f;
    public const int FightPetBarW = 140, FightPetBarH = 16, FightPetBarY = 380, FightPetLabelY = 354;
    public const int FightEnemyBarW = 400, FightEnemyBarH = 24, FightEnemyBarY = 60, FightEnemyMargin = 40, FightEnemyNameY = 24;
    public const float FightTipY = 250f, FightEndTextY = 300f;

    // --- Fight visual: popups (floating text positions) ---
    public const float FightPopupCenterY = 250f;
    public static readonly Vector2 FightPopupPet = new(300, 200), FightPopupPetSub = new(300, 260);
    public static readonly Vector2 FightPopupEnemyDmg = new(1000, 150), FightPopupEnemyStatus = new(1000, 220);

    // --- Fight visual: effects (screen shake = strength, seconds; fades = per second) ---
    public const float FightHurtShake = 14f, FightHurtShakeTime = 0.35f;
    public const float FightHitShake = 8f, FightHitShakeTime = 0.2f;
    public const float FightFlashFade = 4f, FightLungeFade = 3f;
    public const float FightPetLunge = 30f, FightEnemyLunge = 60f;
    public const float FightPoisonTint = 0.25f;
    /// <summary>Seconds the VICTORY / DEFEATED banner stays before leaving the fight, and its dim strength.</summary>
    public const float FightEndDelay = 1.5f, FightEndDim = 0.4f;

    // --- Fight visual: colors ---
    public static readonly Color FightFloor = new(30, 22, 24), FightFloorEdge = new(80, 50, 44);
    public static readonly Color FightRedTint = new(60, 0, 10);
    public const float FightRedTintAlpha = 0.18f;
    public static readonly Color FightPoisonColor = new(170, 90, 220);
    /// <summary>Backdrop (night base seen through the door): world X and brightness.</summary>
    public const float FightBackdropX = 3000f, FightBackdropLight = 0.85f;

    // Upgrade costs (Figma Scene 12)
    public const int QteCoin = 100, QtePoints = 1, QteMax = 3;
    public const int EnergyCoin = 150, EnergyPoints = 3;
    public const int ProgressCoin = 20, ProgressPoints = 2, ProgressMax = 2;
}

public class GameState
{
    public int Day = 1;
    public int Coin = Balance.StartCoin;
    public int Energy = Balance.BaseEnergy;
    public int PlayerLevel = 1;
    public int PlayerExp;
    public int Points;

    public int QteUpgrade;
    public int EnergyUpgrade;
    public int ProgressUpgrade;

    public readonly List<Pet> Pets = new();
    public bool DoorDone;
    public bool DisasterSeen;
    public bool SeaTea;

    public int MaxEnergy => Math.Min(Balance.BaseEnergy + EnergyUpgrade, Balance.MaxEnergyCap);
    public int PlayerMaxExp => 10 + (PlayerLevel - 1) * 10;
    public float TrainGainMultiplier => 1f + 0.5f * ProgressUpgrade;   // +10 -> +15
    public float QteZoneMultiplier => 1f + 0.2f * QteUpgrade;

    public IEnumerable<Pet> Alive => Pets.Where(p => !p.Dead);

    /// <summary>A door event is waiting (Day 2-5, once per day).</summary>
    public bool DoorEventPending => Day >= 2 && Day <= Balance.LastDay && !DoorDone;

    public GameState(Species starter) => Pets.Add(Pet.Create(starter));

    /// <summary>Coin gained; <paramref name="source"/> labels it in telemetry.</summary>
    public void AddCoin(int amount, string source)
    {
        Coin += amount;
        Telemetry.Coin(amount, source, Coin);
    }

    /// <summary>Coin spent; <paramref name="sink"/> labels it in telemetry. Callers check affordability first.</summary>
    public void SpendCoin(int amount, string sink)
    {
        Coin -= amount;
        Telemetry.Coin(-amount, sink, Coin);
    }

    public void AddEnergy(int delta, string reason)
    {
        Energy += delta;
        Telemetry.Energy(delta, reason, Energy, MaxEnergy);
    }

    /// <summary>Returns true on player level up.</summary>
    public bool AddExp(int amount, string reason = "qte")
    {
        bool up = false;
        PlayerExp += amount;
        while (PlayerExp >= PlayerMaxExp)
        {
            PlayerExp -= PlayerMaxExp;
            PlayerLevel++;
            Points++;
            up = true;
        }
        Telemetry.Exp(amount, reason, this, up);
        return up;
    }

    /// <summary>End-of-day processing, then morning of the next day. Returns report lines.</summary>
    public List<string> AdvanceDay()
    {
        var report = new List<string>();
        foreach (var p in Pets.Where(p => !p.Dead))
        {
            if (p.Stomach <= 0)
            {
                float hpBefore = p.Hp;
                p.Hp -= Balance.StarveDamage;
                Telemetry.Emit("starve", "pet", p.Name, "hpBefore", (int)MathF.Ceiling(hpBefore), "hpAfter", p.HpShown, "died", p.Dead);
                if (p.Dead) Telemetry.PetDeath(p, "starve", hpBefore);
                report.Add(p.Dead
                    ? $"{p.Name} starved and has fallen..."
                    : $"{p.Name} is starving! -{Balance.StarveDamage} HP");
            }
        }

        Day++;
        AddCoin(Balance.DailyCoin, "daily");
        AddEnergy(MaxEnergy - Energy, "daily_restore");
        DoorDone = false;

        foreach (var p in Pets.Where(p => !p.Dead))
        {
            p.Stomach -= Balance.DailyStomachDecay;
            p.Clean -= Balance.DailyCleanDecay;
            p.ClampStats();
        }

        report.Insert(0, $"Day {Day}. +{Balance.DailyCoin} coin, energy restored.");
        report.Add($"Every pet got hungrier (Stomach -{Balance.DailyStomachDecay}) and dirtier (Clean -{Balance.DailyCleanDecay}).");
        if (DoorEventPending) report.Add("...something is knocking at the red door.");
        return report;
    }
}
