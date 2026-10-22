using System.IO;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using TernakKu.App.Services;
using TernakKu.App.ViewModels.Dialogs;
using TernakKu.Core.Models;
using TernakKu.Core.Rules;

namespace TernakKu.App.ViewModels.Pages;

public partial class TernakViewModel(ShellViewModel shell, TernakFilter? filter) : PageViewModel(shell), IPunyaTambah
{
    public record OpsiAngka(int Nilai, string Teks);
    public record OpsiTeks(string Nilai, string Teks);
    public record ChipSpesies(string Id, string Label, int Jumlah, bool Aktif);
    public record Baris(int Id, string Kode, string Nama, string Ras, string Spesies, string Kelamin, string Umur, string Kandang, string Berat, string Kondisi, string Tone);
    public record TombolHal(string Teks, int No, bool Aktif, bool BisaDiklik);

    const int PerHalaman = 12;

    sealed record Data(Ternak T, double? Berat);

    public override string Key => "ternak";

    [ObservableProperty] private string subjudul = "";
    [ObservableProperty] private string cari = "";
    [ObservableProperty] private int kandangId;
    [ObservableProperty] private string status = "Aktif";
    [ObservableProperty] private string spesiesId = "all";
    [ObservableProperty] private int halaman = 1;
    [ObservableProperty] private int totalHalaman = 1;
    [ObservableProperty] private string ringkas = "";
    [ObservableProperty] private List<Baris> daftar = [];
    [ObservableProperty] private List<ChipSpesies> chips = [];
    [ObservableProperty] private List<TombolHal> tombolHalaman = [];
    [ObservableProperty] private List<OpsiAngka> opsiKandang = [];

    public List<OpsiTeks> OpsiStatus { get; } = [new("Aktif", "Aktif"), new("Terjual", "Terjual"), new("Mati", "Mati"), new("Semua", "Semua status")];
    public bool BolehTambah => Session.BolehTambahTernak;

    List<Data> semua = [];
    List<Spesies> spesiesList = [];
    List<Data> hasil = [];
    bool dimuat;

    partial void OnCariChanged(string value) { Halaman = 1; Terapkan(); }
    partial void OnKandangIdChanged(int value) { Halaman = 1; Terapkan(); }
    partial void OnStatusChanged(string value) { Halaman = 1; Terapkan(); }
    partial void OnSpesiesIdChanged(string value) { Halaman = 1; Terapkan(); }

    public override void Load()
    {
        var pertama = !dimuat;
        using (var db = Session.Buka())
        {
            var list = db.Ternak.Include(t => t.Ras).ThenInclude(r => r.Spesies).Include(t => t.Kandang).ToList();
            var terakhir = db.Penimbangan.AsEnumerable().GroupBy(p => p.TernakId)
                .ToDictionary(g => g.Key, g => g.OrderBy(p => p.Tanggal).ThenBy(p => p.Id).Last().BeratKg);
            semua = list.Select(t => new Data(t, terakhir.TryGetValue(t.Id, out var b) ? b : null)).ToList();
            spesiesList = db.Spesies.OrderBy(s => s.Id).ToList();
            var kandang = db.Kandang.OrderBy(k => k.Kode).ToList();
            OpsiKandang = [new(0, "Semua kandang"), .. kandang.Select(k => new OpsiAngka(k.Id, k.Label))];
            var nAktif = list.Count(t => t.Status == StatusTernak.Aktif);
            Subjudul = $"{Format.Angka(nAktif)} ekor aktif di {kandang.Count} kandang";
        }

        if (pertama)
        {
            dimuat = true;
            if (filter is not null)
            {
                Cari = filter.Cari ?? "";
                KandangId = filter.KandangId ?? 0;
                SpesiesId = filter.SpesiesId?.ToString() ?? "all";
                Status = filter.Status?.ToString() ?? "Aktif";
            }
        }
        Terapkan();
        if (pertama && filter?.BukaTambah == true) Tambah();
    }

