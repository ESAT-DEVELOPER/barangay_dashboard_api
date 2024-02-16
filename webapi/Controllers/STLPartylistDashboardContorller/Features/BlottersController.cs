using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using webapi.App.Aggregates.Common;
using webapi.App.Aggregates.STLPartylistDashboard;
using webapi.App.Aggregates.STLPartylistDashboard.Features;
using webapi.App.Aggregates.SubscriberAppAggregate.Common;
using webapi.App.STLDashboardModel;
using Comm.Commons.Extensions;
using Newtonsoft.Json;
using System;
using webapi.App.Features.UserFeature;
using System.Collections.Generic;
using webapi.App.RequestModel.Common;
using System.Text;

namespace webapi.Controllers.STLPartylistDashboardContorller.Features
{
    [Route("app/v1/stldashboard")]
    [ApiController]
    [ServiceFilter(typeof(SubscriberAuthenticationAttribute))]
    public class BlotterController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IBlottersRepository _repo;
        private readonly IAccountRepository _loginrepo;

        public BlotterController(IConfiguration config, IBlottersRepository repo, IAccountRepository loginrepo)
        {
            _repo = repo;
            _config = config;
            _loginrepo = loginrepo;
        }

        [HttpPost]
        [Route("blotter/save")]
        public async Task<IActionResult> SaveBlotter([FromBody] Blotter info)
        {
            var valResult = await validityReport(info);
            if (valResult.result == Results.Failed)
                return Ok(new { Status = "error", Message = valResult.message });
            if (valResult.result != Results.Success)
                return NotFound();

            var result = await _repo.SaveBlotterV2(info);
            if (result.result == Results.Success)
                return Ok(new { result = result.result, message = result.message });
            return NotFound();
        }

        [HttpPost]
        [Route("blotter/update")]
        public async Task<IActionResult> UpdateBlotter([FromBody] Blotter info)
        {
            var valResult = await validityReport(info);
            if (valResult.result == Results.Failed)
                return Ok(new { Status = "error", Message = valResult.message });
            if (valResult.result != Results.Success)
                return NotFound();

            //var result = await _repo.UpdateBlotter(info);
            //if (result.result == Results.Success)
            //    return Ok(new { result = result.result, message = result.message });
            //return NotFound();

            var result = await _repo.UpdateBlotterV2(info);
            if (result.result == Results.Success)
                return Ok(new { result = result.result, message = result.message });
            return NotFound();
        }
        [HttpPost]
        [Route("blotter/received")]
        public async Task<IActionResult> ReceivedBlotter([FromBody] Blotter info)
        {
            var result = await _repo.BlotterReceivedAsync(info);
            if (result.result == Results.Success)
                return Ok(new { result = result.result, message = result.message });
            return NotFound();
        }

        //[HttpPost]
        //[Route("blotter/load")]
        //public async Task<IActionResult> LoadBlotter(int id, int row, string from, string to)
        //{
        //    var result = await _repo.LoadBlotter(id,row,from, to);
        //    if (result.result == Results.Success)
        //        return Ok(result.blotter);
        //    return NotFound();
        //}

        [HttpPost]
        [Route("blotter/load")]
        public async Task<IActionResult> LoadBlotter(int status)
        {
            var result = await _repo.LoadBlotterV2(status);
            if (result.result == Results.Success)
                return Ok(result.blotter);
            return NotFound();
        }
        [HttpPost]
        [Route("blotter/notification")]
        public async Task<IActionResult> LoadBlotterNotification([FromBody] FilterRequest req)
        {
            var result = await _repo.LoadBlotterNotification(req);
            var res = await _repo.TotalBlotterNotificationAsync(req);
            if (result.result == Results.Success)
                return Ok(new { Status = "ok", blotter = result.blotter, totalblotter = res.total_blotter });
            if (result.result == Results.Failed)
                return Ok(new { Status = "error", blotter = result.blotter, totalblotter = res.total_blotter });
            return NotFound();
        }

        [HttpPost]
        [Route("blotter/summon/load")]
        public async Task<IActionResult> LoadSummon(int row, string from, string to, bool isSummon = true)
        {
            var result = await _repo.LoadSummon(row,from,to);
            if (result.result == Results.Success)
                return Ok(result.summon);
            return NotFound();
        }

