using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using TernakKu.App.Services;
using TernakKu.App.ViewModels.Dialogs;
using TernakKu.Core.Models;
using TernakKu.Core.Rules;

namespace TernakKu.App.ViewModels.Pages;

public partial class KandangViewModel(ShellViewModel shell, int? pilih) : PageViewModel(shell)
{
    public record KartuPetak(int Id, string Kode, string Nama, string Sub, int Isi, int Kapasitas, int Persen, string Tone, bool Dipilih)
    {
        public string Jumlah => $"{Isi}";
        public string KapTeks =>$"{Persen}% · kapasitas {Kapasitas}";
    }
    public record BarisPakan(string Nama, string Jumlah);
    public record BarisTernak(int Id, string Label, string Berat, string Kondisi, string Tone, bool AdaKondisi);

    public override string Key => "kandang";

    [ObservableProperty] private string subjudul = "";
    [ObservableProperty] private List<KartuPetak> petak = [];
    [ObservableProperty] private int terpilihId;

    // Side panel
    [ObservableProperty] private string judulPanel = "";
    [ObservableProperty] private string subPanel = "";
    [ObservableProperty] private int persenPanel;
    [ObservableProperty] private string tonePanel = "ok";
    [ObservableProperty] private List<BarisPakan> pakanHariIni = [];
    [ObservableProperty] private int jumlahSakit;
    [ObservableProperty] private string vaksinBerikut = "—";
    [ObservableProperty] private List<BarisTernak> ternakPanel = [];
    [ObservableProperty] private int isiPanel;

    public bool BolehMaster => Session.BolehMaster;
    public bool BolehPakan => Session.BolehBuka("pakan") && Session.BolehPakan;
    public bool AdaPanel => TerpilihId != 0;
    public bool AdaSakit => JumlahSakit > 0;
    public bool Sehat => JumlahSakit == 0;

    partial void OnJumlahSakitChanged(int value) { OnPropertyChanged(nameof(AdaSakit)); OnPropertyChanged(nameof(Sehat)); }
    partial void OnTerpilihIdChanged(int value) => OnPropertyChanged(nameof(AdaPanel));

    List<Kandang> kandang = [];
    List<Ternak> aktif = [];
    Dictionary<int, double> berat = [];
    List<PemberianPakan> pakanLog = [];
    List<JadwalVaksinasi> jadwal = [];
    bool awal = true;

    public override void Load()
    {
        var hari = DateTime.Today;
        using var db = Session.Buka();
        kandang = db.Kandang.Include(k => k.Spesies).OrderBy(k => k.Kode).ToList();
        aktif = db.Ternak.Where(t => t.Status == StatusTernak.Aktif).OrderBy(t => t.KodeTernak).ToList();
        berat = db.Penimbangan.AsEnumerable().GroupBy(p => p.TernakId).ToDictionary(g => g.Key, g => g.OrderBy(p => p.Tanggal).ThenBy(p => p.Id).Last().BeratKg);
        pakanLog = db.PemberianPakan.Include(p => p.Pakan).Where(p => p.Waktu >= hari && p.Waktu < hari.AddDays(1)).OrderBy(p => p.Waktu).ToList();
        jadwal = db.JadwalVaksinasi.Include(j => j.ObatVaksin).Include(j => j.Ternak).Where(j => j.Status != StatusJadwal.Selesai && j.Ternak.Status == StatusTernak.Aktif).ToList();

        if (awal && pilih is { } pid && kandang.Any(k => k.Id == pid)) TerpilihId = pid;
        awal = false;
        if (!kandang.Any(k => k.Id == TerpilihId)) TerpilihId = kandang.FirstOrDefault()?.Id ?? 0;

        var tot = kandang.Sum(k => k.Kapasitas);
        var isiTot = aktif.Count(t => t.KandangId != null);
        Subjudul = $"{kandang.Count} kandang · {Format.Angka(isiTot)} dari {Format.Angka(tot)} tempat terisi ({AturanBisnis.PersenTerisi(isiTot, tot)}%)";
        Segarkan();
    }

    void Segarkan()
    {
        Petak = kandang.Select(k =>
        {
            var isi = aktif.Count(t => t.KandangId == k.Id);
            var pct = AturanBisnis.PersenTerisi(isi, k.Kapasitas);
            return new KartuPetak(k.Id, k.Kode, k.Nama, $"{k.Spesies?.Nama ?? "Campuran"} · {k.Jenis}", isi, k.Kapasitas, pct, AturanBisnis.TingkatKeterisian(pct), k.Id == TerpilihId);
        }).ToList();

        var sel = kandang.FirstOrDefault(k => k.Id == TerpilihId);
        if (sel is null) return;
        var daftar = aktif.Where(t => t.KandangId == sel.Id).ToList();
        var persen = AturanBisnis.PersenTerisi(daftar.Count, sel.Kapasitas);
        var hari = DateTime.Today;
        JudulPanel = $"Kandang {sel.Kode} · {sel.Nama}";
        SubPanel = $"{sel.Jenis} · {daftar.Count} dari {sel.Kapasitas} ekor";
        PersenPanel = persen;
        TonePanel = AturanBisnis.TingkatKeterisian(persen);
        IsiPanel = daftar.Count;
        PakanHariIni = pakanLog.Where(l => l.KandangId == sel.Id).Select(l => new BarisPakan(l.Pakan.Nama, $"{Format.Angka(l.Jumlah)} {l.Pakan.Satuan} · {Format.Jam(l.Waktu)}")).ToList();
        JumlahSakit = daftar.Count(t => t.Kondisi != Kondisi.Sehat);
        var next = jadwal.Where(j => j.Ternak.KandangId == sel.Id).OrderBy(j => j.TanggalRencana).FirstOrDefault();
        VaksinBerikut = next is null ? "—" : $"{next.ObatVaksin.Nama.Replace("Vaksin ", "")} · {AturanBisnis.HariRelatif(next.TanggalRencana, hari)}";
        TernakPanel = daftar.Take(6).Select(t => new BarisTernak(t.Id, t.Label, berat.TryGetValue(t.Id, out var b) ? $"{Format.Angka(b, 1)} kg" : "",
            t.Kondisi switch { Kondisi.Diobati => "Diobati", Kondisi.Periksa => "Perlu diperiksa", _ => "" },
            t.Kondisi == Kondisi.Diobati ? "danger" : "warn", t.Kondisi != Kondisi.Sehat)).ToList();
    }

    [RelayCommand]
    void Pilih(int id)
    {
        TerpilihId = id;
        Segarkan();
    }

    [RelayCommand] void BeriPakan() => Shell.Buka("pakan", TerpilihId);
    [RelayCommand] void LihatTernak() => Shell.Buka("ternak", new TernakFilter(KandangId: TerpilihId));
    [RelayCommand] void BukaTernak(int id) => Shell.Buka("detail", id);

    [RelayCommand]
    void TambahKandang()
    {
        if (Session.BolehMaster) Shell.TampilDialog(new KandangDialogViewModel(Shell));
    }
}
