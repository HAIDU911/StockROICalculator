using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace StockROICalculator
{
    public partial class MainForm : Form
    {
        private readonly List<Company> _companies;

        // ---- Theme palette ----
        private static readonly Color ColorPrimaryDark = Color.FromArgb(20, 54, 92);
        private static readonly Color ColorPrimary = Color.FromArgb(31, 97, 141);
        private static readonly Color ColorAccentGreen = Color.FromArgb(39, 138, 87);
        private static readonly Color ColorBackground = Color.FromArgb(240, 243, 247);
        private static readonly Color ColorCard = Color.White;
        private static readonly Color ColorBorder = Color.FromArgb(206, 214, 222);
        private static readonly Color ColorTextDark = Color.FromArgb(33, 37, 41);
        private static readonly Color ColorBestRow = Color.FromArgb(198, 239, 206);
        private static readonly Color ColorBestRowText = Color.FromArgb(20, 90, 50);
        private static readonly Color ColorBannerGood = Color.FromArgb(223, 247, 232);
        private static readonly Color ColorBannerGoodText = Color.FromArgb(20, 108, 67);
        private static readonly Color ColorBannerWarn = Color.FromArgb(255, 249, 219);
        private static readonly Color ColorBannerWarnText = Color.FromArgb(133, 100, 4);
        private static readonly Color ColorBannerBad = Color.FromArgb(253, 226, 226);
        private static readonly Color ColorBannerBadText = Color.FromArgb(160, 30, 30);

        // ---- Menu / chrome ----
        private MenuStrip menuStrip;
        private TabControl tabControl;

        // ---- Tab 1: Single Company ROI Calculator ----
        private TabPage tabSingle;
        private ComboBox cmbCompany;
        private NumericUpDown numShares;
        private TextBox txtBuyPrice;
        private TextBox txtSellPrice;
        private RadioButton rbFiler1;
        private RadioButton rbNonFiler1;
        private Button btnCalculate;
        private Button btnClearSingle;
        private TextBox txtResult;

        // ---- Tab 2: Compare All Companies ----
        private TabPage tabCompare;
        private TextBox txtInvestment;
        private TextBox txtAssumedProfitPercent;
        private RadioButton rbFiler2;
        private RadioButton rbNonFiler2;
        private Button btnCompare;
        private DataGridView dgvResults;
        private Label lblRecommendation;

        public MainForm()
        {
            _companies = Company.GetCompanies();
            BuildUI();
        }

        // =========================================================================================
        //  UI CONSTRUCTION
        // =========================================================================================
        private void BuildUI()
        {
            this.Text = "Stock ROI & Tax Calculator";
            this.Width = 1100;
            this.Height = 750;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(900, 620);
            this.Font = new Font("Segoe UI", 9F);
            this.BackColor = ColorBackground;
            this.WindowState = FormWindowState.Maximized; // open full-screen sized

            BuildMenu();
            var banner = BuildTopBanner();
            BuildTabs();

            menuStrip.Dock = DockStyle.Top;
            banner.Dock = DockStyle.Top;
            tabControl.Dock = DockStyle.Fill;

            // WinForms docking convention: add the Fill-docked control first, then
            // Top-docked controls in the order they should stack from the edge inward
            // (last one added ends up closest to the true top edge of the form).
            this.Controls.Add(tabControl);
            this.Controls.Add(banner);
            this.Controls.Add(menuStrip);
            this.MainMenuStrip = menuStrip;
        }

        private Panel BuildTopBanner()
        {
            var banner = new Panel { Height = 64, BackColor = ColorPrimaryDark };

            var title = new Label
            {
                Text = "Stock ROI & Tax Calculator",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 15F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(20, 6)
            };

            var subtitle = new Label
            {
                Text = "Estimate after-tax returns and compare 5 listed companies before you invest",
                ForeColor = Color.FromArgb(198, 216, 232),
                Font = new Font("Segoe UI", 9F),
                AutoSize = true,
                Location = new Point(22, 36)
            };

            banner.Controls.Add(title);
            banner.Controls.Add(subtitle);
            return banner;
        }

        private void BuildMenu()
        {
            menuStrip = new MenuStrip();
            menuStrip.BackColor = Color.White;
            menuStrip.Font = new Font("Segoe UI", 9.5F);

            var fileMenu = new ToolStripMenuItem("File");
            var exitItem = new ToolStripMenuItem("Exit", null, (s, e) => Application.Exit());
            fileMenu.DropDownItems.Add(exitItem);

            var toolsMenu = new ToolStripMenuItem("Tools");
            var goSingle = new ToolStripMenuItem("ROI Calculator (Single Company)", null,
                (s, e) => tabControl.SelectedTab = tabSingle);
            var goCompare = new ToolStripMenuItem("Compare All Companies", null,
                (s, e) => tabControl.SelectedTab = tabCompare);
            toolsMenu.DropDownItems.Add(goSingle);
            toolsMenu.DropDownItems.Add(goCompare);

            var helpMenu = new ToolStripMenuItem("Help");
            var aboutItem = new ToolStripMenuItem("About / Instructions", null, (s, e) => ShowAbout());
            helpMenu.DropDownItems.Add(aboutItem);

            menuStrip.Items.Add(fileMenu);
            menuStrip.Items.Add(toolsMenu);
            menuStrip.Items.Add(helpMenu);
        }

        private void BuildTabs()
        {
            tabControl = new TabControl();
            tabControl.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);

            tabSingle = new TabPage("  ROI Calculator  ") { BackColor = ColorBackground };
            tabCompare = new TabPage("  Compare All Companies  ") { BackColor = ColorBackground };

            BuildSingleTab();
            BuildCompareTab();

            tabControl.TabPages.Add(tabSingle);
            tabControl.TabPages.Add(tabCompare);
        }

        /// <summary>Wraps a control in a thin 1px colored border by padding it inside a colored panel.</summary>
        private Panel WrapWithBorder(Control inner)
        {
            var outer = new Panel { BackColor = ColorBorder, Padding = new Padding(1) };
            inner.Dock = DockStyle.Fill;
            outer.Controls.Add(inner);
            return outer;
        }

        /// <summary>Builds a colored section-header strip (used atop each card).</summary>
        private Panel BuildCardHeader(string text, Color backColor)
        {
            var header = new Panel { Dock = DockStyle.Top, Height = 32, BackColor = backColor };
            var lbl = new Label
            {
                Text = "  " + text,
                Dock = DockStyle.Fill,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft
            };
            header.Controls.Add(lbl);
            return header;
        }

        /// <summary>
        /// Builds a two-column TableLayoutPanel with fixed-height ABSOLUTE rows:
        /// one 48px row per field, one 60px row reserved for buttons, and a final
        /// Percent(100) filler row. Because every row has a guaranteed fixed height
        /// (rather than relying on auto-sizing), the label/input pairs always line
        /// up and the button row is always rendered directly beneath the last field.
        /// </summary>
        private TableLayoutPanel BuildFieldsTable(int fieldRowCount)
        {
            var table = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                BackColor = ColorCard
            };
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 270));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            table.RowCount = fieldRowCount + 2; // + button row + filler row
            for (int i = 0; i < fieldRowCount; i++)
                table.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 64));   // button row
            table.RowStyles.Add(new RowStyle(SizeType.Percent, 100));  // filler / spacer

            return table;
        }

        /// <summary>Places a label + its input control on one aligned table row.</summary>
        private void AddFieldRow(TableLayoutPanel table, int row, string labelText, Control input)
        {
            var lbl = new Label
            {
                Text = labelText,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = ColorTextDark,
                Margin = new Padding(20, 0, 10, 0)
            };
            input.Anchor = AnchorStyles.Left;
            input.Margin = new Padding(0, 11, 10, 5);

            table.Controls.Add(lbl, 0, row);
            table.Controls.Add(input, 1, row);
        }

        /// <summary>Places one or more buttons, left-aligned under the input column, on their own table row.</summary>
        private void AddButtonRow(TableLayoutPanel table, int row, params Button[] buttons)
        {
            var flow = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                WrapContents = false,
                Margin = new Padding(0, 12, 0, 8)
            };
            foreach (var b in buttons)
            {
                b.Margin = new Padding(0, 0, 15, 0);
                flow.Controls.Add(b);
            }

            table.Controls.Add(new Label { Dock = DockStyle.Fill }, 0, row); // keeps column alignment
            table.Controls.Add(flow, 1, row);
        }

        private Button StylePrimaryButton(Button b)
        {
            b.FlatStyle = FlatStyle.Flat;
            b.FlatAppearance.BorderSize = 0;
            b.BackColor = ColorPrimary;
            b.ForeColor = Color.White;
            b.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            b.Cursor = Cursors.Hand;
            b.UseVisualStyleBackColor = false;
            return b;
        }

        private Button StyleSecondaryButton(Button b)
        {
            b.FlatStyle = FlatStyle.Flat;
            b.FlatAppearance.BorderSize = 1;
            b.FlatAppearance.BorderColor = ColorBorder;
            b.BackColor = Color.White;
            b.ForeColor = ColorTextDark;
            b.Font = new Font("Segoe UI", 9.5F);
            b.Cursor = Cursors.Hand;
            b.UseVisualStyleBackColor = false;
            return b;
        }

        private RadioButton StyleRadio(RadioButton r)
        {
            r.Font = new Font("Segoe UI", 9.5F);
            r.AutoSize = true;
            return r;
        }

        // =========================================================================================
        //  TAB 1 UI: Single company ROI calculator
        // =========================================================================================
        private void BuildSingleTab()
        {
            var content = new Panel { Dock = DockStyle.Fill, BackColor = ColorBackground };

            // ---- Input card (5 fields + button row, built on a fixed-row TableLayoutPanel) ----
            var table = BuildFieldsTable(5);

            cmbCompany = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Size = new Size(420, 25),
                Font = new Font("Segoe UI", 9.5F)
            };
            foreach (var c in _companies) cmbCompany.Items.Add(c);
            cmbCompany.SelectedIndex = 0;
            AddFieldRow(table, 0, "Select Company:", cmbCompany);

            numShares = new NumericUpDown
            {
                Minimum = 1,
                Maximum = 1000000,
                Value = 100,
                Size = new Size(150, 25),
                Font = new Font("Segoe UI", 9.5F)
            };
            AddFieldRow(table, 1, "Number of Shares to Buy:", numShares);

            txtBuyPrice = new TextBox { Size = new Size(150, 25), Text = "50", Font = new Font("Segoe UI", 9.5F) };
            AddFieldRow(table, 2, "Buy Price per Share ($):", txtBuyPrice);

            txtSellPrice = new TextBox { Size = new Size(150, 25), Text = "60", Font = new Font("Segoe UI", 9.5F) };
            AddFieldRow(table, 3, "Expected Sell Price per Share ($):", txtSellPrice);

            var filerPanel = new FlowLayoutPanel { AutoSize = true };
            rbFiler1 = StyleRadio(new RadioButton { Text = "Filer (2% Govt. Tax)", Checked = true });
            rbNonFiler1 = StyleRadio(new RadioButton { Text = "Non-Filer (4% Govt. Tax)", Margin = new Padding(25, 3, 0, 0) });
            filerPanel.Controls.Add(rbFiler1);
            filerPanel.Controls.Add(rbNonFiler1);
            AddFieldRow(table, 4, "Tax Filer Status:", filerPanel);

            btnCalculate = StylePrimaryButton(new Button { Text = "Calculate ROI", Size = new Size(150, 36) });
            btnCalculate.Click += BtnCalculate_Click;
            btnClearSingle = StyleSecondaryButton(new Button { Text = "Clear", Size = new Size(100, 36) });
            btnClearSingle.Click += (s, e) => ClearSingleTab();
            AddButtonRow(table, 5, btnCalculate, btnClearSingle);

            var inputInner = new Panel { BackColor = ColorCard };
            inputInner.Controls.Add(table);                                        // Fill, add first
            inputInner.Controls.Add(BuildCardHeader("Trade Details", ColorPrimary)); // Top, add last

            var inputCard = WrapWithBorder(inputInner);
            inputCard.Location = new Point(20, 20);
            inputCard.Size = new Size(1040, 350);
            inputCard.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            // ---- Result card ----
            txtResult = new TextBox
            {
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.None,
                Font = new Font("Consolas", 10F),
                BackColor = Color.White,
                ForeColor = ColorTextDark,
                Text = "Enter your trade details above and click \"Calculate ROI\" to see the full breakdown here."
            };
            var resultPad = new Panel { Dock = DockStyle.Fill, Padding = new Padding(12), BackColor = ColorCard };
            resultPad.Controls.Add(txtResult);

            var resultInner = new Panel { BackColor = ColorCard };
            resultInner.Controls.Add(resultPad);                                             // Fill, add first
            resultInner.Controls.Add(BuildCardHeader("Result Breakdown", ColorAccentGreen));  // Top, add last

            var resultCard = WrapWithBorder(resultInner);
            resultCard.Location = new Point(20, inputCard.Bottom + 15);
            resultCard.Size = new Size(1040, 260);
            resultCard.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            content.Controls.Add(resultCard);
            content.Controls.Add(inputCard);

            tabSingle.Controls.Add(content);
        }

        // =========================================================================================
        //  TAB 2 UI: Compare all companies
        // =========================================================================================
        private void BuildCompareTab()
        {
            var content = new Panel { Dock = DockStyle.Fill, BackColor = ColorBackground };

            // ---- Input card (3 fields + button row) ----
            var table = BuildFieldsTable(3);

            txtInvestment = new TextBox { Size = new Size(150, 25), Text = "10000", Font = new Font("Segoe UI", 9.5F) };
            AddFieldRow(table, 0, "Investment Amount ($):", txtInvestment);

            txtAssumedProfitPercent = new TextBox { Size = new Size(150, 25), Text = "15", Font = new Font("Segoe UI", 9.5F) };
            AddFieldRow(table, 1, "Assumed Profit on Sale (%):", txtAssumedProfitPercent);

            var filerPanel = new FlowLayoutPanel { AutoSize = true };
            rbFiler2 = StyleRadio(new RadioButton { Text = "Filer (2% Govt. Tax)", Checked = true });
            rbNonFiler2 = StyleRadio(new RadioButton { Text = "Non-Filer (4% Govt. Tax)", Margin = new Padding(25, 3, 0, 0) });
            filerPanel.Controls.Add(rbFiler2);
            filerPanel.Controls.Add(rbNonFiler2);
            AddFieldRow(table, 2, "Tax Filer Status:", filerPanel);

            btnCompare = StylePrimaryButton(new Button { Text = "Compare All 5 Companies", Size = new Size(220, 36) });
            btnCompare.Click += BtnCompare_Click;
            AddButtonRow(table, 3, btnCompare);

            var inputInner = new Panel { BackColor = ColorCard };
            inputInner.Controls.Add(table);                                             // Fill, add first
            inputInner.Controls.Add(BuildCardHeader("Scenario Settings", ColorPrimary)); // Top, add last

            var inputCard = WrapWithBorder(inputInner);
            inputCard.Location = new Point(20, 20);
            inputCard.Size = new Size(1040, 270);
            inputCard.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            // ---- Recommendation banner ----
            lblRecommendation = new Label
            {
                Text = "  Click \"Compare All 5 Companies\" to see the recommendation.",
                Location = new Point(20, inputCard.Bottom + 15),
                Size = new Size(1040, 40),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                BackColor = ColorBannerWarn,
                ForeColor = ColorBannerWarnText,
                TextAlign = ContentAlignment.MiddleLeft,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            // ---- Comparison grid card ----
            // AutoSizeColumnsMode is intentionally None (not Fill): each column gets an
            // explicit width, so when the total column width is wider than the visible
            // grid area, the grid shows its own horizontal scrollbar instead of squeezing
            // (and cutting off) columns.
            dgvResults = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None,
                ScrollBars = ScrollBars.Both,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                GridColor = ColorBorder,
                EnableHeadersVisualStyles = false,
                Font = new Font("Segoe UI", 9.3F),
                RowTemplate = { Height = 30 }
            };
            dgvResults.ColumnHeadersDefaultCellStyle.BackColor = ColorPrimary;
            dgvResults.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvResults.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.3F, FontStyle.Bold);
            dgvResults.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvResults.ColumnHeadersHeight = 34;
            dgvResults.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 248, 251);
            dgvResults.DefaultCellStyle.SelectionBackColor = Color.FromArgb(210, 230, 245);
            dgvResults.DefaultCellStyle.SelectionForeColor = ColorTextDark;
            dgvResults.DefaultCellStyle.Padding = new Padding(4, 2, 4, 2);

            dgvResults.Columns.Add("Company", "Company");
            dgvResults.Columns.Add("Worth", "Worth ($)");
            dgvResults.Columns.Add("CompanyTax", "Co. Tax %");
            dgvResults.Columns.Add("GrossProfit", "Gross Profit ($)");
            dgvResults.Columns.Add("CompanyTaxAmt", "Company Tax ($)");
            dgvResults.Columns.Add("GovtTaxAmt", "Govt. Tax ($)");
            dgvResults.Columns.Add("TotalTax", "Total Tax ($)");
            dgvResults.Columns.Add("NetProfit", "Net Profit ($)");
            dgvResults.Columns.Add("NetROI", "Net ROI (%)");

            int[] widths = { 150, 110, 90, 130, 130, 120, 110, 130, 110 };
            for (int i = 0; i < dgvResults.Columns.Count; i++)
                dgvResults.Columns[i].Width = widths[i];

            dgvResults.Columns["Company"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            foreach (DataGridViewColumn col in dgvResults.Columns)
            {
                if (col.Name != "Company")
                    col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }

            var gridPad = new Panel { Dock = DockStyle.Fill, Padding = new Padding(8), BackColor = ColorCard };
            gridPad.Controls.Add(dgvResults);

            var gridInner = new Panel { BackColor = ColorCard };
            gridInner.Controls.Add(gridPad);                                                                  // Fill, add first
            gridInner.Controls.Add(BuildCardHeader("Comparison (sorted by best Net ROI)", ColorAccentGreen)); // Top, add last

            var gridCard = WrapWithBorder(gridInner);
            gridCard.Location = new Point(20, lblRecommendation.Bottom + 15);
            gridCard.Size = new Size(1040, 300);
            gridCard.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            content.Controls.Add(gridCard);
            content.Controls.Add(lblRecommendation);
            content.Controls.Add(inputCard);

            tabCompare.Controls.Add(content);
        }

        // =========================================================================================
        //  TAB 1 LOGIC: Single company ROI calculation
        // =========================================================================================
        private void BtnCalculate_Click(object sender, EventArgs e)
        {
            if (cmbCompany.SelectedItem == null)
            {
                MessageBox.Show("Please select a company first.", "Input Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!decimal.TryParse(txtBuyPrice.Text.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out decimal buyPrice) || buyPrice <= 0)
            {
                MessageBox.Show("Please enter a valid positive Buy Price.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtBuyPrice.Focus();
                return;
            }

            if (!decimal.TryParse(txtSellPrice.Text.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out decimal sellPrice) || sellPrice < 0)
            {
                MessageBox.Show("Please enter a valid, non-negative Sell Price.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtSellPrice.Focus();
                return;
            }

            var company = (Company)cmbCompany.SelectedItem;
            int shares = (int)numShares.Value;
            bool isFiler = rbFiler1.Checked;

            decimal totalInvestment = shares * buyPrice;
            decimal totalSaleValue = shares * sellPrice;
            decimal grossProfit = totalSaleValue - totalInvestment;

            decimal companyTaxAmount = 0m;
            decimal govtTaxAmount = 0m;
            decimal govtTaxPercent = Company.GetGovernmentTaxPercent(isFiler);

            // Per problem statement: taxes (company + govt.) only apply when there IS a profit.
            if (grossProfit > 0)
            {
                companyTaxAmount = grossProfit * company.CompanyTaxPercent / 100m;
                govtTaxAmount = grossProfit * govtTaxPercent / 100m;
            }

            decimal totalTax = companyTaxAmount + govtTaxAmount;
            decimal netProfit = grossProfit - totalTax;
            decimal netROI = totalInvestment == 0 ? 0 : (netProfit / totalInvestment) * 100m;

            var sb = new System.Text.StringBuilder();
            sb.AppendLine("==================== ROI CALCULATION RESULT ====================");
            sb.AppendLine($"Company                  : {company.Name}");
            sb.AppendLine($"Company Worth            : ${company.Worth:N2}");
            sb.AppendLine($"Shares Purchased         : {shares}");
            sb.AppendLine($"Buy Price / Share        : ${buyPrice:N2}");
            sb.AppendLine($"Sell Price / Share       : ${sellPrice:N2}");
            sb.AppendLine("------------------------------------------------------------------");
            sb.AppendLine($"Total Investment         : ${totalInvestment:N2}");
            sb.AppendLine($"Total Sale Value         : ${totalSaleValue:N2}");
            sb.AppendLine($"Gross Profit / (Loss)    : ${grossProfit:N2}");
            sb.AppendLine("------------------------------------------------------------------");
            if (grossProfit > 0)
            {
                sb.AppendLine($"Company Tax ({company.CompanyTaxPercent}%)       : ${companyTaxAmount:N2}");
                sb.AppendLine($"Govt. Tax ({(isFiler ? "Filer 2%" : "Non-Filer 4%")})     : ${govtTaxAmount:N2}");
                sb.AppendLine($"Total Tax Deducted      : ${totalTax:N2}");
            }
            else
            {
                sb.AppendLine("No profit was made, so NO tax (company or government) applies.");
            }
            sb.AppendLine("------------------------------------------------------------------");
            sb.AppendLine($"NET PROFIT / (LOSS)      : ${netProfit:N2}");
            sb.AppendLine($"NET ROI                  : {netROI:N2} %");
            sb.AppendLine("===================================================================");

            txtResult.Text = sb.ToString();
            txtResult.SelectionStart = 0;
            txtResult.ScrollToCaret();
        }

        private void ClearSingleTab()
        {
            cmbCompany.SelectedIndex = 0;
            numShares.Value = 100;
            txtBuyPrice.Text = "50";
            txtSellPrice.Text = "60";
            rbFiler1.Checked = true;
            txtResult.Text = "Enter your trade details above and click \"Calculate ROI\" to see the full breakdown here.";
        }

        // =========================================================================================
        //  TAB 2 LOGIC: Compare all 5 companies under an equal, assumed profit scenario
        // =========================================================================================
        private void BtnCompare_Click(object sender, EventArgs e)
        {
            if (!decimal.TryParse(txtInvestment.Text.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out decimal investment) || investment <= 0)
            {
                MessageBox.Show("Please enter a valid positive Investment Amount.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtInvestment.Focus();
                return;
            }

            if (!decimal.TryParse(txtAssumedProfitPercent.Text.Trim(), NumberStyles.Number | NumberStyles.AllowLeadingSign,
                CultureInfo.InvariantCulture, out decimal profitPercent))
            {
                MessageBox.Show("Please enter a valid Assumed Profit % (can be negative for a loss scenario).",
                    "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtAssumedProfitPercent.Focus();
                return;
            }

            bool isFiler = rbFiler2.Checked;
            decimal govtTaxPercent = Company.GetGovernmentTaxPercent(isFiler);

            var results = new List<ROIResult>();

            foreach (var company in _companies)
            {
                decimal grossProfit = investment * profitPercent / 100m;
                decimal companyTaxAmount = 0m;
                decimal govtTaxAmount = 0m;

                if (grossProfit > 0)
                {
                    companyTaxAmount = grossProfit * company.CompanyTaxPercent / 100m;
                    govtTaxAmount = grossProfit * govtTaxPercent / 100m;
                }

                decimal totalTax = companyTaxAmount + govtTaxAmount;
                decimal netProfit = grossProfit - totalTax;
                decimal netROI = (netProfit / investment) * 100m;

                results.Add(new ROIResult
                {
                    CompanyName = company.Name,
                    Worth = company.Worth,
                    CompanyTaxPercent = company.CompanyTaxPercent,
                    GrossProfit = grossProfit,
                    CompanyTaxAmount = companyTaxAmount,
                    GovernmentTaxAmount = govtTaxAmount,
                    TotalTaxAmount = totalTax,
                    NetProfit = netProfit,
                    NetROIPercent = netROI
                });
            }

            // Sort best net ROI first, so the recommended company is always on top.
            var sorted = results.OrderByDescending(r => r.NetROIPercent).ToList();

            dgvResults.Rows.Clear();
            foreach (var r in sorted)
            {
                dgvResults.Rows.Add(
                    r.CompanyName,
                    r.Worth.ToString("N0"),
                    r.CompanyTaxPercent.ToString("0.0") + "%",
                    r.GrossProfit.ToString("N2"),
                    r.CompanyTaxAmount.ToString("N2"),
                    r.GovernmentTaxAmount.ToString("N2"),
                    r.TotalTaxAmount.ToString("N2"),
                    r.NetProfit.ToString("N2"),
                    r.NetROIPercent.ToString("N2") + "%"
                );
            }

            if (dgvResults.Rows.Count > 0)
            {
                var bestRow = dgvResults.Rows[0];
                bestRow.DefaultCellStyle.BackColor = ColorBestRow;
                bestRow.DefaultCellStyle.ForeColor = ColorBestRowText;
                bestRow.DefaultCellStyle.Font = new Font(dgvResults.Font, FontStyle.Bold);
            }

            var best = sorted.First();
            if (profitPercent > 0)
            {
                lblRecommendation.BackColor = ColorBannerGood;
                lblRecommendation.ForeColor = ColorBannerGoodText;
                lblRecommendation.Text =
                    $"  Recommendation: {best.CompanyName} gives the highest Net ROI ({best.NetROIPercent:N2}%) " +
                    $"for the same {profitPercent:N2}% assumed gross profit, because it charges the lowest combined tax.";
            }
            else if (profitPercent == 0)
            {
                lblRecommendation.BackColor = ColorBannerWarn;
                lblRecommendation.ForeColor = ColorBannerWarnText;
                lblRecommendation.Text = "  At 0% profit, no tax applies anywhere -- all companies give an identical (zero) Net ROI.";
            }
            else
            {
                lblRecommendation.BackColor = ColorBannerBad;
                lblRecommendation.ForeColor = ColorBannerBadText;
                lblRecommendation.Text =
                    "  This is a loss scenario. No taxes apply on a loss, so all 5 companies show the same Net ROI (a loss).";
            }
        }

        private void ShowAbout()
        {
            MessageBox.Show(
                "Stock ROI & Tax Calculator\n\n" +
                "This tool helps you estimate the Return on Investment (ROI) after taxes " +
                "when buying and selling shares of one of 5 listed companies.\n\n" +
                "Rules applied:\n" +
                " - Company tax is charged ONLY if you make a profit when selling.\n" +
                " - Company tax rates: A=5%, B=5.5%, C=7.5%, D=6.3%, E=9.9%.\n" +
                " - Government tax (in addition to company tax, only on profit):\n" +
                "     Filer = 2%,  Non-Filer = 4%.\n\n" +
                "Use the 'ROI Calculator' tab to test one company at a time, or the " +
                "'Compare All Companies' tab to see which company would net you the best " +
                "return if all of them delivered the same hypothetical profit percentage " +
                "(useful since future price movement is unknown).",
                "About / Instructions",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
    }
}