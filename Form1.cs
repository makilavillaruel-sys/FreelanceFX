using CurrencyApp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection.PortableExecutable;
using System.Windows.Forms;

namespace CurrencyApp
{
    public partial class Form1 : Form
    {
        private readonly ExchangeRates _service = new ExchangeRates();
        private readonly List<ConversionRecords> _history = new List<ConversionRecords>();
        public Form1()
        {
            InitializeComponent();
            BuildUI();
            LoadCurrenciesAsync();
        }
        private void BuildUI()
        {
            // ?? Form ????????????????????????????????????????????????
            this.Text = "FreelanceFX — Currency Converter";
            this.Size = new Size(720, 680);
            this.MinimumSize = new Size(720, 680);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Segoe UI", 9.5f);
            this.BackColor = Color.FromArgb(245, 247, 250);

            // ?? Header Panel ????????????????????????????????????????
            pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = Color.FromArgb(30, 60, 114)
            };
            lblTitle = new Label
            {
                Text = "FreelanceFX",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 20f, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(20, 12)
            };

            lblSubtitle = new Label
            {
                Text = "Real-time currency converter for freelancers & small businesses",
                ForeColor = Color.FromArgb(180, 210, 255),
                Font = new Font("Segoe UI", 9f),
                AutoSize = true,
                Location = new Point(22, 50)
            };
            pnlHeader.Controls.AddRange(new Control[] { lblTitle, lblSubtitle });

