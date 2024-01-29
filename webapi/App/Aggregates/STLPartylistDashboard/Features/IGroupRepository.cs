using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using webapi.App.Aggregates.Common;
using Comm.Commons.Extensions;
using webapi.App.RequestModel.AppRecruiter;
using webapi.App.Aggregates.SubscriberAppAggregate.Common;
using Infrastructure.Repositories;
using webapi.App.Model.User;
using webapi.Commons.AutoRegister;
using webapi.App.Features.UserFeature;

namespace webapi.App.Aggregates.STLPartylistDashboard.Features
{
    [Service.ITransient(typeof(GroupRepository))]
    public interface IGroupRepository
    {
        Task<(Results result, String message)> GroupAsyn(STLMembership membership, bool isUpdate = false);
        Task<(Results result, String message)> SiteLeaderAsyn(STLMembership leader, bool isUpdate = false);
        Task<(Results result, object grp)> Group();
        Task<(Results result, object grp)> Subsrciber_List(string search, int row);
        Task<(Results result, object grp)> GroupLeader(int usrtype);
        Task<(Results result, object ldr)> SiteLeader(string plid, string pgrpid, string usrid, string num_row, string search);
        Task<(Results result, object ldr)> BarangayOfficial(string plid, string pgrpid, string usrid, string num_row, string search);
        Task<(Results result, object ldr)> SiteLeader1(string plid, string pgrpid, string usrid, string num_row, string search);
        Task<(Results result, String message)> MemberAsysnc(STLMembership mem, bool isUpdate = false);
        Task<(Results result, String message)> ResetpasswordAsyn(ResetGroupPassword request);
        Task<(Results result, String message)> DashboardResetpasswordAsyn(ResetGroupPassword request);
        Task<(Results result, String message)> TransferMember(TransferMember request);
        Task<(Results result, String message)> TransferAllMember(TransferMember request);
        Task<(Results result, object act)> AllAccount();
        Task<(Results result, object act)> LeaderAccount(string groupid);
        Task<(Results result, String message)> BlockAccount(BlockAccount acct);
        Task<(Results result, String message)> PromoteMembertoLeader(STLMembership request);
        Task<(Results result, String message)> DowngradeLeaderToMember(STLMembership request);
        Task<(Results result, String message)> ChangeAccountType(STLMembership request);
        Task<(Results result, object brgyprof)> BarangayProfile(BRGYProfileRequest request);
        Task<(Results result, String message, string brgyprofid)> TotalRegisterVoter(TotalRegisterVoter request);
        Task<(Results result, String message)> RequestChangePassword(ResetPassword request);
        Task<(Results result, object accnt)> LoadAccount();
    }
    public class GroupRepository : IGroupRepository
    {
        private readonly ISubscriber _identity;
        public readonly IRepository _repo;
        public STLAccount account { get { return _identity.AccountIdentity(); } }
        public GroupRepository (ISubscriber identity, IRepository repo)
        {
            _identity = identity;
            _repo = repo;
        }

        public async Task<(Results result, string message)> GroupAsyn(STLMembership membership, bool isUpdate = false)
        {
            var result = _repo.DSpQuery<dynamic>($"spfn_CBACBB02", new Dictionary<string, object>()
            {
                {"parmplid",membership.PLID },
                {"parmpgrpid",isUpdate?membership.PGRPID : "" },
                {"parmfnm",membership.Firstname },
                {"parmlnm",membership.Lastname },
                {"parmmnm",membership.Middlename },
                {"parmnnm",membership.Nickname },
                {"parmmobno",membership.MobileNumber },
                {"parmgender",membership.Gender },
                {"parmmstastus",membership.MaritalStatus },
                {"parmemladd",membership.EmailAddress },
                {"parmhmeadd",membership.HomeAddress },
                {"parmprsntadd",membership.PresentAddress },
                {"parmreg",membership.Region },
                {"parmprov",membership.Province },
                {"parmmun",membership.Municipality },
                {"parmbrgy",membership.Barangay },
                {"parmsitio",membership.Sitio },
                {"parmbdate",membership.BirthDate },
                {"parmctznshp",membership.Citizenship },
                {"parmbldType",membership.BloodType },
                {"parmntnlty",membership.Nationality },
                {"parmoccptn",membership.Occupation },
                {"parmsklls",membership.Skills },
                {"parmsubtype",membership.SubscriberType },

                {"parmusername",membership.Username },
                {"parmpassword",membership.Userpassword },
                {"parmusrid",(isUpdate?membership.Userid:"") },
                //{"parmpsncd",membership.psncd },
                {"parmoptrid",account.USR_ID }
            }).FirstOrDefault();
            if (result != null)
            {
                var row = ((IDictionary<string, object>)result);
                var ResultCode = row["RESULT"].Str();
                if (ResultCode == "1")
                    return (Results.Success, "Successfully saved!");
                else if (ResultCode == "3")
                    return (Results.Failed, "Mobile Number already exist!");
                else if (ResultCode == "4")
                    return (Results.Failed, "Group already exist!");
            }
            return (Results.Null, null);
        }

