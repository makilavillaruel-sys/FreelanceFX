namespace CurrencyApp
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
        // ── Controls ────────────────────────────────────────────────
        private Panel pnlHeader;
        private Label lblTitle;
        private Label lblSubtitle;
        private Panel pnlMain;
        private GroupBox grpConvert;
        private Label lblAmount;
        private TextBox txtAmount;
        private Label lblFrom;
        private ComboBox cmbFrom;
        private Label lblArrow;
        private Label lblTo;
        private ComboBox cmbTo;
        private Button btnSwap;
        private Button btnConvert;
        private Panel pnlResult;
        private Label lblResultValue;
        private Label lblResultRate;
        private GroupBox grpHistory;
        private ListBox lstHistory;
        private Button btnClearHistory;
        private Button btnExport;
        private Label lblStatus;
        private Label lblLastUpdate;
        private PictureBox picLoader;


        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Text = "Form1";
        }

        #endregion
    }
}
