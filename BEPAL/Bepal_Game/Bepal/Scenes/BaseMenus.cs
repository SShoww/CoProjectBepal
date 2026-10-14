using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Bepal;

/// <summary>Figma Scene 12 — spend Coin + Points (Points come from Player level ups).</summary>
public class UpgradeScene : Scene
{
    readonly GameState _gs;
    public GameState State => _gs;
    readonly Button[] _plus = new Button[3];
    readonly Popups _popups = new();

    public override bool Overlay => true;

    public UpgradeScene(GameState gs)
    {
        _gs = gs;
        for (int i = 0; i < 3; i++) _plus[i] = new Button(PlusRect(i), "+");
    }

    /// <summary>Hit box of the "+" button of row <paramref name="i"/> (also used by the autoplay bot).</summary>
    public static Rectangle PlusRect(int i) => new(880, 250 + i * 110, 64, 64);

    (string name, string desc, int coin, int points, int level, int max)[] Rows =>
    new[]
    {
        ("QTE", "Train dots get bigger", Balance.QteCoin, Balance.PointCost(0, _gs.QteUpgrade), _gs.QteUpgrade, Balance.QteMax),
        ("Energy", "+1 Energy every day", Balance.EnergyCoin, Balance.PointCost(1, _gs.EnergyUpgrade), _gs.EnergyUpgrade,
            Balance.MaxEnergyCap - Balance.BaseEnergy),
        ("Progress Bar", "Train progress +50% each", Balance.ProgressCoin, Balance.PointCost(2, _gs.ProgressUpgrade), _gs.ProgressUpgrade,
            Balance.ProgressMax),
    };

    public override void Update(float dt)
    {
        _popups.Update(dt);
        var rows = Rows;
        for (int i = 0; i < 3; i++)
        {
            var r = rows[i];
            _plus[i].Enabled = r.level < r.max && _gs.Coin >= r.coin && _gs.Points >= r.points;
            if (!_plus[i].Update()) continue;
            _gs.SpendCoin(r.coin, "upgrade");
            _gs.Points -= r.points;
            Audio.Play(Sfx.UiCoin);
            switch (i)
            {
                case 0: _gs.QteUpgrade++; break;
                case 1: _gs.EnergyUpgrade++; _gs.AddEnergy(1, "upgrade"); break;
                case 2: _gs.ProgressUpgrade++; break;
            }
            Telemetry.Emit("upgrade_buy", "name", i switch { 0 => "QTE", 1 => "Energy", _ => "Progress" }, "newLevel", r.level + 1,
                "coinCost", r.coin, "pointCost", r.points, "coinAfter", _gs.Coin, "pointsAfter", _gs.Points);
            _popups.Add($"{r.name} upgraded!", Palette.Coin, new Vector2(Gfx.W / 2f, 140));
        }
        if (Input.Back) M.Remove(this);
    }

