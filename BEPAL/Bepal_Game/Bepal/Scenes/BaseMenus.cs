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
        ("QTE", "Train dots get bigger", Balance.QteCoin, Balance.QtePoints, _gs.QteUpgrade, Balance.QteMax),
        ("Energy", "+1 Energy every day", Balance.EnergyCoin, Balance.EnergyPoints, _gs.EnergyUpgrade,
            Balance.MaxEnergyCap - Balance.BaseEnergy),
        ("Progress Bar", "Train progress +10 -> +15", Balance.ProgressCoin, Balance.ProgressPoints, _gs.ProgressUpgrade,
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
        Gfx.Text(sb, Gfx.Big, "Upgrade", new Vector2(panel.X + 30, panel.Y + 10), Palette.Warm);
        Gfx.Text(sb, Gfx.Font, $"Points : {_gs.Points}     Coin : {_gs.Coin}", new Vector2(panel.Right - 30, panel.Y + 30), Palette.Coin, 1f);
        var rows = Rows;
        for (int i = 0; i < 3; i++)
        {
            var r = rows[i];
            int y = 250 + i * 110;
            Gfx.Text(sb, Gfx.Font, r.name, new Vector2(320, y + 2), Palette.Text);
            Gfx.Text(sb, Gfx.Small, r.desc, new Vector2(320, y + 34), Palette.Dim);
            for (int k = 0; k < r.max; k++)
                Gfx.Rect(sb, 600 + k * 26, y + 8, 20, 20, k < r.level ? Palette.Coin : new Color(60, 50, 56));
            string req = r.level >= r.max ? "MAX" : $"REQ. {r.coin} coin . {r.points} points";
            Gfx.Text(sb, Gfx.Small, req, new Vector2(600, y + 38), r.level >= r.max ? Palette.Coin : Palette.Dim);
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
        Gfx.Rect(sb, book.Center.X - 3, book.Y, 6, book.Height, new Color(190, 170, 140));
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

/// <summary>Day 3 merchant shop (only after refusing to sell). Items take effect on purchase.</summary>
public class ShopScene : Scene
{
    readonly GameState _gs;
    readonly Button[] _buy = new Button[3];
    readonly Button _leave = new(new Rectangle(Gfx.W / 2 - 100, 600, 200, 50), "Leave");
    public GameState State => _gs;
    readonly Popups _popups = new();
    float _time;

    public override bool Overlay => true;

    record Item(string Name, int Price, string Desc, Color Color);

    static readonly Item[] Items =
    {
        new("Crab Apple", 25, "One pet: +18 HP, +20 Stomach", new Color(220, 70, 60)),
        new("Caffeine Tonic", 40, "+2 Energy today", new Color(230, 160, 60)),
        new("Sea Tea", 18, "Dodge zone +20% in your next fight", new Color(90, 190, 220)),
    };

    public ShopScene(GameState gs)
    {
        _gs = gs;
        for (int i = 0; i < 3; i++) _buy[i] = new Button(BuyRect(i), $"Buy  {Items[i].Price}");
    }

    /// <summary>Hit box of the Buy button of item <paramref name="i"/> (also used by the autoplay bot).</summary>
    public static Rectangle BuyRect(int i) => new(250 + i * 280, 470, 220, 50);

    public override void Update(float dt)
    {
        _time += dt;
        _popups.Update(dt);
        _buy[0].Enabled = _gs.Coin >= Items[0].Price && _gs.Alive.Any();
        _buy[1].Enabled = _gs.Coin >= Items[1].Price;
        _buy[2].Enabled = _gs.Coin >= Items[2].Price && !_gs.SeaTea;

        if (_buy[0].Update())
            M.Push(new PetPickScene("Who gets the Crab Apple?", _gs.Pets, p =>
            {
                _gs.SpendCoin(Items[0].Price, "shop_apple");
                Telemetry.Emit("shop_buy", "item", "Crab Apple", "price", Items[0].Price, "target", p.Name);
                Audio.Play(Sfx.UiCoin);
                p.Hp += 18;
                p.Stomach += 20;
                p.ClampStats();
                _popups.Add($"{p.Name} munches happily", Palette.Heal, new Vector2(Gfx.W / 2f, 150));
            }, onCancel: () => { }));
        if (_buy[1].Update())
        {
            _gs.SpendCoin(Items[1].Price, "shop_tonic");
            Telemetry.Emit("shop_buy", "item", "Caffeine Tonic", "price", Items[1].Price, "target", "");
            Audio.Play(Sfx.UiCoin);
            _gs.AddEnergy(2, "tonic");
            _popups.Add("+2 Energy!", Palette.Heal, new Vector2(Gfx.W / 2f, 150));
        }
        if (_buy[2].Update())
        {
            _gs.SpendCoin(Items[2].Price, "shop_tea");
            Telemetry.Emit("shop_buy", "item", "Sea Tea", "price", Items[2].Price, "target", "");
            Audio.Play(Sfx.UiCoin);
            _gs.SeaTea = true;
            _popups.Add("Sea Tea ready for the next fight", Palette.Clean, new Vector2(Gfx.W / 2f, 150));
        }
        if (_leave.Update() || Input.Back) M.Remove(this);
    }

    public override void Draw(SpriteBatch sb)
    {
        Ui.Dim(sb, 0.75f);
        Art.Merchant(sb, new Vector2(1150, 700), _time);
        Gfx.Text(sb, Gfx.Big, "Merchant", new Vector2(Gfx.W / 2f, 60), Palette.Warm, 0.5f);
        Gfx.Text(sb, Gfx.Font, $"Coin : {_gs.Coin}", new Vector2(Gfx.W / 2f, 130), Palette.Coin, 0.5f);
        for (int i = 0; i < 3; i++)
        {
            var it = Items[i];
            var card = new Rectangle(240 + i * 280, 200, 240, 330);
            Ui.Panel(sb, card);
            Gfx.Circle(sb, new Vector2(card.Center.X, card.Y + 90), 44, it.Color);
            Gfx.Circle(sb, new Vector2(card.Center.X - 14, card.Y + 74), 12, Color.White * 0.4f);
            Gfx.Text(sb, Gfx.Font, it.Name, new Vector2(card.Center.X, card.Y + 150), Palette.Text, 0.5f);
            Gfx.Text(sb, Gfx.Small, Gfx.Wrap(Gfx.Small, it.Desc, 200), new Vector2(card.Center.X, card.Y + 190), Palette.Dim, 0.5f);
            _buy[i].Draw(sb);
        }
        _leave.Draw(sb);
        _popups.Draw(sb);
    }
}