            // ?? Main Panel ??????????????????????????????????????????
            pnlMain = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20),
                AutoScroll = true
            };
            // ?? Converter GroupBox ??????????????????????????????????
            grpConvert = new GroupBox
            {
                Text = "Convert Currency",
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 60, 114),
                Location = new Point(20, 20),
                Size = new Size(660, 200)
            };
            lblAmount = new Label { Text = "Amount:", Location = new Point(15, 35), AutoSize = true };
            txtAmount = new TextBox
            {
                Location = new Point(15, 55),
                Size = new Size(150, 28),
                Font = new Font("Segoe UI", 11f),
                Text = "1.00"
            };
            lblFrom = new Label { Text = "From:", Location = new Point(185, 35), AutoSize = true };
            cmbFrom = new ComboBox
            {
                Location = new Point(185, 55),
                Size = new Size(150, 28),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10f)
            };

            lblArrow = new Label
            {
                Text = "?",
                Font = new Font("Segoe UI", 18f),
                ForeColor = Color.FromArgb(30, 60, 114),
                Location = new Point(348, 50),
                AutoSize = true
            };

            lblTo = new Label { Text = "To:", Location = new Point(380, 35), AutoSize = true };
            cmbTo = new ComboBox
            {
                Location = new Point(380, 55),
                Size = new Size(150, 28),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10f)
            };

            btnSwap = new Button
            {
                Text = "? Swap",
                Location = new Point(545, 53),
                Size = new Size(90, 30),
                BackColor = Color.FromArgb(224, 231, 255),
                ForeColor = Color.FromArgb(30, 60, 114),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnSwap.FlatAppearance.BorderColor = Color.FromArgb(30, 60, 114);
            btnSwap.Click += BtnSwap_Click;

            btnConvert = new Button
            {
                Text = "Convert",
                Location = new Point(15, 110),
                Size = new Size(620, 42),
                BackColor = Color.FromArgb(30, 60, 114),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11f, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnConvert.FlatAppearance.BorderSize = 0;
            btnConvert.Click += BtnConvert_Click;


            // ?? Result Panel ????????????????????????????????????????
            pnlResult = new Panel
            {
                Location = new Point(15, 162),
                Size = new Size(620, 28),
                BackColor = Color.FromArgb(235, 245, 255)
            };

            lblResultValue = new Label
            {
                Text = "Enter an amount and press Convert",
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 60, 114),
                AutoSize = true,
                Location = new Point(10, 5)
            };


            lblResultRate = new Label
            {
                Text = "",
                Font = new Font("Segoe UI", 8.5f),
                ForeColor = Color.Gray,
                AutoSize = true,
                Location = new Point(10, 4)
            };

            pnlResult.Controls.Add(lblResultValue);

            grpConvert.Controls.AddRange(new Control[]
             {
                lblAmount, txtAmount,
                lblFrom, cmbFrom,
                lblArrow,
                lblTo, cmbTo,
                btnSwap, btnConvert, pnlResult
            });

            // ?? History GroupBox ????????????????????????????????????
            grpHistory = new GroupBox
            {
                Text = "Conversion History",
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 60, 114),
                Location = new Point(20, 235),
                Size = new Size(660, 320)
            };
            lstHistory = new ListBox
            {
                Location = new Point(15, 25),
                Size = new Size(625, 240),
                Font = new Font("Consolas", 9f),
                HorizontalScrollbar = true,
                BorderStyle = BorderStyle.FixedSingle
            };
            btnClearHistory = new Button
            {
                Text = "Clear History",
                Location = new Point(15, 278),
                Size = new Size(130, 32),
                BackColor = Color.FromArgb(255, 235, 235),
                ForeColor = Color.DarkRed,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };

            btnClearHistory.FlatAppearance.BorderColor = Color.DarkRed;
            btnClearHistory.Click += (s, e) => { _history.Clear(); lstHistory.Items.Clear(); };

            btnExport = new Button
            {
                Text = "Export to CSV",
                Location = new Point(510, 278),
                Size = new Size(130, 32),
                BackColor = Color.FromArgb(230, 255, 235),
                ForeColor = Color.DarkGreen,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnExport.FlatAppearance.BorderColor = Color.DarkGreen;
            btnExport.Click += BtnExport_Click;

            grpHistory.Controls.AddRange(new Control[] { lstHistory, btnClearHistory, btnExport });

            // ?? Status Bar ??????????????????????????????????????????
            lblStatus = new Label
            {
                Text = "Loading currencies...",
                Location = new Point(20, 570),
                AutoSize = true,
                ForeColor = Color.Gray,
                Font = new Font("Segoe UI", 8.5f)
            };

            lblLastUpdate = new Label
            {
                Text = "",
                Location = new Point(500, 570),
                AutoSize = true,
                ForeColor = Color.Gray,
                Font = new Font("Segoe UI", 8.5f)
            };
            pnlMain.Controls.AddRange(new Control[]
          {
                grpConvert, grpHistory, lblStatus, lblLastUpdate
          });
            this.Controls.AddRange(new Control[] { pnlMain, pnlHeader });

            // Allow Enter key to trigger convert
            txtAmount.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) BtnConvert_Click(s, e); };
        }
        // ?? Load currencies from API ?????????????????????????????????
        private async void LoadCurrenciesAsync()
        {
            try
            {
                lblStatus.Text = " Fetching live rates...";
                var currencies = await _service.GetAvailableCurrenciesAsync();

                cmbFrom.Items.Clear();
                cmbTo.Items.Clear();
                foreach (var c in currencies)
                {
                    cmbFrom.Items.Add(c);
                    cmbTo.Items.Add(c);
                }

                // Smart defaults for freelancers
                cmbFrom.SelectedItem = "USD";
                cmbTo.SelectedItem = "TTD"; // Trinidad & Tobago Dollar - local relevance
                if (cmbTo.SelectedIndex < 0) cmbTo.SelectedItem = "EUR";

                lblStatus.Text = $" {currencies.Count} currencies loaded.";
                lblLastUpdate.Text = $"Last updated: {_service.GetLastUpdateTime()}";
            }
            catch (Exception ex)
            {
                lblStatus.Text = " Failed to load rates. Check internet connection.";
                MessageBox.Show($"Error loading currencies:\n{ex.Message}", "Connection Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        // ?? Convert button ???????????????????????????????????????????
        private async void BtnConvert_Click(object sender, EventArgs e)
        {
            if (!double.TryParse(txtAmount.Text, out double amount) || amount <= 0)
            {
                MessageBox.Show("Please enter a valid positive amount.", "Invalid Input",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtAmount.Focus();
                return;
            }

            if (cmbFrom.SelectedItem == null || cmbTo.SelectedItem == null)
            {
                MessageBox.Show("Please select both currencies.", "Missing Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string from = cmbFrom.SelectedItem.ToString();
            string to = cmbTo.SelectedItem.ToString();

            btnConvert.Enabled = false;
            btnConvert.Text = "Converting...";
            lblStatus.Text = " Fetching latest rates...";

            try
            {
                double result = await _service.ConvertAsync(amount, from, to);
                double rate = result / amount;

                // Show result
                pnlResult.BackColor = Color.FromArgb(220, 245, 220);
                lblResultValue.Text = $"  {amount:N2} {from}  =  {result:N4} {to}";
                lblResultValue.ForeColor = Color.FromArgb(20, 100, 20);

                // Add to history
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
                lstHistory.Items.Insert(0, record.ToString());

                lblStatus.Text = $" Converted successfully. Rate: 1 {from} = {rate:N4} {to}";
                lblLastUpdate.Text = $"Last updated: {_service.GetLastUpdateTime()}";
            }
            catch (Exception ex)
            {
                pnlResult.BackColor = Color.FromArgb(255, 230, 230);
                lblResultValue.Text = "  Conversion failed. See status below.";
                lblResultValue.ForeColor = Color.DarkRed;
                lblStatus.Text = $" Error: {ex.Message}";
            }
            finally
            {
                btnConvert.Enabled = true;
                btnConvert.Text = "Convert";
            }
        }
        // ?? Swap currencies ??????????????????????????????????????????
        private void BtnSwap_Click(object sender, EventArgs e)
        {
            var temp = cmbFrom.SelectedItem;
            cmbFrom.SelectedItem = cmbTo.SelectedItem;
            cmbTo.SelectedItem = temp;
        }
        // ?? Export history to CSV ????????????????????????????????????
        private void BtnExport_Click(object sender, EventArgs e)
        {
            if (_history.Count == 0)
            {
                MessageBox.Show("No history to export.", "Export", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            using (var dlg = new SaveFileDialog
            {
                Filter = "CSV Files (*.csv)|*.csv",
                FileName = $"conversions_{DateTime.Now:yyyyMMdd_HHmm}.csv",
                Title = "Export Conversion History"
            })
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var lines = new System.Collections.Generic.List<string>
                        {
                            "Date,Time,Amount From,Currency From,Amount To,Currency To,Rate"
                        };
                        foreach (var r in _history)
                        {
                            lines.Add($"{r.Timestamp:dd/MM/yyyy},{r.Timestamp:HH:mm},{r.AmountFrom},{r.CurrencyFrom},{r.AmountTo:N4},{r.CurrencyTo},{r.Rate:N6}");
                        }
                        System.IO.File.WriteAllLines(dlg.FileName, lines);
                        MessageBox.Show($"Exported {_history.Count} record(s) successfully.", "Export Complete",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Export failed:\n{ex.Message}", "Export Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}

   


   





