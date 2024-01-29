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

namespace webapi.Controllers.STLPartylistDashboardContorller.Features
{
    [Route("app/v1/stldashboard")]
    [ApiController]
    [ServiceFilter(typeof(SubscriberAuthenticationAttribute))]
    public class TemplateController:ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly ITemplateRepository _repo;
        public TemplateController(IConfiguration config, ITemplateRepository repo)
        {
            _config = config;
            _repo = repo;
        }
        [HttpPost]
        [Route("templatetype")]
        public async Task<IActionResult> Task0a()
        {
            var result = await _repo.Load_TemplateType();
            if (result.result == Results.Success)
                return Ok(result.tpl);
            return NotFound();
        }
        [HttpPost]
        [Route("templatetype1")]
        public async Task<IActionResult> Task0a1(string plid, string pgrpid)
        {
            var result = await _repo.Load_TemplateType1(plid, pgrpid);
            if (result.result == Results.Success)
                return Ok(result.tpl);
            return NotFound();
        }
        [HttpPost]
        [Route("templatetype/new")]
        public async Task<IActionResult> Task0b([FromBody] TemplateType req)
        {
            var result = await _repo.TemplateTypeAsync(req);
            if (result.result == Results.Success)
                return Ok(new { result = result.result, message = result.message, TPLID = result.tplid });
            else if (result.result == Results.Failed)
                return Ok(new { result = result.result, message = result.message });
            return NotFound();
        }
        [HttpPost]
        [Route("templatetype/edit")]
        public async Task<IActionResult> Task0c([FromBody] TemplateType req)
        {
            var result = await _repo.TemplateTypeAsync(req);
            if (result.result == Results.Success)
                return Ok(new { result = result.result, message = result.message, TPLID = result.tplid });
            else if (result.result == Results.Failed)
                return Ok(new { result = result.result, message = result.message });
            return NotFound();
        }



        [HttpPost]
        [Route("templatedoc/tagline")]
        public async Task<IActionResult> Task0d1(string templateid)
        {
            var result = await _repo.Load_Tagline(templateid);
            if (result.result == Results.Success)
                return Ok(result.tagline);
            return NotFound();
        }
        [HttpPost]
        [Route("templatedoc")]
        public async Task<IActionResult> Task0d(string templateid)
        {
            var result = await _repo.Load_TemplateDoc(templateid);
            if (result.result == Results.Success)
                return Ok(result.tpl);
            return NotFound();
        }

        [HttpPost]
        [Route("templatedoc1")]
        public async Task<IActionResult> Task0d1(string plid, string pgrpid, string templateid)
        {
            var result = await _repo.Load_TemplateDoc1(plid, pgrpid, templateid);
            if (result.result == Results.Success)
                return Ok(result.tpl);
            return NotFound();
        }
        [HttpPost]
        [Route("templatedoc/new")]
        public async Task<IActionResult> Task0e([FromBody] TemplateDocument req)
        {
            var result = await _repo.TemplateDocAsync(req);
            if (result.result == Results.Success)
                return Ok(new { result = result.result, message = result.message, TPLID = result.tplid });
            else if (result.result == Results.Failed)
                return Ok(new { result = result.result, message = result.message });
            return NotFound();
        }
        [HttpPost]
        [Route("templatedoc/edit")]
        public async Task<IActionResult> Task0f([FromBody] TemplateDocument req)
        {
            var result = await _repo.TemplateDocAsync(req);
            if (result.result == Results.Success)
                return Ok(new { result = result.result, message = result.message, TPLID = result.tplid });
            else if (result.result == Results.Failed)
                return Ok(new { result = result.result, message = result.message });
            return NotFound();
        }



        [HttpPost]
        [Route("taglineitem")]
        public async Task<IActionResult> Task0g([FromBody] ItemDescription req)
        {
            var result = await _repo.Load_ItemTab(req);
            if (result.result == Results.Success)
                return Ok(result.item);
            return NotFound();
        }
        [HttpPost]
        [Route("taglineitem/new")]
        public async Task<IActionResult> Task0h([FromBody] ItemDescription req)
        {
            var result = await _repo.ItemTabAsyn(req);
            if (result.result == Results.Success)
                return Ok(new { result = result.result, message = result.message, DescriptionID = result.descid });
            else if (result.result == Results.Failed)
                return Ok(new { result = result.result, message = result.message });
            return NotFound();
        }
        [HttpPost]
        [Route("taglineitem/edit")]
        public async Task<IActionResult> Task0i([FromBody] ItemDescription req)
        {
            var result = await _repo.ItemTabAsyn(req);
            if (result.result == Results.Success)
                return Ok(new { result = result.result, message = result.message, DescriptionID = result.descid });
            else if (result.result == Results.Failed)
                return Ok(new { result = result.result, message = result.message });
            return NotFound();
        }


