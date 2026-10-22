using System.IO;
using System.Text.Json;
using TernakKu.Data;

namespace TernakKu.App.Services;

// Per-computer preferences, stored next to the databases in %LOCALAPPDATA%\TernakKu\settings.json.
public class Settings
{
    public string Theme { get; set; } = "light";
    public bool PadatTabel { get; set; }
    public bool NavCiut { get; set; }

    // "Ingat saya": when set, the app opens straight into this database and account.
    public string? IngatMode { get; set; }
    public string? IngatUsername { get; set; }

    static string PathFile => Path.Combine(Database.Folder, "settings.json");

    public static Settings Current { get; private set; } = Load();

    static Settings Load()
    {
        try
        {
            if (File.Exists(PathFile)) return JsonSerializer.Deserialize<Settings>(File.ReadAllText(PathFile)) ?? new Settings();
        }
        catch (Exception ex) when (ex is IOException or JsonException or UnauthorizedAccessException)
        {
            // A broken settings file just falls back to defaults.
        }
        return new Settings();
    }

    public void Save()
    {
        Directory.CreateDirectory(Database.Folder);
        File.WriteAllText(PathFile, JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true }));
    }
}
