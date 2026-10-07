using System;
using Microsoft.Xna.Framework;

namespace Bepal;

public enum Species { Mossling, Nibbleclaw, Blinkbun, Toothless }

public class Pet
{
    public Species Species;
    public string Name = "";
    public string Element = "";
    public string Description = "";
    public Color Color;
    public int BaseMaxHp;
    public int BaseAtk;

    public float Hp;
    public float Stomach;
    public float Clean;
    public int Level = 1;
    public float Progress;

    /// <summary>Passive skill description shown in the Notebook; null = species has no passive yet.</summary>
    public string? Passive;
    public bool PassiveUnlocked => Passive != null && Level >= Balance.PassiveLevel;
    public bool Has(Species s) => Species == s && PassiveUnlocked;

    // GDD 06 §3.5: Clean <= 25 lowers Max HP, Clean <= 50 lowers ATK.
    public int MaxHp => (int)((BaseMaxHp + (Level - 1) * Balance.HpPerLevel) * (Clean <= 25 ? 0.8f : 1f));
    public int Atk => (int)MathF.Round((BaseAtk + (Level - 1) * Balance.AtkPerLevel) * (Clean <= 50 ? 0.75f : 1f));
    public int MaxProgress => 100 + (Level - 1) * 50;
    public bool Dead => Hp <= 0;
    public int HpShown => (int)MathF.Ceiling(Math.Max(0, Hp));

    public void ClampStats()
    {
        Stomach = MathHelper.Clamp(Stomach, 0, 100);
        Clean = MathHelper.Clamp(Clean, 0, 100);
        Hp = MathHelper.Clamp(Hp, 0, MaxHp);
    }

    /// <summary>Adds train progress; returns number of level-ups.</summary>
    public int AddProgress(float amount)
    {
        int ups = 0;
        Progress += amount;
        while (Progress >= MaxProgress)
        {
            Progress -= MaxProgress;
            Level++;
            Hp += Balance.HpPerLevel;
            ups++;
        }
        ClampStats();
        return ups;
    }

    public static Pet Create(Species s) => s switch
    {
        Species.Mossling => new Pet
        {
            Species = s, Name = "Mossling", Element = "Plant", Color = new Color(206, 78, 70),
            BaseMaxHp = 100, BaseAtk = 20, Hp = 100, Stomach = 80, Clean = 70,
            Description = "A soft, moss-covered creature with a single bud on its head. Calm, but startles easily. Its eyes never quite blink.",
            Passive = "Every attack heals this pet for 1% of its Max HP.",
        },
        Species.Nibbleclaw => new Pet
        {
            Species = s, Name = "Nibbleclaw", Element = "Beast", Color = new Color(222, 150, 70),
            BaseMaxHp = 90, BaseAtk = 25, Hp = 90, Stomach = 70, Clean = 80,
            Description = "Half cat, half anteater. Long claws hide under its fur. Quick, curious and a little too interested in your fingers.",
            Passive = "Consecutive Attack hits without a miss build a combo: each hit deals more damage than the last. A miss resets it.",
        },
        Species.Blinkbun => new Pet
        {
            Species = s, Name = "Blinkbun", Element = "Shadow", Color = new Color(92, 176, 112),
            BaseMaxHp = 110, BaseAtk = 15, Hp = 110, Stomach = 60, Clean = 60,
            Description = "A long-eared rabbit with a third eye on its forehead. Patient and tough. The third eye watches you sleep.",
            Passive = "Every attack warps the needle to 12 o'clock.",
        },
        _ => new Pet
        {
            Species = s, Name = "Toothless", Element = "Acid", Color = new Color(52, 46, 64),
            BaseMaxHp = 120, BaseAtk = 15, Hp = 60, Stomach = 60, Clean = 60,
            Description = "A slick black reptile with no teeth and a throat sac full of purple acid. Tamed... mostly.",
            Passive = "Every attack poisons the enemy: it takes more damage and hits weaker. Does not stack.",
        },
    };
}

public class Enemy
{
    public string Name = "";
    public string AttackName = "";
    public float MaxHp;
    public float Hp;
    public int Atk;
    public Color Color;
    public float Size = 1f;
    /// <summary>Radians per second the Dodge zone shrinks.</summary>
    public float DodgeShrink;
    public float NeedleSpeed;
    public bool IsBoss;

    public static Enemy Toothless() => new()
    {
        Name = "Toothless", AttackName = "Acid", MaxHp = Balance.ToothlessHp, Hp = Balance.ToothlessHp, Atk = Balance.ToothlessAtk, Color = new Color(52, 46, 64),
        DodgeShrink = Balance.ToothlessDodgeShrink, NeedleSpeed = Balance.ToothlessNeedleSpeed,
    };

    public static Enemy BigZ() => new()
    {
        Name = "Big Z", AttackName = "Crush", MaxHp = Balance.BigZHp, Hp = Balance.BigZHp, Atk = Balance.BigZAtk, Color = new Color(120, 24, 36),
        Size = 1.7f, DodgeShrink = Balance.BigZDodgeShrink, NeedleSpeed = Balance.BigZNeedleSpeed, IsBoss = true,
    };
}
