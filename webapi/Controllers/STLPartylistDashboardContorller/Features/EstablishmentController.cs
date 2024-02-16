using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using webapi.App.Aggregates.Common;
using webapi.App.Aggregates.STLPartylistDashboard.Features;
using webapi.App.Aggregates.SubscriberAppAggregate.Common;
using webapi.App.RequestModel.AppRecruiter;
using webapi.App.RequestModel.Common;
using Comm.Commons.Extensions;
using webapi.App.Features.UserFeature;
using Newtonsoft.Json;

namespace webapi.Controllers.STLPartylistDashboardContorller.Features
{
    [Route("app/v1/stldashboard")]
    [ApiController]
    [ServiceFilter(typeof(SubscriberAuthenticationAttribute))]
    public class EstablishmentController:ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IEstablishmentRepository _repo;
        public EstablishmentController(IConfiguration config, IEstablishmentRepository repo)
        {
            _config = config;
            _repo = repo;
        }
        [HttpPost]
        [Route("establishment/list")]
        public async Task<IActionResult> Task0a(Establishment_Request req)
        {
            var result = await _repo.Load_Establishment(req);
            if (result.result == Results.Success)
                return Ok(result.est);
            return NotFound();
        }
        [HttpPost]
        [Route("establishment/new")]
        public async Task<IActionResult> Task0b([FromBody] Establishment req)
        {
            var valresult = await validity(req);
            if (valresult.result == Results.Failed)
                return Ok(new { Status = "error", message = valresult.message });
            else if (valresult.result != Results.Success)
                return NotFound();

            var result = await _repo.EstablishmentAsync(req);
            if (result.result == Results.Success)
                return Ok(new { Status = "ok", message = result.message, Establishment_ID = result.estid, Logo = req.Company_Logo });
            else if (result.result == Results.Failed)
                return Ok(new { Status = "error", message = result.message, Logo = req.Company_Logo });
            return NotFound();
        }
        [HttpPost]
        [Route("establishment/edit")]
        public async Task<IActionResult> Task0c([FromBody] Establishment req)
        {
            var valresult = await validity(req);
            if (valresult.result == Results.Failed)
                return Ok(new { Status = "error", message = valresult.message });
            else if (valresult.result != Results.Success)
                return NotFound();

            var result = await _repo.EstablishmentAsync(req, true);
            if (result.result == Results.Success)
                return Ok(new { Status = "ok", message = result.message, Establishment_ID = result.estid, Logo = req.Company_Logo });
            else if (result.result == Results.Failed)
                return Ok(new { Status = "error", message = result.message, Logo = req.Company_Logo });
            return NotFound();
        }

        private async Task<(Results result, string message)> validity(Establishment request)
        {
            if (request == null)
                return (Results.Null, null);
            //if (!request.CompanyLogo.IsEmpty())
            //    return (Results.Success, null);
            if (request.CompanyLogo.IsEmpty())
            {
                request.Company_Logo = "";
                return (Results.Success, null);
            }
            if (request.CompanyLogo.StartsWith("http"))
            {
                request.Company_Logo = request.CompanyLogo;
                return (Results.Success, null);
            }
           
            
            else
            {
                byte[] bytes = Convert.FromBase64String(request.CompanyLogo.Str());
                if (bytes.Length == 0)
                {
                    request.Company_Logo = "";
                    return (Results.Success, null);
                }
                var res = await ImgService.SendAsync(bytes);
                bytes.Clear();
                if (res == null)
                    return (Results.Failed, "Please contact to admin.");
                var json = JsonConvert.DeserializeObject<Dictionary<string, object>>(res);
                if (json["status"].Str() != "error")
                {
                    //request.Company_Logo = json["url"].Str();
                    request.Company_Logo = (json["url"].Str()).Replace(_config["Portforwarding:LOCAL"].Str(), _config["Portforwarding:URL"].Str()).Replace("https", "http");
                    return (Results.Success, null);
                }
            }
            
            return (Results.Null, "Make sure selected image is invalid");
        }
    }
}
