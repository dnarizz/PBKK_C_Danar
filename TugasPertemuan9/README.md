# Laporan Praktikum — Pengenalan Framework CodeIgniter 3 (Latihan 1 & 2)

## Implementasi Arsitektur MVC, Template SB Admin, CRUD, Upload File, SIAKAD, dan Toko Online

| | |
|---|---|
| **Mata Kuliah** | Pemrograman Berbasis Kerangka Kerja (PBKK C) |
| **Topik** | Framework CodeIgniter 3, Arsitektur MVC, Bootstrap SB Admin, CRUD, File Upload, Autentikasi |
| **Nama** | Danar Rizkia Yuda |
| **NRP** | 5025241128 |
| **Tools** | Laragon, Apache, PHP 8.1.10, MySQL 8.0.30, CodeIgniter 3.1.13, Bootstrap 4 (SB Admin 2) |

---

## Tujuan Praktikum

Praktikum ini mencakup dua rangkaian tugas pengenalan framework **CodeIgniter 3** (mengacu pada modul PBKK-2: Latihan 1 dan Latihan 2): membangun aplikasi manajemen produk buah (`tokobuah`) dari nol secara bertahap pada **Latihan 1**, serta mengembangkan dua studi kasus aplikasi web lengkap — **Sistem Informasi Akademik (`sisfo_akademik`)** dan **Aplikasi Toko Online (`toko_online`)** — pada **Latihan 2**.

```
Latihan 1 (7 Tahap)                     Latihan 2 (2 Proyek Lengkap)
┌──────────────────────┐                ┌──────────────────────────────┐
│ tokobuah/            │                │ sisfo_akademik/ (32 video)   │
│  ├─ Welcome Biodata  │                │  └─ 13 Tabel MySQL + Modul   │
│  ├─ Router & About   │                │     Dosen (#25) & KRS/KHS    │
│  ├─ SB Admin Partial │                ├──────────────────────────────┤
│  ├─ CRUD products    │                │ toko_online/ (16 video)      │
│  ├─ File Upload      │                │  └─ Katalog, Cart, Invoice,  │
│  └─ Login Admin      │                │     Trigger Stok, Role Auth  │
└──────────┬───────────┘                └──────────────┬───────────────┘
           │                                           │
           └──────────────► CodeIgniter 3 MVC ◄────────┘
                                   │
                                   ▼
                             MySQL 8.0 (Laragon)
```

Setelah praktikum, mahasiswa mampu:

- Memahami arsitektur **Model-View-Controller (MVC)** dan struktur direktori CodeIgniter 3 (`application/`, `system/`, `assets/`).
- Mengonfigurasi routing URL bersih (`routes.php`, `.htaccess`), autoload library/helper/model, dan koneksi database MySQL (`mysqli`).
- Memisahkan tata letak antarmuka Bootstrap **SB Admin** menjadi *view partials* yang modular (`head`, `navbar`, `sidebar`, `breadcrumb`, `footer`, `modal`, `js`).
- Mengimplementasikan operasi **CRUD** melalui Model dengan validasi `form_validation` serta penanganan **file upload** (`upload` library).
- Mengamankan halaman admin menggunakan **autentikasi session** dan hashing password (`password_verify()`).
- Membangun dan menguji sistem terintegrasi skala menengah (SIAKAD kampus dan e-commerce dengan trigger stok otomatis) pada lingkungan **PHP 8.1 / Laragon**.

---

## Struktur Folder

