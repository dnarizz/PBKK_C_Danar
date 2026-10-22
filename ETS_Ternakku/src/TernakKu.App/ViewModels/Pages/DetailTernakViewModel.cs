using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using TernakKu.App.Services;
using TernakKu.App.ViewModels.Dialogs;
using TernakKu.Core.Models;
using TernakKu.Core.Rules;

namespace TernakKu.App.ViewModels.Pages;

public partial class DetailTernakViewModel(ShellViewModel shell, int ternakId) : PageViewModel(shell)
{
    public record KartuStat(string Label, string Nilai, string Satuan, string Sub, string Tone, string Ikon);
    public record BarisBerat(string Tanggal, string Berat, string Selisih, string Tone, string Dada);
    public record Pasangan(string Kunci, string Nilai);
    public record ItemRiwayat(string Tgl, string Judul, string Sub, bool Waspada);
    public record BarisKawin(string Tanggal, string Metode, string Status, string Tone, string Perkiraan);
    public record Anak(int Id, string Label, string Sub);
    public record Simpul(int Id, string Peran, string Judul, string Sub, bool Tidak, bool Saya)
    {
        public bool BisaDibuka => !Tidak && !Saya;
        public bool AdaSub => Sub.Length > 0;
    }

    public override string Key => "detail";

    [ObservableProperty, NotifyPropertyChangedFor(nameof(TidakAda))] private bool ada;
    public bool TidakAda => !Ada;

    // Profile card
    [ObservableProperty] private string kode = "";
    [ObservableProperty] private string namaTampil = "";
    [ObservableProperty] private string kondisiTeks = "";
    [ObservableProperty] private string kondisiTone = "neutral";
    [ObservableProperty] private List<string> fakta = [];
    [ObservableProperty] private bool bisaMutasi;
    [ObservableProperty] private bool bisaTimbang;
    [ObservableProperty] private List<KartuStat> kartu = [];

    // Tabs
    [ObservableProperty, NotifyPropertyChangedFor(nameof(TabPertumbuhan), nameof(TabProfil), nameof(TabKesehatan), nameof(TabReproduksi), nameof(TabSilsilah))] private string tab = "pertumbuhan";
    public bool TabPertumbuhan { get => Tab == "pertumbuhan"; set { if (value) Tab = "pertumbuhan"; } }
    public bool TabProfil { get => Tab == "profil"; set { if (value) Tab = "profil"; } }
    public bool TabKesehatan { get => Tab == "kesehatan"; set { if (value) Tab = "kesehatan"; } }
    public bool TabReproduksi { get => Tab == "reproduksi"; set { if (value) Tab = "reproduksi"; } }
    public bool TabSilsilah { get => Tab == "silsilah"; set { if (value) Tab = "silsilah"; } }

    // Growth
    [ObservableProperty] private List<double> nilaiBerat = [];
    [ObservableProperty] private List<string> labelBerat = [];
    [ObservableProperty] private double? target;
    [ObservableProperty] private List<BarisBerat> riwayatBerat = [];

    // Profile
    [ObservableProperty] private List<Pasangan> profil = [];

    // Health
    [ObservableProperty] private List<ItemRiwayat> riwayatSehat = [];
    [ObservableProperty] private bool bisaPeriksa;

    // Reproduction
    [ObservableProperty] private bool betina;
    [ObservableProperty] private string judulKawin = "";
    [ObservableProperty] private List<BarisKawin> kawin = [];
    [ObservableProperty] private string ringkasPejantan = "";
    [ObservableProperty] private List<Anak> keturunan = [];

    // Family tree
    [ObservableProperty] private bool silsilahKosong;
    [ObservableProperty] private string pesanSilsilah = "";
    [ObservableProperty] private List<List<Simpul>> tingkat = [];

