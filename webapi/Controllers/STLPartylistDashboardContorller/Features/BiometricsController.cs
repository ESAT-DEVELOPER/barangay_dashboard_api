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
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Net;
using Newtonsoft.Json.Linq;
using com.innovatrics.enrollment;
using Renci.SshNet;
using System.Drawing;
using System.Linq;

namespace webapi.Controllers.STLPartylistDashboardContorller.Features
{
    [Route("app/v1/stldashboard")]
    [ApiController]
    [ServiceFilter(typeof(SubscriberAuthenticationAttribute))]
    public class BiometricsController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IBiometricsRepository _repo;
        private static string token = null;
        private static string APIUrl = null;

        public BiometricsController(IConfiguration config, IBiometricsRepository repo)
        {
            _config = config;
            _repo = repo;
        }

        [HttpPost]
        [Route("biometrics/registration")]
        public async Task<IActionResult> BiometricsRegistration([FromBody] Biometrics info)
        {
            var valResult = await validityReport(info);
            if (valResult.result == Results.Failed)
                return Ok(new { Status = "error", Message = valResult.message });
            if (valResult.result != Results.Success)
                return NotFound();

            var result = await _repo.biometricsRegistration(info);
            if(result.status == Results.Success)
                return Ok(new { status = result.status, message = result.message });
            return Ok(new { status = Results.Failed, message = "Failed"});
        }

        [HttpPost]
        [Route("fingerprint/search")]
        public async Task<IActionResult> Biometrics([FromBody] Biometrics info)
        {
            try
            {

                List<dynamic> fingerprints = new List<dynamic>();

                var results = await _repo.searchFingerprint(ConvertToDBColName(info.FingerprintPosition));
                if (results.result == Results.Success)
                    fingerprints = (List<dynamic>)results.fingerprints;

                var reg_fingerprints = fingerprints.Select(x => new { 
                    UserId = x.USR_ID,
                    BiometricsId = x.BMTRCS_ID,
                    Fullname = x.FLL_NM,
                    fingerprints = new List<string>()
                    {
                        x ?.LFT_THMB ?? null,
                        x ?.LFT_NDX ?? null,
                        x ?.LFT_MDL ?? null,
                        x ?.LFT_RNG ?? null,
                        x ?.LFT_PNK ?? null,
                        x ?.RGHT_THMB ?? null,
                        x ?.RGHT_NDX ?? null,
                        x ?.RGHT_MDL ?? null,
                        x ?.RGHT_RNG ?? null, 
                        x ?.RGHT_PNK ?? null
                    }                
                }).ToList();

                PrintPosition position;
                string searchFingerprint;

                switch (info.FingerprintPosition)
                {
                    case "LEFT_THUMB": position = PrintPosition.LEFT_THUMB; break;
                    case "LEFT_INDEX": position = PrintPosition.LEFT_INDEX; break;
                    case "LEFT_MIDDLE": position = PrintPosition.LEFT_MIDDLE; break;
                    case "LEFT_RING": position = PrintPosition.LEFT_RING; break;
                    case "LEFT_LITTLE": position = PrintPosition.LEFT_LITTLE; break;
                    case "RIGHT_THUMB": position = PrintPosition.RIGHT_THUMB; break;
                    case "RIGHT_INDEX": position = PrintPosition.RIGHT_INDEX; break;
                    case "RIGHT_MIDDLE": position = PrintPosition.RIGHT_MIDDLE; break;
                    case "RIGHT_RING": position = PrintPosition.RIGHT_RING; break;
                    case "RIGHT_LITTLE": position = PrintPosition.RIGHT_LITTLE; break;
                    default: position = PrintPosition.LEFT_THUMB; break;
                }
                var result = await VerifyFingerprint(reg_fingerprints, info.SearchFingerprint, position);

                if(((dynamic)result).percentage > 49)
                    return Ok(new { status = Results.Success, message = "Match found", searchResult = result });
                return Ok(new { status = Results.Failed, message = "No Match" });
            }
            catch (Exception e)
            {
                return Ok(new
                {
                    respCode = 2001,
                    respMessage = e.ToString()
                });
            }
        }

