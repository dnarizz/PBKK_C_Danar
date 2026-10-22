using TernakKu.Core.Models;
using TernakKu.Core.Rules;

namespace TernakKu.Data;

public record AkunDemo(Peran Peran, string Username, string Password, string Nama);

// Fills a new database with realistic sample data. The guest database gets a smaller farm.
public static class Seeder
{
    public const string PasswordAwal = "ternakku";

    // Shown in the login screen's credentials panel and used to seed the account database.
    public static readonly IReadOnlyList<AkunDemo> AkunDemo =
    [
        new(Peran.Admin, "admin", PasswordAwal, "Admin"),
        new(Peran.Petugas, "petugas", PasswordAwal, "Budi Santoso"),
        new(Peran.Mantri, "mantri", PasswordAwal, "drh. Sari Wulandari")
    ];

    static readonly string[] NamaJantan = ["Arjuna", "Gatot", "Jaka", "Bagus", "Tegar", "Raja", "Brewok", "Kumbang", "Guntur", "Samson", "Badai", "Joko", "Ableh", "Dodo", "Petruk", "Bejo"];
    static readonly string[] NamaBetina = ["Sri", "Melati", "Mawar", "Ayu", "Dewi", "Putri", "Lestari", "Wangi", "Kenanga", "Sekar", "Ratna", "Tari", "Intan", "Ningsih", "Asih", "Laras"];

