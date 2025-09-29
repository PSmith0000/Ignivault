using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ignivault.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class ReportsController : ControllerBase
    {
        private readonly IReportsService _reportsService;
        public ReportsController(IReportsService reportsService)
        {
            _reportsService = reportsService;
        }
        /// <summary>
        /// Gets the full administrative report including item distribution, user activity, and vault size history.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetReports()
        {
            var report = await _reportsService.GenerateFullReportAsync();
            return Ok(report);
        }
    }
}
