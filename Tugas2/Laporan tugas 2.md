Program ini merupakan aplikasi berbasis konsol (CLI) yang dibangun menggunakan bahasa pemrograman C# pada lingkungan .NET (Visual Studio). Tujuan pembuatan program adalah untuk menerapkan konsep dasar Pemrograman Berorientasi Objek (OOP) dan struktur data koleksi (Collection) dalam mengelola data mahasiswa secara dinamis (CRUD: Create, Read, Delete, Search).

## atribut
Class Mahasiswa

Berfungsi sebagai blueprint / representasi entitas data mahasiswa.

Properti:

NIM (string): Nomor Induk Mahasiswa.

Nama (string): Nama lengkap mahasiswa.

Prodi (string): Program studi mahasiswa.

IPK (double): Nilai indeks prestasi kumulatif (skala 0.00 – 4.00).

Constructor: Menginisialisasi nilai properti saat objek baru dibuat menggunakan kata kunci new Mahasiswa(...).

List<Mahasiswa> daftarMahasiswa

Struktur data dinamis bawaan System.Collections.Generic untuk menampung seluruh objek mahasiswa di dalam memori saat program berjalan.

## Menu
Menu Utama & Perulangan (Main)

Menggunakan perulangan do-while agar menu interaktif terus muncul sampai pengguna memilih opsi 5 (Keluar).

int.TryParse: Digunakan untuk memvalidasi input menu agar program tidak crash jika pengguna memasukkan selain angka.

switch-case: Mengarahkan eksekusi ke metode spesifik berdasarkan nomor menu yang dipilih.

Fungsi Tambah Data (TambahMahasiswa)

Mengambil input string (NIM, Nama, Prodi) via Console.ReadLine().

Validasi IPK: Menggunakan perulangan while(true) dan double.TryParse untuk memastikan pengguna hanya bisa lanjut jika menginput angka valid dalam rentang 0.0 <= IPK <= 4.0.

Data yang sudah valid dimasukkan ke list via daftarMahasiswa.Add().

Fungsi Tampil Data (TampilkanMahasiswa)

Memeriksa apakah list kosong (daftarMahasiswa.Count == 0).

Jika ada data, menampilkannya dalam bentuk tabel rapi menggunakan fitur format string console ({0,-15} {1,-25}...). Format :F2 digunakan untuk membatasi tampilan IPK 2 angka di belakang koma.

Fungsi Cari Data (CariMahasiswa)

Menggunakan perulangan foreach untuk menelusuri data dalam list.

Menggunakan perbandingan string StringComparison.OrdinalIgnoreCase pada NIM agar pencarian bersifat case-insensitive (tidak sensitif huruf besar/kecil).

Jika ditemukan, detail mahasiswa akan dicetak ke layar.

Fungsi Hapus Data (HapusMahasiswa)

Mirip dengan mekanisme pencarian, program mencocokkan input NIM target dengan data di list.

Jika objek ditemukan, method daftarMahasiswa.Remove(mahasiswaDitemukan) dipanggil untuk menghapus objek tersebut dari memori.

