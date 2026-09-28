using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Bepal;

public static class Input
{
    static KeyboardState _k, _pk;
    static MouseState _m, _pm;

    public static void Update()
    {
        _pk = _k;
        _pm = _m;
        _k = Keyboard.GetState();
        _m = Microsoft.Xna.Framework.Input.Mouse.GetState();
    }

    /// <summary>Dev tool: replace this frame's input (used by the autoplay bot).</summary>
    public static void Inject(Keys[] down, Point? click = null)
    {
        _pk = _k;
        _pm = _m;
        _k = new KeyboardState(down);
        var pos = click ?? Point.Zero;
        _m = new MouseState(pos.X, pos.Y, 0, click.HasValue ? ButtonState.Pressed : ButtonState.Released,
            ButtonState.Released, ButtonState.Released, ButtonState.Released, ButtonState.Released);
    }

    /// <summary>Swallow this frame's presses so they don't leak into a newly opened scene.</summary>
    public static void Consume()
    {
        _pk = _k;
        _pm = _m;
    }

    public static bool Down(Keys key) => _k.IsKeyDown(key);
    public static bool Pressed(Keys key) => _k.IsKeyDown(key) && !_pk.IsKeyDown(key);
    public static Point Mouse => _m.Position;
    public static bool Click => _m.LeftButton == ButtonState.Pressed && _pm.LeftButton == ButtonState.Released;

    public static bool Confirm => Pressed(Keys.Space) || Pressed(Keys.Enter);
    public static bool Back => Pressed(Keys.Escape);
}
