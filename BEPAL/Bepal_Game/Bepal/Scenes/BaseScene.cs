using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Bepal;

/// <summary>
/// The sanctuary (GDD 06 §2): walk with A/D, Space to interact. Door on the far right.
/// [dark edge] - bed - upgrade - [hearth + pets] - doctor - notebook - [red door]
/// </summary>
public class BaseScene : Scene
{
    public static readonly Backdrop SharedBackdrop = new();

    public const float WorldW = 3840;
    public const float GroundY = 600;
    // Hard walls stop the player; speed into a wall tapers to half between the soft line and the wall
    const float WallLeft = 120, SoftLeft = 630, SoftRight = 3400, WallRight = 3730;
    // The player's centre stops half a sprite width short of each wall so the sprite never pokes through
    static readonly float MinX = WallLeft + Art.PlayerHalfWidth, MaxX = WallRight - Art.PlayerHalfWidth;
    // Big windows: centres 630 + 520*i, between the hanging lamps; last one ends ~160px short of the door
    const int WindowCount = 6;
    const float WindowW = 320, WindowH = 160, WindowTop = GroundY - 210;
    static float WindowX(int i) => 630 + 520 * i;
    static bool InWindow(float x)
    {
        for (int i = 0; i < WindowCount; i++)
            if (x + 8 > WindowX(i) - WindowW / 2 && x < WindowX(i) + WindowW / 2) return true;
        return false;
    }
    public const float BedX = 300, UpgradeX = 900, HearthX = 1900, DoctorX = 2650, BookX = 3100, DoorX = 3640;

    readonly GameState _gs;
    float _playerX = HearthX - 150;
    int _facing = 1;
    float _walkT;
    float _vel;          // px/s, signed; PlayerX stays the physics position
    readonly SpriteMotion _motion = new();   // pose for the player image (single still or primitives)
    float _faceVis = 1;  // smoothed facing for drawing only
    int _lastStep;
    float _camX, _look;
    float _time;
    float _knockT;
    bool _debug;          // F3: position overlay
    public float Night;   // driven by NightScene

    // Prompt: fades in when the nearest spot changes
    (Kind, Pet?) _promptKey;
    float _promptT;
    Smoothed _expShown;

    struct Dust { public Vector2 Pos, Vel; public float Age; }
    readonly Dust[] _dust = new Dust[Balance.DustMax];
    int _dustNext;

    class PetActor
    {
        public float X, Start, Target, Wait, T, Dur, Hop, Phase;   // Phase: fixed per-pet offset so idle bobs aren't in sync
        public bool Moving;
        public int Facing = 1;
        public Smoothed Hp;
    }

    readonly Dictionary<Pet, PetActor> _actors = new();
    readonly Random _rng = RunSeed.Make(2);

    public enum Kind { Bed, Upgrade, Doctor, Book, Door, Pet }

    public record Spot(Kind Kind, float X, string Label, Pet? Pet = null);

    Spot? _near;
    public Spot? Near => _near;
    public float PlayerX => _playerX;
    public float PetX(Pet p) => Actor(p).X;

    public BaseScene(GameState gs)
    {
        _gs = gs;
        _expShown.Snap(gs.PlayerExp);
        for (int i = 0; i < _dust.Length; i++) _dust[i].Age = Balance.DustLife;   // all slots start expired
    }

    /// <summary>Spawn position override (used by the screenshot tool).</summary>
    public float StartX
    {
        init
        {
            _playerX = value;
            _camX = MathHelper.Clamp(value - Gfx.W / 2f, 0, WorldW - Gfx.W);
        }
    }

    public GameState State => _gs;

    PetActor Actor(Pet p)
    {
        if (!_actors.TryGetValue(p, out var a))
        {
            a = new PetActor { X = HearthX + _rng.Next(-300, 300), Phase = (float)_rng.NextDouble() * 6.28f };
            a.Target = a.Start = a.X;
            a.Hp.Snap(p.Hp);
            _actors[p] = a;
        }
        return a;
    }