    public override void Draw(SpriteBatch sb)
    {
        Ui.Dim(sb, 0.7f);
        var panel = new Rectangle(280, 110, 720, 520);
        Ui.Panel(sb, panel);
        Gfx.Text(sb, Gfx.Big, "Upgrade", new Vector2(panel.X + Ui.Pad + 6, panel.Y + Ui.Pad), Palette.Warm);
        Gfx.Text(sb, Gfx.Font, $"Points : {_gs.Points}     Coin : {_gs.Coin}", new Vector2(panel.Right - Ui.Pad - 6, panel.Y + Ui.Pad + 8), Palette.Coin, 1f);
        Gfx.Text(sb, Gfx.Small, Gfx.Wrap(Gfx.Small, "Points come from Player level-ups: keep caring for your pets (Train, Feed, Clean, Heal).", panel.Width - 2 * Ui.Pad - 12),
            new Vector2(panel.X + Ui.Pad + 6, panel.Y + 84), Palette.Dim);
        var rows = Rows;
        for (int i = 0; i < 3; i++)
        {
            var r = rows[i];
            int y = 250 + i * 110;
            Gfx.Text(sb, Gfx.Font, r.name, new Vector2(320, y + 2), Palette.Text);
            Gfx.Text(sb, Gfx.Small, r.desc, new Vector2(320, y + 34), Palette.Dim);
            for (int k = 0; k < r.max; k++)
                Gfx.Rect(sb, 600 + k * 26, y + 8, 20, 20, k < r.level ? Palette.Coin : new Color(60, 50, 56));
            bool maxed = r.level >= r.max;
            string req = maxed ? "MAX" : $"REQ. {r.coin} coin . {r.points} {(r.points == 1 ? "point" : "points")}";
            Gfx.Text(sb, Gfx.Small, req, new Vector2(600, y + 38), maxed ? Palette.Coin : Palette.Dim);
            if (!maxed && (_gs.Points < r.points || _gs.Coin < r.coin))
            {
                string why = _gs.Points < r.points ? $"Need {r.points - _gs.Points} more {(r.points - _gs.Points == 1 ? "point" : "points")}" : $"Need {r.coin - _gs.Coin} more coin";
                Gfx.Text(sb, Gfx.Small, why, new Vector2(600, y + 60), Palette.Danger);
            }
            _plus[i].Draw(sb);
        }
        _popups.Draw(sb);
        Ui.Hint(sb, "Click +  to upgrade   |   ESC  Back");
    }
}

/// <summary>Figma Scene 13 — revive fallen pets to HP 1.</summary>
public class DoctorScene : Scene
{
    readonly GameState _gs;
    public GameState State => _gs;
    float _time;

    public override bool Overlay => true;

    public DoctorScene(GameState gs) => _gs = gs;

    List<Pet> Fallen => _gs.Pets.Where(p => p.Dead).ToList();

    public static Rectangle Card(int i, int n) => new(Gfx.W / 2 - (n * 230 + (n - 1) * 20) / 2 + i * 250, 220, 230, 300);

    public override void Update(float dt)
    {
        _time += dt;
        var fallen = Fallen;
        for (int i = 0; i < fallen.Count; i++)
        {
            if (!Input.Click || !Card(i, fallen.Count).Contains(Input.Mouse)) continue;
            var pet = fallen[i];
            bool afford = _gs.Coin >= Balance.ReviveCost;
            M.Push(new ChoiceScene(afford ? $"Are you sure? Revive {pet.Name} for {Balance.ReviveCost} coin." : "You don't have enough coin.",
                new[]
                {
                    new Option("Yes", () =>
                    {
                        _gs.SpendCoin(Balance.ReviveCost, "revive");
                        Audio.Play(Sfx.UiCoin);
                        pet.Hp = 1;
                        Telemetry.Revive(pet, Balance.ReviveCost);
                        M.Remove(this);
                        M.Push(DialogueScene.Say("Doctor", null, $"{pet.Name} is breathing again. Barely. Keep it fed and clean."));
                    }, afford),
                    new Option("No", () => { }),
                }, cancellable: true));
            return;
        }
        if (Input.Back) M.Remove(this);
    }

    public override void Draw(SpriteBatch sb)
    {
        Ui.Dim(sb, 0.75f);
        Art.Doctor(sb, new Vector2(150, 640), _time);
        Gfx.Text(sb, Gfx.Big, "Choose pet", new Vector2(Gfx.W / 2f, 90), Palette.Text, 0.5f);
        Gfx.Text(sb, Gfx.Font, $"Coin : {_gs.Coin}", new Vector2(Gfx.W - 40, 30), Palette.Coin, 1f);
        var fallen = Fallen;
        if (fallen.Count == 0)
            Gfx.Text(sb, Gfx.Font, "\"All your pets are alive. Come back if that changes.\"", new Vector2(Gfx.W / 2f, 330), Palette.Dim, 0.5f);
        for (int i = 0; i < fallen.Count; i++)
        {
            var r = Card(i, fallen.Count);
            bool hover = r.Contains(Input.Mouse);
            Ui.Panel(sb, r);
            if (hover) Gfx.Outline(sb, r, Palette.Warm, 4);
            Art.Pet(sb, fallen[i].Species, fallen[i].Color, new Vector2(r.Center.X, r.Y + 180), 1.1f, _time, Input.Mouse.ToVector2(),
                1f, true, hover);
            Gfx.Text(sb, Gfx.Font, fallen[i].Name, new Vector2(r.Center.X, r.Y + 200), Palette.Text, 0.5f);
            Gfx.Circle(sb, new Vector2(r.Center.X - 34, r.Y + 258), 10, Palette.Coin);
            Gfx.Text(sb, Gfx.Font, $"{Balance.ReviveCost}", new Vector2(r.Center.X - 18, r.Y + 244), Palette.Coin);
        }
        Ui.Hint(sb, "Click a pet to revive   |   ESC  Back");
    }
}

