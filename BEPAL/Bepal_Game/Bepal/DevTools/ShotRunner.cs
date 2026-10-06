using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bepal;

/// <summary>
/// Dev tool: <c>dotnet run -- --shots &lt;dir&gt;</c> sets up each screen, simulates a moment of play,
/// saves a PNG per screen, then exits. Used to check layouts without clicking through the game.
/// </summary>
public class ShotRunner
{
    readonly string _dir;
    readonly List<(string name, Func<SceneManager> setup, float seconds)> _shots = new();
    int _index = -1;

    public ShotRunner(string dir)
    {
        _dir = dir;
        Directory.CreateDirectory(dir);

        GameState Gs(int day = 2)
        {
            var gs = new GameState(Species.Mossling) { Day = day, Coin = 420, Points = 2, PlayerExp = 6 };
            gs.Pets.Add(Pet.Create(Species.Blinkbun));
            gs.Pets.Add(Pet.Create(Species.Toothless));
            gs.Pets[2].Hp = 0;
            return gs;
        }

        SceneManager With(params Scene[] scenes)
        {
            var m = new SceneManager();
            m.ResetNow(scenes[0]);
            for (int i = 1; i < scenes.Length; i++) m.Push(scenes[i]);
            return m;
        }

        _shots.Add(("01_menu", () => With(new MainMenuScene()), 1.2f));
        _shots.Add(("02_choose", () => With(new ChooseStarterScene()), 0.5f));
        _shots.Add(("03_base", () => With(new BaseScene(Gs())), 1.0f));
        _shots.Add(("04_base_door", () => With(new BaseScene(Gs()) { StartX = 3450 }), 1.5f));
        _shots.Add(("05_base_bed_night", () => With(new BaseScene(Gs()) { StartX = 400, Night = 0.9f }), 1.5f));
        _shots.Add(("06_care", () => { var g = Gs(); return With(new BaseScene(g), new CareSelectScene(g, g.Pets[0])); }, 0.6f));
        foreach (var c in Enum.GetValues<Care>())
            _shots.Add(($"07_qte_{c}", () => { var g = Gs(); return With(new BaseScene(g), new QteScene(g, g.Pets[0], c)); }, 0.7f));
        _shots.Add(("08_fight", () => { var g = Gs(); return With(new FightScene(g, Enemy.Toothless(), g.Pets[0], _ => { })); }, 0.8f));
        _shots.Add(("09_fight_bigz", () => { var g = Gs(5); return With(new FightScene(g, Enemy.BigZ(), g.Pets[1], _ => { })); }, 0.8f));
        _shots.Add(("10_upgrade", () => { var g = Gs(); return With(new BaseScene(g), new UpgradeScene(g)); }, 0.2f));
        _shots.Add(("11_doctor", () => { var g = Gs(); return With(new BaseScene(g), new DoctorScene(g)); }, 0.2f));
        _shots.Add(("12_notebook", () => { var g = Gs(); return With(new BaseScene(g), new NotebookScene(g)); }, 0.2f));
        _shots.Add(("13_shop", () => { var g = Gs(3); return With(new BaseScene(g), new ShopScene(g)); }, 0.2f));
        _shots.Add(("14_dialogue", () => { var g = Gs(); return With(new BaseScene(g), DialogueScene.Say("Toothless", null, "Arrrrrrhrhrhrhhrrhrhrha")); }, 2f));
        _shots.Add(("15_choice", () => { var g = Gs(); return With(new BaseScene(g), new ChoiceScene("Toothless is at the door. What will you do?",
            new[] { new Option("Chase", () => { }), new Option("Tame", () => { }) })); }, 0.2f));
        _shots.Add(("16_tbc", () => With(new ToBeContinuedScene(5)), 3f));
        _shots.Add(("17_rain_day4", () => With(new BaseScene(Gs(4)) { StartX = 1250 }), 3f));
        _shots.Add(("18_storm_after_door", () => { var g = Gs(4); g.DoorDone = true; return With(new BaseScene(g) { StartX = 1250 }); }, 4f));
    }

    /// <summary>Returns false when all shots are done.</summary>
    public bool Step(GraphicsDevice gd, SpriteBatch sb)
    {
        _index++;
        if (_index >= _shots.Count) return false;
        var (name, setup, seconds) = _shots[_index];
        var m = setup();
        const float dt = 1 / 60f;
        for (float t = 0; t < seconds; t += dt)
        {
            Gfx.UpdateShake(dt);
            m.Update(dt);
        }

        using var rt = new RenderTarget2D(gd, Gfx.W, Gfx.H);
        gd.SetRenderTarget(rt);
        gd.Clear(Color.Black);
        Gfx.Begin(sb);
        m.Draw(sb);
        sb.End();
        gd.SetRenderTarget(null);
        using var fs = File.Create(Path.Combine(_dir, name + ".png"));
        rt.SaveAsPng(fs, Gfx.W, Gfx.H);
        return true;
    }
}