        public async Task<(Results result, object grp)> Group()
        {
            var result = _repo.DQuery<dynamic>($"dbo.spfn_CBABDB01");
            if (result != null)
                return (Results.Success, result);
            return (Results.Null, null);
        }

        public async Task<(Results result, string message)> RequestChangePassword(ResetPassword request)
        {
            var result = _repo.DSpQuery<dynamic>($"spfn_BDABS0CSP0A", new Dictionary<string, object>()
            {
                {"parmplid",request.PL_ID },
                {"parmpgrpid",request.PGRP_ID },
                {"parmoptrid",account.USR_ID },
                {"parmmobno",request.MobileNumber },
                {"parmusrid",request.USR_ID },
                {"parmactid",request.ACT_ID }
            }).FirstOrDefault();
            if (result != null)
            {
                var row = ((IDictionary<string, object>)result);
                var ResultCode = row["RESULT"].Str();
                if (ResultCode == "1")
                    return (Results.Success, "Successfully saved!");
                else if (ResultCode == "2")
                    return (Results.Failed, "Invalid Mobile Number, Please try again!");
                else if (ResultCode == "3")
                    return (Results.Failed, "Mobile Number was not exist, Please try again!");
                else if (ResultCode == "0")
                    return (Results.Failed, "You requested to change password for invalid account, Please try again!");
                return (Results.Failed, "Something wrong in your data, Please try again!");
            }
            return (Results.Null, null);
        }
        public async Task<(Results result, string message)> ResetpasswordAsyn(ResetGroupPassword request)
        {
            var result = _repo.DSpQuery<dynamic>($"spfn_BDABO0COP", new Dictionary<string, object>()
            {
                {"parmplid",request.PL_ID },
                {"parmpgrpid",request.PGRP_ID },
                {"parmoptrid",account.USR_ID },
                //{"parmcompagnt",request.isAgent },
                {"@parmtrgtid",request.USR_ID },
                //{"@parmusrpsswrd",request.NewPassword }
            }).FirstOrDefault();
            if (result != null)
            {
                var row = ((IDictionary<string, object>)result);
                var ResultCode = row["RESULT"].Str();
                if (ResultCode == "1")
                    return (Results.Success, "Successfully saved!");
                return (Results.Failed, "Something wrong in your data, Please try again!");
            }
            return (Results.Null, null);
        }

        public async Task<(Results result, string message)> DashboardResetpasswordAsyn(ResetGroupPassword request)
        {
            var result = _repo.DSpQuery<dynamic>($"spfn_BUDBO0COP", new Dictionary<string, object>()
            {
                {"parmplid",request.PL_ID },
                {"parmpgrpid",request.PGRP_ID },
                {"parmoptrid",account.USR_ID },
                //{"parmcompagnt",request.isAgent },
                {"@parmtrgtid",request.USR_ID },
                //{"@parmusrpsswrd",request.NewPassword }
            }).FirstOrDefault();
            if (result != null)
            {
                var row = ((IDictionary<string, object>)result);
                var ResultCode = row["RESULT"].Str();
                if (ResultCode == "1")
                    return (Results.Success, "Successfully saved!");
                return (Results.Failed, "Something wrong in your data, Please try again!");
            }
            return (Results.Null, null);
        }

        public async Task<(Results result, object grp)> GroupLeader(int usrtype)
        {
            var result = _repo.DSpQuery<dynamic>($"dbo.spfn_BEABDBLLL0A", new Dictionary<string, object>()
            {
                {"parmplid",account.PL_ID },
                {"parmpgrpid",account.PGRP_ID },
                {"parmusrtyp", usrtype }
            });
            if (result != null)
                return (Results.Success, result);
            return (Results.Null, null);
        }