/// <summary>Figma Scene 9-11 — pet discovery (owned pets) and Disaster (seen disasters only).</summary>
public class NotebookScene : Scene
{
    readonly GameState _gs;
    int _tab = -1;   // -1 = main page, 0 = pets, 1 = disaster
    int _page;
    float _time;
    readonly Button _pets = new(new Rectangle(380, 300, 240, 120), "pet discovery");
    readonly Button _disaster = new(new Rectangle(660, 300, 240, 120), "Disaster");

    public override bool Overlay => true;

    public NotebookScene(GameState gs, int tab = -1)
    {
        _gs = gs;
        _tab = tab;
    }

    public override void Update(float dt)
    {
        _time += dt;
        if (_tab == -1)
        {
            _disaster.Enabled = _gs.DisasterSeen;
            if (_pets.Update()) { _tab = 0; _page = 0; }
            else if (_disaster.Update()) { _tab = 1; _page = 0; }
            else if (Input.Back) M.Remove(this);
            return;
        }
        int pages = _tab == 0 ? _gs.Pets.Count : 1;
        if (Input.Pressed(Keys.D) || Input.Pressed(Keys.Right)) _page = Math.Min(_page + 1, pages - 1);
        if (Input.Pressed(Keys.A) || Input.Pressed(Keys.Left)) _page = Math.Max(_page - 1, 0);
        if (Input.Back) _tab = -1;
    }

