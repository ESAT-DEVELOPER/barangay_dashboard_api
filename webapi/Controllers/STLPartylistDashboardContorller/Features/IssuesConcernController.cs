using Microsoft.AspNetCore.Mvc;
using webapi.App.Aggregates.Common;
using webapi.App.Aggregates.SubscriberAppAggregate.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using webapi.App.Aggregates.STLPartylistDashboard.Features;
using webapi.App.RequestModel.Common;
using webapi.App.RequestModel.Feature;
using System.Text;
using webapi.App.Features.UserFeature;
using Newtonsoft.Json;
using Comm.Commons.Extensions;

namespace webapi.Controllers.STLPartylistDashboardContorller.Features
{
    [Route("app/v1/stldashboard/concern")]
    [ApiController]
    [ServiceFilter(typeof(SubscriberAuthenticationAttribute))]
    public class IssuesConcernController:ControllerBase
    {
        private readonly IIssuesConcernRepository _supRepo;
        public IssuesConcernController(IIssuesConcernRepository supRepo)
        {
            _supRepo = supRepo;
        }

        [HttpPost]
        [Route("report/history")]
        public async Task<IActionResult> Task04([FromBody] FilterRequest request)
        {
            var result = await _supRepo.LoadIssuesConcern(request);
            if (result.result == Results.Success)
                return Ok(result.concern);
            return NotFound();
        }

        [HttpPost]
        [Route("report/problem/ticketno")]
        public async Task<IActionResult> Task08([FromBody] FilterRequest request)
        {
            var result = await _supRepo.LoadIssuesConcernTickectNo(request);
            if (result.result == Results.Success)
                return Ok(result.concern);
            return NotFound();
        }

        [HttpPost]
        [Route("report/problem/attachment")]
        public async Task<IActionResult> Task06([FromBody] ReportAProblemRequest request)
        {
            var result = await _supRepo.LoadIssuesConcernAttachment(request);
            if (result.result == Results.Success)
                return Ok(result.concern);
            return NotFound();
        }

        [HttpPost]
        [Route("report/problem/process")]
        public async Task<IActionResult> Task03([FromBody] ReportAProblemRequest request)
        {
            var repoResult = await _supRepo.ProcessIssuesConcernAsync(request);
            if (repoResult.result == Results.Success)
                return Ok(new { Status = "ok", Message = repoResult.message, Content = request });
            else if (repoResult.result == Results.Failed)
                return Ok(new { Status = "error", Message = repoResult.message });
            return NotFound();
        }

        [HttpPost]
        [Route("report/problem/closed")]
        public async Task<IActionResult> Task05([FromBody] ReportAProblemRequest request)
        {
            var valResult = await validity(request);
            if (valResult.result == Results.Failed)
                return Ok(new { Status = "error", Message = valResult.message });
            if (valResult.result != Results.Success)
                return NotFound();

            var repoResult = await _supRepo.ClosedIssuesConcernAsync(request);
            if (repoResult.result == Results.Success)
                return Ok(new { Status = "ok", Message = repoResult.message, Content = request });
            else if (repoResult.result == Results.Failed)
                return Ok(new { Status = "error", Message = repoResult.message });
            return NotFound();
        }

        [HttpPost]
        [Route("report/notification")]
        public async Task<IActionResult> Task07([FromBody] FilterRequest request)
        {
            var result = await _supRepo.LoadIssuesConcern1(request);
            var res = await _supRepo.TotalIssuesConcernAsync(request);

            if (result.result == Results.Success)
                return Ok(new { Status = "ok", loadissuesconcern = result.concern, totalissuesconcern = res.total_concern });
            if (result.result == Results.Failed)
                return Ok(new { Status = "error", loadissuesconcern = result.concern, totalissuesconcern = res.total_concern });
            return NotFound();
        }

        private async Task<(Results result, string message)> validity(ReportAProblemRequest request)
        {
            if (request == null)
                return (Results.Null, null);
            //if(!request.ImageUrl.IsEmpty())
            //    return (Results.Success, null);

            if (request.Attachments == null || request.Attachments.Count < 1)
                return (Results.Success, null);
            //var attachments = request.Attachments;
            StringBuilder sb = new StringBuilder();
            //request.iAttachments = "";
            for (int i = 0; i < request.Attachments.Count; i++)
            {
                var attachment = request.Attachments[i];
                if (attachment.IsEmpty()) continue;
                if (attachment.StartsWith("http"))
                {
                    sb.Append($"<item LNK_URL=\"{ attachment }\" />");
                }
                else
                {
                    byte[] bytes = Convert.FromBase64String(attachment.Str());
                    if (bytes.Length == 0)
                        return (Results.Failed, "Make sure selected image is valid.");

                    var res = await ImgService.SendAsync(bytes);
                    bytes.Clear();
                    if (res == null)
                        return (Results.Failed, "Please contact to admin.");

                    var json = JsonConvert.DeserializeObject<Dictionary<string, object>>(res);
                    if (json["status"].Str() != "error")
                    {
                        string url = (json["url"].Str()).Replace(_config["Portforwarding:LOCAL"].Str(), _config["Portforwarding:URL"].Str()).Replace("https", "http");
                        sb.Append($"<item LNK_URL=\"{ url }\" />");
                        request.Attachments[i] = url;
                    }
                    else return (Results.Failed, "Make sure selected image is valid.");
                }
            }
            if (sb.Length > 0)
            {
                request.iAttachments = sb.ToString();
                //request.Attachments = null;
                return (Results.Success, null);
            }
            return (Results.Failed, "Make sure selected image is valid.");
        }
    }
}
