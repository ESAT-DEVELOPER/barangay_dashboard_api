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
using webapi.App.Features.UserFeature;

namespace webapi.App.Aggregates.STLPartylistDashboard.Features
{
    [Service.ITransient(typeof(VirtualIDRepository))]
    public interface IVirtualIDRepository
    {
        Task<(Results result, string message)> generateVirtualID(VirtualID detail);
    }
    public class VirtualIDRepository : IVirtualIDRepository
    {
        private readonly ISubscriber _identity;
        public readonly IRepository _repo;
        public STLAccount account { get { return _identity.AccountIdentity(); } }
        public VirtualIDRepository(ISubscriber identity, IRepository repo)
        {
            _identity = identity;
            _repo = repo;
        }

        public async Task<(Results result, string message)> generateVirtualID(VirtualID detail)
        {
            var result = _repo.DSpQuery<dynamic>($"dbo.spfn_BIMSSIDFRNTBCK", new Dictionary<string, object>()
            {
                {"parmplid", account.PL_ID},
                {"parmpgrpid", account.PGRP_ID},
                {"parmusrid", detail.UserId},
                {"parmfrntid", detail.FrontIdImageUrl},
                {"parmbckid", detail.BackIdImageUrl}
            }).FirstOrDefault();
            if (result != null)
            {
                var row = ((IDictionary<string, object>)result);
                string ResultCode = row["RESULT"].Str();
                if (ResultCode == "1")
                {
                    await Generate_Virtual_ID(row, detail, account.PL_ID, account.PGRP_ID, detail.UserId);
                    return (Results.Success, "Successfully save");
                }
                else if (ResultCode == "0")
                {
                    return (Results.Failed, "Already Exist");
                }
                else if (ResultCode == "2")
                {
                    return (Results.Null, null);
                }
            }
            return (Results.Null, null);
        }
        public async Task<bool> Generate_Virtual_ID(IDictionary<string, object> row, object content, string plid, string pgrpid, string userid)
        {
            //var settings = STLSubscriberDto.GetGroup(row);
            var notifications = SubscriberDto.VirtualIDNofitication(row);
            await Pusher.PushAsync($"/{plid}/{pgrpid}/{userid}/virtualid"
                , new { type = "virtualid", content = content });
            return false;
        }
    }
}
