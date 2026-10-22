using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using TernakKu.App.Services;
using TernakKu.Core.Models;
using TernakKu.Core.Rules;

namespace TernakKu.App.ViewModels.Pages;

public partial class TugasItem : ObservableObject
{
    public string Kunci { get; init; } = "";
    public string Jam { get; init; } = "";
    public string Ikon { get; init; } = "";
    public string Judul { get; init; } = "";
    public string Sub { get; init; } = "";
    public bool Selesai { get; init; }
    public bool Terlambat { get; init; }
    public bool BisaDitandai { get; init; }
    public string? Tombol { get; init; }
    public Action? AksiTombol { get; init; }
    public Action? AksiTandai { get; init; }
    public bool AdaTombol => Tombol is not null;
}

public record PenandaSekarang(string Jam);

public record Pengingat(string Kunci, string Tone, string Ikon, string Judul, string Sub, string? Tombol, Action? Aksi, bool Selesai, int Urutan)
{
    public bool AdaTombol => Tombol is not null;
}

public record PetakKandang(int Id, string Kode, string Nama, int Isi, int Kapasitas, int Persen, string Tone, int Sakit)
{
    public string Teks => $"{Isi}/{Kapasitas}";
    public string Tip => $"Kandang {Kode} · {Nama}: {Isi} dari {Kapasitas} ekor";
}

public record Silo(string Nama, string Stok, double Persen, bool Rendah, string Tip);

public record SegeraLahir(int TernakId, string Hitung, bool Dekat, string Kode, string Sub);

public record BarisTimbang(int TernakId, string Kode, string Ras, string Berat, string Adg, string Tone, string Tanggal);

public partial class DashboardViewModel(ShellViewModel shell) : PageViewModel(shell)
{
    public override string Key => "dashboard";

    [ObservableProperty] private string sapaan = "";
    [ObservableProperty] private string tanggal = "";
    [ObservableProperty] private string ringkasHariIni = "";
    [ObservableProperty] private double progresTugas;
    [ObservableProperty] private List<object> tugas = [];

    [ObservableProperty] private int totalAktif;
    [ObservableProperty] private List<double> jumlahSpesies = [];
    [ObservableProperty] private int baruBulanIni;
    [ObservableProperty] private string filterPopulasi = "all";
    [ObservableProperty] private List<double> populasi = [];
    [ObservableProperty] private List<string> labelBulan = [];
    [ObservableProperty] private string judulPopulasi = "";
    [ObservableProperty] private string pertumbuhan = "";

    [ObservableProperty] private List<Pengingat> pengingat = [];
    [ObservableProperty] private int jumlahPengingat;
    [ObservableProperty] private List<PetakKandang> petak = [];
    [ObservableProperty] private List<PetakKandang> petakBaris2 = [];
    [ObservableProperty] private List<Silo> silo = [];
    [ObservableProperty] private int pakanRendah;
    [ObservableProperty] private List<SegeraLahir> lahir = [];
    [ObservableProperty] private int jumlahBunting;
    [ObservableProperty] private int diobati;
    [ObservableProperty] private int periksa;
    [ObservableProperty] private int vaksin7;
    [ObservableProperty] private string laba = "";
    [ObservableProperty] private bool labaPositif = true;
    [ObservableProperty] private List<double> trenLaba = [];
    [ObservableProperty] private string masuk = "";
    [ObservableProperty] private string keluar = "";
    [ObservableProperty] private List<BarisTimbang> timbang = [];

    public bool AdaPakan => Session.BolehBuka("pakan");
    public bool AdaKeuangan => Session.BolehBuka("keuangan");
    public bool AdaReproduksi => Session.BolehBuka("reproduksi");

    public void Cari(string teks) => Shell.Buka("ternak", new TernakFilter(Cari: teks.Trim()));
    public bool BolehTambah => Session.BolehTambahTernak;
    public bool IsTamu => Session.IsTamu;
    public string NamaFile => Session.NamaFile;

    // Bento column widths (stars). Missing tiles hand their width to the neighbours.
    public double KolomPen => AdaPakan ? 5 : 6;
    public double KolomPakan => AdaPakan ? 4 : 0;
    public double KolomLahir => AdaPakan ? 3 : 6;
    public double KolomSehat => AdaKeuangan ? 3 : 6;
    public double KolomUang => AdaKeuangan ? 3 : 0;
    public double KolomTimbang => 6;

