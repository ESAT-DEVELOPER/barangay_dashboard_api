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
using System.IO;
using System.Text;

namespace webapi.Controllers.STLPartylistDashboardContorller.Features
{
    [Route("app/v1/stldashboard")]
    [ApiController]
    [ServiceFilter(typeof(SubscriberAuthenticationAttribute))]
    public class RequestDocumentController:ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IRequestDocumentRepository _repo;
        public RequestDocumentController(IConfiguration config, IRequestDocumentRepository repo)
        {
            _config = config;
            _repo = repo;
        }
        [HttpPost]
        [Route("reqdoc")]
        public async Task<IActionResult> Task01([FromBody] FilterRequest request)
        {
            var result = await _repo.LoadRequestDocument(request);
            if (result.result == Results.Success)
                return Ok(result.reqdoc);
            return NotFound();
        }

        [HttpPost]
        [Route("reqdoc/new")]
        public async Task<IActionResult> Task02([FromBody] RequestDocument request)
        {
            var valResult = await validity(request);
            if (valResult.result == Results.Failed)
                return Ok(new { Status = "error", Message = valResult.message });
            if (valResult.result != Results.Success)
                return NotFound();

            var repoResult = await _repo.RequestDocumentAsync(request);

            if (repoResult.result == Results.Success)
            {
                if (request.isFree == "1")
                {
                    FilterRequest f = new FilterRequest();
                    f.Search = repoResult.reqdocid;
                    f.num_row = "0";
                    f.Status = "1";
                    var result = await _repo.LoadRequestDocument(f);
                    return Ok(new { Status = "ok", ReqDocID = repoResult.reqdocid, requestdocument=result.reqdoc, Message = repoResult.message });
                }
                return Ok(new { Status = "ok", ReqDocID = repoResult.reqdocid, Message = repoResult.message });
            }
                
            else if (repoResult.result == Results.Failed)
                return Ok(new { Status = "error", Message = repoResult.message });
            return NotFound();
        }

        [HttpPost]
        [Route("reqdoc/newdoc")]
        public async Task<IActionResult> Task14([FromBody] RequestDocument request)
        {
            var valResult = await validity(request);
            if (valResult.result == Results.Failed)
                return Ok(new { Status = "error", Message = valResult.message });
            if (valResult.result != Results.Success)
                return NotFound();

            var repoResult = await _repo.ResidenceRequestDocumentAsync(request);

            if (repoResult.result == Results.Success)
            {
                if (request.isFree == "1")
                {
                    FilterRequest f = new FilterRequest();
                    f.Search = repoResult.reqdocid;
                    f.num_row = "0";
                    f.Status = "1";
                    var result = await _repo.LoadRequestDocument(f);
                    return Ok(new { Status = "ok", ReqDocID = repoResult.reqdocid, requestdocument = result.reqdoc, ControlNo=repoResult.controlno, Message = repoResult.message });
                }
                return Ok(new { Status = "ok", ReqDocID = repoResult.reqdocid, ControlNo=repoResult.controlno, Message = repoResult.message });
            }

            else if (repoResult.result == Results.Failed)
                return Ok(new { Status = "error", Message = repoResult.message });
            return NotFound();
        }

        [HttpPost]
        [Route("reqdoc/edit")]
        public async Task<IActionResult> Task03([FromBody] RequestDocument request)
        {
            var valResult = await validity(request);
            if (valResult.result == Results.Failed)
                return Ok(new { Status = "error", Message = valResult.message });
            if (valResult.result != Results.Success)
                return NotFound();

            var repoResult = await _repo.UpdateRequestDocumentAsync(request);
            if (repoResult.result == Results.Success)
                return Ok(new { Status = "ok", ReqDocID = repoResult.reqdocid, URLAttachment=request.URLAttachment, Message = repoResult.message });
            else if (repoResult.result == Results.Failed)
                return Ok(new { Status = "error", Message = repoResult.message });
            return NotFound();
        }