```
Tugas 9/
├── README.md                              ← laporan lengkap ini
├── setup_laragon.bat                      ← otomasi junction Laragon + impor 3 database
├── image-1.png .. image-15.png            ← bukti screenshot live seluruh aplikasi
│
├── tokobuah/                              ← LATIHAN 1 (Tahap 2-1 s/d 2-7)
│   ├── application/
│   │   ├── config/                        ← autoload, config, constants, database, routes
│   │   ├── controllers/
│   │   │   ├── Welcome.php                ← halaman Welcome Biodata, /about, /contact
│   │   │   └── admin/
│   │   │       ├── Login.php              ← autentikasi & logout admin
│   │   │       ├── Overview.php           ← dashboard statistik, grafik, tabel
│   │   │       └── Products.php           ← controller CRUD & upload foto produk
│   │   ├── models/
│   │   │   ├── Product_model.php          ← aturan validasi, CRUD & _uploadImage()
│   │   │   └── User_model.php             ← verifikasi login & last_login
│   │   └── views/
│   │       ├── _partials/                 ← public_header.php & public_footer.php
│   │       ├── welcome_message.php        ← halaman biodata mahasiswa
│   │       ├── about.php / contact.php
│   │       └── admin/
│   │           ├── _partials/             ← head, navbar, sidebar, breadcrumb, footer, modal, js
│   │           ├── overview.php
│   │           ├── login_page.php
│   │           └── product/               ← list.php, new_form.php, edit_form.php
│   ├── assets/                            ← SB Admin 2, Bootstrap 4, Chart.js, DataTables
│   ├── database/tokobuah.sql              ← tabel products (5 data) & users (admin)
│   └── upload/product/default.jpg         ← folder penyimpanan foto produk
│
├── sisfo_akademik/                        ← LATIHAN 2A (SIAKAD Kampus — 32 Modul)
│   ├── application/
│   │   ├── controllers/
│   │   │   ├── landing_page.php           ← portal publik kampus
│   │   │   └── administrator/             ← auth, dashboard, jurusan, prodi, matakuliah,
│   │   │                                     mahasiswa, dosen (#25), tahun_akademik,
│   │   │                                     krs, nilai, transkrip_nilai, user, dll.
│   │   ├── models/                        ← 15 model (termasuk dosen_model.php)
│   │   └── views/                         ← landing_page, templates_administrator, 30+ view admin
│   ├── assets/uploads/                    ← foto mahasiswa & dosen
│   └── database/sisfo_akademik.sql        ← 13 tabel + data awal lengkap
│
└── toko_online/                           ← LATIHAN 2B (E-Commerce — 16 Modul #20-#35)
    ├── application/
    │   ├── controllers/
    │   │   ├── Welcome.php / dashboard.php ← katalog, pencarian, keranjang, checkout
    │   │   ├── kategori.php / auth.php / registrasi.php
    │   │   └── admin/                     ← dashboard_admin, data_barang, invoice
    │   ├── models/                        ← model_auth, model_barang, model_invoice, model_kategori
    │   └── views/                         ← katalog, detail, keranjang, pembayaran, panel admin
    ├── assets/uploads/                    ← SVG ilustrasi 8 produk awal + default.svg
    └── database/toko_online.sql           ← 4 tabel + trigger pesanan_penjualan
```

---

## Cara Menjalankan

1. Buka aplikasi **Laragon**, lalu klik **Start All** (pastikan **Apache** dan **MySQL** berjalan).
2. Klik dua kali file `setup_laragon.bat` pada folder `Tugas 9/`:
   - Script otomatis membuat *Directory Junction* ke `C:\laragon\www\tokobuah`, `C:\laragon\www\sisfo_akademik`, dan `C:\laragon\www\toko_online`.
   - Script otomatis mengimpor `tokobuah.sql`, `sisfo_akademik.sql`, dan `toko_online.sql` ke MySQL Laragon.
3. Akses masing-masing aplikasi di browser:

| Proyek | URL Utama | URL Login Admin | Kredensial Pengujian |
|---|---|---|---|
| **Latihan 1 — Tokobuah** | `http://localhost/tokobuah` | `http://localhost/tokobuah/admin/login` | `admin` (atau `admin@tokobuah.test`) / `admin123` |
| **Latihan 2A — SIAKAD** | `http://localhost/sisfo_akademik` | `http://localhost/sisfo_akademik/administrator/auth` | `admin` / `admin` *(NIM uji KHS/KRS: `1219001`)* |
| **Latihan 2B — Toko Online** | `http://localhost/toko_online` | `http://localhost/toko_online/auth/login` | Admin: `admin` / `123` \| Customer: `user` / `123` |

---

# LATIHAN 1 — Aplikasi Tokobuah dengan CodeIgniter 3

Latihan 1 mengikuti seluruh 7 sub-modul pada panduan PBKK-2 (`tokobuah`), dari instalasi kerangka MVC hingga pengamanan akses user admin.

## Tahap 2-1 — Instalasi, Konfigurasi CI, dan Welcome Biodata

