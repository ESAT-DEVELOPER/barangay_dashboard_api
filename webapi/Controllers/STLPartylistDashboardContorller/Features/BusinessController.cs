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
using webapi.App.RequestModel.Common;
using webapi.App.STLDashboardModel;
using webapi.App.Features.UserFeature;
using Newtonsoft.Json;

namespace webapi.Controllers.STLPartylistDashboardContorller.Features
{
    [Route("app/v1/stldashboard")]
    [ApiController]
    [ServiceFilter(typeof(SubscriberAuthenticationAttribute))]
    public class BusinessController:ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IBussinessRepository _repo;
        public BusinessController(IConfiguration config, IBussinessRepository repo)
        {
            _config = config;
            _repo = repo;
        }

        [HttpPost]
        [Route("business/new")]
        public async Task<IActionResult> Task0a([FromBody] BusinesseRequest request)
        {
            var result = await _repo.BussinessAsync(request);
            if (result.result == Results.Success)
                return Ok(new { Status = "ok", Message = result.message, Business=result.biz });
            if (result.result == Results.Failed)
                return Ok(new { Status = "error", Message = result.message });
            return NotFound();
        }
        [HttpPost]
        [Route("business/edit")]
        public async Task<IActionResult> Task0b([FromBody] BusinesseRequest request)
        {
            var result = await _repo.UpdateBussinessAsync(request);
            if (result.result == Results.Success)
                return Ok(new { Status = "ok", Message = result.message });
            if (result.result == Results.Failed)
                return Ok(new { Status = "error", Message = result.message });
            return NotFound();
        }
        [HttpPost]
        [Route("business/type")]
        public async Task<IActionResult> Task0c()
        {
            var result = await _repo.LoadTYpe();
            if (result.result == Results.Success)
                return Ok(result.type);
            return NotFound();
        }
        [HttpPost]
        [Route("business")]
        public async Task<IActionResult> Task0d([FromBody] FilterRequest request)
        {
            var result = await _repo.Load_RegisteredBusiness(request);
            if (result.result == Results.Success)
                return Ok(result.business);
            return NotFound();
        }

        [HttpPost]
        [Route("business/dochistory")]
        public async Task<IActionResult> Task0e([FromBody] FilterRequest request)
        {
            var result = await _repo.Load_BusinessDocHistory(request);
            if (result.result == Results.Success)
                return Ok(result.dochistory);
            return NotFound();
        }

        [HttpPost]
        [Route("businessownershiptype")]
        public async Task<IActionResult> Task0f()
        {
            var result = await _repo.Load_BusinessOwnershipType();
            if (result.result == Results.Success)
                return Ok(result.bizowrnshptyp);
            return NotFound();
        }
        [HttpPost]
        [Route("businessownershiptype/new")]
        public async Task<IActionResult> Task0g([FromBody] BusinessOwnershipType request)
        {
            var result = await _repo.BusinessOwnershipTypeAsync(request);
            if (result.result == Results.Success)
                return Ok(new { Status = "ok", Message = result.message, BusinessOwnershiptypeID = result.bizownrshptypid });
            if (result.result == Results.Failed)
                return Ok(new { Status = "error", Message = result.message });
            return NotFound();
        }
        [HttpPost]
        [Route("businessownershiptype/edit")]
        public async Task<IActionResult> Task0h([FromBody] BusinessOwnershipType request)
        {
            var result = await _repo.BusinessOwnershipTypeAsync(request);
            if (result.result == Results.Success)
                return Ok(new { Status = "ok", Message = result.message, BusinessOwnershiptypeID = result.bizownrshptypid });
            if (result.result == Results.Failed)
                return Ok(new { Status = "error", Message = result.message });
            return NotFound();
        }
        [HttpPost]
        [Route("businessownershiptype/remove")]
        public async Task<IActionResult> Task0i([FromBody] BusinessOwnershipType request)
        {
            var result = await _repo.RemoveBusinessOwnershiptype(request);
            if (result.result == Results.Success)
                return Ok(new { Status = "ok", Message = result.message });
            if (result.result == Results.Failed)
                return Ok(new { Status = "error", Message = result.message });
            return NotFound();
        }

