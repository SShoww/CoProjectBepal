using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Bepal;

/// <summary>Dev tool: F2 overlay that lists every <see cref="SceneCatalog"/> entry and jumps straight to it.</summary>
public class SceneJumpScene : Scene
{
    const int Rows = 12, CellW = 340, CellH = 40, Top = 130;

    public override bool Overlay => true;
    int _sel;

    static int Cols => (SceneCatalog.All.Count + Rows - 1) / Rows;
    static int Left => (Gfx.W - Cols * CellW) / 2;

    static Rectangle Cell(int i) => new(Left + i / Rows * CellW, Top + i % Rows * CellH, CellW - 8, CellH - 6);

    void Go(int i)
    {
        SceneCatalog.Apply(M, SceneCatalog.All[i]);
    }

    public override void Update(float dt)
    {
        int n = SceneCatalog.All.Count;
        if (Input.Back || Input.Pressed(Keys.F2)) { M.Remove(this); return; }
        if (Input.Pressed(Keys.Down) || Input.Pressed(Keys.S)) _sel = (_sel + 1) % n;
        if (Input.Pressed(Keys.Up) || Input.Pressed(Keys.W)) _sel = (_sel + n - 1) % n;
        if (Input.Pressed(Keys.Right) || Input.Pressed(Keys.D)) _sel = System.Math.Min(_sel + Rows, n - 1);
        if (Input.Pressed(Keys.Left) || Input.Pressed(Keys.A)) _sel = System.Math.Max(_sel - Rows, 0);

        for (int i = 0; i < n; i++)
            if (Cell(i).Contains(Input.Mouse))
            {
                _sel = i;
                if (Input.Click) { Go(i); return; }
            }
        if (Input.Confirm) Go(_sel);
    }

    public override void Draw(SpriteBatch sb)
    {
        Ui.Dim(sb, 0.8f);
        Gfx.Text(sb, Gfx.Big, "Scene Jump (dev)", new Vector2(Gfx.W / 2f, 60), Palette.Warm, 0.5f);
        for (int i = 0; i < SceneCatalog.All.Count; i++)
        {
            var r = Cell(i);
            bool on = i == _sel;
            Gfx.Rect(sb, r, on ? Palette.PanelEdge : Palette.Panel);
            Gfx.Text(sb, Gfx.Small, SceneCatalog.All[i].Name, new Vector2(r.X + 10, r.Y + (r.Height - Gfx.Small.LineSpacing) / 2f),
                on ? Color.Black : Palette.Text, shadow: false);
        }
        Ui.Hint(sb, "W/S/A/D or mouse: pick   Space/Click: jump   Esc/F2: close");
    }
}
