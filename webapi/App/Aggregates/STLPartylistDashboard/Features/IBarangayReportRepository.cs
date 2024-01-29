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
using webapi.App.STLDashboardModel;
using webapi.App.Features.UserFeature;


namespace webapi.App.Aggregates.STLPartylistDashboard.Features
{
    [Service.ITransient(typeof(BarangayReportRepository))]
    public interface IBarangayReportRepository
    {

        Task<(Results result, object household)> LoadReportperSitio(Report report);
        Task<(Results result, object household)> DataAnalytics(Report report);

        Task<(Results result, object household)> GetStatisticalData(StatisticalData data);
        Task<(Results result, object household)> GetStatisticalData02(StatisticalData data);

        Task<(Results result, object statistics)> GetStatistics0100(Sitioreport data);//---//
        Task<(Results result, object statistics)> GetStatistics0200(int type, string code);

        Task<(Results result, object statistics)> GetStatistics01A01(Sitioreport data);//---/
        Task<(Results result, object statistics)> GetStatistics01A02(Sitioreport data);//---/
        Task<(Results result, object statistics)> GetStatistics01A03(Sitioreport data);//---/
        Task<(Results result, object statistics)> GetStatistics01A04(Sitioreport data);//---/
        Task<(Results result, object statistics)> GetStatistics01A05(Sitioreport data);//---/
        Task<(Results result, object statistics)> GetStatistics01A06(Sitioreport data);//---/

        Task<(Results result, object statistics)> GetStatistics02A01(int type1, int type2, string code);
        Task<(Results result, object statistics)> GetStatistics02A02(int type1, int type2, string code);
        Task<(Results result, object statistics)> GetStatistics02A03(int type1, int type2, string code);
        Task<(Results result, object statistics)> GetStatistics02A04(int type1, int type2, string code);
        Task<(Results result, object statistics)> GetStatistics02A05(int type1, int type2, string code);
        Task<(Results result, object statistics)> GetStatistics02A06(int type1, int type2, string code);


        Task<(Results result, object statistics)> GetStatistics01B01(Sitioreport data);//---/
        Task<(Results result, object statistics)> GetStatistics01B02(Sitioreport data);//---/
        Task<(Results result, object statistics)> GetStatistics01B03(Sitioreport data);//---/
        Task<(Results result, object statistics)> GetStatistics01B04(Sitioreport data);//---/

        Task<(Results result, object statistics)> GetStatistics02B01(int type1, int type2, string code);
        Task<(Results result, object statistics)> GetStatistics02B02(int type1, int type2, string code);
        Task<(Results result, object statistics)> GetStatistics02B03(int type1, int type2, string code);
        Task<(Results result, object statistics)> GetStatistics02B04(int type1, int type2, string code);


        Task<(Results result, object statistics)> GetStatistics01C01(Sitioreport data);//---/
        Task<(Results result, object statistics)> GetStatistics01C02(Sitioreport data);//---/
        Task<(Results result, object statistics)> GetStatistics01C03(Sitioreport data);//---/
        Task<(Results result, object statistics)> GetStatistics01C04(Sitioreport data);//---/

        Task<(Results result, object statistics)> GetStatistics02C01(int type1, int type2, string code);
        Task<(Results result, object statistics)> GetStatistics02C02(int type1, int type2, string code);
        Task<(Results result, object statistics)> GetStatistics02C03(int type1, int type2, string code);
        Task<(Results result, object statistics)> GetStatistics02C04(int type1, int type2, string code);

        Task<(Results result, object data)> MaximizedControl01lvl01(Sitioreport data);//---/
        Task<(Results result, object data)> MaximizedControl02lvl01(int loctype, string code, int type);
        Task<(Results result, object data)> MaximizedControl01lvl03(Sitioreport data);//---/
        Task<(Results result, object data)> MaximizedControl02lvl03(int loctype, string code, int type);

        Task<(Results result, object household)> DynamicReportData(Report report);

        Task<(Results result, object household)> GetComplaints(string from, string to);
        Task<(Results result, object household)> GetOrgs(Sitioreport data);//---/

        Task<(Results result, object household)> GetPreferences(ReportSettings settings);

