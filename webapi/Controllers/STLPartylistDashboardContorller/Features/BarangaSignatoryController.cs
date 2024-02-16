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
using Newtonsoft.Json;
using System.Text;
using System.Security.Policy;

namespace webapi.Controllers.STLPartylistDashboardContorller.Features
{
    [Route("app/v1/stldashboard")]
    [ApiController]
    [ServiceFilter(typeof(SubscriberAuthenticationAttribute))]
    public class BarangaSignatoryController:ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IBarangaySignatoryRepository _repo;
        public BarangaSignatoryController(IConfiguration config, IBarangaySignatoryRepository repo)
        {
            _config = config;
            _repo = repo;
        }
        [HttpPost]
        [Route("signatory")]
        public async Task<IActionResult> Group()
        {
            var result = await _repo.LoadSignatory();
            if (result.result == Results.Success)
                return Ok(result.signatory);
            return NotFound();
        }


        [HttpPost]
        [Route("signatorya")]
        public async Task<IActionResult> Signatuory()
        {
            var result = await _repo.LoadSignatoryA();
            if (result.result == Results.Success)
                return Ok(result.signatory);
            return NotFound();
        }

        [HttpPost]
        [Route("barangaysignatory")]
        public async Task<IActionResult> execute0a([FromBody] BarangaySignatory req)
        {
            var result = await _repo.Load_Signature(req);
            if (result.result == Results.Success)
                return Ok(result.sig);
            return NotFound();
        }
        [HttpPost]
        [Route("signatory/update")]
        public async Task<IActionResult> execute0b([FromBody] BrgySignatory request)
        {
            var valsig = await validity(request);
            if (valsig.result == Results.Failed)
                return Ok(new { Status = "error", Message = valsig.message });
            //if (valsig.result != Results.Success)
            //    return NotFound();

            var result = await _repo.SignatoryAsync(request);
            if (result.result == Results.Success)
                return Ok(new { result = result.result, message = result.message });
            else if(result.result==Results.Failed)
                return Ok(new { result = result.result, message = result.message });
            return NotFound();
        }

        [HttpPost]
        [Route("signatory/update02")]
        public async Task<IActionResult> execute0b02([FromBody] BarangaySignatures signatory)
        {
            var valsig = await validitysignatory(signatory);
            if (valsig.result == Results.Failed)
                return Ok(new { Status = "error", Message = valsig.message });
            if (valsig.result != Results.Success)
                return NotFound();

            var result = await _repo.SignatoryAsync02(signatory);
            if (result.result == Results.Success)
                return Ok(new { result = result.result, message = result.message });
            else if (result.result == Results.Failed)
                return Ok(new { result = result.result, message = result.message });
            return NotFound();
        }

        

        private async Task<(Results result, string message)> validitysignatory(BarangaySignatures request)
        {
            //List<STLMembership.GovAttachment> _tempList = new List<STLMembership.GovAttachment>();
            //Dictionary<string, string> dictionary = new Dictionary<string, string>();
            if (request == null)
                return (Results.Null, null);
            if (request.signatories.Count < 1)
                return (Results.Success, null);
            byte[] bytes = null;
            StringBuilder sb = new StringBuilder();
            foreach (var item in request.signatories)

            {
                if (item.base64stringattachment != null && item.NEW_UPLOAD == "1")
                {
                    bytes = Convert.FromBase64String(item.base64stringattachment);
                    if (bytes.Length == 0)
                        return (Results.Failed, "Make sure selected document path is invalid.");
                    var res = await ImgService.SendAsync(bytes);
                    bytes.Clear();
                    if (res == null)
                        return (Results.Failed, "Please contact to admin.");
                    var json = JsonConvert.DeserializeObject<Dictionary<string, object>>(res);
                    if (json["status"].Str() != "error")
                    {
                        //dictionary.Add(item.Index, json["url"].Str().Replace("www.", ""));
                        //string url = json["url"].Str();
                        string url = (json["url"].Str()).Replace(_config["Portforwarding:LOCAL"].Str(), _config["Portforwarding:URL"].Str()).Replace("https", "http");

                        sb.Append($"<item OFFICIAL_ID=\"{item.ELECTED_OFFICIAL_ID}\" BRGY_POSITION=\"{item.BRGY_POSITION}\" ELECTED_OFFICIAL=\"{item.ELECTED_OFFICIAL}\" COMMITTEE=\"{item.COMMITTEE}\" SIGNATURE_URL=\"{url}\"/>");
                        //request.GovIDList[i] = url;
                    }
                    else
                        sb.Append($"<item OFFICIAL_ID=\"{item.ELECTED_OFFICIAL_ID}\" BRGY_POSITION=\"{item.BRGY_POSITION}\" ELECTED_OFFICIAL=\"{item.ELECTED_OFFICIAL}\" COMMITTEE=\"{item.COMMITTEE}\" SIGNATURE_URL=\"\"/>");
                }
                else
                    sb.Append($"<item OFFICIAL_ID=\"{item.ELECTED_OFFICIAL_ID}\" BRGY_POSITION=\"{item.BRGY_POSITION}\" ELECTED_OFFICIAL=\"{item.ELECTED_OFFICIAL}\" COMMITTEE=\"{item.COMMITTEE}\" SIGNATURE_URL=\"{item.SIGNATURE_URL}\"/>");

            }
            if (sb.Length > 0)
            {
                request.isignatories = sb.ToString();
                return (Results.Success, null);
            }
            return (Results.Failed, "Make sure selected image is valid.");
        }

