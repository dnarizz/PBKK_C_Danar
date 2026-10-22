using CommunityToolkit.Mvvm.ComponentModel;
using TernakKu.App.Services;
using TernakKu.Core.Models;

namespace TernakKu.App.ViewModels.Dialogs;

public record PilihanPeran(Peran Nilai, string Label);

public partial class TambahPenggunaViewModel(ShellViewModel shell) : DialogViewModel(shell)
{
    public override string Judul => "Tambah pengguna";
    public override bool Lebar => true;

    public GalatForm Galat { get; } = new();
    public List<PilihanPeran> DaftarPeran { get; } =
        [new(Peran.Petugas, "Petugas kandang"), new(Peran.Mantri, "Mantri"), new(Peran.Admin, "Admin")];

    [ObservableProperty] private string nama = "";
    [ObservableProperty] private string username = "";
    [ObservableProperty] private Peran peran = Peran.Petugas;
    // PasswordBox has no bindable value; the view copies it here.
    [ObservableProperty] private string password = "";
    [ObservableProperty] private string ulangi = "";

    protected override bool Simpan()
    {
        Galat.Bersih();
        var user = Username.Trim().ToLowerInvariant();
        using var db = Session.Buka();
        if (string.IsNullOrWhiteSpace(Nama)) Galat.Set("Nama", "Isi nama");
        if (user.Length == 0) Galat.Set("Username", "Isi username");
        else if (db.Pengguna.Any(u => u.Username == user)) Galat.Set("Username", "Username sudah dipakai");
        if (Password.Length < 6) Galat.Set("Password", "Minimal 6 karakter");
        else if (Password != Ulangi) Galat.Set("Ulangi", "Password tidak sama");
        if (!Galat.Kosong) return false;

        var baru = new Pengguna
        {
            Username = user, NamaLengkap = Nama.Trim(), Peran = Peran, Aktif = true,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(Password)
        };
        db.Pengguna.Add(baru);
        db.SaveChanges();
        var id = baru.Id;
        Shell.MuatUlang();
        Shell.TampilSnack($"Akun {user} dibuat", () =>
        {
            using var d = Session.Buka();
            var x = d.Pengguna.Find(id);
            if (x is null) return;
            d.Pengguna.Remove(x);
            d.SaveChanges();
        });
        return true;
    }
}
