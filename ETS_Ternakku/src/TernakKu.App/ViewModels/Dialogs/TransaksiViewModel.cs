using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using TernakKu.App.Services;
using TernakKu.Core.Models;

namespace TernakKu.App.ViewModels.Dialogs;

// Field error messages for a form. Bind with Galat[Nama] for the text and Galat.Ada[Nama] for the red border.
public class GalatForm : ObservableObject
{
    readonly Dictionary<string, string> pesan = [];
    public GalatForm() => Ada = new GalatAda(this);

    public string this[string kunci] => pesan.GetValueOrDefault(kunci, "");
    public GalatAda Ada { get; }
    public bool Kosong => pesan.Count == 0;

    public void Set(string kunci, string teks)
    {
        pesan[kunci] = teks;
        Ubah();
    }

    public void Bersih()
    {
        pesan.Clear();
        Ubah();
    }

    void Ubah()
    {
        OnPropertyChanged("Item[]");
        Ada.Ubah();
    }

    public sealed class GalatAda(GalatForm induk) : ObservableObject
    {
        public bool this[string kunci] => induk[kunci].Length > 0;
        internal void Ubah() => OnPropertyChanged("Item[]");
    }
}

public static class InputAngka
{
    // Accepts both "1.5" and "1,5".
    public static bool TryParse(string? teks, out double nilai) =>
        double.TryParse((teks ?? "").Trim().Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out nilai);
}

public partial class TransaksiViewModel : DialogViewModel
{
    static readonly string[] KategoriMasuk = ["Penjualan susu", "Penjualan pupuk", "Lain-lain"];
    static readonly string[] KategoriKeluar = ["Gaji pekerja", "Obat dan vaksin", "Listrik dan air", "Perbaikan kandang", "Lain-lain"];

    public TransaksiViewModel(ShellViewModel shell) : base(shell)
    {
        Kategori = KategoriKeluar[0];
    }

    public override string Judul => "Catat transaksi";

    public GalatForm Galat { get; } = new();
    public DateTime BatasTanggal => DateTime.Today;

    [ObservableProperty] private bool masuk;
    string kategori = "";

    // Ignores null: the ComboBox pushes null while its list is being replaced.
    public string? Kategori
    {
        get => kategori;
        set { if (value is not null) SetProperty(ref kategori, value); }
    }
    [ObservableProperty] private string jumlah = "";
    [ObservableProperty] private DateTime? tanggal = DateTime.Today;
    [ObservableProperty] private string keterangan = "";

    public bool Keluar
    {
        get => !Masuk;
        set { if (value) Masuk = false; }
    }

    public string[] DaftarKategori => Masuk ? KategoriMasuk : KategoriKeluar;

    partial void OnMasukChanged(bool value)
    {
        OnPropertyChanged(nameof(Keluar));
        OnPropertyChanged(nameof(DaftarKategori));
        Kategori = DaftarKategori[0];
    }

    protected override bool Simpan()
    {
        Galat.Bersih();
        if (!InputAngka.TryParse(Jumlah, out var rp) || rp <= 0) Galat.Set("Jumlah", "Isi jumlah lebih dari 0");
        if (Tanggal is null) Galat.Set("Tanggal", "Isi tanggal");
        if (!Galat.Kosong) return false;

        var baris = new Keuangan
        {
            Tanggal = Tanggal!.Value.Date,
            Jenis = Masuk ? JenisKeuangan.Masuk : JenisKeuangan.Keluar,
            Kategori = Kategori ?? "Lain-lain",
            Jumlah = (long)Math.Round(rp),
            Keterangan = string.IsNullOrWhiteSpace(Keterangan) ? Kategori ?? "Lain-lain" : Keterangan.Trim(),
            SumberTabel = "Manual"
        };
        using (var db = Session.Buka())
        {
            db.Keuangan.Add(baris);
            db.SaveChanges();
        }
        var id = baris.Id;
        Shell.MuatUlang();
        Shell.TampilSnack("Transaksi dicatat", () =>
        {
            using var db = Session.Buka();
            var x = db.Keuangan.Find(id);
            if (x is null) return;
            db.Keuangan.Remove(x);
            db.SaveChanges();
        });
        return true;
    }
}
