using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using TernakKu.App.Services;
using TernakKu.App.ViewModels.Dialogs;
using TernakKu.Core.Models;
using TernakKu.Core.Rules;

namespace TernakKu.App.ViewModels.Pages;

public record KartuStokPakan(string Nama, string Status, string Tone, string Jumlah, string Satuan, double Persen, string Sub);

public record OpsiKandangPakan(int Id, string Label, int Isi, int SpesiesId);

public record OpsiPakanPakan(int Id, string Label, string Nama, string Satuan, double Stok);

public record BarisPemberianPakan(string Waktu, string Kandang, string Pakan, string Jumlah, string Dicatat);

public record BarisPakanMasuk(string Tanggal, string Pakan, string Pemasok, string Jumlah, string Total);

public partial class PakanViewModel(ShellViewModel shell, int? kandangId) : PageViewModel(shell), IPunyaTambah
{
    public override string Key => "pakan";

    public bool BolehPakan => Session.BolehPakan;
    public string Subjudul => $"Stok gudang dan pemberian pakan hari ini · {Format.TanggalPanjang(DateTime.Today)}";

    [ObservableProperty] private List<KartuStokPakan> kartu = [];
    [ObservableProperty] private int kolomKartu = 3;
    [ObservableProperty] private List<OpsiKandangPakan> daftarKandang = [];
    [ObservableProperty] private List<OpsiPakanPakan> daftarPakan = [];
    [ObservableProperty] private int? idKandang;
    [ObservableProperty] private int? idPakan;
    [ObservableProperty] private List<BarisPemberianPakan> hariIni = [];
    [ObservableProperty] private string jumlahCatatan = "";
    [ObservableProperty] private List<BarisPakanMasuk> masuk = [];

    [ObservableProperty] private string jumlah = "";
    [ObservableProperty] private string satuan = "kg";
    [ObservableProperty] private string pesanError = "";
    [ObservableProperty] private string hint = "";
    [ObservableProperty] private bool tidakValid;
    [ObservableProperty] private string teksJatah = "";
    [ObservableProperty] private string teksSetengah = "";

    int nilaiJatah;
    int? awal = kandangId;
    bool sudahMuat;

    public IRelayCommand TambahCommand => tambahCommand ??= new RelayCommand(() => Shell.TampilDialog(new PakanMasukViewModel(Shell, null)), () => Session.BolehPakan);
    IRelayCommand? tambahCommand;

    partial void OnJumlahChanged(string value) => Hitung();
    partial void OnIdKandangChanged(int? value) => Hitung();
    partial void OnIdPakanChanged(int? value) => Hitung();

    public override void Load()
    {
        var hari = DateTime.Today;
        using var db = Session.Buka();
        var kandang = db.Kandang.OrderBy(k => k.Kode).ToList();
        var pakan = db.Pakan.OrderBy(p => p.Id).ToList();
        var aktif = db.Ternak.Where(t => t.Status == StatusTernak.Aktif && t.KandangId != null).Select(t => t.KandangId!.Value).ToList();

        var kartuBaru = pakan.Select(x =>
        {
            var st = AturanBisnis.HitungStatusStok(x.Stok, x.StokMinimum);
            var (teks, tone) = st switch { StatusStok.Habis => ("Habis", "danger"), StatusStok.Menipis => ("Menipis", "warn"), _ => ("Aman", "ok") };
            var cukup = x.PemakaianHarian > 0 ? $" · cukup ± {(int)Math.Floor(x.Stok / x.PemakaianHarian)} hari" : "";
            var persen = x.StokMinimum > 0 ? x.Stok / (x.StokMinimum * 3) * 100 : 0;
            return new KartuStokPakan(x.Nama, teks, tone, N(x.Stok), x.Satuan, Math.Clamp(persen, 0, 100),
                $"Minimum {N(x.StokMinimum)} · pakai {N(x.PemakaianHarian, x.PemakaianHarian < 1 ? 1 : 0)}/hari{cukup}");
        }).ToList();
        Kartu = kartuBaru;
        KolomKartu = kartuBaru.Count <= 4 ? Math.Max(1, kartuBaru.Count) : kartuBaru.Count <= 6 ? 3 : 4;

        var pilihK = IdKandang;
        var pilihP = IdPakan;
        DaftarKandang = kandang.Select(k => { var isi = aktif.Count(i => i == k.Id); return new OpsiKandangPakan(k.Id, $"{k.Kode} · {k.Nama} ({isi} ekor)", isi, k.SpesiesId ?? 1); }).ToList();
        DaftarPakan = pakan.Select(p => new OpsiPakanPakan(p.Id, $"{p.Nama} · stok {N(p.Stok)} {p.Satuan}", p.Nama, p.Satuan, p.Stok)).ToList();
        if (!sudahMuat && awal is int a && kandang.Any(k => k.Id == a)) pilihK = a;
        sudahMuat = true;
        IdKandang = kandang.Any(k => k.Id == pilihK) ? pilihK : kandang.FirstOrDefault()?.Id;
        IdPakan = pakan.Any(p => p.Id == pilihP) ? pilihP : pakan.FirstOrDefault()?.Id;
        Hitung();

        var log = db.PemberianPakan.Include(p => p.Kandang).Include(p => p.Pakan)
            .Where(p => p.Waktu >= hari && p.Waktu < hari.AddDays(1)).AsEnumerable().OrderByDescending(p => p.Waktu).ThenByDescending(p => p.Id).ToList();
        HariIni = log.Select(l => new BarisPemberianPakan(Format.Jam(l.Waktu), l.Kandang.Label, l.Pakan.Nama, $"{N(l.Jumlah)} {l.Pakan.Satuan}", l.DicatatOleh)).ToList();
        JumlahCatatan = $"{log.Count} catatan";

        var pm = db.PakanMasuk.Include(m => m.Pakan).AsEnumerable().OrderByDescending(m => m.Tanggal).ThenByDescending(m => m.Id).Take(6).ToList();
        Masuk = pm.Select(m => new BarisPakanMasuk(Format.Tanggal(m.Tanggal), m.Pakan.Nama, m.Pemasok, $"{N(m.Jumlah)} {m.Pakan.Satuan}", Format.Rupiah(m.TotalHarga))).ToList();
    }