    void Terapkan()
    {
        if (!dimuat) return;
        var q = Cari.Trim().ToLowerInvariant();
        var basis = semua.Where(d => (Status == "Semua" || d.T.Status.ToString() == Status)
            && (KandangId == 0 || d.T.KandangId == KandangId)
            && (q.Length == 0 || $"{d.T.KodeTernak} {d.T.Nama} {d.T.Ras.Nama} {d.T.Ras.Spesies.Nama}".ToLowerInvariant().Contains(q))).ToList();

        Chips = [new("all", "Semua", basis.Count, SpesiesId == "all"),
            .. spesiesList.Select(s => new ChipSpesies(s.Id.ToString(), s.Nama, basis.Count(d => d.T.Ras.SpesiesId == s.Id), SpesiesId == s.Id.ToString()))];

        hasil = basis.Where(d => SpesiesId == "all" || d.T.Ras.SpesiesId.ToString() == SpesiesId)
            .OrderBy(d => d.T.Ras.SpesiesId).ThenBy(d => d.T.KodeTernak, StringComparer.Ordinal).ToList();
        TotalHalaman = Math.Max(1, (int)Math.Ceiling(hasil.Count / (double)PerHalaman));
        if (Halaman > TotalHalaman) Halaman = TotalHalaman;
        if (Halaman < 1) Halaman = 1;
        var hari = DateTime.Today;
        var irisan = hasil.Skip((Halaman - 1) * PerHalaman).Take(PerHalaman).ToList();
        Daftar = irisan.Select(d =>
        {
            var t = d.T;
            var (kondisi, tone) = t.Status != StatusTernak.Aktif ? (t.Status.ToString(), "neutral")
                : t.Kondisi switch { Kondisi.Sehat => ("Sehat", "ok"), Kondisi.Diobati => ("Diobati", "danger"), _ => ("Perlu diperiksa", "warn") };
            return new Baris(t.Id, t.KodeTernak, string.IsNullOrEmpty(t.Nama) ? "—" : t.Nama, t.Ras.Nama, t.Ras.Spesies.Nama, t.JenisKelamin.ToString(),
                AturanBisnis.Umur(t.TanggalLahir, hari), t.Kandang?.Label ?? "—", d.Berat is { } b ? $"{Format.Angka(b, 1)} kg" : "—", kondisi, tone);
        }).ToList();
        Ringkas = $"Menampilkan {irisan.Count} dari {Format.Angka(hasil.Count)} ternak · halaman {Halaman} dari {TotalHalaman}";

        var tombol = new List<TombolHal>();
        var sebelumnya = 0;
        for (var i = 1; i <= TotalHalaman; i++)
        {
            if (i == 1 || i == TotalHalaman || Math.Abs(i - Halaman) <= 1) { tombol.Add(new(i.ToString(), i, i == Halaman, true)); sebelumnya = i; }
            else if (sebelumnya != -1) { tombol.Add(new("…", 0, false, false)); sebelumnya = -1; }
        }
        TombolHalaman = tombol;
        PrevCommand.NotifyCanExecuteChanged();
        NextCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand] void PilihSpesies(string id) => SpesiesId = id;

    [RelayCommand]
    void KePage(int no)
    {
        if (no < 1 || no > TotalHalaman || no == Halaman) return;
        Halaman = no;
        Terapkan();
    }

    [RelayCommand(CanExecute = nameof(BisaPrev))] void Prev() => KePage(Halaman - 1);
    [RelayCommand(CanExecute = nameof(BisaNext))] void Next() => KePage(Halaman + 1);
    bool BisaPrev() => Halaman > 1;
    bool BisaNext() => Halaman < TotalHalaman;

    [RelayCommand] void Buka(int id) => Shell.Buka("detail", id);

    [RelayCommand]
    void Tambah()
    {
        if (!Session.BolehTambahTernak) return;
        Shell.TampilDialog(new TambahTernakViewModel(Shell));
    }

    [RelayCommand]
    void Ekspor()
    {
        try
        {
            var folder = Path.Combine(Environment.GetEnvironmentVariable("USERPROFILE") ?? Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Documents", "TernakKu");
            Directory.CreateDirectory(folder);
            var path = Path.Combine(folder, $"data-ternak-{DateTime.Today:yyyy-MM-dd}.csv");
            static string Sel(string s) => s.Contains(';') || s.Contains('"') || s.Contains('\n') ? $"\"{s.Replace("\"", "\"\"")}\"" : s;
            var sb = new StringBuilder();
            sb.AppendLine("Kode;Nama;Ras;Spesies;Kelamin;Tanggal lahir;Umur;Kandang;Berat terakhir (kg);Status;Kondisi");
            var hari = DateTime.Today;
            foreach (var d in hasil)
            {
                var t = d.T;
                sb.AppendLine(string.Join(';', new[]
                {
                    t.KodeTernak, t.Nama, t.Ras.Nama, t.Ras.Spesies.Nama, t.JenisKelamin.ToString(), t.TanggalLahir.ToString("yyyy-MM-dd"),
                    AturanBisnis.Umur(t.TanggalLahir, hari), t.Kandang?.Label ?? "", d.Berat is { } b ? b.ToString("0.0", Format.Id) : "", t.Status.ToString(), t.Kondisi.ToString()
                }.Select(Sel)));
            }
            File.WriteAllText(path, sb.ToString(), new UTF8Encoding(true));
            Shell.TampilSnack($"Data ternak diekspor ke {path}");
        }
        catch (Exception ex)
        {
            Shell.TampilSnack($"Ekspor gagal: {ex.Message}");
        }
    }
}
