using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;
using TernakKu.App.Services;
using TernakKu.App.ViewModels.Pages;
using TernakKu.Core.Models;
using TernakKu.Core.Rules;

namespace TernakKu.App.ViewModels.Dialogs;

public partial class TambahTernakViewModel : DialogViewModel
{
    public record OpsiKandang(int Id, string Teks, bool Bisa);
    public record OpsiInduk(int Id, string Teks);
    public record OpsiAsal(AsalUsul Nilai, string Teks);

    public override string Judul => "Tambah ternak";
    public override bool Lebar => true;

    public List<Spesies> DaftarSpesies { get; }
    public List<OpsiKandang> DaftarKandang { get; }
    public JenisKelamin[] DaftarKelamin { get; } = [JenisKelamin.Jantan, JenisKelamin.Betina];
    public List<OpsiAsal> DaftarAsal { get; } = [new(AsalUsul.Beli, "Dibeli"), new(AsalUsul.Lahir, "Lahir di peternakan")];
    public DateTime BatasLahir { get; } = DateTime.Today;

    readonly List<Ternak> betinaAktif;
    bool otomatis;
    bool kodeDiubah;

    [ObservableProperty] private int spesiesId;
    [ObservableProperty] private List<Ras> daftarRas = [];
    [ObservableProperty] private int rasId;
    [ObservableProperty, NotifyPropertyChangedFor(nameof(KodeSalah))] private string kode = "";
    [ObservableProperty, NotifyPropertyChangedFor(nameof(KodeSalah))] private string kodeError = "";
    [ObservableProperty] private string nama = "";
    [ObservableProperty] private JenisKelamin jenisKelamin = JenisKelamin.Jantan;
    [ObservableProperty, NotifyPropertyChangedFor(nameof(LahirSalah))] private DateTime? tanggalLahir = DateTime.Today.AddDays(-365);
    [ObservableProperty, NotifyPropertyChangedFor(nameof(LahirSalah))] private string lahirError = "";
    [ObservableProperty, NotifyPropertyChangedFor(nameof(KandangSalah))] private int kandangId;
    [ObservableProperty, NotifyPropertyChangedFor(nameof(KandangSalah))] private string kandangError = "";
    [ObservableProperty, NotifyPropertyChangedFor(nameof(BeratSalah))] private string berat = "";
    [ObservableProperty, NotifyPropertyChangedFor(nameof(BeratSalah))] private string beratError = "";
    [ObservableProperty, NotifyPropertyChangedFor(nameof(AsalBeli))] private AsalUsul asal = AsalUsul.Beli;
    [ObservableProperty] private string hargaBeli = "";
    [ObservableProperty] private List<OpsiInduk> daftarInduk = [];
    [ObservableProperty] private int indukId;

    public bool AsalBeli => Asal == AsalUsul.Beli;
    public bool KodeSalah => KodeError.Length > 0;
    public bool LahirSalah => LahirError.Length > 0;
    public bool KandangSalah => KandangError.Length > 0;
    public bool BeratSalah => BeratError.Length > 0;

    public TambahTernakViewModel(ShellViewModel shell) : base(shell)
    {
        using var db = Session.Buka();
        DaftarSpesies = db.Spesies.Include(s => s.Ras).OrderBy(s => s.Id).ToList();
        var isi = db.Ternak.Where(t => t.Status == StatusTernak.Aktif && t.KandangId != null).AsEnumerable().GroupBy(t => t.KandangId!.Value).ToDictionary(g => g.Key, g => g.Count());
        DaftarKandang = db.Kandang.OrderBy(k => k.Kode).AsEnumerable().Select(k =>
        {
            var n = isi.GetValueOrDefault(k.Id);
            var penuh = AturanBisnis.KandangPenuh(n, k.Kapasitas);
            return new OpsiKandang(k.Id, $"{k.Kode} · {k.Nama} ({n}/{k.Kapasitas}{(penuh ? ", penuh" : "")})", !penuh);
        }).ToList();
        betinaAktif = db.Ternak.Include(t => t.Ras).Where(t => t.Status == StatusTernak.Aktif && t.JenisKelamin == JenisKelamin.Betina).OrderBy(t => t.KodeTernak).ToList();

        var pertama = DaftarKandang.FirstOrDefault(k => k.Bisa);
        KandangId = pertama?.Id ?? 0;
        var kandang = pertama is null ? null : db.Kandang.Find(pertama.Id);
        SpesiesId = kandang?.SpesiesId ?? DaftarSpesies[0].Id;
    }

