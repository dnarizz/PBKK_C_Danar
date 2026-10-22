using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;
using TernakKu.App.Services;
using TernakKu.Core.Models;

namespace TernakKu.App.ViewModels.Dialogs;

// Error text under a field plus a flag for the red border.
public partial class PesanBidang : ObservableObject
{
    [ObservableProperty, NotifyPropertyChangedFor(nameof(Ada))] private string teks = "";
    public bool Ada => Teks.Length > 0;
}

public record OpsiPakanMasuk(int Id, string Label, string Satuan, long Harga);

public partial class PakanMasukViewModel : DialogViewModel
{
    public PakanMasukViewModel(ShellViewModel shell, int? pakanId) : base(shell)
    {
        using var db = Session.Buka();
        DaftarPakan = db.Pakan.OrderBy(p => p.Id).AsEnumerable()
            .Select(p => new OpsiPakanMasuk(p.Id, $"{p.Nama} (stok {Format.Angka(p.Stok)} {p.Satuan})", p.Satuan, p.HargaPerSatuan)).ToList();
        IdPakan = DaftarPakan.Any(p => p.Id == pakanId) ? pakanId!.Value : DaftarPakan.FirstOrDefault()?.Id ?? 0;
    }

    public override string Judul => "Catat pakan masuk";
    public List<OpsiPakanMasuk> DaftarPakan { get; }
    public DateTime BatasTanggal => DateTime.Today;

    [ObservableProperty, NotifyPropertyChangedFor(nameof(LabelJumlah), nameof(PlaceholderTotal), nameof(HintTotal))] private int idPakan;
    [ObservableProperty, NotifyPropertyChangedFor(nameof(PlaceholderTotal))] private string jumlah = "";
    [ObservableProperty] private string total = "";
    [ObservableProperty] private string pemasok = "";
    [ObservableProperty] private DateTime? tanggal = DateTime.Today;

    public PesanBidang ErrJumlah { get; } = new();
    public PesanBidang ErrTotal { get; } = new();
    public PesanBidang ErrTanggal { get; } = new();

    OpsiPakanMasuk? Pilihan => DaftarPakan.FirstOrDefault(p => p.Id == IdPakan);
    public string LabelJumlah => $"Jumlah ({Pilihan?.Satuan})";
    public string PlaceholderTotal => Baca(Jumlah, out var j) && j > 0 && Pilihan is { } p ? Format.Angka(Math.Round(j * p.Harga)) : "0";
    public string HintTotal => Pilihan is { } p ? $"Kosongkan untuk {Format.Rupiah(p.Harga)} per {p.Satuan}" : "";

    static bool Baca(string s, out double nilai) =>
        double.TryParse(s.Trim().Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out nilai);

    protected override bool Simpan()
    {
        ErrJumlah.Teks = ErrTotal.Teks = ErrTanggal.Teks = "";
        var ok = Baca(Jumlah, out var j);
        if (!(ok && j > 0)) ErrJumlah.Teks = "Isi jumlah lebih dari 0";
        long? totalManual = null;
        if (!string.IsNullOrWhiteSpace(Total))
        {
            if (Baca(Total, out var t) && t > 0) totalManual = (long)Math.Round(t);
            else ErrTotal.Teks = "Isi total harga lebih dari 0 atau kosongkan";
        }
        if (Tanggal is null || Tanggal.Value.Date > DateTime.Today) ErrTanggal.Teks = "Tanggal tidak boleh di masa depan";
        if (Pilihan is null || ErrJumlah.Ada || ErrTotal.Ada || ErrTanggal.Ada) return false;

        var tgl = Tanggal!.Value.Date;
        var pemasokTeks = string.IsNullOrWhiteSpace(Pemasok) ? "—" : Pemasok.Trim();
        int pmId, kuId;
        string nama, satuan;
        long totalHarga;
        using (var db = Session.Buka())
        {
            var p = db.Pakan.Find(IdPakan)!;
            totalHarga = totalManual ?? (long)Math.Round(j * p.HargaPerSatuan);
            nama = p.Nama;
            satuan = p.Satuan;
            using var tx = db.Database.BeginTransaction();
            p.Stok += j;
            var pm = new PakanMasuk { PakanId = p.Id, Tanggal = tgl, Jumlah = j, TotalHarga = totalHarga, Pemasok = pemasokTeks, DicatatOleh = Session.Username };
            db.PakanMasuk.Add(pm);
            db.SaveChanges();
            var ku = new Keuangan
            {
                Tanggal = tgl, Jenis = JenisKeuangan.Keluar, Kategori = "Pakan", Jumlah = totalHarga,
                Keterangan = $"{p.Nama} {Format.Angka(j)} {p.Satuan} · {pemasokTeks}", SumberTabel = "PakanMasuk", SumberId = pm.Id
            };
            db.Keuangan.Add(ku);
            db.SaveChanges();
            tx.Commit();
            pmId = pm.Id;
            kuId = ku.Id;
        }
        var pakanId = IdPakan;
        Shell.TampilSnack($"{nama} bertambah {Format.Angka(j)} {satuan}", () =>
        {
            using var db = Session.Buka();
            var p = db.Pakan.Find(pakanId);
            if (p is not null) p.Stok -= j;
            var pm = db.PakanMasuk.Find(pmId);
            if (pm is not null) db.PakanMasuk.Remove(pm);
            var ku = db.Keuangan.Find(kuId);
            if (ku is not null) db.Keuangan.Remove(ku);
            db.SaveChanges();
        });
        Shell.MuatUlang();
        Shell.SegarkanStatus();
        return true;
    }
}