        Task<(Results result, string message)> AddReportPreference(ReportSettings settings);

    }
    public class BarangayReportRepository : IBarangayReportRepository
    {
        private readonly ISubscriber _identity;
        public readonly IRepository _repo;
        public STLAccount account { get { return _identity.AccountIdentity(); } }
        public BarangayReportRepository(ISubscriber identity, IRepository repo)
        {
            _identity = identity;
            _repo = repo;
        }


        public async Task<(Results result, object household)> LoadReportperSitio(Report report)
        {
            var results = _repo.DSpQuery<dynamic>($"dbo.spfn_REPORTINGperSitio", new Dictionary<string, object>()
            {
                //{"parmplid",account.PL_ID },
                {"parmplid", account.PL_ID },
                {"parmpgrpid", account.PGRP_ID },

                {"parmxml", report.XML },

                {"parmbrgycode", "" },

                {"parmnum", report.typeOfReport },

                {"parmcode", report.code },
                {"parmloctype", report.loctype },
                {"parmaccttype", account.ACT_TYP },

            });
            if (results != null)
                return (Results.Success, results);
            return (Results.Null, null);
        }

        public async Task<(Results result, object household)> DataAnalytics(Report report)
        {
            var results = _repo.DSpQuery<dynamic>($"dbo.spfn_DATAANALYTICS", new Dictionary<string, object>()
            {
                //{"parmplid",account.PL_ID },
                {"parmxml", report.XML },
                {"parmbrgycode", report.brgyCode },

            });
            if (results != null)
                return (Results.Success, results);
            return (Results.Null, null);
        }

        public async Task<(Results result, object household)> GetStatisticalData(StatisticalData data)
        {
            var results = _repo.DSpQuery<dynamic>($"dbo.spfn_STATISTICS", new Dictionary<string, object>()
            {
                {"parmplid",account.PL_ID },
                {"parmpgrpid",account.PGRP_ID },
                {"parmaccttype",account.ACT_TYP },
                {"parmcode", data.code },
                {"parmloctype", data.loctype },
                {"parmxml", data.xml },


            });
            if (results != null)
                return (Results.Success, results);
            return (Results.Null, null);
        }

        public async Task<(Results result, object household)> GetStatisticalData02(StatisticalData data)
        {
            var results = _repo.DSpQuery<dynamic>($"dbo.spfn_STATISTICS01", new Dictionary<string, object>()
            {

                {"parmsubtype",account.SUB_TYP },
                {"parmcode", data.code },
                {"parmloctype", data.loctype },
                {"parmxml", data.xml },


            });
            if (results != null)
                return (Results.Success, results);
            return (Results.Null, null);
        }

        public async Task<(Results result, object statistics)> GetStatistics0100(Sitioreport data)
        {
            var results = _repo.DSpQuery<dynamic>($"dbo.spfn_STATISTICS_01_00", new Dictionary<string, object>()
            {
                {"parmplid",account.PL_ID },
                {"parmpgrpid",account.PGRP_ID },
                {"parmxml", data.json },
            });
            if (results != null)
                return (Results.Success, results);
            return (Results.Null, null);
        }

        public async Task<(Results result, object statistics)> GetStatistics0200(int type, string code)
        {
            var results = _repo.DSpQuery<dynamic>($"dbo.spfn_STATISTICS_02_00", new Dictionary<string, object>()
            {
                {"parmtype", type },
                {"parmcode", code },
                //{"parmxml", xml },
            });
            if (results != null)
                return (Results.Success, results);
            return (Results.Null, null);
        }

        
        // A1
        public async Task<(Results result, object statistics)> GetStatistics01A01(Sitioreport data)
        {
            var results = _repo.DSpQuery<dynamic>($"dbo.spfn_STATISTICS_01_A01", new Dictionary<string, object>()
            {
                {"parmplid",account.PL_ID },
                {"parmpgrpid",account.PGRP_ID },
                {"parmxml", data.json },
            });
            if (results != null)
                return (Results.Success, results);
            return (Results.Null, null);
        }