        [HttpPost]
        [Route("businessclearance")]
        public async Task<IActionResult> Task0j([FromBody] BrgyBusinessClearance request)
        {
            var result = await _repo.Load_BrgyBizClearance(request);
            if (result.result == Results.Success)
                return Ok(result.brgybizclearance);
            return NotFound();
        }

        [HttpPost]
        [Route("businessclearance/id")]
        public async Task<IActionResult> Task0j1([FromBody] BrgyBusinessClearance request)
        {
            var result = await _repo.Load_BrgyBizClearancebyID(request);
            if (result.result == Results.Success)
                return Ok(result.brgybizclearance);
            return NotFound();
        }

        [HttpPost]
        [Route("businessclearance/new")]
        public async Task<IActionResult> Task0k([FromBody] BrgyBusinessClearance request)
        {
            var result = await _repo.BrgyBusinessClearanceAsync(request);
            if (result.result == Results.Success)
                return Ok(new { Status = "ok", Message = result.message, Content = request });
            if (result.result == Results.Failed)
                return Ok(new { Status = "error", Message = result.message });
            return NotFound();
        }

        [HttpPost]
        [Route("businessclearance/export")]
        public async Task<IActionResult> Task0k1([FromBody] BrgyBusinessClearance request)
        {
            var valresult = await validityReport(request);
            if (valresult.result == Results.Failed)
                return Ok(new { Status = "error", Message = valresult.message });
            if (valresult.result != Results.Success)
                return NotFound();

            var result = await _repo.Generate_BrgyBizClearance(request);
            if (result.result == Results.Success)
                return Ok(new { Status = "ok", Message = result.message });
            if (result.result == Results.Failed)
                return Ok(new { Status = "error", Message = result.message });
            return NotFound();
        }

        [HttpPost]
        [Route("businessclearance/release")]
        public async Task<IActionResult> Task0l([FromBody] BrgyBusinessClearance request)
        {
            var result = await _repo.ReleaseAsync(request);
            if (result.result == Results.Success)
                return Ok(new { Status = "ok", Message = result.message, Release = result.release });
            if (result.result == Results.Failed)
                return Ok(new { Status = "error", Message = result.message });
            return NotFound();
        }
        [HttpPost]
        [Route("businessclearance/cancel")]
        public async Task<IActionResult> Task0m([FromBody] BrgyBusinessClearance request)
        {
            var result = await _repo.CancellAsync(request);
            if (result.result == Results.Success)
                return Ok(new { Status = "ok", Message = result.message, Cancel = result.cancel });
            if (result.result == Results.Failed)
                return Ok(new { Status = "error", Message = result.message });
            return NotFound();
        }

        [HttpPost]
        [Route("businessclearance/received")]
        public async Task<IActionResult> Task0n([FromBody] BrgyBusinessClearance request)
        {
            var result = await _repo.ReceivedBrgyBusinessClearanceRequestAsync(request);
            if (result.result == Results.Success)
                return Ok(new { Status = "ok", Message = result.message, Content = request });
            if (result.result == Results.Failed)
                return Ok(new { Status = "error", Message = result.message });
            return NotFound();
        }
        [HttpPost]
        [Route("businessclearance/process")]
        public async Task<IActionResult> Task0O([FromBody] BrgyBusinessClearance request)
        {
            var result = await _repo.ProcessBrgyBusinessClearanceRequestAsync(request);
            if (result.result == Results.Success)
                return Ok(new { Status = "ok", Message = result.message });
            if (result.result == Results.Failed)
                return Ok(new { Status = "error", Message = result.message });
            return NotFound();
        }

        private async Task<(Results result, string message)> validityReport(BrgyBusinessClearance request)
        {
            if (request == null)
                return (Results.Null, null);
            if (request.ExportedDocument.IsEmpty())
                return (Results.Success, null);
            byte[] bytes = Convert.FromBase64String(request.ExportedDocument.Str());
            if (bytes.Length == 0)
                return (Results.Failed, "Make sure you have internet connection.");
            var res = await ReportService.SendAsync(bytes, $"businessclearance_{request.PLID}{request.PGRPID}{request.BusinessClearanceID}");
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
