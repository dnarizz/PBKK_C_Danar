namespace CalculatorApp
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        private Label lblTitle;
        private TextBox txtDisplay;
        private Button btn7, btn8, btn9, btnDivide;
        private Button btn4, btn5, btn6, btnMultiply;
        private Button btn1, btn2, btn3, btnMinus;
        private Button btn0, btnDecimal, btnClear, btnPlus;
        private Button btnEquals;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblTitle = new Label();
            this.txtDisplay = new TextBox();
            this.btn7 = new Button();
            this.btn8 = new Button();
            this.btn9 = new Button();
            this.btnDivide = new Button();
            this.btn4 = new Button();
            this.btn5 = new Button();
            this.btn6 = new Button();
            this.btnMultiply = new Button();
            this.btn1 = new Button();
            this.btn2 = new Button();
            this.btn3 = new Button();
            this.btnMinus = new Button();
            this.btn0 = new Button();
            this.btnDecimal = new Button();
            this.btnClear = new Button();
            this.btnPlus = new Button();
            this.btnEquals = new Button();
            this.SuspendLayout();

            // lblTitle
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Text = "Calculator";
            this.lblTitle.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            this.lblTitle.Location = new Point(10, 10);
            this.lblTitle.Size = new Size(305, 35);
            this.lblTitle.TextAlign = ContentAlignment.MiddleCenter;

            // txtDisplay
            this.txtDisplay.Name = "txtDisplay";
            this.txtDisplay.Text = "0";
            this.txtDisplay.Font = new Font("Segoe UI", 20F);
            this.txtDisplay.TextAlign = HorizontalAlignment.Right;
            this.txtDisplay.ReadOnly = true;
            this.txtDisplay.Location = new Point(10, 55);
            this.txtDisplay.Size = new Size(305, 45);

            // baris 1
            this.btn7.Name = "btn7";
            this.btn7.Text = "7";
            this.btn7.Font = new Font("Segoe UI", 14F);
            this.btn7.Location = new Point(10, 115);
            this.btn7.Size = new Size(70, 55);
            this.btn7.Click += NumberButton_Click;

            this.btn8.Name = "btn8";
            this.btn8.Text = "8";
            this.btn8.Font = new Font("Segoe UI", 14F);
            this.btn8.Location = new Point(88, 115);
            this.btn8.Size = new Size(70, 55);
            this.btn8.Click += NumberButton_Click;

            this.btn9.Name = "btn9";
            this.btn9.Text = "9";
            this.btn9.Font = new Font("Segoe UI", 14F);
            this.btn9.Location = new Point(166, 115);
            this.btn9.Size = new Size(70, 55);
            this.btn9.Click += NumberButton_Click;

            this.btnDivide.Name = "btnDivide";
            this.btnDivide.Text = "\u00F7";
            this.btnDivide.Font = new Font("Segoe UI", 14F);
            this.btnDivide.Location = new Point(244, 115);
            this.btnDivide.Size = new Size(70, 55);
            this.btnDivide.Click += OperatorButton_Click;

            // baris 2
            this.btn4.Name = "btn4";
            this.btn4.Text = "4";
            this.btn4.Font = new Font("Segoe UI", 14F);
            this.btn4.Location = new Point(10, 178);
            this.btn4.Size = new Size(70, 55);
            this.btn4.Click += NumberButton_Click;

            this.btn5.Name = "btn5";
            this.btn5.Text = "5";
            this.btn5.Font = new Font("Segoe UI", 14F);
            this.btn5.Location = new Point(88, 178);
            this.btn5.Size = new Size(70, 55);
            this.btn5.Click += NumberButton_Click;

            this.btn6.Name = "btn6";
            this.btn6.Text = "6";
            this.btn6.Font = new Font("Segoe UI", 14F);
            this.btn6.Location = new Point(166, 178);
            this.btn6.Size = new Size(70, 55);
            this.btn6.Click += NumberButton_Click;

            this.btnMultiply.Name = "btnMultiply";
            this.btnMultiply.Text = "\u00D7";
            this.btnMultiply.Font = new Font("Segoe UI", 14F);
            this.btnMultiply.Location = new Point(244, 178);
            this.btnMultiply.Size = new Size(70, 55);
            this.btnMultiply.Click += OperatorButton_Click;

            // baris 3
            this.btn1.Name = "btn1";
            this.btn1.Text = "1";
            this.btn1.Font = new Font("Segoe UI", 14F);
            this.btn1.Location = new Point(10, 241);
            this.btn1.Size = new Size(70, 55);
            this.btn1.Click += NumberButton_Click;

            this.btn2.Name = "btn2";
            this.btn2.Text = "2";
            this.btn2.Font = new Font("Segoe UI", 14F);
            this.btn2.Location = new Point(88, 241);
            this.btn2.Size = new Size(70, 55);
            this.btn2.Click += NumberButton_Click;

            this.btn3.Name = "btn3";
            this.btn3.Text = "3";
            this.btn3.Font = new Font("Segoe UI", 14F);
            this.btn3.Location = new Point(166, 241);
            this.btn3.Size = new Size(70, 55);
            this.btn3.Click += NumberButton_Click;

            this.btnMinus.Name = "btnMinus";
            this.btnMinus.Text = "\u2212";
            this.btnMinus.Font = new Font("Segoe UI", 14F);
            this.btnMinus.Location = new Point(244, 241);
            this.btnMinus.Size = new Size(70, 55);
            this.btnMinus.Click += OperatorButton_Click;

            // baris 4
            this.btn0.Name = "btn0";
            this.btn0.Text = "0";
            this.btn0.Font = new Font("Segoe UI", 14F);
            this.btn0.Location = new Point(10, 304);
            this.btn0.Size = new Size(70, 55);
            this.btn0.Click += NumberButton_Click;

            this.btnDecimal.Name = "btnDecimal";
            this.btnDecimal.Text = ".";
            this.btnDecimal.Font = new Font("Segoe UI", 14F);
            this.btnDecimal.Location = new Point(88, 304);
            this.btnDecimal.Size = new Size(70, 55);
            this.btnDecimal.Click += btnDecimal_Click;

            this.btnClear.Name = "btnClear";
            this.btnClear.Text = "C";
            this.btnClear.Font = new Font("Segoe UI", 14F);
            this.btnClear.Location = new Point(166, 304);
            this.btnClear.Size = new Size(70, 55);
            this.btnClear.Click += btnClear_Click;

            this.btnPlus.Name = "btnPlus";
            this.btnPlus.Text = "+";
            this.btnPlus.Font = new Font("Segoe UI", 14F);
            this.btnPlus.Location = new Point(244, 304);
            this.btnPlus.Size = new Size(70, 55);
            this.btnPlus.Click += OperatorButton_Click;

            // baris 5 — = lebar penuh
            this.btnEquals.Name = "btnEquals";
            this.btnEquals.Text = "=";
            this.btnEquals.Font = new Font("Segoe UI", 14F);
            this.btnEquals.Location = new Point(10, 367);
            this.btnEquals.Size = new Size(304, 55);
            this.btnEquals.Click += btnEquals_Click;

            // tambah semua ke form
            this.Controls.AddRange(new Control[] {
                lblTitle, txtDisplay,
                btn7, btn8, btn9, btnDivide,
                btn4, btn5, btn6, btnMultiply,
                btn1, btn2, btn3, btnMinus,
                btn0, btnDecimal, btnClear, btnPlus,
                btnEquals
            });

            this.ClientSize = new Size(325, 440);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "CalculatorApp";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}