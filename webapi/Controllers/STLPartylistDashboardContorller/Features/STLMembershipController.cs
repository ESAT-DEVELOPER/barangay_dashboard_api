using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using webapi.App.Aggregates.Common;
using webapi.App.Aggregates.STLPartylistDashboard.Features;
using webapi.App.RequestModel.AppRecruiter;
using Comm.Commons.Extensions;
using webapi.App.Features.UserFeature;
using Newtonsoft.Json;
using webapi.App.Aggregates.STLPartylistDashboard;
using webapi.App.Model.User;
using System.Security.Claims;
using Comm.Commons.Advance;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using webapi.App.Aggregates.Common.Dto;
using System.Text;
using webapi.App.Aggregates.SubscriberAppAggregate.Common;
using webapi.App.RequestModel.Common;
using webapi.App.STLDashboardModel;

namespace webapi.Controllers.STLPartylistMembership.Features
{
    [Route("app/v1/stldashboard")]
    [ApiController]
    [ServiceFilter(typeof(SubscriberAuthenticationAttribute))]
    public class STLMembershipController:ControllerBase
    {

        private readonly IConfiguration _config;
        private readonly ISTLMembershipRepository _repo;
        private readonly IAccountRepository _loginrepo;
        public STLMembershipController(IConfiguration config,  ISTLMembershipRepository repo, IAccountRepository loginrepo)
        {
            _config = config;
            _repo = repo;
            _loginrepo = loginrepo;
        }
        [HttpPost]
        [Route("accountmenu")]
        public async Task<IActionResult> Task0a()
        {
            var result = await _repo.LoadAccountAccess();
            if (result.result == Results.Success)
                return Ok(result.account);
            return NotFound();
        }
        [HttpPost]
        [Route("membership/new")]
        public async Task<IActionResult> Task0b([FromBody] STLMembership request)
        {
            var reporesult = await _repo.MembershipAsync(request);
            if (reporesult.result == Results.Success)
                return Ok(new { result = reporesult.result, Message = reporesult.message });
            else if (reporesult.result == Results.Failed)
                return Ok(new { Status = "error", Message = reporesult.message });
            return NotFound();
        }

        [HttpPost]
        [Route("membership/decd")]
        public async Task<IActionResult> Task0b1([FromBody] DOD req)
        {
            var reporesult = await _repo.ResidenceDODAsyn(req);
            if (reporesult.result == Results.Success)
                return Ok(new { Status = "ok", Message = reporesult.message, Content=req });
            else if (reporesult.result == Results.Failed)
                return Ok(new { Status = "error", Message = reporesult.message });
            return NotFound();
        }


        [HttpPost]
        [Route("deathcertificate/export")]
        public async Task<IActionResult> Task0a1([FromBody] DOD request)
        {
            var valresult = await validity_exportdocument(request);
            if (valresult.result == Results.Failed)
                return Ok(new { Status = "error", Message = valresult.message });
            if (valresult.result != Results.Success)
                return NotFound();

            var result = await _repo.Generate_DeathCertificate(request);
            if (result.result == Results.Success)
                return Ok(new { Status = "ok", Message = result.message });
            if (result.result == Results.Failed)
                return Ok(new { Status = "error", Message = result.message });
            return NotFound();
        }

        [HttpPost]
        [Route("deathcertificate/release")]
        public async Task<IActionResult> Death_Certificate([FromBody] DOD request)
        {
            var result = await _repo.Release_DeathCertificateAsync(request);
            if (result.result == Results.Success)
                return Ok(new { Status = "ok", Message = result.message, Release = result.release });
            if (result.result == Results.Failed)
                return Ok(new { Status = "error", Message = result.message });
            return NotFound();
        }
        private async Task<(Results result, string message)> validityReport_DeathCertificate(DOD request)
        {
            if (request == null)
                return (Results.Null, null);
            if (request.ExportedDocument.IsEmpty())
                return (Results.Success, null);
            byte[] bytes = Convert.FromBase64String(request.ExportedDocument.Str());
            if (bytes.Length == 0)
                return (Results.Failed, "Make sure you have internet connection.");
            var res = await ReportService.SendAsync(bytes, $"deathcertificate_{request.PLID}{request.PGRPID}{request.DeathCertificateNo}");
            bytes.Clear();
            if (res == null)
                return (Results.Failed, "Please contact to admin.");
            var json = JsonConvert.DeserializeObject<Dictionary<string, object>>(res);
            if (json["status"].Str() != "error")
            {
                //request.URLDocument = json["url"].Str();
                request.URLDocument = (json["url"].Str()).Replace(_config["Portforwarding:LOCAL"].Str(), _config["Portforwarding:URL"].Str());
                return (Results.Success, null);
            }
            return (Results.Null, "Make sure you have internet connection.");
        }

