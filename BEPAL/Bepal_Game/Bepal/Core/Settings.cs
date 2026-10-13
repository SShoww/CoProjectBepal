using System;
using System.Globalization;
using System.IO;

namespace Bepal;

/// <summary>
/// Player options: music and sound-effect volume (0..1, multiplied onto <see cref="Balance.MusicVolume"/> and each SFX's own volume).
/// Saved to %APPDATA%/BePal/settings.txt (<c>music=0.8</c> lines); never read or written under --autoplay / --shots (<see cref="Persist"/>).
/// </summary>
public static class Settings
{
    public static float Music = 1f, Sfx = 1f;
    public static bool Persist;

    static string FilePath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "BePal", "settings.txt");

    public static void Load()
    {
        if (!Persist) return;
        try
        {
            if (!File.Exists(FilePath)) return;
            foreach (var line in File.ReadAllLines(FilePath))
            {
                var kv = line.Split('=', 2);
                if (kv.Length != 2 || !float.TryParse(kv[1], NumberStyles.Float, CultureInfo.InvariantCulture, out float v)) continue;
                v = Math.Clamp(v, 0f, 1f);
                if (kv[0].Trim() == "music") Music = v;
                else if (kv[0].Trim() == "sfx") Sfx = v;
            }
        }
        catch (Exception e) { Console.WriteLine($"[settings] could not read: {e.Message}"); }
    }

    public static void Save()
    {
        if (!Persist) return;
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);
            File.WriteAllLines(FilePath, new[]
            {
                "music=" + Music.ToString("0.##", CultureInfo.InvariantCulture),
                "sfx=" + Sfx.ToString("0.##", CultureInfo.InvariantCulture),
            });
        }
        catch (Exception e) { Console.WriteLine($"[settings] could not save: {e.Message}"); }
    }
}