        public async Task<(Results result, object statistics)> GetStatistics01A02(Sitioreport data)
        {
            var results = _repo.DSpQuery<dynamic>($"dbo.spfn_STATISTICS_01_A02", new Dictionary<string, object>()
            {
                {"parmplid",account.PL_ID },
                {"parmpgrpid",account.PGRP_ID },
                {"parmxml", data.json },
            });
            if (results != null)
                return (Results.Success, results);
            return (Results.Null, null);
        }

        public async Task<(Results result, object statistics)> GetStatistics01A03(Sitioreport data)
        {
            var results = _repo.DSpQuery<dynamic>($"dbo.spfn_STATISTICS_01_A03", new Dictionary<string, object>()
            {
                {"parmplid",account.PL_ID },
                {"parmpgrpid",account.PGRP_ID },
                {"parmxml", data.json },
            });
            if (results != null)
                return (Results.Success, results);
            return (Results.Null, null);
        }

        public async Task<(Results result, object statistics)> GetStatistics01A04(Sitioreport data)
        {
            var results = _repo.DSpQuery<dynamic>($"dbo.spfn_STATISTICS_01_A04", new Dictionary<string, object>()
            {
                {"parmplid",account.PL_ID },
                {"parmpgrpid",account.PGRP_ID },
                {"parmxml", data.json },
            });
            if (results != null)
                return (Results.Success, results);
            return (Results.Null, null);
        }

        public async Task<(Results result, object statistics)> GetStatistics01A05(Sitioreport data)
        {
            var results = _repo.DSpQuery<dynamic>($"dbo.spfn_STATISTICS_01_A05", new Dictionary<string, object>()
            {
                {"parmplid",account.PL_ID },
                {"parmpgrpid",account.PGRP_ID },
                {"parmxml", data.json },
            });
            if (results != null)
                return (Results.Success, results);
            return (Results.Null, null);
        }

        public async Task<(Results result, object statistics)> GetStatistics01A06(Sitioreport data)
        {
            var results = _repo.DSpQuery<dynamic>($"dbo.spfn_STATISTICS_01_A06", new Dictionary<string, object>()
            {
                {"parmplid",account.PL_ID },
                {"parmpgrpid",account.PGRP_ID },
                {"parmxml", data.json },
            });
            if (results != null)
                return (Results.Success, results);
            return (Results.Null, null);
        }

        //End of A1
        //--- A2
        public async Task<(Results result, object statistics)> GetStatistics02A01(int type1, int type2, string code)
        {
            var results = _repo.DSpQuery<dynamic>($"dbo.spfn_STATISTICS_02_A01", new Dictionary<string, object>()
            {
                {"parmstattype",type1 },
                {"parmloctype", type2 },
                {"parmcode", code },
            });
            if (results != null)
                return (Results.Success, results);
            return (Results.Null, null);
        }

        public async Task<(Results result, object statistics)> GetStatistics02A02(int type1, int type2, string code)
        {
            var results = _repo.DSpQuery<dynamic>($"dbo.spfn_STATISTICS_02_A02", new Dictionary<string, object>()
            {
                {"parmstattype",type1 },
                {"parmloctype", type2 },
                {"parmcode", code },
            });
            if (results != null)
                return (Results.Success, results);
            return (Results.Null, null);
        }

        public async Task<(Results result, object statistics)> GetStatistics02A03(int type1, int type2, string code)
        {
            var results = _repo.DSpQuery<dynamic>($"dbo.spfn_STATISTICS_02_A03", new Dictionary<string, object>()
            {
                {"parmstattype",type1 },
                {"parmloctype", type2 },
                {"parmcode", code },
            });
            if (results != null)
                return (Results.Success, results);
            return (Results.Null, null);
        }

        public async Task<(Results result, object statistics)> GetStatistics02A04(int type1, int type2, string code)
        {
            var results = _repo.DSpQuery<dynamic>($"dbo.spfn_STATISTICS_02_A04", new Dictionary<string, object>()
            {
                {"parmstattype",type1 },
                {"parmloctype", type2 },
                {"parmcode", code },
            });
            if (results != null)
                return (Results.Success, results);
            return (Results.Null, null);
        }

