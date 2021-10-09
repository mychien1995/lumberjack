using System.Collections.Generic;
using Lumberjack.Server.Models;
using Lumberjack.Server.Models.Common;
using Lumberjack.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lumberjack.Server.Controllers
{
    [ApiController]
    [Route("api/admin/[controller]")]
    public class ShardsController : AdminController
    {
        private readonly IShardManager _shardManager;

        public ShardsController(IShardManager shardManager)
        {
            _shardManager = shardManager;
        }

        [HttpGet]
        public IActionResult GetAll()
            => new ObjectResult(new DataResponse<List<ShardModel>>(_shardManager.GetShards()));
    }
}