using Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using webapi.App.Aggregates.Common;
using Comm.Commons.Extensions;
using webapi.App.Aggregates.SubscriberAppAggregate.Common;
using webapi.App.Model.User;
using webapi.Commons.AutoRegister;
using Microsoft.AspNetCore.Mvc;
using webapi.App.STLDashboardModel;
using System.Runtime.CompilerServices;

namespace webapi.App.Aggregates.STLPartylistDashboard.Features
{
    [Service.ITransient(typeof(MobileVerificationRepository))]
    public interface IMobileVerificationRepository
    {
        Task<(Results result, string message)> approveMobileVerification(string userid, int approve);
        Task<(Results result, object articles)> getMobileVerficationList(string userid, string date);
    }
    public class MobileVerificationRepository : IMobileVerificationRepository
    {
        private readonly ISubscriber _identity;
        public readonly IRepository _repo;
        public STLAccount account { get { return _identity.AccountIdentity(); } }
        public MobileVerificationRepository(ISubscriber identity, IRepository repo)
        {
            _identity = identity;
            _repo = repo;
        }

        public async Task<(Results result, string message)> approveMobileVerification(string userid,int approve)
        {
            var result = _repo.DSpQuery<dynamic>($"dbo.spfn_UPDVRFYACNT", new Dictionary<string, object>()
            {
                {"parmusrid", userid},
                {"parmapprove", approve}
            }).FirstOrDefault();
            if (result != null)
            {
                var row = ((IDictionary<string, object>)result);
                string ResultCode = row["RESULT"].Str();
                if (ResultCode == "1")
                {
                    return (Results.Success, "Successfully posted");
                }
                else if (ResultCode == "0")
                {
                    return (Results.Failed, "Account does'nt exist!");
                }
                else if (ResultCode == "2")
                {
                    return (Results.Null, "System Error");
                }
            }
            return (Results.Null, "System Error");
        }



        public async Task<(Results result, object articles)> getMobileVerficationList(string userid, string date)
        {
            var result = _repo.DSpQuery<dynamic>($"dbo.spfn_GETVRFYACNT", new Dictionary<string, object>()
                {
                    {"parmplid", account.PL_ID },
                    {"parmpgrpid", account.PGRP_ID },
                    {"parmusrid", userid },
                    {"parmdate", date }
                });
            if (result != null)
                return (Results.Success, result);
            return (Results.Null, null);
        }
    }
}
