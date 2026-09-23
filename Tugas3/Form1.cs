using System.Globalization;

namespace CalculatorApp
{
    public partial class Form1 : Form
    {
        double firstNumber = 0;
        double secondNumber = 0;
        double result = 0;
        string operation = "";
        bool startNewNumber = true;

        public Form1()
        {
            InitializeComponent();
        }

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

        private void OperatorButton_Click(object? sender, EventArgs e)
        {
            Button button = (Button)sender!;
            firstNumber = double.Parse(txtDisplay.Text, CultureInfo.InvariantCulture);
            operation = button.Text;
            startNewNumber = true;
        }

        private void btnEquals_Click(object? sender, EventArgs e)
        {
            try
            {
                if (operation == "") return;

                secondNumber = double.Parse(txtDisplay.Text, CultureInfo.InvariantCulture);

                switch (operation)
                {
                    case "+":
                        result = firstNumber + secondNumber;
                        break;
                    case "\u2212":
                        result = firstNumber - secondNumber;
                        break;
                    case "\u00D7":
                        result = firstNumber * secondNumber;
                        break;
                    case "\u00F7":
                        if (secondNumber == 0)
                            throw new DivideByZeroException("Pembagian dengan nol tidak diperbolehkan.");
                        result = firstNumber / secondNumber;
                        break;
                    default:
                        return;
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
    }
}