    public override void Draw(SpriteBatch sb)
    {
        Ui.Dim(sb, 0.7f);
        var book = new Rectangle(160, 90, 960, 560);
        Gfx.Rect(sb, book, new Color(236, 222, 196));
        Gfx.Outline(sb, book, new Color(120, 70, 50), 6);
        int spineTop = _tab == -1 ? book.Y + 190 : book.Y;   // cover page: keep the spine clear of the title
        Gfx.Rect(sb, book.Center.X - 3, spineTop, 6, book.Bottom - spineTop, new Color(190, 170, 140));
        var ink = new Color(60, 40, 36);

        if (_tab == -1)
        {
            Gfx.Text(sb, Gfx.Big, "Survival Notebook", new Vector2(Gfx.W / 2f, 160), ink, 0.5f, false);
            _pets.Draw(sb);
            _disaster.Draw(sb);
            if (!_gs.DisasterSeen)
                Gfx.Text(sb, Gfx.Small, "(no disasters recorded yet)", new Vector2(780, 430), ink, 0.5f, false);
            Ui.Hint(sb, "Click a section   |   ESC  Back");
            return;
        }

        if (_tab == 0)
        {
            var p = _gs.Pets[Math.Min(_page, _gs.Pets.Count - 1)];
            Art.Pet(sb, p.Species, p.Color, new Vector2(400, 420), 1.6f, _time, new Vector2(900, 300), 1f, p.Dead);
            Gfx.Text(sb, Gfx.Big, p.Name, new Vector2(400, 470), ink, 0.5f, false);
            Gfx.Text(sb, Gfx.Font, $"Element : {p.Element}", new Vector2(400, 530), ink, 0.5f, false);
            int x = 680, y = 150;
            Gfx.Text(sb, Gfx.Font, $"LV. {p.Level}   ATK {p.Atk}", new Vector2(x, y), ink, 0, false);
            Gfx.Text(sb, Gfx.Font, $"Health : {p.HpShown}/{p.MaxHp}", new Vector2(x, y + 40), ink, 0, false);
            Gfx.Text(sb, Gfx.Font, $"Stomach : {(int)p.Stomach}", new Vector2(x, y + 80), ink, 0, false);
            Gfx.Text(sb, Gfx.Font, $"Clean : {(int)p.Clean}", new Vector2(x, y + 120), ink, 0, false);
            Gfx.Text(sb, Gfx.Small, Gfx.Wrap(Gfx.Small, p.Description, 380), new Vector2(x, y + 180), ink, 0, false);
            if (p.Passive != null)
            {
                var pass = new Vector2(x, y + 180 + Gfx.Small.MeasureString(Gfx.Wrap(Gfx.Small, p.Description, 380)).Y + 16);
                if (p.PassiveUnlocked)
                {
                    Gfx.Text(sb, Gfx.Font, "Passive", pass, ink, 0, false);
                    Gfx.Text(sb, Gfx.Small, Gfx.Wrap(Gfx.Small, p.Passive, 380), pass + new Vector2(0, 34), ink, 0, false);
                }
                else
                    Gfx.Text(sb, Gfx.Small, $"Passive - unlocks at Level {Balance.PassiveLevel}", pass, ink * 0.55f, 0, false);
            }
            Gfx.Text(sb, Gfx.Small, $"{_page + 1} / {_gs.Pets.Count}", new Vector2(Gfx.W / 2f, 610), ink, 0.5f, false);
        }
        else
        {
            var c = new Vector2(400, 330);
            Gfx.Circle(sb, c, 110, new Color(60, 60, 90));
            Gfx.Ellipse(sb, c + new Vector2(-30, -20), 80, 40, new Color(90, 90, 120));
            Gfx.Line(sb, c + new Vector2(10, 10), c + new Vector2(-10, 60), Palette.Coin, 6);
            Gfx.Line(sb, c + new Vector2(-10, 60), c + new Vector2(14, 70), Palette.Coin, 6);
            Gfx.Line(sb, c + new Vector2(14, 70), c + new Vector2(-6, 120), Palette.Coin, 6);
            Gfx.Text(sb, Gfx.Big, "Thunder storm", new Vector2(400, 480), ink, 0.5f, false);
            Gfx.Text(sb, Gfx.Small, Gfx.Wrap(Gfx.Small,
                "A storm of dust and lightning that tears across the planet without warning. Mud blows in through every crack " +
                "of the sanctuary. Every pet gets filthy (Clean -50). Filthy pets hit softer (Clean 50 or less) and grow frail " +
                "(Clean 25 or less). Clean them before anything else arrives.", 380), new Vector2(680, 160), ink, 0, false);
        }
        Ui.Hint(sb, "A / D  Turn page   |   ESC  Back");
    }
}

/// <summary>Item card shared by the shop and the inventory.</summary>
static class ItemCards
{
    /// <summary>Draws the card into <paramref name="card"/>; offsets scale with the card, the text wraps at a fixed width so it never re-wraps mid-animation.</summary>
    public static void Draw(SpriteBatch sb, ItemInfo it, Rectangle card, int owned)
    {
        float s = card.Width / (float)Items.CardW;
        Ui.Panel(sb, card);
        Gfx.Circle(sb, new Vector2(card.Center.X, card.Y + 42 * s), 26 * s, it.Color);
        Gfx.Circle(sb, new Vector2(card.Center.X - 8 * s, card.Y + 33 * s), 7 * s, Color.White * 0.4f);
        Gfx.Text(sb, Gfx.Font, it.Name, new Vector2(card.Center.X, card.Y + 78 * s), Palette.Text, 0.5f);
        Gfx.Text(sb, Gfx.Small, Gfx.Wrap(Gfx.Small, it.Desc, Items.CardW - 40), new Vector2(card.Center.X, card.Y + 108 * s), Palette.Dim, 0.5f);
        if (owned > 0) Gfx.Text(sb, Gfx.Small, $"x{owned}", new Vector2(card.Right - 14, card.Y + 10), Palette.Coin, 1f);
    }
}

