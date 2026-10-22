using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using TernakKu.App.Services;
using TernakKu.App.ViewModels.Dialogs;
using TernakKu.Core.Models;
using TernakKu.Core.Rules;

namespace TernakKu.App.ViewModels.Pages;

public record KartuStatKesehatan(string Judul, string Nilai, string Satuan, string Sub, bool SubBahaya, string Tone, string Ikon);

public record BarisPerhatian(int TernakId, string Label, string Kandang, string Kondisi, string Tone, string Catatan);

public record BarisRiwayatSehat(int TernakId, string Tanggal, string Kode, string Keluhan, string Diagnosa, string Tindakan);

// One cell of the month calendar. Pips hold the status tones of that day's schedules (warn, ok, danger).
public record HariKalender(DateTime Tanggal, string Angka, bool LuarBulan, bool HariIni, bool Dipilih, List<string> Pip);

public record JadwalGrup(string Judul, string Sub, string Status, string Tone, bool BisaSelesai, List<int> Ids);

public record MeterObat(string Nama, string Stok, double Persen, bool Rendah);

public partial class KesehatanViewModel(ShellViewModel shell) : PageViewModel(shell), IPunyaTambah
{
    public override string Key => "kesehatan";
    public bool BolehKesehatan => Session.BolehKesehatan;

    [ObservableProperty] private List<KartuStatKesehatan> statistik = [];
    [ObservableProperty] private List<BarisPerhatian> perhatian = [];
    [ObservableProperty] private List<BarisRiwayatSehat> riwayat = [];
    [ObservableProperty] private string judulBulan = "";
    [ObservableProperty] private List<HariKalender> hari = [];
    [ObservableProperty] private string judulHari = "";
    [ObservableProperty] private List<JadwalGrup> jadwalHari = [];
    [ObservableProperty] private List<MeterObat> obat = [];

    DateTime bulan = new(DateTime.Today.Year, DateTime.Today.Month, 1);
    DateTime hariDipilih = DateTime.Today;

    public IRelayCommand TambahCommand => tambah ??= new RelayCommand(() => Shell.TampilDialog(new PeriksaViewModel(Shell, null)), () => Session.BolehKesehatan);
    IRelayCommand? tambah;

    // Used by the schedule dialog so the calendar shows the new schedule.
    public void LompatKe(DateTime tanggal)
    {
        bulan = new DateTime(tanggal.Year, tanggal.Month, 1);
        hariDipilih = tanggal.Date;
    }

