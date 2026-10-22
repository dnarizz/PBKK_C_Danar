using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using TernakKu.App.Services;
using TernakKu.Core.Models;

namespace TernakKu.App.ViewModels.Dialogs;

public record OpsiMaster(int Id, string Nama);

// One dialog for add/edit of spesies, ras, kandang, pakan and obat.
public partial class MasterEditViewModel : DialogViewModel
{
    readonly string entitas;
    readonly int? id;

    public MasterEditViewModel(ShellViewModel shell, string entitas, int? id) : base(shell)
    {
        this.entitas = entitas;
        this.id = id;
        using var db = Session.Buka();
        DaftarSpesies = db.Spesies.OrderBy(s => s.Id).AsEnumerable().Select(s => new OpsiMaster(s.Id, s.Nama)).ToList();
        DaftarSpesiesCampur = [new OpsiMaster(0, "Campuran"), .. DaftarSpesies];
        SpesiesId = entitas == "kandang" ? 0 : DaftarSpesies.FirstOrDefault()?.Id ?? 0;
        Jenis = entitas == "obat" ? nameof(JenisObat.Obat) : nameof(JenisKandang.Penggemukan);
        if (entitas == "obat") Satuan = "";
        if (id is null) return;

        switch (entitas)
        {
            case "spesies":
                var sp = db.Spesies.Find(id)!;
                Nama = sp.Nama; KodePrefix = sp.KodePrefix; MasaBunting = sp.MasaBuntingHari.ToString();
                break;
            case "ras":
                var ras = db.Ras.Find(id)!;
                Nama = ras.Nama; SpesiesId = ras.SpesiesId;
                break;
            case "kandang":
                var k = db.Kandang.Find(id)!;
                Kode = k.Kode; Nama = k.Nama; Jenis = k.Jenis.ToString(); SpesiesId = k.SpesiesId ?? 0; Kapasitas = k.Kapasitas.ToString();
                break;
            case "pakan":
                var p = db.Pakan.Find(id)!;
                Nama = p.Nama; Satuan = p.Satuan; StokMinimum = Teks(p.StokMinimum); Harga = p.HargaPerSatuan.ToString(); Pemakaian = Teks(p.PemakaianHarian);
                break;
            case "obat":
                var o = db.ObatVaksin.Find(id)!;
                Nama = o.Nama; Jenis = o.Jenis.ToString(); Satuan = o.Satuan; StokMinimum = Teks(o.StokMinimum);
                break;
        }
    }

    static string Teks(double n) => n.ToString("0.##", CultureInfo.InvariantCulture);

    string Label => entitas == "obat" ? "obat atau vaksin" : entitas;
    public override string Judul => $"{(id is null ? "Tambah" : "Ubah")} {Label}";

    public GalatForm Galat { get; } = new();

    public bool IsSpesies => entitas == "spesies";
    public bool IsRas => entitas == "ras";
    public bool IsKandang => entitas == "kandang";
    public bool IsPakan => entitas == "pakan";
    public bool IsObat => entitas == "obat";
    public bool PakaiNama => true;
    public bool PakaiSpesies => IsRas || IsKandang;
    public bool PakaiStokMinimum => IsPakan || IsObat;

    public List<OpsiMaster> DaftarSpesies { get; }
    public List<OpsiMaster> DaftarSpesiesCampur { get; }
    public List<OpsiMaster> DaftarSpesiesTampil => IsKandang ? DaftarSpesiesCampur : DaftarSpesies;
    public string[] DaftarJenisKandang { get; } = Enum.GetNames<JenisKandang>();
    public string[] DaftarJenisObat { get; } = Enum.GetNames<JenisObat>();
    public string[] DaftarSatuanPakan { get; } = ["kg", "karung", "buah"];

    [ObservableProperty] private string nama = "";
    [ObservableProperty] private string kode = "";
    [ObservableProperty] private string kodePrefix = "";
    [ObservableProperty] private string masaBunting = "";
    [ObservableProperty] private int spesiesId;
    [ObservableProperty] private string jenis = "";
    [ObservableProperty] private string kapasitas = "";
    [ObservableProperty] private string satuan = "kg";
    [ObservableProperty] private string stokMinimum = "";
    [ObservableProperty] private string harga = "";
    [ObservableProperty] private string pemakaian = "";

