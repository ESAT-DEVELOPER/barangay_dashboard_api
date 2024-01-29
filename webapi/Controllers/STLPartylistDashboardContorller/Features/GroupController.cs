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

namespace webapi.Controllers.STLPartylistDashboardContorller.Features
{
    [Route("app/v1/stldashboard")]
    [ApiController]
    [ServiceFilter(typeof(SubscriberAuthenticationAttribute))]
    public class GroupController:ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IGroupRepository _repo;
        public GroupController(IConfiguration config, IGroupRepository repo)
        {
            _config = config;
            _repo = repo;
        }
        [HttpPost]
        [Route("group")]
        public async Task<IActionResult> Group()
        {
            var result = await _repo.Group();
            if (result.result == Results.Success)
                return Ok(result.grp);
            return NotFound();
        }
        [HttpPost]
        [Route("subscriberlist")]
        public async Task<IActionResult> subscriberlist(string search, int row)
        {
            var result = await _repo.Subsrciber_List(search, row);
            if (result.result == Results.Success)
                return Ok(result.grp);
            return NotFound();
        }
        [HttpPost]
        [Route("account")]
        public async Task<IActionResult> Task0g()
        {
            var result = await _repo.LoadAccount();
            if (result.result == Results.Success)
                return Ok(result.accnt);
            return NotFound();
        }

        [HttpPost]
        [Route("group/leader")]
        public async Task<IActionResult> GroupLeader(int usrtype)
        {
            var result = await _repo.GroupLeader(usrtype);
            if (result.result == Results.Success)
                return Ok(result.grp);
            return NotFound();
        }


        [HttpPost]
        [Route("group/barangayprofile")]
        public async Task<IActionResult> BarangayProfile([FromBody] BRGYProfileRequest request)
        {
            var result = await _repo.BarangayProfile(request);
            if (result.result == Results.Success)
                return Ok(result.brgyprof);
            return NotFound();
        }

        [HttpPost]
        [Route("group/barangay/totalvoter")]
        public async Task<IActionResult> BarangayTotalVoter(TotalRegisterVoter request)
        {
            var result = await _repo.TotalRegisterVoter(request);
            if (result.result == Results.Success)
                return Ok(new { result=result.result, message=result.message, brgyprofid=result.brgyprofid});
            return NotFound();
        }

        [HttpPost]
        [Route("group/siteleader")]
        public async Task<IActionResult> SiteLeader(string plid, string pgrpid, string usrid, string num_row, string search)
        {
            var result = await _repo.SiteLeader(plid,pgrpid,usrid, num_row, search);
            if (result.result == Results.Success)
                return Ok(result.ldr);
            return NotFound();
        }
        [HttpPost]
        [Route("group/barangayofficial")]
        public async Task<IActionResult> BarangayOffical(string plid, string pgrpid, string usrid, string num_row, string search)
        {
            var result = await _repo.BarangayOfficial(plid, pgrpid, usrid, num_row, search);
            if (result.result == Results.Success)
                return Ok(result.ldr);
            return NotFound();
        }
        [HttpPost]
        [Route("group/siteleader1")]
        public async Task<IActionResult> SiteLeader1(string plid, string pgrpid, string usrid, string num_row, string search)
        {
            var result = await _repo.SiteLeader1(plid,pgrpid,usrid, num_row, search);
            if (result.result == Results.Success)
                return Ok(result.ldr);
            return NotFound();
        }
        [HttpPost]
        [Route("group/allaccount")]
        public async Task<IActionResult> AllAccount()
        {
            var result = await _repo.AllAccount();
            if (result.result == Results.Success)
                return Ok(result.act);
            return NotFound();
        }

        [HttpPost]
        [Route("group/leaderaccount")]
        public async Task<IActionResult> LeaderAccount(string groupid)
        {
            var result = await _repo.LeaderAccount(groupid);
            if (result.result == Results.Success)
                return Ok(result.act);
            return NotFound();
        }
        [HttpPost]
        [Route("group/new")]
        public async Task<IActionResult> Task0a([FromBody] STLMembership group) 
        {
            var result = await _repo.GroupAsyn(group);
            if (result.result == Results.Success)
                return Ok(new { result = result.result, message = result.message });
            if (result.result == Results.Failed)
                return Ok(new { result = result.result, message = result.message });
            return NotFound();
        }

        [HttpPost]
        [Route("group/edit")]
        public async Task<IActionResult> Task0b([FromBody] STLMembership group)
        {
            var result = await _repo.GroupAsyn(group,true);
            if (result.result == Results.Success)
                return Ok(new { result = result.result, message = result.message });
            if (result.result == Results.Failed)
                return Ok(new { result = result.result, message = result.message });
            return NotFound();
        }

        [HttpPost]
        [Route("group/blockaccount")]
        public async Task<IActionResult> BlockAccount([FromBody] BlockAccount acct)
        {
            var result = await _repo.BlockAccount(acct);
            if (result.result == Results.Success)
                return Ok(new { result = result.result, message = result.message });
            if (result.result == Results.Failed)
                return Ok(new { result = result.result, message = result.message });
            return NotFound();
        }