    partial void OnSpesiesIdChanged(int value)
    {
        var sp = DaftarSpesies.FirstOrDefault(s => s.Id == value);
        DaftarRas = sp?.Ras.OrderBy(r => r.Id).ToList() ?? [];
        RasId = DaftarRas.FirstOrDefault()?.Id ?? 0;
        DaftarInduk = [new(0, "— tidak diketahui —"), .. betinaAktif.Where(t => t.Ras.SpesiesId == value).Select(t => new OpsiInduk(t.Id, t.Label))];
        IndukId = 0;
        if (!kodeDiubah) { otomatis = true; Kode = KodeBerikut(value); otomatis = false; }
    }

    partial void OnKodeChanged(string value)
    {
        if (!otomatis) kodeDiubah = true;
    }

    static string KodeBerikut(int spesiesId)
    {
        using var db = Session.Buka();
        var sp = db.Spesies.First(s => s.Id == spesiesId);
        var angka = db.Ternak.Where(t => t.Ras.SpesiesId == spesiesId).Select(t => t.KodeTernak).AsEnumerable()
            .Select(k => k.Split('-').Last()).ToList();
        var maks = angka.Select(a => int.TryParse(a, out var n) ? n : 0).DefaultIfEmpty(0).Max();
        var lebar = Math.Max(3, angka.Select(a => a.Length).DefaultIfEmpty(3).Max());
        return $"{sp.KodePrefix}-{(maks + 1).ToString($"D{lebar}")}";
    }

    static bool Angka(string s, out double v) =>
        double.TryParse(s.Trim().Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out v);

    protected override bool Simpan()
    {
        KodeError = LahirError = KandangError = BeratError = "";
        var kodeBaru = Kode.Trim().ToUpperInvariant();
        using var db = Session.Buka();
        if (kodeBaru.Length == 0) KodeError = "Kode wajib diisi";
        else if (db.Ternak.Any(t => t.KodeTernak == kodeBaru)) KodeError = $"Kode {kodeBaru} sudah dipakai";
        if (!Angka(Berat, out var kg) || kg <= 0) BeratError = "Isi berat lebih dari 0";
        if (KandangId == 0) KandangError = "Pilih kandang";
        else
        {
            var kap = db.Kandang.Where(k => k.Id == KandangId).Select(k => k.Kapasitas).FirstOrDefault();
            var isi = db.Ternak.Count(t => t.KandangId == KandangId && t.Status == StatusTernak.Aktif);
            if (AturanBisnis.KandangPenuh(isi, kap)) KandangError = "Kandang sudah penuh";
        }
        if (TanggalLahir is null || TanggalLahir.Value.Date > DateTime.Today) LahirError = "Tanggal lahir tidak valid";
        if (KodeError.Length + BeratError.Length + KandangError.Length + LahirError.Length > 0) return false;

        long? harga = null;
        if (AsalBeli && long.TryParse(HargaBeli.Trim().Replace(".", ""), out var h) && h > 0) harga = h;
        var user = Session.Username;
        var t = new Ternak
        {
            KodeTernak = kodeBaru, Nama = Nama.Trim(), RasId = RasId, KandangId = KandangId, JenisKelamin = JenisKelamin,
            TanggalLahir = TanggalLahir!.Value.Date, AsalUsul = Asal, TanggalMasuk = DateTime.Today, HargaBeli = harga,
            IndukId = !AsalBeli && IndukId != 0 ? IndukId : null, Status = StatusTernak.Aktif, Kondisi = Kondisi.Sehat, DicatatOleh = user
        };
        t.Penimbangan.Add(new Penimbangan { Tanggal = DateTime.Today, BeratKg = kg, DicatatOleh = user });
        db.Ternak.Add(t);
        db.SaveChanges();
        var id = t.Id;
        var sp = SpesiesId;

        Shell.TampilSnack($"{kodeBaru} ditambahkan", () =>
        {
            using var d = Session.Buka();
            d.Penimbangan.RemoveRange(d.Penimbangan.Where(p => p.TernakId == id));
            var x = d.Ternak.Find(id);
            if (x is not null) d.Ternak.Remove(x);
            d.SaveChanges();
        });
        Shell.Buka("ternak", new TernakFilter(SpesiesId: sp));
        return true;
    }
}