/// <summary>
/// Horizontal item carousel shared by the shop and the inventory.
/// Horizontal Scrub: drag maps 1:1 onto the track (one card stride per <see cref="Balance.TrackStride"/> px); wheel, A/D, arrows and a tap on a card step one card.
/// Staging: the card nearest the centre is the hero (full size, bright, the only clickable one); the rest shrink to 0.9 and dim to 0.4.
/// </summary>
sealed class ItemTrack
{
    static readonly Rectangle Band = new(0, 150, Gfx.W, 290);
    const int BarW = 360, BarY = 470;

    readonly Button _prev = new(new Rectangle(Gfx.W / 2 - BarW / 2 - 84, BarY - 18, 68, 40), "<");
    readonly Button _next = new(new Rectangle(Gfx.W / 2 + BarW / 2 + 16, BarY - 18, 68, 40), ">");
    Smoothed _pos;
    int _focus;
    bool _press, _drag;
    int _pressX;
    float _pressPos;

    public int Focus => _focus;

    /// <summary>Centre card <paramref name="i"/> (snap = no animation, used by the autoplay bot and the shots).</summary>
    public void SetFocus(int i, bool snap = false)
    {
        _focus = Math.Clamp(i, 0, Items.Count - 1);
        if (snap) _pos.Snap(_focus);
    }

    /// <summary>Is card <paramref name="i"/> the one in the middle (and so clickable)?</summary>
    public bool IsHero(int i) => MathF.Abs(i - _pos.Value) < 0.5f;

    public Rectangle Card(int i)
    {
        float d = i - _pos.Value;
        float hero = 1 - MathF.Min(MathF.Abs(d), 1);
        return Items.ScaledCard(Gfx.W / 2f + d * Balance.TrackStride, MathHelper.Lerp(Balance.StageHeroScale, 1f, Ease.OutCubic(hero)));
    }

    int Nearest => Math.Clamp((int)MathF.Round(_pos.Value), 0, Items.Count - 1);

    void Step(int dir)
    {
        int f = Math.Clamp(_focus + dir, 0, Items.Count - 1);
        if (f == _focus) return;
        _focus = f;
        Audio.Play(Sfx.UiClick);
    }

    public void Update(float dt)
    {
        _prev.Enabled = _focus > 0;
        _next.Enabled = _focus < Items.Count - 1;
        if (_prev.Update()) { _focus--; }
        if (_next.Update()) { _focus++; }
        if (Input.Wheel != 0) Step(Input.Wheel > 0 ? -1 : 1);
        if (Input.Pressed(Keys.A) || Input.Pressed(Keys.Left)) Step(-1);
        if (Input.Pressed(Keys.D) || Input.Pressed(Keys.Right)) Step(1);

        if (Input.Click && Band.Contains(Input.Mouse))
        {
            _press = true;
            _drag = false;
            _pressX = Input.Mouse.X;
            _pressPos = _pos.Value;
        }
        if (_press)
        {
            float dx = Input.Mouse.X - _pressX;
            if (!_drag && MathF.Abs(dx) > Balance.TrackDragSlop) _drag = true;
            if (_drag) _pos.Snap(Math.Clamp(_pressPos - dx / Balance.TrackStride, 0, Items.Count - 1));
            if (!Input.MouseHeld)
            {
                _press = false;
                if (_drag) _focus = Nearest;
                else
                    for (int i = 0; i < Items.Count; i++)
                        if (i != Nearest && Card(i).Contains(Input.Mouse)) _focus = i;
                _drag = false;
            }
        }
        if (!_drag) _pos.Update(_focus, Balance.TrackSnapK, dt);
    }

    /// <summary>Dim a card that isn't the hero (draw after the card and its button).</summary>
    public void DrawDim(SpriteBatch sb, int i)
    {
        float hero = 1 - MathF.Min(MathF.Abs(i - _pos.Value), 1);
        float a = (1 - Balance.StageDimAlpha) * (1 - Ease.OutCubic(hero));
        if (a > 0.01f) Ui.Frame(sb, Card(i), Color.Black * a, Color.Transparent);
    }

