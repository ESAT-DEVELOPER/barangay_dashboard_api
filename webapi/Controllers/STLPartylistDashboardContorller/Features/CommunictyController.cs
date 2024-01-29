using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using webapi.App.Aggregates.SubscriberAppAggregate.Common;
using Microsoft.Extensions.Configuration;
using webapi.App.Aggregates.STLPartylistDashboard.Features;
using webapi.App.STLDashboardModel;
using webapi.App.Aggregates.Common;
using webapi.App.RequestModel.Common;

namespace webapi.Controllers.STLPartylistDashboardContorller.Features
{
    [Route("app/v1/stldashboard")]
    [ApiController]
    [ServiceFilter(typeof(SubscriberAuthenticationAttribute))]
    public class CommunictyController:ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly ICommunityRepository _repo;
        public CommunictyController(IConfiguration config, ICommunityRepository repo)
        {
            _config = config;
            _repo = repo;
        }
        [HttpPost]
        [Route("community/new")]
        public async Task<IActionResult> SendCommunityAsync([FromBody] cCommunity request)
        {
            var result = await _repo.SendRequestCommunityAsync(request);
            if (result.result == Results.Success)
            {
                return Ok(new { Status = "ok", Message = result.message, Content = request });
            }
            else if (result.result == Results.Failed)
                return Ok(new { Status = "error", Message = result.message, Content = request });
            return NotFound();
        }
        [HttpPost]
        [Route("community/update")]
        public async Task<IActionResult> SendUpdateRequestCommunityAsync([FromBody] cCommunity request)
        {
            var result = await _repo.SendUpdateRequestCommunityAsync(request);
            if (result.result == Results.Success)
            {
                return Ok(new { Status = "ok", Message = result.message, Content = request });
            }
            else if (result.result == Results.Failed)
                return Ok(new { Status = "error", Message = result.message, Content = request });
            return NotFound();
        }
        [HttpPost]
        [Route("community/accept")]
        public async Task<IActionResult> AcceptRequestCommunityAsync([FromBody] AcptCommunity request)
        {
            var result = await _repo.AcceptRequestCommunityAsync(request);
            if (result.result == Results.Success)
            {
                return Ok(new { Status = "ok", Message = result.message });
            }
            else if (result.result == Results.Failed)
                return Ok(new { Status = "error", Message = result.message });
            return NotFound();
        }
        [HttpPost]
        [Route("community/list")]
        public async Task<IActionResult> LoadCommunityListAsync([FromBody] FilterRequest request)
        {
            var result = await _repo.LoadCommunityListAsync(request);
            if (result.result == Results.Success)
            {
                return Ok(result.comm);
            }
            return NotFound();
        }
    }
}