    IEnumerable<Spot> Spots()
    {
        yield return new Spot(Kind.Bed, BedX, "Sleep (End day)");
        yield return new Spot(Kind.Upgrade, UpgradeX, "Upgrade");
        yield return new Spot(Kind.Doctor, DoctorX, "Doctor");
        yield return new Spot(Kind.Book, BookX, "Notebook");
        yield return new Spot(Kind.Door, DoorX, _gs.DoorEventPending ? "Open the door" : "Red door");
        foreach (var p in _gs.Pets) yield return new Spot(Kind.Pet, Actor(p).X, p.Dead ? $"{p.Name} (fallen)" : $"Care for {p.Name}", p);
    }

    public override void Update(float dt)
    {
        _time += dt;
        Rain.Target = _gs.Day == 4 ? (_gs.DoorDone ? 1f : 0.5f) : 0f;

        // Knocking repeats (clip + a short pause) until the red door is opened
        _knockT -= dt;
        if (!_gs.DoorEventPending) _knockT = 0;
        else if (_knockT <= 0)
        {
            Audio.Play(Sfx.DoorKnock);
            _knockT = Balance.KnockInterval;
        }

        // Walking: accelerate toward the input direction, brake when released, turn around faster than a plain start
        int move = (Input.Down(Keys.D) || Input.Down(Keys.Right) ? 1 : 0) - (Input.Down(Keys.A) || Input.Down(Keys.Left) ? 1 : 0);
        bool run = Input.Down(Keys.LeftShift) || Input.Down(Keys.RightShift);
        float top = run ? Balance.PlayerRunSpeed : Balance.PlayerMaxSpeed;
        float accel = move == 0 ? Balance.PlayerDecel
            : MathF.Sign(_vel) == -move ? Balance.PlayerTurnAccel : Balance.PlayerAccel;
        _vel = Ease.MoveToward(_vel, move * top, accel * dt);
        if (_vel < 0)
            _vel = MathF.Max(_vel, -WallCap(top, (_playerX - MinX) / (SoftLeft - MinX)));
        else if (_vel > 0)
            _vel = MathF.Min(_vel, WallCap(top, (MaxX - _playerX) / (MaxX - SoftRight)));
        if (move != 0) _facing = move;
        _faceVis = Ease.Damp(_faceVis, _facing, Balance.FaceDamp, dt);
        _playerX += _vel * dt;
        if (_playerX < MinX || _playerX > MaxX)
        {
            _playerX = MathHelper.Clamp(_playerX, MinX, MaxX);
            _vel = 0;
        }
        float speed = MathF.Abs(_vel) / Balance.PlayerMaxSpeed;   // 0..1 walking, up to ~1.7 running
        _walkT += dt * speed;
        _motion.Update(dt, _vel, _facing);
        if (MathF.Abs(_vel) > Balance.FootstepMinSpeed) Audio.Loop(Sfx.Footstep, 0.6f);
        UpdateDust(dt, speed);

        // Camera: look ahead of the motion, and only follow once the focus leaves the deadzone
        _look = Ease.Damp(_look, MathHelper.Clamp(_vel * Balance.CamLookScale, -Balance.CamLookMax, Balance.CamLookMax), Balance.CamLookK, dt);
        float focus = _playerX + _look;
        float off = focus - (_camX + Gfx.W / 2f);
        float want = off > Balance.CamDeadzone ? focus - Balance.CamDeadzone - Gfx.W / 2f
            : off < -Balance.CamDeadzone ? focus + Balance.CamDeadzone - Gfx.W / 2f : _camX;
        _camX = MathHelper.Clamp(Ease.Damp(_camX, want, Balance.CamFollowK, dt), 0, WorldW - Gfx.W);

        // Pets wander around the hearth light, easing between spots
        foreach (var p in _gs.Pets)
        {
            var a = Actor(p);
            a.Hp.Update(p.Hp, Balance.BarSmoothK, dt);
            if (p.Dead) continue;
            if (a.Moving)
            {
                a.T += dt;
                float u = a.Dur <= 0 ? 1 : MathF.Min(a.T / a.Dur, 1);
                a.X = MathHelper.Lerp(a.Start, a.Target, Ease.InOutSine(u));
                // One hop per PetHopLen px actually travelled, so hops slow to a stop with the ease instead of fighting it
                a.Hop = MathF.Abs(MathF.Sin(MathF.PI * MathF.Abs(a.X - a.Start) / Balance.PetHopLen)) * Balance.PetHopPx * MathF.Sin(MathF.PI * u);
                if (u >= 1) { a.X = a.Target; a.Moving = false; a.Hop = 0; }
            }
            else
            {
                a.Wait -= dt;
                if (a.Wait <= 0)
                {
                    a.Target = HearthX + _rng.Next(-420, 420);
                    a.Wait = 1.5f + (float)_rng.NextDouble() * 3;
                    if (MathF.Abs(a.Target - a.X) >= 4)
                    {
                        a.Start = a.X;
                        a.T = 0;
                        a.Dur = MathF.Abs(a.Target - a.X) / Balance.PetWalkSpeed * Balance.PetEaseMul;
                        a.Facing = Math.Sign(a.Target - a.X);
                        a.Moving = true;
                    }
                }
            }
        }
        foreach (var gone in _actors.Keys.Where(k => !_gs.Pets.Contains(k)).ToList()) _actors.Remove(gone);

        // Nearest interactable; pets win ties so you can always reach them
        _near = Spots()
            .Select(s => (s, d: MathF.Abs(s.X - _playerX) - (s.Kind == Kind.Pet ? 20 : 0)))
            .Where(t => t.d < (t.s.Kind == Kind.Pet ? 60 : 90))
            .OrderBy(t => t.d)
            .Select(t => t.s)
            .FirstOrDefault();

        // Prompt fade-in restarts whenever the nearest spot changes (pets move, so compare identity, not X)
        var key = (_near?.Kind ?? Kind.Pet, _near?.Pet);
        if (_near == null || key != _promptKey) _promptT = 0;
        _promptKey = key;
        _promptT += dt;

        if (_gs.PlayerExp < _expShown.Value) _expShown.Snap(_gs.PlayerExp);   // level-up wrapped the bar
        _expShown.Update(_gs.PlayerExp, Balance.BarSmoothK, dt);

        if (Input.Pressed(Keys.F3)) _debug = !_debug;
        if (Input.Pressed(Keys.Space) && _near != null) Interact(_near);
        if (Input.Back)
        {
            _vel = 0;
            M.Push(new ChoiceScene("Paused", new[]
            {
                new Option("Resume", () => { }),
                new Option("Main Menu", () => M.Reset(new MainMenuScene())),
            }, cancellable: true));
        }
    }

