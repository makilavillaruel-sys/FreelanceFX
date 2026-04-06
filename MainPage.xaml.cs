using Microsoft.Maui.Controls;

namespace CurrencyApp
{
    public class MainPage : ContentPage
    {
        private readonly ExchangeRates _service = new ExchangeRates();
        private readonly List<ConversionRecords> _history = new();
        private readonly List<string> _historyStrings = new();

        private Entry txtAmount;
        private Picker cmbFrom;
        private Picker cmbTo;
        private Label lblResult;
        private Label lblRate;
        private Frame resultFrame;
        private Label lblStatus;
        private CollectionView lstHistory;

        public MainPage()
        {
            InitializeComponent();
            Title = "FreelanceFX";
            BuildUI();
            LoadCurrenciesAsync();
        }

        private void BuildUI()
        {
            txtAmount = new Entry { Placeholder = "Enter amount", Keyboard = Keyboard.Numeric, Text = "1.00" };
            cmbFrom = new Picker { Title = "Select currency" };
            cmbTo = new Picker { Title = "Select currency" };
            lblResult = new Label { FontSize = 18, FontAttributes = FontAttributes.Bold, TextColor = Color.FromArgb("#145A32"), HorizontalOptions = LayoutOptions.Center };
            lblRate = new Label { FontSize = 12, TextColor = Colors.Gray, HorizontalOptions = LayoutOptions.Center };
            resultFrame = new Frame { BorderColor = Colors.LightGray, CornerRadius = 8, Padding = 16, IsVisible = false,
                Content = new VerticalStackLayout { Children = { lblResult, lblRate } } };
            lblStatus = new Label { FontSize = 12, TextColor = Colors.Gray, HorizontalOptions = LayoutOptions.Center };

            var swapBtn = new Button { Text = "⇅ Swap", BackgroundColor = Color.FromArgb("#E0E7FF"), TextColor = Color.FromArgb("#1E3C72") };
            swapBtn.Clicked += BtnSwap_Click;

            var convertBtn = new Button { Text = "Convert", BackgroundColor = Color.FromArgb("#1E3C72"), TextColor = Colors.White,
                FontAttributes = FontAttributes.Bold, FontSize = 16 };
            convertBtn.Clicked += BtnConvert_Click;

            lstHistory = new CollectionView { HeightRequest = 200,
                ItemTemplate = new DataTemplate(() => {
                    var lbl = new Label { FontSize = 11, Padding = new Thickness(4) };
                    lbl.SetBinding(Label.TextProperty, ".");
                    return lbl;
                })
            };

            Content = new ScrollView
            {
                Content = new VerticalStackLayout
                {
                    Padding = new Thickness(20),
                    Spacing = 12,
                    Children =
                    {
                        new Label { Text = "FreelanceFX", FontSize = 28, FontAttributes = FontAttributes.Bold, TextColor = Color.FromArgb("#1E3C72"), HorizontalOptions = LayoutOptions.Center },
                        new Label { Text = "Currency Converter", FontSize = 14, TextColor = Colors.Gray, HorizontalOptions = LayoutOptions.Center },
                        new Frame { BorderColor = Colors.LightGray, CornerRadius = 8, Padding = 16,
                            Content = new VerticalStackLayout { Spacing = 10, Children = {
                                new Label { Text = "Amount", FontAttributes = FontAttributes.Bold },
                                txtAmount,
                                new Label { Text = "From Currency", FontAttributes = FontAttributes.Bold },
                                cmbFrom,
                                swapBtn,
                                new Label { Text = "To Currency", FontAttributes = FontAttributes.Bold },
                                cmbTo,
                                convertBtn
                            }}
                        },
                        resultFrame,
                        lblStatus,
                        new Label { Text = "Conversion History", FontAttributes = FontAttributes.Bold, FontSize = 14, Margin = new Thickness(0,10,0,0) },
                        lstHistory
                    }
                }
            };
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
                lblStatus.Text = "Failed to load rates.";
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