        [HttpPost]
        [Route("residenttype")]
        public async Task<IActionResult> Task0j()
        {
            var result = await _repo.Load_ResType();
            if (result.result == Results.Success)
                return Ok(result.restype);
            return NotFound();
        }
        [HttpPost]
        [Route("residenttype/new")]
        public async Task<IActionResult> Task0k([FromBody] ResidentType req)
        {
            var result = await _repo.ResidentTypeAsync(req);
            if (result.result == Results.Success)
                return Ok(new { result = result.result, message = result.message, RestTypId = result.resid });
            else if (result.result == Results.Failed)
                return Ok(new { result = result.result, message = result.message });
            return NotFound();
        }
        [HttpPost]
        [Route("residenttype/edit")]
        public async Task<IActionResult> Task0l([FromBody] ResidentType req)
        {
            var result = await _repo.ResidentTypeAsync(req);
            if (result.result == Results.Success)
                return Ok(new { result = result.result, message = result.message, RestTypId = result.resid });
            else if (result.result == Results.Failed)
                return Ok(new { result = result.result, message = result.message });
            return NotFound();
        }


        [HttpPost]
        [Route("residenttype/remove")]
        public async Task<IActionResult> Task0m([FromBody] ResidentType req)
        {
            var result = await _repo.RemoveResidentType(req);
            if (result.result == Results.Success)
                return Ok(new { result = result.result, message = result.message});
            else if (result.result == Results.Failed)
                return Ok(new { result = result.result, message = result.message });
            return NotFound();
        }

        [HttpPost]
        [Route("templatetype/remove")]
        public async Task<IActionResult> Task0n([FromBody] TemplateType req)
        {
            var result = await _repo.RemoveTemplateType(req);
            if (result.result == Results.Success)
                return Ok(new { result = result.result, message = result.message });
            else if (result.result == Results.Failed)
                return Ok(new { result = result.result, message = result.message });
            return NotFound();
        }

        [HttpPost]
        [Route("templatedoc/remove")]
        public async Task<IActionResult> Task0o([FromBody] TemplateDocument req)
        {
            var result = await _repo.RemoveTemplateDoc(req);
            if (result.result == Results.Success)
                return Ok(new { result = result.result, message = result.message });
            else if (result.result == Results.Failed)
                return Ok(new { result = result.result, message = result.message });
            return NotFound();
        }

        [HttpPost]
        [Route("templateforms/update")]
        public async Task<IActionResult> Task0p([FromBody] FormTemplate req)
        {
            var result = await _repo.FormTemplateAsync(req);
            if (result.result == Results.Success)
                return Ok(new { result = result.result, message = result.message, Content = req });
            else if (result.result == Results.Failed)
                return Ok(new { result = result.result, message = result.message });
            return NotFound();
        }

        [HttpPost]
        [Route("licensetemplateforms/update")]
        public async Task<IActionResult> Task0p1([FromBody] FormTemplate req)
        {
            var result = await _repo.FormLicenseTemplateAsync(req);
            if (result.result == Results.Success)
                return Ok(new { result = result.result, message = result.message, Content = req });
            else if (result.result == Results.Failed)
                return Ok(new { result = result.result, message = result.message });
            return NotFound();
        }

        [HttpPost]
        [Route("templateforms")]
        public async Task<IActionResult> Task0q([FromBody] FormTemplate req)
        {
            var result = await _repo.Load_TemplateForm(req);
            if (result.result == Results.Success)
                return Ok(result.templateform);
            return NotFound();
        }
        [HttpPost]
        [Route("licensetemplateforms")]
        public async Task<IActionResult> Task0q1([FromBody] FormTemplate req)
        {
            var result = await _repo.Load_LicenseTemplateForm(req);
            if (result.result == Results.Success)
                return Ok(result.templateform);
            return NotFound();
        }
        [HttpPost]
        [Route("templateforms/tagline")]
        public async Task<IActionResult> Task0r(string formsid)
        {
            var result = await _repo.TemplateForms_Tagline(formsid);
            if (result.result == Results.Success)
                return Ok(result.tagline);
            return NotFound();
        }

        [HttpPost]
        [Route("deathcertificatedetails/tagline")]
        public async Task<IActionResult> Task0s(string formsid)
        {
            var result = await _repo.DeathCertificateDetails_Tagline(formsid);
            if (result.result == Results.Success)
                return Ok(result.tagline);
            return NotFound();
        }
    }
}
