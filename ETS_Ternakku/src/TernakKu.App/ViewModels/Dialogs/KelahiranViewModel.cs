using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;
using TernakKu.App.Services;
using TernakKu.Core.Models;

namespace TernakKu.App.ViewModels.Dialogs;

public record OpsiKandangLahir(int Id, string Label);
public record OpsiAngkaLahir(int Nilai);
public record OpsiKelaminLahir(JenisKelamin Nilai, string Teks);

public partial class AnakKelahiran(int nomor) : ObservableObject
{
    static readonly List<OpsiKelaminLahir> Opsi = [new(JenisKelamin.Jantan, "Jantan"), new(JenisKelamin.Betina, "Betina")];
    public int Nomor { get; } = nomor;
    public string LabelKelamin => $"Anak {Nomor} · kelamin";
    public string LabelBerat => $"Anak {Nomor} · berat lahir (kg)";
    public List<OpsiKelaminLahir> DaftarKelamin => Opsi;
    [ObservableProperty] private JenisKelamin kelamin = JenisKelamin.Betina;
    [ObservableProperty] private string berat = "";
}

public partial class KelahiranViewModel : DialogViewModel
{
    readonly int perkawinanId;
    readonly int indukId;
    readonly string kodeInduk;
    readonly bool sapi;
    readonly AnakKelahiran[] semuaAnak = [new(1), new(2), new(3), new(4)];

    public KelahiranViewModel(ShellViewModel shell, int perkawinanId) : base(shell)
    {
        this.perkawinanId = perkawinanId;
        using var db = Session.Buka();
        var k = db.Perkawinan.Include(x => x.Induk).ThenInclude(t => t.Ras).ThenInclude(r => r.Spesies).First(x => x.Id == perkawinanId);
        indukId = k.IndukId;
        kodeInduk = k.Induk.KodeTernak;
        sapi = k.Induk.Ras.Spesies.Nama == "Sapi";
        var isi = db.Ternak.Where(t => t.Status == StatusTernak.Aktif && t.KandangId != null).Select(t => t.KandangId!.Value).ToList();
        DaftarKandang = db.Kandang.OrderBy(x => x.Kode).AsEnumerable().Select(x => new OpsiKandangLahir(x.Id, $"{x.Kode} · {x.Nama} ({isi.Count(i => i == x.Id)}/{x.Kapasitas})")).ToList();
        IdKandang = k.Induk.KandangId ?? DaftarKandang.FirstOrDefault()?.Id ?? 0;
        Perbarui();
    }

    public override string Judul => $"Catat kelahiran · {kodeInduk}";
    public override bool Lebar => true;

    public List<OpsiKandangLahir> DaftarKandang { get; }
    public List<OpsiAngkaLahir> DaftarJumlah { get; } = [new(1), new(2), new(3), new(4)];
    public List<OpsiAngkaLahir> DaftarMati { get; } = [new(0), new(1), new(2), new(3), new(4)];
    public DateTime BatasTanggal => DateTime.Today;

    [ObservableProperty] private DateTime? tanggal = DateTime.Today;
    [ObservableProperty] private int jumlah = 1;
    [ObservableProperty] private int mati;
    [ObservableProperty] private int idKandang;
    [ObservableProperty] private List<AnakKelahiran> anak = [];
    [ObservableProperty] private string ringkas = "";

    public PesanBidang ErrMati { get; } = new();
    public PesanBidang ErrKandang { get; } = new();
    public PesanBidang ErrTanggal { get; } = new();

    partial void OnJumlahChanged(int value) => Perbarui();
    partial void OnMatiChanged(int value) => Perbarui();

    int Hidup => Math.Max(0, Jumlah - Mati);

    void Perbarui()
    {
        Anak = semuaAnak.Take(Math.Min(Hidup, 4)).ToList();
        Ringkas = $"{Hidup} anak akan didaftarkan ke data ternak dengan induk {kodeInduk}.";
    }

