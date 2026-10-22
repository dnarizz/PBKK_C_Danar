using Microsoft.EntityFrameworkCore;

namespace TernakKu.Data;

// Account data and guest data live in two separate SQLite files that never read each other.
public enum JenisDatabase { Akun, Tamu }

public static class Database
{
    public static string Folder { get; } =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TernakKu");

    public static string NamaFile(JenisDatabase jenis) => jenis == JenisDatabase.Tamu ? "tamu.db" : "ternakku.db";

    public static string PathFile(JenisDatabase jenis) => Path.Combine(Folder, NamaFile(jenis));

    public static TernakDbContext Buka(JenisDatabase jenis)
    {
        var options = new DbContextOptionsBuilder<TernakDbContext>()
            .UseSqlite($"Data Source={PathFile(jenis)}")
            .Options;
        return new TernakDbContext(options);
    }

    // Creates the file and fills it with sample data the first time it is opened.
    public static void Siapkan(JenisDatabase jenis)
    {
        Directory.CreateDirectory(Folder);
        using var db = Buka(jenis);
        db.Database.EnsureCreated();
        if (!db.Spesies.Any()) Seeder.Isi(db, jenis);
    }

    // Deletes a database file and seeds it again (used by "Kosongkan database tamu").
    public static void Reset(JenisDatabase jenis)
    {
        using (var db = Buka(jenis)) db.Database.EnsureDeleted();
        Siapkan(jenis);
    }

    public static string Backup(JenisDatabase jenis, string folderTujuan)
    {
        Directory.CreateDirectory(folderTujuan);
        var tujuan = Path.Combine(folderTujuan, $"{Path.GetFileNameWithoutExtension(NamaFile(jenis))}-backup-{DateTime.Now:yyyy-MM-dd-HHmm}.db");
        Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();
        File.Copy(PathFile(jenis), tujuan, overwrite: true);
        return tujuan;
    }

    public static void Pulihkan(JenisDatabase jenis, string fileBackup)
    {
        Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();
        File.Copy(fileBackup, PathFile(jenis), overwrite: true);
    }
}