        public async Task<(Results result, object statistics)> GetStatistics02A05(int type1, int type2, string code)
        {
            var results = _repo.DSpQuery<dynamic>($"dbo.spfn_STATISTICS_02_A05", new Dictionary<string, object>()
            {
                {"parmstattype",type1 },
                {"parmloctype", type2 },
                {"parmcode", code },
            });
            if (results != null)
                return (Results.Success, results);
            return (Results.Null, null);
        }

        public async Task<(Results result, object statistics)> GetStatistics02A06(int type1, int type2, string code)
        {
            var results = _repo.DSpQuery<dynamic>($"dbo.spfn_STATISTICS_02_A06", new Dictionary<string, object>()
            {
                {"parmstattype",type1 },
                {"parmloctype", type2 },
                {"parmcode", code },
            });
            if (results != null)
                return (Results.Success, results);
            return (Results.Null, null);
        }

        // End of A2

        //B1

        public async Task<(Results result, object statistics)> GetStatistics01B01(Sitioreport data)
        {
            var results = _repo.DSpQuery<dynamic>($"dbo.spfn_STATISTICS_01_B01", new Dictionary<string, object>()
            {
                {"parmplid",account.PL_ID },
                {"parmpgrpid",account.PGRP_ID },
                {"parmxml", data.json },
            });
            if (results != null)
                return (Results.Success, results);
            return (Results.Null, null);
        }

        public async Task<(Results result, object statistics)> GetStatistics01B02(Sitioreport data)
        {
            var results = _repo.DSpQuery<dynamic>($"dbo.spfn_STATISTICS_01_B02", new Dictionary<string, object>()
            {
                {"parmplid",account.PL_ID },
                {"parmpgrpid",account.PGRP_ID },
                {"parmxml", data.json },
            });
            if (results != null)
                return (Results.Success, results);
            return (Results.Null, null);
        }

        public async Task<(Results result, object statistics)> GetStatistics01B03(Sitioreport data)
        {
            var results = _repo.DSpQuery<dynamic>($"dbo.spfn_STATISTICS_01_B03", new Dictionary<string, object>()
            {
                {"parmplid",account.PL_ID },
                {"parmpgrpid",account.PGRP_ID },
                {"parmxml", data.json },
            });
            if (results != null)
                return (Results.Success, results);
            return (Results.Null, null);
        }

        public async Task<(Results result, object statistics)> GetStatistics01B04(Sitioreport data)
        {
            var results = _repo.DSpQuery<dynamic>($"dbo.spfn_STATISTICS_01_B04", new Dictionary<string, object>()
            {
                {"parmplid",account.PL_ID },
                {"parmpgrpid",account.PGRP_ID },
                {"parmxml", data.json },
            });
            if (results != null)
                return (Results.Success, results);
            return (Results.Null, null);
        }

        //End of B1
        // B2
        public async Task<(Results result, object statistics)> GetStatistics02B01(int type1, int type2, string code)
        {
            var results = _repo.DSpQuery<dynamic>($"dbo.spfn_STATISTICS_02_B01", new Dictionary<string, object>()
            {
                {"parmstattype",type1 },
                {"parmloctype", type2 },
                {"parmcode", code },
            });
            if (results != null)
                return (Results.Success, results);
            return (Results.Null, null);
        }

        public async Task<(Results result, object statistics)> GetStatistics02B02(int type1, int type2, string code)
        {
            var results = _repo.DSpQuery<dynamic>($"dbo.spfn_STATISTICS_02_B02", new Dictionary<string, object>()
            {
                {"parmstattype",type1 },
                {"parmloctype", type2 },
                {"parmcode", code },
            });
            if (results != null)
                return (Results.Success, results);
            return (Results.Null, null);
        }

        public async Task<(Results result, object statistics)> GetStatistics02B03(int type1, int type2, string code)
        {
            var results = _repo.DSpQuery<dynamic>($"dbo.spfn_STATISTICS_02_B03", new Dictionary<string, object>()
            {
                {"parmstattype",type1 },
                {"parmloctype", type2 },
                {"parmcode", code },
            });
            if (results != null)
                return (Results.Success, results);
            return (Results.Null, null);
        }

