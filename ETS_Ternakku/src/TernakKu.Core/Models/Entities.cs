namespace TernakKu.Core.Models;

// Every table has Id and DibuatPada. Money is stored in whole rupiah (long) so SQLite can sum it.
public abstract class Entitas
{
    public int Id { get; set; }
    public DateTime DibuatPada { get; set; } = DateTime.Now;
}

public class Pengguna : Entitas
{
    public string Username { get; set; } = "";
    public string NamaLengkap { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public Peran Peran { get; set; }
    public bool Aktif { get; set; } = true;
    public DateTime? LoginTerakhir { get; set; }
}

public class Spesies : Entitas
{
    public string Nama { get; set; } = "";
    public string KodePrefix { get; set; } = "";
    public int MasaBuntingHari { get; set; }
    public List<Ras> Ras { get; set; } = [];
}

public class Ras : Entitas
{
    public int SpesiesId { get; set; }
    public Spesies Spesies { get; set; } = null!;
    public string Nama { get; set; } = "";
}

public class Kandang : Entitas
{
    public string Kode { get; set; } = "";
    public string Nama { get; set; } = "";
    public JenisKandang Jenis { get; set; }
    public int? SpesiesId { get; set; }
    public Spesies? Spesies { get; set; }
    public int Kapasitas { get; set; }
    public List<Ternak> Ternak { get; set; } = [];

    public string Label => $"{Kode} · {Nama}";
}

public class Ternak : Entitas
{
    public string KodeTernak { get; set; } = "";
    public string Nama { get; set; } = "";
    public int RasId { get; set; }
    public Ras Ras { get; set; } = null!;
    public int? KandangId { get; set; }
    public Kandang? Kandang { get; set; }
    public JenisKelamin JenisKelamin { get; set; }
    public DateTime TanggalLahir { get; set; }
    public AsalUsul AsalUsul { get; set; }
    public DateTime TanggalMasuk { get; set; }
    public long? HargaBeli { get; set; }
    public int? IndukId { get; set; }
    public Ternak? Induk { get; set; }
    public int? PejantanId { get; set; }
    public Ternak? Pejantan { get; set; }
    public StatusTernak Status { get; set; } = StatusTernak.Aktif;
    public Kondisi Kondisi { get; set; } = Kondisi.Sehat;
    public string DicatatOleh { get; set; } = "";
    public List<Penimbangan> Penimbangan { get; set; } = [];

    public string Label => string.IsNullOrEmpty(Nama) ? KodeTernak : $"{KodeTernak} · {Nama}";
}

public class Penimbangan : Entitas
{
    public int TernakId { get; set; }
    public Ternak Ternak { get; set; } = null!;
    public DateTime Tanggal { get; set; }
    public double BeratKg { get; set; }
    public double? LingkarDadaCm { get; set; }
    public string DicatatOleh { get; set; } = "";
}

public class Pakan : Entitas
{
    public string Nama { get; set; } = "";
    public string Satuan { get; set; } = "kg";
    public double Stok { get; set; }
    public double StokMinimum { get; set; }
    public long HargaPerSatuan { get; set; }
    public double PemakaianHarian { get; set; }
}

public class PakanMasuk : Entitas
{
    public int PakanId { get; set; }
    public Pakan Pakan { get; set; } = null!;
    public DateTime Tanggal { get; set; }
    public double Jumlah { get; set; }
    public long TotalHarga { get; set; }
    public string Pemasok { get; set; } = "";
    public string DicatatOleh { get; set; } = "";
}

public class PemberianPakan : Entitas
{
    public int KandangId { get; set; }
    public Kandang Kandang { get; set; } = null!;
    public int PakanId { get; set; }
    public Pakan Pakan { get; set; } = null!;
    public DateTime Waktu { get; set; }
    public double Jumlah { get; set; }
    public string DicatatOleh { get; set; } = "";
}

public class ObatVaksin : Entitas
{
    public string Nama { get; set; } = "";
    public JenisObat Jenis { get; set; }
    public string Satuan { get; set; } = "";
    public double Stok { get; set; }
    public double StokMinimum { get; set; }
}

public class Kesehatan : Entitas
{
    public int TernakId { get; set; }
    public Ternak Ternak { get; set; } = null!;
    public DateTime Tanggal { get; set; }
    public string Keluhan { get; set; } = "";
    public string Diagnosa { get; set; } = "";
    public string Tindakan { get; set; } = "";
    public int? ObatVaksinId { get; set; }
    public ObatVaksin? ObatVaksin { get; set; }
    public double? Dosis { get; set; }
    public string DicatatOleh { get; set; } = "";
}

public class JadwalVaksinasi : Entitas
{
    public int TernakId { get; set; }
    public Ternak Ternak { get; set; } = null!;
    public int ObatVaksinId { get; set; }
    public ObatVaksin ObatVaksin { get; set; } = null!;
    public DateTime TanggalRencana { get; set; }
    public DateTime? TanggalRealisasi { get; set; }
    public StatusJadwal Status { get; set; } = StatusJadwal.Terjadwal;
}

public class Perkawinan : Entitas
{
    public int IndukId { get; set; }
    public Ternak Induk { get; set; } = null!;
    public int? PejantanId { get; set; }
    public Ternak? Pejantan { get; set; }
    public MetodeKawin Metode { get; set; }
    public string KodeStraw { get; set; } = "";
    public DateTime Tanggal { get; set; }
    public DateTime PerkiraanLahir { get; set; }
    public StatusBunting Status { get; set; } = StatusBunting.Menunggu;
    public Kelahiran? Kelahiran { get; set; }
    public string DicatatOleh { get; set; } = "";
}

public class Kelahiran : Entitas
{
    public int PerkawinanId { get; set; }
    public Perkawinan Perkawinan { get; set; } = null!;
    public DateTime Tanggal { get; set; }
    public int JumlahAnak { get; set; }
    public int JumlahMati { get; set; }
    public string Catatan { get; set; } = "";
}

public class Mutasi : Entitas
{
    public int TernakId { get; set; }
    public Ternak Ternak { get; set; } = null!;
    public DateTime Tanggal { get; set; }
    public JenisMutasi Jenis { get; set; }
    public int? KandangAsalId { get; set; }
    public int? KandangTujuanId { get; set; }
    public long? HargaJual { get; set; }
    public string Pembeli { get; set; } = "";
    public string Penyebab { get; set; } = "";
    public string DicatatOleh { get; set; } = "";
}

public class Keuangan : Entitas
{
    public DateTime Tanggal { get; set; }
    public JenisKeuangan Jenis { get; set; }
    public string Kategori { get; set; } = "";
    public long Jumlah { get; set; }
    public string Keterangan { get; set; } = "";
    public string SumberTabel { get; set; } = "Manual";
    public int? SumberId { get; set; }
}

// Routine tasks on the dashboard timeline that were ticked off by hand.
public class TugasSelesai : Entitas
{
    public DateTime Tanggal { get; set; }
    public string Kunci { get; set; } = "";
}