    /// <summary>Progress cue: arrows, a thumb on a bar and "n / N".</summary>
    public void DrawCue(SpriteBatch sb)
    {
        int x = Gfx.W / 2 - BarW / 2;
        Gfx.Rect(sb, new Rectangle(x, BarY, BarW, 4), Palette.PanelEdge * 0.6f);
        int thumbW = BarW / Items.Count;
        float t = Items.Count > 1 ? Math.Clamp(_pos.Value / (Items.Count - 1), 0f, 1f) : 0f;
        Gfx.Rect(sb, new Rectangle(x + (int)((BarW - thumbW) * t), BarY - 2, thumbW, 8), Palette.Warm);
        _prev.Draw(sb);
        _next.Draw(sb);
        Gfx.Text(sb, Gfx.Small, $"{Nearest + 1} / {Items.Count}", new Vector2(Gfx.W / 2f, BarY + 18), Palette.Text, 0.5f);
        Gfx.Text(sb, Gfx.Small, "Wheel / drag / A D  Browse", new Vector2(Gfx.W / 2f, BarY + 42), Palette.Dim, 0.5f);
    }
}

/// <summary>Day 3 merchant shop (after Sell or Refuse). Items go into the box in the base; use them later from there.</summary>
public class ShopScene : Scene
{
    readonly GameState _gs;
    readonly Button[] _buy = new Button[Items.Count];
    readonly Button _leave = new(new Rectangle(Gfx.W / 2 - 100, 650, 200, 46), "Leave");
    readonly Action? _onClose;
    public GameState State => _gs;
    readonly Popups _popups = new();
    readonly ItemTrack _track = new();
    float _time;

    public override bool Overlay => true;
    public override string? Music => Bgm.Merchant;

    public ShopScene(GameState gs, Action? onClose = null)
    {
        _gs = gs;
        _onClose = onClose;
        for (int i = 0; i < Items.Count; i++) _buy[i] = new Button(Items.HeroButtonRect, $"Buy  {Items.All[i].Price}");
    }

    /// <summary>Put item <paramref name="i"/> in the middle of the track (the autoplay bot and the shots use this before clicking <see cref="BuyRect"/>).</summary>
    public void Focus(int i, bool snap = true) => _track.SetFocus(i, snap);

    /// <summary>Hit box of the Buy button of the centred item (also used by the autoplay bot).</summary>
    public static Rectangle BuyRect => Items.HeroButtonRect;

    void Close()
    {
        M.Remove(this);
        _onClose?.Invoke();
    }

    public override void Update(float dt)
    {
        _time += dt;
        _popups.Update(dt);
        _track.Update(dt);
        for (int i = 0; i < Items.Count; i++)
        {
            var it = Items.All[i];
            _buy[i].Rect = Items.ButtonRect(_track.Card(i));
            _buy[i].Enabled = _track.IsHero(i) && _gs.Coin >= it.Price && _gs.Owned(it.Item) < Balance.ShopMaxOwned;
            if (!_buy[i].Update()) continue;
            _gs.SpendCoin(it.Price, ShopSink(it.Item));
            _gs.AddItem(it.Item);
            Telemetry.Emit("shop_buy", "item", it.Name, "price", it.Price, "target", "");
            Audio.Play(Sfx.UiCoin);
            _popups.Add($"{it.Name} added to your box", Palette.Heal, new Vector2(Gfx.W / 2f, 140));
        }
        if (_leave.Update() || Input.Back) Close();
    }

    static string ShopSink(Item i) => i switch
    {
        Item.CrabApple => "shop_apple",
        Item.CaffeineTonic => "shop_tonic",
        Item.SeaTea => "shop_tea",
        Item.CloudyGlasses => "shop_glasses",
        Item.BalletShoes => "shop_ballet",
        Item.TornNotebook => "shop_notebook",
        _ => "shop_ribbon",
    };