        public async Task<(Results result, object statistics)> GetStatistics02B04(int type1, int type2, string code)
        {
            var results = _repo.DSpQuery<dynamic>($"dbo.spfn_STATISTICS_02_B04", new Dictionary<string, object>()
            {
                {"parmstattype",type1 },
                {"parmloctype", type2 },
                {"parmcode", code },
            });
            if (results != null)
                return (Results.Success, results);
            return (Results.Null, null);
        }
        // End of B2
        // C1

        public async Task<(Results result, object statistics)> GetStatistics01C01(Sitioreport data)
        {
            var results = _repo.DSpQuery<dynamic>($"dbo.spfn_STATISTICS_01_C01", new Dictionary<string, object>()
            {
                {"parmplid",account.PL_ID },
                {"parmpgrpid",account.PGRP_ID },
                {"parmxml", data.json },
            });
            if (results != null)
                return (Results.Success, results);
            return (Results.Null, null);
        }

        public async Task<(Results result, object statistics)> GetStatistics01C02(Sitioreport data)
        {
            var results = _repo.DSpQuery<dynamic>($"dbo.spfn_STATISTICS_01_C02", new Dictionary<string, object>()
            {
                {"parmplid",account.PL_ID },
                {"parmpgrpid",account.PGRP_ID },
                {"parmxml", data.json },
            });
            if (results != null)
                return (Results.Success, results);
            return (Results.Null, null);
        }
        public async Task<(Results result, object statistics)> GetStatistics01C03(Sitioreport data)
        {
            var results = _repo.DSpQuery<dynamic>($"dbo.spfn_STATISTICS_01_C03", new Dictionary<string, object>()
            {
                {"parmplid",account.PL_ID },
                {"parmpgrpid",account.PGRP_ID },
                {"parmxml", data.json },
            });
            if (results != null)
                return (Results.Success, results);
            return (Results.Null, null);
        }

        public async Task<(Results result, object statistics)> GetStatistics01C04(Sitioreport data)
        {
            var results = _repo.DSpQuery<dynamic>($"dbo.spfn_STATISTICS_01_C04", new Dictionary<string, object>()
            {
                {"parmplid",account.PL_ID },
                {"parmpgrpid",account.PGRP_ID },
                {"parmxml", data.json },
            });
            if (results != null)
                return (Results.Success, results);
            return (Results.Null, null);
        }

        //End of C1
        //C2
        public async Task<(Results result, object statistics)> GetStatistics02C01(int type1, int type2, string code)
        {
            var results = _repo.DSpQuery<dynamic>($"dbo.spfn_STATISTICS_02_C01", new Dictionary<string, object>()
            {
                {"parmstattype",type1 },
                {"parmloctype", type2 },
                {"parmcode", code },
            });
            if (results != null)
                return (Results.Success, results);
            return (Results.Null, null);
        }

        public async Task<(Results result, object statistics)> GetStatistics02C02(int type1, int type2, string code)
        {
            var results = _repo.DSpQuery<dynamic>($"dbo.spfn_STATISTICS_02_C02", new Dictionary<string, object>()
            {
                {"parmstattype",type1 },
                {"parmloctype", type2 },
                {"parmcode", code },
            });
            if (results != null)
                return (Results.Success, results);
            return (Results.Null, null);
        }
        public async Task<(Results result, object statistics)> GetStatistics02C03(int type1, int type2, string code)
        {
            var results = _repo.DSpQuery<dynamic>($"dbo.spfn_STATISTICS_02_C03", new Dictionary<string, object>()
            {
                {"parmstattype",type1 },
                {"parmloctype", type2 },
                {"parmcode", code },
            });
            if (results != null)
                return (Results.Success, results);
            return (Results.Null, null);
        }

        public async Task<(Results result, object statistics)> GetStatistics02C04(int type1, int type2, string code)
        {
            var results = _repo.DSpQuery<dynamic>($"dbo.spfn_STATISTICS_02_C04", new Dictionary<string, object>()
            {
                {"parmstattype",type1 },
                {"parmloctype", type2 },
                {"parmcode", code },
            });
            if (results != null)
                return (Results.Success, results);
            return (Results.Null, null);
        }

