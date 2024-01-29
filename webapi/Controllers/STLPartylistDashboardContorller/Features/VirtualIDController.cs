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
    public class VirtualIDController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IVirtualIDRepository _repo;
        public VirtualIDController(IConfiguration config, IVirtualIDRepository repo)
        {
            _config = config;
            _repo = repo;
        }

        [HttpPost]
        [Route("virtualid/generate")]
        public async Task<IActionResult> generateVirtualID([FromBody] VirtualID param)
        {
            var valResult = await validityReport(param);
            if (valResult.result == Results.Failed)
                return Ok(new { Status = "error", Message = valResult.message });
            if (valResult.result != Results.Success)
                return NotFound();

            var result = await _repo.generateVirtualID(param);
            if (result.result == Results.Success)
                return Ok(new { result = result.result, message = result.message });
            else if (result.result == Results.Failed)
                return Ok(new { result = result.result, message = result.message });
            return NotFound();
        }

        private async Task<(Results result, string message)> validityReport(VirtualID param)
        {
            if (param == null)
                return (Results.Null, null);
            if (param.FrontIdImageUrl.IsEmpty() || param.BackIdImageUrl.IsEmpty())
                return (Results.Success, null);
            byte[] bytes1 = Convert.FromBase64String(param.FrontIdImageUrl.Str());
            byte[] bytes2 = Convert.FromBase64String(param.BackIdImageUrl.Str());
            List<byte[]> bytes = new List<byte[]>();
            bytes.Add(bytes1);
            bytes.Add(bytes2);
            if (bytes.Count < 2)
                return (Results.Failed, "Make sure selected document path is invalid.");
            var res1 = await ImgService.SendAsync(bytes1);
            var res2 = await ImgService.SendAsync(bytes2);
            bytes1.Clear();
            bytes2.Clear();
            if (res1 == null && res2 == null)
                return (Results.Failed, "Please contact to admin.");
            var json1 = JsonConvert.DeserializeObject<Dictionary<string, object>>(res1);
            var json2 = JsonConvert.DeserializeObject<Dictionary<string, object>>(res2);
            if (json1["status"].Str() != "error" || json2["status"].Str() != "error")
            {
                //request.URLDocument = (json["url"].Str()).Replace(_config["Portforwarding:LOCAL"].Str(), _config["Portforwarding:URL"].Str());
                param.FrontIdImageUrl = (json1["url"].Str().Replace("www.", "")).Replace(_config["Portforwarding:LOCAL"].Str(), _config["Portforwarding:URL"].Str());
                param.BackIdImageUrl = json2["url"].Str().Replace("www.", "").Replace(_config["Portforwarding:LOCAL"].Str(), _config["Portforwarding:URL"].Str());

                return (Results.Success, null);
            }
            return (Results.Null, "Make sure selected image is invalid");
        }
    }
}
