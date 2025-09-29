using ignivault.Shared.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ignivault.Shared.DTOs
{
    public class Reports
    {
        /// <summary>
        /// ItemDistributionDto represents the distribution of different types of items stored in the vault.
        /// </summary>
        public class ItemDistributionDto
        {
            public string ItemType { get; set; }
            public int Count { get; set; }
        }

        /// <summary>
        /// ActivitySummaryDto represents a summary of user activities by type.
        /// </summary>
        public class ActivitySummaryDto
        {
            public string ActivityType { get; set; }
            public int Count { get; set; }
        }


        /// <summary>
        /// VaultSizeHistoryDto represents the size of the vault for a specific month.
        /// </summary>
        public class VaultSizeHistoryDto
        {
            public string Month { get; set; }
            public double SizeInMB { get; set; }
        }

        /// <summary>
        /// VaultSizeReportDto represents the overall vault size report including current size, monthly average, and historical data.
        /// </summary>
        public class VaultSizeReportDto
        {
            public double CurrentSizeInMB { get; set; }
            public double MonthlyAverageInMB { get; set; }
            public List<VaultSizeHistoryDto> MonthlyData { get; set; }
        }


        /// <summary>
        /// FullReportDto aggregates all the different report sections into a single DTO for the main reports page.
        /// </summary>
        public class FullReportDto
        {
            public List<ItemDistributionDto> ItemDistribution { get; set; }
            public List<UserActivityDto> AllUserActivity { get; set; }
            public List<ActivitySummaryDto> ActivitySummary { get; set; }
            public VaultSizeReportDto VaultSizeReport { get; set; }
        }
    }
}
