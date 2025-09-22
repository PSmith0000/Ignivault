using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ignivault.Shared.DTOs.Reports;

namespace ignivault.ApiClient.Admin.Reports
{
    public interface IReportsApiClient
    {
        Task<ApiResponse<FullReportDto>> GetReportsAsync();
    }

    public class ReportsApiClient : IReportsApiClient
    {
        private readonly HttpClient _httpClient;
        public ReportsApiClient(HttpClient httpClient) { _httpClient = httpClient; }

        public async Task<ApiResponse<FullReportDto>> GetReportsAsync()
        {
            var report = await _httpClient.GetFromJsonAsync<FullReportDto>("api/reports");
            return new ApiResponse<FullReportDto> { IsSuccess = true, Data = report };
        }
    }
}
