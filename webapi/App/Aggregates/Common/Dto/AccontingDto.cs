using System;
using System.Collections.Generic;
using Comm.Commons.Extensions;
using System.Linq;

namespace webapi.App.Aggregates.Common
{
    public class AccontingDto
    {
        public static IEnumerable<dynamic> PasaCredits(IEnumerable<dynamic> list)
        {
            if (list == null) return null;
            return list.Select(e => PasaCredit(e));
        }
        public static IDictionary<string, object> PasaCredit(IDictionary<string, object> data)
        {
            dynamic o = Dynamic.Object;
            o.Amount = data["TOT_AMT"].Str().ToDecimalDouble();
            o.DateTransaction = data["RGS_TRN_TS"].To<DateTime>().ToString("MMMM dd, yyyy hh:mm:ss tt");
            o.Fullname = data["RGS_USR_FLL_NM"].Str().Trim();
            o.ReloaderID = data["RGS_ACT_ID"].Str().Trim();
            o.Reloader = data["USR_FLL_NM"].Str().Trim();
            return o;
        }

        public static IDictionary<string, object> AccountingProfile(IDictionary<string, object> data)
        {
            dynamic o = Dynamic.Object;
            o.AccountID = data["ACT_ID"].Str();
            o.CreditBalance = data["ACT_CRDT_BAL"].Str().ToDecimalDouble();
            return o;
        }

        public static IEnumerable<dynamic> PurchaseCredits(IEnumerable<dynamic> list)
        {
            if (list == null) return null;
            return list.Select(e => PurchaseCredit(e));
        }
        public static IDictionary<string, object> PurchaseCredit(IDictionary<string, object> data)
        {
            dynamic o = Dynamic.Object;
            o.PurchaseOrderNo = data["PO_NO"].Str();
            o.Amount = data["TOT_AMT"].Str().ToDecimalDouble();
            o.DateRequested = data["RGS_TRN_TS"].To<DateTime>().ToString("MMMM dd, yyyy hh:mm:ss tt");

            if (data["REQ_STAT"].Str().Equals("1"))
            {
                o.DateTransaction = data["UPD_TRN_TS"].To<DateTime>().ToString("MMMM dd, yyyy hh:mm:ss tt");
            }
            o.Requestor = data["RGS_USR_FLL_NM"].Str().Trim();
            //o.ReloaderID = data["RGS_ACT_ID"].Str().Trim();
            //o.Reloader = data["USR_FLL_NM"].Str().Trim();
            return o;
        }
    }
}