    public override void Load()
    {
        var hari = DateTime.Today;
        using var db = Session.Buka();
        var t = db.Ternak.Include(x => x.Ras).ThenInclude(r => r.Spesies).Include(x => x.Kandang).FirstOrDefault(x => x.Id == ternakId);
        if (t is null) { Ada = false; return; }
        Ada = true;
        var sp = t.Ras.Spesies;
        var w = db.Penimbangan.Where(p => p.TernakId == t.Id).OrderBy(p => p.Tanggal).ThenBy(p => p.Id).ToList();
        var last = w.LastOrDefault();
        var first = w.FirstOrDefault();
        var g = AturanBisnis.Adg(w);
        var k = t.Kandang;
        var aktif = t.Status == StatusTernak.Aktif;
        var kawinList = db.Perkawinan.Where(x => x.IndukId == t.Id).OrderByDescending(x => x.Tanggal).ToList();
        var bunting = kawinList.FirstOrDefault(x => x.Status == StatusBunting.Bunting);
        var anak = db.Ternak.Include(x => x.Ras).Where(x => x.IndukId == t.Id || x.PejantanId == t.Id).OrderBy(x => x.KodeTernak).ToList();

        Kode = t.KodeTernak;
        NamaTampil = string.IsNullOrEmpty(t.Nama) ? t.KodeTernak : t.Nama;
        (KondisiTeks, KondisiTone) = !aktif ? (t.Status.ToString(), "neutral")
            : t.Kondisi switch { Kondisi.Sehat => ("Sehat", "ok"), Kondisi.Diobati => ("Diobati", "danger"), _ => ("Perlu diperiksa", "warn") };
        Fakta =
        [
            $"{sp.Nama} {t.Ras.Nama} · {t.JenisKelamin}",
            $"Lahir {Format.Tanggal(t.TanggalLahir)} · {AturanBisnis.Umur(t.TanggalLahir, hari)}",
            k is not null ? $"Kandang {k.Label}" : t.Status.ToString(),
            t.AsalUsul == AsalUsul.Beli ? $"Dibeli {Format.Tanggal(t.TanggalMasuk)}" : "Lahir di peternakan"
        ];
        BisaMutasi = aktif && Session.BolehMutasi;
        BisaTimbang = aktif && Session.BolehTimbang;

        // Stat cards
        var penggemukan = t.JenisKelamin == JenisKelamin.Jantan && k?.Jenis == JenisKandang.Penggemukan;
        double? tujuan = penggemukan ? (sp.Nama == "Sapi" ? 450 : 40) : null;
        KartuStat keempat;
        if (tujuan is { } tg)
        {
            int? eta = g is > 0 && last is not null ? (int)Math.Max(0, Math.Ceiling((tg - last.BeratKg) / g.Value)) : null;
            var siap = last is not null && last.BeratKg >= tg;
            keempat = new($"Target jual {tg} kg", eta is null ? "—" : siap ? "Siap" : $"± {eta}", eta is not null && !siap ? "hari" : "",
                g is not null ? $"jika ADG tetap {Format.Angka(g.Value, 2)} kg/hari" : "Butuh dua penimbangan", "info", "trend");
        }
        else if (t.JenisKelamin == JenisKelamin.Betina)
        {
            keempat = bunting is not null
                ? new("Status reproduksi", "Bunting", "", $"Perkiraan lahir {Format.Tanggal(bunting.PerkiraanLahir)} · {AturanBisnis.HariRelatif(bunting.PerkiraanLahir, hari)}", "warn", "repro")
                : new("Status reproduksi", "Kosong", "", kawinList.Count > 0 ? $"Terakhir kawin {Format.Tanggal(kawinList[0].Tanggal)}" : "Belum ada catatan perkawinan", "warn", "repro");
        }
        else
        {
            keempat = new("Anak tercatat", anak.Count(x => x.PejantanId == t.Id).ToString(), "ekor", "Sebagai pejantan", "info", "repro");
        }
        var naik = last is not null && first is not null ? last.BeratKg - first.BeratKg : (double?)null;
        Kartu =
        [
            new("Berat terakhir", last is null ? "—" : Format.Angka(last.BeratKg, 1), "kg", last is null ? "Belum ditimbang" : $"Ditimbang {Format.Tanggal(last.Tanggal)}", "ok", "scale"),
            new("ADG 30 hari", g is null ? "—" : Format.Selisih(g, 2), "kg/hari", g is null ? "Butuh dua penimbangan" : g >= 0 ? "Pertambahan bobot harian" : "Berat turun, periksa kondisi", g < 0 ? "danger" : "ok", "trend"),
            new("Naik sejak masuk", naik is null ? "—" : Format.Selisih(naik, 1), "kg", naik is null ? "" : $"dari {Format.Angka(first!.BeratKg, 1)} kg dalam {(int)(last!.Tanggal.Date - first.Tanggal.Date).TotalDays} hari", "ok", "tag"),
            keempat
        ];

        // Growth
        NilaiBerat = w.Select(x => x.BeratKg).ToList();
        LabelBerat = w.Select(x => Format.TanggalPendek(x.Tanggal)).ToList();
        Target = tujuan;
        RiwayatBerat = Enumerable.Range(0, w.Count).Reverse().Select(i =>
        {
            double? d = i > 0 ? w[i].BeratKg - w[i - 1].BeratKg : null;
            return new BarisBerat(Format.Tanggal(w[i].Tanggal), $"{Format.Angka(w[i].BeratKg, 1)} kg", d is null ? "—" : Format.Selisih(d, 1), d is null ? "" : d >= 0 ? "ok" : "danger",
                w[i].LingkarDadaCm is { } c ? $"{Format.Angka(c)} cm" : "—");
        }).ToList();

        // Profile
        var induk = t.IndukId is { } iid ? db.Ternak.Include(x => x.Ras).FirstOrDefault(x => x.Id == iid) : null;
        var pej = t.PejantanId is { } pid ? db.Ternak.Include(x => x.Ras).FirstOrDefault(x => x.Id == pid) : null;
        Profil =
        [
            new("Kode ear tag", t.KodeTernak),
            new("Nama", string.IsNullOrEmpty(t.Nama) ? "—" : t.Nama),
            new("Spesies · ras", $"{sp.Nama} · {t.Ras.Nama}"),
            new("Jenis kelamin", t.JenisKelamin.ToString()),
            new("Tanggal lahir", $"{Format.Tanggal(t.TanggalLahir)} · {AturanBisnis.Umur(t.TanggalLahir, hari)}"),
            new("Asal usul", t.AsalUsul == AsalUsul.Beli ? $"Dibeli · {Format.Tanggal(t.TanggalMasuk)}" : "Lahir di peternakan"),
            new("Harga beli", t.HargaBeli is { } hb ? Format.Rupiah(hb) : "—"),
            new("Kandang", k?.Label ?? "—"),
            new("Induk", induk?.Label ?? "—"),
            new("Pejantan", pej?.Label ?? "—"),
            new("Status", t.Status.ToString()),
            new("Dicatat oleh", t.DicatatOleh)
        ];

        // Health timeline
        var rec = db.Kesehatan.Where(x => x.TernakId == t.Id).ToList();
        var jd = db.JadwalVaksinasi.Include(x => x.ObatVaksin).Where(x => x.TernakId == t.Id).ToList();
        var tl = rec.Select(r => (D: r.Tanggal, Item: new ItemRiwayat($"{Format.Tanggal(r.Tanggal)} · {(string.IsNullOrEmpty(r.DicatatOleh) ? "Mantri" : r.DicatatOleh)}", r.Diagnosa, $"{r.Keluhan} · {r.Tindakan}", true)))
            .Concat(jd.Select(j =>
            {
                var st = AturanBisnis.StatusJadwalHariIni(j, hari);
                var d = j.TanggalRealisasi ?? j.TanggalRencana;
                return (D: d, Item: new ItemRiwayat($"{Format.Tanggal(d)} · Mantri", $"{j.ObatVaksin.Nama} · {st.ToString().ToLowerInvariant()}",
                    st == StatusJadwal.Terjadwal ? $"Dijadwalkan {AturanBisnis.HariRelatif(j.TanggalRencana, hari)}" : st == StatusJadwal.Terlewat ? "Belum dilakukan" : "Tanpa keluhan setelah vaksin.", st != StatusJadwal.Selesai));
            }))
            .OrderByDescending(x => x.D).Select(x => x.Item).ToList();
        RiwayatSehat = tl;
        BisaPeriksa = Session.BolehKesehatan && Session.BolehBuka("kesehatan") && aktif;

        // Reproduction
        Betina = t.JenisKelamin == JenisKelamin.Betina;
        JudulKawin = Betina ? "Riwayat perkawinan" : "Sebagai pejantan";
        var pejantanIds = db.Perkawinan.Where(x => x.PejantanId == t.Id).Count();
        RingkasPejantan = $"{pejantanIds} perkawinan tercatat dengan {t.KodeTernak} sebagai pejantan.";
        var kodePejantan = db.Ternak.ToDictionary(x => x.Id, x => x.KodeTernak);
        Kawin = kawinList.Select(x =>
        {
            var (teks, tone) = x.Status switch
            {
                StatusBunting.Bunting => ("Bunting", "warn"),
                StatusBunting.Menunggu => ("Menunggu cek", "info"),
                StatusBunting.Gagal => ("Gagal", "neutral"),
                _ => ("Sudah lahir", "ok")
            };
            return new BarisKawin(Format.Tanggal(x.Tanggal), x.Metode == MetodeKawin.IB ? $"IB · {x.KodeStraw}" : $"Alami · {(x.PejantanId is { } pj && kodePejantan.TryGetValue(pj, out var kd) ? kd : "—")}",
                teks, tone, Format.Tanggal(x.PerkiraanLahir));
        }).ToList();
        Keturunan = anak.Select(a => new Anak(a.Id, a.Label, $"{a.JenisKelamin} · {AturanBisnis.Umur(a.TanggalLahir, hari)}")).ToList();

        // Family tree
        Ternak? nenek = induk?.IndukId is { } nid ? db.Ternak.Include(x => x.Ras).FirstOrDefault(x => x.Id == nid) : null;
        Ternak? kakek = pej?.PejantanId is { } kid ? db.Ternak.Include(x => x.Ras).FirstOrDefault(x => x.Id == kid) : null;
        Simpul Node(Ternak? x, string peran) => x is null
            ? new(0, peran, "Tidak diketahui", "", true, false)
            : new(x.Id, peran, x.Label, $"{x.Ras.Nama} · {AturanBisnis.Umur(x.TanggalLahir, hari)}", false, false);
        SilsilahKosong = induk is null && pej is null && t.AsalUsul == AsalUsul.Beli;
        PesanSilsilah = $"{NamaTampil} dibeli dari luar, jadi induk dan pejantannya tidak ada di data.";
        var tree = new List<List<Simpul>>
        {
            new() { Node(nenek, "Nenek (dari induk)"), Node(kakek, "Kakek (dari pejantan)") },
            new() { Node(induk, "Induk"), Node(pej, "Pejantan") },
            new() { new(t.Id, "Ternak ini", t.Label, $"{t.Ras.Nama} · {AturanBisnis.Umur(t.TanggalLahir, hari)}", false, true) }
        };
        if (anak.Count > 0) tree.Add(anak.Take(6).Select(a => Node(a, "Anak")).ToList());
        Tingkat = tree;
    }

    [RelayCommand] void KeDaftar() => Shell.Buka("ternak");
    [RelayCommand] void Buka(int id) { if (id != 0 && id != ternakId) Shell.Buka("detail", id); }
    [RelayCommand] void Mutasi() { if (BisaMutasi) Shell.TampilDialog(new MutasiDialogViewModel(Shell, ternakId)); }
    [RelayCommand] void Timbang() { if (BisaTimbang) Shell.TampilDialog(new TimbangViewModel(Shell, ternakId)); }
    [RelayCommand] void Periksa() => Shell.Buka("kesehatan");
}