    static string KodeBerikut(TernakKu.Data.TernakDbContext db, Spesies sp, List<string> terpakai)
    {
        var semua = db.Ternak.Where(t => t.KodeTernak.StartsWith(sp.KodePrefix + "-")).Select(t => t.KodeTernak).ToList().Concat(terpakai).ToList();
        var angka = semua.Select(c => c[(c.IndexOf('-') + 1)..]).ToList();
        var maks = angka.Select(a => int.TryParse(a, out var n) ? n : 0).DefaultIfEmpty(0).Max();
        var pad = Math.Max(3, angka.Select(a => a.Length).DefaultIfEmpty(3).Max());
        return $"{sp.KodePrefix}-{(maks + 1).ToString().PadLeft(pad, '0')}";
    }

    protected override bool Simpan()
    {
        ErrMati.Teks = ErrKandang.Teks = ErrTanggal.Teks = "";
        var hidup = Mati > Jumlah ? -1 : Hidup;
        if (hidup < 0) ErrMati.Teks = "Lebih banyak dari jumlah anak";
        if (Tanggal is null || Tanggal.Value.Date > DateTime.Today) ErrTanggal.Teks = "Tanggal tidak boleh di masa depan";
        if (ErrMati.Ada || ErrTanggal.Ada) return false;

        var tgl = Tanggal!.Value.Date;
        var idBaru = new List<int>();
        int lahirId;
        using (var db = Session.Buka())
        {
            var k = db.Perkawinan.Include(x => x.Induk).ThenInclude(t => t.Ras).ThenInclude(r => r.Spesies).First(x => x.Id == perkawinanId);
            if (hidup > 0)
            {
                var kd = db.Kandang.Find(IdKandang);
                var isi = db.Ternak.Count(t => t.Status == StatusTernak.Aktif && t.KandangId == IdKandang);
                if (kd is null) { ErrKandang.Teks = "Pilih kandang"; return false; }
                if (isi + hidup > kd.Kapasitas) { ErrKandang.Teks = "Kapasitas kandang tidak cukup"; return false; }
            }
            var kelahiran = new Kelahiran { PerkawinanId = k.Id, Tanggal = tgl, JumlahAnak = Jumlah, JumlahMati = Mati };
            db.Kelahiran.Add(kelahiran);
            var sp = k.Induk.Ras.Spesies;
            var baru = new List<Ternak>();
            var kodeBaru = new List<string>();
            for (var i = 0; i < hidup; i++)
            {
                var a = semuaAnak[i];
                var ok = double.TryParse(a.Berat.Trim().Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out var b);
                var berat = ok && b > 0 ? b : sapi ? 25 : 3;
                var kode = KodeBerikut(db, sp, kodeBaru);
                kodeBaru.Add(kode);
                var t = new Ternak
                {
                    KodeTernak = kode, Nama = "", RasId = k.Induk.RasId, KandangId = IdKandang, JenisKelamin = a.Kelamin, TanggalLahir = tgl,
                    AsalUsul = AsalUsul.Lahir, TanggalMasuk = tgl, IndukId = k.IndukId, PejantanId = k.PejantanId, Status = StatusTernak.Aktif,
                    Kondisi = Kondisi.Sehat, DicatatOleh = Session.Username
                };
                t.Penimbangan.Add(new Penimbangan { Ternak = t, Tanggal = tgl, BeratKg = berat, DicatatOleh = Session.Username });
                db.Ternak.Add(t);
                baru.Add(t);
            }
            k.Status = StatusBunting.Lahir;
            db.SaveChanges();
            idBaru = baru.Select(t => t.Id).ToList();
            lahirId = kelahiran.Id;
        }
        var perk = perkawinanId;
        Shell.TampilSnack($"{hidup} anak {kodeInduk} didaftarkan", () =>
        {
            using var db = Session.Buka();
            db.Penimbangan.RemoveRange(db.Penimbangan.Where(p => idBaru.Contains(p.TernakId)));
            db.Ternak.RemoveRange(db.Ternak.Where(t => idBaru.Contains(t.Id)));
            var l = db.Kelahiran.Find(lahirId);
            if (l is not null) db.Kelahiran.Remove(l);
            var p = db.Perkawinan.Find(perk);
            if (p is not null) p.Status = StatusBunting.Bunting;
            db.SaveChanges();
        });
        Shell.MuatUlang();
        Shell.SegarkanStatus();
        return true;
    }
}
