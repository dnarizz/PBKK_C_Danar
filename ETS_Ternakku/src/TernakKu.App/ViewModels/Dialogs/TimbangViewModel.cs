using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using TernakKu.App.Services;
using TernakKu.Core.Models;

namespace TernakKu.App.ViewModels.Dialogs;

public partial class TimbangViewModel : DialogViewModel
{
    readonly int ternakId;
    readonly string kode;

    public override string Judul => $"Catat penimbangan · {kode}";
    public DateTime BatasTanggal { get; } = DateTime.Today;
    public string Petunjuk { get; } = "";

    [ObservableProperty] private DateTime? tanggal = DateTime.Today;
    [ObservableProperty, NotifyPropertyChangedFor(nameof(BeratSalah))] private string berat = "";
    [ObservableProperty, NotifyPropertyChangedFor(nameof(BeratSalah))] private string beratError = "";
    [ObservableProperty] private string lingkarDada = "";

    public bool BeratSalah => BeratError.Length > 0;

    public TimbangViewModel(ShellViewModel shell, int ternakId) : base(shell)
    {
        this.ternakId = ternakId;
        using var db = Session.Buka();
        kode = db.Ternak.Where(t => t.Id == ternakId).Select(t => t.KodeTernak).First();
        var terakhir = db.Penimbangan.Where(p => p.TernakId == ternakId).OrderByDescending(p => p.Tanggal).ThenByDescending(p => p.Id).FirstOrDefault();
        if (terakhir is not null) Petunjuk = $"Terakhir {Format.Angka(terakhir.BeratKg, 1)} kg · {Format.TanggalPendek(terakhir.Tanggal)}";
    }

    static double? Angka(string s) =>
        double.TryParse(s.Trim().Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out var v) ? v : null;

    protected override bool Simpan()
    {
        BeratError = "";
        var kg = Angka(Berat);
        if (kg is null or <= 0) { BeratError = "Isi berat lebih dari 0"; return false; }
        var tgl = (Tanggal ?? DateTime.Today).Date;
        var dada = Angka(LingkarDada);
        int id;
        using (var db = Session.Buka())
        {
            var p = new Penimbangan { TernakId = ternakId, Tanggal = tgl, BeratKg = kg.Value, LingkarDadaCm = dada is > 0 ? dada : null, DicatatOleh = Session.Username };
            db.Penimbangan.Add(p);
            db.SaveChanges();
            id = p.Id;
        }
        Shell.TampilSnack($"Penimbangan {kode} disimpan", () =>
        {
            using var d = Session.Buka();
            var x = d.Penimbangan.Find(id);
            if (x is not null) { d.Penimbangan.Remove(x); d.SaveChanges(); }
        });
        Shell.MuatUlang();
        return true;
    }
}
