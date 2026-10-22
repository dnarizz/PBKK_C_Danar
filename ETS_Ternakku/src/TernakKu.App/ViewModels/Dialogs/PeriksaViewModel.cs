using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;
using TernakKu.App.Services;
using TernakKu.Core.Models;

namespace TernakKu.App.ViewModels.Dialogs;

public record OpsiTernakPeriksa(int Id, string Label);
public record OpsiKondisiPeriksa(Kondisi Nilai, string Teks);
public record OpsiObatPeriksa(int Id, string Label, string Satuan, double Stok);

public partial class PeriksaViewModel : DialogViewModel
{
    public PeriksaViewModel(ShellViewModel shell, int? ternakId) : base(shell)
    {
        using var db = Session.Buka();
        var aktif = db.Ternak.Include(t => t.Kandang).Where(t => t.Status == StatusTernak.Aktif).AsEnumerable().OrderBy(t => t.KodeTernak).ToList();
        DaftarTernak = [new(0, "Pilih ternak"), .. aktif.Select(t => new OpsiTernakPeriksa(t.Id, $"{t.Label} — {t.Kandang?.Kode ?? "tanpa kandang"}"))];
        DaftarObat = [new(0, "— tanpa obat —", "", 0), .. db.ObatVaksin.OrderBy(o => o.Id).AsEnumerable()
            .Select(o => new OpsiObatPeriksa(o.Id, $"{o.Nama} · stok {Format.Angka(o.Stok)} {o.Satuan}", o.Satuan, o.Stok))];
        IdTernak = aktif.Any(t => t.Id == ternakId) ? ternakId!.Value : 0;
    }

    public override string Judul => "Catat pemeriksaan";
    public override bool Lebar => true;

    public List<OpsiTernakPeriksa> DaftarTernak { get; }
    public List<OpsiObatPeriksa> DaftarObat { get; }
    public List<OpsiKondisiPeriksa> DaftarKondisi { get; } =
        [new(Kondisi.Diobati, "Diobati"), new(Kondisi.Sehat, "Sehat"), new(Kondisi.Periksa, "Perlu diperiksa lagi")];
    public DateTime BatasTanggal => DateTime.Today;

    [ObservableProperty] private int idTernak;
    [ObservableProperty] private DateTime? tanggal = DateTime.Today;
    [ObservableProperty] private Kondisi kondisi = Kondisi.Diobati;
    [ObservableProperty] private string keluhan = "";
    [ObservableProperty] private string diagnosa = "";
    [ObservableProperty] private string tindakan = "";
    [ObservableProperty, NotifyPropertyChangedFor(nameof(AdaObat), nameof(LabelDosis))] private int idObat;
    [ObservableProperty] private string dosis = "";

    public PesanBidang ErrTernak { get; } = new();
    public PesanBidang ErrTanggal { get; } = new();
    public PesanBidang ErrKeluhan { get; } = new();
    public PesanBidang ErrDiagnosa { get; } = new();
    public PesanBidang ErrDosis { get; } = new();

    public bool AdaObat => IdObat != 0;
    public string LabelDosis => AdaObat ? $"Dosis ({DaftarObat.First(o => o.Id == IdObat).Satuan})" : "Dosis";

    protected override bool Simpan()
    {
        foreach (var e in new[] { ErrTernak, ErrTanggal, ErrKeluhan, ErrDiagnosa, ErrDosis }) e.Teks = "";
        if (IdTernak == 0) ErrTernak.Teks = "Pilih ternak";
        if (Tanggal is null || Tanggal.Value.Date > DateTime.Today) ErrTanggal.Teks = "Tanggal tidak boleh di masa depan";
        if (string.IsNullOrWhiteSpace(Keluhan)) ErrKeluhan.Teks = "Isi keluhan";
        if (string.IsNullOrWhiteSpace(Diagnosa)) ErrDiagnosa.Teks = "Isi diagnosa";
        var obat = DaftarObat.FirstOrDefault(o => o.Id == IdObat && o.Id != 0);
        double dosisNilai = 0;
        if (obat is not null)
        {
            if (!double.TryParse(Dosis.Trim().Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out dosisNilai) || dosisNilai <= 0) ErrDosis.Teks = "Isi dosis";
            else if (dosisNilai > obat.Stok) ErrDosis.Teks = $"Stok hanya {Format.Angka(obat.Stok)} {obat.Satuan}";
        }
        if (ErrTernak.Ada || ErrTanggal.Ada || ErrKeluhan.Ada || ErrDiagnosa.Ada || ErrDosis.Ada) return false;

        int catatanId;
        Kondisi sebelum;
        string kode;
        using (var db = Session.Buka())
        {
            var t = db.Ternak.Find(IdTernak)!;
            kode = t.KodeTernak;
            sebelum = t.Kondisi;
            var k = new Kesehatan
            {
                TernakId = t.Id, Tanggal = Tanggal!.Value.Date, Keluhan = Keluhan.Trim(), Diagnosa = Diagnosa.Trim(),
                Tindakan = string.IsNullOrWhiteSpace(Tindakan) ? "—" : Tindakan.Trim(),
                ObatVaksinId = obat?.Id, Dosis = obat is null ? null : dosisNilai, DicatatOleh = Session.Username
            };
            db.Kesehatan.Add(k);
            if (obat is not null) db.ObatVaksin.Find(obat.Id)!.Stok -= dosisNilai;
            t.Kondisi = Kondisi;
            db.SaveChanges();
            catatanId = k.Id;
        }
        var ternakId = IdTernak;
        var obatId = obat?.Id;
        var pakai = dosisNilai;
        Shell.TampilSnack($"Pemeriksaan {kode} disimpan", () =>
        {
            using var db = Session.Buka();
            var k = db.Kesehatan.Find(catatanId);
            if (k is not null) db.Kesehatan.Remove(k);
            if (obatId is int oid) { var o = db.ObatVaksin.Find(oid); if (o is not null) o.Stok += pakai; }
            var t = db.Ternak.Find(ternakId);
            if (t is not null) t.Kondisi = sebelum;
            db.SaveChanges();
        });
        Shell.MuatUlang();
        Shell.SegarkanStatus();
        return true;
    }
}