    static string N(double v, int? des = null) => Format.Angka(v, des ?? (Math.Abs(v - Math.Round(v)) < 0.0005 ? 0 : 1));

    // Daily ration per head, as in the prototype jatah().
    static int Jatah(OpsiKandangPakan k, OpsiPakanPakan p)
    {
        if (p.Satuan != "kg") return Math.Max(1, (int)Math.Round(k.Isi / 20.0, MidpointRounding.AwayFromZero));
        double[] rate = p.Nama switch
        {
            "Konsentrat" => [2, 0.4, 0.3],
            "Dedak" => [1, 0.25, 0.2],
            "Rumput gajah" => [25, 4, 3],
            "Hijauan campur" => [8, 2, 1.5],
            "Ampas tahu" => [3, 0.5, 0.4],
            _ => [5, 1, 1]
        };
        return (int)Math.Round(k.Isi * rate[Math.Clamp(k.SpesiesId, 1, 3) - 1], MidpointRounding.AwayFromZero);
    }

    bool TryJumlah(out double nilai) =>
        double.TryParse(Jumlah.Trim().Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out nilai);

    void Hitung()
    {
        var p = DaftarPakan.FirstOrDefault(x => x.Id == IdPakan);
        var k = DaftarKandang.FirstOrDefault(x => x.Id == IdKandang);
        if (p is null || k is null) { SimpanCommand.NotifyCanExecuteChanged(); return; }
        Satuan = p.Satuan;
        nilaiJatah = Jatah(k, p);
        TeksJatah = $"Jatah harian · {N(nilaiJatah)} {p.Satuan}";
        TeksSetengah = $"Setengah · {N(Math.Round(nilaiJatah / 2.0, MidpointRounding.AwayFromZero))}";
        var kosong = string.IsNullOrWhiteSpace(Jumlah);
        var ok = TryJumlah(out var j);
        var lebih = !kosong && ok && j > p.Stok;
        var salah = !kosong && !(ok && j > 0);
        TidakValid = lebih || salah;
        PesanError = lebih ? $"Melebihi stok gudang ({N(p.Stok)} {p.Satuan})" : salah ? "Isi jumlah lebih dari 0" : "";
        Hint = TidakValid ? "" : $"Stok gudang: {N(p.Stok)} {p.Satuan}";
        SimpanCommand.NotifyCanExecuteChanged();
    }

    bool BisaSimpan() => Session.BolehPakan && !string.IsNullOrWhiteSpace(Jumlah) && !TidakValid && IdKandang is not null && IdPakan is not null;

    [RelayCommand]
    void IsiCepat(string jenis)
    {
        var nilai = jenis == "jatah" ? nilaiJatah : Math.Round(nilaiJatah / 2.0, MidpointRounding.AwayFromZero);
        Jumlah = nilai.ToString("0.##", CultureInfo.InvariantCulture);
    }

    [RelayCommand(CanExecute = nameof(BisaSimpan))]
    void Simpan()
    {
        if (!TryJumlah(out var j) || IdKandang is not int kid || IdPakan is not int pid) return;
        int baris;
        string nama, unit;
        using (var db = Session.Buka())
        {
            var p = db.Pakan.Find(pid)!;
            if (!AturanBisnis.PemberianPakanValid(j, p.Stok)) { Hitung(); return; }
            p.Stok -= j;
            var row = new PemberianPakan { KandangId = kid, PakanId = pid, Waktu = DateTime.Now, Jumlah = j, DicatatOleh = Session.Username };
            db.PemberianPakan.Add(row);
            db.SaveChanges();
            baris = row.Id;
            nama = p.Nama;
            unit = p.Satuan;
        }
        Jumlah = "";
        Shell.TampilSnack($"{N(j)} {unit} {nama} tercatat", () =>
        {
            using var db = Session.Buka();
            var p = db.Pakan.Find(pid);
            if (p is not null) p.Stok += j;
            var row = db.PemberianPakan.Find(baris);
            if (row is not null) db.PemberianPakan.Remove(row);
            db.SaveChanges();
        });
        Load();
        Shell.SegarkanStatus();
    }
}