        public async Task<(Results result, object brgyprof)> BarangayProfile(BRGYProfileRequest request)
        {
            var result = _repo.DSpQuery<dynamic>($"dbo.spfn_BDBLLB0A", new Dictionary<string, object>()
            {
                {"parmplid",request.plid },
                {"parmpgrpid",request.pgrpid },
                {"parmrownum",request.num_row },
                {"parmreg",request.reg },
                {"parmprov",request.prov },
                {"parmmun",request.mun },
                {"parmbrgy",request.brgy }
            });
            if (result != null)
                return (Results.Success, result);
            return (Results.Null, null);
        }
        public async Task<(Results result, object ldr)> BarangayOfficial(string plid, string pgrpid, string usrid, string num_row, string search)
        {
            var result = _repo.DSpQuery<dynamic>($"dbo.spfn_BEABDBLLL0C", new Dictionary<string, object>()
            {
                {"parmplid",plid },
                {"parmpgrpid",pgrpid },
                {"parmusrid", usrid },
                {"parmrownum", num_row },
                {"parmsrch", search }
            });
            if (result != null)
                return (Results.Success, SubscriberDto.Get_GroupLeaderAccount(result));
            return (Results.Null, null);
        }
        public async Task<(Results result, object ldr)> SiteLeader(string plid, string pgrpid, string usrid, string num_row, string search)
        {
            var result = _repo.DSpQuery<dynamic>($"dbo.spfn_BEABDBLLL0B", new Dictionary<string, object>()
            {
                {"parmplid",plid },
                {"parmpgrpid",pgrpid },
                {"parmusrid", usrid },
                {"parmrownum", num_row },
                {"parmsrch", search }
            });
            if (result != null)
                return (Results.Success, SubscriberDto.Get_GroupLeaderAccount(result));
            return (Results.Null, null);
        }

        public async Task<(Results result, object ldr)> SiteLeader1(string plid, string pgrpid, string usrid, string num_row, string search)
        {
            var result = _repo.DSpQuery<dynamic>($"dbo.spfn_BEABDBLLL0B3", new Dictionary<string, object>()
            {
                {"parmplid",plid },
                {"parmpgrpid",pgrpid },
                {"parmusrid", usrid },
                {"parmrownum",num_row },
                {"parmsrch",search }
            });
            if (result != null)
                return (Results.Success, SubscriberDto.Get_GroupLeaderAccount(result));
            return (Results.Null, null);
        }

        public async Task<(Results result, object act)> AllAccount()
        {
            var result = _repo.DSpQuery<dynamic>($"dbo.spfn_BEABDBBDA0F", new Dictionary<string, object>()
            {
                {"parmplid",account.PL_ID },
                {"parmpgrpid",account.PGRP_ID }
            });
            if (result != null)
                return (Results.Success, result);
            return (Results.Null, null);
        }

        public async Task<(Results result, object act)> LeaderAccount(string groupid)
        {
            var result = _repo.DSpQuery<dynamic>($"dbo.spfn_BDABDBCAACBB07", new Dictionary<string, object>()
            {
                {"parmpgrpid", groupid }
            });
            if (result != null)
                return (Results.Success, SubscriberDto.GetLeaderAccount(result));
            return (Results.Null, null);
        }