    List<Ternak> semuaAktif = [];
    Dictionary<int, DateTime> keluarPada = [];

    public override void Load()
    {
        var hari = DateTime.Today;
        using var db = Session.Buka();
        var jam = DateTime.Now.Hour;
        Sapaan = $"{(jam < 4 || jam >= 18 ? "Selamat malam" : jam < 11 ? "Selamat pagi" : jam < 15 ? "Selamat siang" : "Selamat sore")}, {Session.Nama.Replace("drh. ", "").Split(' ')[0]}";
        Tanggal = Format.TanggalPanjang(hari);

        var aktif = db.Ternak.Include(t => t.Ras).ThenInclude(r => r.Spesies).Include(t => t.Kandang).Where(t => t.Status == StatusTernak.Aktif).ToList();
        semuaAktif = aktif;
        TotalAktif = aktif.Count;
        JumlahSpesies = db.Spesies.OrderBy(s => s.Id).Take(3).AsEnumerable().Select(s => (double)aktif.Count(t => t.Ras.SpesiesId == s.Id)).ToList();
        BaruBulanIni = aktif.Count(t => t.TanggalMasuk.Year == hari.Year && t.TanggalMasuk.Month == hari.Month);
        keluarPada = db.Mutasi.Where(m => m.Jenis != JenisMutasi.Pindah).AsEnumerable().GroupBy(m => m.TernakId).ToDictionary(g => g.Key, g => g.Max(m => m.Tanggal));
        HitungPopulasi(db);

        // Reminders
        var list = new List<Pengingat>();
        var jadwal = db.JadwalVaksinasi.Include(j => j.ObatVaksin).Include(j => j.Ternak).ThenInclude(t => t.Kandang)
            .Where(j => j.Ternak.Status == StatusTernak.Aktif).ToList();
        foreach (var g in jadwal.Where(j => AturanBisnis.JadwalPerluDiingat(j, hari)).GroupBy(j => (j.ObatVaksinId, j.Ternak.KandangId, j.TanggalRencana.Date)))
        {
            var j = g.First();
            var ids = g.Select(x => x.Id).ToList();
            list.Add(new Pengingat($"v{ids[0]}", "danger", "syringe", $"Vaksinasi {j.ObatVaksin.Nama.Replace("Vaksin ", "")}",
                $"Kandang {j.Ternak.Kandang?.Kode} · {ids.Count} ekor · {AturanBisnis.HariRelatif(j.TanggalRencana, hari)}",
                Session.BolehKesehatan ? "Selesai" : null, () => VaksinSelesai(ids), false, 1));
        }
        foreach (var g in jadwal.Where(j => AturanBisnis.StatusJadwalHariIni(j, hari) == StatusJadwal.Terlewat).GroupBy(j => (j.ObatVaksinId, j.Ternak.KandangId, j.TanggalRencana.Date)))
        {
            var j = g.First();
            list.Add(new Pengingat($"l{j.Id}", "danger", "alert", $"Vaksin {j.ObatVaksin.Nama.Replace("Vaksin ", "")} terlewat",
                $"Kandang {j.Ternak.Kandang?.Kode} · {g.Count()} ekor · {AturanBisnis.HariRelatif(j.TanggalRencana, hari)}",
                Session.BolehBuka("kesehatan") ? "Jadwalkan" : null, () => Shell.Buka("kesehatan"), false, 0));
        }
        var bunting = db.Perkawinan.Include(k => k.Induk).ThenInclude(t => t.Ras).ThenInclude(r => r.Spesies)
            .Where(k => k.Status == StatusBunting.Bunting).OrderBy(k => k.PerkiraanLahir).ToList();
        foreach (var k in bunting.Where(k => (k.PerkiraanLahir - hari).TotalDays is >= -3 and <= 7))
            list.Add(new Pengingat($"k{k.Id}", "ok", "calendar", $"Perkiraan lahir {k.Induk.KodeTernak}",
                $"{k.Induk.Ras.Spesies.Nama} {k.Induk.Ras.Nama} · {Format.Tanggal(k.PerkiraanLahir)}", "Lihat", () => Shell.Buka("detail", k.IndukId), false, 2));
        var pakan = db.Pakan.OrderBy(p => p.Id).ToList();
        foreach (var p in pakan.Where(p => p.Stok < p.StokMinimum))
            list.Add(new Pengingat($"p{p.Id}", "warn", "alert", $"{p.Nama} tinggal {Format.Angka(p.Stok)} {p.Satuan}",
                $"Stok minimum {Format.Angka(p.StokMinimum)} {p.Satuan}", AdaPakan ? "Pesan" : null, () => Shell.Buka("pakan"), false, 3));
        Pengingat = list.OrderBy(x => x.Urutan).Take(5).ToList();
        JumlahPengingat = list.Count;

        // Pens
        var kandang = db.Kandang.OrderBy(k => k.Kode).ToList();
        var petak = kandang.Select(k =>
        {
            var isi = aktif.Count(t => t.KandangId == k.Id);
            var pct = AturanBisnis.PersenTerisi(isi, k.Kapasitas);
            return new PetakKandang(k.Id, k.Kode, k.Nama, isi, k.Kapasitas, pct, AturanBisnis.TingkatKeterisian(pct), aktif.Count(t => t.KandangId == k.Id && t.Kondisi != Kondisi.Sehat));
        }).ToList();
        var half = (petak.Count + 1) / 2;
        Petak = petak.Take(half).ToList();
        PetakBaris2 = petak.Skip(half).ToList();

        // Feed silos
        Silo = pakan.Take(6).Select(p => new Silo(p.Nama.Split(' ')[0], $"{Format.Angka(p.Stok)} {p.Satuan}",
            Math.Min(100, p.Stok / (p.StokMinimum * 3) * 100), p.Stok < p.StokMinimum,
            $"{p.Nama}: {Format.Angka(p.Stok)} {p.Satuan}, minimum {Format.Angka(p.StokMinimum)}")).ToList();
        PakanRendah = pakan.Count(p => p.Stok < p.StokMinimum);

        // Births
        JumlahBunting = bunting.Count;
        Lahir = bunting.Take(4).Select(k =>
        {
            var d = (int)(k.PerkiraanLahir - hari).TotalDays;
            return new SegeraLahir(k.IndukId, d >= 0 ? $"H-{d}" : $"+{-d}", d <= 7, k.Induk.KodeTernak, $"{k.Induk.Ras.Spesies.Nama} {k.Induk.Ras.Nama} · {Format.TanggalPendek(k.PerkiraanLahir)}");
        }).ToList();

        // Health
        Diobati = aktif.Count(t => t.Kondisi == Kondisi.Diobati);
        Periksa = aktif.Count(t => t.Kondisi == Kondisi.Periksa);
        Vaksin7 = jadwal.Count(j => j.Status == StatusJadwal.Terjadwal && (j.TanggalRencana.Date - hari).TotalDays is >= 0 and <= 7);

        // Money
        if (AdaKeuangan)
        {
            var mulai = new DateTime(hari.Year, hari.Month, 1).AddMonths(-5);
            var rows = db.Keuangan.Where(x => x.Tanggal >= mulai).ToList();
            var bulan = Enumerable.Range(0, 6).Select(i => mulai.AddMonths(i)).ToList();
            var laba = bulan.Select(b => (double)rows.Where(x => x.Tanggal.Year == b.Year && x.Tanggal.Month == b.Month).Sum(x => x.Jenis == JenisKeuangan.Masuk ? x.Jumlah : -x.Jumlah)).ToList();
            var ini = rows.Where(x => x.Tanggal.Year == hari.Year && x.Tanggal.Month == hari.Month).ToList();
            var m = ini.Where(x => x.Jenis == JenisKeuangan.Masuk).Sum(x => x.Jumlah);
            var k = ini.Where(x => x.Jenis == JenisKeuangan.Keluar).Sum(x => x.Jumlah);
            Laba = Format.RupiahPendek(m - k);
            LabaPositif = m >= k;
            TrenLaba = laba;
            Masuk = Format.RupiahPendek(m);
            Keluar = Format.RupiahPendek(k);
        }

        // Latest weighings
        var latest = db.Penimbangan.Include(w => w.Ternak).ThenInclude(t => t.Ras).OrderByDescending(w => w.Tanggal).ThenByDescending(w => w.Id).Take(5).ToList();
        Timbang = latest.Select(w =>
        {
            var prev = db.Penimbangan.Where(x => x.TernakId == w.TernakId && (x.Tanggal < w.Tanggal || (x.Tanggal == w.Tanggal && x.Id < w.Id)))
                .OrderByDescending(x => x.Tanggal).ThenByDescending(x => x.Id).FirstOrDefault();
            double? g = prev is null ? null : (w.BeratKg - prev.BeratKg) / Math.Max(1, (w.Tanggal.Date - prev.Tanggal.Date).TotalDays);
            return new BarisTimbang(w.TernakId, w.Ternak.KodeTernak, w.Ternak.Ras.Nama, $"{Format.Angka(w.BeratKg, 1)} kg", Format.Selisih(g), g is null ? "" : g >= 0 ? "ok" : "danger", Format.TanggalPendek(w.Tanggal));
        }).ToList();

        BuatTugas(db, jadwal, aktif);
    }

