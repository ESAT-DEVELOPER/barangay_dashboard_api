using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using webapi.App.Aggregates.Common;
using webapi.App.Aggregates.STLPartylistDashboard.Features;
using webapi.App.RequestModel.AppRecruiter;

namespace webapi.Controllers.STLPartylistDashboardContorller.Features
{
    [Route("app/v1/stldashboard")]
    public class FetchNewsController:ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IFetchNewsRepository _repo;
        public FetchNewsController(IConfiguration config, IFetchNewsRepository repo)
        {
            _config = config;
            _repo = repo;
        }
        [HttpPost]
        [Route("news/fetch")]
        public async Task<IActionResult> Task0a([FromBody] FetchLoadNews req)
        {
            var result = await _repo.FetchNewsAsync(req);
            if (result.result == Results.Success)
                return Ok(new { result = result.result, message = result.message });
            return NotFound();
        }
    }
}
