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

    /// <summary>Reserved for pet skills (GDD 06: passives are out of prototype scope, pending design talk).</summary>
    public string? Passive;

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
        },
        Species.Nibbleclaw => new Pet
        {
            Species = s, Name = "Nibbleclaw", Element = "Beast", Color = new Color(222, 150, 70),
            BaseMaxHp = 90, BaseAtk = 25, Hp = 90, Stomach = 70, Clean = 80,
            Description = "Half cat, half anteater. Long claws hide under its fur. Quick, curious and a little too interested in your fingers.",
        },
        Species.Blinkbun => new Pet
        {
            Species = s, Name = "Blinkbun", Element = "Shadow", Color = new Color(92, 176, 112),
            BaseMaxHp = 110, BaseAtk = 15, Hp = 110, Stomach = 60, Clean = 60,
            Description = "A long-eared rabbit with a third eye on its forehead. Patient and tough. The third eye watches you sleep.",
        },
        _ => new Pet
        {
            Species = s, Name = "Toothless", Element = "Acid", Color = new Color(52, 46, 64),
            BaseMaxHp = 120, BaseAtk = 15, Hp = 60, Stomach = 60, Clean = 60,
            Description = "A slick black reptile with no teeth and a throat sac full of purple acid. Tamed... mostly.",
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
        Name = "Toothless", AttackName = "Acid", MaxHp = 120, Hp = 120, Atk = 15, Color = new Color(52, 46, 64),
        DodgeShrink = 0.11f, NeedleSpeed = 2.8f,
    };

    public static Enemy BigZ() => new()
    {
        Name = "Big Z", AttackName = "Crush", MaxHp = 9999, Hp = 9999, Atk = 20, Color = new Color(120, 24, 36),
        Size = 1.7f, DodgeShrink = 0.26f, NeedleSpeed = 3.4f, IsBoss = true,
    };
}
