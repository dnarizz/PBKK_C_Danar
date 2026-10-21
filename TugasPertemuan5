# LAPORAN HANDS-ON LAB
## Mini Project: Student Registration App dengan WPF

**Mata Kuliah:** .NET Framework Programming
**Topik:** WPF & GUI
**Nama:** Danar Rizkia Yuda
**NRP:** 5025241128
**Kelas:** PBKK C

---

## 1. Tujuan Praktikum

Praktikum ini bertujuan untuk:
1. Membuat project WPF pada Visual Studio.
2. Memahami struktur project WPF, yaitu pemisahan antara UI (XAML) dan logika (C#).
3. Membuat antarmuka menggunakan XAML.
4. Menggunakan control TextBox, ComboBox, RadioButton, Button, dan ListBox.
5. Menangani event Click dan menghubungkan UI dengan kode C#.
6. Membangun aplikasi desktop sederhana dengan validasi input serta operasi tambah dan hapus data.

## 2. Tema Aplikasi

**Tema yang dipilih:** Student Registration (pendaftaran mahasiswa)

**Deskripsi singkat:**
Aplikasi digunakan untuk mencatat data mahasiswa yang terdiri dari NIM, nama, program studi, dan jenis kelamin. Data yang disimpan ditampilkan pada daftar di sisi kanan form. Aplikasi menyediakan validasi agar data tidak kosong, tombol reset untuk mengosongkan form, dan tombol hapus untuk menghapus data terpilih. Data disimpan sementara di memori dan hilang saat aplikasi ditutup.

## 3. Tools dan Lingkungan Pengembangan

| Komponen | Keterangan |
|---|---|
| IDE | Visual Studio |
| Bahasa | C# |
| Framework | [.NET / .NET Framework versi] |
| Template Project | WPF Application |

## 4. Struktur Project

```
StudentRegistrationApp
├── App.xaml
├── App.xaml.cs
├── MainWindow.xaml
├── MainWindow.xaml.cs
└── Properties
```

| File | Fungsi |
|---|---|
| App.xaml | Konfigurasi aplikasi secara global. |
| App.xaml.cs | Logika tingkat aplikasi (code-behind dari App.xaml). |
| MainWindow.xaml | Mendefinisikan desain antarmuka jendela utama. |
| MainWindow.xaml.cs | Berisi logika C# dan event handler untuk jendela utama. |

Hubungan antar file: XAML mendefinisikan tampilan, sedangkan C# menangani perilaku. Keduanya terhubung melalui atribut `x:Name` (untuk mengakses control dari kode) dan atribut `Click` (untuk mengaitkan event ke method).

## 5. Rancangan Antarmuka (UI)

### 5.1 Daftar Control

| No | Control | x:Name | Fungsi |
|---|---|---|---|
| 1 | TextBlock | - | Menampilkan label dan judul. |
| 2 | TextBox | txtNim | Input NIM mahasiswa. |
| 3 | TextBox | txtNama | Input nama mahasiswa. |
| 4 | ComboBox | cmbProdi | Memilih program studi dari daftar tetap. |
| 5 | RadioButton | rbLaki, rbPerempuan | Memilih jenis kelamin (hanya satu dapat aktif). |
| 6 | Button | - | Tombol Simpan, Reset, dan Hapus. |
| 7 | ListBox | lstMahasiswa | Menampilkan data mahasiswa yang tersimpan. |

Total control berbeda: 6 jenis (TextBlock, TextBox, ComboBox, RadioButton, Button, ListBox), memenuhi syarat minimal 5.

### 5.2 Tata Letak

Jendela dibagi menjadi tiga kolom menggunakan `Grid`: kolom kiri untuk form input, kolom tengah (lebar 20) sebagai jarak, dan kolom kanan untuk daftar data. Di dalam tiap kolom, control disusun vertikal dengan `StackPanel`. RadioButton dan tombol disusun horizontal dengan `StackPanel Orientation="Horizontal"`.

### 5.3 Tampilan Aplikasi

[GAMBAR: tampilan utama aplikasi]

<img width="1237" height="852" alt="image" src="https://github.com/user-attachments/assets/457a551c-3be9-4f17-973e-76da55ba9115" />


### 5.4 Kode XAML

```xml
<!-- Tempelkan kode MainWindow.xaml -->
```

## 6. Implementasi Kode C#

### 6.1 Event yang Digunakan

| No | Event | Method | Fungsi |
|---|---|---|---|
| 1 | Click | BtnSimpan_Click | Memvalidasi input, lalu menambahkan data ke ListBox. |
| 2 | Click | BtnReset_Click | Mengosongkan seluruh isian form. |
| 3 | Click | BtnHapus_Click | Menghapus item terpilih dari ListBox. |

Total: 3 event, memenuhi syarat minimal.

### 6.2 Validasi Input

Sebelum data disimpan, program memeriksa empat kondisi secara berurutan:
- NIM tidak boleh kosong atau hanya spasi (`string.IsNullOrWhiteSpace`).
- Nama tidak boleh kosong atau hanya spasi.
- Program studi harus dipilih (`SelectedItem != null`).
- Salah satu jenis kelamin harus dipilih (`IsChecked`).

1. percobaan NIM tidak boleh kosong
<img width="1237" height="870" alt="image" src="https://github.com/user-attachments/assets/dc68627e-f719-4150-8ee7-8d98418956bf" />
2. percobaan Nama tidak boleh kosong
<img width="1237" height="867" alt="image" src="https://github.com/user-attachments/assets/de6c3c1e-f91b-46d4-9e75-fe3dc5dc378d" />
3. percobaan Prodi tidak boleh kosong
<img width="1247" height="880" alt="image" src="https://github.com/user-attachments/assets/811b5f2b-2b20-41a7-b1a1-d8b61b38320e" />
4. percobaan Gender tidak boleh kosong
<img width="1252" height="875" alt="image" src="https://github.com/user-attachments/assets/f98d1b11-3f18-4fc1-b4b9-989b8db61038" />
5. percobaan Reset Data
<img width="518" height="878" alt="image" src="https://github.com/user-attachments/assets/bccdb797-4aa5-428b-8915-3a0060807521" />
<img width="523" height="862" alt="image" src="https://github.com/user-attachments/assets/4e09da08-d550-4f5c-8cf5-bccaa6e93d2c" />


### 6.3 Operasi Tambah (Simpan)

Setelah validasi lolos, nilai dari tiap control dibaca, digabung menjadi satu string berformat `NIM | Nama | Prodi | Jenis Kelamin`, lalu ditambahkan ke ListBox melalui `lstMahasiswa.Items.Add()`. Pesan konfirmasi ditampilkan menggunakan `MessageBox`.
1. percobaan Semua data lengkap
<img width="1247" height="881" alt="image" src="https://github.com/user-attachments/assets/495abda8-b02b-4a4e-8b83-dd9ae92b3781" />


### 6.4 Operasi Hapus

Program memeriksa apakah ada item terpilih pada ListBox. 
1. percobaan Hapus Data yang dipilih
<img width="1227" height="856" alt="image" src="https://github.com/user-attachments/assets/e0c14512-3070-471a-b39a-865ab9579267" />
<img width="1242" height="872" alt="image" src="https://github.com/user-attachments/assets/9d97fd56-f90a-4745-a3f4-8022f19061ba" />
<img width="1253" height="882" alt="image" src="https://github.com/user-attachments/assets/bc804cd5-22e8-4a1c-b21b-03bb0e37e69f" />
<img width="1255" height="875" alt="image" src="https://github.com/user-attachments/assets/3754f3b5-a9f4-4f94-94cd-280fb7038dbd" />


### 6.5 Uji DATABASE
1. Pembuatan Database di SSMS
<img width="1917" height="720" alt="image" src="https://github.com/user-attachments/assets/7057b322-46d1-443f-8e95-9df489cc60ce" />
2. Menambah data pada aplikasi
<img width="1241" height="871" alt="image" src="https://github.com/user-attachments/assets/36f0ff15-727c-4230-9c72-d9c3cc4a1f50" />
3. Lalu aplikasi diclose dan dibuka ulang, pastikan data tetap tersimpan
