using TernakKu.Core.Models;

namespace TernakKu.Core.Rules;

// Business rules shared by every screen. Pure C#, no UI or database dependency.
public static class AturanBisnis
{
    public const int HariPengingatVaksin = 3;
    public const int HariCekBunting = 60;

    public static DateTime PerkiraanLahir(DateTime tanggalKawin, int masaBuntingHari) =>
        tanggalKawin.Date.AddDays(masaBuntingHari);

    // ADG in kg/day between the latest weighing and the one at least `rentangHari` before it.
    public static double? Adg(IReadOnlyList<Penimbangan> urut, int rentangHari = 30)
    {
        if (urut.Count < 2) return null;
        var akhir = urut[^1];
        var batas = akhir.Tanggal.Date.AddDays(-rentangHari);
        var awal = urut[0];
        foreach (var p in urut)
            if (p.Tanggal.Date <= batas) awal = p;
        if (ReferenceEquals(awal, akhir)) awal = urut[^2];
        var hari = (akhir.Tanggal.Date - awal.Tanggal.Date).TotalDays;
        return hari > 0 ? (akhir.BeratKg - awal.BeratKg) / hari : null;
    }

    public static string Umur(DateTime lahir, DateTime hariIni)
    {
        var bulan = (int)Math.Floor((hariIni.Date - lahir.Date).TotalDays / 30.44);
        var tahun = bulan / 12;
        return tahun > 0 ? $"{tahun} th {bulan % 12} bl" : $"{Math.Max(bulan, 0)} bl";
    }

    public static StatusStok HitungStatusStok(double stok, double minimum) =>
        stok <= 0 ? StatusStok.Habis : stok < minimum ? StatusStok.Menipis : StatusStok.Aman;

    public static bool KandangPenuh(int isi, int kapasitas) => isi >= kapasitas;

    public static int PersenTerisi(int isi, int kapasitas) =>
        kapasitas <= 0 ? 0 : (int)Math.Round(isi * 100.0 / kapasitas);

    // Below 80% green, 80-99% amber, full red.
    public static string TingkatKeterisian(int persen) => persen >= 100 ? "danger" : persen >= 80 ? "warn" : "ok";

    public static bool PemberianPakanValid(double jumlah, double stok) => jumlah > 0 && jumlah <= stok;

    public static bool JadwalPerluDiingat(JadwalVaksinasi j, DateTime hariIni)
    {
        var selisih = (j.TanggalRencana.Date - hariIni.Date).TotalDays;
        return j.Status == StatusJadwal.Terjadwal && selisih >= 0 && selisih <= HariPengingatVaksin;
    }

    public static StatusJadwal StatusJadwalHariIni(JadwalVaksinasi j, DateTime hariIni) =>
        j.Status == StatusJadwal.Terjadwal && j.TanggalRencana.Date < hariIni.Date ? StatusJadwal.Terlewat : j.Status;

    public static string HariRelatif(DateTime tanggal, DateTime hariIni)
    {
        var n = (int)(tanggal.Date - hariIni.Date).TotalDays;
        return n switch
        {
            0 => "hari ini",
            1 => "besok",
            -1 => "kemarin",
            > 0 => $"{n} hari lagi",
            _ => $"{-n} hari lalu"
        };
    }

    public static readonly IReadOnlyDictionary<Peran, string> LabelPeran = new Dictionary<Peran, string>
    {
        [Peran.Admin] = "Pemilik peternakan",
        [Peran.Petugas] = "Petugas kandang",
        [Peran.Mantri] = "Mantri hewan",
        [Peran.Tamu] = "Database tamu"
    };
}
