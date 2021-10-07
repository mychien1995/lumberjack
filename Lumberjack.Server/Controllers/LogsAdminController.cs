using System.Threading.Tasks;
using Lumberjack.Server.Models;
using Lumberjack.Server.Models.Common;
using Lumberjack.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lumberjack.Server.Controllers
{
    [ApiController]
    [Route("api/admin/logs")]
    public class LogsAdminController : AdminController
    {
        private readonly ILogQueryHandler _handler;

        public LogsAdminController(ILogQueryHandler handler)
        {
            _handler = handler;
        }

        [HttpPost]
        public async Task<IActionResult> Query([FromBody] LogsQuery query)
        {
            var result = await _handler.Query(query);
            return new ObjectResult(new DataResponse<SearchResult<LogEntry>>(result));
        }
    }
}
