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
using webapi.App.Features.UserFeature;
using webapi.App.RequestModel.AppRecruiter;
using webapi.App.RequestModel.Common;
using webapi.App.STLDashboardModel;
using Newtonsoft.Json;

namespace webapi.Controllers.STLPartylistDashboardContorller.Features
{
    [Route("app/v1/stldashboard")]
    [ApiController]
    [ServiceFilter(typeof(SubscriberAuthenticationAttribute))]
    public class LegalDocumentController:ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly ILegalDocumentsRepository _repo; 
        public LegalDocumentController(IConfiguration config, ILegalDocumentsRepository repo)
        {
            _config = config;
            _repo = repo;
        }

        [HttpPost]
        [Route("formtab/new")]
        public async Task<IActionResult> Task0a([FromBody] LegalDocument request)
        {
            var result = await _repo.FormTabAsyn(request);
            if (result.result == Results.Success)
                return Ok(new { Status = "ok", Message = result.message, FormTab = result.formtab });
            if (result.result == Results.Failed)
                return Ok(new { Status = "error", Message = result.message });
            return NotFound();
        }

        [HttpPost]
        [Route("formtab")]
        public async Task<IActionResult> Task0b(LegalDocument req)
        {
            var result = await _repo.Load_FormTab(req);
            if (result.result == Results.Success)
                return Ok(result.formtab);
            return NotFound();
        }
        [HttpPost]
        [Route("formtab/remove")]
        public async Task<IActionResult> Task0c([FromBody] LegalDocument request)
        {
            var result = await _repo.Delete_FormTab(request);
            if (result.result == Results.Success)
                return Ok(new { Status = "ok", Message = result.message });
            if (result.result == Results.Failed)
                return Ok(new { Status = "error", Message = result.message });
            return NotFound();
        }

        [HttpPost]
        [Route("legaldocument/new")]
        public async Task<IActionResult> Task0d([FromBody] LegalDocument_Transaction request)
        {
            var result = await _repo.LegalDocAsync(request);
            if (result.result == Results.Success)
                return Ok(new { Status = "ok", Message = result.message, LegalDoc = result.legaldoc });
            if (result.result == Results.Failed)
                return Ok(new { Status = "error", Message = result.message });
            return NotFound();
        }


        [HttpPost]
        [Route("legaldocument/generate")]
        public async Task<IActionResult> Task0d2([FromBody] LegalDocument_Transaction request)
        {
            var valresult = await validityReport(request);
            if (valresult.result == Results.Failed)
                return Ok(new { Status = "error", Message = valresult.message });
            if (valresult.result != Results.Success)
                return NotFound();

            var result = await _repo.Generate_LegalDoc(request);
            if (result.result == Results.Success)
                return Ok(new { Status = "ok", Message = result.message });
            if (result.result == Results.Failed)
                return Ok(new { Status = "error", Message = result.message });
            return NotFound();
        }

        [HttpPost]
        [Route("legaldocument/process")]
        public async Task<IActionResult> Task0d1([FromBody] LegalDocument_Transaction request)
        {
            var result = await _repo.ProcessOtheDocumentRequest(request);
            if (result.result == Results.Success)
                return Ok(new { Status = "ok", Message = result.message });
            if (result.result == Results.Failed)
                return Ok(new { Status = "error", Message = result.message });
            return NotFound();
        }

        [HttpPost]
        [Route("legaldocument")]
        public async Task<IActionResult> Task0e(LegalDocument_Transaction req)
        {
            var result = await _repo.Load_LegalDoc(req);
            if (result.result == Results.Success)
                return Ok(result.legaldoc);
            return NotFound();
        }

        [HttpPost]
        [Route("legaldocument/id")]
        public async Task<IActionResult> Task0e1(LegalDocument_Transaction req)
        {
            var result = await _repo.Load_LegalDocID(req);
            if (result.result == Results.Success)
                return Ok(result.legaldoc);
            return NotFound();
        }
        [HttpPost]
        [Route("deathcertificate/id")]
        public async Task<IActionResult> Task0e12(LegalDocument_Transaction req)
        {
            var result = await _repo.Load_DeathCertificateID(req);
            if (result.result == Results.Success)
                return Ok(result.legaldoc);
            return NotFound();
        }
        [HttpPost]
        [Route("legaldocument/details")]
        public async Task<IActionResult> Task0f(LegalDocument_Transaction req)
        {
            var result = await _repo.Load_LegalDocDetails(req);
            if (result.result == Results.Success)
                return Ok(result.legaldocdetails);
            return NotFound();
        }
        [HttpPost]
        [Route("legaldocument/release")]
        public async Task<IActionResult> Task0b([FromBody] LegalDocument_Transaction request)
        {
            var result = await _repo.ReleaseAsync(request);
            if (result.result == Results.Success)
                return Ok(new { Status = "ok", Message = result.message, Release = result.release });
            if (result.result == Results.Failed)
                return Ok(new { Status = "error", Message = result.message });
            return NotFound();
        }
        [HttpPost]
        [Route("legaldocument/received")]
        public async Task<IActionResult> Task0b1([FromBody] LegalDocument_Transaction request)
        {
            var result = await _repo.ReceivedOtheDocumentRequest(request);
            if (result.result == Results.Success)
                return Ok(new { Status = "ok", Message = result.message, Content = request });
            if (result.result == Results.Failed)
                return Ok(new { Status = "error", Message = result.message });
            return NotFound();
        }
        [HttpPost]
        [Route("legaldocument/cancel")]
        public async Task<IActionResult> Task0c([FromBody] LegalDocument_Transaction request)
        {
            var result = await _repo.CancellAsync(request);
            if (result.result == Results.Success)
                return Ok(new { Status = "ok", Message = result.message, Cancel = result.cancel });
            if (result.result == Results.Failed)
                return Ok(new { Status = "error", Message = result.message });
            return NotFound();
        }

        private async Task<(Results result, string message)> validityReport(LegalDocument_Transaction request)
        {
            if (request == null)
                return (Results.Null, null);
            if (request.ExportedDocument.IsEmpty())
                return (Results.Success, null);
            byte[] bytes = Convert.FromBase64String(request.ExportedDocument.Str());
            if (bytes.Length == 0)
                return (Results.Failed, "Make sure you have internet connection.");
            var res = await ReportService.SendAsync(bytes, $"certificate_{request.PLID}{request.PGRPID}{request.LegalFormsID}");
            bytes.Clear();
            if (res == null)
                return (Results.Failed, "Please contact to admin.");
            var json = JsonConvert.DeserializeObject<Dictionary<string, object>>(res);
            if (json["status"].Str() != "error")
            {
                //request.URLDocument = json["url"].Str();
                request.URLDocument = (json["url"].Str()).Replace(_config["Portforwarding:LOCAL"].Str(), _config["Portforwarding:URL"].Str()).Replace("https", "http");
                return (Results.Success, null);
            }
            return (Results.Null, "Make sure you have internet connection.");
        }
    }
}