        [HttpPost]
        [Route("logout")]
        public async Task<IActionResult> Log_out()
        {
            var reporesult = await _repo.LogOutAsyn();
            if (reporesult.result == Results.Success)
                return Ok(new { Status = "ok", Message = reporesult.message });
            else if (reporesult.result == Results.Failed)
                return Ok(new { Status = "error", Message = reporesult.message });
            return NotFound();
        }

        [HttpPost]
        [Route("registration/new")]
        public async Task<IActionResult> Task0c([FromBody] STLMembership request)
        {
            var valresult = await validity(request);
            if (valresult.result == Results.Failed)
                return Ok(new { result = "error", Message = valresult.message });
            if (valresult.result != Results.Success)
                return NotFound();
            var valresult1 = await validitysignatue(request);
            if (valresult1.result == Results.Failed)
                return Ok(new { result = "error", Message = valresult.message });
            if (valresult1.result != Results.Success)
                return NotFound();

            var valresult2 = await validitygov(request);
            if (valresult2.result == Results.Failed)
                return Ok(new { result = "error", Message = valresult.message });
            if (valresult2.result != Results.Success)
                return NotFound();

            var reporesult = await _repo.MembershipAsync(request, true);
            if (reporesult.result == Results.Success)
                return Ok(new { result = reporesult.result, Message = reporesult.message, User_ID = reporesult.UserID ,AcctID = reporesult.AcctID, profilepicture=request.ImageUrl });
            else if (reporesult.result == Results.Failed)
                return Ok(new { result = reporesult.result, Message = reporesult.message });
            return NotFound();
        }

        [HttpPost]
        [Route("registration/edit")]
        public async Task<IActionResult> Task0c1([FromBody] STLMembership request)
        {
            var valresult = await validity(request);
            //if (valresult.result == Results.Failed)
            //    return Ok(new { Status = "error", Message = valresult.message });
            //if (valresult.result != Results.Success)
            //    return NotFound();
            var valresult1 = await validitysignatue(request);
            if (valresult1.result == Results.Failed)
                return Ok(new { Status = "error", Message = valresult.message });
            if (valresult1.result != Results.Success)
                return NotFound();

            var valresult2 = await validitygov(request);
            if (valresult2.result == Results.Failed)
                return Ok(new { Status = "error", Message = valresult.message });
            if (valresult2.result != Results.Success)
                return NotFound();


            if (request.ImageList != null)
            {
                var valResult = await validityReport(request);
                if (valResult.result == Results.Failed)
                    return Ok(new { Status = "error", Message = valResult.message });
                if (valResult.result != Results.Success)
                    return NotFound();
            }
            

            var reporesult = await _repo.MembershipAsync(request,true);
            if (reporesult.result == Results.Success)
                return Ok(new { result = reporesult.result, Message = reporesult.message, User_ID = reporesult.UserID, AcctID = reporesult.AcctID, profilepicture = request.ImageUrl });
            else if (reporesult.result == Results.Failed)
                return Ok(new { result = reporesult.result, Message = reporesult.message });
            return NotFound();
        }

        [HttpPost]
        [Route("account/access")]
        public async Task<IActionResult> Task0d(string userid)
        {
            var result = await _repo.LoadUserAccess(userid);
            if (result.result == Results.Success)
                return Ok(result.access);
            return NotFound();
        }

        [HttpPost]
        [Route("account/skin")]
        public async Task<IActionResult> Task0d01(string skin)
        {
            var result = await _repo.AssigendSkin(skin);
            if (result.result == Results.Success)
                return Ok(new { Status = "ok", Message=result.message});
            return NotFound();
        }
        [HttpPost]
        [Route("account/assignedaccess")]
        public async Task<IActionResult> Task0e(string userid, string menu)
        {
            var result = await _repo.AssignedAccess(userid, menu);
            if (result.result == Results.Success)
                return Ok(new { result=result.result,message=result.message});
            return NotFound();
        }