        public async Task<(Results result, string message)> SiteLeaderAsyn(STLMembership leader, bool isUpdate = false)
        {
            var results = _repo.DSpQueryMultiple("dbo.spfn_BDABDBCAACBB03", new Dictionary<string, object>()
            {
                //11
                {"parmplid",leader.PLID },
                {"parmpgrpid",leader.PGRPID },
                {"parmldrtype",leader.Leadertype },
                {"parmfnm",leader.Firstname },
                {"parmlnm",leader.Lastname },
                {"parmmnm",leader.Middlename },
                {"parmnnm",leader.Nickname },
                {"parmmobno",leader.MobileNumber },
                {"parmgender",leader.Gender },
                {"parmmstastus",leader.MaritalStatus },
                {"parmemladd",leader.EmailAddress },

                //2
                {"parmprecentno", leader.PrecentNumber},
                {"parmclusterno", leader.ClusterNumber },

                //2
                {"parmhmeadd",leader.HomeAddress },
                {"parmprsntadd",leader.PresentAddress },

                //6
                {"parmreg",leader.Region },
                {"parmprov",leader.Province },
                {"parmmun",leader.Municipality },
                {"parmbrgy",leader.Barangay },
                {"@parmsitio",leader.Sitio },
                {"@parmlocsite",leader.LocationSite },

                //6
                {"parmbdate",leader.BirthDate },
                {"parmctznshp",leader.Citizenship },
                {"parmbldType",leader.BloodType },
                {"parmntnlty",leader.Nationality },
                {"parmoccptn",leader.Occupation },
                {"parmsklls",leader.Skills },

                //2
                {"parmprfpic",leader.ImageUrl },
                {"parmImgUrl",leader.ImageUrl },
                {"parmsignature",leader.SignatureURL },
                {"parmwithdisability",leader.WDisability },
                {"parmtypedisability",leader.TypeDisability },
                {"parmregistervoter",leader.RegisterVoter },


                //4
                {"parmusertype",leader.AccountType },
                {"parmusername",leader.Username },
                //{"parmpassword",leader.Userpassword },
                {"@parmrefID",leader.RefGroupID },
                {"parmusrid",(isUpdate?leader.Userid:"") },
                {"parmoptrid",account.USR_ID },
            }).ReadSingleOrDefault();
            if (results != null)
            {
                var row = ((IDictionary<string, object>)results);
                string ResultCode = row["RESULT"].Str();
                if (ResultCode == "1")
                    return (Results.Success, "Succesfull save!");
                else if (ResultCode == "2")
                    return (Results.Failed, "Invalid Mobile Number!");
                else if (ResultCode == "3")
                    return (Results.Failed, "Username already exist!");
                else if (ResultCode == "4")
                    return (Results.Failed, "You are already Member of this Group!");
                else if (ResultCode == "5")
                    return (Results.Failed, "Username already exist!");
            }
            return (Results.Null, null);
        }

        public async Task<(Results result, String message)> MemberAsysnc(STLMembership mem, bool isUpdate = false)
        {
            var results = _repo.DSpQueryMultiple("dbo.spfn_BDABDBCAACBB04", new Dictionary<string, object>()
            {
                {"parmplid",mem.PLID },
                {"parmpgrpid",mem.PGRPID },
                {"parmldrtype",mem.Leadertype },
                {"parmfnm",mem.Firstname },
                {"parmlnm",mem.Lastname },
                {"parmmnm",mem.Middlename },
                {"parmnnm",mem.Nickname },
                {"parmmobno",mem.MobileNumber },
                {"parmgender",mem.Gender },
                {"parmmstastus",mem.MaritalStatus },
                {"parmemladd",mem.EmailAddress },

                {"parmprecentno", mem.PrecentNumber},
                {"parmclusterno", mem.ClusterNumber },

                {"parmhmeadd",mem.HomeAddress },
                {"parmprsntadd",mem.PresentAddress },

                {"parmreg",mem.Region },
                {"parmprov",mem.Province },
                {"parmmun",mem.Municipality },
                {"parmbrgy",mem.Barangay },
                {"@parmsitio",mem.Sitio },

                {"parmbdate",mem.BirthDate },
                {"parmctznshp",mem.Citizenship },
                {"parmbldType",mem.BloodType },
                {"parmntnlty",mem.Nationality },
                {"parmoccptn",mem.Occupation },
                {"parmsklls",mem.Skills },

                {"parmprfpic",mem.ImageUrl },
                {"parmImgUrl",mem.ImageUrl },
                {"parmsignature",mem.SignatureURL },
                {"parmwithdisability",mem.WDisability },
                {"parmtypedisability",mem.TypeDisability },
                {"parmregistervoter",mem.RegisterVoter },

                {"parmusertype",mem.AccountType },
                {"parmusername",mem.Username },
                //{"parmpassword",mem.Userpassword },
                {"parmrefID",mem.RefGroupID },
                //{"parmrefldrID",mem.RefLDRID },
                {"parmusrid",(isUpdate?mem.Userid:"") },
                {"parmoptrid",account.USR_ID },
            }).ReadSingleOrDefault();
            if (results != null)
            {
                var row = ((IDictionary<string, object>)results);
                string ResultCode = row["RESULT"].Str();
                if (ResultCode == "1")
                    return (Results.Success, "Succesfull saved!");
                else if (ResultCode == "2")
                    return (Results.Failed, "Invalid Mobile Number!");
                else if (ResultCode == "3")
                    return (Results.Failed, "Username already exist!");
                else if (ResultCode == "4")
                    return (Results.Failed, "You are already Member of this Group!");
                else if (ResultCode == "5")
                    return (Results.Failed, "Username already exist!");
            }
            return (Results.Null, null);
        }

