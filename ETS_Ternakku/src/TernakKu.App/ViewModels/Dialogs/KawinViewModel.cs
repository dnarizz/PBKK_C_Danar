using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using TernakKu.App.Services;
using TernakKu.Core.Models;
using TernakKu.Core.Rules;

namespace TernakKu.App.ViewModels.Dialogs;

public record OpsiIndukKawin(int Id, string Label, int SpesiesId, string Spesies, int Masa);
public record OpsiPejantanKawin(int Id, string Label, int SpesiesId);

public partial class KawinViewModel : DialogViewModel
{
    readonly List<OpsiPejantanKawin> semuaPejantan;

    public KawinViewModel(ShellViewModel shell) : base(shell)
    {
        using var db = Session.Buka();
        var sibuk = db.Perkawinan.Where(k => k.Status == StatusBunting.Bunting || k.Status == StatusBunting.Menunggu).Select(k => k.IndukId).ToHashSet();
        var aktif = db.Ternak.Include(t => t.Ras).ThenInclude(r => r.Spesies).Where(t => t.Status == StatusTernak.Aktif).AsEnumerable().OrderBy(t => t.KodeTernak).ToList();
        DaftarInduk = [new(0, "Pilih induk", 0, "", 0), .. aktif.Where(t => t.JenisKelamin == JenisKelamin.Betina && !sibuk.Contains(t.Id))
            .Select(t => new OpsiIndukKawin(t.Id, $"{t.Label} — {t.Ras.Spesies.Nama} {t.Ras.Nama}", t.Ras.SpesiesId, t.Ras.Spesies.Nama, t.Ras.Spesies.MasaBuntingHari))];
        semuaPejantan = aktif.Where(t => t.JenisKelamin == JenisKelamin.Jantan).Select(t => new OpsiPejantanKawin(t.Id, $"{t.Label} — {t.Ras.Nama}", t.Ras.SpesiesId)).ToList();
        DaftarPejantan = [new(0, "Pilih pejantan", 0), .. semuaPejantan];
        Hitung();
    }

    public override string Judul => "Catat perkawinan";
    public override bool Lebar => true;

    public List<OpsiIndukKawin> DaftarInduk { get; }
    public DateTime BatasTanggal => DateTime.Today;

    [ObservableProperty] private List<OpsiPejantanKawin> daftarPejantan;
    [ObservableProperty] private int idInduk;
    [ObservableProperty, NotifyPropertyChangedFor(nameof(IsAlami))] private bool ib;
    [ObservableProperty] private DateTime? tanggal = DateTime.Today;
    [ObservableProperty] private int idPejantan;
    [ObservableProperty] private string kodeStraw = "";
    [ObservableProperty] private string perkiraan = "—";
    [ObservableProperty] private string hintPerkiraan = "Pilih induk dulu";

    public bool IsAlami => !Ib;

    public PesanBidang ErrInduk { get; } = new();
    public PesanBidang ErrPejantan { get; } = new();
    public PesanBidang ErrStraw { get; } = new();
    public PesanBidang ErrTanggal { get; } = new();

    partial void OnIdIndukChanged(int value)
    {
        var sp = DaftarInduk.First(i => i.Id == value).SpesiesId;
        DaftarPejantan = [new(0, "Pilih pejantan", 0), .. semuaPejantan.Where(p => value == 0 || p.SpesiesId == sp)];
        IdPejantan = 0;
        Hitung();
    }

    partial void OnTanggalChanged(DateTime? value) => Hitung();

    [RelayCommand] void AturMetode(string metode) => Ib = metode == "ib";

    void Hitung()
    {
        var induk = DaftarInduk.FirstOrDefault(i => i.Id == IdInduk);
        if (induk is null || induk.Id == 0)
        {
            Perkiraan = "—";
            HintPerkiraan = "Pilih induk dulu";
            return;
        }
        Perkiraan = Tanggal is { } t ? Format.Tanggal(AturanBisnis.PerkiraanLahir(t, induk.Masa)) : "—";
        HintPerkiraan = $"Tanggal kawin + {induk.Masa} hari ({induk.Spesies})";
    }

    protected override bool Simpan()
    {
        foreach (var e in new[] { ErrInduk, ErrPejantan, ErrStraw, ErrTanggal }) e.Teks = "";
        var induk = DaftarInduk.FirstOrDefault(i => i.Id == IdInduk && i.Id != 0);
        if (induk is null) ErrInduk.Teks = "Pilih induk";
        if (Ib && string.IsNullOrWhiteSpace(KodeStraw)) ErrStraw.Teks = "Isi kode straw";
        if (!Ib && IdPejantan == 0) ErrPejantan.Teks = "Pilih pejantan";
        if (Tanggal is null || Tanggal.Value.Date > DateTime.Today) ErrTanggal.Teks = "Tanggal tidak boleh di masa depan";
        if (ErrInduk.Ada || ErrPejantan.Ada || ErrStraw.Ada || ErrTanggal.Ada || induk is null) return false;

        var tgl = Tanggal!.Value.Date;
        int id;
        string kode;
        using (var db = Session.Buka())
        {
            kode = db.Ternak.Find(induk.Id)!.KodeTernak;
            var k = new Perkawinan
            {
                IndukId = induk.Id, PejantanId = Ib ? null : IdPejantan, Metode = Ib ? MetodeKawin.IB : MetodeKawin.Alami,
                KodeStraw = Ib ? KodeStraw.Trim() : "", Tanggal = tgl, PerkiraanLahir = AturanBisnis.PerkiraanLahir(tgl, induk.Masa),
                Status = StatusBunting.Menunggu, DicatatOleh = Session.Username
            };
            db.Perkawinan.Add(k);
            db.SaveChanges();
            id = k.Id;
        }
        Shell.TampilSnack($"Perkawinan {kode} dicatat", () =>
        {
            using var db = Session.Buka();
            var k = db.Perkawinan.Find(id);
            if (k is not null) { db.Perkawinan.Remove(k); db.SaveChanges(); }
        });
        Shell.MuatUlang();
        Shell.SegarkanStatus();
        return true;
    }
}
