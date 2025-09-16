namespace ignivault.Data.Models.Data
{
    public class VaultStorageMonthly
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public double TotalSizeKb { get; set; }
        public double AverageItemSizeKb { get; set; }
        public int ItemCount { get; set; }
    }
}