Konfigurasi inti disesuaikan agar kompatibel dengan **PHP 8.1** serta berjalan fleksibel baik melalui virtual host/subfolder Laragon maupun server lokal:

- **`constants.php`**: mendefinisikan `SITE_NAME` bernilai `'Tokobuah'`.
- **`config.php`**: deteksi otomatis `base_url`, `$config['index_page'] = ''`, dan `$config['sess_cookie_name'] = 'ci_session_tokobuah'` (terisolasi dari proyek lain di `localhost`).
- **`autoload.php`**: memuat library `database`, `session`, `form_validation` serta helper `url`, `form`, `security`.
- **`Welcome.php`**: method `index()` mengirim array profil biodata mahasiswa (**Danar Rizkia Yuda**, NRP **5025241128**, Kelas **PBKK C**, Teknik Informatika ITS) ke `views/welcome_message.php`.

```php
private function _getBiodata()
{
    return array(
        'nama'        => 'Danar Rizkia Yuda',
        'nrp'         => '5025241128',
        'kelas'       => 'PBKK C',
        'prodi'       => 'Teknik Informatika',
        'mata_kuliah' => 'Pemrograman Berbasis Kerangka Kerja (PBKK C)',
        'kampus'      => 'Institut Teknologi Sepuluh Nopember (ITS)',
        'moto'        => 'Belajar CodeIgniter 3 MVC dengan terstruktur dan konsisten.'
    );
}
```

![Latihan 1 (2-1) - Halaman Welcome dan Biodata Mahasiswa](image-1.png)

## Tahap 2-2 — Fungsi Router dan Controller

Routing URI dikonfigurasi pada `tokobuah/application/config/routes.php` agar URL bersih langsung memanggil controller dan method yang sesuai, didukung oleh `.htaccess`:

```php
$route['default_controller'] = 'welcome';
$route['about']   = 'welcome/about';
$route['contact'] = 'welcome/contact';
$route['admin']   = 'admin/overview';
```

| URL / Route | Controller & Method | View yang Dirender | Status |
|---|---|---|---|
| `/` | `Welcome::index()` | `welcome_message.php` | ✅ |
| `/about` & `/welcome/about` | `Welcome::about()` | `about.php` | ✅ |
| `/contact` & `/welcome/contact` | `Welcome::contact()` | `contact.php` | ✅ |
| `/admin` | `admin/Overview::index()` | `admin/overview.php` | ✅ |

![Latihan 1 (2-2) - Halaman About via Router dan Controller](image-2.png)

## Tahap 2-3 & 2-4 — Bootstrap SB Admin dan View Partials

Antarmuka panel admin menggunakan aset **Bootstrap 4 (SB Admin 2)**, **Chart.js**, dan **DataTables** yang dipecah menjadi 8 *view partials* di `tokobuah/application/views/admin/_partials/`:

```
application/views/admin/
├── _partials/
│   ├── head.php         → meta tag, dynamic <title>, FontAwesome, SB Admin & DataTables CSS
│   ├── navbar.php       → topbar hijau bg-success, search bar, notifikasi, dropdown Admin
│   ├── sidebar.php      → sidebar gelap bg-dark (Overview, Products submenu, Users, Logout)
│   ├── breadcrumb.php   → breadcrumb otomatis dari segmen URI (mis. Admin / Products / Add)
│   ├── footer.php       → Copyright © Tokobuah 2026
│   ├── scrolltop.php    → tombol gulir ke atas (#page-top)
│   ├── modal.php        → #logoutModal, #deleteModal, dan #userProfileModal
│   └── js.php           → jQuery, Bootstrap Bundle, Chart.js, DataTables init, sb-admin-2.js
├── overview.php         → 4 kartu ringkasan, Area Chart Example, dan DataTable produk
└── product/             → list.php, new_form.php, edit_form.php
```

Halaman dashboard `admin/Overview` menampilkan 4 kartu metrik utama (**Total Products**, **Total Harga Katalog**, **Rata-rata Harga**, **Registered Users**), grafik area **Chart.js**, dan tabel produk.

![Latihan 1 (2-3 & 2-4) - Dashboard Admin SB Admin dengan View Partials](image-4.png)

## Tahap 2-5 — Model, Database, dan CRUD Produk

Database `tokobuah` (`tokobuah/database/tokobuah.sql`) dirancang sesuai struktur tabel `products` pada modul PBKK-2 beserta 5 data awal buah:

