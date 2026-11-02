using System;
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
    int _index = -1;

    public ShotRunner(string dir)
    {
        _dir = dir;
        Directory.CreateDirectory(dir);
    }

    /// <summary>Returns false when all shots are done.</summary>
    public bool Step(GraphicsDevice gd, SpriteBatch sb)
    {
        _index++;
        if (_index >= SceneCatalog.All.Count) return false;
        var (name, _, seconds) = SceneCatalog.All[_index];
        var m = new SceneManager();
        SceneCatalog.Apply(m, SceneCatalog.All[_index]);
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
