﻿using Infrastructure.Repositories;
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

namespace webapi.App.Aggregates.STLPartylistDashboard.Features
{
    [Service.ITransient(typeof(BlotterRepository))]
    public interface IBlottersRepository
    {
        Task<(Results result, string message)> SaveBlotter(Blotter info);
        Task<(Results result, string message)> UpdateBlotter(Blotter info);
        Task<(Results result, object blotter)> LoadBlotter(string plid, string pgrpid);
        Task<(Results result, string message)> SaveSummon(Blotter info);
        Task<(Results result, string message)> UpdateSummon(Blotter info);
        Task<(Results result, string message)> ResolveSummon(Blotter info);
        Task<(Results result, string message)> RemoveSummon(Blotter info);
        Task<(Results result, object summon)> LoadSummon(string plid, string pgrpid);
        Task<(Results result, object caseNo)> UpdatedCaseNo(string plid, string pgrpid);
        Task<(Results result, object brgycpt)> GetBrgyCpt(string plid, string pgrpid);
        Task<(Results result, object docpath)> Reprint(string plid, string pgrpid, string brgycsno, string colname);
        Task<(Results result, object signatures)> GetSignature(string plid, string pgrpid);
        Task<(Results result, object header)> GetHeader(string plid, string pgrpid);
    }

    public class BlotterRepository : IBlottersRepository
    {
        private readonly ISubscriber _identity;
        public readonly IRepository _repo;
        public STLAccount account { get { return _identity.AccountIdentity(); } }

        public BlotterRepository(ISubscriber identity, IRepository repo)
        {
            _identity = identity;
            _repo = repo;
        }