    static float WallCap(float top, float dist) => top * (0.5f + 0.5f * Ease.Smoothstep(dist));

    void UpdateDust(float dt, float speed)
    {
        for (int i = 0; i < _dust.Length; i++)
        {
            if (_dust[i].Age >= Balance.DustLife) continue;
            _dust[i].Age += dt;
            _dust[i].Pos += _dust[i].Vel * dt;
            _dust[i].Vel *= 1 - MathF.Min(1, 4 * dt);
        }
        // One puff per foot-down (each half of the walk cycle) once moving fast enough
        int step = (int)MathF.Floor(_walkT * 12f / MathF.PI);
        if (step == _lastStep) return;
        _lastStep = step;
        if (speed <= Balance.DustMinSpeed) return;
        int dir = MathF.Sign(_vel) < 0 ? 1 : -1;   // kicked up behind the feet
        _dust[_dustNext] = new Dust
        {
            Pos = new Vector2(_playerX + (step % 2 == 0 ? -6 : 6), GroundY + 4),
            Vel = new Vector2(dir * (18 + 6 * (step % 3)), -(14 + 4 * (step % 4))),
        };
        _dustNext = (_dustNext + 1) % _dust.Length;
    }

    void DrawDust(SpriteBatch sb)
    {
        foreach (var d in _dust)
        {
            if (d.Age >= Balance.DustLife) continue;
            float t = d.Age / Balance.DustLife;
            Gfx.Circle(sb, new Vector2(Sx(d.Pos.X), d.Pos.Y), 3 + 4 * t, new Color(150, 124, 104) * (0.4f * (1 - t)));
        }
    }

