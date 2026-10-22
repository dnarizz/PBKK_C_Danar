using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TernakKu.App.Services;
using TernakKu.App.ViewModels.Dialogs;
using TernakKu.Core.Models;
using TernakKu.Data;

namespace TernakKu.App.ViewModels.Pages;

public partial class PengaturanViewModel(ShellViewModel shell) : PageViewModel(shell)
{
    public override string Key => "pengaturan";

    [ObservableProperty] private string tema = Settings.Current.Theme == "dark" ? "dark" : "light";
    [ObservableProperty, NotifyPropertyChangedFor(nameof(KeteranganKepadatan))] private string kepadatan = Settings.Current.PadatTabel ? "padat" : "nyaman";
    [ObservableProperty, NotifyPropertyChangedFor(nameof(KeteranganIngat), nameof(Mengingat))] private string? ingatMode = Settings.Current.IngatMode;

    partial void OnTemaChanged(string value)
    {
        ThemeService.Apply(value);
        Settings.Current.Theme = value;
        Settings.Current.Save();
    }

    partial void OnKepadatanChanged(string value)
    {
        global::TernakKu.App.App.ApplyDensity(value == "padat");
        Settings.Current.PadatTabel = value == "padat";
        Settings.Current.Save();
    }

    public string KeteranganKepadatan => $"Tinggi baris {(Kepadatan == "padat" ? 36 : 44)} px";

    public bool Mengingat => IngatMode is not null;

    public string KeteranganIngat
    {
        get
        {
            if (IngatMode is null) return "Login atau pilihan tamu diminta setiap kali aplikasi dibuka";
            var sebagai = IngatMode == "tamu" ? "Tamu" : Settings.Current.IngatUsername == Session.Username ? Session.Nama : Settings.Current.IngatUsername;
            return $"Aplikasi langsung terbuka sebagai {sebagai} ({(IngatMode == "tamu" ? "tamu.db" : "ternakku.db")}) tanpa login";
        }
    }

    public bool IsTamu => Session.IsTamu;
    public bool PunyaAkun => !Session.IsTamu;
    public string Username => Session.Username;
    public string PathDatabase => Database.PathFile(Session.Db);
    public string LabelDatabase => Session.IsTamu ? "Database tamu" : "Database akun";
    public string ToneDatabase => Session.IsTamu ? "warn" : "ok";
    public bool BolehBackup => Session.Peran == Peran.Admin || Session.IsTamu;

    [RelayCommand]
    void Lupakan()
    {
        MainViewModel.Lupakan();
        IngatMode = null;
    }

    [RelayCommand]
    void IngatSaya()
    {
        MainViewModel.Ingat(Session.IsTamu ? "tamu" : "akun", Session.IsTamu ? null : Session.Username);
        IngatMode = Settings.Current.IngatMode;
    }

    [RelayCommand]
    void GantiPassword() => Shell.TampilDialog(new GantiPasswordViewModel(Shell));

    [RelayCommand]
    void Backup()
    {
        var dlg = new Microsoft.Win32.OpenFolderDialog { Title = "Pilih folder tujuan backup" };
        if (dlg.ShowDialog() != true) return;
        try
        {
            var file = Database.Backup(Session.Db, dlg.FolderName);
            Shell.TampilSnack($"Backup tersimpan: {Path.GetFileName(file)}");
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            Shell.TampilSnack("Backup gagal: folder tidak bisa ditulis");
        }
    }

    [RelayCommand]
    void Pulihkan() => Shell.TampilDialog(new KonfirmasiViewModel(Shell, "Pulihkan database?",
        $"Data di {Session.NamaFile} akan diganti dengan isi file backup yang dipilih.", "Pilih file backup", false, () =>
        {
            var dlg = new Microsoft.Win32.OpenFileDialog { Title = "Pilih file backup", Filter = "Database TernakKu (*.db)|*.db" };
            if (dlg.ShowDialog() != true) return false;
            try
            {
                Database.Pulihkan(Session.Db, dlg.FileName);
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                Shell.TampilSnack("Pemulihan gagal: file tidak bisa dibaca");
                return true;
            }
            Shell.Buka("dashboard");
            Shell.TampilSnack($"Database dipulihkan dari {Path.GetFileName(dlg.FileName)}");
            return true;
        }));

    [RelayCommand]
    void KosongkanTamu() => Shell.TampilDialog(new KonfirmasiViewModel(Shell, "Kosongkan database tamu?",
        "Semua perubahan di tamu.db hilang dan diganti data contoh awal. Database akun tidak tersentuh.", "Kosongkan", true, () =>
        {
            Database.Reset(JenisDatabase.Tamu);
            Shell.Buka("pengaturan");
            Shell.TampilSnack("Database tamu dikosongkan");
            return true;
        }));
}