        [HttpPost]
        [Route("blotter/summon/save")]
        public async Task<IActionResult> SaveSummon([FromBody] Blotter info, bool isSummon = true)
        {
            //var valResult = await validityReport(info);
            //if (valResult.result == Results.Failed)
            //    return Ok(new { Status = "error", Message = valResult.message });
            //if (valResult.result != Results.Success)
            //    return NotFound();

            var result = await _repo.SaveSummon(info);
            if (result.result == Results.Success)
                return Ok(new { result = result.result, message = result.message });
            return NotFound();
        }

        [HttpPost]
        [Route("blotter/summon/update")]
        public async Task<IActionResult> UpdateSummon([FromBody] Blotter info, bool isSummon = true)
        {
            //var valResult = await validityReport(info);
            //if (valResult.result == Results.Failed)
            //    return Ok(new { Status = "error", Message = valResult.message });
            //if (valResult.result != Results.Success)
            //    return NotFound();

            var result = await _repo.UpdateSummon(info);
            if (result.result == Results.Success)
                return Ok(new { result = result.result, message = result.message });
            return NotFound();
        }

        [HttpPost]
        [Route("blotter/summon/resolve")]
        public async Task<IActionResult> ResolveSummon([FromBody] Blotter info)
        {
            var result = await _repo.ResolveSummon(info);
            if (result.result == Results.Success)
                return Ok(new { result = result.result, message = result.message });
            return NotFound();
        }

        [HttpPost]
        [Route("blotter/summon/unsummon")]
        public async Task<IActionResult> RemoveSummon([FromBody] Blotter info, bool isSummon = true)
        {
            var result = await _repo.RemoveSummon(info);
            if (result.result == Results.Success)
                return Ok(new { result = result.result, message = result.message });
            return NotFound();
        }

        [HttpPost]
        [Route("blotter/getbrgycpt")]
        public async Task<IActionResult> GetBrgyCpt(string plid, string pgrpid)
        {
            var result = await _repo.GetBrgyCpt(plid, pgrpid);
            if (result.result == Results.Success)
                return Ok(result.brgycpt);
            return NotFound();
        }

        [HttpPost]
        [Route("blotter/controlno")]
        public async Task<IActionResult> GetControlNo()
        {
            var result = await _repo.GetControlNo();
            if (result.result == Results.Success)
                return Ok(result.caseNo);
            return NotFound();
        }

        [HttpPost]
        [Route("blotter/reprint")]
        public async Task<IActionResult> Reprint(string plid, string pgrpid, string brgycsno, string colname)
        {
            var result = await _repo.Reprint(plid, pgrpid, brgycsno, colname);
            if (result.result == Results.Success)
                return Ok(result.docpath);
            return NotFound();
        }

        [HttpPost]
        [Route("blotter/summon/signatures")]
        public async Task<IActionResult> GetSignature(string plid, string pgrpid)
        {
            var result = await _repo.GetSignature(plid, pgrpid);
            if (result.result == Results.Success)
                return Ok(result.signatures);
            return NotFound();
        }

        [HttpPost]
        [Route("blotter/summon/header")]
        public async Task<IActionResult> GetHeader(string plid, string pgrpid)
        {
            var result = await _repo.GetHeader(plid, pgrpid);
            if (result.result == Results.Success)
                return Ok(result.header);
            return NotFound();
        }

        [HttpPost]
        [Route("blotter/summon/getallresidents")]
        public async Task<IActionResult> GetAllResidents()
        {
            var result = await _repo.GetAllResidents();
            if (result.result == Results.Success)
                return Ok(result.residents);
            return NotFound();
        }

