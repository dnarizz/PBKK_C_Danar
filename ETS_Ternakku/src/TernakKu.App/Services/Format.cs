using System.Globalization;

namespace TernakKu.App.Services;

// Indonesian number, money and date formatting used across the screens.
public static class Format
{
    public static readonly CultureInfo Id = CultureInfo.GetCultureInfo("id-ID");

    public static string Angka(double n, int desimal = 0) => n.ToString($"N{desimal}", Id);

    public static string Rupiah(double n) => $"Rp {Math.Round(n).ToString("N0", Id)}";

    public static string RupiahPendek(double n)
    {
        var a = Math.Abs(n);
        var s = a >= 1e9 ? $"{(a / 1e9).ToString("N1", Id)} M"
            : a >= 1e6 ? $"{(a / 1e6).ToString("N1", Id)} jt"
            : a >= 1e3 ? $"{(a / 1e3).ToString("N0", Id)} rb"
            : a.ToString("N0", Id);
        return $"{(n < 0 ? "−" : "")}Rp {s}";
    }

    public static string Tanggal(DateTime d) => d.ToString("d MMM yyyy", Id);
    public static string TanggalPendek(DateTime d) => d.ToString("d MMM", Id);
    public static string TanggalPanjang(DateTime d) => d.ToString("dddd, d MMMM yyyy", Id);
    public static string Jam(DateTime d) => d.ToString("HH.mm", Id);

    public static string Selisih(double? v, int desimal = 2) =>
        v is null ? "—" : $"{(v >= 0 ? "+" : "−")}{Math.Abs(v.Value).ToString($"N{desimal}", Id)}";
}
