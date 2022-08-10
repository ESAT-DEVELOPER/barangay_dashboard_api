﻿using Microsoft.AspNetCore.Mvc;
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
    }
}