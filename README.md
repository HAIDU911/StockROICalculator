# 📈 Stock ROI & Tax Calculator

A menu-driven **C# Windows Forms** desktop app that helps an investor estimate the **after-tax Return on Investment (ROI)** when buying and selling shares — and compares 5 listed companies against each other when the investor has no way of knowing in advance which stock will actually rise.

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet&logoColor=white)
![Platform](https://img.shields.io/badge/platform-Windows-0078D6?logo=windows&logoColor=white)
![WinForms](https://img.shields.io/badge/UI-Windows%20Forms-blue)
![License](https://img.shields.io/badge/license-MIT-green)

---

## 📝 Problem Statement

There are 5 companies listed on the stock exchange, each worth more than $100,000. Every company charges tax **only if the investor makes a profit** when selling — no profit means no tax. If there is a profit, the company charges its own tax, **plus** the investor must separately pay government tax based on filer status:

| | Company Tax (on profit only) |
|---|---|
| Company A | 5% |
| Company B | 5.5% |
| Company C | 7.5% |
| Company D | 6.3% |
| Company E | 9.9% |

**Government tax** (in addition to company tax, also only on profit):
- Filer → **2%**
- Non-Filer → **4%**

The challenge: the investor has **no information** about which company's stock will rise. This app can't predict the market — but it *can* calculate, precisely, which company's tax structure lets the investor keep the most of any profit they do make, and it gives a full interactive tool for running the numbers on real or hypothetical trades.

## ✨ Features

- **Menu-based, fully interactive** — `File`, `Tools`, and `Help` menus for navigation, plus two tabs for the two core workflows.
- **ROI Calculator tab** — pick one company, enter shares / buy price / sell price / filer status, and get a complete breakdown: total investment, gross profit, company tax, government tax, total tax, net profit, and net ROI %.
- **Compare All Companies tab** — since future price movement is unknown, enter one investment amount and one *assumed* profit %, applied equally to all 5 companies. The app ranks them by net ROI after both taxes and highlights the best one — isolating exactly the variable the investor *can* know in advance (the tax rate).
- **Input validation** with clear, friendly error messages (no crashes on bad input).
- **No profit → no tax**, correctly enforced everywhere per the problem statement.
- Clean, professional, color-coded UI (themed cards, colored section headers, a color-coded recommendation banner, and a sortable, scrollable comparison grid).
- Opens maximized; the comparison grid scrolls horizontally if a lower resolution can't show every column at once.


## 🛠️ Tech Stack

- **C# / .NET 8** (SDK-style project)
- **Windows Forms** (UI built entirely in code — no designer file needed)

## 📂 Project Structure

```
StockROICalculator/
├── StockROICalculator.csproj   # SDK-style WinForms project (net8.0-windows)
├── Program.cs                  # Application entry point
├── Company.cs                  # The 5 companies + tax rules
├── ROIResult.cs                # Helper model for the comparison grid
├── MainForm.cs                 # All UI (built in code) + business logic
└── README.md
```

## 🚀 Getting Started

### Prerequisites
- Windows 10/11
- [.NET 8 SDK](https://dotnet.microsoft.com/download) **or** Visual Studio 2022 with the **.NET desktop development** workload

### Run with Visual Studio
1. Clone the repo:
   ```bash
   git clone https://github.com/<your-username>/StockROICalculator.git
   ```
2. Open `StockROICalculator.csproj` in Visual Studio 2022.
3. Press **F5** (or click ▶ **Start**) to build and run.

### Run from the command line
```bash
git clone https://github.com/<your-username>/StockROICalculator.git
cd StockROICalculator
dotnet build
dotnet run
```

## 📖 How to Use It

1. **ROI Calculator tab**
   - Select a company, number of shares, buy price, expected sell price, and your filer status.
   - Click **Calculate ROI** to see the full tax and profit breakdown.
   - Click **Clear** to reset the form.

2. **Compare All Companies tab**
   - Enter an investment amount and an assumed profit % (this is applied equally to every company, since none of them can be predicted in advance).
   - Click **Compare All 5 Companies**.
   - The grid ranks all 5 companies by net ROI after tax, with the best option highlighted in green, and a colored banner explains the recommendation.

## 🧮 Calculation Logic

```
Total Investment   = Shares × Buy Price
Total Sale Value   = Shares × Sell Price
Gross Profit       = Total Sale Value − Total Investment

If Gross Profit > 0:
    Company Tax     = Gross Profit × Company Tax %
    Government Tax  = Gross Profit × (2% if Filer, else 4%)
Else:
    Company Tax = Government Tax = 0        # no tax without profit

Net Profit = Gross Profit − Company Tax − Government Tax
Net ROI %  = (Net Profit / Total Investment) × 100
```

## 🗺️ Roadmap / Ideas

- [ ] Persist trade history to a local file/database
- [ ] Export the comparison grid to CSV/Excel
- [ ] Add a chart visualizing net ROI per company
- [ ] Live stock price lookup via an API (optional, for real-world data)

## 🤝 Contributing

Issues and pull requests are welcome. If you spot a bug or have an idea for an improvement, feel free to open an issue.

## 📄 License

This project is licensed under the [MIT License](LICENSE).

## ⚠️ Disclaimer

This tool is for educational purposes only and does not constitute financial or tax advice. Tax rates and rules used here are illustrative (as defined in the assignment's problem statement), not a reflection of any real jurisdiction's laws. Always consult a qualified financial advisor before making investment decisions.