    void Interact(Spot s)
    {
        _vel = 0;
        Audio.Play(Sfx.Interact);
        switch (s.Kind)
        {
            case Kind.Bed:
                if (_gs.DoorEventPending)
                    M.Push(DialogueScene.Say("", null, "Knock... knock... Something is at the red door. You can't sleep with that noise."));
                else
                    M.Push(new ChoiceScene($"End Day {_gs.Day} and go to sleep?", new[]
                    {
                        new Option("Sleep", () => M.Push(new NightScene(this))),
                        new Option("Not yet", () => { }),
                    }, cancellable: true));
                break;
            case Kind.Upgrade:
                M.Push(new UpgradeScene(_gs));
                break;
            case Kind.Doctor:
                M.Push(new DoctorScene(_gs));
                break;
            case Kind.Book:
                M.Push(new NotebookScene(_gs));
                break;
            case Kind.Door:
                if (_gs.Day == 1)
                    M.Push(DialogueScene.Say("", null, "It's quiet outside. Take today to get to know your pet."));
                else if (!_gs.DoorEventPending)
                    M.Push(DialogueScene.Say("", null, "Nothing else is out there today... you hope."));
                else DayEvents.OpenDoor(M, _gs);
                break;
            case Kind.Pet:
                if (s.Pet!.Dead)
                    M.Push(DialogueScene.Say("", null, $"{s.Pet.Name} isn't moving. The Doctor might be able to help."));
                else M.Push(new CareSelectScene(_gs, s.Pet));
                break;
        }
    }

    float Sx(float worldX) => worldX - _camX;

    public override void Draw(SpriteBatch sb)
    {
        float night = MathHelper.Clamp(Night, 0, 1);
        SharedBackdrop.Draw(sb, _camX, night, _time, GroundY);
        Rain.DrawSky(sb);

        DrawStructure(sb, night);

        // Hearth light
        var hearth = new Vector2(Sx(HearthX), GroundY - 40);
        float flicker = 0.9f + 0.1f * MathF.Sin(_time * 9) * MathF.Sin(_time * 5.3f);
        Gfx.BeginAdditive(sb);
        Gfx.Glow(sb, hearth, 620 * flicker, 360 * flicker, new Color(255, 150, 70) * (0.28f + 0.2f * night));
        Gfx.Glow(sb, hearth, 180, 140, new Color(255, 190, 110) * 0.55f);
        Gfx.EndAdditive(sb);
        Gfx.Rect(sb, hearth.X - 50, GroundY - 26, 100, 26, new Color(70, 60, 60));
        for (int i = 0; i < 3; i++)
            Gfx.Ellipse(sb, hearth + new Vector2(-18 + i * 18, -4 - MathF.Abs(MathF.Sin(_time * 7 + i)) * 10), 12, 26,
                Color.Lerp(new Color(255, 120, 40), new Color(255, 220, 120), i % 2));

        DrawObjects(sb);

        // Pets
        var playerHead = new Vector2(Sx(_playerX), GroundY - 90);
        foreach (var p in _gs.Pets)
        {
            var a = Actor(p);
            var feet = new Vector2(Sx(a.X), GroundY + 6 - a.Hop);
            bool hl = _near?.Pet == p;
            Art.Pet(sb, p.Species, p.Color, feet, 0.8f, _time + a.Phase, playerHead, 1f, p.Dead, hl);
            if (hl) Gfx.Text(sb, Gfx.Small, p.Name, new Vector2(feet.X, feet.Y - 132), Palette.Warm, 0.5f);
            Ui.Bar(sb, new Rectangle((int)feet.X - 30, (int)GroundY + 6 - 86, 60, 8), a.Hp.Value, p.MaxHp, Palette.Hp);
        }

        DrawDust(sb);
        var feetP = new Vector2(Sx(_playerX), GroundY + 6);
        var pose = _motion.Pose;
        if (Art.PlayerSprite != null)
            Art.PlayerImage(sb, feetP, pose, _motion.Face);
        else
            Art.Player(sb, feetP, _facing, _walkT,
                lean: pose.Rot * 92f, bob: -pose.Offset.Y, faceScale: _motion.Face,
                breath: (pose.Scale.Y - 1) * 32f);

        // Ground, then the sprite foreground (parallax 1.3) in front of everything
        Gfx.Rect(sb, 0, GroundY, Gfx.W, Gfx.H - GroundY, Gfx.Lerp(new Color(58, 40, 34), new Color(24, 18, 22), night));
        Gfx.Rect(sb, 0, GroundY, Gfx.W, 4, Gfx.Lerp(new Color(110, 74, 56), new Color(44, 32, 36), night));
        SharedBackdrop.DrawForeground(sb, _camX, night);

        // Darkness beyond the walls
        EdgeDark(sb, Sx(WallLeft), -1);
        EdgeDark(sb, Sx(DoorX + 80), 1);
        Gfx.Rect(sb, 0, 0, Gfx.W, Gfx.H, new Color(10, 6, 24) * (night * 0.35f));
        Rain.DrawFlash(sb);

        // Interaction prompt
        if (_near != null && M.Top == this)
        {
            float fade = Ease.OutCubic(_promptT / Balance.PromptFadeTime);
            var pos = new Vector2(Sx(_near.X), GroundY - (_near.Kind == Kind.Pet ? 150 : 200) + MathF.Sin(_time * 3.2f) * 4);
            string text = $"[Space] {_near.Label}";
            var size = Gfx.Small.MeasureString(text);
            Gfx.Rect(sb, pos.X - size.X / 2 - 10, pos.Y - 4, size.X + 20, size.Y + 8, Color.Black * (0.6f * fade));
            Gfx.Text(sb, Gfx.Small, text, new Vector2(pos.X, pos.Y), Palette.Warm * fade, 0.5f);
            if (_near.Pet is { } np) CareSelectScene.PetStats(sb, np, new Rectangle(20, 470, 250, 210));
        }

        DrawHud(sb);
        if (_debug) DrawDebug(sb);
    }