        public async Task<(Results result, object data)> MaximizedControl01lvl01(Sitioreport data)
        {
            switch(data.type)
            {
                case 1:
                    var results = _repo.DSpQuery<dynamic>($"dbo.spfn_STATISTICS_01_A01", new Dictionary<string, object>()
                    {
                        {"parmplid",account.PL_ID },
                        {"parmpgrpid",account.PGRP_ID },
                        {"parmxml", data.json },
                        {"parmtype", 2 },
                    });
                    if (results != null)
                        return (Results.Success, results);
                    else return (Results.Null, null);

                case 2:
                    var results2 = _repo.DSpQuery<dynamic>($"dbo.spfn_STATISTICS_01_A02", new Dictionary<string, object>()
                    {
                        {"parmplid",account.PL_ID },
                        {"parmpgrpid",account.PGRP_ID },
                        {"parmxml", data.json },
                        {"parmtype", 2 },
                    });
                    if (results2 != null)
                        return (Results.Success, results2);
                    else return (Results.Null, null);

                case 3:
                    var results3 = _repo.DSpQuery<dynamic>($"dbo.spfn_STATISTICS_01_A03", new Dictionary<string, object>()
                    {
                        {"parmplid",account.PL_ID },
                        {"parmpgrpid",account.PGRP_ID },
                        {"parmxml", data.json },
                        {"parmtype", 2 },
                    });
                    if (results3 != null)
                        return (Results.Success, results3);
                    else return (Results.Null, null);

                case 4:
                    var results4 = _repo.DSpQuery<dynamic>($"dbo.spfn_STATISTICS_01_A04", new Dictionary<string, object>()
                    {
                        {"parmplid",account.PL_ID },
                        {"parmpgrpid",account.PGRP_ID },
                        {"parmxml", data.json },
                        {"parmtype", 2 },
                    });
                    if (results4 != null)
                        return (Results.Success, results4);
                    else return (Results.Null, null);

                case 5:
                    var results5 = _repo.DSpQuery<dynamic>($"dbo.spfn_STATISTICS_01_A05", new Dictionary<string, object>()
                    {
                        {"parmplid",account.PL_ID },
                        {"parmpgrpid",account.PGRP_ID },
                        {"parmxml", data.json },
                        {"parmtype", 2 },
                    });
                    if (results5 != null)
                        return (Results.Success, results5);
                    else return (Results.Null, null);

                case 6:
                    var results6 = _repo.DSpQuery<dynamic>($"dbo.spfn_STATISTICS_01_A06", new Dictionary<string, object>()
                    {
                        {"parmplid",account.PL_ID },
                        {"parmpgrpid",account.PGRP_ID },
                        {"parmxml", data.json },
                        {"parmtype", 2 },
                    });
                    if (results6 != null)
                        return (Results.Success, results6);
                    else return (Results.Null, null);

                default: return (Results.Null, null);
            }
        }


