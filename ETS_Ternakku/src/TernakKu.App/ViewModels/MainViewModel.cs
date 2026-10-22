using CommunityToolkit.Mvvm.ComponentModel;
using TernakKu.App.Services;
using TernakKu.Core.Models;
using TernakKu.Data;

namespace TernakKu.App.ViewModels;

// Top level: shows the login screen or the app shell, and owns the title-bar database chip.
public partial class MainViewModel : ObservableObject
{
    [ObservableProperty] private object? current;
    [ObservableProperty] private bool showDbChip;
    [ObservableProperty] private bool isTamu;
    [ObservableProperty] private string dbChipText = "";
    [ObservableProperty] private string dbChipIcon = "db";

    // Opens straight into the remembered database and account when "Ingat saya" was ticked.
    public void Mulai()
    {
        var s = Settings.Current;
        if (s.IngatMode == "tamu") { MasukTamu(ingat: false); return; }
        if (s.IngatMode == "akun" && s.IngatUsername is { } u)
        {
            Database.Siapkan(JenisDatabase.Akun);
            using var db = Database.Buka(JenisDatabase.Akun);
            var user = db.Pengguna.FirstOrDefault(x => x.Username == u && x.Aktif);
            if (user is not null) { MasukAkun(user, ingat: false); return; }
            Lupakan();
        }
        Current = new LoginViewModel(this);
    }

    public void MasukAkun(Pengguna user, bool ingat)
    {
        Session.Masuk(JenisDatabase.Akun, user.Username, user.NamaLengkap, user.Peran);
        if (ingat) Ingat("akun", user.Username);
        BukaShell();
    }

    public void MasukTamu(bool ingat)
    {
        Database.Siapkan(JenisDatabase.Tamu);
        Session.Masuk(JenisDatabase.Tamu, "tamu", "Tamu", Peran.Tamu);
        if (ingat) Ingat("tamu", null);
        BukaShell();
    }

    void BukaShell()
    {
        IsTamu = Session.IsTamu;
        DbChipText = IsTamu ? $"Mode tamu · {Session.NamaFile}" : Session.NamaFile;
        DbChipIcon = IsTamu ? "guest" : "db";
        ShowDbChip = true;
        Current = new ShellViewModel(this);
    }

    public void Keluar()
    {
        Lupakan();
        ShowDbChip = false;
        Current = new LoginViewModel(this);
    }

    public static void Ingat(string mode, string? username)
    {
        Settings.Current.IngatMode = mode;
        Settings.Current.IngatUsername = username;
        Settings.Current.Save();
    }

    public static void Lupakan()
    {
        Settings.Current.IngatMode = null;
        Settings.Current.IngatUsername = null;
        Settings.Current.Save();
    }
}
