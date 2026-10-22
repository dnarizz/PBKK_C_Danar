using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;
using TernakKu.App.Services;
using TernakKu.App.ViewModels.Pages;
using TernakKu.Core.Models;
using TernakKu.Core.Rules;

namespace TernakKu.App.ViewModels.Dialogs;

public partial class MutasiDialogViewModel : DialogViewModel
{
    public record OpsiTernak(int Id, string Teks, string Info);
    public record OpsiTujuan(int Id, string Teks, bool Bisa);

    public override string Judul => "Mutasi ternak";
    public override bool Lebar => true;
    public DateTime BatasTanggal { get; } = DateTime.Today;

    readonly List<Ternak> aktif;
    readonly List<Kandang> kandang;
    readonly Dictionary<int, int> isi;

    public List<OpsiTernak> DaftarTernak { get; }
    [ObservableProperty] private List<OpsiTujuan> daftarTujuan = [];

    [ObservableProperty, NotifyPropertyChangedFor(nameof(IsPindah), nameof(IsJual), nameof(IsMati), nameof(Catatan))] private JenisMutasi jenis = JenisMutasi.Pindah;
    [ObservableProperty, NotifyPropertyChangedFor(nameof(TernakSalah), nameof(Info))] private int ternakId;
    [ObservableProperty, NotifyPropertyChangedFor(nameof(TernakSalah))] private string ternakError = "";
    [ObservableProperty] private DateTime? tanggal = DateTime.Today;
    [ObservableProperty, NotifyPropertyChangedFor(nameof(TujuanSalah))] private int kandangTujuanId;
    [ObservableProperty, NotifyPropertyChangedFor(nameof(TujuanSalah))] private string tujuanError = "";
    [ObservableProperty, NotifyPropertyChangedFor(nameof(HargaSalah))] private string hargaJual = "";
    [ObservableProperty, NotifyPropertyChangedFor(nameof(HargaSalah))] private string hargaError = "";
    [ObservableProperty, NotifyPropertyChangedFor(nameof(PembeliSalah))] private string pembeli = "";
    [ObservableProperty, NotifyPropertyChangedFor(nameof(PembeliSalah))] private string pembeliError = "";
    [ObservableProperty, NotifyPropertyChangedFor(nameof(PenyebabSalah))] private string penyebab = "";
    [ObservableProperty, NotifyPropertyChangedFor(nameof(PenyebabSalah))] private string penyebabError = "";

    public bool IsPindah { get => Jenis == JenisMutasi.Pindah; set { if (value) Jenis = JenisMutasi.Pindah; } }
    public bool IsJual { get => Jenis == JenisMutasi.Jual; set { if (value) Jenis = JenisMutasi.Jual; } }
    public bool IsMati { get => Jenis == JenisMutasi.Mati; set { if (value) Jenis = JenisMutasi.Mati; } }
    public bool TernakSalah => TernakError.Length > 0;
    public bool TujuanSalah => TujuanError.Length > 0;
    public bool HargaSalah => HargaError.Length > 0;
    public bool PembeliSalah => PembeliError.Length > 0;
    public bool PenyebabSalah => PenyebabError.Length > 0;

    public string Info => aktif.FirstOrDefault(t => t.Id == TernakId) is { } t ? $"{t.Ras.Spesies.Nama} {t.Ras.Nama} · kandang {t.Kandang?.Label ?? "—"}" : "";
    public string Catatan => Jenis switch
    {
        JenisMutasi.Jual => "Status ternak berubah menjadi Terjual dan pemasukan otomatis tercatat di Keuangan.",
        JenisMutasi.Mati => "Status ternak berubah menjadi Mati dan tidak bisa dipilih lagi.",
        _ => ""
    };

    public MutasiDialogViewModel(ShellViewModel shell, int? ternakId) : base(shell)
    {
        using var db = Session.Buka();
        aktif = db.Ternak.Include(t => t.Ras).ThenInclude(r => r.Spesies).Include(t => t.Kandang)
            .Where(t => t.Status == StatusTernak.Aktif).OrderBy(t => t.KodeTernak).ToList();
        kandang = db.Kandang.OrderBy(k => k.Kode).ToList();
        isi = aktif.Where(t => t.KandangId != null).GroupBy(t => t.KandangId!.Value).ToDictionary(g => g.Key, g => g.Count());
        DaftarTernak = [new(0, "Pilih ternak aktif", ""), .. aktif.Select(t => new OpsiTernak(t.Id, $"{t.Label} — {t.Kandang?.Kode}", ""))];
        if (ternakId is { } id && aktif.Any(t => t.Id == id)) TernakId = id;
        else BuatTujuan();
    }