    public override void Load()
    {
        var sekarang = DateTime.Today;
        using var db = Session.Buka();
        var aktif = db.Ternak.Include(t => t.Kandang).Where(t => t.Status == StatusTernak.Aktif).ToList();
        var jadwal = db.JadwalVaksinasi.Include(j => j.ObatVaksin).Include(j => j.Ternak).ThenInclude(t => t.Kandang)
            .Where(j => j.Ternak.Status == StatusTernak.Aktif).ToList();
        var daftarObat = db.ObatVaksin.OrderBy(o => o.Id).ToList();
        var catatan = db.Kesehatan.Include(k => k.Ternak).AsEnumerable().OrderByDescending(k => k.Tanggal).ThenByDescending(k => k.Id).ToList();

        // Stat cards
        var diobati = aktif.Where(t => t.Kondisi == Kondisi.Diobati).ToList();
        var periksa = aktif.Where(t => t.Kondisi == Kondisi.Periksa).ToList();
        var vak7 = jadwal.Count(j => j.Status == StatusJadwal.Terjadwal && (j.TanggalRencana.Date - sekarang).TotalDays is >= 0 and <= 7);
        var lewat = jadwal.Count(j => AturanBisnis.StatusJadwalHariIni(j, sekarang) == StatusJadwal.Terlewat);
        var tipis = daftarObat.Where(o => o.Stok < o.StokMinimum).ToList();
        Statistik =
        [
            new("Sedang diobati", diobati.Count.ToString(), "ekor", "Dalam masa pengobatan", false, "danger", "heart"),
            new("Perlu diperiksa", periksa.Count.ToString(), "ekor", "Dilaporkan petugas kandang", false, "warn", "alert"),
            new("Vaksin 7 hari ke depan", vak7.ToString(), "ekor", lewat > 0 ? $"{lewat} terlewat" : "Tidak ada yang terlewat", lewat > 0, "info", "syringe"),
            new("Stok obat menipis", tipis.Count.ToString(), "jenis", tipis.Count > 0 ? string.Join(", ", tipis.Select(o => o.Nama.Split(" (")[0])) : "Semua aman", false, "warn", "box")
        ];

        // Animals needing attention
        Perhatian = diobati.Concat(periksa).Select(t =>
        {
            var r = catatan.FirstOrDefault(k => k.TernakId == t.Id);
            return new BarisPerhatian(t.Id, t.Label, t.Kandang?.Label ?? "—", t.Kondisi == Kondisi.Diobati ? "Diobati" : "Perlu diperiksa",
                t.Kondisi == Kondisi.Diobati ? "danger" : "warn", r is null ? "Laporan petugas" : $"{r.Diagnosa} · {Format.TanggalPendek(r.Tanggal)}");
        }).ToList();
        Riwayat = catatan.Take(12).Select(r => new BarisRiwayatSehat(r.TernakId, Format.TanggalPendek(r.Tanggal), r.Ternak.KodeTernak, r.Keluhan, r.Diagnosa, r.Tindakan)).ToList();

        // Calendar
        JudulBulan = bulan.ToString("MMMM yyyy", Format.Id);
        var awal = bulan.AddDays(-(((int)bulan.DayOfWeek + 6) % 7));
        var perHari = jadwal.GroupBy(j => j.TanggalRencana.Date).ToDictionary(g => g.Key, g => g.ToList());
        Hari = Enumerable.Range(0, 42).Select(i =>
        {
            var d = awal.AddDays(i);
            var pip = perHari.TryGetValue(d, out var js)
                ? js.Select(j => AturanBisnis.StatusJadwalHariIni(j, sekarang) switch { StatusJadwal.Selesai => "ok", StatusJadwal.Terlewat => "danger", _ => "warn" }).Distinct().Take(3).ToList()
                : [];
            return new HariKalender(d, d.Day.ToString(), d.Month != bulan.Month, d == sekarang, d == hariDipilih, pip);
        }).ToList();

        JudulHari = Format.TanggalPanjang(hariDipilih);
        var hariIni = perHari.TryGetValue(hariDipilih, out var daftar) ? daftar : [];
        JadwalHari = hariIni
            .GroupBy(j => (j.ObatVaksinId, j.Ternak.KandangId, Status: AturanBisnis.StatusJadwalHariIni(j, sekarang)))
            .Select(g =>
            {
                var j = g.First();
                var (teks, tone) = g.Key.Status switch { StatusJadwal.Selesai => ("Selesai", "ok"), StatusJadwal.Terlewat => ("Terlewat", "danger"), _ => ("Terjadwal", "info") };
                return new JadwalGrup(j.ObatVaksin.Nama, $"Kandang {j.Ternak.Kandang?.Kode ?? "—"} · {g.Count()} ekor", teks, tone,
                    j.Status == StatusJadwal.Terjadwal && Session.BolehKesehatan, g.Select(x => x.Id).ToList());
            }).ToList();

        Obat = daftarObat.Select(o => new MeterObat(o.Nama, $"{Format.Angka(o.Stok)} {o.Satuan}", o.StokMinimum > 0 ? Math.Clamp(o.Stok / (o.StokMinimum * 3) * 100, 0, 100) : 0, o.Stok < o.StokMinimum)).ToList();
    }

    [RelayCommand] void BukaTernak(int id) => Shell.Buka("detail", id);

    [RelayCommand] void Jadwalkan() => Shell.TampilDialog(new JadwalVaksinViewModel(Shell));

    [RelayCommand] void Periksa(int id) => Shell.TampilDialog(new PeriksaViewModel(Shell, id));

    [RelayCommand]
    void Bulan(string arah)
    {
        bulan = bulan.AddMonths(int.Parse(arah));
        Load();
    }

    [RelayCommand]
    void PilihHari(DateTime tanggal)
    {
        hariDipilih = tanggal.Date;
        if (tanggal.Month != bulan.Month || tanggal.Year != bulan.Year) bulan = new DateTime(tanggal.Year, tanggal.Month, 1);
        Load();
    }

    [RelayCommand]
    void TandaiSehat(int id)
    {
        Kondisi sebelum;
        string kode;
        using (var db = Session.Buka())
        {
            var t = db.Ternak.Find(id);
            if (t is null) return;
            sebelum = t.Kondisi;
            kode = t.KodeTernak;
            t.Kondisi = Kondisi.Sehat;
            db.SaveChanges();
        }
        Shell.TampilSnack($"{kode} ditandai sehat", () =>
        {
            using var db = Session.Buka();
            var t = db.Ternak.Find(id);
            if (t is not null) { t.Kondisi = sebelum; db.SaveChanges(); }
        });
        Load();
        Shell.SegarkanStatus();
    }

    [RelayCommand]
    void Selesai(List<int> ids)
    {
        using (var db = Session.Buka())
        {
            foreach (var j in db.JadwalVaksinasi.Where(j => ids.Contains(j.Id)))
            {
                j.Status = StatusJadwal.Selesai;
                j.TanggalRealisasi = DateTime.Today;
            }
            db.SaveChanges();
        }
        Shell.TampilSnack($"Vaksinasi {ids.Count} ekor ditandai selesai", () =>
        {
            using var db = Session.Buka();
            foreach (var j in db.JadwalVaksinasi.Where(j => ids.Contains(j.Id))) { j.Status = StatusJadwal.Terjadwal; j.TanggalRealisasi = null; }
            db.SaveChanges();
        });
        Load();
        Shell.SegarkanStatus();
    }
}