```
using System;
using System.Collections.Generic;

namespace DataMahasiswa
{
    // Class untuk merepresentasikan data mahasiswa
    class Mahasiswa
    {
        public string NIM { get; set; }
        public string Nama { get; set; }
        public string Prodi { get; set; }
        public double IPK { get; set; }

        // Constructor
        public Mahasiswa(string nim, string nama, string prodi, double ipk)
        {
            NIM = nim;
            Nama = nama;
            Prodi = prodi;
            IPK = ipk;
        }
    }

    class Program
    {
        // List untuk menyimpan data mahasiswa
        static List<Mahasiswa> daftarMahasiswa = new List<Mahasiswa>();

        static void Main(string[] args)
        {
            int pilihan;
            do
            {
                TampilkanMenu();
                Console.Write("Pilihan: ");
                string input = Console.ReadLine();

                if (!int.TryParse(input, out pilihan))
                {
                    pilihan = 0;
                }

                Console.WriteLine();

                switch (pilihan)
                {
                    case 1:
                        TambahMahasiswa();
                        break;
                    case 2:
                        TampilkanMahasiswa();
                        break;
                    case 3:
                        CariMahasiswa();
                        break;
                    case 4:
                        HapusMahasiswa();
                        break;
                    case 5:
                        Console.WriteLine("Terima kasih telah menggunakan program.");
                        break;
                    default:
                        Console.WriteLine("Pilihan tidak tersedia!");
                        break;
                }

                if (pilihan != 5)
                {
                    Console.WriteLine();
                    Console.WriteLine("Tekan ENTER untuk melanjutkan...");
                    Console.ReadLine();
                }

            } while (pilihan != 5);
        }

        // METHOD MENAMPILKAN MENU
        static void TampilkanMenu()
        {
            Console.Clear();
            Console.WriteLine("========================================");
            Console.WriteLine("        SISTEM DATA MAHASISWA           ");
            Console.WriteLine("========================================");
            Console.WriteLine("1. Tambah Mahasiswa");
            Console.WriteLine("2. Tampilkan Mahasiswa");
            Console.WriteLine("3. Cari Mahasiswa");
            Console.WriteLine("4. Hapus Mahasiswa");
            Console.WriteLine("5. Keluar");
            Console.WriteLine("========================================");
        }

        // METHOD TAMBAH MAHASISWA
        static void TambahMahasiswa()
        {
            Console.Clear();
            Console.WriteLine("========================================");
            Console.WriteLine("            TAMBAH MAHASISWA            ");
            Console.WriteLine("========================================");

            Console.Write("NIM           : ");
            string nim = Console.ReadLine();

            Console.Write("Nama          : ");
            string nama = Console.ReadLine();

            Console.Write("Program Studi : ");
            string prodi = Console.ReadLine();

            double ipk;
            while (true)
            {
                Console.Write("IPK (0 - 4)   : ");
                if (double.TryParse(Console.ReadLine(), out ipk))
                {
                    if (ipk >= 0 && ipk <= 4)
                    {
                        break;
                    }
                }
                Console.WriteLine("IPK harus berupa angka antara 0.0 - 4.0.");
            }

            Mahasiswa mahasiswa = new Mahasiswa(nim, nama, prodi, ipk);
            daftarMahasiswa.Add(mahasiswa);

            Console.WriteLine();
            Console.WriteLine("Data mahasiswa berhasil ditambahkan.");
        }

        // METHOD MENAMPILKAN DATA
        static void TampilkanMahasiswa()
        {
            Console.Clear();
            Console.WriteLine("==================================================================");
            Console.WriteLine("                         DAFTAR MAHASISWA                         ");
            Console.WriteLine("==================================================================");

            if (daftarMahasiswa.Count == 0)
            {
                Console.WriteLine("Belum ada data mahasiswa.");
                return;
            }

            Console.WriteLine("{0,-15} {1,-25} {2,-20} {3,5}", "NIM", "Nama", "Prodi", "IPK");
            Console.WriteLine("------------------------------------------------------------------");

            foreach (Mahasiswa m in daftarMahasiswa)
            {
                Console.WriteLine("{0,-15} {1,-25} {2,-20} {3,5:F2}", m.NIM, m.Nama, m.Prodi, m.IPK);
            }
        }

        // METHOD MENCARI MAHASISWA
        static void CariMahasiswa()
        {
            Console.Clear();
            Console.WriteLine("========================================");
            Console.WriteLine("             CARI MAHASISWA             ");
            Console.WriteLine("========================================");

            Console.Write("Masukkan NIM: ");
            string nimCari = Console.ReadLine();

            Mahasiswa mahasiswaDitemukan = null;
            foreach (Mahasiswa m in daftarMahasiswa)
            {
                if (m.NIM.Equals(nimCari, StringComparison.OrdinalIgnoreCase))
                {
                    mahasiswaDitemukan = m;
                    break;
                }
            }

            Console.WriteLine();
            if (mahasiswaDitemukan != null)
            {
                Console.WriteLine("Data ditemukan!");
                Console.WriteLine("NIM   : " + mahasiswaDitemukan.NIM);
                Console.WriteLine("Nama  : " + mahasiswaDitemukan.Nama);
                Console.WriteLine("Prodi : " + mahasiswaDitemukan.Prodi);
                Console.WriteLine("IPK   : " + mahasiswaDitemukan.IPK.ToString("F2"));
            }
            else
            {
                Console.WriteLine("Mahasiswa dengan NIM tersebut tidak ditemukan.");
            }
        }

        // METHOD MENGHAPUS MAHASISWA
        static void HapusMahasiswa()
        {
            Console.Clear();
            Console.WriteLine("========================================");
            Console.WriteLine("            HAPUS MAHASISWA             ");
            Console.WriteLine("========================================");

            Console.Write("Masukkan NIM: ");
            string nimHapus = Console.ReadLine();

            Mahasiswa mahasiswaDitemukan = null;
            foreach (Mahasiswa m in daftarMahasiswa)
            {
                if (m.NIM.Equals(nimHapus, StringComparison.OrdinalIgnoreCase))
                {
                    mahasiswaDitemukan = m;
                    break;
                }
            }

            if (mahasiswaDitemukan != null)
            {
                daftarMahasiswa.Remove(mahasiswaDitemukan);
                Console.WriteLine();
                Console.WriteLine("Data mahasiswa berhasil dihapus.");
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine("Data mahasiswa tidak ditemukan.");
            }
        }
    }
}
```

## Kesimpulan
Program berhasil menerapkan pilar dasar OOP seperti pembentukan kelas (Class), enkapsulasi properti, dan konstruktor (Constructor).

Penggunaan List<T> memudahkan pengelolaan data dinamis tanpa perlu membatasi jumlah data di awal seperti pada array konvensional.

Penambahan validasi tipe data (TryParse) dan rentang nilai memastikan integritas data serta mencegah terjadinya error saat runtime (unhandled exception).