    protected override bool Simpan()
    {
        Galat.Bersih();
        double min = 0, pakai = 0, harga = 0, bunting = 0, kap = 0;
        if (string.IsNullOrWhiteSpace(Nama)) Galat.Set("Nama", "Wajib diisi");
        switch (entitas)
        {
            case "spesies":
                if (KodePrefix.Trim().Length != 2 || !KodePrefix.Trim().All(char.IsLetter)) Galat.Set("KodePrefix", "Isi 2 huruf");
                if (!InputAngka.TryParse(MasaBunting, out bunting) || bunting <= 0) Galat.Set("MasaBunting", "Isi masa bunting");
                break;
            case "kandang":
                if (string.IsNullOrWhiteSpace(Kode)) Galat.Set("Kode", "Wajib diisi");
                if (!InputAngka.TryParse(Kapasitas, out kap) || kap <= 0) Galat.Set("Kapasitas", "Isi kapasitas");
                break;
            case "pakan":
                if (!InputAngka.TryParse(StokMinimum, out min) || min < 0) Galat.Set("StokMinimum", "Isi angka 0 atau lebih");
                if (!InputAngka.TryParse(Harga, out harga) || harga < 0) Galat.Set("Harga", "Isi angka 0 atau lebih");
                if (!InputAngka.TryParse(Pemakaian, out pakai) || pakai < 0) Galat.Set("Pemakaian", "Isi angka 0 atau lebih");
                break;
            case "obat":
                if (string.IsNullOrWhiteSpace(Satuan)) Galat.Set("Satuan", "Wajib diisi");
                if (!InputAngka.TryParse(StokMinimum, out min) || min < 0) Galat.Set("StokMinimum", "Isi angka 0 atau lebih");
                break;
        }

        using var db = Session.Buka();
        if (entitas == "kandang" && !string.IsNullOrWhiteSpace(Kode))
        {
            var kodeBaru = Kode.Trim().ToUpperInvariant();
            if (db.Kandang.Any(k => k.Kode == kodeBaru && k.Id != (id ?? 0))) Galat.Set("Kode", "Kode sudah dipakai");
        }
        if (!Galat.Kosong) return false;

        var nm = Nama.Trim();
        switch (entitas)
        {
            case "spesies":
                var sp = id is null ? db.Spesies.Add(new Spesies()).Entity : db.Spesies.Find(id)!;
                sp.Nama = nm; sp.KodePrefix = KodePrefix.Trim().ToUpperInvariant(); sp.MasaBuntingHari = (int)bunting;
                break;
            case "ras":
                var ras = id is null ? db.Ras.Add(new Ras()).Entity : db.Ras.Find(id)!;
                ras.Nama = nm; ras.SpesiesId = SpesiesId;
                break;
            case "kandang":
                var kd = id is null ? db.Kandang.Add(new Kandang()).Entity : db.Kandang.Find(id)!;
                kd.Kode = Kode.Trim().ToUpperInvariant(); kd.Nama = nm; kd.Jenis = Enum.Parse<JenisKandang>(Jenis);
                kd.SpesiesId = SpesiesId == 0 ? null : SpesiesId; kd.Kapasitas = (int)kap;
                break;
            case "pakan":
                var pk = id is null ? db.Pakan.Add(new Pakan { Stok = 0 }).Entity : db.Pakan.Find(id)!;
                pk.Nama = nm; pk.Satuan = Satuan; pk.StokMinimum = min; pk.HargaPerSatuan = (long)Math.Round(harga); pk.PemakaianHarian = pakai;
                break;
            case "obat":
                var ob = id is null ? db.ObatVaksin.Add(new ObatVaksin { Stok = 0 }).Entity : db.ObatVaksin.Find(id)!;
                ob.Nama = nm; ob.Jenis = Enum.Parse<JenisObat>(Jenis); ob.Satuan = Satuan.Trim(); ob.StokMinimum = min;
                break;
        }
        db.SaveChanges();
        Shell.MuatUlang();
        Shell.SegarkanStatus();
        Shell.TampilSnack($"Data {Label} disimpan");
        return true;
    }
}
