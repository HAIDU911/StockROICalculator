using System.Collections.Generic;

namespace StockROICalculator
{
    /// <summary>
    /// Represents one of the 5 listed companies.
    /// Worth is approximate current market worth (all > $100,000 as per the problem statement).
    /// CompanyTaxPercent is the tax charged by the company ONLY when the seller makes a profit.
    /// If there is no profit (loss or break-even), the company charges no tax at all.
    /// </summary>
    public class Company
    {
        public string Name { get; set; }
        public decimal Worth { get; set; }
        public decimal CompanyTaxPercent { get; set; }

        public Company(string name, decimal worth, decimal companyTaxPercent)
        {
            Name = name;
            Worth = worth;
            CompanyTaxPercent = companyTaxPercent;
        }

        public override string ToString()
        {
            return $"{Name}  (Worth: ${Worth:N0} | Company Tax on Profit: {CompanyTaxPercent}%)";
        }

        /// <summary>
        /// The fixed list of 5 companies used throughout the application.
        /// </summary>
        public static List<Company> GetCompanies()
        {
            return new List<Company>
            {
                new Company("Company A", 120000m, 5.0m),
                new Company("Company B", 150000m, 5.5m),
                new Company("Company C", 110000m, 7.5m),
                new Company("Company D", 135000m, 6.3m),
                new Company("Company E", 105000m, 9.9m)
            };
        }

        /// <summary>Government tax percentage based on filer status (fixed by law, not by company).</summary>
        public static decimal GetGovernmentTaxPercent(bool isFiler)
        {
            return isFiler ? 2.0m : 4.0m;
        }
    }
}