```sql
CREATE TABLE `products` (
  `product_id` varchar(64) NOT NULL,
  `name` varchar(255) NOT NULL,
  `price` int(11) NOT NULL,
  `image` varchar(255) NOT NULL DEFAULT 'default.jpg',
  `description` text NOT NULL,
  PRIMARY KEY (`product_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
```

```
products
├── product_id   → PRIMARY KEY (uniqid string)
├── name         → Nama buah (required)
├── price        → Harga integer Rupiah (required|numeric)
├── image        → Nama file gambar (DEFAULT 'default.jpg')
└── description  → Deskripsi produk (required)
```

`Product_model.php` mendeklarasikan seluruh properti secara eksplisit (`$product_id`, `$name`, `$price`, `$image`, `$description`) agar bebas deprecation warning pada PHP 8.1, serta menyediakan method `rules()`, `getAll()`, `getById($id)`, `save()`, `update()`, dan `delete($id)`.

![Latihan 1 (2-5) - Daftar Produk CRUD pada Tabel products](image-5.png)

## Tahap 2-6 — Penanganan File Upload

Form tambah produk (`admin/products/add`) dan edit produk (`admin/products/edit/<id>`) memakai `enctype="multipart/form-data"`. Method `_uploadImage()` di `Product_model.php` memvalidasi file (`gif|jpg|png|jpeg`, maks 2 MB) dengan nama file mengikuti `$this->product_id` dan menyimpannya ke `tokobuah/upload/product/`. Apabila tidak ada file diunggah, produk otomatis memakai `default.jpg`. Saat produk diperbarui dengan foto baru atau dihapus, `_deleteImage()` membersihkan file gambar lama (kecuali `default.jpg`).

![Latihan 1 (2-6) - Form Tambah Produk dan Pemilihan File Upload](image-6.png)

Pengujian langsung dilakukan dengan menambahkan produk **Durian Montong** (harga `85000`) beserta file foto `durian_upload.jpg`: form berhasil divalidasi, gambar tersimpan ke `upload/product/`, dan notifikasi **"Berhasil disimpan"** tampil bersama baris produk baru di DataTables.

![Latihan 1 (2-6) - Produk Durian Montong Berhasil Tersimpan Beserta Foto](image-7.png)

> Setelah `image-7.png` diambil sebagai bukti, baris uji **Durian Montong** dihapus kembali melalui tombol **Delete** (`/admin/products/delete/<id>`) untuk memverifikasi fungsi hapus beserta pembersihan file foto agar data awal `tokobuah` tetap bersih (5 produk awal).

## Tahap 2-7 — Mengatur Akses User (Login & Proteksi Session)

Autentikasi admin memakai tabel `users` (`user_id`, `username`, `password` bcrypt, `email`, `full_name`, `role`, `last_login`, `is_active`). `User_model::doLogin()` menerima input **email atau username**, mencocokkan password menggunakan `password_verify()`, memeriksa `role === 'admin'`, menyimpan sesi `user_logged`, dan memperbarui kolom `last_login`. Constructor `admin/Overview` dan `admin/Products` memeriksa `$this->user_model->isNotLogin()` dan mengarahkan akses ilegal ke `/admin/login`.

| Pengujian Keamanan & CRUD `tokobuah` | Hasil yang Diharapkan | Hasil Aktual | Status |
|---|---|---|---|
| Akses `/admin` tanpa login | Redirect ke `/admin/login` | Dialihkan ke `/admin/login` | ✅ Berhasil |
| Login dengan `admin` / `admin123` | Masuk ke dashboard & set `last_login` | Redirect ke `/admin`, tampil `Administrator Tokobuah` | ✅ Berhasil |
| Tambah `Durian Montong` + upload `.jpg` | Validasi lolos, file tersimpan, alert tampil | Muncul `"Berhasil disimpan"` & thumbnail foto | ✅ Berhasil |
| Hapus `Durian Montong` | Baris & file gambar terhapus | Produk hilang dari tabel, sisa 5 data awal | ✅ Berhasil |

![Latihan 1 (2-7) - Halaman Login Admin Tokobuah](image-3.png)

### Output yang Dikumpulkan (Latihan 1)

| No | Output | Lokasi |
|---|---|---|
| 1 | Welcome message dengan profil biodata mahasiswa | `tokobuah/application/controllers/Welcome.php` & `image-1.png` |
| 2 | Router & Controller (`/about` dan `/contact`) | `tokobuah/application/config/routes.php` & `image-2.png` |
| 3 | Template Bootstrap SB Admin + 8 View Partials | `tokobuah/application/views/admin/_partials/` & `image-4.png` |
| 4 | Skema SQL tabel `products` & `users` + CRUD Model | `tokobuah/database/tokobuah.sql`, `Product_model.php` & `image-5.png` |
| 5 | Form & handler Upload Foto Produk | `tokobuah/application/views/admin/product/new_form.php` & `image-6.png`, `image-7.png` |
| 6 | Autentikasi Login Admin & proteksi session | `User_model.php`, `admin/Login.php` & `image-3.png` |

### Checkpoint Praktikum Latihan 1

- [x] CodeIgniter 3.1.13 terkonfigurasi dengan clean URL dan kompatibel PHP 8.1.
- [x] Halaman Welcome menampilkan biodata Danar Rizkia Yuda (tanpa alamat email).
- [x] Custom routing `/about`, `/contact`, dan `/admin` berfungsi.
- [x] Layout admin SB Admin dipecah menjadi 8 partial (`head`, `navbar`, `sidebar`, `breadcrumb`, `footer`, `scrolltop`, `modal`, `js`).
- [x] Database `tokobuah` dan tabel `products` serta `users` berhasil diimpor.
- [x] CRUD produk (Create, Read, Update, Delete) dan upload foto berjalan normal.
- [x] Halaman admin terlindungi autentikasi session (`admin/login`).

---

# LATIHAN 2 — Proyek Aplikasi Web Lengkap dengan CodeIgniter 3

Latihan 2 mengimplementasikan dua playlist studi kasus lengkap dari kanal *Framework Indonesia*: **Sistem Informasi Akademik (`sisfo_akademik`)** dan **Toko Online (`toko_online`)**.

## Bagian 2A — Sistem Informasi Akademik (`sisfo_akademik`)

Aplikasi `sisfo_akademik` terdiri atas portal publik kampus (`landing_page`) dan panel administrator akademik dengan **13 tabel MySQL** (`sisfo_akademik/database/sisfo_akademik.sql`):

```
sisfo_akademik (13 Tabel)
├── Akademik Master : jurusan, prodi, matakuliah, mahasiswa (4), dosen (3), tahun_akademik
├── Transaksi Nilai : krs (5) ──► transkrip_nilai (3)
└── Portal & Akses  : user (2), identitas, tentang_kampus, informasi, hubungi
```

| Kelompok Modul (Playlist #01–#32) | Controller Utama | Fitur yang Diimplementasikan | Status |
|---|---|---|---|
| **#01–#03** Setup, Login/Logout, Dashboard | `administrator/auth`, `dashboard` | Login admin MD5 + session guard + Control Panel modal | ✅ |
| **#04–#09** Jurusan, Prodi, Mata Kuliah | `jurusan`, `prodi`, `matakuliah` | CRUD lengkap + detail mata kuliah & SKS | ✅ |
| **#10–#12** Halaman Mahasiswa | `administrator/mahasiswa` | CRUD + detail biodata + upload foto mahasiswa | ✅ |
| **#13–#14** Tahun Akademik | `administrator/tahun_akademik` | Pengelolaan tahun akademik & semester aktif | ✅ |
| **#15–#24** KRS, Input Nilai, KHS, Transkrip | `krs`, `nilai`, `transkrip_nilai` | Pengambilan KRS, input nilai, hitung IP/IPK, cetak transkrip | ✅ |
| **#25** Halaman Dosen | `administrator/dosen` | CRUD data dosen (NIDN, alamat, kontak, upload foto) | ✅ |
| **#26–#32** Users & Info Kampus | `user`, `identitas`, `tentang_kampus`, `informasi`, `hubungi_kami` | Manajemen user, profil kampus, berita, pesan pengunjung | ✅ |

![Latihan 2A - Landing Page Portal Akademik Universitas Teknologi Nusantara](image-8.png)

![Latihan 2A - Dashboard Administrator SIAKAD dan Menu Akademik](image-9.png)

### Implementasi Modul Dosen (`#25`)

