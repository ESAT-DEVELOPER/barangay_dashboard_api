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
using Comm.Commons.Extensions;
using Newtonsoft.Json;
using webapi.App.Features.UserFeature;

namespace webapi.Controllers.STLPartylistDashboardContorller.Features
{
    [Route("app/v1/stldashboard")]
    [ApiController]
    [ServiceFilter(typeof(SubscriberAuthenticationAttribute))]
    public class APKController:ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IAPKRepository _repo;
        public APKController(IConfiguration config, IAPKRepository repo)
        {
            _config = config;
            _repo = repo;
        }


        [HttpPost]
        [Route("apk")]
        public async Task<IActionResult> APK()
        {
            var result = await _repo.LoadAPK();
            if (result.result == Results.Success)
                return Ok(result.apk);
            return NotFound();
        }
        [HttpPost]
        [Route("apk/new")]
        public async Task<IActionResult> Task0a([FromBody] APK request)
        {
            var valresult = await validityAPKt(request);
            if (valresult.result == Results.Failed)
                return Ok(new { Status = "error", Message = valresult.message });
            if (valresult.result != Results.Success)
                return NotFound();

            var result = await _repo.APKAsync(request);
            if (result.result == Results.Success)
                return Ok(new { result = result.result, message = result.message, trn=result.TRNNo });
            if (result.result == Results.Failed)
                return Ok(new { result = result.result, message = result.message, trn = result.TRNNo });
            return NotFound();
        }
        [HttpPost]
        [Route("apk/edit")]
        public async Task<IActionResult> Task0b([FromBody] APK request)
        {
            var result = await _repo.APKAsync(request,true);
            if (result.result == Results.Success)
                return Ok(new { result = result.result, message = result.message, trn = result.TRNNo });
            if (result.result == Results.Failed)
                return Ok(new { result = result.result, message = result.message, trn = result.TRNNo });
            return NotFound();
        }
        [HttpPost]
        [Route("apk/setprimary")]
        public async Task<IActionResult> Task0c([FromBody] APK request)
        {
            var result = await _repo.SetPrimaryUpdate(request);
            if (result.result == Results.Success)
                return Ok(new { result = result.result, message = result.message });
            if (result.result == Results.Failed)
                return Ok(new { result = result.result, message = result.message });
            return NotFound();
        }

        private async Task<(Results result, string message)> validityAPKt(APK request)
        {
            if (request == null)
                return (Results.Null, null);
            if (request.APK_File.Str().IsEmpty())
                return (Results.Success, null);

            var resApp = ApkUploader.SendAsync("app", $"{request.APKVerno}_{request.PL_ID}{request.PGRP_ID}", request.APK_File).Result;
            var data = JsonConvert.DeserializeObject<dynamic>(resApp);
            if(data.status == "success")
            {
                request.APKPathCBA = data.url;

                request.APKPath = data.url_version;
                return (Results.Success, null);
            }
            return (Results.Null, "Make sure you have internet connection.");
        }
    }
}