    partial void OnTernakIdChanged(int value) => BuatTujuan();

    void BuatTujuan()
    {
        var asal = aktif.FirstOrDefault(t => t.Id == TernakId)?.KandangId;
        DaftarTujuan = [new(0, "Pilih kandang", true), .. kandang.Select(k =>
        {
            var n = isi.GetValueOrDefault(k.Id);
            var penuh = AturanBisnis.KandangPenuh(n, k.Kapasitas);
            return new OpsiTujuan(k.Id, $"{k.Kode} · {k.Nama} ({n}/{k.Kapasitas}{(penuh ? ", penuh" : "")})", !penuh && k.Id != asal);
        })];
        KandangTujuanId = 0;
    }

    protected override bool Simpan()
    {
        TernakError = TujuanError = HargaError = PembeliError = PenyebabError = "";
        long harga = 0;
        if (TernakId == 0) TernakError = "Pilih ternak";
        if (Jenis == JenisMutasi.Pindah && KandangTujuanId == 0) TujuanError = "Pilih kandang tujuan";
        if (Jenis == JenisMutasi.Jual)
        {
            if (!long.TryParse(HargaJual.Trim().Replace(".", "").Replace(",", ""), out harga) || harga <= 0) HargaError = "Isi harga jual";
            if (string.IsNullOrWhiteSpace(Pembeli)) PembeliError = "Isi nama pembeli";
        }
        if (Jenis == JenisMutasi.Mati && string.IsNullOrWhiteSpace(Penyebab)) PenyebabError = "Isi penyebab";
        if (TernakError.Length + TujuanError.Length + HargaError.Length + PembeliError.Length + PenyebabError.Length > 0) return false;

        var tgl = (Tanggal ?? DateTime.Today).Date;
        using var db = Session.Buka();
        var t = db.Ternak.First(x => x.Id == TernakId);
        var asal = t.KandangId;
        var statusLama = t.Status;
        if (Jenis == JenisMutasi.Pindah)
        {
            var kap = db.Kandang.Where(k => k.Id == KandangTujuanId).Select(k => k.Kapasitas).First();
            var n = db.Ternak.Count(x => x.KandangId == KandangTujuanId && x.Status == StatusTernak.Aktif);
            if (AturanBisnis.KandangPenuh(n, kap)) { TujuanError = "Kandang sudah penuh"; return false; }
        }

        var m = new Mutasi { TernakId = t.Id, Tanggal = tgl, Jenis = Jenis, DicatatOleh = Session.Username };
        Keuangan? uang = null;
        switch (Jenis)
        {
            case JenisMutasi.Pindah:
                m.KandangAsalId = asal;
                m.KandangTujuanId = KandangTujuanId;
                t.KandangId = KandangTujuanId;
                break;
            case JenisMutasi.Jual:
                m.KandangAsalId = asal;
                m.HargaJual = harga;
                m.Pembeli = Pembeli.Trim();
                t.Status = StatusTernak.Terjual;
                t.KandangId = null;
                uang = new Keuangan { Tanggal = tgl, Jenis = JenisKeuangan.Masuk, Kategori = "Penjualan ternak", Jumlah = harga, Keterangan = $"{t.KodeTernak} · {m.Pembeli}", SumberTabel = "Mutasi" };
                break;
            default:
                m.KandangAsalId = asal;
                m.Penyebab = Penyebab.Trim();
                t.Status = StatusTernak.Mati;
                t.KandangId = null;
                break;
        }
        db.Mutasi.Add(m);
        if (uang is not null) db.Keuangan.Add(uang);
        db.SaveChanges();

        var mid = m.Id;
        var uid = uang?.Id;
        var tid = t.Id;
        var kode = t.KodeTernak;
        Shell.TampilSnack($"Mutasi {kode} disimpan", () =>
        {
            using var d = Session.Buka();
            var x = d.Ternak.Find(tid);
            if (x is not null) { x.Status = statusLama; x.KandangId = asal; }
            var mm = d.Mutasi.Find(mid);
            if (mm is not null) d.Mutasi.Remove(mm);
            if (uid is { } u && d.Keuangan.Find(u) is { } kk) d.Keuangan.Remove(kk);
            d.SaveChanges();
        });
        if (Shell.Halaman is DetailTernakViewModel) Shell.MuatUlang();
        else Shell.Buka("mutasi");
        return true;
    }
}
