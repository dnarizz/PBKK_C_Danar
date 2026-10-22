using System.Collections.ObjectModel;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using TernakKu.App.Services;
using TernakKu.Core.Models;
using TernakKu.Core.Rules;
using TernakKu.App.ViewModels.Pages;

namespace TernakKu.App.ViewModels;

public partial class NavItem(string key, string label, string icon, string group) : ObservableObject
{
    public string Key { get; } = key;
    public string Label { get; } = label;
    public string Icon { get; } = icon;
    public string Group { get; } = group;
    [ObservableProperty] private bool isActive;
    [ObservableProperty] private bool isFirstInGroup;
    [ObservableProperty] private int count;
    [ObservableProperty] private string tone = "";
    [ObservableProperty] private string tip = "";
}

// The app frame: floating nav bar, current page, dialogs and the snackbar.
public partial class ShellViewModel : ObservableObject
{
    readonly MainViewModel main;
    readonly DispatcherTimer snackTimer = new() { Interval = TimeSpan.FromSeconds(3) };

    public ShellViewModel(MainViewModel main)
    {
        this.main = main;
        Ciut = Settings.Current.NavCiut;
        var menu = new (string Group, (string Key, string Label, string Icon)[] Items)[]
        {
            ("Utama", [("dashboard", "Dashboard", "dashboard"), ("ternak", "Ternak", "tag"), ("kandang", "Kandang", "home")]),
            ("Operasional", [("pakan", "Pakan", "box"), ("kesehatan", "Kesehatan", "heart"), ("reproduksi", "Reproduksi", "repro"), ("mutasi", "Mutasi", "swap")]),
            ("Bisnis", [("keuangan", "Keuangan", "wallet"), ("laporan", "Laporan", "file")]),
            ("Sistem", [("master", "Data master", "db"), ("pengguna", "Pengguna", "users"), ("pengaturan", "Pengaturan", "settings")])
        };
        foreach (var (group, items) in menu)
        {
            var first = true;
            foreach (var (key, label, icon) in items.Where(i => Session.BolehBuka(i.Key)))
            {
                Nav.Add(new NavItem(key, label, icon, group) { IsFirstInGroup = first, Tip = label });
                first = false;
            }
        }
        snackTimer.Tick += (_, _) => { snackTimer.Stop(); Snack = null; };
        using (var db = Session.Buka()) NamaPeternakan = db.Pengguna.Any() ? "Peternakan Sumber Makmur" : "Peternakan Contoh";
        Buka("dashboard");
    }

    public ObservableCollection<NavItem> Nav { get; } = [];

    public string NamaPeternakan { get; }
    public string NamaFile => Session.IsTamu ? $"Mode tamu · {Session.NamaFile}" : Session.NamaFile;
    public bool IsTamu => Session.IsTamu;
    public string NamaUser => Session.Nama;
    public string LabelPeran => AturanBisnis.LabelPeran[Session.Peran];
    public string Inisial
    {
        get
        {
            var s = string.Concat(Session.Nama.Replace("drh. ", "").Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(w => char.ToUpperInvariant(w[0])));
            return s.Length > 2 ? s[..2] : s;
        }
    }

    [ObservableProperty] private bool ciut;
    [ObservableProperty] private PageViewModel? halaman;
    [ObservableProperty] private DialogViewModel? dialog;
    [ObservableProperty] private SnackViewModel? snack;
    // Farm scene for the background (by menu group); the sidebar shows the next one so they never repeat.
    [ObservableProperty, NotifyPropertyChangedFor(nameof(AdeganSidebar))] private int adegan;
    public int AdeganSidebar => (Adegan + 1) % 3;

    partial void OnCiutChanged(bool value)
    {
        Settings.Current.NavCiut = value;
        Settings.Current.Save();
    }

    [RelayCommand]
    void ToggleNav() => Ciut = !Ciut;

    [RelayCommand]
    public void Buka(string key) => Buka(key, null);

    public void Buka(string key, object? arg)
    {
        var navKey = key == "detail" ? "ternak" : key;
        if (!Session.BolehBuka(navKey)) return;
        PageViewModel page = key switch
        {
            "dashboard" => new DashboardViewModel(this),
            "ternak" => new TernakViewModel(this, arg as TernakFilter),
            "detail" => new DetailTernakViewModel(this, (int)arg!),
            "kandang" => new KandangViewModel(this, arg as int?),
            "pakan" => new PakanViewModel(this, arg as int?),
            "kesehatan" => new KesehatanViewModel(this),
            "reproduksi" => new ReproduksiViewModel(this),
            "mutasi" => new MutasiViewModel(this),
            "keuangan" => new KeuanganViewModel(this),
            "laporan" => new LaporanViewModel(this),
            "master" => new MasterViewModel(this),
            "pengguna" => new PenggunaViewModel(this),
            "pengaturan" => new PengaturanViewModel(this),
            _ => new DashboardViewModel(this)
        };
        page.Load();
        foreach (var n in Nav) n.IsActive = n.Key == navKey;
        var group = Nav.FirstOrDefault(n => n.Key == navKey)?.Group;
        Adegan = group switch { "Operasional" => 1, "Bisnis" or "Sistem" => 2, _ => 0 };
        Halaman = page;
        SegarkanStatus();
    }

    public void MuatUlang() => Halaman?.Load();

    // Status dots and counts on the nav items.
    public void SegarkanStatus()
    {
        using var db = Session.Buka();
        var hari = DateTime.Today;
        var status = new Dictionary<string, (int n, string tone, string what)>
        {
            ["pakan"] = (db.Pakan.AsEnumerable().Count(p => p.Stok < p.StokMinimum), "warn", "stok menipis"),
            ["kesehatan"] = (db.Ternak.Count(t => t.Status == StatusTernak.Aktif && t.Kondisi != Kondisi.Sehat), "danger", "perlu perhatian"),
            ["reproduksi"] = (db.Perkawinan.Count(k => k.Status == StatusBunting.Bunting && k.PerkiraanLahir <= hari.AddDays(7)), "warn", "segera lahir")
        };
        foreach (var n in Nav)
        {
            if (status.TryGetValue(n.Key, out var s) && s.n > 0) { n.Count = s.n; n.Tone = s.tone; n.Tip = $"{n.Label} · {s.n} {s.what}"; }
            else { n.Count = 0; n.Tone = ""; n.Tip = n.Label; }
        }
    }

    public void TampilDialog(DialogViewModel vm) => Dialog = vm;

    public void TutupDialog() => Dialog = null;

    public void TampilSnack(string teks, Action? undo = null)
    {
        snackTimer.Stop();
        Snack = new SnackViewModel(teks, undo);
        snackTimer.Start();
    }

    [RelayCommand]
    void Undo()
    {
        var u = Snack?.Undo;
        snackTimer.Stop();
        Snack = null;
        if (u is null) return;
        u();
        MuatUlang();
        SegarkanStatus();
    }

    [RelayCommand]
    void Keluar() => main.Keluar();
}
