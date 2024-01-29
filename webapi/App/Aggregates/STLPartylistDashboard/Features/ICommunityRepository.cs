using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using webapi.Commons.AutoRegister;
using webapi.App.STLDashboardModel;
using webapi.App.Aggregates.Common;
using webapi.App.Aggregates.SubscriberAppAggregate.Common;
using Infrastructure.Repositories;
using webapi.App.Model.User;
using Comm.Commons.Extensions;
using webapi.App.RequestModel.Common;
using webapi.App.Aggregates.Common.Dto;

namespace webapi.App.Aggregates.STLPartylistDashboard.Features
{
    [Service.ITransient(typeof(CommunityRepository))]
    public interface ICommunityRepository
    {
        Task<(Results result, String message)> SendRequestCommunityAsync(cCommunity req);
        Task<(Results result, String message)> SendUpdateRequestCommunityAsync(cCommunity req);
        Task<(Results result, object comm)> LoadCommunityListAsync(FilterRequest req);
        Task<(Results result, String message)> AcceptRequestCommunityAsync(AcptCommunity req);
    }
    public class CommunityRepository:ICommunityRepository
    {
        private readonly ISubscriber _identity;
        private readonly IRepository _repo;
        public STLAccount account { get { return _identity.AccountIdentity(); } }
        public CommunityRepository(ISubscriber identity, IRepository repo)
        {
            _identity = identity;
            _repo = repo;
        }

        public async Task<(Results result, string message)> SendRequestCommunityAsync(cCommunity req)
        {
            req.CommunityID = ((int)DateTime.Now.ToTimeMillisecond()).ToString("X");
            var results = _repo.DSpQueryMultiple($"dbo.spfn_BIMSRAC000A", new Dictionary<string, object>()
            {
                {"parmplid", account.PL_ID },
                {"parmpgrpid", account.PGRP_ID },
                {"parmcommid", req.CommunityID},
                {"parmcommname", req.CommunityName},
                {"parmcommdescription", req.CommunityDescription},
                {"parmtypelevel", req.TypeLevel},
                {"parmscopeleveldescription", req.ScopeLevelDescription},
                {"parmscopeleveldescriptionval", req.ScopeLevelDescriptionVal},
                {"parmuserid", account.USR_ID},
            }).ReadSingleOrDefault();
            if (results != null)
            {
                var row = ((IDictionary<string, object>)results);
                string ResultCode = row["RESULT"].Str();
                if (ResultCode == "1")
                    return (Results.Success, "Successfully Send your request.");
                else if (ResultCode == "2")
                    return (Results.Failed, "Check your request, Please try again!");
            }
            return (Results.Null, "Please check your Internet Connection.");
        }

        public async Task<(Results result, object comm)> LoadCommunityListAsync(FilterRequest req)
        {
            var result = _repo.DSpQueryMultiple($"dbo.spfn_BIMSRAC000B", new Dictionary<string, object>()
            {
                {"parmplid",account.PL_ID },
                {"parmpgrpid",account.PGRP_ID },
                {"parmrequeststatus",req.Status },
                {"parmrownum",req.num_row },
                {"parmtypelevel",req.TypeLevel },
                {"parmsearch",req.Search }
            });
            if (result != null)
                return (Results.Success, STLSubscriberDto.GetAllCommunityList(result.Read<dynamic>(), req.Userid, 100));
            return (Results.Null, null);
        }

        public async Task<(Results result, string message)> SendUpdateRequestCommunityAsync(cCommunity req)
        {
            var results = _repo.DSpQueryMultiple($"dbo.spfn_BIMSRAC000A1", new Dictionary<string, object>()
            {
                {"parmplid", account.PL_ID },
                {"parmpgrpid", account.PGRP_ID },
                {"parmcommid", req.CommunityID},
                {"parmcommname", req.CommunityName},
                {"parmcommdescription", req.CommunityDescription},
                {"parmtypelevel", req.TypeLevel},
                {"parmscopeleveldescription", req.ScopeLevelDescription},
                {"parmscopeleveldescriptionval", req.ScopeLevelDescriptionVal},
                {"parmuserid", account.USR_ID},
            }).ReadSingleOrDefault();
            if (results != null)
            {
                var row = ((IDictionary<string, object>)results);
                string ResultCode = row["RESULT"].Str();
                if (ResultCode == "1")
                    return (Results.Success, "Successfully update your request.");
                else if (ResultCode == "2")
                    return (Results.Failed, "Check your request, Please try again!");
            }
            return (Results.Null, "Please check your Internet Connection.");
        }

        public async Task<(Results result, string message)> AcceptRequestCommunityAsync(AcptCommunity req)
        {
            var results = _repo.DSpQueryMultiple($"dbo.spfn_BIMSRAC000A2", new Dictionary<string, object>()
            {
                {"parmplid", req.PL_ID },
                {"parmpgrpid", req.PGRP_ID },
                {"parmcommid", req.CommunityID},
                {"parmreqstatus", req.RequestStatus },
                {"parmuserid", account.USR_ID},
            }).ReadSingleOrDefault();
            if (results != null)
            {
                var row = ((IDictionary<string, object>)results);
                string ResultCode = row["RESULT"].Str();
                if (ResultCode == "1")
                    return (Results.Success, " successfully accepted!");
                else if (ResultCode == "2")
                    return (Results.Failed, "Check your data, Please try again!");
                else if (ResultCode == "3")
                    return (Results.Failed, " successfully decline!");
            }
            return (Results.Null, "Please check your Internet Connection.");
        }
    }
}
