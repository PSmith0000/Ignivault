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
        public class ItemDistributionDto
        {
            public string ItemType { get; set; }
            public int Count { get; set; }
        }

        public class ActivitySummaryDto
        {
            public string ActivityType { get; set; }
            public int Count { get; set; }
        }

        public class VaultSizeHistoryDto
        {
            public string Month { get; set; }
            public double SizeInMB { get; set; }
        }

        public class VaultSizeReportDto
        {
            public double CurrentSizeInMB { get; set; }
            public double MonthlyAverageInMB { get; set; }
            public List<VaultSizeHistoryDto> MonthlyData { get; set; }
        }

        public class FullReportDto
        {
            public List<ItemDistributionDto> ItemDistribution { get; set; }
            public List<UserActivityDto> AllUserActivity { get; set; }
            public List<ActivitySummaryDto> ActivitySummary { get; set; }
            public VaultSizeReportDto VaultSizeReport { get; set; }
        }
    }
}
