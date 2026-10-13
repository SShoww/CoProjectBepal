using System;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Bepal;

/// <summary>Shop items. Bought into <see cref="GameState.Inventory"/> (the box in the base) and used later.</summary>
public enum Item { CrabApple, CaffeineTonic, SeaTea, CloudyGlasses, BalletShoes, TornNotebook, FadedRibbon }

public record ItemInfo(Item Item, string Name, int Price, string Desc, Color Color);

public static class Items
{
    public const int Count = 7;

    /// <summary>Catalog in shop order (index = card slot; the first three keep their old slots).</summary>
    public static readonly ItemInfo[] All =
    {
        new(Item.CrabApple, "Crab Apple", Balance.CrabApplePrice, $"One pet: +{Balance.CrabAppleHp} HP, +{Balance.CrabAppleStomach} Stomach", new Color(220, 70, 60)),
        new(Item.CaffeineTonic, "Caffeine Tonic", Balance.TonicPrice, $"+{Balance.TonicEnergy} Energy (up to max)", new Color(230, 160, 60)),
        new(Item.SeaTea, "Sea Tea", Balance.SeaTeaPrice, "Dodge zone +20% in your next fight", new Color(90, 190, 220)),
        new(Item.CloudyGlasses, "Cloudy Glasses", Balance.GlassesPrice, $"Blocks {Balance.GlassesShield} hits in your next fight", new Color(170, 180, 200)),
        new(Item.BalletShoes, "Ballet Shoes", Balance.BalletPrice, $"Perfect zones +{Balance.BalletPerfectBonus * 100:0}% today, Stomach -{Balance.BalletStomachExtra} tonight", new Color(240, 140, 190)),
        new(Item.TornNotebook, "Torn Notebook", Balance.NotebookPrice, $"Train Progress +{Balance.NotebookTrainBonus * 100:0}% today", new Color(190, 160, 110)),
        new(Item.FadedRibbon, "Faded Ribbon", Balance.RibbonPrice, $"Clean loss -{Balance.RibbonCleanDecayCut * 100:0}% tonight", new Color(200, 90, 130)),
    };

    public static ItemInfo Info(Item i) => All[(int)i];

    // ---- card layout shared by the shop and the inventory (1280x720: 4 + 3 cards) ----
    public const int CardW = 270, CardH = 230, CardGap = 20, Row1Y = 150, Row2Y = 400;

    public static Rectangle CardRect(int i)
    {
        int perRow = i < 4 ? 4 : Count - 4;
        int x0 = Gfx.W / 2 - (perRow * CardW + (perRow - 1) * CardGap) / 2;
        int col = i < 4 ? i : i - 4;
        return new Rectangle(x0 + col * (CardW + CardGap), i < 4 ? Row1Y : Row2Y, CardW, CardH);
    }

    /// <summary>Buy / Use button of card <paramref name="i"/> (also used by the autoplay bot).</summary>
    public static Rectangle ButtonRect(int i)
    {
        var c = CardRect(i);
        return new Rectangle(c.X + 25, c.Y + CardH - 52, CardW - 50, 40);
    }

    /// <summary>Why the item can't be used right now, or null if it can.</summary>
    public static string? UseBlock(GameState gs, Item item) => item switch
    {
        Item.CrabApple => gs.Alive.Any() ? null : "No pet",
        Item.CaffeineTonic => gs.Energy < gs.MaxEnergy ? null : "Energy full",
        Item.SeaTea => gs.SeaTea ? "Active" : null,
        Item.CloudyGlasses => gs.GlassesShield > 0 ? "Active" : null,
        Item.BalletShoes => gs.BalletActive ? "Active" : null,
        Item.TornNotebook => gs.NotebookActive ? "Active" : null,
        _ => gs.RibbonActive ? "Active" : null,
    };

    /// <summary>Consumes one <paramref name="item"/> and applies it; returns the popup text. <paramref name="target"/> is only used by Crab Apple.</summary>
    public static string Apply(GameState gs, Item item, Pet? target)
    {
        gs.TakeItem(item);
        string msg;
        switch (item)
        {
            case Item.CrabApple:
                target!.Hp += Balance.CrabAppleHp;
                target.Stomach += Balance.CrabAppleStomach;
                target.ClampStats();
                msg = $"{target.Name} munches happily";
                break;
            case Item.CaffeineTonic:
                msg = $"+{gs.AddEnergy(Balance.TonicEnergy, "tonic")} Energy!";
                break;
            case Item.SeaTea:
                gs.SeaTea = true;
                msg = "Sea Tea ready for the next fight";
                break;
            case Item.CloudyGlasses:
                gs.GlassesShield = Balance.GlassesShield;
                msg = "Cloudy Glasses ready for the next fight";
                break;
            case Item.BalletShoes:
                gs.BalletActive = true;
                msg = "Ballet Shoes on: bigger Perfect zones today";
                break;
            case Item.TornNotebook:
                gs.NotebookActive = true;
                msg = "Torn Notebook: Train Progress boosted today";
                break;
            default:
                gs.RibbonActive = true;
                msg = "Faded Ribbon: pets stay cleaner tonight";
                break;
        }
        Telemetry.Emit("item_use", "item", Info(item).Name, "target", target?.Name ?? "", "left", gs.Owned(item));
        return msg;
    }
}
