using CommunityToolkit.Mvvm.ComponentModel;
using TernakKu.App.Services;
using TernakKu.App.ViewModels.Pages;
using TernakKu.Core.Models;

namespace TernakKu.App.ViewModels.Dialogs;

public record OpsiKandangJadwal(int Id, string Label);
public record OpsiVaksinJadwal(int Id, string Label);

public partial class JadwalVaksinViewModel : DialogViewModel
{
    public JadwalVaksinViewModel(ShellViewModel shell) : base(shell)
    {
        using var db = Session.Buka();
        var aktif = db.Ternak.Where(t => t.Status == StatusTernak.Aktif && t.KandangId != null).Select(t => t.KandangId!.Value).ToList();
        DaftarKandang = db.Kandang.OrderBy(k => k.Kode).AsEnumerable().Select(k => new OpsiKandangJadwal(k.Id, $"{k.Kode} · {k.Nama} ({aktif.Count(i => i == k.Id)} ekor)")).ToList();
        DaftarVaksin = db.ObatVaksin.Where(o => o.Jenis == JenisObat.Vaksin).OrderBy(o => o.Id).AsEnumerable().Select(o => new OpsiVaksinJadwal(o.Id, o.Nama)).ToList();
        IdKandang = DaftarKandang.FirstOrDefault()?.Id ?? 0;
        IdVaksin = DaftarVaksin.FirstOrDefault()?.Id ?? 0;
    }

    public override string Judul => "Jadwalkan vaksin";
    public override string TombolOk => "Jadwalkan";

    public List<OpsiKandangJadwal> DaftarKandang { get; }
    public List<OpsiVaksinJadwal> DaftarVaksin { get; }
    public DateTime BatasAwal => DateTime.Today;

    [ObservableProperty] private int idKandang;
    [ObservableProperty] private int idVaksin;
    [ObservableProperty] private DateTime? rencana = DateTime.Today.AddDays(7);

    public PesanBidang ErrRencana { get; } = new();
    public PesanBidang ErrKandang { get; } = new();

    protected override bool Simpan()
    {
        ErrRencana.Teks = ErrKandang.Teks = "";
        if (Rencana is null || Rencana.Value.Date < DateTime.Today) ErrRencana.Teks = "Pilih tanggal hari ini atau setelahnya";
        if (IdKandang == 0 || IdVaksin == 0) ErrKandang.Teks = "Pilih kandang dan vaksin";
        if (ErrRencana.Ada || ErrKandang.Ada) return false;

        var tgl = Rencana!.Value.Date;
        List<int> ids;
        using (var db = Session.Buka())
        {
            var ternak = db.Ternak.Where(t => t.Status == StatusTernak.Aktif && t.KandangId == IdKandang).ToList();
            if (ternak.Count == 0) { ErrKandang.Teks = "Kandang ini tidak berisi ternak aktif"; return false; }
            var baris = ternak.Select(t => new JadwalVaksinasi { TernakId = t.Id, ObatVaksinId = IdVaksin, TanggalRencana = tgl, Status = StatusJadwal.Terjadwal }).ToList();
            db.JadwalVaksinasi.AddRange(baris);
            db.SaveChanges();
            ids = baris.Select(b => b.Id).ToList();
        }
        Shell.TampilSnack($"{ids.Count} ekor dijadwalkan {Format.Tanggal(tgl)}", () =>
        {
            using var db = Session.Buka();
            db.JadwalVaksinasi.RemoveRange(db.JadwalVaksinasi.Where(j => ids.Contains(j.Id)));
            db.SaveChanges();
        });
        if (Shell.Halaman is KesehatanViewModel kv) kv.LompatKe(tgl);
        Shell.MuatUlang();
        Shell.SegarkanStatus();
        return true;
    }
}