    public static void Isi(TernakDbContext db, JenisDatabase jenis)
    {
        var tamu = jenis == JenisDatabase.Tamu;
        var rnd = new Random(tamu ? 77 : 2026);
        var hariIni = DateTime.Today;
        T Pilih<T>(IReadOnlyList<T> list) => list[rnd.Next(list.Count)];
        double Antara(double a, double b) => a + rnd.NextDouble() * (b - a);
        int Int(int a, int b) => rnd.Next(a, b + 1);

        // Users (only the account database has real accounts).
        if (!tamu)
        {
            foreach (var a in AkunDemo)
                db.Pengguna.Add(new Pengguna { Username = a.Username, NamaLengkap = a.Nama, Peran = a.Peran, PasswordHash = BCrypt.Net.BCrypt.HashPassword(a.Password), LoginTerakhir = hariIni.AddDays(-Int(0, 2)) });
            db.Pengguna.Add(new Pengguna { Username = "yanto", NamaLengkap = "Yanto", Peran = Peran.Petugas, Aktif = false, PasswordHash = BCrypt.Net.BCrypt.HashPassword(PasswordAwal), LoginTerakhir = hariIni.AddDays(-64) });
        }

        // Master data.
        var sapi = new Spesies { Nama = "Sapi", KodePrefix = "SP", MasaBuntingHari = 283 };
        var kambing = new Spesies { Nama = "Kambing", KodePrefix = "KB", MasaBuntingHari = 150 };
        var domba = new Spesies { Nama = "Domba", KodePrefix = "DM", MasaBuntingHari = 147 };
        var spesies = new[] { sapi, kambing, domba };
        db.Spesies.AddRange(spesies);
        var ras = new List<Ras>
        {
            new() { Spesies = sapi, Nama = "Bali" }, new() { Spesies = sapi, Nama = "Limosin" }, new() { Spesies = sapi, Nama = "Simental" }, new() { Spesies = sapi, Nama = "PO" },
            new() { Spesies = kambing, Nama = "Etawa" }, new() { Spesies = kambing, Nama = "Kacang" }, new() { Spesies = kambing, Nama = "Boer" },
            new() { Spesies = domba, Nama = "Garut" }, new() { Spesies = domba, Nama = "Merino" }
        };
        db.Ras.AddRange(ras);

        var defKandang = tamu
            ? new (string, string, JenisKandang, Spesies?, int, int)[]
            {
                ("A", "Penggemukan", JenisKandang.Penggemukan, sapi, 10, 6), ("B", "Indukan", JenisKandang.Indukan, sapi, 10, 8),
                ("C", "Kambing", JenisKandang.Indukan, kambing, 12, 10), ("D", "Domba", JenisKandang.Penggemukan, domba, 8, 4)
            }
            : new (string, string, JenisKandang, Spesies?, int, int)[]
            {
                ("A", "Penggemukan", JenisKandang.Penggemukan, sapi, 40, 38), ("B", "Indukan", JenisKandang.Indukan, sapi, 60, 52),
                ("C", "Kambing", JenisKandang.Penggemukan, kambing, 80, 61), ("D", "Domba", JenisKandang.Penggemukan, domba, 40, 30),
                ("E", "Sapi muda", JenisKandang.Penggemukan, sapi, 30, 22), ("G", "Kambing indukan", JenisKandang.Indukan, kambing, 40, 33),
                ("K", "Karantina", JenisKandang.Karantina, null, 30, 12)
            };
        var kandang = defKandang.Select(k => new Kandang { Kode = k.Item1, Nama = k.Item2, Jenis = k.Item3, Spesies = k.Item4, Kapasitas = k.Item5 }).ToList();
        db.Kandang.AddRange(kandang);

        // Animals with weighing history.
        var nomor = new Dictionary<Spesies, int> { [sapi] = 0, [kambing] = 0, [domba] = 0 };
        var ternak = new List<Ternak>();
        var timbang = new List<Penimbangan>();
        string KodeBaru(Spesies s)
        {
            nomor[s]++;
            return $"{s.KodePrefix}-{nomor[s].ToString(s == sapi ? "D4" : "D3")}";
        }
        Ternak Tambah(Kandang? k, Spesies? sp = null, JenisKelamin? jk = null, int? umurHari = null)
        {
            var s = sp ?? k?.Spesies ?? Pilih(spesies);
            var kelamin = jk ?? (k?.Jenis switch
            {
                JenisKandang.Indukan => rnd.NextDouble() < 0.9 ? JenisKelamin.Betina : JenisKelamin.Jantan,
                JenisKandang.Penggemukan => rnd.NextDouble() < 0.85 ? JenisKelamin.Jantan : JenisKelamin.Betina,
                _ => rnd.NextDouble() < 0.5 ? JenisKelamin.Jantan : JenisKelamin.Betina
            });
            var umur = umurHari ?? (s == sapi ? Int(320, 1500) : Int(200, 1100));
            var lahir = hariIni.AddDays(-umur);
            var beli = rnd.NextDouble() < 0.62;
            var masuk = beli ? hariIni.AddDays(-Int(40, Math.Min(umur - 60, 420))) : lahir;
            double berat, adg;
            if (s == sapi) { berat = Math.Min(480, 120 + umur * 0.28 + Antara(-20, 30)); adg = kelamin == JenisKelamin.Jantan ? Antara(0.55, 1.0) : Antara(0.28, 0.55); }
            else if (s == kambing) { berat = Math.Min(62, 14 + umur * 0.04 + Antara(-4, 6)); adg = Antara(0.05, 0.12); }
            else { berat = Math.Min(48, 14 + umur * 0.032 + Antara(-3, 5)); adg = Antara(0.06, 0.14); }
            berat = Math.Round(berat, 1);
            var t = new Ternak
            {
                KodeTernak = KodeBaru(s),
                Nama = rnd.NextDouble() < 0.55 ? Pilih(kelamin == JenisKelamin.Jantan ? NamaJantan : NamaBetina) : "",
                Ras = Pilih(ras.Where(r => r.Spesies == s).ToList()),
                Kandang = k,
                JenisKelamin = kelamin,
                TanggalLahir = lahir,
                AsalUsul = beli ? AsalUsul.Beli : AsalUsul.Lahir,
                TanggalMasuk = masuk,
                HargaBeli = beli ? (long)Math.Round((s == sapi ? Antara(9, 18) : Antara(1.6, 3.4)) * 1e6 / 50000) * 50000 : null,
                DicatatOleh = "admin"
            };
            ternak.Add(t);
            var rentang = (hariIni - masuk).TotalDays;
            var n = Math.Min(10, Math.Max(2, (int)(rentang / 14)));
            for (var i = n - 1; i >= 0; i--)
            {
                var lalu = i * 14 + (i == 0 ? Int(0, 3) : 0);
                var b = berat - adg * lalu + (i == 0 ? 0 : Antara(-0.6, 0.6) * (s == sapi ? 2 : 0.3));
                timbang.Add(new Penimbangan { Ternak = t, Tanggal = hariIni.AddDays(-lalu), BeratKg = Math.Round(Math.Max(b, 4), 1), LingkarDadaCm = s == sapi ? Int(140, 200) : Int(55, 85), DicatatOleh = Pilih(new[] { "petugas", "admin" }) });
            }
            return t;
        }

        foreach (var (k, def) in kandang.Zip(defKandang))
            for (var i = 0; i < def.Item6; i++)
            {
                if (k.Jenis == JenisKandang.Karantina) Tambah(k, Pilih(new[] { sapi, sapi, kambing, domba }), umurHari: Int(250, 900));
                else if (k.Kode == "E") Tambah(k, umurHari: Int(240, 480));
                else Tambah(k);
            }

        if (!tamu)
        {
            // The animal used in the design mock-ups.
            var bima = ternak.First(t => t.Kandang == kandang[0]);
            bima.Nama = "Bima"; bima.Ras = ras[1]; bima.JenisKelamin = JenisKelamin.Jantan; bima.AsalUsul = AsalUsul.Beli;
            bima.TanggalLahir = hariIni.AddDays(-830); bima.TanggalMasuk = hariIni.AddDays(-203); bima.HargaBeli = 14_500_000;
            var milik = timbang.Where(w => w.Ternak == bima).ToList();
            for (var i = 0; i < milik.Count; i++) milik[i].BeratKg = Math.Round(412.5 - (milik.Count - 1 - i) * 14 * 0.92 + (i % 2 == 1 ? 0.4 : -0.3), 1);
            milik[^1].BeratKg = 412.5;
        }

        // A few purchases this month so the dashboard shows recent arrivals.
        foreach (var (t, i) in ternak.Where(t => t.AsalUsul == AsalUsul.Beli && t.Nama != "Bima").Take(tamu ? 2 : 6).Select((t, i) => (t, i)))
            t.TanggalMasuk = hariIni.AddDays(-Math.Min(i * 3, hariIni.Day - 1));

        // Parents for animals born on the farm.
        var betina = ternak.Where(t => t.JenisKelamin == JenisKelamin.Betina).ToList();
        var jantan = ternak.Where(t => t.JenisKelamin == JenisKelamin.Jantan).ToList();
        foreach (var t in ternak.Where(t => t.AsalUsul == AsalUsul.Lahir))
        {
            if (rnd.NextDouble() >= 0.7) continue;
            var sp = t.Ras.Spesies;
            t.Induk = betina.FirstOrDefault(x => x.Ras.Spesies == sp && x != t && x.TanggalLahir < t.TanggalLahir);
            t.Pejantan = jantan.FirstOrDefault(x => x.Ras.Spesies == sp && x != t && x.TanggalLahir < t.TanggalLahir);
        }

        // Sold, dead and moved animals.
        var mutasi = new List<Mutasi>();
        var keuangan = new List<Keuangan>();
        for (var i = 0; i < (tamu ? 3 : 15); i++)
        {
            var t = Tambah(null, Pilih(new[] { sapi, kambing, kambing, domba }));
            var jual = i % 5 != 4;
            t.Status = jual ? StatusTernak.Terjual : StatusTernak.Mati;
            var tgl = hariIni.AddDays(-Int(3, 170));
            if (jual)
            {
                var harga = (long)Math.Round((t.Ras.Spesies == sapi ? Antara(16, 26) : Antara(2.5, 4.5)) * 1e6 / 50000) * 50000;
                var pembeli = Pilih(new[] { "Pak Darto", "CV Sumber Rejeki", "Bu Ningsih", "Pasar Hewan Muntilan", "H. Saleh" });
                mutasi.Add(new Mutasi { Ternak = t, Tanggal = tgl, Jenis = JenisMutasi.Jual, HargaJual = harga, Pembeli = pembeli, DicatatOleh = "admin" });
                keuangan.Add(new Keuangan { Tanggal = tgl, Jenis = JenisKeuangan.Masuk, Kategori = "Penjualan ternak", Jumlah = harga, Keterangan = $"{t.KodeTernak} · {pembeli}", SumberTabel = "Mutasi" });
            }
            else mutasi.Add(new Mutasi { Ternak = t, Tanggal = tgl, Jenis = JenisMutasi.Mati, Penyebab = Pilih(new[] { "Kembung (bloat)", "Diare berat", "Cacingan parah" }), DicatatOleh = "mantri" });
        }
        var aktif = ternak.Where(t => t.Status == StatusTernak.Aktif).ToList();
        for (var i = 0; i < (tamu ? 2 : 6); i++)
        {
            var t = Pilih(aktif);
            var asal = kandang[(kandang.IndexOf(t.Kandang!) + 1) % kandang.Count];
            mutasi.Add(new Mutasi { Ternak = t, Tanggal = hariIni.AddDays(-Int(2, 60)), Jenis = JenisMutasi.Pindah, KandangAsalId = null, KandangTujuanId = null, Penyebab = $"{asal.Kode}>{t.Kandang!.Kode}", DicatatOleh = "petugas" });
        }

        // Health.
        var obat = new List<ObatVaksin>
        {
            new() { Nama = "Vaksin PMK", Jenis = JenisObat.Vaksin, Satuan = "dosis", Stok = 46, StokMinimum = 20 },
            new() { Nama = "Vaksin Anthrax", Jenis = JenisObat.Vaksin, Satuan = "dosis", Stok = 30, StokMinimum = 20 },
            new() { Nama = "Albendazole (obat cacing)", Jenis = JenisObat.Obat, Satuan = "bolus", Stok = 18, StokMinimum = 25 },
            new() { Nama = "Oxytetracycline", Jenis = JenisObat.Obat, Satuan = "ml", Stok = 240, StokMinimum = 100 },
            new() { Nama = "Vitamin B kompleks", Jenis = JenisObat.Vitamin, Satuan = "ml", Stok = 320, StokMinimum = 100 },
            new() { Nama = "Antiseptik povidone", Jenis = JenisObat.Obat, Satuan = "ml", Stok = 60, StokMinimum = 80 }
        };
        db.ObatVaksin.AddRange(obat);
        var keluhan = new (string, string, string, int?, double?)[]
        {
            ("Nafsu makan turun", "Indigesti ringan", "Vitamin B kompleks 10 ml, pakan hijauan", 4, 10),
            ("Diare", "Enteritis", "Oxytetracycline 15 ml, cairan elektrolit", 3, 15),
            ("Luka di kaki", "Luka terbuka", "Dibersihkan, antiseptik", 5, 5),
            ("Kurus, bulu kusam", "Cacingan", "Albendazole 1 bolus", 2, 1),
            ("Perut kembung", "Bloat ringan", "Minyak nabati 100 ml, diajak jalan", null, null),
            ("Batuk dan pilek", "ISPA", "Oxytetracycline 10 ml", 3, 10)
        };
        var sakit = new List<Ternak>();
        for (var i = 0; i < (tamu ? 5 : 22); i++)
        {
            var t = Pilih(aktif);
            var k = Pilih(keluhan);
            var lalu = i < (tamu ? 1 : 3) ? Int(0, 3) : Int(4, 120);
            db.Kesehatan.Add(new Kesehatan { Ternak = t, Tanggal = hariIni.AddDays(-lalu), Keluhan = k.Item1, Diagnosa = k.Item2, Tindakan = k.Item3, ObatVaksin = k.Item4 is int oi ? obat[oi] : null, Dosis = k.Item5, DicatatOleh = "mantri" });
            if (lalu <= 3) sakit.Add(t);
        }
        for (var i = 0; i < sakit.Count; i++) sakit[i].Kondisi = i < 2 ? Kondisi.Diobati : Kondisi.Periksa;
        for (var i = 0; i < (tamu ? 1 : 2); i++) { var t = Pilih(aktif); if (t.Kondisi == Kondisi.Sehat) t.Kondisi = Kondisi.Periksa; }

        List<Ternak> DiKandang(string kode) => aktif.Where(t => t.Kandang?.Kode == kode).ToList();
        void Jadwal(IEnumerable<Ternak> list, ObatVaksin o, int offset, StatusJadwal status)
        {
            foreach (var t in list)
                db.JadwalVaksinasi.Add(new JadwalVaksinasi { Ternak = t, ObatVaksin = o, TanggalRencana = hariIni.AddDays(offset), TanggalRealisasi = status == StatusJadwal.Selesai ? hariIni.AddDays(offset) : null, Status = status });
        }
        Jadwal(DiKandang("B").Take(tamu ? 4 : 12), obat[0], 0, StatusJadwal.Terjadwal);
        Jadwal(DiKandang("C").Take(tamu ? 3 : 8), obat[1], 6, StatusJadwal.Terjadwal);
        Jadwal(DiKandang("A").Take(tamu ? 3 : 10), obat[0], -12, StatusJadwal.Selesai);
        Jadwal(DiKandang("D").Take(tamu ? 2 : 4), obat[0], -3, StatusJadwal.Terlewat);
        Jadwal(DiKandang("A").Take(tamu ? 2 : 6), obat[1], 13, StatusJadwal.Terjadwal);
        if (!tamu) Jadwal(DiKandang("G").Take(6), obat[0], 21, StatusJadwal.Terjadwal);

        // Reproduction.
        var nBunting = tamu ? 4 : 18;
        var indukAktif = aktif.Where(t => t.JenisKelamin == JenisKelamin.Betina).ToList();
        var dipakai = new HashSet<Ternak>();
        for (var i = 0; i < nBunting + (tamu ? 3 : 8); i++)
        {
            var ibu = indukAktif.FirstOrDefault(t => !dipakai.Contains(t));
            if (ibu is null) break;
            dipakai.Add(ibu);
            var sp = ibu.Ras.Spesies;
            var bapak = aktif.FirstOrDefault(t => t.JenisKelamin == JenisKelamin.Jantan && t.Ras.Spesies == sp);
            int lalu; StatusBunting status;
            if (i < nBunting) { lalu = i < 3 ? sp.MasaBuntingHari - Int(1, 6) : Int(70, sp.MasaBuntingHari - 10); status = StatusBunting.Bunting; }
            else if (i < nBunting + (tamu ? 2 : 5)) { lalu = Int(10, 50); status = StatusBunting.Menunggu; }
            else { lalu = Int(80, 140); status = StatusBunting.Gagal; }
            var ib = sp == sapi && rnd.NextDouble() < 0.6;
            var tgl = hariIni.AddDays(-lalu);
            db.Perkawinan.Add(new Perkawinan
            {
                Induk = ibu, Pejantan = ib ? null : bapak, Metode = ib ? MetodeKawin.IB : MetodeKawin.Alami,
                KodeStraw = ib ? $"{Pilih(new[] { "LIM", "SIM", "BAL" })}-{Int(10000, 99999)}" : "",
                Tanggal = tgl, PerkiraanLahir = AturanBisnis.PerkiraanLahir(tgl, sp.MasaBuntingHari), Status = status, DicatatOleh = "mantri"
            });
        }

        // Feed.
        var pakan = tamu
            ? new List<Pakan>
            {
                new() { Nama = "Konsentrat", Satuan = "kg", Stok = 90, StokMinimum = 60, HargaPerSatuan = 4500, PemakaianHarian = 25 },
                new() { Nama = "Rumput gajah", Satuan = "kg", Stok = 420, StokMinimum = 150, HargaPerSatuan = 600, PemakaianHarian = 140 },
                new() { Nama = "Dedak", Satuan = "kg", Stok = 35, StokMinimum = 40, HargaPerSatuan = 3200, PemakaianHarian = 12 },
                new() { Nama = "Mineral blok", Satuan = "buah", Stok = 6, StokMinimum = 4, HargaPerSatuan = 18000, PemakaianHarian = 0.5 }
            }
            : new List<Pakan>
            {
                new() { Nama = "Konsentrat", Satuan = "kg", Stok = 120, StokMinimum = 200, HargaPerSatuan = 4500, PemakaianHarian = 85 },
                new() { Nama = "Dedak", Satuan = "kg", Stok = 140, StokMinimum = 150, HargaPerSatuan = 3200, PemakaianHarian = 40 },
                new() { Nama = "Rumput gajah", Satuan = "kg", Stok = 2400, StokMinimum = 800, HargaPerSatuan = 600, PemakaianHarian = 950 },
                new() { Nama = "Hijauan campur", Satuan = "kg", Stok = 900, StokMinimum = 400, HargaPerSatuan = 700, PemakaianHarian = 300 },
                new() { Nama = "Ampas tahu", Satuan = "kg", Stok = 380, StokMinimum = 150, HargaPerSatuan = 1500, PemakaianHarian = 90 },
                new() { Nama = "Mineral blok", Satuan = "buah", Stok = 24, StokMinimum = 10, HargaPerSatuan = 18000, PemakaianHarian = 1 }
            };
        db.Pakan.AddRange(pakan);
        var jam = new[] { (6, 10), (6, 25), (6, 40), (7, 5), (7, 20), (7, 35) };
        for (var i = 0; i < (tamu ? 3 : 5); i++)
        {
            var k = kandang[i];
            var p = pakan[i % 2 == 0 ? 0 : (tamu ? 1 : 2)];
            var isi = defKandang[i].Item6;
            var konsentrat = p.Nama == "Konsentrat";
            var jumlah = Math.Round(k.Spesies == sapi ? isi * (konsentrat ? 2 : 12) : isi * (konsentrat ? 0.4 : 3));
            db.PemberianPakan.Add(new PemberianPakan { Kandang = k, Pakan = p, Waktu = hariIni.AddHours(jam[i].Item1).AddMinutes(jam[i].Item2), Jumlah = jumlah, DicatatOleh = "petugas" });
        }
        var pemasok = new[] { "UD Tani Makmur", "KUD Sido Rukun", "Pak Wiryo (rumput)", "Toko Pakan Sentosa" };
        for (var m = 0; m < 6; m++)
            foreach (var p in pakan.Take(tamu ? 2 : 5))
            {
                var jumlah = Math.Round(p.PemakaianHarian * Int(12, 20) / 10) * 10;
                var tgl = hariIni.AddDays(-(m * 30 + Int(2, 25)));
                var nama = Pilih(pemasok);
                var pm = new PakanMasuk { Pakan = p, Tanggal = tgl, Jumlah = jumlah, TotalHarga = (long)(jumlah * p.HargaPerSatuan), Pemasok = nama, DicatatOleh = "admin" };
                db.PakanMasuk.Add(pm);
                keuangan.Add(new Keuangan { Tanggal = tgl, Jenis = JenisKeuangan.Keluar, Kategori = "Pakan", Jumlah = pm.TotalHarga, Keterangan = $"{p.Nama} {jumlah:N0} {p.Satuan} · {nama}", SumberTabel = "PakanMasuk" });
            }

        // Finance rows entered by hand.
        for (var m = 0; m < 6; m++)
        {
            var dasar = hariIni.AddDays(-m * 30);
            var rows = new List<(JenisKeuangan, string, long, string)>
            {
                (JenisKeuangan.Keluar, "Gaji pekerja", tamu ? 2_500_000 : 7_500_000, "Gaji 3 anak kandang"),
                (JenisKeuangan.Keluar, "Obat dan vaksin", Int(4, 12) * 150_000L, "Belanja apotek hewan"),
                (JenisKeuangan.Keluar, "Listrik dan air", Int(5, 8) * 100_000L, "Tagihan bulanan"),
                (JenisKeuangan.Masuk, "Penjualan pupuk", Int(6, 14) * 100_000L, "Pupuk kandang karungan")
            };
            if (!tamu && m % 2 == 0) rows.Add((JenisKeuangan.Masuk, "Penjualan susu", Int(18, 30) * 100_000L, "Susu kambing etawa"));
            foreach (var r in rows)
                keuangan.Add(new Keuangan { Tanggal = dasar.AddDays(-Int(0, 26)), Jenis = r.Item1, Kategori = r.Item2, Jumlah = r.Item3, Keterangan = r.Item4, SumberTabel = "Manual" });
        }

        db.Ternak.AddRange(ternak);
        db.Penimbangan.AddRange(timbang);
        db.SaveChanges();

        // Moves reference kandang ids, which exist only after the first save.
        foreach (var m in mutasi.Where(m => m.Jenis == JenisMutasi.Pindah))
        {
            var kode = m.Penyebab.Split('>');
            m.KandangAsalId = kandang.First(k => k.Kode == kode[0]).Id;
            m.KandangTujuanId = kandang.First(k => k.Kode == kode[1]).Id;
            m.Penyebab = "";
        }
        db.Mutasi.AddRange(mutasi);
        db.Keuangan.AddRange(keuangan);
        db.SaveChanges();
    }
}