    /// <summary>F3 overlay: player/camera numbers plus world-space markers for walls, spots and interaction range.</summary>
    void DrawDebug(SpriteBatch sb)
    {
        void VLine(float worldX, Color c, float w = 2) => Gfx.Rect(sb, Sx(worldX) - w / 2, 0, w, Gfx.H, c);
        VLine(WallLeft, Color.Red * 0.6f);
        VLine(WallRight, Color.Red * 0.6f);
        VLine(MinX, Color.Yellow * 0.6f, 1);   // where the player's centre actually stops
        VLine(MaxX, Color.Yellow * 0.6f, 1);
        VLine(SoftLeft, Color.Orange * 0.6f);
        VLine(SoftRight, Color.Orange * 0.6f);
        foreach (var s in Spots())
        {
            float r = s.Kind == Kind.Pet ? 80 : 90;   // pets: 60 reach + 20 bonus (see _near)
            Gfx.Rect(sb, Sx(s.X - r), GroundY + 20, r * 2, 6, (s == _near ? Color.Lime : Color.Cyan) * 0.5f);
            VLine(s.X, Color.Cyan * 0.5f, 1);
        }
        VLine(_playerX, Color.Magenta * 0.8f);

        string[] lines =
        {
            $"PlayerX {_playerX:0.0}  vel {_vel:0.0}  face {_facing}",
            $"CamX {_camX:0.0}  look {_look:0.0}  screenX {Sx(_playerX):0.0}",
            $"Near {(_near == null ? "-" : _near.Kind + " @ " + _near.X.ToString("0"))}",
            $"World {WorldW:0}  wall {WallLeft:0}..{WallRight:0}  soft {SoftLeft:0}/{SoftRight:0}",
        };
        var box = new Rectangle(10, 10, 560, 20 + lines.Length * 22);
        Gfx.Rect(sb, box, Color.Black * 0.7f);
        for (int i = 0; i < lines.Length; i++)
            Gfx.Text(sb, Gfx.Small, lines[i], new Vector2(box.X + 10, box.Y + 10 + i * 22), Color.White, 0f);
    }

    void EdgeDark(SpriteBatch sb, float edgeX, int dir)
    {
        const int steps = 16;
        for (int i = 0; i < steps; i++)
        {
            float w = 18;
            float x = dir < 0 ? edgeX - (i + 1) * w : edgeX + i * w;
            Gfx.Rect(sb, x, 0, w + 1, Gfx.H, Color.Black * ((i + 1) / (float)steps * 0.85f));
        }
        if (dir < 0) Gfx.Rect(sb, -10, 0, MathF.Max(0, edgeX - steps * 18 + 10), Gfx.H, Color.Black * 0.85f);
        else Gfx.Rect(sb, edgeX + steps * 18, 0, Gfx.W, Gfx.H, Color.Black * 0.85f);
    }

