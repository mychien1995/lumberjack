using System;
using System.Threading.Tasks;
using Lumberjack.Server.Models;
using Lumberjack.Server.Models.Common;
using Lumberjack.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lumberjack.Server.Controllers
{
    [ApiController]
    [Route("api/admin/[controller]")]
    public class ApplicationsController : AdminController
    {
        private readonly IApplicationManager _applicationManager;

        public ApplicationsController(IApplicationManager applicationManager)
        {
            _applicationManager = applicationManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
            => new ObjectResult(new DataResponse<SearchResult<ApplicationModel>>(await _applicationManager.GetAll()));

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute]Guid id)
            => new ObjectResult(new DataResponse<ApplicationModel>(await _applicationManager.GetById(id)));

        [HttpPost]
        public async Task<IActionResult> Persist(ApplicationModel input)
        => new ObjectResult(new DataResponse<ApplicationModel>(await _applicationManager.Persist(input)));


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            await _applicationManager.Delete(id);
            return new ObjectResult(new DataResponse<object>(null));
        }
    }
}
