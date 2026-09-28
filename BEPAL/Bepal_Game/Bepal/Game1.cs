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
            _scenes.ResetNow(new MainMenuScene());

            var args = System.Environment.GetCommandLineArgs();
            int i = System.Array.IndexOf(args, "--shots");
            if (i >= 0 && i + 1 < args.Length) _shots = new ShotRunner(args[i + 1]);
            if (System.Array.IndexOf(args, "--autoplay") >= 0)
                _bot = new AutoPlay(_scenes, refuseMerchant: System.Array.IndexOf(args, "--refuse") >= 0);
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
                    _scenes.Update(1 / 60f);
                }
                if (_bot.Finished)
                {
                    var log = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "bepal_autoplay.log");
                    System.IO.File.WriteAllLines(log, _bot.Log);
                    System.Console.WriteLine(log);
                    Exit();
                }
                return;
            }
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Input.Update();
            Gfx.UpdateShake(dt);
            _scenes.Update(dt);
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