    void HitungPopulasi(TernakKu.Data.TernakDbContext db)
    {
        var hari = DateTime.Today;
        var semua = db.Ternak.Include(t => t.Ras).ToList();
        var bulan = Enumerable.Range(0, 12).Select(i => new DateTime(hari.Year, hari.Month, 1).AddMonths(i - 11)).ToList();
        int? sp = FilterPopulasi == "all" ? null : int.Parse(FilterPopulasi);
        Populasi = bulan.Select(b =>
        {
            var akhir = b.AddMonths(1).AddDays(-1);
            if (akhir > hari) akhir = hari;
            return (double)semua.Count(t => (sp is null || t.Ras.SpesiesId == sp) && t.TanggalMasuk.Date <= akhir
                && (t.Status == StatusTernak.Aktif || (keluarPada.TryGetValue(t.Id, out var d) && d.Date > akhir)));
        }).ToList();
        LabelBulan = bulan.Select(b => b.ToString("MMM", Format.Id)).ToList();
        var nama = sp is null ? "Semua spesies" : db.Spesies.First(s => s.Id == sp).Nama;
        JudulPopulasi = $"{nama} · 12 bulan";
        var awal = Populasi[0];
        var pct = awal > 0 ? (int)Math.Round((Populasi[^1] - awal) / awal * 100) : 0;
        Pertumbuhan = $"{(pct >= 0 ? "+" : "")}{pct}%";
    }