        public async Task<(Results result, object data)> MaximizedControl02lvl01(int loctype, string code, int type)
        {
            switch (type)
            {
                case 1:
                    var results = _repo.DSpQuery<dynamic>($"dbo.spfn_STATISTICS_02_A01", new Dictionary<string, object>()
                    {
                        {"parmstattype",2 },
                        {"parmloctype", loctype },
                        {"parmcode", code },
                    });
                    if (results != null)
                        return (Results.Success, results);
                    else return (Results.Null, null);

                case 2:
                    var results2 = _repo.DSpQuery<dynamic>($"dbo.spfn_STATISTICS_02_A02", new Dictionary<string, object>()
                    {
                        {"parmstattype",2 },
                        {"parmloctype", loctype },
                        {"parmcode", code },
                    });
                    if (results2 != null)
                        return (Results.Success, results2);
                    else return (Results.Null, null);

                case 3:
                    var results3 = _repo.DSpQuery<dynamic>($"dbo.spfn_STATISTICS_02_A03", new Dictionary<string, object>()
                    {
                        {"parmstattype",2 },
                        {"parmloctype", loctype },
                        {"parmcode", code },
                    });
                    if (results3 != null)
                        return (Results.Success, results3);
                    else return (Results.Null, null);

                case 4:
                    var results4 = _repo.DSpQuery<dynamic>($"dbo.spfn_STATISTICS_02_A04", new Dictionary<string, object>()
                    {
                        {"parmstattype",2 },
                        {"parmloctype", loctype },
                        {"parmcode", code },
                    });
                    if (results4 != null)
                        return (Results.Success, results4);
                    else return (Results.Null, null);

                case 5:
                    var results5 = _repo.DSpQuery<dynamic>($"dbo.spfn_STATISTICS_02_A05", new Dictionary<string, object>()
                    {
                        {"parmstattype",2 },
                        {"parmloctype", loctype },
                        {"parmcode", code },
                    });
                    if (results5 != null)
                        return (Results.Success, results5);
                    else return (Results.Null, null);

                case 6:
                    var results6 = _repo.DSpQuery<dynamic>($"dbo.spfn_STATISTICS_02_A06", new Dictionary<string, object>()
                    {
                        {"parmstattype",2 },
                        {"parmloctype", loctype },
                        {"parmcode", code },
                    });
                    if (results6 != null)
                        return (Results.Success, results6);
                    else return (Results.Null, null);

                default: return (Results.Null, null);
            }
        }
        public async Task<(Results result, object data)> MaximizedControl01lvl03(Sitioreport data)
        {
            switch (data.type)
            {
                case 1:
                    var results = _repo.DSpQuery<dynamic>($"dbo.spfn_STATISTICS_01_C01", new Dictionary<string, object>()
                    {
                        {"parmplid",account.PL_ID },
                        {"parmpgrpid",account.PGRP_ID },
                        {"parmxml", data.json },
                        {"parmtype", 2 },
                    });
                    if (results != null)
                        return (Results.Success, results);
                    else return (Results.Null, null);

                case 2:
                    var results2 = _repo.DSpQuery<dynamic>($"dbo.spfn_STATISTICS_01_C02", new Dictionary<string, object>()
                    {
                        {"parmplid",account.PL_ID },
                        {"parmpgrpid",account.PGRP_ID },
                        {"parmxml", data.json },
                        {"parmtype", 2 },
                    });
                    if (results2 != null)
                        return (Results.Success, results2);
                    else return (Results.Null, null);

                case 3:
                    var results3 = _repo.DSpQuery<dynamic>($"dbo.spfn_STATISTICS_01_C03", new Dictionary<string, object>()
                    {
                        {"parmplid",account.PL_ID },
                        {"parmpgrpid",account.PGRP_ID },
                        {"parmxml", data.json },
                        {"parmtype", 2 },
                    });
                    if (results3 != null)
                        return (Results.Success, results3);
                    else return (Results.Null, null);

                case 4:
                    var results4 = _repo.DSpQuery<dynamic>($"dbo.spfn_STATISTICS_01_C04", new Dictionary<string, object>()
                    {
                        {"parmplid",account.PL_ID },
                        {"parmpgrpid",account.PGRP_ID },
                        {"parmxml", data.json },
                        {"parmtype", 2 },
                    });
                    if (results4 != null)
                        return (Results.Success, results4);
                    else return (Results.Null, null);

                default: return (Results.Null, null);
            }
        }

        public async Task<(Results result, object data)> MaximizedControl02lvl03(int loctype, string code, int type)
        {
            switch (type)
            {
                case 1:
                    var results = _repo.DSpQuery<dynamic>($"dbo.spfn_STATISTICS_02_C01", new Dictionary<string, object>()
                    {
                        {"parmstattype",2 },
                        {"parmloctype", loctype },
                        {"parmcode", code },
                    });
                    if (results != null)
                        return (Results.Success, results);
                    else return (Results.Null, null);

                case 2:
                    var results2 = _repo.DSpQuery<dynamic>($"dbo.spfn_STATISTICS_02_C02", new Dictionary<string, object>()
                    {
                        {"parmstattype",2 },
                        {"parmloctype", loctype },
                        {"parmcode", code },
                    });
                    if (results2 != null)
                        return (Results.Success, results2);
                    else return (Results.Null, null);

                case 3:
                    var results3 = _repo.DSpQuery<dynamic>($"dbo.spfn_STATISTICS_02_C03", new Dictionary<string, object>()
                    {
                        {"parmstattype",2 },
                        {"parmloctype", loctype },
                        {"parmcode", code },
                    });
                    if (results3 != null)
                        return (Results.Success, results3);
                    else return (Results.Null, null);

                case 4:
                    var results4 = _repo.DSpQuery<dynamic>($"dbo.spfn_STATISTICS_02_C04", new Dictionary<string, object>()
                    {
                        {"parmstattype",2 },
                        {"parmloctype", loctype },
                        {"parmcode", code },
                    });
                    if (results4 != null)
                        return (Results.Success, results4);
                    else return (Results.Null, null);

                default: return (Results.Null, null);
            }
        }

