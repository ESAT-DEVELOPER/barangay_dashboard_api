
using Infrastructure.Repositories;
using System.IO;
using System.Threading.Tasks;
using webapi.App.Aggregates.Common;
using webapi.App.Aggregates.SubscriberAppAggregate.Common;
using webapi.App.Model.User;
using webapi.App.STLDashboardModel;
using webapi.Commons.AutoRegister;
using Dapper;
using System.Linq;
using System.Data;
using System.Collections.Generic;
using System;
using Comm.Commons.Extensions;
using Newtonsoft.Json;
using Dapper;
using System.Linq;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace webapi.App.Aggregates.STLPartylistDashboard.Features
{
    [Service.ITransient(typeof(BiometricsRepository))]
    public interface IBiometricsRepository
    {
        Task<(Results status, string message)> biometricsRegistration(Biometrics info);
        Task<(Results result, object fingerprints)> searchFingerprint(string printPosition);
    }

    public class BiometricsRepository : IBiometricsRepository
    {
        private readonly ISubscriber _identity;
        public readonly IRepository _repo;
        public STLAccount account { get { return _identity.AccountIdentity(); } }

        public BiometricsRepository(ISubscriber identity, IRepository repo, IConfiguration config)
        {
            _identity = identity;
            _repo = repo;
        }

        public async Task<(Results status, string message)> biometricsRegistration(Biometrics info)
        {
            var result = _repo.DSpQuery<dynamic>($"dbo.spfn_ADDFINGERREG", new Dictionary<string, object>()
            {
                {"parmplid",account.PL_ID},
                {"parmpgrpid",account.PGRP_ID},
                {"parmusrid",info.RegisteredID},
                {"parmrptfinger",info.Fingerprints[0]},
                {"parmrmdfinger",info.Fingerprints[1]},
                {"parmrrgfinger",info.Fingerprints[2]},
                {"parmrpkfinger",info.Fingerprints[3]},
                {"parmlpkfinger",info.Fingerprints[4]},
                {"parmlrgfinger",info.Fingerprints[5]},
                {"parmlmdfinger",info.Fingerprints[6]},
                {"parmlptfinger",info.Fingerprints[7]},
                {"parmlthumb",info.Fingerprints[8]},
                {"parmrthumb",info.Fingerprints[9]}
            }).FirstOrDefault();

            if (result != null)
            {
                var row = ((IDictionary<string, object>)result);
                string ResultCode = row["RESULT"].Str();
                if (ResultCode == "1")
                    return (Results.Success, "Successfully save");
                else if (ResultCode == "0")
                    return (Results.Failed, "Already Exist");
                else if (ResultCode == "2")
                    return (Results.Null, null);
            }
            return (Results.Null, null);

        }

        public async Task<(Results result, object fingerprints)> searchFingerprint(string printPosition)
        {
            try
            {
                var result = _repo.DSpQuery<dynamic>($"dbo.spfn_READFINGERPRINTS", new Dictionary<string, object>()
                {
                    {"parmplid",account.PL_ID },
                    {"parmpgrpid",account.PGRP_ID },
                    {"parmprntpstn", printPosition }
                });
                if (result != null)
                    return (Results.Success, result);
                return (Results.Null, null);
            }
            catch (System.Exception)
            {
                return (Results.Null, null);
            }
        }

    };
}
