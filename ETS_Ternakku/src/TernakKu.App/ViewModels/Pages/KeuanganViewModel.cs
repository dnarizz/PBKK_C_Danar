using System.IO;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TernakKu.App.Services;
using TernakKu.App.ViewModels.Dialogs;
using TernakKu.Core.Models;

namespace TernakKu.App.ViewModels.Pages;

public record KartuStat(string Judul, string Nilai, string Sub, string Tone, string Ikon, bool Berwarna);

public record BarKategori(string Nama, string Jumlah, double Persen);

public record BarisKeuangan(string Tanggal, string Keterangan, string Kategori, string Sumber, string SumberTone, string Jumlah, bool Masuk);

// Semicolon-separated CSV with a BOM so Excel opens it with the right encoding.
public static class CsvTulis
{
    public static void Simpan(string path, IEnumerable<string> kepala, IEnumerable<IEnumerable<string>> baris)
    {
        static string Sel(string s) => s.IndexOfAny([';', '"', '\n', '\r']) >= 0 ? $"\"{s.Replace("\"", "\"\"")}\"" : s;
        var sb = new StringBuilder();
        sb.AppendLine(string.Join(';', kepala.Select(Sel)));
        foreach (var b in baris) sb.AppendLine(string.Join(';', b.Select(Sel)));
        File.WriteAllText(path, sb.ToString(), new UTF8Encoding(true));
    }
}

public partial class KeuanganViewModel(ShellViewModel shell) : PageViewModel(shell), IPunyaTambah
{
    public override string Key => "keuangan";

    [ObservableProperty] private string rentang = "1";
    [ObservableProperty] private string jenisFilter = "Semua";
    [ObservableProperty] private List<KartuStat> kartu = [];
    [ObservableProperty] private List<double> arusMasuk = [];
    [ObservableProperty] private List<double> arusKeluar = [];
    [ObservableProperty] private List<string> labelBulan = [];
    [ObservableProperty] private List<BarKategori> kategori = [];
    [ObservableProperty] private List<BarisKeuangan> baris = [];

    List<Keuangan> tampil = [];

    partial void OnRentangChanged(string value) => Load();
    partial void OnJenisFilterChanged(string value) => Load();

    public override void Load()
    {
        var hari = DateTime.Today;
        var bulanIni = new DateTime(hari.Year, hari.Month, 1);
        using var db = Session.Buka();
        var semua = db.Keuangan.ToList();

        var mulai = bulanIni.AddMonths(-(int.Parse(Rentang) - 1));
        var dalam = semua.Where(x => x.Tanggal.Date >= mulai && x.Tanggal.Date <= hari).ToList();
        var masuk = dalam.Where(x => x.Jenis == JenisKeuangan.Masuk).ToList();
        var keluar = dalam.Where(x => x.Jenis == JenisKeuangan.Keluar).ToList();
        var m = masuk.Sum(x => x.Jumlah);
        var k = keluar.Sum(x => x.Jumlah);
        var laba = m - k;
        Kartu =
        [
            new("Pemasukan", Format.RupiahPendek(m), $"{masuk.Count} transaksi", "ok", "download", false),
            new("Pengeluaran", Format.RupiahPendek(k), $"{keluar.Count} transaksi", "warn", "upload", false),
            new("Laba bersih", Format.RupiahPendek(laba), m > 0 ? $"Margin {Math.Round(laba * 100.0 / m)}%" : "—", laba >= 0 ? "ok" : "danger", "trend", true)
        ];

        var bulan = Enumerable.Range(0, 6).Select(i => bulanIni.AddMonths(i - 5)).ToList();
        double Total(DateTime b, JenisKeuangan j) => semua.Where(x => x.Jenis == j && x.Tanggal.Year == b.Year && x.Tanggal.Month == b.Month).Sum(x => x.Jumlah);
        ArusMasuk = bulan.Select(b => Total(b, JenisKeuangan.Masuk)).ToList();
        ArusKeluar = bulan.Select(b => Total(b, JenisKeuangan.Keluar)).ToList();
        LabelBulan = bulan.Select(b => b.ToString("MMM", Format.Id)).ToList();

        Kategori = keluar.GroupBy(x => x.Kategori).Select(g => (Nama: g.Key, Jumlah: g.Sum(x => x.Jumlah))).OrderByDescending(g => g.Jumlah)
            .Select(g => new BarKategori(g.Nama, Format.RupiahPendek(g.Jumlah), k > 0 ? g.Jumlah * 100.0 / k : 0)).ToList();

        tampil = dalam.Where(x => JenisFilter == "Semua" || x.Jenis.ToString() == JenisFilter).OrderByDescending(x => x.Tanggal).ThenByDescending(x => x.Id).ToList();
        Baris = tampil.Select(x =>
        {
            var (sumber, tone) = x.SumberTabel switch
            {
                "PakanMasuk" => ("Otomatis · pakan", "info"),
                "Mutasi" => ("Otomatis · penjualan", "info"),
                _ => ("Manual", "neutral")
            };
            var masukan = x.Jenis == JenisKeuangan.Masuk;
            return new BarisKeuangan(Format.Tanggal(x.Tanggal), x.Keterangan, x.Kategori, sumber, tone, $"{(masukan ? "+" : "−")}{Format.Rupiah(x.Jumlah)}", masukan);
        }).ToList();
    }

    [RelayCommand]
    void Tambah() => Shell.TampilDialog(new TransaksiViewModel(Shell));

    [RelayCommand]
    void Ekspor()
    {
        var dlg = new Microsoft.Win32.SaveFileDialog
        {
            FileName = $"keuangan-{DateTime.Today:yyyy-MM-dd}.csv",
            Filter = "CSV (dibuka di Excel)|*.csv",
            DefaultExt = ".csv"
        };
        if (dlg.ShowDialog() != true) return;
        CsvTulis.Simpan(dlg.FileName, ["Tanggal", "Keterangan", "Kategori", "Sumber", "Jenis", "Jumlah"],
            tampil.Select(x => (IEnumerable<string>)[x.Tanggal.ToString("yyyy-MM-dd"), x.Keterangan, x.Kategori, x.SumberTabel, x.Jenis.ToString(), x.Jumlah.ToString()]));
        Shell.TampilSnack($"Tersimpan: {Path.GetFileName(dlg.FileName)}");
    }
}