    void DrawStructure(SpriteBatch sb, float night)
    {
        // Back wall of the sanctuary
        float l = Sx(WallLeft), r = Sx(WallRight);
        var wall = Gfx.Lerp(new Color(86, 58, 52), new Color(40, 28, 32), night);
        float wallTop = GroundY - 230;
        var panel = Color.Lerp(wall, Color.Black, 0.25f);
        var frame = Color.Lerp(wall, Color.Black, 0.55f);
        // Wall in segments so the big windows leave the backdrop visible
        float cursor = l;
        for (int i = 0; i < WindowCount; i++)
        {
            float wx = Sx(WindowX(i)) - WindowW / 2;
            Gfx.Rect(sb, cursor, wallTop, wx - cursor, 230, wall);
            Gfx.Rect(sb, wx, wallTop, WindowW, WindowTop - wallTop, wall);                            // above the glass
            Gfx.Rect(sb, wx, WindowTop + WindowH, WindowW, GroundY - WindowTop - WindowH, wall);      // below the glass
            cursor = wx + WindowW;
        }
        Gfx.Rect(sb, cursor, wallTop, r - cursor, 230, wall);
        Gfx.Rect(sb, l - 20, GroundY - 250, r - l + 40, 22, Color.Lerp(wall, Color.Black, 0.35f));   // roof beam
        for (float x = WallLeft; x < WallRight; x += 160)
            if (!InWindow(x)) Gfx.Rect(sb, Sx(x), wallTop, 8, 230, panel);                           // wall panels
        // Big windows onto the outside (kept clear of the door)
        for (int i = 0; i < WindowCount; i++)
        {
            float wx = Sx(WindowX(i)) - WindowW / 2;
            if (wx > Gfx.W || wx + WindowW < 0) continue;
            Gfx.Rect(sb, wx - 10, WindowTop - 10, WindowW + 20, 10, frame);                          // frame: top, bottom, left, right
            Gfx.Rect(sb, wx - 10, WindowTop + WindowH, WindowW + 20, 10, frame);
            Gfx.Rect(sb, wx - 10, WindowTop, 10, WindowH, frame);
            Gfx.Rect(sb, wx + WindowW, WindowTop, 10, WindowH, frame);
            Gfx.Rect(sb, wx, WindowTop, WindowW, WindowH, Color.White * (0.06f * (1 - night)));       // faint glass tint
            Gfx.Rect(sb, wx + WindowW / 2 - 4, WindowTop, 8, WindowH, frame);                         // mullion
            Gfx.Rect(sb, wx, WindowTop + WindowH / 2 - 4, WindowW, 8, frame);                         // transom
            Gfx.Rect(sb, wx - 14, WindowTop + WindowH + 10, WindowW + 28, 8, panel);                  // sill
        }
        // Hanging lamps
        for (float x = WallLeft + 250; x < WallRight; x += 520)
        {
            var c = new Vector2(Sx(x), GroundY - 190);
            Gfx.Line(sb, c - new Vector2(0, 38), c, new Color(30, 24, 26), 2);
            Gfx.Glow(sb, c, 80, 80, Palette.Warm * 0.35f);
            Gfx.Circle(sb, c, 9, Palette.Warm);
        }
    }

