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

namespace webapi.Controllers.STLPartylistDashboardContorller.Features
{
    [Route("app/v1/stldashboard")]
    [ApiController]
    [ServiceFilter(typeof(SubscriberAuthenticationAttribute))]
    public class FingerprintScannerController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IFingerprintScannerRepository _repo;
        private readonly IAccountRepository _loginrepo;

        public FingerprintScannerController(IConfiguration config, IFingerprintScannerRepository repo, IAccountRepository loginrepo)
        {
            _repo = repo;
            _config = config;
            _loginrepo = loginrepo;
        }

        [HttpPost]
        [Route("fingerprints/save")]
        public async Task<IActionResult> SaveFingerprints([FromBody] Fingerprint info)
        {
            var valResult = await validityReport(info);
            if (valResult.result == Results.Failed)
                return Ok(new { Status = "error", Message = valResult.message });
            if (valResult.result != Results.Success)
                return NotFound();

            var result = await _repo.SaveFingerprints(info);
            if (result.result == Results.Success)
                return Ok(new { result = result.result, message = result.message });
            return NotFound();
        }

        [HttpPost]
        [Route("fingerprints/update")]
        public async Task<IActionResult> UpdateFingerprints([FromBody] Fingerprint info)
        {
            var valResult = await validityReport(info);
            if (valResult.result == Results.Failed)
                return Ok(new { Status = "error", Message = valResult.message });
            if (valResult.result != Results.Success)
                return NotFound();

            var result = await _repo.UpdateFingerprints(info);
            if (result.result == Results.Success)
                return Ok(new { result = result.result, message = result.message });
            return NotFound();
        }

        [HttpPost]
        [Route("fingerprints/load")]
        public async Task<IActionResult> LoadFingerprints(string userId)
        {
            var result = await _repo.LoadFingerprints(userId);
            if (result.result == Results.Success)
                return Ok(result.fingerprints);
            return NotFound();
        }

        private async Task<(Results result, string message)> validityReport(Fingerprint request)
        {
            List<Fingerprint.FingerImage> _tempList = new List<Fingerprint.FingerImage>();
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            if (request == null)
                return (Results.Null, null);
            if (request.ImageList.Count < 1)
                return (Results.Success, null);
            byte[] bytes = null;
            foreach (var item in request.ImageList)
            {
                if (item.Image != null)
                {
                    bytes = Convert.FromBase64String(item.Image);
                    if (bytes.Length == 0)
                        return (Results.Failed, "Make sure selected document path is invalid.");
                    var res = await ImgService.SendAsync(bytes);
                    bytes.Clear();
                    if (res == null)
                        return (Results.Failed, "Please contact to admin.");
                    var json = JsonConvert.DeserializeObject<Dictionary<string, object>>(res);
                    if (json["status"].Str() != "error")
                        dictionary.Add(item.Index, json["url"].Str().Replace(_config["Portforwarding:LOCAL"].Str(), _config["Portforwarding:URL"].Str()).Replace("https", "http"));
                }
                else
                    dictionary.Add(item.Index, null);

            }
            request.Json = JsonConvert.SerializeObject(dictionary, Formatting.None);
            return (Results.Success, null);
        }
    }
}