Untuk melengkapi seluruh 32 bagian playlist, modul **Dosen** ditambahkan secara utuh:

- **Tabel `dosen`** di `sisfo_akademik.sql` (`id_dosen`, `nidn`, `nama_dosen`, `alamat`, `jenis_kelamin`, `email`, `telp`, `photo`) dengan 3 data awal dosen.
- **Model `dosen_model.php`** (didaftarkan pada `autoload.php`) dan **Controller `administrator/dosen.php`** (proteksi session admin, validasi form, upload foto ke `assets/uploads/` via `_uploadPhoto()`, serta CRUD lengkap).
- **4 View**: `dosen.php`, `dosen_form.php`, `dosen_update.php`, `dosen_detail.php`, serta tautan navigasi pada `sidebar.php` dan Control Panel `dashboard.php`.

```sql
CREATE TABLE `dosen` (
  `id_dosen` int(11) NOT NULL AUTO_INCREMENT,
  `nidn` varchar(50) NOT NULL,
  `nama_dosen` varchar(100) NOT NULL,
  `alamat` varchar(255) NOT NULL,
  `jenis_kelamin` varchar(20) NOT NULL,
  `email` varchar(100) NOT NULL,
  `telp` varchar(20) NOT NULL,
  `photo` varchar(255) NOT NULL DEFAULT 'default.jpg',
  PRIMARY KEY (`id_dosen`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
```

