using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bepal;

public abstract class Scene
{
    public SceneManager M = null!;

    /// <summary>Overlays are drawn on top of the scene below them (which keeps drawing but stops updating).</summary>
    public virtual bool Overlay => false;

    public virtual void Enter() { }
    public abstract void Update(float dt);
    public abstract void Draw(SpriteBatch sb);
}

/// <summary>Scene stack with fade transitions for full-screen changes.</summary>
public class SceneManager
{
    readonly List<Scene> _stack = new();
    Scene? _pending;
    Scene[] _pendingOverlays = Array.Empty<Scene>();
    float _fade;          // 0 = clear, 1 = black
    int _fadeDir;         // 1 = fading out, -1 = fading in

    public Scene? Top => _stack.Count > 0 ? _stack[^1] : null;

    public void Push(Scene s)
    {
        s.M = this;
        _stack.Add(s);
        s.Enter();
        Input.Consume();
    }

    public void Pop()
    {
        if (_stack.Count > 0) _stack.RemoveAt(_stack.Count - 1);
        Input.Consume();
    }

    /// <summary>Remove <paramref name="s"/> wherever it is in the stack.</summary>
    public void Remove(Scene s)
    {
        _stack.Remove(s);
        Input.Consume();
    }

    /// <summary>Clear the stack and show <paramref name="s"/> (plus any overlays on top of it) after a fade.</summary>
    public void Reset(Scene s, params Scene[] overlays)
    {
        _pending = s;
        _pendingOverlays = overlays;
        _fadeDir = 1;
    }

    public void ResetNow(Scene s)
    {
        _stack.Clear();
        Push(s);
    }

    public void Update(float dt)
    {
        if (_fadeDir != 0)
        {
            _fade += _fadeDir * dt * 2.5f;
            if (_fadeDir == 1 && _fade >= 1)
            {
                _fade = 1;
                _stack.Clear();
                if (_pending != null) Push(_pending);
                foreach (var o in _pendingOverlays) Push(o);
                _pending = null;
                _pendingOverlays = Array.Empty<Scene>();
                _fadeDir = -1;
            }
            else if (_fadeDir == -1 && _fade <= 0)
            {
                _fade = 0;
                _fadeDir = 0;
            }
            if (_fadeDir == 1) return;
        }
        Top?.Update(dt);
    }

    public void Draw(SpriteBatch sb)
    {
        int first = _stack.Count - 1;
        while (first > 0 && _stack[first].Overlay) first--;
        for (int i = Math.Max(first, 0); i < _stack.Count; i++) _stack[i].Draw(sb);
        if (_fade > 0) Gfx.Rect(sb, -20, -20, Gfx.W + 40, Gfx.H + 40, Color.Black * _fade);
    }
}
