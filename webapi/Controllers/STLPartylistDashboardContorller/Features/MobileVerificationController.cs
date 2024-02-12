using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using webapi.App.Aggregates.Common;
using webapi.App.Aggregates.STLPartylistDashboard;
using webapi.App.Aggregates.STLPartylistDashboard.Features;
using webapi.App.Aggregates.SubscriberAppAggregate.Common;
using webapi.App.Features.UserFeature;
using webapi.App.STLDashboardModel;
using Comm.Commons.Extensions;
using Newtonsoft.Json;

namespace webapi.Controllers.STLPartylistDashboardContorller.Features
{
    [Route("app/v1/stldashboard")]
    [ApiController]
    [ServiceFilter(typeof(SubscriberAuthenticationAttribute))]
    public class MobileVerificationController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IMobileVerificationRepository _repo;
        public MobileVerificationController(IConfiguration config, IMobileVerificationRepository repo)
        {
            _config = config;
            _repo = repo;
        }

        [HttpPost]
        [Route("mobile/verification/approve")]
        public async Task<IActionResult> uploadArticle(string userid)
        {
            var result = await _repo.approveMobileVerification(userid);
            if (result.result == Results.Success)
                return Ok(new { result = result.result, message = result.message });
            else if (result.result == Results.Failed)
                return Ok(new { result = result.result, message = result.message });
            return NotFound();
        }

        [HttpPost]
        [Route("mobile/verification/list")]
        public async Task<IActionResult> getMobileVerificationList(string userid, string date)
        {
            var result = await _repo.getMobileVerficationList(userid,date);
            if (result.result == Results.Success)
                return Ok(result.articles);
            return NotFound();
        }
    }
}
