using System;
using System.Collections.Generic;
using System.Linq;

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

    /// <summary>Returns true on player level up.</summary>
    public bool AddExp(int amount)
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
                p.Hp -= Balance.StarveDamage;
                report.Add(p.Dead
                    ? $"{p.Name} starved and has fallen..."
                    : $"{p.Name} is starving! -{Balance.StarveDamage} HP");
            }
        }

        Day++;
        Coin += Balance.DailyCoin;
        Energy = MaxEnergy;
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
