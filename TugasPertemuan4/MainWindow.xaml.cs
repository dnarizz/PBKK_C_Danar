using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Data.SqlClient; // Ganti ke System.Data.SqlClient jika memakai .NET Framework klasik

namespace StudentRegistrationApp
{
    // Model Mahasiswa dengan kolom Id (Level 3)
    public class Mahasiswa
    {
        public int Id { get; set; }
        public string Nim { get; set; }
        public string Nama { get; set; }
        public string Prodi { get; set; }
        public string JenisKelamin { get; set; }
        public DateTime? TanggalLahir { get; set; }
        public string NoTelepon { get; set; }
        public string Alamat { get; set; }
    }

    public partial class MainWindow : Window
    {
        // Connection String menuju SQL Server LocalDB
        private readonly string connectionString = @"Server=(localdb)\MSSQLLocalDB;Database=StudentDb;Trusted_Connection=True;TrustServerCertificate=True;";
        private int selectedMahasiswaId = -1;

        public MainWindow()
        {
            InitializeComponent();
            RefreshTabel();
        }

        #region Database Operations (CRUD)

        // Read & Search Data dari Database (Level 2 & 3)
        private void RefreshTabel(string keyword = "")
        {
            List<Mahasiswa> list = new List<Mahasiswa>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"SELECT Id, Nim, Nama, Prodi, JenisKelamin, TanggalLahir, Alamat, NoTelepon 
                                     FROM Mahasiswa 
                                     WHERE Nim LIKE @Search OR Nama LIKE @Search 
                                     ORDER BY Id DESC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Search", $"%{keyword}%");

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                list.Add(new Mahasiswa
                                {
                                    Id = reader.GetInt32(0),
                                    Nim = reader.GetString(1),
                                    Nama = reader.GetString(2),
                                    Prodi = reader.GetString(3),
                                    JenisKelamin = reader.GetString(4),
                                    TanggalLahir = reader.IsDBNull(5) ? (DateTime?)null : reader.GetDateTime(5),
                                    Alamat = reader.IsDBNull(6) ? "" : reader.GetString(6),
                                    NoTelepon = reader.IsDBNull(7) ? "" : reader.GetString(7)
                                });
                            }
                        }
                    }
                }

                dgMahasiswa.ItemsSource = null;
                dgMahasiswa.ItemsSource = list;

                // Counter Jumlah Mahasiswa (Level 2)
                lblCounter.Text = $"Jumlah Mahasiswa: {list.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Koneksi database gagal: {ex.Message}", "Error Database", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Live Search Mahasiswa (Level 2)
        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            RefreshTabel(txtSearch.Text.Trim());
        }

        // Create: Insert data ke database SQL Server (Level 3)
        private void BtnSimpan_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm()) return;

            string gender = rbLaki.IsChecked == true ? "Laki-laki" : "Perempuan";
            string prodi = (cmbProdi.SelectedItem as ComboBoxItem)?.Content.ToString();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"INSERT INTO Mahasiswa (Nim, Nama, Prodi, JenisKelamin, TanggalLahir, Alamat, NoTelepon) 
                                     VALUES (@Nim, @Nama, @Prodi, @Gender, @TglLahir, @Alamat, @Telepon)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Nim", txtNim.Text.Trim());
                        cmd.Parameters.AddWithValue("@Nama", txtNama.Text.Trim());
                        cmd.Parameters.AddWithValue("@Prodi", prodi);
                        cmd.Parameters.AddWithValue("@Gender", gender);
                        cmd.Parameters.AddWithValue("@TglLahir", (object)dpTanggalLahir.SelectedDate ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Alamat", string.IsNullOrWhiteSpace(txtAlamat.Text) ? (object)DBNull.Value : txtAlamat.Text.Trim());
                        cmd.Parameters.AddWithValue("@Telepon", string.IsNullOrWhiteSpace(txtTelepon.Text) ? (object)DBNull.Value : txtTelepon.Text.Trim());

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Data mahasiswa berhasil disimpan ke SQL Server!", "Informasi", MessageBoxButton.OK, MessageBoxImage.Information);
                ResetForm();
                RefreshTabel();
            }
            catch (SqlException ex)
            {
                // Error Code 2627 / 2601 menandakan pelanggaran Unique Key pada kolom NIM
                if (ex.Number == 2627 || ex.Number == 2601)
                {
                    MessageBox.Show("NIM sudah terdaftar di database! Gunakan NIM lain.", "Peringatan", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    MessageBox.Show($"Gagal menyimpan ke database: {ex.Message}", "Error SQL", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // Update: Memperbarui data yang ada di database (Level 2 & 3)
        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (selectedMahasiswaId == -1 || !ValidateForm()) return;

            string gender = rbLaki.IsChecked == true ? "Laki-laki" : "Perempuan";
            string prodi = (cmbProdi.SelectedItem as ComboBoxItem)?.Content.ToString();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"UPDATE Mahasiswa 
                                     SET Nim = @Nim, Nama = @Nama, Prodi = @Prodi, JenisKelamin = @Gender, 
                                         TanggalLahir = @TglLahir, Alamat = @Alamat, NoTelepon = @Telepon 
                                     WHERE Id = @Id";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", selectedMahasiswaId);
                        cmd.Parameters.AddWithValue("@Nim", txtNim.Text.Trim());
                        cmd.Parameters.AddWithValue("@Nama", txtNama.Text.Trim());
                        cmd.Parameters.AddWithValue("@Prodi", prodi);
                        cmd.Parameters.AddWithValue("@Gender", gender);
                        cmd.Parameters.AddWithValue("@TglLahir", (object)dpTanggalLahir.SelectedDate ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Alamat", string.IsNullOrWhiteSpace(txtAlamat.Text) ? (object)DBNull.Value : txtAlamat.Text.Trim());
                        cmd.Parameters.AddWithValue("@Telepon", string.IsNullOrWhiteSpace(txtTelepon.Text) ? (object)DBNull.Value : txtTelepon.Text.Trim());

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Data mahasiswa berhasil diperbarui di database!", "Informasi", MessageBoxButton.OK, MessageBoxImage.Information);
                ResetForm();
                RefreshTabel();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Gagal memperbarui data: {ex.Message}", "Error SQL", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Delete: Menghapus data dari database dengan konfirmasi (Level 2 & 3)
        private void BtnHapus_Click(object sender, RoutedEventArgs e)
        {
            if (dgMahasiswa.SelectedItem is not Mahasiswa mhs)
            {
                MessageBox.Show("Pilih baris mahasiswa yang ingin dihapus terlebih dahulu!", "Peringatan", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MessageBoxResult result = MessageBox.Show(
                $"Apakah Anda yakin ingin menghapus data {mhs.Nama} ({mhs.Nim}) dari database?",
                "Konfirmasi Hapus",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        string query = "DELETE FROM Mahasiswa WHERE Id = @Id";

                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@Id", mhs.Id);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    MessageBox.Show("Data berhasil dihapus dari database!", "Informasi", MessageBoxButton.OK, MessageBoxImage.Information);
                    ResetForm();
                    RefreshTabel();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Gagal menghapus data: {ex.Message}", "Error SQL", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        #endregion

        #region Helper & UI Form Events

        private void DgMahasiswa_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgMahasiswa.SelectedItem is Mahasiswa mhs)
            {
                selectedMahasiswaId = mhs.Id;

                txtNim.Text = mhs.Nim;
                txtNama.Text = mhs.Nama;

                foreach (ComboBoxItem item in cmbProdi.Items)
                {
                    if (item.Content.ToString() == mhs.Prodi)
                    {
                        cmbProdi.SelectedItem = item;
                        break;
                    }
                }

                if (mhs.JenisKelamin == "Laki-laki")
                    rbLaki.IsChecked = true;
                else if (mhs.JenisKelamin == "Perempuan")
                    rbPerempuan.IsChecked = true;

                dpTanggalLahir.SelectedDate = mhs.TanggalLahir;
                txtTelepon.Text = mhs.NoTelepon;
                txtAlamat.Text = mhs.Alamat;

                btnSimpan.IsEnabled = false;
                btnUpdate.IsEnabled = true;
            }
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            ResetForm();
        }

        private void ResetForm()
        {
            selectedMahasiswaId = -1;
            txtNim.Clear();
            txtNama.Clear();
            cmbProdi.SelectedIndex = -1;
            rbLaki.IsChecked = false;
            rbPerempuan.IsChecked = false;
            dpTanggalLahir.SelectedDate = null;
            txtTelepon.Clear();
            txtAlamat.Clear();

            btnSimpan.IsEnabled = true;
            btnUpdate.IsEnabled = false;
            dgMahasiswa.SelectedItem = null;
            txtNim.Focus();
        }

        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(txtNim.Text))
            {
                MessageBox.Show("NIM wajib diisi!", "Validasi", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtNim.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtNama.Text))
            {
                MessageBox.Show("Nama Mahasiswa wajib diisi!", "Validasi", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtNama.Focus();
                return false;
            }

            if (cmbProdi.SelectedItem == null)
            {
                MessageBox.Show("Pilih salah satu program studi!", "Validasi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (rbLaki.IsChecked != true && rbPerempuan.IsChecked != true)
            {
                MessageBox.Show("Pilih jenis kelamin!", "Validasi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        #endregion
    }
}