![Latihan 2A - Halaman Pengelolaan Data Dosen (Modul #25)](image-10.png)

### Pengujian KHS (NIM `1219001` — Ahmad Fauzi)

Pengujian modul **KHS** (`administrator/nilai`) dengan NIM `1219001` pada Tahun Akademik `2025/2026 (Ganjil)` menampilkan 3 mata kuliah (`MK001` Pemrograman Berbasis Kerangka Kerja, `MK002` Basis Data Lanjut, `MK003` Algoritma dan Struktur Data), total **10 SKS**, total skor **37**, dan **Index Prestasi: 3.70**.

![Latihan 2A - Kartu Hasil Studi Mahasiswa NIM 1219001](image-11.png)

---

## Bagian 2B — Aplikasi Toko Online (`toko_online`)

Aplikasi `toko_online` mencakup seluruh 16 video (`#20`–`#35`) dengan pemisahan peran **Customer** (`role_id = 2`) dan **Admin** (`role_id = 1`), lengkap dengan **4 tabel MySQL** dan 1 trigger database:

```
toko_online (4 Tabel + 1 Trigger)
├── tb_barang (8 produk)      ◄──── TRIGGER `pesanan_penjualan` (stok - NEW.jumlah)
├── tb_user   (admin & user)              ▲
└── tb_invoice (1) ── 1:N ──► tb_pesanan (2)
```

```sql
CREATE TRIGGER `pesanan_penjualan` AFTER INSERT ON `tb_pesanan` FOR EACH ROW BEGIN
    UPDATE tb_barang SET stok = stok - NEW.jumlah
    WHERE id_brg = NEW.id_brg;
END
```

| Bagian Playlist (#20–#35) | Controller / View | Fitur | Status |
|---|---|---|---|
| **#20–#21, #30–#31** Katalog, Detail, Kategori, Cari | `Welcome`, `dashboard`, `kategori` | Carousel promo, 8 produk awal, filter 5 kategori, pencarian | ✅ |
| **#22–#24, #32** Panel Admin Barang & Stok | `admin/dashboard_admin`, `admin/data_barang` | Dashboard ringkasan, CRUD barang + upload gambar, kelola stok | ✅ |
| **#25–#27** Keranjang Belanja & Checkout | `dashboard` (`cart` library) | Tambah item ke keranjang, detail sub-total, hapus, form bayar | ✅ |
| **#28–#29** Invoice & Detail Pesanan | `admin/invoice`, `model_invoice` | Pencatatan `tb_invoice` + `tb_pesanan` & rincian tagihan | ✅ |
| **#33–#35** Login, Registrasi, Logout | `auth`, `registrasi` | Registrasi customer baru (`role_id = 2`), redirect sesuai role | ✅ |

> Selama verifikasi visual `toko_online`, diperbaiki penulisan URL gambar produk pada 8 file view (`dashboard.php`, `detail_barang.php`, `elektronik.php`, `pakaian_anak_anak.php`, `pakaian_pria.php`, `pakaian_wanita.php`, `peralatan_olahraga.php`, dan `admin/detail_barang.php`) dari `base_url() . '/assets/uploads/'` menjadi `base_url('assets/uploads/' . ...)` agar tidak membentuk double-slash `//assets/`, serta isolasi `sess_cookie_name` di ketiga proyek.

![Latihan 2B - Halaman Katalog Produk Toko Online](image-12.png)

![Latihan 2B - Keranjang Belanja Customer (2 Item)](image-13.png)

![Latihan 2B - Panel Admin Kelola Data Barang](image-14.png)

![Latihan 2B - Panel Admin Invoice Penjualan](image-15.png)

---

## Tanggung Jawab Komponen MVC

| Lapisan MVC | Direktori | Tanggung Jawab |
|---|---|---|
| **Config & Routing** | `application/config/` | Menentukan `base_url`, `sess_cookie_name`, koneksi MySQL, autoload, dan pemetaan route URI |
| **Controller** | `application/controllers/` | Menerima request HTTP, mengecek autentikasi session, memvalidasi input, memanggil Model, dan memuat View |
| **Model** | `application/models/` | Menangani query SQL (`Query Builder` / `JOIN`), aturan validasi, proses login, serta upload/hapus file gambar |
| **View & Partials** | `application/views/` | Menyajikan HTML Bootstrap 4 / SB Admin 2 secara modular melalui partial header, navbar, sidebar, modal, dan footer |

## Ringkasan Pengujian & Verifikasi

| Pengujian | Cakupan | Hasil Aktual | Status |
|---|---|---|---|
| **PHP Syntax Lint (`php -l`)** | Seluruh `.php` di `tokobuah`, `sisfo_akademik`, `toko_online` | **218 file PHP** diperiksa, **0 error** | ✅ Berhasil |
| **Impor Database MySQL 8.0** | `tokobuah.sql`, `sisfo_akademik.sql`, `toko_online.sql` | Semua tabel & trigger terbuat (`products`: 5, `users`: 1, `dosen`: 3, `mahasiswa`: 4, `krs`: 5, `tb_barang`: 8, `tb_invoice`: 1) | ✅ Berhasil |
| **Uji Live Browser (Headless Chrome)** | 15 halaman lintas 3 aplikasi (`1366×900`) | Seluruh halaman merender tanpa PHP error, 404, atau gambar rusak (`image-1.png`–`image-15.png`) | ✅ Berhasil |
| **Uji Alur Interaktif End-to-End** | Login admin, proteksi route, tambah + upload produk, hapus uji, lookup KHS `1219001`, keranjang belanja | Seluruh alur transaksi & proteksi akses berjalan sesuai harapan | ✅ Berhasil |

## Checkpoint Latihan 2

**SIAKAD (`sisfo_akademik`):** ✅ 13 Tabel MySQL  ✅ Landing Page & Login  ✅ Modul Akademik (Jurusan, Prodi, Matkul, Mahasiswa, **Dosen #25**)  ✅ KRS, Input Nilai, KHS (IP 3.70), Transkrip  ✅ Pengaturan & Info Kampus

**Toko Online (`toko_online`):** ✅ 4 Tabel + Trigger `pesanan_penjualan`  ✅ Katalog & 5 Kategori (SVG tampil bersih)  ✅ Keranjang Belanja & Checkout  ✅ Admin Data Barang & Invoice  ✅ Login Multi-Role & Registrasi

## Alur Request MVC pada CodeIgniter 3

```
1. Browser mengakses URL aplikasi (mis. /admin/products)
        ↓
2. .htaccess meneruskan request bersih ke index.php
        ↓
3. CodeIgniter Router membaca config/routes.php
        ↓
4. Controller tujuan dieksekusi & memeriksa session login
        ↓
5. Controller memanggil Model untuk query MySQL / validasi / upload
        ↓
6. Model mengembalikan hasil objek/array ke Controller
        ↓
7. Controller memuat View & _partials (navbar, sidebar, konten, footer)
        ↓
8. Response HTML + Bootstrap SB Admin 2 dikirim ke Browser
        ↓
9. Halaman interaktif (DataTables, Chart.js, Modal) tampil ke pengguna
```
