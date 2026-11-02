using System;

namespace Bepal;

/// <summary>Small easing / smoothing helpers for movement polish. Frame-rate independent where a dt is involved.</summary>
public static class Ease
{
    public static float OutCubic(float t) { t = Math.Clamp(t, 0, 1); return 1 - MathF.Pow(1 - t, 3); }

    public static float InOutSine(float t) => -(MathF.Cos(MathF.PI * Math.Clamp(t, 0, 1)) - 1) / 2;

    /// <summary>Overshoots past 1 slightly before settling.</summary>
    public static float OutBack(float t)
    {
        const float c1 = 1.70158f, c3 = c1 + 1;
        t = Math.Clamp(t, 0, 1) - 1;
        return 1 + c3 * t * t * t + c1 * t * t;
    }

    public static float Smoothstep(float t) { t = Math.Clamp(t, 0, 1); return t * t * (3 - 2 * t); }

    public static float MoveToward(float cur, float target, float maxDelta) =>
        MathF.Abs(target - cur) <= maxDelta ? target : cur + MathF.Sign(target - cur) * maxDelta;

    /// <summary>Exponential smoothing: closes a fraction (1 - e^(-k dt)) of the gap each step.</summary>
    public static float Damp(float cur, float target, float k, float dt) => cur + (target - cur) * (1 - MathF.Exp(-k * dt));
}

/// <summary>A value that eases toward a target (for bars and other display-only numbers).</summary>
public struct Smoothed
{
    public float Value;

    public void Update(float target, float k, float dt)
    {
        Value = Ease.Damp(Value, target, k, dt);
        if (MathF.Abs(target - Value) < 0.01f) Value = target;
    }

    public void Snap(float v) => Value = v;
}