        public async Task<(Results result, object household)> DynamicReportData(Report report)
        {
            var results = _repo.DSpQuery<dynamic>($"dbo.spfn_REPORTINGCustom", new Dictionary<string, object>()
            {
                //{"parmplid",account.PL_ID },
                {"parmplid", account.PL_ID },
                {"parmpgrpid", account.PGRP_ID },
                {"parmxml", report.XML },
                {"parmbrgycode", report.brgyCode },
                {"parmcolumnBitString", report.columnBits },
                {"parmfilterBitString", report.filterBits },
                {"parmagefrom", report.agefrom },
                {"parmageto", report.ageto },
                {"parmcode", report.code },
                {"parmloctype", report.loctype },
                {"parmaccttype", account.ACT_TYP },

            });
            if (results != null)
                return (Results.Success, results);
            return (Results.Null, null);
        }

        public async Task<(Results result, object household)> GetComplaints(string from, string to)
        {
            var results = _repo.DSpQuery<dynamic>($"dbo.spfn_COMPLAINTSSHOW", new Dictionary<string, object>()
            {
                //{"parmplid",account.PL_ID },
                {"parmplid",  account.PL_ID },
                {"parmpgrpid", account.PGRP_ID},
                {"parmfrom", from },
                {"parmto", to},

            });
            if (results != null)
                return (Results.Success, results);
            return (Results.Null, null);
        }

        public async Task<(Results result, object household)> GetOrgs(Sitioreport data)
        {
            var results = _repo.DSpQuery<dynamic>($"dbo.spfn_REPORTINGorgz", new Dictionary<string, object>()
            {
                //{"parmplid",account.PL_ID },
                {"parmplid", account.PL_ID },
                {"parmpgrpid", account.PGRP_ID},
                {"parmxml", data.json },


            });
            if (results != null)
                return (Results.Success, results);
            return (Results.Null, null);
        }

        public async Task<(Results result, object household)> GetPreferences(ReportSettings settings)
        {
            var results = _repo.DSpQuery<dynamic>($"dbo.spfn_REPORTPREFERENCE_GET", new Dictionary<string, object>()
            {
                //{"parmplid",account.PL_ID },
                {"parmplid", settings.plid},
                {"parmpgrpid", settings.pgrpid},
                {"parmuserid", settings.userid },

            });
            if (results != null)
                return (Results.Success, results);
            return (Results.Null, null);
        }

        public async Task<(Results result, string message)> AddReportPreference(ReportSettings settings)
        {
            var result = _repo.DSpQuery<dynamic>($"dbo.spfn_REPORTPREFERENCE_INSERT", new Dictionary<string, object>()
            {

                {"parmplid", settings.plid},
                {"parmpgrpid", settings.pgrpid },
                {"parmuserid", settings.userid},
                {"parmpreferencedescription", settings.description},
                {"parmlayoutindex", settings.layoutIndex },
                {"parmtitle", settings.title},
                {"parmsubtitle", settings.subtitle},
                {"parmdateformatindex", settings.dateFormat },
                {"parmselectbits", settings.selectBits.ToString()},
                {"parmdistinctionbits", settings.distinctionBits.ToString()},
                {"parmagefrom", settings.from },
                {"parmageto", settings.to},
                {"parmchartindex", settings.chartIndex.ToString()},
                {"parmsignatureindex", settings.signIndex.ToString() },
                {"parmisactive", settings.isactive },


            });

            if (result != null)
            {
                var row = ((IDictionary<string, object>)result);
                string ResultCode = row["RESULT"].Str();
                if (ResultCode == "1")
                    return (Results.Success, "Successfully saved!");
                else if (ResultCode == "2")
                    return (Results.Failed, "Already Exists!");

            }
            return (Results.Null, null);

        }

    }
}