        private async Task<object> VerifyFingerprint(object reg_fingerprints, string searchFingerprint, PrintPosition position)
        {
            try
            {
                decimal percentage = 0;
                System.IO.File.WriteAllBytes(AppDomain.CurrentDomain.BaseDirectory + "fingerprint/SEARCH_FINGERPRINT.png", Convert.FromBase64String(searchFingerprint));


                foreach (var item in (dynamic)reg_fingerprints)
                {

                    foreach (var print in item.fingerprints)
                    {
                        if (print == null) continue;
                        SaveImageFile(print);

                        string first = AppDomain.CurrentDomain.BaseDirectory + @"fingerprint\SEARCH_FINGERPRINT.png";
                        string second = AppDomain.CurrentDomain.BaseDirectory + @"fingerprint\REGISTERED_FINGERPRINT.png";

                        PngImageEncoder pngImageEncoder = new PngImageEncoder();

                        //Registered Fingerprint
                        var printImg1 = Image.Decode(BinaryFile.ReadAll(first));
                        var print1 = new Print(printImg1, position);
                        var extractor = new ICSExtractor();
                        var extractedPrint1 = extractor.Extract(print1);

                        var probe = new Applicant();
                        probe.AddPrint(extractedPrint1);

                        //Current Fingerprint
                        var printImg2 = Image.Decode(BinaryFile.ReadAll(second));
                        var print2 = new Print(printImg2, position);
                        var extractedPrint2 = extractor.Extract(print2);

                        var gallery = new Applicant();
                        gallery.AddPrint(extractedPrint2);

                        var matcher = new ApplicantICSVerifyMatcher();
                        var scores = probe.SimilarWith(gallery, matcher);

                        percentage = (decimal)(((double)scores.GetPrintScore() / 100) * 10);

                        //System.IO.File.Delete(first);
                        System.IO.File.Delete(second);
                        if (percentage > 49)
                        {
                            System.IO.File.Delete(first);
                            return new { fullname = item.Fullname, percentage = percentage };
                        }
                    }

                }

                return new { fullname="", percentage = 0 };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return 0;
            }
        }

        private void SaveImageFile(string searchFingerprint)
        {
            using (WebClient client = new WebClient())
            {
                if(searchFingerprint != null)
                    client.DownloadFile(new Uri(searchFingerprint), AppDomain.CurrentDomain.BaseDirectory + @"fingerprint\REGISTERED_FINGERPRINT.png");
            }
        }

        private string ConvertToDBColName(string printPosition)
        {
            switch(printPosition)
            {
                case "LEFT_THUMB": return "LFT_THMB";
                case "LEFT_INDEX": return "LFT_NDX";
                case "LEFT_MIDDLE": return "LFT_MDL";
                case "LEFT_RING": return "LFT_RNG";
                case "LEFT_LITTLE": return "LFT_PNK";
                case "RIGHT_THUMB": return "RGHT_THMB";
                case "RIGHT_INDEX": return "RGHT_NDX";
                case "RIGHT_MIDDLE": return "RGHT_MDL";
                case "RIGHT_RING": return "RGHT_RNG";
                case "RIGHT_LITTLE": return "RGHT_PNK";
                default: return null;
            }
        }



        //function methods
        private async Task<(Results result, String message)> validityReport(Biometrics request)
        {
            List<string> tempList = new List<string>();
            if (request == null)
                return (Results.Null, null);
            if (request.Fingerprints.Count < 1)
                return (Results.Success, null);
            byte[] bytes = null;
            foreach (var item in request.Fingerprints)
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
                {
                    string url = (json["url"].Str()).Replace(_config["Portforwarding:LOCAL"].Str(), _config["Portforwarding:URL"].Str()).Replace("https", "http");
                    tempList.Add(url);
                    //tempList.Add(json["url"].Str().Replace("www.", ""));
                }
                    
            }
            request.Fingerprints = null;
            request.Fingerprints = tempList;
            return (Results.Success, null);
        }
    }
}
