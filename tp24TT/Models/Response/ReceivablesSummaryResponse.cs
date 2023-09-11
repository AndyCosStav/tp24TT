namespace tp24TT.Models.Response
{
    public class ReceivablesSummaryResponse
    {
        public int TotalEntries { get; set; }
        public decimal PercentageOpen { get; set; }
        public decimal PercentageClosed { get; set; }
        public decimal PercentageCancelled { get; set; }
        public decimal TotalPaidValueOpen { get; set; } 
        public decimal TotalPaidValueClosed { get; set; } 
    }
}
