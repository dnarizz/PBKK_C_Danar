using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TernakKu.App.Services;
using TernakKu.App.ViewModels.Dialogs;
using TernakKu.Core.Models;

namespace TernakKu.App.ViewModels.Pages;

public record BarisSpesies(int Id, string Nama, string Kode, string Bunting);
public record BarisRas(int Id, string Nama, string Spesies, int Aktif);
public record BarisKandang(int Id, string Kode, string Nama, string Jenis, string Spesies, int Kapasitas, int Terisi);
public record BarisPakan(int Id, string Nama, string Satuan, string StokMinimum, string Harga, string Pemakaian);
public record BarisObat(int Id, string Nama, string Jenis, string JenisTone, string Satuan, string Stok, string StokMinimum, bool Rendah);

public partial class MasterViewModel(ShellViewModel shell) : PageViewModel(shell)
{
    public override string Key => "master";

    public bool BolehMaster => Session.BolehMaster;

    [ObservableProperty, NotifyPropertyChangedFor(nameof(TabSpesies), nameof(TabKandang), nameof(TabPakan), nameof(TabObat))] private string tab = "spesies";
    public bool TabSpesies => Tab == "spesies";
    public bool TabKandang => Tab == "kandang";
    public bool TabPakan => Tab == "pakan";
    public bool TabObat => Tab == "obat";

    [ObservableProperty] private List<BarisSpesies> spesies = [];
    [ObservableProperty] private List<BarisRas> ras = [];
    [ObservableProperty] private List<BarisKandang> kandang = [];
    [ObservableProperty] private List<BarisPakan> pakan = [];
    [ObservableProperty] private List<BarisObat> obat = [];

    public override void Load()
    {
        using var db = Session.Buka();
        var aktif = db.Ternak.Where(t => t.Status == StatusTernak.Aktif).Select(t => new { t.RasId, t.KandangId }).ToList();
        var daftarSpesies = db.Spesies.OrderBy(s => s.Id).ToList();
        var nama = daftarSpesies.ToDictionary(s => s.Id, s => s.Nama);

        Spesies = daftarSpesies.Select(s => new BarisSpesies(s.Id, s.Nama, s.KodePrefix, $"{s.MasaBuntingHari} hari")).ToList();
        Ras = db.Ras.OrderBy(r => r.Id).AsEnumerable().Select(r => new BarisRas(r.Id, r.Nama, nama.GetValueOrDefault(r.SpesiesId, "—"), aktif.Count(t => t.RasId == r.Id))).ToList();
        Kandang = db.Kandang.OrderBy(k => k.Kode).AsEnumerable().Select(k => new BarisKandang(k.Id, k.Kode, k.Nama, k.Jenis.ToString(),
            k.SpesiesId is { } sid ? nama.GetValueOrDefault(sid, "—") : "Campuran", k.Kapasitas, aktif.Count(t => t.KandangId == k.Id))).ToList();
        Pakan = db.Pakan.OrderBy(p => p.Id).AsEnumerable().Select(p => new BarisPakan(p.Id, p.Nama, p.Satuan, Format.Angka(p.StokMinimum),
            Format.Rupiah(p.HargaPerSatuan), Format.Angka(p.PemakaianHarian, p.PemakaianHarian < 1 ? 1 : 0))).ToList();
        Obat = db.ObatVaksin.OrderBy(o => o.Id).AsEnumerable().Select(o => new BarisObat(o.Id, o.Nama, o.Jenis.ToString(),
            o.Jenis == JenisObat.Vaksin ? "info" : o.Jenis == JenisObat.Obat ? "warn" : "ok", o.Satuan, Format.Angka(o.Stok), Format.Angka(o.StokMinimum), o.Stok < o.StokMinimum)).ToList();
    }

    void Edit(string entitas, int? id) => Shell.TampilDialog(new MasterEditViewModel(Shell, entitas, id));

    [RelayCommand] void TambahSpesies() => Edit("spesies", null);
    [RelayCommand] void TambahRas() => Edit("ras", null);
    [RelayCommand] void TambahKandang() => Edit("kandang", null);
    [RelayCommand] void TambahPakan() => Edit("pakan", null);
    [RelayCommand] void TambahObat() => Edit("obat", null);
    [RelayCommand] void UbahSpesies(int id) => Edit("spesies", id);
    [RelayCommand] void UbahRas(int id) => Edit("ras", id);
    [RelayCommand] void UbahKandang(int id) => Edit("kandang", id);
    [RelayCommand] void UbahPakan(int id) => Edit("pakan", id);
    [RelayCommand] void UbahObat(int id) => Edit("obat", id);
}
