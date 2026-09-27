# Stock ROI & Tax Calculator (C# Windows Forms)

## Problem Recap
5 companies are listed on the stock exchange, each worth more than $100,000.
- Companies charge tax **only if you make a profit** when you sell (no tax on a loss/break-even).
- Company tax rates: **Company A = 5%, Company B = 5.5%, Company C = 7.5%, Company D = 6.3%, Company E = 9.9%**.
- In addition, the **government** charges its own tax on the same profit: **2% for a Filer, 4% for a Non-Filer**.
- Goal: help the investor decide which company would net the best ROI, given that future price movement is unknown.

## How the app solves it
The app has two interactive tabs (plus a menu bar for navigation):

1. **ROI Calculator** — pick one company, enter shares/buy price/sell price and filer status.
   The app computes total investment, gross profit, company tax, government tax, net profit,
   and net ROI %, with a full text breakdown.

2. **Compare All Companies** — since nobody can predict which company's stock will rise, this
   tab lets you enter an investment amount and one **assumed** profit percentage, applied
   equally to all 5 companies. The app then shows a sortable grid of net ROI per company
   after both taxes, highlights the best one in green, and prints a plain-English
   recommendation. Because the gross profit is the same for every company, this isolates
   exactly what the question asks: **which company's tax structure lets you keep the most
   profit** — a fair, data-driven proxy for "best ROI" when future prices are unknown.

Both tabs validate input (missing/negative/non-numeric values show a message box) and
apply the "no tax without profit" rule described in the problem.

## Project files
```
StockROICalculator/
  StockROICalculator.csproj   <- SDK-style WinForms project (.NET 8)
  Program.cs                  <- application entry point
  Company.cs                  <- the 5 companies + tax rules
  ROIResult.cs                <- helper model for the comparison grid
  MainForm.cs                 <- all UI + business logic
```

## How to build & run (Visual Studio 2022, Windows)
1. Install **Visual Studio 2022** with the **.NET desktop development** workload
   (this installs .NET 8 SDK and Windows Forms tooling).
2. Copy this whole `StockROICalculator` folder to your Windows machine.
3. Double-click `StockROICalculator.csproj` to open it in Visual Studio
   (or `File > Open > Project/Solution`).
4. Press **F5** (or click the green ▶ "Start" button) to build and run.

## How to build & run (command line, Windows with .NET 8 SDK installed)
```
cd StockROICalculator
dotnet build
dotnet run
```

## Taking your screenshots for submission
Once the app is running on Windows:
1. Screenshot the **ROI Calculator** tab with sample inputs and the result breakdown.
2. Screenshot the **Compare All Companies** tab with the grid filled in and the
   recommendation label.
3. Screenshot the **Help > About / Instructions** dialog.
4. Optionally, screenshot a validation message (e.g., leaving Buy Price empty and
   clicking Calculate) to show error handling.
Paste these screenshots into your submission document alongside this source code.