        [HttpPost]
        [Route("memberaccount")]
        public async Task<IActionResult> Task0f(string search)
        {
            var result = await _repo.LoadAccount(search);
            if (result.result == Results.Success)
                return Ok(result.account);
            return NotFound();
        }
        [HttpPost]
        [Route("selectmemberaccount")]
        public async Task<IActionResult> Task0f1(string search)
        {
            var result = await _repo.SelectAccount(search);
            if (result.result == Results.Success)
                return Ok(result.account);
            return NotFound();
        }
        [HttpPost]
        [Route("memberaccountnotelected")]
        public async Task<IActionResult> Task0f01(string search)
        {
            var result = await _repo.LoadAccount_NotElected(search);
            if (result.result == Results.Success)
                return Ok(result.account);
            return NotFound();
        }

        [HttpPost]
        [Route("dashboarduser")]
        public async Task<IActionResult> Task0f1(string plid, string pgrpid)
        {
            var result = await _repo.DashboardUserAccount(plid, pgrpid);
            if (result.result == Results.Success)
                return Ok(result.account);
            return NotFound();
        }

        [HttpPost]
        [Route("householdaccount")]
        public async Task<IActionResult> Task01(string search)
        {
            var result = await _repo.LoadAccountSearch(search);
            if (result.result == Results.Success)
                return Ok(result.account);
            return NotFound();
        }
        [HttpPost]
        [Route("masterlist")]
        public async Task<IActionResult> Task0g()
        {
            var result = await _repo.LoadMasterList();
            if (result.result == Results.Success)
                return Ok(result.account);
            return NotFound();
        }
        [HttpPost]
        [Route("masterlist1")]
        public async Task<IActionResult> Task0g1([FromBody] MasterListRequest request) 
        {
            var result = await _repo.LoadMasterList1(request);
            if (result.result == Results.Success)
                return Ok(result.account);
            return NotFound();
        }

        [HttpPost]
        [Route("account/id")]
        public async Task<IActionResult> Task0g1A([FromBody] MasterListRequest request)
        {
            var result = await _repo.LoadMasterList2(request);
            if (result.result == Results.Success)
                return Ok(result.account);
            return NotFound();
        }

        [HttpPost]
        [Route("totalregister")]
        public async Task<IActionResult> Task0g2()
        {
            var result = await _repo.TotalREgister();
            if (result.result == Results.Success)
                return Ok(result.account);
            return NotFound();
        }

        [HttpPost]
        [Route("parent")]
        public async Task<IActionResult> Task0g3([FromBody] FilterRequest request)
        {
            var result = await _repo.Load_Parent(request);
            if (result.result == Results.Success)
                return Ok(result.parent);
            return NotFound();
        }
        [HttpPost]
        [Route("account/validity")]
        public async Task<IActionResult> Task0h([FromBody] ValidityAccount request)
        {
            var result = await _repo.UpdateValidityAccount(request);
            if (result.result == Results.Success)
                return Ok(new { result = result.result, message = result.message });
            return NotFound();
        }
        [HttpPost]
        [Route("changeaccountype")]
        public async Task<IActionResult> Task0i([FromBody] ChangeAccountType request)
        {
            var result = await _repo.ChangeAccounttype(request);
            if (result.result == Results.Success)
                return Ok(new { result = result.result, message = result.message });
            else if(result.result!=Results.Null)
                return Ok(new { result = result.result, message = result.message });
            return NotFound();
        }

        [HttpPost]
        [Route("children")]
        public async Task<IActionResult> Task0j(string userid)
        {
            var result = await _repo.Load_Children(userid);
            if (result.result == Results.Success)
                return Ok(result.children);
            return NotFound();
        }
        [HttpPost]
        [Route("educatt")]
        public async Task<IActionResult> Task0k(string plid, string pgrpid, string userid)
        {
            var result = await _repo.Load_EducationalAttainment(plid, pgrpid, userid);
            if (result.result == Results.Success)
                return Ok(result.educatt);
            return NotFound();
        }
        [HttpPost]
        [Route("emphist")]
        public async Task<IActionResult> Task0l(string plid, string pgrpid, string userid)
        {
            var result = await _repo.Load_EmploymentHistory(plid, pgrpid, userid);
            if (result.result == Results.Success)
                return Ok(result.emphist);
            return NotFound();
        }
        [HttpPost]
        [Route("residence/organization")]
        public async Task<IActionResult> Task0m(string plid, string pgrpid, string userid)
        {
            var result = await _repo.Load_Organization(plid, pgrpid, userid);
            if (result.result == Results.Success)
                return Ok(result.orgz);
            return NotFound();
        }
        [HttpPost]
        [Route("profilepicture/history")]
        public async Task<IActionResult> Task0n(string plid, string pgrpid, string userid)
        {
            var result = await _repo.Load_ProfilePictureHistory(plid, pgrpid, userid);
            if (result.result == Results.Success)
                return Ok(result.prfpic);
            return NotFound();
        }
        [HttpPost]
        [Route("proffesion")]
        public async Task<IActionResult> Task0o(string search)
        {
            var result = await _repo.Load_Profession(search);
            if (result.result == Results.Success)
                return Ok(result.prof);
            return NotFound();
        }
        [HttpPost]
        [Route("occupation")]
        public async Task<IActionResult> Task0p(string search)
        {
            var result = await _repo.Load_Occupation(search);
            if (result.result == Results.Success)
                return Ok(result.occ);
            return NotFound();
        }
        [HttpPost]
        [Route("skills")]
        public async Task<IActionResult> Task0q(string search)
        {
            var result = await _repo.Load_Skills(search);
            if (result.result == Results.Success)
                return Ok(result.skl);
            return NotFound();
        }

