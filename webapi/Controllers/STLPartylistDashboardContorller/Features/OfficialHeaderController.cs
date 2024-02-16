using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Comm.Commons.Extensions;
using webapi.App.Aggregates.Common;
using webapi.App.Aggregates.STLPartylistDashboard.Features;
using webapi.App.Aggregates.SubscriberAppAggregate.Common;
using webapi.App.RequestModel.AppRecruiter;
using webapi.App.Features.UserFeature;
using Newtonsoft.Json;

namespace webapi.Controllers.STLPartylistDashboardContorller.Features
{
    [Route("app/v1/stldashboard")]
    [ApiController]
    [ServiceFilter(typeof(SubscriberAuthenticationAttribute))]
    public class OfficialHeaderController:ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IOfficialHeaderRepository _repo;
        public OfficialHeaderController(IConfiguration config, IOfficialHeaderRepository repo)
        {
            _config = config;
            _repo = repo;
        }
        [HttpPost]
        [Route("officiallogo")]
        public async Task<IActionResult> Task01()
        {
            var result = await _repo.LoadOfficialLogo();
            if (result.result == Results.Success)
                return Ok(result.ofllogo);
            return NotFound();
        }
        [HttpPost]
        [Route("officiallogo/setup")]
        public async Task<IActionResult> Task02([FromBody] OfficialHeader request)
        {
            var valResult = await validity(request);
            if (valResult.result == Results.Failed)
                return Ok(new { Status = "error", Message = valResult.message });
            //if (valResult.result != Results.Success)
            //    return NotFound();

            var repoResult = await _repo.OfficialHaeaderAsyn(request);
            if (repoResult.result == Results.Success)
                return Ok(new { Status = "ok", Message = repoResult.message });
            else if (repoResult.result == Results.Failed)
                return Ok(new { Status = "error", Message = repoResult.message });
            return NotFound();
        }


        private async Task<(Results result, string message)> validity(OfficialHeader request)
        {
            if (request == null)
                return (Results.Null, null);
            if (request.iBrgyOfficialLogo.IsEmpty() && request.BrgyLogoChange == 1)
                return (Results.Failed, "Please select an Barangay Official Logo.");
            if (request.iMunicipalLogo.IsEmpty() && request.MunLogoChange == 1)
                return (Results.Failed, "Please select Municipality/ Cities Logo");

            if ((!request.iBrgyOfficialLogo.IsEmpty() && request.BrgyLogoChange == 1) || (!request.iMunicipalLogo.IsEmpty() && request.MunLogoChange == 1))
            {
                
                if(!request.iBrgyOfficialLogo.IsEmpty() && request.BrgyLogoChange == 1)
                {
                    byte[] brgybytes = Convert.FromBase64String(request.iBrgyOfficialLogo.Str());
                    if (brgybytes.Length == 0)
                        return (Results.Failed, "Make sure selected image is not invalid.");
                    var resbrgy = await ImgService.SendAsync(brgybytes);
                    brgybytes.Clear();
                    if (resbrgy == null)
                        return (Results.Failed, "Please contact to admin");
                    var jsonbrgy = JsonConvert.DeserializeObject<Dictionary<string, object>>(resbrgy);
                    //if(jsonbrgy["status"].Str() != "error")
                    //    request.BrgyOfficialLogo = jsonbrgy["url"].Str();
                    if(jsonbrgy["status"].Str() != "error")
                    {
                        //request.BrgyOfficialLogo = jsonbrgy["url"].Str();
                        request.BrgyOfficialLogo = (jsonbrgy["url"].Str()).Replace(_config["Portforwarding:LOCAL"].Str(), _config["Portforwarding:URL"].Str()).Replace("https", "http");
                    }
                }
                if (!request.iMunicipalLogo.IsEmpty() && request.MunLogoChange == 1)
                {
                    byte[] munbytes = Convert.FromBase64String(request.iMunicipalLogo.Str());
                    if (munbytes.Length == 0)
                        return (Results.Failed, "Make sure selected image is not invalid.");
                    var resmun = await ImgService.SendAsync(munbytes);
                    munbytes.Clear();
                    if (resmun == null)
                        return (Results.Failed, "Please contact to admin");
                    var jsonmun = JsonConvert.DeserializeObject<Dictionary<string, object>>(resmun);
                    if (jsonmun["status"].Str() != "error")
                    {
                        //request.MunicipalLogo = jsonmun["url"].Str();
                        request.MunicipalLogo = (jsonmun["url"].Str()).Replace(_config["Portforwarding:LOCAL"].Str(), _config["Portforwarding:URL"].Str()).Replace("https", "http");
                    }
                }
                return (Results.Success, null);


            }
            return (Results.Null, "Make sure Your selected Barangay and Municipality/ Cities Logo is not invalid");
        }
    }
}
