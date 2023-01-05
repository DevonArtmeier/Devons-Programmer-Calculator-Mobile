using System.Linq.Expressions;
using System.Text;

namespace ProgrammerCalcMAUI;

public partial class MainPage : ContentPage
{
    private long Results;
    private string Error = "";
    private long ResultsNumBase = 10;
    private int ResultsSize = 64;
    private bool ResultsUnsigned;

	public MainPage()
	{
		InitializeComponent();
    }

    private void DisplayResults()
    {
        if (txtResults == null)
            return;

        if (Error.Length > 0)
        {
            // Error
            txtResults.Text = Error;
            return;
        }

        // Get base number to display
        long msb = 1L << (ResultsSize - 1);
        long mask = (ResultsUnsigned ? msb : 0) + (msb - 1);
        long baseNum = Results & mask;
        bool negative = false;
        if (!ResultsUnsigned && (Results & msb) != 0)
        {
            negative = true;
            baseNum = -baseNum & mask;
            if (baseNum == 0)
                baseNum = msb;
        }

        // Convert number to string
        StringBuilder stringBuilder = new StringBuilder();
        while (baseNum != 0)
        {
            stringBuilder.Insert(0, ((ulong)baseNum % (ulong)ResultsNumBase).ToString("X1"));
            baseNum = (long)((ulong)baseNum / (ulong)ResultsNumBase);
        }
        if (stringBuilder.Length == 0)
            stringBuilder.Append("0");

        // Add prefix
        switch (ResultsNumBase)
        {
            case 2:
                // Binary
                stringBuilder.Insert(0, "0b");
                break;

            case 8:
                // Octal
                stringBuilder.Insert(0, "0o");
                break;

            case 16:
                // Hexadecimal
                stringBuilder.Insert(0, "0x");
                break;
        }
        if (negative)
        {
            // Negative sign
            stringBuilder.Insert(0, "-");
        }

        txtResults.Text = stringBuilder.ToString();
    }

    private void InsertButtonText(Button button, string additional)
    {
        if (button != null)
        {
            string text = (button.Text + additional).Replace("&&", "&");
            int selection = txtExpression.CursorPosition;

            StringBuilder stringBuilder = new StringBuilder(txtExpression.Text);
            if (txtExpression.SelectionLength > 0)
                stringBuilder.Remove(selection, txtExpression.SelectionLength);
            stringBuilder.Insert(selection, text);

            txtExpression.Text = stringBuilder.ToString();
            txtExpression.CursorPosition = selection + text.Length;
        }
    }

    private void Button_Insert(object sender, EventArgs e)
    {
        InsertButtonText((Button)sender, "");
    }

    private void Button_InsertFunc(object sender, EventArgs e)
    {
        InsertButtonText((Button)sender, "(");
    }

    private void Button_Equals(object sender, EventArgs e)
    {
        long results;
        Calculator.Calculate(txtExpression.Text, out results, out Error);
        Results = results;
        DisplayResults();
    }

    private void Button_Clear(object sender, EventArgs e)
    {
        txtExpression.Text = "";
        txtResults.Text = "";
    }

    private void Button_Backspace(object sender, EventArgs e)
    {
        int selection = txtExpression.CursorPosition;

        StringBuilder stringBuilder = new StringBuilder(txtExpression.Text);
        if (txtExpression.SelectionLength > 0)
            stringBuilder.Remove(selection, txtExpression.SelectionLength);
        else if (selection > 0)
            stringBuilder.Remove(--selection, 1);

        txtExpression.Text = stringBuilder.ToString();
        txtExpression.CursorPosition = selection;
    }

    private void RadioBase_CheckedChanged(object sender, EventArgs e)
    {
        ResultsNumBase = int.Parse((string)((RadioButton)sender).Value);
        DisplayResults();
    }

    private void RadioSize_CheckedChanged(object sender, EventArgs e)
    {
        ResultsSize = int.Parse((string)((RadioButton)sender).Value);
        DisplayResults();
    }

    private void RadioSign_CheckedChanged(object sender, EventArgs e)
    {
        ResultsUnsigned = bool.Parse((string)((RadioButton)sender).Value);
        DisplayResults();
    }

    private void Radio_Checked(object sender, EventArgs e)
    {
        // Hack to set multiple default checked flags on load for radio buttons,
        // because just setting IsChecked in the XAML doesn't work
        ((RadioButton)sender).IsChecked = true;
        DisplayResults();
    }
}

