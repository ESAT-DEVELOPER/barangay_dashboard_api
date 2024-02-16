using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using webapi.App.Aggregates.Common;
using webapi.App.Aggregates.STLPartylistDashboard.Features;
using webapi.App.STLDashboardModel;
using Comm.Commons.Extensions;
using webapi.App.Features.UserFeature;
using Newtonsoft.Json;
using webapi.App.Aggregates.SubscriberAppAggregate.Common;
using webapi.App.RequestModel.Common;

namespace webapi.Controllers.SubscriberAppControllers.Features
{
    [Route("app/v1/stldashboard")]
    [ApiController]
    [ServiceFilter(typeof(SubscriberAuthenticationAttribute))]
    public class MemorandumController:ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IMemorandumRepository _repo;
        public MemorandumController(IConfiguration config, IMemorandumRepository repo)
        {
            _config = config;
            _repo = repo;
        }
        [HttpPost]
        [Route("memo")]
        public async Task<IActionResult> Task04([FromBody] FilterRequest request)
        {
            var result = await _repo.LoadMemorandum(request);
            if (result.result == Results.Success)
                return Ok(result.memo);
            return NotFound();
        }

        [HttpPost]
        [Route("memo/new")]
        public async Task<IActionResult> Task01([FromBody] Memorandum request)
        {
            var valResult = await validity(request);
            if (valResult.result == Results.Failed)
                return Ok(new { Status = "error", Message = valResult.message });
            if (valResult.result != Results.Success)
                return NotFound();

            var repoResult = await _repo.MemorandumAsync(request);
            if (repoResult.result == Results.Success)
                return Ok(new { Status = "ok", MemoID=repoResult.memoid, MemoURL=request.MemorandumURL, Message = repoResult.message });
            else if (repoResult.result == Results.Failed)
                return Ok(new { Status = "error", Message = repoResult.message });
            return NotFound();
        }
        [HttpPost]
        [Route("memo/edit")]
        public async Task<IActionResult> Task02([FromBody] Memorandum request)
        {
            var valResult = await validity(request);
            if (valResult.result == Results.Failed)
                return Ok(new { Status = "error", Message = valResult.message });
            if (valResult.result != Results.Success)
                return NotFound();

            var repoResult = await _repo.UpdateMemorandumAsync(request);
            if (repoResult.result == Results.Success)
                return Ok(new { Status = "ok", MemoID = request.MemoId, MemoURL = request.MemorandumURL, Message = repoResult.message });
            else if (repoResult.result == Results.Failed)
                return Ok(new { Status = "error", Message = repoResult.message });
            return NotFound();
        }

        private async Task<(Results result, string message)> validity(Memorandum request)
        {
            if (request == null)
                return (Results.Null, null);
            if (request.Attachment.IsEmpty())
                return (Results.Success, null);
            byte[] bytes = Convert.FromBase64String(request.Attachment.Str());
            if (bytes.Length == 0)
                return (Results.Failed, "Make sure selected image is invalid.");
            var res = await PDFService.SendAsync(bytes);
            bytes.Clear();
            if (res == null)
                return (Results.Failed, "Please contact to admin.");
            var json = JsonConvert.DeserializeObject<Dictionary<string, object>>(res);
            if (json["status"].Str() != "error")
            {
                request.MemorandumURL = (json["url"].Str()).Replace(_config["Portforwarding:LOCAL"].Str(), _config["Portforwarding:URL"].Str()).Replace("https", "http");
                return (Results.Success, null);
            }
            return (Results.Null, "Make sure selected image is invalid");
        }
    }
}
