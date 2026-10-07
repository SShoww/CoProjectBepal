using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bepal;

/// <summary>Transform applied to a single still image, pivoting at the feet. Rot in radians (+ = lean right), Scale is 1 at rest.</summary>
public struct Pose
{
    public Vector2 Offset;
    public float Rot;
    public Vector2 Scale;
    public static Pose Rest => new() { Scale = Vector2.One };
}

/// <summary>
/// Procedural "animation" for a character that has only ONE still image: idle breathing, step bob + sway,
/// lean into the run, squash on foot-fall, stretch/squash on start/stop, and a flip-through-zero turn.
/// Feed it the physics every frame; read <see cref="Pose"/>. Both the primitive <see cref="Art.Player"/> and
/// <see cref="Draw"/> (for a real sprite) consume the same pose, so tuning lives in one place (Balance "Movement feel").
/// </summary>
public class SpriteMotion
{
    public Pose Pose = Pose.Rest;
    /// <summary>Visual facing -1..1; slides through 0 on a turn so the image flips smoothly.</summary>
    public float Face = 1;

    float _phase, _time, _stretch, _prevVel;

    /// <param name="vel">Signed px/s.</param>
    /// <param name="facing">-1 / +1 logical facing.</param>
    public void Update(float dt, float vel, int facing)
    {
        _time += dt;
        float speed = MathF.Abs(vel) / Balance.PlayerMaxSpeed;              // 0..~1.7 (run)
        float run = MathHelper.Clamp((MathF.Abs(vel) - Balance.PlayerMaxSpeed) / (Balance.PlayerRunSpeed - Balance.PlayerMaxSpeed), 0, 1);
        float moving = MathHelper.Clamp(speed, 0, 1);
        _phase += dt * speed * Balance.StrideRate;                           // stride advances with speed, so legs never "skate"
        Face = Ease.Damp(Face, facing, Balance.FaceDamp, dt);

        float stride = MathF.Abs(MathF.Sin(_phase));                         // 0 at foot-fall, 1 mid-stride
        float accel = dt > 0 ? (vel - _prevVel) / dt : 0;
        _prevVel = vel;
        // Accelerating stretches along the motion, braking squashes it (smoothed so it reads as one soft pulse)
        float dir = MathF.Sign(vel);
        float along = dir == 0 ? 0 : MathHelper.Clamp(accel * dir / Balance.PlayerAccel, -1, 1);
        _stretch = Ease.Damp(_stretch, along, 14, dt);

        float breath = MathF.Sin(_time * 2f) * Balance.BreathScale * (1 - moving);
        float bobPx = Balance.WalkBobPx * (1 + run) * stride * moving;
        float footfall = (1 - stride) * Balance.StepSquash * moving;        // slight squash as the foot lands
        float sy = 1 + breath - footfall - _stretch * Balance.StartStopSquash * 0.5f;   // volume-ish preserving: wider = shorter
        float sx = 1 - breath * 0.6f + footfall * 0.6f + _stretch * Balance.StartStopSquash;

        Pose = new Pose
        {
            Offset = new Vector2(0, -bobPx),
            Rot = dir * (Balance.WalkLeanRad * moving + Balance.RunLeanRad * run)
                  + MathF.Sin(_phase) * Balance.StrideSwayRad * moving,
            Scale = new Vector2(sx, sy),
        };
    }

    /// <summary>
    /// Draw part <paramref name="src"/> of a single still <paramref name="tex"/> (origin = bottom centre of src) standing on
    /// <paramref name="feet"/>, <paramref name="height"/> px tall. Point-sampled so pixel art stays crisp.
    /// </summary>
    public static void Draw(SpriteBatch sb, Texture2D tex, Rectangle src, Vector2 feet, float height, Pose p, float face, Color? tint = null)
    {
        float k = height / src.Height;
        // Negative scale gets culled by SpriteBatch, so flip with SpriteEffects; |face| still squashes the image through zero on a turn
        var scale = new Vector2(k * p.Scale.X * MathF.Abs(face), k * p.Scale.Y);
        sb.End();
        Gfx.Begin(sb, null, SamplerState.PointClamp);
        sb.Draw(tex, feet + p.Offset, src, tint ?? Color.White, p.Rot, new Vector2(src.Width / 2f, src.Height), scale, face < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
        sb.End();
        Gfx.Begin(sb);
    }
}