        [HttpPost]
        [Route("group/siteleader/member/promote")]
        public async Task<IActionResult> PromoteMember([FromBody] STLMembership request)
        {
            var result = await _repo.PromoteMembertoLeader(request);
            if (result.result == Results.Success)
                return Ok(new { result = result.result, message = result.message });
            else if (result.result == Results.Failed)
                return Ok(new { result = result.result, message = result.message });
            return NotFound();
        }

        [HttpPost]
        [Route("group/changeaccttype")]
        public async Task<IActionResult> ChangeAccountType([FromBody] STLMembership request)
        {
            var result = await _repo.ChangeAccountType(request);
            if (result.result == Results.Success)
                return Ok(new { result = result.result, message = result.message });
            else if (result.result == Results.Failed)
                return Ok(new { result = result.result, message = result.message });
            return NotFound();
        }

        [HttpPost]
        [Route("group/siteleader/leader/downgrade")]
        public async Task<IActionResult> DowngradeLeader([FromBody] STLMembership request)
        {
            var result = await _repo.DowngradeLeaderToMember(request);
            if (result.result == Results.Success)
                return Ok(new { result = result.result, message = result.message });
            else if (result.result == Results.Failed)
                return Ok(new { result = result.result, message = result.message });
            return NotFound();
        }

        [HttpPost]
        [Route("group/siteleader/new")]
        public async Task<IActionResult> NewSiteLeader([FromBody] STLMembership leader)
        {
            var result = await _repo.SiteLeaderAsyn(leader);
            if (result.result == Results.Success)
                return Ok(new { result = result.result, message = result.message });
            else if (result.result == Results.Failed)
                return Ok(new { result = result.result, message = result.message });
            return NotFound();
        }
        [HttpPost]
        [Route("group/siteleader/edit")]
        public async Task<IActionResult> UpdateSiteLeader([FromBody] STLMembership leader)
        {
            var result = await _repo.SiteLeaderAsyn(leader, true);
            if (result.result == Results.Success)
                return Ok(new { result = result.result, message = result.message });
            else if (result.result == Results.Failed)
                return Ok(new { result = result.result, message = result.message });
            return NotFound();
        }

        [HttpPost]
        [Route("group/siteleader/member/new")]
        public async Task<IActionResult> NewMember([FromBody] STLMembership leader)
        {
            var result = await _repo.MemberAsysnc(leader);
            if (result.result == Results.Success)
                return Ok(new { result = result.result, message = result.message });
            else if (result.result == Results.Failed)
                return Ok(new { result = result.result, message = result.message });
            return NotFound();
        }
        [HttpPost]
        [Route("group/siteleader/member/edit")]
        public async Task<IActionResult> UpdateMember([FromBody] STLMembership leader)
        {
            var result = await _repo.MemberAsysnc(leader, true);
            if (result.result == Results.Success)
                return Ok(new { result = result.result, message = result.message });
            else if (result.result == Results.Failed)
                return Ok(new { result = result.result, message = result.message });
            return NotFound();
        }

        [HttpPost]
        [Route("group/siteleader/member/transfer")]
        public async Task<IActionResult> TransferMember([FromBody] TransferMember request)
        {
            var result = await _repo.TransferMember(request);
            if (result.result == Results.Success)
                return Ok(new { result = result.result, message = result.message });
            else if (result.result == Results.Failed)
                return Ok(new { result = result.result, message = result.message });
            return NotFound();
        }
        [HttpPost]
        [Route("group/siteleader/member/transferall")]
        public async Task<IActionResult> TransferAllMember([FromBody] TransferMember request)
        {
            var result = await _repo.TransferAllMember(request);
            if (result.result == Results.Success)
                return Ok(new { result = result.result, message = result.message });
            else if (result.result == Results.Failed)
                return Ok(new { result = result.result, message = result.message });
            return NotFound();
        }

        [HttpPost]
        [Route("group/resetpassword")]
        public async Task<IActionResult> Task0c([FromBody] ResetGroupPassword request)
        {
            var result = await _repo.ResetpasswordAsyn(request);
            if (result.result == Results.Success)
                return Ok(new { result = result.result, message = result.message });
            if (result.result == Results.Failed)
                return Ok(new { result = result.result, message = result.message });
            return NotFound();
        }

        [HttpPost]
        [Route("dashboard/resetpassword")]
        public async Task<IActionResult> Task0c1([FromBody] ResetGroupPassword request)
        {
            var result = await _repo.DashboardResetpasswordAsyn(request);
            if (result.result == Results.Success)
                return Ok(new { result = result.result, message = result.message });
            if (result.result == Results.Failed)
                return Ok(new { result = result.result, message = result.message });
            return NotFound();
        }
        [HttpPost]
        [Route("group/member/resetpassword")]
        public async Task<IActionResult> Task0cA([FromBody] ResetPassword request)
        {
            var result = await _repo.RequestChangePassword(request);
            if (result.result == Results.Success)
                return Ok(new { result = result.result, message = result.message });
            if (result.result == Results.Failed)
                return Ok(new { result = result.result, message = result.message });
            return NotFound();
        }
    }
}
