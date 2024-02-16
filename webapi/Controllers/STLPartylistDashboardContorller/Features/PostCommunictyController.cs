using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using webapi.App.Aggregates.SubscriberAppAggregate.Common;
using Microsoft.Extensions.Configuration;
using webapi.App.Aggregates.STLPartylistDashboard.Features;
using webapi.App.STLDashboardModel;
using webapi.App.Aggregates.Common;
using webapi.App.RequestModel.Common;
using Comm.Commons.Extensions;
using webapi.App.Features.UserFeature;
using Newtonsoft.Json;

namespace webapi.Controllers.STLPartylistDashboardContorller.Features
{
    [Route("app/v1/stldashboard")]
    [ApiController]
    [ServiceFilter(typeof(SubscriberAuthenticationAttribute))]
    public class PostCommunictyController:ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IPostCommunityRepository _repo;
        public PostCommunictyController(IConfiguration config, IPostCommunityRepository repo)
        {
            _config = config;
            _repo = repo;
        }
        [HttpPost]
        [Route("postcommunity/new")]
        public async Task<IActionResult> PostCommunityAsync([FromBody] cPostCommunity request)
        {
            var valres = await validity(request);
            if (valres.result == Results.Failed)
                return Ok(new { result = "error", Message = valres.message });
            var result = await _repo.PostCommunityAsync(request);
            if (result.result == Results.Success)
            {
                return Ok(new { Status = "ok", Message = result.message, Content = request });
            }
            else if (result.result == Results.Failed)
                return Ok(new { Status = "error", Message = result.message, Content = request });
            return NotFound();
        }
        [HttpPost]
        [Route("postcommunity/update")]
        public async Task<IActionResult> UpdatePostCommunityAsync([FromBody] cPostCommunity request)
        {
            var valres = await validity(request);
            if (valres.result == Results.Failed)
                return Ok(new { result = "error", Message = valres.message });
            var result = await _repo.UpdatePostCommunityAsync(request);
            if (result.result == Results.Success)
            {
                return Ok(new { Status = "ok", Message = result.message, Content = request });
            }
            else if (result.result == Results.Failed)
                return Ok(new { Status = "error", Message = result.message, Content = request });
            return NotFound();
        }
        [HttpPost]
        [Route("postcommunity/accept")]
        public async Task<IActionResult> AcceptRequestCommunityAsync([FromBody] AcptCommunity request)
        {
            var result = await _repo.AcceptRequestCommunityAsync(request);
            if (result.result == Results.Success)
            {
                return Ok(new { Status = "ok", Message = result.message });
            }
            else if (result.result == Results.Failed)
                return Ok(new { Status = "error", Message = result.message });
            return NotFound();
        }
        [HttpPost]
        [Route("postcommunity/list")]
        public async Task<IActionResult> LoadPostCommunityListAsync([FromBody] FilterRequest request)
        {
            var result = await _repo.LoadPostCommunityListAsync(request);
            if (result.result == Results.Success)
            {
                return Ok(result.comm);
            }
            return NotFound();
        }

        [HttpPost]
        [Route("commentpostcommunity/remove")]
        public async Task<IActionResult> RemoveCommentPostCommunityAsync([FromBody] cCommentPostCommunity request)
        {
            var result = await _repo.RemoveCommentPostCommunityAsync(request);
            if (result.result == Results.Success)
            {
                return Ok(new { Status = "ok", Message = result.message });
            }
            else if (result.result == Results.Failed)
                return Ok(new { Status = "error", Message = result.message });
            return NotFound();
        }


        [HttpPost]
        [Route("commentpostcommunity/list")]
        public async Task<IActionResult> LoadCommentPostCommunityListAsync([FromBody] FilterRequest request)
        {
            var result = await _repo.LoadCommentPostCommunityListAsync(request);
            if (result.result == Results.Success)
                return Ok(result.comment);
            else if (result.result != Results.Null)
                return Ok(result.comment);
            return NotFound();
        }

        private async Task<(Results result, string message)> validity(cPostCommunity req)
        {
            if (req == null)
                return (Results.Null, "Make sure uploaded image is valid");
            if (!req.URL.IsEmpty())
                return (Results.Success, null);
            if (!req.ImgURL.IsEmpty())
            {
                if (req.ImgURL.StartsWith("http"))
                {
                    req.URL = req.ImgURL;
                    return (Results.Success, null);
                }
                req.ImgURL = "";
                req.URL = "";
                return (Results.Success, null);
            }
            else
            {
                byte[] bytes = Convert.FromBase64String(req.base64Attachment.Str());
                if(bytes.Length == 0)
                {
                    req.URL = "";
                    req.ImgURL = "";
                    return (Results.Success, null);
                }
                var res = await ImgService.SendAsync(bytes);
                bytes.Clear();
                if (res == null)
                    return (Results.Failed, "Please contact to admin");
                var json = JsonConvert.DeserializeObject<Dictionary<string, object>>(res);
                if(json["status"].Str() != "error")
                {
                    req.URL = (json["url"].Str()).Replace(_config["Portforwarding:LOCAL"].Str(), _config["Portforwarding:URL"].Str()).Replace("https", "http");
                    req.ImgURL = req.URL;
                    return (Results.Success, null);
                }
            }
            return (Results.Null, "Make sure uploaded image is valid");
        }
    }
}