    [RelayCommand]
    void Spesies(string id)
    {
        FilterPopulasi = id;
        using var db = Session.Buka();
        HitungPopulasi(db);
    }

    // Today's timeline: feeding and vaccines come from data, routine tasks are ticked by hand.
    void BuatTugas(TernakKu.Data.TernakDbContext db, List<JadwalVaksinasi> jadwal, List<Ternak> aktif)
    {
        var hari = DateTime.Today;
        var sekarang = Format.Jam(DateTime.Now);
        var manual = db.TugasSelesai.Where(t => t.Tanggal == hari).Select(t => t.Kunci).ToHashSet();
        var list = new List<TugasItem>();
        TugasItem Rutin(string kunci, string jam, string ikon, string judul, string sub, string? tombol = null, Action? aksi = null) => new()
        {
            Kunci = kunci, Jam = jam, Ikon = ikon, Judul = judul, Sub = sub, Selesai = manual.Contains(kunci), BisaDitandai = true,
            Terlambat = !manual.Contains(kunci) && string.CompareOrdinal(jam, sekarang) < 0, Tombol = tombol, AksiTombol = aksi, AksiTandai = () => Tandai(kunci)
        };
        if (Session.BolehBuka("pakan"))
        {
            var pagi = db.PemberianPakan.Include(p => p.Pakan).Where(p => p.Waktu >= hari && p.Waktu < hari.AddDays(1)).OrderBy(p => p.Waktu).AsEnumerable().Where(p => p.Waktu.Hour < 12).ToList();
            list.Add(pagi.Count > 0
                ? new TugasItem { Kunci = "pakan-pagi", Jam = Format.Jam(pagi[0].Waktu), Ikon = "box", Judul = "Pakan pagi", Sub = $"{pagi.Select(p => p.KandangId).Distinct().Count()} kandang · {Format.Angka(pagi.Sum(p => p.Jumlah))} kg", Selesai = true, Tombol = "Lihat", AksiTombol = () => Shell.Buka("pakan") }
                : Rutin("pakan-pagi", "06.30", "box", "Pakan pagi", "Semua kandang", "Catat", () => Shell.Buka("pakan")));
            list.Add(Rutin("pakan-sore", "16.00", "box", "Pakan sore", "Semua kandang", "Catat", () => Shell.Buka("pakan")));
        }
        if (Session.BolehBuka("kesehatan"))
        {
            var i = 0;
            foreach (var g in jadwal.Where(j => j.TanggalRencana.Date == hari && j.Status != StatusJadwal.Terlewat).GroupBy(j => (j.ObatVaksinId, j.Ternak.KandangId)))
            {
                var j = g.First();
                var ids = g.Select(x => x.Id).ToList();
                var done = g.All(x => x.Status == StatusJadwal.Selesai);
                var jam = $"{9 + i++:00}.00";
                list.Add(new TugasItem
                {
                    Kunci = $"vak-{ids[0]}", Jam = jam, Ikon = "syringe", Judul = $"Vaksin {j.ObatVaksin.Nama.Replace("Vaksin ", "")}",
                    Sub = $"Kandang {j.Ternak.Kandang?.Kode} · {ids.Count} ekor", Selesai = done, Terlambat = !done && string.CompareOrdinal(jam, sekarang) < 0,
                    BisaDitandai = !done && Session.BolehKesehatan, AksiTandai = () => VaksinSelesai(ids), Tombol = "Jadwal", AksiTombol = () => Shell.Buka("kesehatan")
                });
            }
            var periksa = aktif.Where(t => t.Kondisi == Kondisi.Periksa).ToList();
            if (periksa.Count > 0)
                list.Add(Rutin("periksa", "10.30", "heart", $"Periksa {periksa.Count} ternak", $"Kandang {string.Join(", ", periksa.Select(t => t.Kandang?.Kode).Distinct())}", "Buka", () => Shell.Buka("kesehatan")));
        }
        if (Session.BolehTimbang)
        {
            var k = db.Kandang.OrderBy(x => x.Kode).First();
            list.Add(Rutin("timbang", "14.00", "scale", $"Timbang kandang {k.Kode}", $"{aktif.Count(t => t.KandangId == k.Id)} ekor · rutin 14 hari", "Daftar", () => Shell.Buka("ternak", new TernakFilter(KandangId: k.Id))));
        }
        list.Add(Rutin("cek-sore", "17.30", "home", "Cek kandang & gudang", "Air minum, pintu, lampu"));
        list = list.OrderBy(t => t.Jam, StringComparer.Ordinal).ToList();

        var items = new List<object>();
        var placed = false;
        foreach (var t in list)
        {
            if (!placed && string.CompareOrdinal(t.Jam, sekarang) > 0) { items.Add(new PenandaSekarang(sekarang)); placed = true; }
            items.Add(t);
        }
        if (!placed) items.Add(new PenandaSekarang(sekarang));
        Tugas = items;
        var selesai = list.Count(t => t.Selesai);
        ProgresTugas = list.Count == 0 ? 0 : selesai * 100.0 / list.Count;
        RingkasHariIni = $"{(JumlahPengingat > 0 ? $"{JumlahPengingat} hal perlu ditindak hari ini" : "Tidak ada yang mendesak hari ini")} · {selesai} dari {list.Count} tugas selesai";
    }

    void Tandai(string kunci)
    {
        using var db = Session.Buka();
        var hari = DateTime.Today;
        var ada = db.TugasSelesai.FirstOrDefault(t => t.Tanggal == hari && t.Kunci == kunci);
        if (ada is null) db.TugasSelesai.Add(new TugasSelesai { Tanggal = hari, Kunci = kunci });
        else db.TugasSelesai.Remove(ada);
        db.SaveChanges();
        Load();
    }

    void VaksinSelesai(List<int> ids)
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
    }

    [RelayCommand] void TambahTernak() => Shell.Buka("ternak", new TernakFilter(BukaTambah: true));
    [RelayCommand] void Jalankan(Action? aksi) => aksi?.Invoke();
    [RelayCommand] void BukaTernak(int id) => Shell.Buka("detail", id);
    [RelayCommand] void BukaKandang(int id) => Shell.Buka("kandang", id);
    [RelayCommand] void Buka(string key) => Shell.Buka(key);
}
