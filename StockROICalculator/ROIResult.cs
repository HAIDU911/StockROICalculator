namespace StockROICalculator
{
    /// <summary>
    /// Holds the computed tax/ROI breakdown for one company, used to populate
    /// the comparison grid on the "Compare Companies" tab.
    /// </summary>
    public class ROIResult
    {
        public string CompanyName { get; set; }
        public decimal Worth { get; set; }
        public decimal CompanyTaxPercent { get; set; }
        public decimal GrossProfit { get; set; }
        public decimal CompanyTaxAmount { get; set; }
        public decimal GovernmentTaxAmount { get; set; }
        public decimal TotalTaxAmount { get; set; }
        public decimal NetProfit { get; set; }
        public decimal NetROIPercent { get; set; }
    }
}