        [HttpPost]
        [Route("reqclearance/new")]
        public async Task<IActionResult> Task04([FromBody] RequestDocument request)
        {
            var valResult = await validity(request);
            if (valResult.result == Results.Failed)
                return Ok(new { Status = "error", Message = valResult.message });
            if (valResult.result != Results.Success)
                return NotFound();

            var repoResult = await _repo.RequestBrgyClearanceAsync(request);
            if (repoResult.result == Results.Success)
            {
                FilterRequest f = new FilterRequest();
                f.Search = repoResult.reqdocid;
                f.num_row = "0";
                f.Status = "1";
                var result = await _repo.LoadRequestDocument(f);
                return Ok(new { Status = "ok", ReqDocID = repoResult.reqdocid, requestdocument = result.reqdoc, Message = repoResult.message });
            }
                
            else if (repoResult.result == Results.Failed)
                return Ok(new { Status = "error", Message = repoResult.message });
            return NotFound();
        }


        [HttpPost]
        [Route("reqclearance/edit")]
        public async Task<IActionResult> Task05([FromBody] RequestDocument request)
        {
            var valResult = await validity(request);
            if (valResult.result == Results.Failed)
                return Ok(new { Status = "error", Message = valResult.message });
            if (valResult.result != Results.Success)
                return NotFound();

            var repoResult = await _repo.UpdateRequestBrgyClearanceAsync(request);
            if (repoResult.result == Results.Success)
                return Ok(new { Status = "ok", ReqDocID = repoResult.reqdocid, URLAttachment = request.URLAttachment, Message = repoResult.message });
            else if (repoResult.result == Results.Failed)
                return Ok(new { Status = "error", Message = repoResult.message });
            return NotFound();
        }

        [HttpPost]
        [Route("reqdoc/generatereport")]
        public async Task<IActionResult> Task06([FromBody] RequestDocument request)
        {
            var valResult = await validityReport(request);
            if (valResult.result == Results.Failed)
                return Ok(new { Status = "error", Message = valResult.message });
            if (valResult.result != Results.Success)
                return NotFound();

            var repoResult = await _repo.PrintRequestDocumentAsync(request);
            if (repoResult.result == Results.Success)
                return Ok(new { Status = "ok", URLDocPath=request.URL_DocPath, Message = repoResult.message });
            else if (repoResult.result == Results.Failed)
                return Ok(new { Status = "error", Message = repoResult.message });
            return NotFound();
        }

        [HttpPost]
        [Route("reqdoc/receivedreqdoc")]
        public async Task<IActionResult> Task07([FromBody] RequestDocument request)
        {
            var repoResult = await _repo.ReceivedRequestDocument(request);
            if (repoResult.result == Results.Success)
                return Ok(new { Status = "ok", Message = repoResult.message });
            else if (repoResult.result == Results.Failed)
                return Ok(new { Status = "error", Message = repoResult.message });
            return NotFound();
        }



        [HttpPost]
        [Route("reqdoc/loadreqdocattm")]
        public async Task<IActionResult> Task08([FromBody] RequestDocument request)
        {
            var result = await _repo.LoadIssuesConcernAttachment(request);
            if (result.result == Results.Success)
                return Ok(result.reqdoc);
            return NotFound();
        }

        [HttpPost]
        [Route("reqdoc/purpose")]
        public async Task<IActionResult> Task09()
        {
            var result = await _repo.LoadPurpose();
            if (result.result == Results.Success)
                return Ok(result.purpose);
            return NotFound();
        }

        [HttpPost]
        [Route("reqdoc/businessname")]
        public async Task<IActionResult> Task10()
        {
            var result = await _repo.LoadBusinessName();
            if (result.result == Results.Success)
                return Ok(result.bizname);
            return NotFound();
        }

        [HttpPost]
        [Route("reqdoc/businessowner")]
        public async Task<IActionResult> Task11()
        {
            var result = await _repo.LoadBusinessOwner();
            if (result.result == Results.Success)
                return Ok(result.bizowner);
            return NotFound();
        }

        [HttpPost]
        [Route("reqdoc/businesstype")]
        public async Task<IActionResult> Task12(string businessname)
        {
            var result = await _repo.LoadBusinessType(businessname);
            if (result.result == Results.Success)
                return Ok(result.businesstype);
            return NotFound();
        }

        [HttpPost]
        [Route("reqdoc/account")]
        public async Task<IActionResult> Task13([FromBody] FilterRequest request)
        {
            var result = await _repo.LoadIndividualRequestDocument(request);
            if (result.result == Results.Success)
                return Ok(result.reqdoc);
            return NotFound();
        }