        private async Task<(Results result, String message)> validityReport(Blotter request)
        {
            /*
            List<string> tempList = new List<string>();
            if (request == null)
                return (Results.Null, null);
            if (request.Attachments.Count < 1)
                return (Results.Success, null);
            byte[] bytes = null;
            foreach (var item in request.Attachments)
            {
                bytes = Convert.FromBase64String(item);
                if (bytes.Length == 0)
                    return (Results.Failed, "Make sure selected document path is invalid.");
                var res = await ImgService.SendAsync(bytes);
                bytes.Clear();
                if (res == null)
                    return (Results.Failed, "Please contact to admin.");
                var json = JsonConvert.DeserializeObject<Dictionary<string, object>>(res);
                if (json["status"].Str() != "error")
                    tempList.Add(json["url"].Str().Replace("www.",""));
            }
            request.Attachments = null;
            request.Attachments = tempList;
            return (Results.Success, request.Attachments);
            */
            //Robert
            if (request == null)
                return (Results.Null, null);
            if (request.Attachments == null || request.Attachments.Count < 1)
                return (Results.Success, null);
            StringBuilder sb = new StringBuilder();
            for(int i = 0; i <request.Attachments.Count; i++)
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
                        string url = (json["url"].Str()).Replace("https", "http");
                        sb.Append($"<item LNK_URL=\"{ url }\" />");
                        request.Attachments[i] = url;
                    }
                    else return (Results.Failed, "Make sure selected image is valid.");
                }
            }
            if (sb.Length > 0)
            {
                request.iAttachments = sb.ToString();
                return (Results.Success, null);
            }
            return (Results.Failed, "Make sure uploaded image is valid.");

            //if (request == null)
            //    return (Results.Null, null);
            //if (request.ReportPath.IsEmpty())
            //    return (Results.Success, null);
            //byte[] bytes = Convert.FromBase64String(request.ReportPath.Str());
            //if (bytes.Length == 0)
            //    return (Results.Failed, "Make sure selected document path is invalid.");
            //var res = await ReportService.SendAsync(bytes, request.Docname);
            //bytes.Clear();
            //if (res == null)
            //    return (Results.Failed, "Please contact to admin.");
            //var json = JsonConvert.DeserializeObject<Dictionary<string, object>>(res);
            //if (json["status"].Str() != "error")
            //{
            //    request.ReportPath = json["url"].Str();

            //    return (Results.Success, null);
            //}
            //return (Results.Null, "Make sure selected image is invalid");
        }

        [HttpPost]
        [Route("document/template")]
        public async Task<IActionResult> GetDocumentTemplate(string docname)
        {
            var result = await _repo.GetDocumentTemplate(docname);
            if (result.result == Results.Success)
                return Ok(result.document);
            return NotFound();
        }

        [HttpPost]
        [Route("blotter/complaint")]
        public async Task<IActionResult> Complaint(string caseno, string createby, string createdate)
        {
            var result = await _repo.Complaint(caseno, createby, createdate);
            if (result.result == Results.Success)
                return Ok(new { result = result.result, message = result.message });
            return NotFound();
        }

        [HttpPost]
        [Route("blotter/cancel")]
        public async Task<IActionResult> Cancel(string caseno, string reason, string createby, string createdate)
        {
            var result = await _repo.Cancel(caseno, reason, createby, createdate);
            if (result.result == Results.Success)
                return Ok(new { result = result.result, message = result.message });
            return NotFound();
        }

        [HttpPost]
        [Route("blotter/report")]
        public async Task<IActionResult> LoadReport()
        {
            var result = await _repo.LoadReport();
            if (result.result == Results.Success)
                return Ok(result.report);
            return NotFound();
        }

        [HttpPost]
        [Route("case/load")]
        public async Task<IActionResult> LoadCaseIdentifier(string name)
        {
            var result = await _repo.LoadCaseIdentifier(name);
            if (result.result == Results.Success)
                return Ok(result.report);
            return NotFound();
        }

        [HttpPost]
        [Route("blotter/save/attachment")]
        public async Task<IActionResult> SaveAttachment(Blotter info)
        {
            var valResult = await validityReport(info);
            if (valResult.result != Results.Success)
                return NotFound();

            //return Ok(valResult.attachments);
            return Ok(null);
        }

        [HttpPost]
        [Route("blotter/load/attachment")]
        public async Task<IActionResult> LoadAttachment(string caseno)
        {
            var result = await _repo.LoadAttachment(caseno);
            if (result.result == Results.Success)
                return Ok(result.attachment);
            return NotFound();
        }

        [HttpPost]
        [Route("blotter/get/complainants")]
        public async Task<IActionResult> LoadComplainants(string caseno, string complainantId)
        {
            var result = await _repo.LoadComplainants(caseno, complainantId);
            if (result.result == Results.Success)
                return Ok(result.attachment);
            return NotFound();
        }

        [HttpPost]
        [Route("blotter/get/respondents")]
        public async Task<IActionResult> LoadRespondents(string caseno, string respondentId)
        {
            var result = await _repo.LoadRespondents(caseno, respondentId);
            if (result.result == Results.Success)
                return Ok(result.attachment);
            return NotFound();
        }

        [HttpPost]
        [Route("blotter/save/form")]
        public async Task<IActionResult> SaveForm(Blotter info)
        {
            var result = await _repo.SaveForm(info);
            if (result.result == Results.Success)
                return Ok(new { result = result.result, message = result.message });
            return NotFound();
        }

        [HttpPost]
        [Route("blotter/load/form")]
        public async Task<IActionResult> LoadForm()
        {
            var result = await _repo.LoadForm();
            if (result.result == Results.Success)
                return Ok(result.template);
            return NotFound();
        }
    }
}