    public override void Draw(SpriteBatch sb)
    {
        Ui.Dim(sb, 0.75f);
        Art.Merchant(sb, new Vector2(1150, 700), _time);
        Gfx.Text(sb, Gfx.Big, "Merchant", new Vector2(Gfx.W / 2f, 24), Palette.Warm, 0.5f);
        Gfx.Text(sb, Gfx.Font, $"Coin : {_gs.Coin}", new Vector2(Gfx.W / 2f, 96), Palette.Coin, 0.5f);
        Gfx.Text(sb, Gfx.Small, $"Bought items go to the box in your base (max {Balance.ShopMaxOwned} of each).", new Vector2(Gfx.W / 2f, 124), Palette.Dim, 0.5f);
        for (int i = 0; i < Items.Count; i++)
        {
            ItemCards.Draw(sb, Items.All[i], _track.Card(i), _gs.Owned(Items.All[i].Item));
            _buy[i].Draw(sb);
            _track.DrawDim(sb, i);
        }
        _track.DrawCue(sb);
        _leave.Draw(sb);
        _popups.Draw(sb);
    }
}

/// <summary>The box in the base: use the items bought from the merchant.</summary>
public class InventoryScene : Scene
{
    readonly GameState _gs;
    readonly Button[] _use = new Button[Items.Count];
    readonly Popups _popups = new();
    readonly ItemTrack _track = new();
    public GameState State => _gs;

    public override bool Overlay => true;

    public InventoryScene(GameState gs)
    {
        _gs = gs;
        for (int i = 0; i < Items.Count; i++) _use[i] = new Button(Items.HeroButtonRect, "Use");
    }

    /// <summary>Put item <paramref name="i"/> in the middle of the track (the autoplay bot uses this before clicking <see cref="UseRect"/>).</summary>
    public void Focus(int i, bool snap = true) => _track.SetFocus(i, snap);

    /// <summary>Hit box of the Use button of the centred item (also used by the autoplay bot).</summary>
    public static Rectangle UseRect => Items.HeroButtonRect;

    public override void Update(float dt)
    {
        _popups.Update(dt);
        _track.Update(dt);
        for (int i = 0; i < Items.Count; i++)
        {
            var it = Items.All[i];
            string? block = Items.UseBlock(_gs, it.Item);
            _use[i].Rect = Items.ButtonRect(_track.Card(i));
            _use[i].Enabled = _track.IsHero(i) && _gs.Owned(it.Item) > 0 && block == null;
            _use[i].Label = _gs.Owned(it.Item) == 0 ? "None" : block ?? "Use";
            if (!_use[i].Update()) continue;
            if (it.Item == Item.CrabApple)
                M.Push(new PetPickScene("Who gets the Crab Apple?", _gs.Pets, p => Use(it.Item, p), onCancel: () => { }));
            else Use(it.Item, null);
        }
        if (Input.Back) M.Remove(this);
    }

    void Use(Item item, Pet? target)
    {
        string msg = Items.Apply(_gs, item, target);
        Audio.Play(Sfx.UiClick);
        _popups.Add(msg, Palette.Heal, new Vector2(Gfx.W / 2f, 120));
    }

    public override void Draw(SpriteBatch sb)
    {
        Ui.Dim(sb, 0.75f);
        Gfx.Text(sb, Gfx.Big, "Box", new Vector2(Gfx.W / 2f, 24), Palette.Warm, 0.5f);
        Gfx.Text(sb, Gfx.Font, $"Energy {_gs.Energy}/{_gs.MaxEnergy}", new Vector2(Gfx.W / 2f, 96), Palette.Warm, 0.5f);
        for (int i = 0; i < Items.Count; i++)
        {
            ItemCards.Draw(sb, Items.All[i], _track.Card(i), _gs.Owned(Items.All[i].Item));
            _use[i].Draw(sb);
            _track.DrawDim(sb, i);
        }
        _track.DrawCue(sb);
        _popups.Draw(sb);
        Ui.Hint(sb, "Click Use   |   ESC  Back");
    }
}
