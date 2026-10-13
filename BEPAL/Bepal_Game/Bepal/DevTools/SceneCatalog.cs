using System;
using System.Collections.Generic;
using System.Linq;

namespace Bepal;

/// <summary>
/// Dev tool: every screen set up with sample data, by name. Used by <see cref="ShotRunner"/> (one PNG each),
/// <c>--scene &lt;name&gt;</c> and the F2 <see cref="SceneJumpScene"/>. Add a scene here and it shows up in all three.
/// </summary>
public static class SceneCatalog
{
    /// <summary><paramref name="Build"/> returns a fresh stack, bottom to top.</summary>
    public record Entry(string Name, Func<Scene[]> Build, float ShotSeconds);

    static GameState Gs(int day = 2)
    {
        var gs = new GameState(Species.Mossling) { Day = day, Coin = 420, Points = 2, PlayerExp = 6 };
        gs.Pets.Add(Pet.Create(Species.Blinkbun));
        gs.Pets.Add(Pet.Create(Species.Toothless));
        gs.Pets[2].Hp = 0;
        return gs;
    }

    static Scene[] S(params Scene[] scenes) => scenes;

    public static readonly IReadOnlyList<Entry> All = BuildAll();

    static List<Entry> BuildAll()
    {
        var l = new List<Entry>();
        void Add(string name, Func<Scene[]> build, float seconds = 0.5f) => l.Add(new Entry(name, build, seconds));

        Add("01_menu", () => S(new MainMenuScene()), 1.2f);
        Add("02_choose", () => S(new ChooseStarterScene()), 0.5f);
        Add("03_base", () => S(new BaseScene(Gs())), 1.0f);
        Add("04_base_door", () => S(new BaseScene(Gs()) { StartX = 3450 }), 1.5f);
        Add("05_base_bed_night", () => S(new BaseScene(Gs()) { StartX = 400, Night = 0.9f }), 1.5f);
        Add("06_care", () => { var g = Gs(); return S(new BaseScene(g), new CareSelectScene(g, g.Pets[0])); }, 0.6f);
        Add("06b_care_hurt", () => { var g = Gs(); g.Pets[0].Hp -= 30; return S(new BaseScene(g), new CareSelectScene(g, g.Pets[0])); }, 0.6f);
        foreach (var c in Enum.GetValues<Care>())
        {
            var care = c;
            Add($"07_qte_{care}", () => { var g = Gs(); return S(new BaseScene(g), new QteScene(g, g.Pets[0], care)); }, 0.7f);
        }
        Add("08_fight", () => { var g = Gs(); return S(new FightScene(g, Enemy.Toothless(), g.Pets[0], _ => { })); }, 0.8f);
        Add("09_fight_bigz", () => { var g = Gs(5); return S(new FightScene(g, Enemy.BigZ(), g.Pets[1], _ => { })); }, 0.8f);
        Add("10_upgrade", () => { var g = Gs(); return S(new BaseScene(g), new UpgradeScene(g)); }, 0.2f);
        Add("10b_upgrade_poor", () => { var g = Gs(); g.Points = 0; g.Coin = 60; return S(new BaseScene(g), new UpgradeScene(g)); }, 0.2f);
        Add("11_doctor", () => { var g = Gs(); return S(new BaseScene(g), new DoctorScene(g)); }, 0.2f);
        Add("12_notebook", () => { var g = Gs(); return S(new BaseScene(g), new NotebookScene(g)); }, 0.2f);
        Add("12b_notebook_pet_lv1", () => { var g = Gs(); return S(new BaseScene(g), new NotebookScene(g, 0)); }, 0.2f);
        Add("12c_notebook_pet_lv2", () => { var g = Gs(); g.Pets[0].Level = Balance.PassiveLevel; return S(new BaseScene(g), new NotebookScene(g, 0)); }, 0.2f);
        Add("13_shop", () => { var g = Gs(3); return S(new BaseScene(g), new ShopScene(g)); }, 0.2f);
        Add("13b_inventory", () =>
        {
            var g = Gs(3);
            g.AddItem(Item.CrabApple); g.AddItem(Item.CrabApple); g.AddItem(Item.CaffeineTonic); g.AddItem(Item.CloudyGlasses); g.AddItem(Item.FadedRibbon);
            g.Energy = 1;
            return S(new BaseScene(g), new InventoryScene(g));
        }, 0.2f);
        Add("13c_base_box", () => { var g = Gs(3); g.AddItem(Item.SeaTea); return S(new BaseScene(g) { StartX = BaseScene.BoxX }); }, 0.3f);
        Add("14_dialogue", () => { var g = Gs(); return S(new BaseScene(g), DialogueScene.Say("Toothless", null, "Arrrrrrhrhrhrhhrrhrhrha")); }, 2f);
        Add("15_choice", () => { var g = Gs(); return S(new BaseScene(g), new ChoiceScene("Toothless is at the door. What will you do?",
            new[] { new Option("Chase", () => { }), new Option("Tame", () => { }) })); }, 0.2f);
        Add("19_options", () => { var g = Gs(); Settings.Music = 0.7f; Settings.Sfx = 0.35f; return S(new BaseScene(g), new OptionsScene()); }, 0.3f);
        Add("16_tbc", () => S(new ToBeContinuedScene(5)), 3f);
        Add("17_rain_day4", () => S(new BaseScene(Gs(4)) { StartX = 1250 }), 3f);
        Add("18_storm_after_door", () => { var g = Gs(4); g.DoorDone = true; return S(new BaseScene(g) { StartX = 1250 }); }, 4f);
        return l;
    }

    /// <summary>Replace the whole stack of <paramref name="m"/> with the entry's scenes (no fade).</summary>
    public static void Apply(SceneManager m, Entry e)
    {
        var stack = e.Build();
        m.ResetNow(stack[0]);
        for (int i = 1; i < stack.Length; i++) m.Push(stack[i]);
    }

    /// <summary>Exact name wins; otherwise a single case-insensitive substring match. Null if none or ambiguous.</summary>
    public static Entry? Find(string query)
    {
        var exact = All.FirstOrDefault(e => e.Name.Equals(query, StringComparison.OrdinalIgnoreCase));
        if (exact != null) return exact;
        var hits = All.Where(e => e.Name.Contains(query, StringComparison.OrdinalIgnoreCase)).ToList();
        return hits.Count == 1 ? hits[0] : null;
    }
}
