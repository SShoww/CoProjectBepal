using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bepal
{
    public class Game1 : Game
    {
        public static Game1 Instance = null!;

        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch = null!;
        private readonly SceneManager _scenes = new();
        private ShotRunner? _shots;
        private AutoPlay? _bot;

        public Game1()
        {
            Instance = this;
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.Title = "BePal";
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = Gfx.W;
            _graphics.PreferredBackBufferHeight = Gfx.H;
            _graphics.ApplyChanges();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            Gfx.Init(GraphicsDevice, Content);

            var args = System.Environment.GetCommandLineArgs();
            string? Arg(string name)
            {
                int k = System.Array.IndexOf(args, name);
                return k >= 0 && k + 1 < args.Length ? args[k + 1] : null;
            }
            bool Has(string name) => System.Array.IndexOf(args, name) >= 0;

            string? botArg = Arg("--bot");
            bool auto = Has("--autoplay") || botArg != null;
            Audio.Muted = auto || Has("--shots");
            Audio.Load(Content);
            if (int.TryParse(Arg("--seed"), out int seed)) RunSeed.Set(seed);
            _scenes.ResetNow(new MainMenuScene());

            string? shotsDir = Arg("--shots");
            if (shotsDir != null) _shots = new ShotRunner(shotsDir);
            if (auto)
                _bot = new AutoPlay(_scenes, refuseMerchant: Has("--refuse"), profile: botArg ?? "perfect", seed: RunSeed.Seed);
            string? telDir = Arg("--telemetry");
            string player = Arg("--player") ?? "";
            // Tester build: a telemetry.txt next to the exe turns telemetry on for a plain double-click
            // (first line that is not blank or "#..." = player name; files go to <exe dir>/data).
            string telFile = System.IO.Path.Combine(System.AppContext.BaseDirectory, "telemetry.txt");
            if (telDir == null && _shots == null && System.IO.File.Exists(telFile))
            {
                telDir = System.IO.Path.Combine(System.AppContext.BaseDirectory, "data");
                if (player == "")
                    player = System.Linq.Enumerable.FirstOrDefault(System.IO.File.ReadLines(telFile), l => l.Trim() != "" && !l.Trim().StartsWith('#'))?.Trim() ?? "";
            }
            if (telDir != null && _shots == null)
            {
                Telemetry.Configure(telDir, _bot?.Profile ?? "human", auto ? "autoplay" : "play", Has("--refuse"), player);
                Telemetry.SceneProbe = () => _scenes.Top?.GetType().Name ?? "";
            }
        }

        protected override void OnExiting(object sender, ExitingEventArgs args)
        {
            Telemetry.End(_bot is { Failed: true } ? "timeout" : "quit");
            Telemetry.Flush();
            base.OnExiting(sender, args);
        }

        protected override void Update(GameTime gameTime)
        {
            if (_shots != null)
            {
                if (!_shots.Step(GraphicsDevice, _spriteBatch)) Exit();
                return;
            }
            if (_bot != null)
            {
                // Run fast: many simulated 60 FPS frames per real frame
                for (int n = 0; n < 60 && !_bot.Finished; n++)
                {
                    _bot.Step();
                    Gfx.UpdateShake(1 / 60f);
                    Telemetry.Tick(1 / 60f);
                    _scenes.Update(1 / 60f);
                }
                if (_bot.Finished)
                {
                    if (_bot.Failed) Telemetry.End("timeout");
                    string suffix = _bot.CustomRun ? $"_{_bot.Profile}_{RunSeed.Seed}" : "";
                    var log = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"bepal_autoplay{suffix}.log");
                    System.IO.File.WriteAllLines(log, _bot.Log);
                    System.Console.WriteLine(log);
                    Exit();
                }
                return;
            }
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Input.Update();
            Gfx.UpdateShake(dt);
            Telemetry.Tick(dt);
            _scenes.Update(dt);
            Audio.EndFrame(dt);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            Gfx.Begin(_spriteBatch);
            _scenes.Draw(_spriteBatch);
            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
