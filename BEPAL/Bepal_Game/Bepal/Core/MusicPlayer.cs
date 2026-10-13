using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace Bepal;

/// <summary>BGM names = file names in Content/Music (no extension).</summary>
public static class Bgm
{
    public const string Menu = "bgm_menu", Base = "bgm_base_cozy", Fight = "bgm_fight", Boss = "bgm_boss_bigz",
        Merchant = "bgm_merchant", Storm = "bgm_thunderstorm", End = "bgm_to_be_continued";

    public static readonly string[] All = { Menu, Base, Fight, Boss, Merchant, Storm, End };
}

/// <summary>
/// Background music: plays the track the scene stack asks for (<see cref="SceneManager.WantedMusic"/>), looping, with a fade out/in when it changes.
/// Silent (never throws) when <see cref="Audio.Muted"/> (--autoplay / --shots) or when no audio device exists.
/// </summary>
public static class MusicPlayer
{
    static readonly Dictionary<string, Song> _songs = new();
    static bool _ok;
    static string? _current;   // track actually playing (null = none)
    static float _level;       // fade level 0..1, times Balance.MusicVolume

    public static void Load(ContentManager content)
    {
        if (Audio.Muted) return;
        foreach (var n in Bgm.All)
        {
            try { _songs[n] = content.Load<Song>("Music/" + n); }
            catch (Exception e) { Console.WriteLine($"[music] missing {n}: {e.Message}"); }
        }
        _ok = _songs.Count > 0;
        if (_ok)
        {
            try { MediaPlayer.IsRepeating = true; MediaPlayer.Volume = 0; }
            catch (Exception e) { Console.WriteLine($"[music] disabled: {e.Message}"); _ok = false; }
        }
    }

    /// <summary>Call every frame with the wanted track (null = silence).</summary>
    public static void Update(float dt, string? wanted)
    {
        if (!_ok) return;
        try
        {
            float step = dt / Balance.MusicFadeTime;
            if (wanted == _current)
                _level = MathF.Min(1, _level + step);
            else
            {
                _level = MathF.Max(0, _level - step);
                if (_level <= 0)
                {
                    _current = wanted;
                    if (wanted != null && _songs.TryGetValue(wanted, out var song)) MediaPlayer.Play(song);
                    else MediaPlayer.Stop();
                }
            }
            MediaPlayer.Volume = _level * Balance.MusicVolume;
        }
        catch (Exception e)
        {
            Console.WriteLine($"[music] disabled: {e.Message}");
            _ok = false;
        }
    }
}
