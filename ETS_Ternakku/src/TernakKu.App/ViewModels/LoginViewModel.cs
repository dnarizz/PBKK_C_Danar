using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TernakKu.Core.Rules;
using TernakKu.Data;

namespace TernakKu.App.ViewModels;

public record BarisAkun(string Peran, string Username, string Password, string Nama, AkunDemo Akun);

public partial class LoginViewModel(MainViewModel main) : ObservableObject
{
    [ObservableProperty] private string username = "";
    [ObservableProperty] private string error = "";
    [ObservableProperty] private bool ingatSaya;
    [ObservableProperty] private bool tampilAkun;
    [ObservableProperty] private bool sibuk;

    public string Versi => $"v{typeof(LoginViewModel).Assembly.GetName().Version?.ToString(2) ?? "1.0"}";

    // Dummy accounts of the account database, shown in the credentials panel.
    public IReadOnlyList<BarisAkun> Akun { get; } = Seeder.AkunDemo
        .Select(a => new BarisAkun(AturanBisnis.LabelPeran[a.Peran], a.Username, a.Password, a.Nama, a))
        .ToList();

    public string FileAkun => Database.NamaFile(JenisDatabase.Akun);
    public string FileTamu => Database.NamaFile(JenisDatabase.Tamu);

    // The view fills the PasswordBox when an account row is picked.
    public event Action<string>? IsiPassword;

    [RelayCommand]
    void ToggleAkun() => TampilAkun = !TampilAkun;

    [RelayCommand]
    void PakaiAkun(BarisAkun baris)
    {
        Username = baris.Username;
        Error = "";
        IsiPassword?.Invoke(baris.Password);
    }

    public void Masuk(string password)
    {
        var u = Username.Trim().ToLowerInvariant();
        if (u.Length == 0 || password.Length == 0) { Error = "Isi username dan password"; return; }
        Sibuk = true;
        try
        {
            Database.Siapkan(JenisDatabase.Akun);
            using var db = Database.Buka(JenisDatabase.Akun);
            var user = db.Pengguna.FirstOrDefault(x => x.Username == u);
            if (user is null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash)) { Error = "Username atau password salah"; return; }
            if (!user.Aktif) { Error = "Akun dinonaktifkan. Hubungi admin."; return; }
            user.LoginTerakhir = DateTime.Now;
            db.SaveChanges();
            main.MasukAkun(user, IngatSaya);
        }
        finally { Sibuk = false; }
    }

    [RelayCommand]
    void Tamu() => main.MasukTamu(IngatSaya);
}
