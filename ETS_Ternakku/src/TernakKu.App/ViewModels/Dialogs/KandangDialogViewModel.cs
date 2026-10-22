using CommunityToolkit.Mvvm.ComponentModel;
using TernakKu.App.Services;
using TernakKu.Core.Models;

namespace TernakKu.App.ViewModels.Dialogs;

public partial class KandangDialogViewModel(ShellViewModel shell) : DialogViewModel(shell)
{
    public record OpsiSpesies(int Id, string Teks);

    public override string Judul => "Tambah kandang";

    public JenisKandang[] DaftarJenis { get; } = [JenisKandang.Penggemukan, JenisKandang.Indukan, JenisKandang.Karantina];
    public List<OpsiSpesies> DaftarSpesies { get; } = Muat();

    static List<OpsiSpesies> Muat()
    {
        using var db = Session.Buka();
        return [new(0, "Campuran"), .. db.Spesies.OrderBy(s => s.Id).AsEnumerable().Select(s => new OpsiSpesies(s.Id, s.Nama))];
    }

    [ObservableProperty, NotifyPropertyChangedFor(nameof(KodeSalah))] private string kode = "";
    [ObservableProperty, NotifyPropertyChangedFor(nameof(KodeSalah))] private string kodeError = "";
    [ObservableProperty, NotifyPropertyChangedFor(nameof(NamaSalah))] private string nama = "";
    [ObservableProperty, NotifyPropertyChangedFor(nameof(NamaSalah))] private string namaError = "";
    [ObservableProperty] private JenisKandang jenis = JenisKandang.Penggemukan;
    [ObservableProperty] private int spesiesId;
    [ObservableProperty, NotifyPropertyChangedFor(nameof(KapasitasSalah))] private string kapasitas = "";
    [ObservableProperty, NotifyPropertyChangedFor(nameof(KapasitasSalah))] private string kapasitasError = "";

    public bool KodeSalah => KodeError.Length > 0;
    public bool NamaSalah => NamaError.Length > 0;
    public bool KapasitasSalah => KapasitasError.Length > 0;

    protected override bool Simpan()
    {
        KodeError = NamaError = KapasitasError = "";
        var k = Kode.Trim().ToUpperInvariant();
        using var db = Session.Buka();
        if (k.Length == 0) KodeError = "Wajib diisi";
        else if (db.Kandang.Any(x => x.Kode == k)) KodeError = $"Kode {k} sudah dipakai";
        if (string.IsNullOrWhiteSpace(Nama)) NamaError = "Wajib diisi";
        if (!int.TryParse(Kapasitas.Trim(), out var kap) || kap <= 0) KapasitasError = "Isi kapasitas";
        if (KodeError.Length + NamaError.Length + KapasitasError.Length > 0) return false;

        var baru = new Kandang { Kode = k, Nama = Nama.Trim(), Jenis = Jenis, SpesiesId = SpesiesId == 0 ? null : SpesiesId, Kapasitas = kap };
        db.Kandang.Add(baru);
        db.SaveChanges();
        var id = baru.Id;
        Shell.TampilSnack($"Kandang {k} ditambahkan", () =>
        {
            using var d = Session.Buka();
            var x = d.Kandang.Find(id);
            if (x is not null) { d.Kandang.Remove(x); d.SaveChanges(); }
        });
        Shell.Buka("kandang", id);
        return true;
    }
}
