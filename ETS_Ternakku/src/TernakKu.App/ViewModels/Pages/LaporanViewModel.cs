using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using TernakKu.App.Services;
using TernakKu.Core.Models;
using TernakKu.Core.Rules;

namespace TernakKu.App.ViewModels.Pages;

// One cell of the report table. Gaya: "kepala", "isi" or "kaki".
public record SelLaporan(string Teks, bool Kanan, string Gaya);

public partial class LaporanViewModel(ShellViewModel shell) : PageViewModel(shell)
{
    public override string Key => "laporan";

    [ObservableProperty] private string jenis = "ternak";
    [ObservableProperty] private DateTime? dari = DateTime.Today.AddDays(-90);
    [ObservableProperty] private DateTime? sampai = DateTime.Today;

    [ObservableProperty] private string judulLaporan = "";
    [ObservableProperty] private string periodeTeks = "";
    [ObservableProperty] private List<SelLaporan> kepala = [];
    [ObservableProperty] private List<List<SelLaporan>> baris = [];
    [ObservableProperty] private List<SelLaporan> kaki = [];
    [ObservableProperty] private bool kosong;

    public string NamaPeternakan => Shell.NamaPeternakan;
    public string Dicetak => $"Dicetak {Format.Tanggal(DateTime.Today)} oleh {Session.Nama}";

    partial void OnJenisChanged(string value) => Load();
    partial void OnDariChanged(DateTime? value) => Load();
    partial void OnSampaiChanged(DateTime? value) => Load();

    public override void Load()
    {
        var dari = (Dari ?? DateTime.Today.AddDays(-90)).Date;
        var sampai = (Sampai ?? DateTime.Today).Date;
        PeriodeTeks = $"{Format.Tanggal(dari)} – {Format.Tanggal(sampai)}";
        using var db = Session.Buka();
        bool Dalam(DateTime d) => d.Date >= dari && d.Date <= sampai;

        // Text-heavy reports keep every column left aligned; numeric reports right-align all but the first.
        var kananSemua = true;
        SelLaporan Sel(string t, int i, string gaya) => new(t, i > 0 && kananSemua, gaya);

        List<string> head;
        List<List<string>> rows;
        List<string>? foot = null;

        switch (Jenis)
        {
            case "kesehatan":
            {
                JudulLaporan = "Laporan kesehatan";
                kananSemua = false;
                head = ["Tanggal", "Ternak", "Diagnosa", "Tindakan"];
                var r = db.Kesehatan.Include(x => x.Ternak).AsEnumerable().Where(x => Dalam(x.Tanggal)).OrderBy(x => x.Tanggal).ThenBy(x => x.Id).ToList();
                rows = r.Take(14).Select(x => new List<string> { Format.TanggalPendek(x.Tanggal), x.Ternak.KodeTernak, x.Diagnosa, x.Tindakan }).ToList();
                foot = [$"{r.Count} pemeriksaan", "", "", ""];
                break;
            }
            case "keuangan":
            {
                JudulLaporan = "Laporan laba-rugi";
                head = ["Bulan", "Pemasukan", "Pengeluaran", "Laba"];
                var r = db.Keuangan.AsEnumerable().Where(x => Dalam(x.Tanggal)).ToList();
                rows = r.GroupBy(x => new DateTime(x.Tanggal.Year, x.Tanggal.Month, 1)).OrderBy(g => g.Key).Select(g =>
                {
                    var a = g.Where(x => x.Jenis == JenisKeuangan.Masuk).Sum(x => x.Jumlah);
                    var b = g.Where(x => x.Jenis == JenisKeuangan.Keluar).Sum(x => x.Jumlah);
                    return new List<string> { g.Key.ToString("MMMM yyyy", Format.Id), Format.Rupiah(a), Format.Rupiah(b), Format.Rupiah(a - b) };
                }).ToList();
                var ta = r.Where(x => x.Jenis == JenisKeuangan.Masuk).Sum(x => x.Jumlah);
                var tb = r.Where(x => x.Jenis == JenisKeuangan.Keluar).Sum(x => x.Jumlah);
                foot = ["Total", Format.Rupiah(ta), Format.Rupiah(tb), Format.Rupiah(ta - tb)];
                break;
            }
            case "pakan":
            {
                JudulLaporan = "Laporan stok pakan";
                head = ["Pakan", "Masuk", "Stok akhir", "Status"];
                var masuk = db.PakanMasuk.AsEnumerable().Where(x => Dalam(x.Tanggal)).GroupBy(x => x.PakanId).ToDictionary(g => g.Key, g => g.Sum(x => x.Jumlah));
                rows = db.Pakan.OrderBy(p => p.Id).AsEnumerable().Select(p => new List<string>
                {
                    p.Nama, $"{Format.Angka(masuk.GetValueOrDefault(p.Id))} {p.Satuan}", $"{Format.Angka(p.Stok)} {p.Satuan}",
                    AturanBisnis.HitungStatusStok(p.Stok, p.StokMinimum) switch { StatusStok.Aman => "Aman", StatusStok.Menipis => "Menipis", _ => "Habis" }
                }).ToList();
                break;
            }
            default:
            {
                JudulLaporan = "Laporan data ternak";
                var spesies = db.Spesies.OrderBy(s => s.Id).Take(3).ToList();
                head = ["Kandang", .. spesies.Select(s => s.Nama), "Total"];
                var aktif = db.Ternak.Include(t => t.Ras).Where(t => t.Status == StatusTernak.Aktif).ToList();
                rows = db.Kandang.OrderBy(k => k.Kode).AsEnumerable().Select(k =>
                {
                    var l = aktif.Where(t => t.KandangId == k.Id).ToList();
                    List<string> baris = [$"{k.Kode} · {k.Nama}", .. spesies.Select(s => l.Count(t => t.Ras.SpesiesId == s.Id).ToString()), l.Count.ToString()];
                    return baris;
                }).ToList();
                foot = ["Total", .. spesies.Select(s => aktif.Count(t => t.Ras.SpesiesId == s.Id).ToString()), aktif.Count.ToString()];
                break;
            }
        }

        Kepala = head.Select((h, i) => Sel(h, i, "kepala")).ToList();
        Baris = rows.Select(r => r.Select((c, i) => Sel(c, i, "isi")).ToList()).ToList();
        Kaki = foot?.Select((c, i) => Sel(c, i, "kaki")).ToList() ?? [];
        Kosong = rows.Count == 0;
        kepalaCsv = head;
        barisCsv = rows;
        kakiCsv = foot;
    }

    List<string> kepalaCsv = [];
    List<List<string>> barisCsv = [];
    List<string>? kakiCsv;

    [RelayCommand]
    void Ekspor()
    {
        var dlg = new Microsoft.Win32.SaveFileDialog
        {
            FileName = $"laporan-{Jenis}-{DateTime.Today:yyyy-MM-dd}.csv",
            Filter = "CSV (dibuka di Excel)|*.csv",
            DefaultExt = ".csv"
        };
        if (dlg.ShowDialog() != true) return;
        var isi = barisCsv.Select(r => (IEnumerable<string>)r).ToList();
        if (kakiCsv is not null) isi.Add(kakiCsv);
        CsvTulis.Simpan(dlg.FileName, kepalaCsv, isi);
        Shell.TampilSnack($"Tersimpan: {Path.GetFileName(dlg.FileName)}");
    }

    // Called by the view after the print dialog (for example "Microsoft Print to PDF") has sent the page.
    public void SelesaiCetak() => Shell.TampilSnack("Laporan dikirim ke pencetak");
}
