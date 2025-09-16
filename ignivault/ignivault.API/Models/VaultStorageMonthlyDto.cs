namespace ignivault.API.Models
{
    public class VaultStorageMonthlyDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public double TotalSizeKb { get; set; }
        public double AverageItemSizeKb { get; set; }
        public int ItemCount { get; set; }
    }
}
