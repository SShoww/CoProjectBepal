using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace Bepal;

/// <summary>SFX names = file names in Content/Sfx (no extension).</summary>
public static class Sfx
{
    public const string QteGreat = "sfx_qte_great", QteMiss = "sfx_qte_miss", QteSelect = "sfx_qte_select";
    public const string UiClick = "sfx_ui_click", UiCoin = "sfx_ui_coin", UiLevelUp = "sfx_ui_levelup";
    public const string FightDodge = "sfx_fight_dodge", FightPetHurt = "sfx_fight_pet_hurt",
        FightPlayerAttack = "sfx_fight_player_attack", FightBossRoar = "sfx_fight_boss_roar";
    public const string DoorKnock = "sfx_event_door_knock", DoorOpen = "sfx_event_door_open", EndDay = "sfx_event_end_day",
        NewPet = "sfx_event_new_pet", Storm = "sfx_event_storm";
    public const string Footstep = "sfx_world_footstep", Interact = "sfx_world_interact";

    /// <summary>Synthesized at startup (no file) — dialogue text tick.</summary>
    public const string Typewriter = "sfx_typewriter";
    /// <summary>Synthesized at startup (no file) — soft two-note chime; replaced the harsh sampled Perfect sound.</summary>
    public const string QtePerfect = "sfx_qte_perfect";

    /// <summary>Files in Content/Sfx.</summary>
    public static readonly string[] All =
    {
        QteGreat, QteMiss, QteSelect, UiClick, UiCoin, UiLevelUp,
        FightDodge, FightPetHurt, FightPlayerAttack, FightBossRoar,
        DoorKnock, DoorOpen, EndDay, NewPet, Storm, Footstep, Interact,
    };
}

/// <summary>
/// Fire-and-forget sound effects. Silent (never throws) when muted or when no audio device exists.
/// <see cref="Muted"/> is set by Game1 for --autoplay / --shots before <see cref="Load"/>.
/// </summary>
public static class Audio
{
    public static bool Muted;

    static bool _ok;
    static double _time;
    static readonly Dictionary<string, SoundEffect> _fx = new();
    static readonly Dictionary<string, double> _last = new();
    static readonly Dictionary<string, SoundEffectInstance> _loops = new();
    static readonly HashSet<string> _touched = new();

    public static void Load(ContentManager content)
    {
        if (Muted) return;
        try
        {
            foreach (var n in Sfx.All)
            {
                try { _fx[n] = content.Load<SoundEffect>("Sfx/" + n); }
                catch (NoAudioHardwareException) { throw; }
                catch (Exception e) { Console.WriteLine($"[audio] missing {n}: {e.Message}"); }
            }
            _fx[Sfx.Typewriter] = Synth(0.03f, t => MathF.Sin(2 * MathF.PI * 1200 * t) * (1f - t / 0.03f));
            _fx[Sfx.QtePerfect] = Synth(0.45f, PerfectChime);
            _ok = _fx.Count > 0;
        }
        catch (Exception e)
        {
            Console.WriteLine($"[audio] disabled: {e.Message}");
            _ok = false;
        }
    }

    /// <summary>G5 then D6 (a rising fifth, both below 1.2 kHz) — each note is a sine plus a quiet 2nd harmonic with an exponential decay, no sharp ring.</summary>
    static float PerfectChime(float t)
    {
        static float Note(float t, float freq) =>
            t < 0 ? 0 : (MathF.Sin(2 * MathF.PI * freq * t) + 0.25f * MathF.Sin(4 * MathF.PI * freq * t)) * MathF.Exp(-9f * t);
        return 0.7f * (Note(t, 783.99f) + Note(t - 0.1f, 1174.66f));
    }

    /// <summary>Mono 16-bit tone from a waveform f(t seconds) in [-1, 1], with attack/release ramps to avoid pops.</summary>
    static SoundEffect Synth(float duration, Func<float, float> wave, int sampleRate = 22050)
    {
        int n = (int)(duration * sampleRate);
        var buf = new byte[n * 2];
        float attack = n * 0.05f, release = n * 0.25f;
        for (int i = 0; i < n; i++)
        {
            float env = i < attack ? i / attack : i > n - release ? (n - i) / release : 1f;
            short s = (short)(Math.Clamp(wave((float)i / sampleRate), -1f, 1f) * env * 22000f);
            buf[i * 2] = (byte)(s & 0xFF);
            buf[i * 2 + 1] = (byte)((s >> 8) & 0xFF);
        }
        return new SoundEffect(buf, sampleRate, AudioChannels.Mono);
    }

    public static void Play(string name, float volume = 1f, float pitch = 0f, float minGap = 0.05f)
    {
        if (Muted || !_ok || !_fx.TryGetValue(name, out var fx)) return;
        if (_last.TryGetValue(name, out var t) && _time - t < minGap) return;
        _last[name] = _time;
        try { fx.Play(Math.Clamp(volume, 0f, 1f), Math.Clamp(pitch, -1f, 1f), 0f); }
        catch (Exception) { _ok = false; }
    }

    /// <summary>Call every frame while the loop should sound; it stops by itself once calls stop (see <see cref="EndFrame"/>).</summary>
    public static void Loop(string name, float volume = 1f)
    {
        if (Muted || !_ok || !_fx.TryGetValue(name, out var fx)) return;
        _touched.Add(name);
        try
        {
            if (!_loops.TryGetValue(name, out var inst))
            {
                inst = fx.CreateInstance();
                inst.IsLooped = true;
                _loops[name] = inst;
            }
            inst.Volume = volume;
            if (inst.State != SoundState.Playing) inst.Play();
        }
        catch (Exception) { _ok = false; }
    }

    /// <summary>Once per real frame after scenes updated: advances the throttle clock and stops loops nobody refreshed.</summary>
    public static void EndFrame(float dt)
    {
        _time += dt;
        if (_loops.Count == 0) return;
        foreach (var (name, inst) in _loops)
        {
            if (_touched.Contains(name) || inst.State != SoundState.Playing) continue;
            try { inst.Stop(); } catch (Exception) { }
        }
        _touched.Clear();
    }
}