        public async Task<(Results result, string message)> SaveBlotter(Blotter info)
        {
            var result = _repo.DSpQuery<dynamic>($"dbo.spfn_BRGYBLOTTERIO", new Dictionary<string, object>()
            {
                {"paramplid",account.PL_ID},
                {"parampgrpid",account.PGRP_ID},
                {"parambrgycsno",info.BarangayCaseNo},
                {"paramrgncd",info.RegionCode},
                {"paramprvcd",info.ProvinceCode},
                {"parammncpl",info.MunicipalCode},
                {"parambrgycd",info.BrgyCode},
                {"paramprk",info.PurokOrSitio},
                {"parambrgycpt",info.BarangayCaptain},
                {"paramcmpnm",info.ComplainantsName},
                {"paramrspnm",info.RespondentsName},
                {"paramacstn",info.Accusations},
                {"paramincp",info.PlaceOfIncident},
                {"paramstmt",info.NarrativeOfIncident},
                {"paramincdt",info.DateTimeOfIncident},
                {"paramcrtdby",info.BarangaySecretary},
                {"paramdocpath",info.ReportPath}
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

        public async Task<(Results result, string message)> UpdateBlotter(Blotter info)
        {
            var result = _repo.DSpQuery<dynamic>($"dbo.spfn_BRGYBLOTTERUO", new Dictionary<string, object>()
            {
                {"paramplid",account.PL_ID},
                {"parampgrpid",account.PGRP_ID},
                {"parambrgycsno",info.BarangayCaseNo},
                {"paramprk",info.PurokOrSitio},
                {"paramcmpnm",info.ComplainantsName},
                {"paramrspnm",info.RespondentsName},
                {"paramacstn",info.Accusations},
                {"paramincp",info.PlaceOfIncident},
                {"paramstmt",info.NarrativeOfIncident},
                {"paramincdt",info.DateTimeOfIncident},
                {"paramcrtdby",info.BarangaySecretary},
                {"parammodby", info.ModifiedBy },
                {"paramdocpath",info.ReportPath}
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

        public async Task<(Results result, object blotter)> LoadBlotter(string plid, string pgrpid)
        {
            try
            {
                var result = _repo.DSpQuery<dynamic>($"dbo.spfn_BRGYBLOTTERIO", new Dictionary<string, object>()
            {
                {"paramplid",plid },
                {"parampgrpid",pgrpid }
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

        public async Task<(Results result, object summon)> LoadSummon(string plid, string pgrpid)
        {
            try
            {
                var result = _repo.DSpQuery<dynamic>($"dbo.spfn_BRGYBLOTTERUO", new Dictionary<string, object>()
            {
                {"paramplid",plid },
                {"parampgrpid",pgrpid },
                {"paramissmn",0 }
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

        public async Task<(Results result, string message)> SaveSummon(Blotter info)
        {
            var result = _repo.DSpQuery<dynamic>($"dbo.spfn_BRGYBLOTTERUO", new Dictionary<string, object>()
            {
                {"paramplid",info.PL_ID},
                {"parampgrpid",info.PGRP_ID },
                {"parambrgycsno",info.BarangayCaseNo },
                {"paramcrtdby",info.BarangaySecretary },
                {"paramissmn", 1 },
                {"paramsmndt", info.SummonDate},
                {"paramdocpath",info.ReportPath}
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

        public async Task<(Results result, string message)> UpdateSummon(Blotter info)
        {
            var result = _repo.DSpQuery<dynamic>($"dbo.spfn_BRGYBLOTTERUO", new Dictionary<string, object>()
            {
                {"paramplid",info.PL_ID},
                {"parampgrpid",info.PGRP_ID },
                {"parambrgycsno",info.BarangayCaseNo },
                {"paramissmn", 1},
                {"paramsmndt", info.SummonDate },
                {"parammodby", info.ModifiedBy },
                {"paramdocpath",info.ReportPath}
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

        public async Task<(Results result, string message)> ResolveSummon(Blotter info)
        {
            var result = _repo.DSpQuery<dynamic>($"dbo.spfn_BRGYBLOTTERUO", new Dictionary<string, object>()
            {
                {"paramplid",info.PL_ID},
                {"parampgrpid",info.PGRP_ID },
                {"parambrgycsno",info.BarangayCaseNo },
                {"paramissmn", 2 },
                {"parammodby", info.ModifiedBy }
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

        public async Task<(Results result, string message)> RemoveSummon(Blotter info)
        {
            var result = _repo.DSpQuery<dynamic>($"dbo.spfn_BRGYBLOTTERUO", new Dictionary<string, object>()
            {
                {"paramplid",info.PL_ID},
                {"parampgrpid",info.PGRP_ID },
                {"parambrgycsno",info.BarangayCaseNo },
                {"paramissmn", 0 },
                {"parammodby", info.ModifiedBy }
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

        public async Task<(Results result, object brgycpt)> GetBrgyCpt(string plid, string pgrpid)
        {
            var result = _repo.DQuery<dynamic>("with bea as (select PL_ID, PGRP_ID,USR_ID,ACT_ID,LDR_TYP from dbo.STLBEA) " +
            "select TOP(1) a.*, b.FLL_NM, b.S_ACTV from bea a left join dbo.STLBDB b on b.PL_ID = a.PL_ID and b.PGRP_ID = a.PGRP_ID and b.USR_ID = a.USR_ID and b.ACT_ID = a.ACT_ID " +
            $"where a.PL_ID='{plid}' and a.PGRP_ID='{pgrpid}' and a.LDR_TYP = '1' and b.S_ACTV = '1';");
            if (result != null)
                return (Results.Success, result);
            return (Results.Null, null);
        }

        public async Task<(Results result, object caseNo)> UpdatedCaseNo(string plid, string pgrpid)
        {
            var result = _repo.DQuery<dynamic>($"Select TOP(1) MAX(BRGY_CASE_NO) as CASENO from dbo.BIMSBLTR where PL_ID='{plid}' and PGRP_ID='{pgrpid}'");
            if (result != null)
                return (Results.Success, result);
            return (Results.Null, null);
        }

        public async Task<(Results result, object docpath)> Reprint(string plid, string pgrpid, string brgycsno, string colname)
        {
            var result = _repo.DQuery<dynamic>($"Select TOP(1) {colname} as DOCPATH from dbo.BIMSBLTR where PL_ID='{plid}' and PGRP_ID='{pgrpid}' and BRGY_CASE_NO='{brgycsno}'");
            if (result != null)
                return (Results.Success, result);
            return (Results.Null, null);
        }

        public async Task<(Results result, object signatures)> GetSignature(string plid, string pgrpid)
        {
            try
            {
                var result = _repo.DSpQuery<dynamic>($"dbo.spfn_BRGY_SIGNATORY0A", new Dictionary<string, object>()
            {
                {"parmplid",plid },
                {"parmpgrpid",pgrpid }
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

        public async Task<(Results result, object header)> GetHeader(string plid, string pgrpid)
        {
            var result = _repo.DQuery<dynamic>($"Select TOP(1) * from dbo.OFFICIAL_HEADER where PL_ID='{plid}' and PGRP_ID='{pgrpid}'");
            if (result != null)
                return (Results.Success, result);
            return (Results.Null, null);
        }
    }
}