        [HttpPost]
        [Route("purpose/new")]
        public async Task<IActionResult> Task14([FromBody] PurposeDetails request)
        {
            var result = await _repo.PurposeAsyn(request);
            if (result.result == Results.Success)
                return Ok(new { result = result.result, message = result.message, PurposeID = result.purposeid });
            else if (result.result == Results.Failed)
                return Ok(new { result = result.result, message = result.message });
            return NotFound();
        }
        [HttpPost]
        [Route("purpose/edit")]
        public async Task<IActionResult> Task15([FromBody] PurposeDetails request)
        {
            var result = await _repo.PurposeAsyn(request, true);
            if (result.result == Results.Success)
                return Ok(new { result = result.result, message = result.message, PurposeID = result.purposeid });
            else if (result.result == Results.Failed)
                return Ok(new { result = result.result, message = result.message });
            return NotFound();
        }

        [HttpPost]
        [Route("purpose/remove")]
        public async Task<IActionResult> Task16([FromBody] PurposeDetails request)
        {
            var result = await _repo.RemovePurpose(request);
            if (result.result == Results.Success)
                return Ok(new { result = result.result, message = result.message });
            else if (result.result == Results.Failed)
                return Ok(new { result = result.result, message = result.message });
            return NotFound();
        }


        [HttpPost]
        [Route("certtype/new")]
        public async Task<IActionResult> Task17([FromBody] CertificateTypeDetails request)
        {
            var result = await _repo.CertificateTypeAsyn(request);
            if (result.result == Results.Success)
                return Ok(new { result = result.result, message = result.message, CertTypID = result.certtypid });
            else if (result.result == Results.Failed)
                return Ok(new { result = result.result, message = result.message });
            return NotFound();
        }

        [HttpPost]
        [Route("certtype/edit")]
        public async Task<IActionResult> Task18([FromBody] CertificateTypeDetails request)
        {
            var result = await _repo.CertificateTypeAsyn(request, true);
            if (result.result == Results.Success)
                return Ok(new { result = result.result, message = result.message, CertTypID = result.certtypid });
            else if (result.result == Results.Failed)
                return Ok(new { result = result.result, message = result.message });
            return NotFound();
        }

        [HttpPost]
        [Route("certtype/remove")]
        public async Task<IActionResult> Task19([FromBody] CertificateTypeDetails request)
        {
            var result = await _repo.RemoveCertificate(request);
            if (result.result == Results.Success)
                return Ok(new { result = result.result, message = result.message });
            else if (result.result == Results.Failed)
                return Ok(new { result = result.result, message = result.message });
            return NotFound();
        }
        [HttpPost]
        [Route("certtype")]
        public async Task<IActionResult> Task20()
        {
            var result = await _repo.LoadCertificateType();
            if (result.result == Results.Success)
                return Ok(result.certtyp);
            return NotFound();
        }


        private async Task<(Results result, string message)> validity(RequestDocument request)
        {
            if (request == null)
                return (Results.Null, null);

            if (request.Attachment == null || request.Attachment.Count < 1)
                return (Results.Success, null);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < request.Attachment.Count; i++)
            {
                var attachment = request.Attachment[i];
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
                    request.Attachment[i] = url;
                }
                else return (Results.Failed, "Make sure selected image is valid.");
            }
            if (sb.Length > 0)
            {
                request.iAttachments = sb.ToString();
                return (Results.Success, null);
            }
            return (Results.Failed, "Make sure selected image is valid.");
        }
        private async Task<(Results result, string message)> validityReport(RequestDocument request)
        {
            if (request == null)
                return (Results.Null, null);
            if (request.BizReport.IsEmpty())
                return (Results.Success, null);
            byte[] bytes = Convert.FromBase64String(request.BizReport.Str());
            if (bytes.Length == 0)
                return (Results.Failed, "Make sure you have internet connection.");
            var res = await ReportService.SendAsync(bytes, request.ControlNo);
            bytes.Clear();
            if (res == null)
                return (Results.Failed, "Please contact to admin.");
            var json = JsonConvert.DeserializeObject<Dictionary<string, object>>(res);
            if (json["status"].Str() != "error")
            {
                request.URL_DocPath = (json["url"].Str()).Replace(_config["Portforwarding:LOCAL"].Str(), _config["Portforwarding:URL"].Str()).Replace("https", "http");

                return (Results.Success, null);
            }
            return (Results.Null, "Make sure you have internet connection.");
        }
    }
}