        //validitysignature
        private async Task<(Results result, string message)> validitysignatue(STLMembership request)
        {
            if (request == null)
                return (Results.Null, null);
            if (!request.SignatureURL.IsEmpty())
                return (Results.Success, null);
            if (request.Signature.IsEmpty())
                //return (Results.Failed, "Please select an Signature.");
                return (Results.Success, "Please select an Signature.");
            if (request.Signature.StartsWith("http"))
            {
                request.SignatureURL = request.Signature;
                return (Results.Success, null);
            }
            else
            {
                byte[] bytes = Convert.FromBase64String(request.Signature.Str());
                if (bytes.Length == 0)
                    return (Results.Failed, "Make sure selected image is invalid.");
                var res = await ImgService.SendAsync(bytes);
                bytes.Clear();
                if (res == null)
                    return (Results.Failed, "Please contact to admin.");
                var json = JsonConvert.DeserializeObject<Dictionary<string, object>>(res);
                if (json["status"].Str() != "error")
                {
                    //request.SignatureURL = json["url"].Str();
                    request.SignatureURL = (json["url"].Str()).Replace(_config["Portforwarding:LOCAL"].Str(), _config["Portforwarding:URL"].Str());
                    return (Results.Success, null);
                }
            }
            return (Results.Null, "Make sure selected image is invalid");
        }

        private async Task<(Results result, string message)> validity(STLMembership request)
        {
            if (request == null)
                return (Results.Null, null);
            if (!request.ImageUrl.IsEmpty())
                return (Results.Success, null);
            if (request.Img.IsEmpty())
            {
                request.ImageUrl = "";
                return (Results.Success, null);
            }
            if (request.Img.StartsWith("http"))
            {
                request.ImageUrl = request.Img;

                return (Results.Success, null);
            }
            else
            {
                byte[] bytes = Convert.FromBase64String(request.Img.Str());
                if (bytes.Length == 0)
                {
                    request.ImageUrl = "";
                    return (Results.Success, null);
                }

                var res = await ImgService.SendAsync(bytes);
                bytes.Clear();
                if (res == null)
                    return (Results.Failed, "Please contact to admin.");
                var json = JsonConvert.DeserializeObject<Dictionary<string, object>>(res);
                if (json["status"].Str() != "error")
                {
                    //request.ImageUrl = json["url"].Str();
                    request.ImageUrl = (json["url"].Str()).Replace(_config["Portforwarding:LOCAL"].Str(), _config["Portforwarding:URL"].Str());
                    return (Results.Success, null);
                }
            }
            return (Results.Null, "Make sure selected image is valid");
        }

        private async Task<(Results result, string message)> validity_exportdocument(DOD request)
        {
            if (request == null)
                return (Results.Null, null);
            if (request.ExportedDocument.IsEmpty())
                return (Results.Success, null);
            byte[] bytes = Convert.FromBase64String(request.ExportedDocument.Str());
            if (bytes.Length == 0)
                return (Results.Failed, "Make sure you have internet connection.");
            var res = await ReportService.SendAsync(bytes, $"deathcertificate_{request.PLID}{request.PGRPID}{request.DeathCertificateNo}");
            bytes.Clear();
            if (res == null)
                return (Results.Failed, "Please contact to admin.");
            var json = JsonConvert.DeserializeObject<Dictionary<string, object>>(res);
            if (json["status"].Str() != "error")
            {
                //request.URLDocument = json["url"].Str();
                request.URLDocument = (json["url"].Str()).Replace(_config["Portforwarding:LOCAL"].Str(), _config["Portforwarding:URL"].Str());
                return (Results.Success, null);
            }
            return (Results.Null, "Make sure you have internet connection.");
        }

