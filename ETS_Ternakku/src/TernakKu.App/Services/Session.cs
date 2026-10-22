using TernakKu.Core.Models;
using TernakKu.Data;

namespace TernakKu.App.Services;

// Who is logged in, which database file is open, and what that role may do.
public static class Session
{
    public static JenisDatabase Db { get; private set; }
    public static string Username { get; private set; } = "";
    public static string Nama { get; private set; } = "";
    public static Peran Peran { get; private set; }

    public static bool IsTamu => Db == JenisDatabase.Tamu;
    public static string NamaFile => Database.NamaFile(Db);

    public static void Masuk(JenisDatabase db, string username, string nama, Peran peran)
    {
        Db = db; Username = username; Nama = nama; Peran = peran;
    }

    public static TernakDbContext Buka() => Database.Buka(Db);

    static readonly Dictionary<Peran, string[]> Halaman = new()
    {
        [Peran.Tamu] = ["dashboard", "ternak", "kandang", "pakan", "kesehatan", "reproduksi", "mutasi", "keuangan", "laporan", "master", "pengaturan"],
        [Peran.Petugas] = ["dashboard", "ternak", "kandang", "pakan", "pengaturan"],
        [Peran.Mantri] = ["dashboard", "ternak", "kandang", "kesehatan", "reproduksi", "pengaturan"]
    };

    public static bool BolehBuka(string halaman) => Peran == Peran.Admin || Halaman[Peran].Contains(halaman);

    // Actions inside pages.
    public static bool BolehTambahTernak => Peran is Peran.Admin or Peran.Tamu;
    public static bool BolehTimbang => Peran is Peran.Admin or Peran.Tamu or Peran.Petugas;
    public static bool BolehMutasi => Peran is Peran.Admin or Peran.Tamu;
    public static bool BolehPakan => Peran is Peran.Admin or Peran.Tamu or Peran.Petugas;
    public static bool BolehKesehatan => Peran is Peran.Admin or Peran.Tamu or Peran.Mantri;
    public static bool BolehMaster => Peran is Peran.Admin or Peran.Tamu;
}
