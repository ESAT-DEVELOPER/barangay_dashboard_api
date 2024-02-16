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

namespace webapi.Controllers.STLPartylistDashboardContorller.Features
{
    [Route("app/v1/stldashboard")]
    [ApiController]
    [ServiceFilter(typeof(SubscriberAuthenticationAttribute))]
    public class ProfilePictureHistoryController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IProfilePictureHistoryRepository _repo;
        public ProfilePictureHistoryController(IConfiguration config, IProfilePictureHistoryRepository repo)
        {
            _config = config;
            _repo = repo;
        }

        [HttpPost]
        [Route("profilepicture/upload")]
        public async Task<IActionResult> Task01([FromBody] Profile_Picture request)
        {
            var valresult = await validity(request);
            if (valresult.result == Results.Failed)
                return Ok(new { Status = "error", Message = valresult.message });
            if (valresult.result != Results.Success)
                return NotFound();

            var repoResult = await _repo.UploadProfilePictureAsync(request);
            if (repoResult.result == Results.Success)
                return Ok(new { Status = "ok", PRF_PIC = request.ImageURL, Message = repoResult.message });
            else if (repoResult.result == Results.Failed)
                return Ok(new { Status = "error", Message = repoResult.message });
            return NotFound();
        }
        [HttpPost]
        [Route("profilepicture/change")]
        public async Task<IActionResult> Task02([FromBody] Profile_Picture request)
        {
            var repoResult = await _repo.ChangeProfilePicture(request);
            if (repoResult.result == Results.Success)
                return Ok(new { Status = "ok", PRF_PIC = request.ImageURL, Message = repoResult.message });
            else if (repoResult.result == Results.Failed)
                return Ok(new { Status = "error", Message = repoResult.message });
            return NotFound();
        }

        private async Task<(Results result, string message)> validity(Profile_Picture request)
        {
            if(request==null)
                return (Results.Null, null);
            if(request.ImageBase64.IsEmpty())
                return (Results.Failed, "No Picture Uploaded");
            if (!request.ImageBase64.IsEmpty())
            {
                byte[] bytes = Convert.FromBase64String(request.ImageBase64.Str());
                if (bytes.Length == 0)
                    return (Results.Failed, "Make sure selected image is not invalid.");
                var res = await ImgService.SendAsync(bytes);
                bytes.Clear();
                if(res==null)
                    return (Results.Failed, "Please contact to admin");
                var json = JsonConvert.DeserializeObject<Dictionary<string, object>>(res);
                if (json["status"].Str() != "error")
                {
                    //request.ImageURL = json["url"].Str();
                    request.ImageURL = (json["url"].Str()).Replace(_config["Portforwarding:LOCAL"].Str(), _config["Portforwarding:URL"].Str()).Replace("https", "http");
                    return (Results.Success, null);
                }
            }
            return (Results.Null, "Make sure selected image is invalid");
        }
    }
}
