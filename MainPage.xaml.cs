namespace CurrencyApp
{
    public partial class MainPage : ContentPage
    {
        private readonly ExchangeRates _service = new ExchangeRates();
        private readonly List<ConversionRecords> _history = new();
        private readonly List<string> _historyStrings = new();

        public MainPage()
        {
            InitializeComponent();
            LoadCurrenciesAsync();
        }

        private async void LoadCurrenciesAsync()
        {
            try
            {
                lblStatus.Text = "Fetching live rates...";
                var currencies = await _service.GetAvailableCurrenciesAsync();
                cmbFrom.ItemsSource = currencies;
                cmbTo.ItemsSource = currencies;
                cmbFrom.SelectedItem = "USD";
                cmbTo.SelectedItem = "TTD";
                lblStatus.Text = $"{currencies.Count} currencies loaded.";
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Failed to load rates. Check connection.";
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private async void BtnConvert_Click(object sender, EventArgs e)
        {
            if (!double.TryParse(txtAmount.Text, out double amount) || amount <= 0)
            {
                await DisplayAlert("Invalid Input", "Enter a valid positive amount.", "OK");
                return;
            }
            if (cmbFrom.SelectedItem == null || cmbTo.SelectedItem == null)
            {
                await DisplayAlert("Missing Selection", "Please select both currencies.", "OK");
                return;
            }

            string from = cmbFrom.SelectedItem.ToString();
            string to = cmbTo.SelectedItem.ToString();
            lblStatus.Text = "Converting...";

            try
            {
                double result = await _service.ConvertAsync(amount, from, to);
                double rate = result / amount;

                lblResult.Text = $"{amount:N2} {from} = {result:N4} {to}";
                lblRate.Text = $"Rate: 1 {from} = {rate:N4} {to}";
                resultFrame.IsVisible = true;
                resultFrame.BackgroundColor = Color.FromArgb("#DCEDC8");

                var record = new ConversionRecords
                {
                    Timestamp = DateTime.Now,
                    AmountFrom = amount,
                    CurrencyFrom = from,
                    AmountTo = result,
                    CurrencyTo = to,
                    Rate = rate
                };
                _history.Insert(0, record);
                _historyStrings.Insert(0, record.ToString());
                lstHistory.ItemsSource = null;
                lstHistory.ItemsSource = _historyStrings;

                lblStatus.Text = $"Done. Rate: 1 {from} = {rate:N4} {to}";
            }
            catch (Exception ex)
            {
                resultFrame.IsVisible = true;
                resultFrame.BackgroundColor = Color.FromArgb("#FFEBEE");
                lblResult.Text = "Conversion failed.";
                lblStatus.Text = $"Error: {ex.Message}";
            }
        }

        private void BtnSwap_Click(object sender, EventArgs e)
        {
            var temp = cmbFrom.SelectedItem;
            cmbFrom.SelectedItem = cmbTo.SelectedItem;
            cmbTo.SelectedItem = temp;
        }
    }
}
