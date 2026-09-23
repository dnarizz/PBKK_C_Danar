```
Danar Rizkia Yuda
5025241128
PBKK C
Membuat Aplikasi Kalkulator
```

## 1. Deskripsi Program

Program ini adalah aplikasi kalkulator desktop sederhana yang dibangun menggunakan C# dengan framework Windows Forms pada .NET 8. Aplikasi mampu melakukan empat operasi aritmatika dasar yaitu penjumlahan, pengurangan, perkalian, dan pembagian, serta mendukung input bilangan desimal dan validasi kesalahan seperti pembagian dengan nol.




## 2. Struktur File

Program terdiri dari empat file utama:

Program.cs — titik masuk aplikasi, menjalankan Form1
Form1.cs — berisi logika kalkulator dan event handler
Form1.Designer.cs — berisi deklarasi dan konfigurasi seluruh kontrol UI
CalculatorApp.csproj — konfigurasi project dan target framework

## 3. Penjelasan Kode

### 3.1 Variabel State (Form1.cs)

csharp
double firstNumber = 0;
double secondNumber = 0;
double result = 0;
string operation = "";
bool startNewNumber = true;

Empat variabel utama digunakan untuk menyimpan state kalkulator. firstNumber menyimpan angka pertama yang diinput sebelum operator dipilih. secondNumber menyimpan angka kedua setelah operator dipilih. result menyimpan hasil perhitungan. operation menyimpan operator yang dipilih (+, −, ×, ÷). startNewNumber adalah flag boolean untuk menentukan apakah input berikutnya adalah angka baru atau lanjutan dari angka yang sedang ditampilkan.

### 3.2 Event Handler Tombol Angka

csharp
private void NumberButton_Click(object? sender, EventArgs e)
{
    Button button = (Button)sender!;
    if (startNewNumber || txtDisplay.Text == "0")
    {
        txtDisplay.Text = button.Text;
        startNewNumber = false;
    }
    else
    {
        txtDisplay.Text += button.Text;
    }
}

Satu handler digunakan untuk semua tombol angka 0–9. Parameter sender di-cast ke Button untuk membaca teks tombol yang diklik. Jika startNewNumber bernilai true atau display menampilkan "0", angka baru menggantikan isi display. Jika tidak, angka ditambahkan ke akhir display.

### 3.3 Event Handler Tombol Operator

csharp
private void OperatorButton_Click(object? sender, EventArgs e)
{
    Button button = (Button)sender!;
    firstNumber = double.Parse(txtDisplay.Text, CultureInfo.InvariantCulture);
    operation = button.Text;
    startNewNumber = true;
}

Saat tombol operator diklik, angka yang sedang ditampilkan disimpan ke firstNumber, operator disimpan ke operation, dan flag startNewNumber diset true agar input berikutnya dimulai sebagai angka baru. InvariantCulture digunakan agar parsing tidak terpengaruh locale sistem yang menggunakan koma sebagai pemisah desimal.

### 3.4 Event Handler Tombol Equals

csharp
private void btnEquals_Click(object? sender, EventArgs e)
{
    try
    {
        if (operation == "") return;
        secondNumber = double.Parse(txtDisplay.Text, CultureInfo.InvariantCulture);
        switch (operation)
        {
            case "+": result = firstNumber + secondNumber; break;
            case "\u2212": result = firstNumber - secondNumber; break;
            case "\u00D7": result = firstNumber * secondNumber; break;
            case "\u00F7":
                if (secondNumber == 0)
                    throw new DivideByZeroException("Pembagian dengan nol tidak diperbolehkan.");
                result = firstNumber / secondNumber;
                break;
        }
        txtDisplay.Text = result.ToString(CultureInfo.InvariantCulture);
        firstNumber = result;
        operation = "";
        startNewNumber = true;
    }
    catch (Exception ex)
    {
        MessageBox.Show(ex.Message, "Error");
        ResetState();
    }
}

Saat tombol = diklik, angka kedua diambil dari display, lalu operasi dipilih menggunakan switch berdasarkan nilai operation. Pembagian dengan nol ditangani secara eksplisit dengan melempar DivideByZeroException. Seluruh proses dibungkus try-catch untuk menjaga aplikasi tetap stabil jika terjadi error input. Karakter operator menggunakan Unicode (U+2212 untuk −, U+00D7 untuk ×, U+00F7 untuk ÷) agar konsisten antara teks tombol dan kondisi switch.

### 3.5 Event Handler Clear dan Desimal

csharp
private void btnClear_Click(object? sender, EventArgs e)
{
    ResetState();
}

private void btnDecimal_Click(object? sender, EventArgs e)
{
    if (startNewNumber)
    {
        txtDisplay.Text = "0.";
        startNewNumber = false;
        return;
    }
    if (!txtDisplay.Text.Contains('.'))
        txtDisplay.Text += ".";
}

private void ResetState()
{
    firstNumber = 0;
    secondNumber = 0;
    result = 0;
    operation = "";
    startNewNumber = true;
    txtDisplay.Text = "0";
}

Tombol C mereset seluruh state ke nilai awal melalui method ResetState(). Tombol desimal menambahkan titik ke display dengan pengecekan agar titik tidak muncul dua kali. Jika input baru dimulai dan langsung menekan desimal, display diisi "0." secara otomatis.

## 4. Hasil Pengujian

Skenario	Input	Expected	Status
Penjumlahan	10 + 20 =	30	✓
Pengurangan	30 − 12 =	18	✓
Perkalian	6 × 7 =	42	✓
Pembagian	100 ÷ 4 =	25	✓
Desimal	2.5 × 4 =	10	✓
Bagi nol	10 ÷ 0 =	Pesan error	✓
Clear	Tekan C	Display = 0	✓

<img width="350" height="492" alt="image" src="https://github.com/user-attachments/assets/7f3effe8-73c2-4294-903a-f47b52b2439b" />
<img width="345" height="487" alt="image" src="https://github.com/user-attachments/assets/d1a8ebfa-1439-46cc-b4d9-bbac461a26bc" />



## 5. Kesimpulan

Aplikasi kalkulator berhasil dibangun menggunakan Windows Forms dengan C#. Program menerapkan konsep event-driven programming melalui event handler, validasi input dengan try-catch, dan penggunaan switch untuk seleksi operasi. Penambahan InvariantCulture dan flag startNewNumber dilakukan sebagai perbaikan terhadap potensi bug pada modul asli terkait locale sistem dan kondisi edge case input.