        private async Task<(Results result, string message)> validityReport(STLMembership request)
        {
            List<STLMembership.FingerImage> _tempList = new List<STLMembership.FingerImage>();
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
                    if (item.Image.StartsWith("http"))
                    {
                        dictionary.Add(item.Index, item.Image.Replace("www.", ""));
                    }
                    else
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
                            //dictionary.Add(item.Index, json["url"].Str().Replace("www.", ""));
                            dictionary.Add(item.Index, json["url"].Str().Replace(_config["Portforwarding:LOCAL"].Str(), _config["Portforwarding:URL"].Str()));
                    }
                    
                }
                else
                    dictionary.Add(item.Index, null);

            }
            request.Json = JsonConvert.SerializeObject(dictionary, Formatting.None);
            return (Results.Success, null);
        }

        private async Task<(Results result, string message)> validitygov(STLMembership request)
        {
            List<STLMembership.GovAttachment> _tempList = new List<STLMembership.GovAttachment>();
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            if (request == null)
                return (Results.Null, null);
            if (request.GovIDList.Count < 1)
                return (Results.Success, null);
            byte[] bytes = null;
            StringBuilder sb = new StringBuilder();
            foreach (var item in request.GovIDList)

            {
                if (item.base64stringattachment != null && item.newupload == "1")
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
                        string url = (json["url"].Str()).Replace(_config["Portforwarding:LOCAL"].Str(), _config["Portforwarding:URL"].Str());
                        sb.Append($"<governemntid GOVVAL_ID=\"{item.govvalid}\" GOVVAL_ID_NO=\"{item.govvalid_no}\" ATTACHMENT=\"{url}\" />");
                        //request.GovIDList[i] = url;
                    }
                    else
                        sb.Append($"<governemntid GOVVAL_ID=\"{item.govvalid}\" GOVVAL_ID_NO=\"{item.govvalid_no}\" ATTACHMENT=\"\" />");
                }
                else
                    sb.Append($"<governemntid GOVVAL_ID=\"{item.govvalid}\" GOVVAL_ID_NO=\"{item.govvalid_no}\" ATTACHMENT=\"{item.attachment}\" />");

            }
            if (sb.Length > 0)
            {
                request.iValidGovernmentID = sb.ToString();
                return (Results.Success, null);
            }
            return (Results.Failed, "Make sure selected image is valid.");
        }

        private object CreateToken(STLAccount user)
        {
            var guid = Guid.NewGuid().ToString();
            var claims = new List<Claim>
            {
                new Claim("token", Cipher.Encrypt(JsonConvert.SerializeObject(new{
                    GUID = Cipher.MD5Hash(guid),
                    PL_ID = user.PL_ID,
                    PGRP_ID = user.PGRP_ID,
                    //PSNCD = user.PSN_CD,
                    USR_ID=user.USR_ID,
                    ACT_ID=user.ACT_ID,
                    MOB_NO=user.MOB_NO,
                    ACT_TYP=user.ACT_TYP,
                }), guid)),
                new Claim(ClaimTypes.Name, user.FLL_NM),
                new Claim(JwtRegisteredClaimNames.Jti, guid),
            };

            DateTime now = DateTime.Now;
            string Issuer = _config["TokenSettings:Issuer"]
                , Audience = _config["TokenSettings:Audience"]
                , Key = _config["TokenSettings:Key"];
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key));
            var signInCred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
            var token = new JwtSecurityToken(
                issuer: Issuer,
                audience: Audience,
                notBefore: now,
                expires: now.Add(TimeSpan.FromSeconds(30)),
                claims: claims,
                signingCredentials: signInCred
            );
            return new { Token = new JwtSecurityTokenHandler().WriteToken(token), ExpirationDate = token.ValidTo, };
        }

        [HttpPost]
        [Route("account/getprofiles")]
        public async Task<IActionResult> Profile01()
        {
            var result = await _repo.ProfileGet();
            if (result.result == Results.Success)
                return Ok(result.household);
            return NotFound();
        }

        [HttpPost]
        [Route("account/getsubscriberprofile")]
        public async Task<IActionResult> Profile07()
        {
            var result = await _repo.ProfileGetSubscriberProfile();
            if (result.result == Results.Success)
                return Ok(result.household);
            return NotFound();
        }

        [HttpPost]
        [Route("account/getresidents")]
        public async Task<IActionResult> SystemUser04([FromBody]SelectUser user)
        {
            var result = await _repo.SelectUser(user);
            if (result.result == Results.Success)
                return Ok(result.household);
            return NotFound();
        }

        [HttpPost]
        [Route("account/getsystemusers")]
        public async Task<IActionResult> SystemUser01()
        {
            var result = await _repo.GetSystemUsers();
            if (result.result == Results.Success)
                return Ok(result.household);
            return NotFound();
        }

        [HttpPost]
        [Route("account/getsystemusersingle")]
        public async Task<IActionResult> SystemUser02(string plid, string pgrpid, string userid)
        {
            var result = await _repo.SystemUserGetSingle(plid, pgrpid, userid);
            if (result.result == Results.Success)
                return Ok(result.household);
            return NotFound();
        }

        [HttpPost]
        [Route("account/updatesystemuser")]
        public async Task<IActionResult> SystemUser03([FromBody] SystemUser user)
        {
            //var result = await _repo.AddPosition(position.JsonString);
            var result = await _repo.SystemUserUpdate(user);
            if (result.result == Results.Success)
                return Ok(new { result = result.result, message = result.message });
            return NotFound();
        }

        [HttpPost]
        [Route("account/updatesystemuserpassword")]
        public async Task<IActionResult> SystemUser09(string plid, string pgrpid, string usrid, string password)
        {
            //var result = await _repo.AddPosition(position.JsonString);
            var result = await _repo.SystemUserUpdatePassword(plid, pgrpid, usrid, password);
            if (result.result == Results.Success)
                return Ok(new { result = result.result, message = result.message });
            return NotFound();
        }

        [HttpPost]
        [Route("account/addaccessprofile")]
        public async Task<IActionResult> Profile02([FromBody] AccessProfile profile)
        {
            var result = await _repo.ProfileAdd(profile);
            if (result.result == Results.Success)
                return Ok(new { result = result.result, message = result.message });
            return NotFound();
        }

        [HttpPost]
        [Route("account/systemuserissubscriber")]
        public async Task<IActionResult> SystemUser10()
        {
            var result = await _repo.SystemUserIsSubscriber();
            if (result.result == Results.Success)
                return Ok(new { result = result.result, message = result.message });
            return NotFound();
        }

        [HttpPost]
        [Route("account/updateaccessprofile")]
        public async Task<IActionResult> Profile04([FromBody] AccessProfile profile)
        {
            //var result = await _repo.AddPosition(position.JsonString);
            var result = await _repo.ProfileUpdate(profile);
            if (result.result == Results.Success)
                return Ok(new { result = result.result, message = result.message });
            return NotFound();
        }

        [HttpPost]
        [Route("account/deleteaccessprofile")]
        public async Task<IActionResult> Profile03(string profileid)
        {
            var result = await _repo.ProfileDelete(profileid);
            if (result.result == Results.Success)
                return Ok(new { result = result.result, message = result.message });
            return NotFound();
        }

        [HttpPost]
        [Route("account/deletesystemuser")]
        public async Task<IActionResult> SystemUser04(string userid)
        {
            var result = await _repo.SystemUserDelete(userid);
            if (result.result == Results.Success)
                return Ok(new { result = result.result, message = result.message });
            return NotFound();
        }

        [HttpPost]
        [Route("account/deletesystemuser02")]
        public async Task<IActionResult> SystemUser11(string plid, string pgrpid, string userid)
        {
            var result = await _repo.SystemUserDelete02(plid, pgrpid, userid);
            if (result.result == Results.Success)
                return Ok(new { result = result.result, message = result.message });
            return NotFound();
        }

        [HttpPost]
        [Route("account/addsystemuser")]
        public async Task<IActionResult> SystemUser06([FromBody] SystemUser user)
        {
            var result = await _repo.SystemUserAdd(user);
            if (result.result == Results.Success)
                return Ok(new { result = result.result, message = result.message });
            return NotFound();
        }

        [HttpPost]
        [Route("account/addsystemuser01")]
        public async Task<IActionResult> SystemUser06(string usrid)
        {
            var result = await _repo.SystemUserAdd01(usrid);
            if (result.result == Results.Success)
                return Ok(new { result = result.result, message = result.message });
            return NotFound();
        }

        
    }
}