        public async Task<(Results result, string message)> TransferMember(TransferMember request)
        {
            var result = _repo.DSpQueryMultiple($"dbo.spfn_BEABS0P1TS", new Dictionary<string, object>()
            {
                {"parmplid",request.PL_ID },
                {"parmpgrpid",request.PGRP_ID },
                {"parmoptrid", account.USR_ID },
                {"parmactid",request.ACT_ID },
                {"parmrefgrpid",request.Ref_Group_ID },
                {"parmrefldrid",request.Ref_LDR_ID }
            }).ReadSingleOrDefault();
            if (result != null)
            {
                var row = ((IDictionary<string, object>)result);
                string ResultCode = row["RESULT"].Str();
                if (ResultCode == "1")
                    return (Results.Success, "Succesfull saved!");
                else if (ResultCode == "2")
                    return (Results.Failed, "Invalid User type!");
                else if (ResultCode == "71")
                    return (Results.Failed, "Account was not exist!");
                else if (ResultCode == "4")
                    return (Results.Failed, "Please check your data, try again!");
                else if (ResultCode == "5")
                    return (Results.Failed, "Username already exist!");
            }
            return (Results.Null, null);
        }

        public async Task<(Results result, string message)> TransferAllMember(TransferMember request)
        {
            var result = _repo.DSpQueryMultiple($"dbo.spfn_BEABS0P1TAS", new Dictionary<string, object>()
            {
                {"parmplid",request.PL_ID },
                {"parmpgrpid",request.PGRP_ID },
                {"parmoptrid", account.USR_ID },
                {"parmactid",request.ACT_ID },
                {"parmrefgrpid",request.Ref_Group_ID },
                {"parmrefldrid",request.Ref_LDR_ID }
            }).ReadSingleOrDefault();
            if (result != null)
            {
                var row = ((IDictionary<string, object>)result);
                string ResultCode = row["RESULT"].Str();
                if (ResultCode == "1")
                    return (Results.Success, "Succesfull saved!");
                else if (ResultCode == "2")
                    return (Results.Failed, "Invalid User type!");
                else if (ResultCode == "71")
                    return (Results.Failed, "Account was not exist!");
                else if (ResultCode == "4")
                    return (Results.Failed, "Please check your data, try again!");
                else if (ResultCode == "3")
                    return (Results.Failed, "Username already exist!");
            }
            return (Results.Null, null);
        }