    void DrawObjects(SpriteBatch sb)
    {
        float g = GroundY;
        // Bed
        float bx = Sx(BedX);
        Gfx.Rect(sb, bx - 90, g - 50, 180, 50, new Color(120, 80, 60));
        Gfx.Rect(sb, bx - 90, g - 70, 180, 26, new Color(200, 170, 150));
        Gfx.Rect(sb, bx - 90, g - 90, 50, 40, new Color(236, 220, 200));
        // Upgrade console
        float ux = Sx(UpgradeX);
        Gfx.Rect(sb, ux - 50, g - 150, 100, 150, new Color(60, 70, 84));
        Gfx.Rect(sb, ux - 38, g - 136, 76, 50, new Color(40, 200, 170) * (0.6f + 0.2f * MathF.Sin(_time * 2)));
        Gfx.Text(sb, Gfx.Small, "UPGRADE", new Vector2(ux, g - 70), new Color(160, 240, 220), 0.5f);
        if (_gs.Points > 0) Gfx.Circle(sb, new Vector2(ux + 50, g - 150), 10, Palette.Coin);
        // Doctor
        Art.Doctor(sb, new Vector2(Sx(DoctorX), g + 6), _time);
        Gfx.Rect(sb, Sx(DoctorX) + 40, g - 60, 80, 60, new Color(200, 200, 210));
        if (_gs.Pets.Any(p => p.Dead)) Gfx.Text(sb, Gfx.Font, "+", new Vector2(Sx(DoctorX), g - 170), Palette.Danger, 0.5f);
        // Notebook desk
        float nx = Sx(BookX);
        Gfx.Rect(sb, nx - 60, g - 70, 120, 12, new Color(110, 76, 56));
        Gfx.Rect(sb, nx - 52, g - 58, 10, 58, new Color(90, 62, 46));
        Gfx.Rect(sb, nx + 42, g - 58, 10, 58, new Color(90, 62, 46));
        Gfx.Rect(sb, nx - 30, g - 84, 60, 14, new Color(180, 60, 60));
        Gfx.Rect(sb, nx - 26, g - 82, 52, 3, new Color(240, 230, 210));
        // The red door
        float dx = Sx(DoorX);
        Gfx.Rect(sb, dx - 90, g - 300, 180, 300, new Color(40, 30, 34));
        Gfx.Rect(sb, dx - 72, g - 282, 144, 282, new Color(170, 30, 38));
        Gfx.Rect(sb, dx - 4, g - 282, 8, 282, new Color(110, 18, 24));
        Gfx.Circle(sb, new Vector2(dx + 50, g - 140), 7, Palette.Coin);
        if (_gs.DoorEventPending)
        {
            float pulse = 0.5f + 0.5f * MathF.Sin(_time * 8);
            Gfx.BeginAdditive(sb);
            Gfx.Glow(sb, new Vector2(dx, g - 150), 220, 260, Palette.Danger * (0.3f + 0.3f * pulse));
            Gfx.EndAdditive(sb);
            float shake = MathF.Sin(_time * 40) * 3 * pulse;
            Gfx.Text(sb, Gfx.Font, "Knock Knock !!", new Vector2(dx + shake, g - 360), Palette.Danger, 0.5f);
        }
    }

    void DrawHud(SpriteBatch sb)
    {
        var r = new Rectangle(16, 14, 560, 64);
        Ui.Panel(sb, r, 0.85f);
        Gfx.Text(sb, Gfx.Font, $"Day {_gs.Day}", new Vector2(r.X + 16, r.Y + 8), Palette.Warm);
        Gfx.Circle(sb, new Vector2(r.X + 118, r.Y + 22), 9, Palette.Coin);
        Gfx.Text(sb, Gfx.Font, $"{_gs.Coin}", new Vector2(r.X + 134, r.Y + 8), Palette.Coin);
        Gfx.Text(sb, Gfx.Small, "Energy", new Vector2(r.X + 16, r.Y + 38), Palette.Dim);
        for (int i = 0; i < _gs.MaxEnergy; i++)
            Gfx.Rect(sb, r.X + 96 + i * 20, r.Y + 41, 14, 14, i < _gs.Energy ? new Color(120, 240, 140) : new Color(50, 60, 50));
        Gfx.Text(sb, Gfx.Font, $"Player LV. {_gs.PlayerLevel}", new Vector2(r.X + 240, r.Y + 8), Palette.Text);
        Ui.Bar(sb, new Rectangle(r.X + 240, r.Y + 40, 180, 14), _expShown.Value, _gs.PlayerMaxExp, Palette.Progress);
        Gfx.Text(sb, Gfx.Small, $"Points {_gs.Points}", new Vector2(r.X + 432, r.Y + 36), Palette.Dim);

        if (_near == null && M.Top == this) Ui.Hint(sb, "A / D  Walk     SHIFT  Run     SPACE  Interact     ESC  Pause");
    }
}
