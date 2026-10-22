using CommunityToolkit.Mvvm.ComponentModel;
using TernakKu.App.Services;

namespace TernakKu.App.ViewModels.Dialogs;

public partial class GantiPasswordViewModel(ShellViewModel shell) : DialogViewModel(shell)
{
    public override string Judul => "Ganti password";

    public GalatForm Galat { get; } = new();

    // PasswordBox has no bindable value; the view copies them here.
    [ObservableProperty] private string lama = "";
    [ObservableProperty] private string baru = "";
    [ObservableProperty] private string ulangi = "";

    protected override bool Simpan()
    {
        Galat.Bersih();
        using var db = Session.Buka();
        var user = db.Pengguna.FirstOrDefault(u => u.Username == Session.Username);
        if (user is null || !BCrypt.Net.BCrypt.Verify(Lama, user.PasswordHash)) Galat.Set("Lama", "Password lama salah");
        if (Baru.Length < 6) Galat.Set("Baru", "Minimal 6 karakter");
        else if (Baru != Ulangi) Galat.Set("Ulangi", "Password tidak sama");
        if (!Galat.Kosong) return false;

        user!.PasswordHash = BCrypt.Net.BCrypt.HashPassword(Baru);
        db.SaveChanges();
        Shell.TampilSnack("Password diganti");
        return true;
    }
}