        [HttpPost]
        [Route("signaturecol")]
        public async Task<IActionResult> execute0c()
        {
            var result = await _repo.Load_Col_Signature();
            if (result.result == Results.Success)
                return Ok(result.col);
            return NotFound();
        }
        private async Task<(Results result, string message)> validity(BrgySignatory request)
        {
            if (request == null)
                return (Results.Null, null);
            //if (request.iBrgyOfficialLogo.IsEmpty())
            //    return (Results.Failed, "Please select an Barangay Official Logo.");
            //if (request.iMunicipalLogo.IsEmpty())
            //    return (Results.Failed, "Please select Municipality/ Cities Logo");

            if(!request.Brgy_Captain.IsEmpty() && !request.Brgy_Captain_sig.IsEmpty())
            {
                byte[] captbytes = Convert.FromBase64String(request.Brgy_Captain_sig.Str());
                if (captbytes.Length == 0)
                    return (Results.Failed, "Make sure selected image is not invalid.");
                var rescapt = await ImgService.SendAsync(captbytes);
                captbytes.Clear();
                if (rescapt == null)
                    return (Results.Failed, "Please contact to admin");
                var jsoncapt = JsonConvert.DeserializeObject<Dictionary<string, object>>(rescapt);
                if (jsoncapt["status"].Str() != "error")
                    request.Brgy_Captain_sig_URL = (jsoncapt["url"].Str()).Replace(_config["Portforwarding:LOCAL"].Str(), _config["Portforwarding:URL"].Str()).Replace("https", "http");
            }
            if (!request.Brgy_FirstCouncilor.IsEmpty() && !request.Brgy_FirstCouncilor_sig.IsEmpty())
            {
                if (!request.Brgy_FirstCouncilor_sig.IsEmpty())
                {
                    byte[] firstbytes = Convert.FromBase64String(request.Brgy_FirstCouncilor_sig.Str());
                    if (firstbytes.Length == 0)
                        return (Results.Failed, "Make sure selected image is not invalid.");
                    var resfirst = await ImgService.SendAsync(firstbytes);
                    firstbytes.Clear();
                    if (resfirst == null)
                        return (Results.Failed, "Please contact to admin");
                    var jsonfirst = JsonConvert.DeserializeObject<Dictionary<string, object>>(resfirst);
                    if (jsonfirst["status"].Str() != "error")
                        request.Brgy_FirstCouncilor_sig_URL = (jsonfirst["url"].Str()).Replace(_config["Portforwarding:LOCAL"].Str(), _config["Portforwarding:URL"].Str()).Replace("https", "http");
                }
                
            }

            if (!request.Brgy_SecondCouncilor.IsEmpty() && !request.Brgy_SecondCouncilor_sig.IsEmpty())
            {
                if (!request.Brgy_SecondCouncilor_sig.IsEmpty())
                {
                    byte[] secondbytes = Convert.FromBase64String(request.Brgy_SecondCouncilor_sig.Str());
                    if (secondbytes.Length == 0)
                        return (Results.Failed, "Make sure selected image is not invalid.");
                    var ressecond = await ImgService.SendAsync(secondbytes);
                    secondbytes.Clear();
                    if (ressecond == null)
                        return (Results.Failed, "Please contact to admin");
                    var jsonsecond = JsonConvert.DeserializeObject<Dictionary<string, object>>(ressecond);
                    if (jsonsecond["status"].Str() != "error")
                        request.Brgy_SecondCouncilor_sig_URL = (jsonsecond["url"].Str()).Replace(_config["Portforwarding:LOCAL"].Str(), _config["Portforwarding:URL"].Str()).Replace("https", "http");
                }
            }

            if (!request.Brgy_ThirdCouncilor.IsEmpty() && !request.Brgy_ThirdCouncilor_sig.IsEmpty())
            {
                if (!request.Brgy_ThirdCouncilor_sig.IsEmpty())
                {
                    byte[] thirdbytes = Convert.FromBase64String(request.Brgy_ThirdCouncilor_sig.Str());
                    if (thirdbytes.Length == 0)
                        return (Results.Failed, "Make sure selected image is not invalid.");
                    var resthird = await ImgService.SendAsync(thirdbytes);
                    thirdbytes.Clear();
                    if (resthird == null)
                        return (Results.Failed, "Please contact to admin");
                    var jsonthird = JsonConvert.DeserializeObject<Dictionary<string, object>>(resthird);
                    if (jsonthird["status"].Str() != "error")
                        request.Brgy_ThirdCouncilor_sig_URL = (jsonthird["url"].Str()).Replace(_config["Portforwarding:LOCAL"].Str(), _config["Portforwarding:URL"].Str()).Replace("https", "http");
                }
                
            }

            if (!request.Brgy_FourthCouncilor.IsEmpty() && !request.Brgy_FourthCouncilor_sig.IsEmpty())
            {
                byte[] fourthbytes = Convert.FromBase64String(request.Brgy_FourthCouncilor.Str());
                if (fourthbytes.Length == 0)
                    return (Results.Failed, "Make sure selected image is not invalid.");
                var resfourth = await ImgService.SendAsync(fourthbytes);
                fourthbytes.Clear();
                if (resfourth == null)
                    return (Results.Failed, "Please contact to admin");
                var jsonfourth = JsonConvert.DeserializeObject<Dictionary<string, object>>(resfourth);
                if (jsonfourth["status"].Str() != "error")
                    request.Brgy_FourthCouncilor_sig_URL = (jsonfourth["url"].Str()).Replace(_config["Portforwarding:LOCAL"].Str(), _config["Portforwarding:URL"].Str()).Replace("https", "http");
            }

            if (!request.Brgy_FifthCouncilor.IsEmpty() && !request.Brgy_FifthCouncilor_sig.IsEmpty())
            {
                byte[] fifthbytes = Convert.FromBase64String(request.Brgy_FifthCouncilor_sig.Str());
                if (fifthbytes.Length == 0)
                    return (Results.Failed, "Make sure selected image is not invalid.");
                var resfifth = await ImgService.SendAsync(fifthbytes);
                fifthbytes.Clear();
                if (resfifth == null)
                    return (Results.Failed, "Please contact to admin");
                var jsonfifth = JsonConvert.DeserializeObject<Dictionary<string, object>>(resfifth);
                if (jsonfifth["status"].Str() != "error")
                    request.Brgy_FifthCouncilor_sig_URL = (jsonfifth["url"].Str()).Replace(_config["Portforwarding:LOCAL"].Str(), _config["Portforwarding:URL"].Str()).Replace("https", "http");
            }

            if (!request.Brgy_SixthCouncilor.IsEmpty() && !request.Brgy_SixthCouncilor_sig.IsEmpty())
            {
                byte[] sixthbytes = Convert.FromBase64String(request.Brgy_SixthCouncilor_sig.Str());
                if (sixthbytes.Length == 0)
                    return (Results.Failed, "Make sure selected image is not invalid.");
                var ressixth = await ImgService.SendAsync(sixthbytes);
                sixthbytes.Clear();
                if (ressixth == null)
                    return (Results.Failed, "Please contact to admin");
                var jsonsixth = JsonConvert.DeserializeObject<Dictionary<string, object>>(ressixth);
                if (jsonsixth["status"].Str() != "error")
                    request.Brgy_SixthCouncilor_sig_URL = (jsonsixth["url"].Str()).Replace(_config["Portforwarding:LOCAL"].Str(), _config["Portforwarding:URL"].Str()).Replace("https", "http");
            }

            if (!request.Brgy_SeventhCouncilor.IsEmpty() && !request.Brgy_SeventhCouncilor_sig.IsEmpty())
            {
                byte[] seventhbytes = Convert.FromBase64String(request.Brgy_SeventhCouncilor_sig.Str());
                if (seventhbytes.Length == 0)
                    return (Results.Failed, "Make sure selected image is not invalid.");
                var resseventh = await ImgService.SendAsync(seventhbytes);
                seventhbytes.Clear();
                if (resseventh == null)
                    return (Results.Failed, "Please contact to admin");
                var jsonseventh = JsonConvert.DeserializeObject<Dictionary<string, object>>(resseventh);
                if (jsonseventh["status"].Str() != "error")
                    request.Brgy_SeventhCouncilor_sig_URL = (jsonseventh["url"].Str()).Replace(_config["Portforwarding:LOCAL"].Str(), _config["Portforwarding:URL"].Str()).Replace("https", "http");
            }

            if (!request.SK_Chairman.IsEmpty() && !request.SK_Chairman_sig.IsEmpty())
            {
                byte[] skchairmanbytes = Convert.FromBase64String(request.SK_Chairman_sig.Str());
                if (skchairmanbytes.Length == 0)
                    return (Results.Failed, "Make sure selected image is not invalid.");
                var resskchairman = await ImgService.SendAsync(skchairmanbytes);
                skchairmanbytes.Clear();
                if (resskchairman == null)
                    return (Results.Failed, "Please contact to admin");
                var jsonskchairman = JsonConvert.DeserializeObject<Dictionary<string, object>>(resskchairman);
                if (jsonskchairman["status"].Str() != "error")
                    request.SK_Chairman_sig_URL = (jsonskchairman["url"].Str()).Replace(_config["Portforwarding:LOCAL"].Str(), _config["Portforwarding:URL"].Str()).Replace("https", "http");
            }
            if (!request.Brgy_Secretary.IsEmpty() && !request.Brgy_Secretary_sig.IsEmpty())
            {
                byte[] secretarybytes = Convert.FromBase64String(request.Brgy_Secretary_sig.Str());
                if (secretarybytes.Length == 0)
                    return (Results.Failed, "Make sure selected image is not invalid.");
                var ressecretary = await ImgService.SendAsync(secretarybytes);
                secretarybytes.Clear();
                if (ressecretary == null)
                    return (Results.Failed, "Please contact to admin");
                var jsonsecretary = JsonConvert.DeserializeObject<Dictionary<string, object>>(ressecretary);
                if (jsonsecretary["status"].Str() != "error")
                    request.Brgy_Secretary_sig_URL = (jsonsecretary["url"].Str()).Replace(_config["Portforwarding:LOCAL"].Str(), _config["Portforwarding:URL"].Str()).Replace("https", "http");
            }
            if (!request.Brgy_Treasurer.IsEmpty() && !request.Brgy_Treasurer_sig.IsEmpty())
            {
                byte[] treasurerbytes = Convert.FromBase64String(request.Brgy_Treasurer_sig.Str());
                if (treasurerbytes.Length == 0)
                    return (Results.Failed, "Make sure selected image is not invalid.");
                var restreasurer = await ImgService.SendAsync(treasurerbytes);
                treasurerbytes.Clear();
                if (restreasurer == null)
                    return (Results.Failed, "Please contact to admin");
                var jsontreasurer = JsonConvert.DeserializeObject<Dictionary<string, object>>(restreasurer);
                if (jsontreasurer["status"].Str() != "error")
                    request.Brgy_Treasurer_sig_URL = (jsontreasurer["url"].Str()).Replace(_config["Portforwarding:LOCAL"].Str(), _config["Portforwarding:URL"].Str()).Replace("https", "http");
            }
            if (!request.Brgy_Chief_Tanod.IsEmpty() && !request.Brgy_Chief_Tanod_sig.IsEmpty())
            {
                byte[] tanodbytes = Convert.FromBase64String(request.Brgy_Chief_Tanod_sig.Str());
                if (tanodbytes.Length == 0)
                    return (Results.Failed, "Make sure selected image is not invalid.");
                var restanod = await ImgService.SendAsync(tanodbytes);
                tanodbytes.Clear();
                if (restanod == null)
                    return (Results.Failed, "Please contact to admin");
                var jsontanod = JsonConvert.DeserializeObject<Dictionary<string, object>>(restanod);
                if (jsontanod["status"].Str() != "error")
                    request.Brgy_Chief_Tanod_sig_URL = (jsontanod["url"].Str()).Replace(_config["Portforwarding:LOCAL"].Str(), _config["Portforwarding:URL"].Str()).Replace("https", "http");
            }
            
            if(!request.Brgy_Captain_sig_URL.IsEmpty() ||
                !request.Brgy_FirstCouncilor_sig_URL.IsEmpty() ||
                !request.Brgy_SecondCouncilor_sig_URL.IsEmpty() ||
                !request.Brgy_ThirdCouncilor_sig_URL.IsEmpty() ||
                !request.Brgy_FourthCouncilor_sig_URL.IsEmpty() ||
                !request.Brgy_FifthCouncilor_sig_URL.IsEmpty() ||
                !request.Brgy_SixthCouncilor_sig_URL.IsEmpty() ||
                !request.Brgy_SeventhCouncilor_sig_URL.IsEmpty() ||
                !request.SK_Chairman_sig_URL.IsEmpty() ||
                !request.Brgy_Secretary_sig_URL.IsEmpty() ||
                !request.Brgy_Treasurer_sig_URL.IsEmpty() ||
                !request.Brgy_Chief_Tanod_sig_URL.IsEmpty()
                )
                return (Results.Success, null);
            return (Results.Null, "Make sure selected one of signatory Signature is valid");
        }
    }
}
