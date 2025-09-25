using ignivault.Shared.DTOs;
using ignivault.WebAPI.Data;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using static ignivault.Shared.DTOs.Reports;

namespace ignivault.WebAPI.Services
{
    /// <summary>
    /// Defines the business logic for generating administrative reports.
    /// </summary>
    public interface IReportsService
    {
        /// <summary>
        /// Generates a comprehensive report including item distribution, user activity, and vault size history.
        /// </summary>
        /// <returns>A DTO containing all the data for the main reports page.</returns>
        Task<FullReportDto> GenerateFullReportAsync();
    }

    public class ReportsService : IReportsService
    {
        private readonly AppDbContext _context;
        public ReportsService(AppDbContext context) { _context = context; }

        public async Task<FullReportDto> GenerateFullReportAsync()
        {
            var itemDistribution = await _context.VaultItems
                .GroupBy(i => i.ItemType)
                .Select(g => new ItemDistributionDto { ItemType = g.Key.ToString(), Count = g.Count() })
                .ToListAsync();

            var allActivity = await _context.UserActivities
                    .OrderByDescending(a => a.Timestamp)
                    .Take(500)
                    .Select(a => new UserActivityDto {
                        ActivityType = a.ActivityType.ToString(),
                        Timestamp = a.Timestamp,
                        Success = a.Success,
                        IpAddress = a.IpAddress,
                        Details = a.Details
                    }).ToListAsync();

            var activitySummary = await _context.UserActivities
                .GroupBy(a => a.ActivityType)
                .Select(g => new ActivitySummaryDto { ActivityType = g.Key.ToString(), Count = g.Count() })
                .ToListAsync();

            var vaultSizeReport = await GenerateVaultSizeReportAsync();

            return new FullReportDto
            {
                ItemDistribution = itemDistribution,
                AllUserActivity = allActivity,
                ActivitySummary = activitySummary,
                VaultSizeReport = vaultSizeReport
            };
        }

        /// <summary>
        /// Generates a specific report on the vault's storage size over the last 12 months.
        /// </summary>
        /// <returns>A DTO containing historical vault size data and current statistics.</returns>
        private async Task<VaultSizeReportDto> GenerateVaultSizeReportAsync()
        {
            var twelveMonthsAgo = DateTime.UtcNow.AddMonths(-12);
            var rawData = await _context.VaultItems
                .Where(item => item.ItemType == ignivault.Shared.DTOs.Vault.VaultItemType.File
                            && item.BlobId != null
                            && item.CreatedAt >= twelveMonthsAgo)
                .Join(_context.StoredBlobs,
                      item => item.BlobId,
                      blob => blob.BlobId,
                      (item, blob) => new { item.CreatedAt, Size = blob.Data.Length })
                .GroupBy(result => new { result.CreatedAt.Year, result.CreatedAt.Month })
                .Select(group => new
                {
                    group.Key.Year,
                    group.Key.Month,
                    TotalSize = group.Sum(x => (long)x.Size)
                })
                .ToListAsync();

            var formattedData = rawData
                .OrderBy(r => r.Year)
                .ThenBy(r => r.Month)
                .Select(r => new VaultSizeHistoryDto
                {
                    Month = new DateTime(r.Year, r.Month, 1).ToString("MMM yyyy"),
                    SizeInMB = r.TotalSize / 1024.0 / 1024.0
                })
                .ToList();

            var totalSize = await _context.StoredBlobs.SumAsync(b => (long)b.Data.Length);

            return new VaultSizeReportDto
            {
                CurrentSizeInMB = totalSize / 1024.0 / 1024.0,
                MonthlyAverageInMB = formattedData.Any() ? formattedData.Average(d => d.SizeInMB) : 0,
                MonthlyData = formattedData
            };
        }
    }
}