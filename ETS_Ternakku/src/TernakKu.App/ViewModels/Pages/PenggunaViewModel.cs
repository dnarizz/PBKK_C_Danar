using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TernakKu.App.Services;
using TernakKu.App.ViewModels.Dialogs;
using TernakKu.Core.Models;
using TernakKu.Core.Rules;
using TernakKu.Data;

namespace TernakKu.App.ViewModels.Pages;

public record BarisPengguna(int Id, string Nama, string Username, string Peran, string PeranTone, bool Aktif, string Status, string Terakhir, bool Saya)
{
    public string TombolGanti => Aktif ? "Nonaktifkan" : "Aktifkan";
    public string StatusTone => Aktif ? "ok" : "neutral";
}

public record BarisAkses(string Fitur, string Admin, string Petugas, string Mantri);

public partial class PenggunaViewModel(ShellViewModel shell) : PageViewModel(shell), IPunyaTambah
{
    public override string Key => "pengguna";

    [ObservableProperty] private List<BarisPengguna> pengguna = [];

    public List<BarisAkses> Akses { get; } =
    [
        new("Data ternak", "Ubah", "Lihat", "Lihat"),
        new("Penimbangan dan pakan", "Ya", "Ya", "—"),
        new("Kesehatan dan reproduksi", "Ya", "—", "Ya"),
        new("Mutasi dan keuangan", "Ya", "—", "—"),
        new("Laporan dan backup", "Ya", "—", "—"),
        new("Kelola pengguna", "Ya", "—", "—")
    ];

    public override void Load()
    {
        var hari = DateTime.Today;
        using var db = Session.Buka();
        Pengguna = db.Pengguna.OrderBy(u => u.Id).AsEnumerable().Select(u => new BarisPengguna(
            u.Id, u.NamaLengkap, u.Username,
            u.Peran switch { Peran.Admin => "Admin", Peran.Mantri => "Mantri", _ => "Petugas" },
            u.Peran switch { Peran.Admin => "ok", Peran.Mantri => "warn", _ => "info" },
            u.Aktif, u.Aktif ? "Aktif" : "Nonaktif",
            u.LoginTerakhir is { } t ? Kapital(AturanBisnis.HariRelatif(t, hari)) : "Belum pernah",
            u.Username == Session.Username)).ToList();
    }

    static string Kapital(string s) => s.Length == 0 ? s : char.ToUpperInvariant(s[0]) + s[1..];

    [RelayCommand]
    void Tambah() => Shell.TampilDialog(new TambahPenggunaViewModel(Shell));

    [RelayCommand]
    void Reset(int id)
    {
        string username, lama;
        using (var db = Session.Buka())
        {
            var u = db.Pengguna.Find(id);
            if (u is null) return;
            username = u.Username;
            lama = u.PasswordHash;
            u.PasswordHash = BCrypt.Net.BCrypt.HashPassword(Seeder.PasswordAwal);
            db.SaveChanges();
        }
        Shell.TampilSnack($"Password {username} direset ke password awal", () =>
        {
            using var db = Session.Buka();
            var u = db.Pengguna.Find(id);
            if (u is null) return;
            u.PasswordHash = lama;
            db.SaveChanges();
        });
    }

    [RelayCommand]
    void Ganti(int id)
    {
        string nama;
        bool aktif;
        using (var db = Session.Buka())
        {
            var u = db.Pengguna.Find(id);
            if (u is null || u.Username == Session.Username) return;
            u.Aktif = !u.Aktif;
            nama = u.NamaLengkap;
            aktif = u.Aktif;
            db.SaveChanges();
        }
        Load();
        Shell.TampilSnack($"{nama} {(aktif ? "diaktifkan" : "dinonaktifkan")}", () =>
        {
            using var db = Session.Buka();
            var u = db.Pengguna.Find(id);
            if (u is null) return;
            u.Aktif = !aktif;
            db.SaveChanges();
        });
    }
}
