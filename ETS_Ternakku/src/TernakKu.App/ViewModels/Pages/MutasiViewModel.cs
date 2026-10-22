using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using TernakKu.App.Services;
using TernakKu.App.ViewModels.Dialogs;
using TernakKu.Core.Models;

namespace TernakKu.App.ViewModels.Pages;

public partial class MutasiViewModel(ShellViewModel shell) : PageViewModel(shell), IPunyaTambah
{
    public record KartuStat(string Label, string Nilai, string Satuan, string Sub, string Tone, string Ikon);
    public record Baris(int TernakId, string Tanggal, string Ternak, string Spesies, string Jenis, string Tone, string Detail, string Oleh);

    public override string Key => "mutasi";

    [ObservableProperty, NotifyPropertyChangedFor(nameof(FSemua), nameof(FPindah), nameof(FJual), nameof(FMati))] private string filter = "Semua";
    [ObservableProperty] private List<KartuStat> kartu = [];
    [ObservableProperty] private List<Baris> daftar = [];

    public bool BolehTambah => Session.BolehMutasi;
    public bool FSemua { get => Filter == "Semua"; set { if (value) Filter = "Semua"; } }
    public bool FPindah { get => Filter == "Pindah"; set { if (value) Filter = "Pindah"; } }
    public bool FJual { get => Filter == "Jual"; set { if (value) Filter = "Jual"; } }
    public bool FMati { get => Filter == "Mati"; set { if (value) Filter = "Mati"; } }

    List<(Mutasi M, Baris B)> semua = [];

    partial void OnFilterChanged(string value) => Terapkan();

    public override void Load()
    {
        var hari = DateTime.Today;
        using var db = Session.Buka();
        var kode = db.Kandang.ToDictionary(k => k.Id, k => k.Kode);
        string Kode(int? id) => id is { } i && kode.TryGetValue(i, out var k) ? k : "—";
        var list = db.Mutasi.Include(m => m.Ternak).ThenInclude(t => t.Ras).ThenInclude(r => r.Spesies).ToList()
            .OrderByDescending(m => m.Tanggal).ThenByDescending(m => m.Id).ToList();
        semua = list.Select(m => (m, new Baris(m.TernakId, Format.Tanggal(m.Tanggal), m.Ternak.Label, m.Ternak.Ras.Spesies.Nama, m.Jenis.ToString(),
            m.Jenis switch { JenisMutasi.Pindah => "info", JenisMutasi.Jual => "ok", _ => "neutral" },
            m.Jenis switch
            {
                JenisMutasi.Pindah => $"{Kode(m.KandangAsalId)} → {Kode(m.KandangTujuanId)}",
                JenisMutasi.Jual => $"{Format.Rupiah(m.HargaJual ?? 0)} · {m.Pembeli}",
                _ => m.Penyebab
            }, m.DicatatOleh))).ToList();

        var jual = list.Where(m => m.Jenis == JenisMutasi.Jual && m.Tanggal.Year == hari.Year && m.Tanggal.Month == hari.Month).ToList();
        var pindah30 = list.Count(m => m.Jenis == JenisMutasi.Pindah && (hari - m.Tanggal.Date).TotalDays <= 30);
        var mati90 = list.Count(m => m.Jenis == JenisMutasi.Mati && (hari - m.Tanggal.Date).TotalDays <= 90);
        Kartu =
        [
            new("Terjual bulan ini", jual.Count.ToString(), "ekor", Format.Rupiah(jual.Sum(m => m.HargaJual ?? 0)), "ok", "wallet"),
            new("Pindah kandang", pindah30.ToString(), "kali", "30 hari terakhir", "info", "swap"),
            new("Kematian", mati90.ToString(), "ekor", "90 hari terakhir", "danger", "alert")
        ];
        Terapkan();
    }

    void Terapkan() => Daftar = semua.Where(x => Filter == "Semua" || x.M.Jenis.ToString() == Filter).Select(x => x.B).ToList();

    [RelayCommand]
    void Tambah()
    {
        if (Session.BolehMutasi) Shell.TampilDialog(new MutasiDialogViewModel(Shell, null));
    }

    [RelayCommand] void Buka(int id) => Shell.Buka("detail", id);
}