        public async Task<(Results result, string message)> PromoteMembertoLeader(STLMembership request)
        {
            var result = _repo.DSpQuery<dynamic>($"dbo.spfn_BEA0A", new Dictionary<string, object>()
            {
                {"parmplid",request.PLID },
                {"parmpgrpid",request.PGRPID },
                {"parmoptid", account.USR_ID },
                {"parmusrid", request.Userid },
                {"parmacctid",request.Acctid },
                {"parmldrtype",request.Leadertype },
                {"parmrefgrpid",request.RefGroupID },
                {"parmlocsite",request.LocationSite }
            }).FirstOrDefault();
            if (result != null)
            {
                var row = ((IDictionary<string, object>)result);
                string ResultCode = row["RESULT"].Str();
                if (ResultCode == "1")
                    return (Results.Success, "Succesfull saved!");
                else if (ResultCode == "0")
                    return (Results.Failed, "Somethings wrong in your data. Please try again!");
            }
            return (Results.Null, null);
        }
        public async Task<(Results result, string message)> BlockAccount(BlockAccount acct)
        {
            var result = _repo.DSpQuery<dynamic>($"dbo.spfn_BEABS0BS", new Dictionary<string, object>()
            {
                {"parmplid",acct.PL_ID },
                {"parmpgrpid",acct.PGRP_ID },
                {"parmoptrid", account.USR_ID },
                {"parmactid",acct.ACT_ID }
            }).FirstOrDefault();
            if (result != null)
            {
                var row = ((IDictionary<string, object>)result);
                string ResultCode = row["RESULT"].Str();
                if (ResultCode == "1")
                {
                    if (row["S_BLK"].Str().Equals("1"))
                    {
                        if (row["HS_USR"].Str().Equals("1"))
                        {
                            await Pusher.PushAsync($"/{acct.PL_ID}/{acct.PGRP_ID}/{row["SUBSCR_ID"].Str()}/downline"
                                , new { type = "device.session-end", message = "Your session has been expired! Your upline blocked by admin", });
                        }
                        await Pusher.PushAsync($"/{acct.PL_ID}/{acct.PGRP_ID}/{row["SUBSCR_ID"]}/notify"
                                , new { type = "device.session-end", message = "Your session has been expired! Your account has blocked by admin", });
                    }
                    return (Results.Success, "Successfully saved!");
                }
                return (Results.Failed, "Somethings wrong in your data. Please try again!");
            } 
            return (Results.Null, null);
        }
        public async Task<(Results result, string message)> ChangeAccountType(STLMembership request)
        {
            var result = _repo.DSpQuery<dynamic>($"dbo.spfn_BEA0B", new Dictionary<string, object>()
            {
                {"parmplid",request.PLID },
                {"parmpgrpid",request.PGRPID },
                {"parmoptid", account.USR_ID },
                {"parmuserid", request.Userid },
                {"parmaccttype",request.Leadertype },
            }).FirstOrDefault();
            if (result != null)
            {
                var row = ((IDictionary<string, object>)result);
                string ResultCode = row["RESULT"].Str();
                if (ResultCode == "1")
                    return (Results.Success, "Succesfull save");
                else if (ResultCode == "0")
                    return (Results.Failed, "Somethings wrong in your data. Please try again!");
            }
            return (Results.Null, null);
        }
        public async Task<(Results result, string message)> DowngradeLeaderToMember(STLMembership request)
        {
            var result = _repo.DSpQuery<dynamic>($"dbo.spfn_BEABS0S1TP", new Dictionary<string, object>()
            {
                {"parmplid",request.PLID },
                {"parmpgrpid",request.PGRPID },
                {"parmoptrid", account.USR_ID },
                {"parmuserid", request.Userid },
                {"parmactid",request.Acctid },
                {"parmrefgroupid",request.RefGroupID },
                {"parmrefldrid",request.RefLDRID }
            }).FirstOrDefault();
            if (result != null)
            {
                var row = ((IDictionary<string, object>)result);
                string ResultCode = row["RESULT"].Str();
                if (ResultCode == "1")
                    return (Results.Success, "Succesfull save!");
                else if (ResultCode == "0")
                    return (Results.Failed, "Somethings wrong in your data. Please try again!");
            }
            return (Results.Null, null);
        }

        public async Task<(Results result, string message, string brgyprofid)> TotalRegisterVoter(TotalRegisterVoter request)
        {
            var result = _repo.DSpQuery<dynamic>($"dbo.spfn_LLB0A", new Dictionary<string, object>()
            {
                {"parmplid",request.PLID },
                {"parmpgrpid",request.PGRPID },
                {"parmregion", request.Reg },
                {"parmprov",request.Prov },
                {"parmmun",request.Mun },
                {"parmbrgy",request.Brgy },
                {"parmbrgypid",request.BrgyProfID },
                {"parmbrgynumbervoter",request.TotalVoter },
                {"parmoptrid", account.USR_ID }
            }).FirstOrDefault();
            if (result != null)
            {
                var row = ((IDictionary<string, object>)result);
                string ResultCode = row["RESULT"].Str();
                if (ResultCode == "1")
                    return (Results.Success, "Succesfull saved!",row["BRGYP_ID"].Str());
                else if (ResultCode == "0")
                    return (Results.Failed, "Somethings wrong in your data. Please try again!",null);
            }
            return (Results.Null, null,null);
        }

        public async Task<(Results result, object accnt)> LoadAccount()
        {
            var results = _repo.DSpQuery<dynamic>($"dbo.spfn_BDABDB05", new Dictionary<string, object>()
            {
                {"parmplid", account.PL_ID},
                {"parmpgrpid",account.PGRP_ID }
            });
            if (results != null)
                return (Results.Success, SubscriberDto.GetMemberAccountList(results));
            return (Results.Null, null);
        }

        public async Task<(Results result, object grp)> Subsrciber_List(string search, int row)
        {
            var results = _repo.DSpQuery<dynamic>($"dbo.spfn_CBABDB03", new Dictionary<string, object>()
            {
                {"parmsrch", search},
                {"parmrownum",row }
            });
            if (results != null)
                return (Results.Success, SubscriberDto.GetSubscriberList(results));
            return (Results.Null, null);
        }
    }
}
