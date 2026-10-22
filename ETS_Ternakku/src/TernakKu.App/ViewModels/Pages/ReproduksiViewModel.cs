using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using TernakKu.App.Services;
using TernakKu.App.ViewModels.Dialogs;
using TernakKu.Core.Models;
using TernakKu.Core.Rules;

namespace TernakKu.App.ViewModels.Pages;

public record KartuStatRepro(string Judul, string Nilai, string Satuan, string Sub, string SubTebal, string Tone, string Ikon);

public record BarisKawin(
    int Id, int IndukId, string Induk, string SpesiesRas, string Tanggal, string Metode, string MetodeDetail,
    string StatusTeks, string StatusTone, string Perkiraan,
    bool AdaMeter, double Persen, string MeterTone, string MeterTeks, string CekTeks,
    bool BisaStatus, bool BisaLahir, bool LahirUtama, string LahirTeks)
{
    public bool LahirPrimer => BisaLahir && LahirUtama;
    public bool LahirBiasa => BisaLahir && !LahirUtama;
}

public partial class ReproduksiViewModel(ShellViewModel shell) : PageViewModel(shell), IPunyaTambah
{
    public override string Key => "reproduksi";
    public bool BolehKesehatan => Session.BolehKesehatan;

    [ObservableProperty] private List<KartuStatRepro> statistik = [];
    [ObservableProperty] private List<BarisKawin> baris = [];
    [ObservableProperty, NotifyPropertyChangedFor(nameof(FilterAktif), nameof(FilterBunting), nameof(FilterMenunggu), nameof(FilterRiwayat))] private string filter = "aktif";

    public bool FilterAktif => Filter == "aktif";
    public bool FilterBunting => Filter == "Bunting";
    public bool FilterMenunggu => Filter == "Menunggu";
    public bool FilterRiwayat => Filter == "riwayat";

    public IRelayCommand TambahCommand => tambah ??= new RelayCommand(() => Shell.TampilDialog(new KawinViewModel(Shell)), () => Session.BolehKesehatan);
    IRelayCommand? tambah;

    public override void Load()
    {
        var hari = DateTime.Today;
        using var db = Session.Buka();
        var semua = db.Perkawinan.Include(k => k.Induk).ThenInclude(t => t.Ras).ThenInclude(r => r.Spesies)
            .Include(k => k.Pejantan).Include(k => k.Kelahiran).AsEnumerable().ToList();

        var bunting = semua.Where(k => k.Status == StatusBunting.Bunting).ToList();
        var lahir7 = bunting.Count(k => (k.PerkiraanLahir.Date - hari).TotalDays <= 7);
        Statistik =
        [
            new("Induk bunting", bunting.Count.ToString(), "ekor", " diperkirakan lahir ≤ 7 hari", lahir7.ToString(), "warn", "repro"),
            new("Menunggu cek bunting", semua.Count(k => k.Status == StatusBunting.Menunggu).ToString(), "ekor", "Dicek ± 60 hari setelah kawin", "", "info", "calendar"),
            new("Lahir tercatat", semua.Count(k => k.Status == StatusBunting.Lahir).ToString(), "kali", "Anak langsung masuk data ternak", "", "ok", "tag"),
            new("Gagal bunting", semua.Count(k => k.Status == StatusBunting.Gagal).ToString(), "kali", "Bisa dikawinkan ulang", "", "danger", "alert")
        ];

        var daftar = semua.Where(k => Filter switch
        {
            "aktif" => k.Status is StatusBunting.Bunting or StatusBunting.Menunggu,
            "riwayat" => k.Status is StatusBunting.Gagal or StatusBunting.Lahir,
            "Bunting" => k.Status == StatusBunting.Bunting,
            _ => k.Status == StatusBunting.Menunggu
        }).OrderBy(k => k.PerkiraanLahir).ToList();

        Baris = daftar.Select(k =>
        {
            var sp = k.Induk.Ras.Spesies;
            var lewat = (int)(hari - k.Tanggal.Date).TotalDays;
            var sisa = (int)(k.PerkiraanLahir.Date - hari).TotalDays;
            var (teks, tone) = k.Status switch
            {
                StatusBunting.Bunting => ("Bunting", "warn"),
                StatusBunting.Menunggu => ("Menunggu cek", "info"),
                StatusBunting.Gagal => ("Gagal", "neutral"),
                _ => ("Sudah lahir", "ok")
            };
            var bisa = Session.BolehKesehatan;
            var meter = k.Status == StatusBunting.Bunting;
            return new BarisKawin(
                k.Id, k.IndukId, k.Induk.Label, $"{sp.Nama} {k.Induk.Ras.Nama}", Format.Tanggal(k.Tanggal),
                k.Metode == MetodeKawin.IB ? "IB" : "Alami", k.Metode == MetodeKawin.IB ? k.KodeStraw : k.Pejantan?.KodeTernak ?? "—",
                teks, tone, Format.Tanggal(k.PerkiraanLahir),
                meter, sp.MasaBuntingHari > 0 ? Math.Clamp(lewat * 100.0 / sp.MasaBuntingHari, 0, 100) : 0, sisa <= 7 ? "warn" : "ok",
                meter ? $"{(sisa >= 0 ? $"H-{sisa}" : $"lewat {-sisa} hari")} · {lewat} dari {sp.MasaBuntingHari} hari" : "",
                k.Status == StatusBunting.Menunggu ? $"Cek bunting {AturanBisnis.HariRelatif(k.Tanggal.Date.AddDays(AturanBisnis.HariCekBunting), hari)}" : "",
                bisa && k.Status == StatusBunting.Menunggu, bisa && meter, sisa <= 7,
                k.Status == StatusBunting.Lahir && k.Kelahiran is { } l ? $"{l.JumlahAnak} anak · {Format.TanggalPendek(l.Tanggal)}" : "");
        }).ToList();
    }

    [RelayCommand]
    void Atur(string nilai)
    {
        Filter = nilai;
        Load();
    }

    [RelayCommand] void BukaTernak(int id) => Shell.Buka("detail", id);

    [RelayCommand] void Lahir(int id) => Shell.TampilDialog(new KelahiranViewModel(Shell, id));

    [RelayCommand] void TandaiBunting(int id) => Ubah(id, StatusBunting.Bunting);

    [RelayCommand] void TandaiGagal(int id) => Ubah(id, StatusBunting.Gagal);

    void Ubah(int id, StatusBunting baru)
    {
        StatusBunting lama;
        string kode;
        using (var db = Session.Buka())
        {
            var k = db.Perkawinan.Include(x => x.Induk).FirstOrDefault(x => x.Id == id);
            if (k is null) return;
            lama = k.Status;
            kode = k.Induk.KodeTernak;
            k.Status = baru;
            db.SaveChanges();
        }
        Shell.TampilSnack(baru == StatusBunting.Bunting ? $"{kode} dinyatakan bunting" : $"Kawin {kode} ditandai gagal", () =>
        {
            using var db = Session.Buka();
            var k = db.Perkawinan.Find(id);
            if (k is not null) { k.Status = lama; db.SaveChanges(); }
        });
        Load();
        Shell.SegarkanStatus();
    }
}
