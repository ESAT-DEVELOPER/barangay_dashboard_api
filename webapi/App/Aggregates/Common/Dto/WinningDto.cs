using System;
using System.Collections.Generic;
using Comm.Commons.Extensions;
using System.Linq;

namespace webapi.App.Aggregates.Common
{
    public class WinningDto
    {
        public static IEnumerable<dynamic> FilterEncashOnProcesses(IEnumerable<dynamic> list, int limit = 50)
        {
            if (list == null) return null;
            var items = EncashOnProcesses(list);
            var count = items.Count();
            if (count >= limit)
            {
                var o = items.Last();
                var filter = (o.NextFilter = Dynamic.Object);
                items = items.Take(count - 1).Concat(new[] { o });
                filter.BaseFilter = o.DateTransaction;
            }
            return items;
        }
        public static IEnumerable<dynamic> EncashOnProcesses(IEnumerable<dynamic> list)
        {
            if (list == null) return null;
            return list.Select(e => EncashOnProcess(e));
        }

        public static IDictionary<string, object> EncashOnProcess(IDictionary<string, object> data)
        {
            dynamic o = Dynamic.Object;
            o.TransactionNo = data["TRN_NO"];
            o.DateTransaction = data["RGS_TRN_TS"];
            string type = data["ENC_TYP"].Str();
            string remittance = data["ENC_COM"].Str();
            bool IsBank = (type.Equals("01"));
            bool IsCashDelivery = (type.Equals("02"));
            bool IsMoneyTransfer = (type.Equals("03"));
            bool IsConvertToGameCredit = (type.Equals("04"));
            if (IsBank)
            {
                var b = (o.Bank = Dynamic.Object);
                b.AccountName = data["NME"].Str();
                b.AccountNo = data["NMBER"].Str();
                o.IsBank = true;
            }
            else if (IsCashDelivery || IsMoneyTransfer)
            {
                var r = (o.Receiver = Dynamic.Object);
                r.Fullname = data["NME"].Str();
                r.MobileNumber = data["NMBER"].Str();
                if (IsCashDelivery)
                {
                    o.IsCashDelivery = true;
                    r.Address = data["ADDR"].Str();
                }
                else o.IsMoneyTransfer = true;
            }
            else if (IsConvertToGameCredit)
            {
                o.IsConvertToGameCredit = true;
            }
            o.Remittance = data["REM_NM"].Str();//Datas.Remittances.GetValue(remittance);
            o.RemittanceLogoUrl = data["REM_LOGO"].Str();
            o.Amount = data["TOT_AMT"].Str().ToDecimalDouble();
            o.IsPending = true;
            try
            {
                DateTime datetime = data["RGS_TRN_TS"].To<DateTime>();
                o.DateRequested = datetime.ToString("MMM dd, yyyy");
                o.TimeRequested = datetime.ToString("hh:mm:ss tt");
                o.FulldateRequested = $"{o.DateRequested} {o.TimeRequested}";
            }
            catch { }
            return o;
        }

        public static IEnumerable<dynamic> FilterEncashCompletions(IEnumerable<dynamic> list, int limit = 50)
        {
            if (list == null) return null;
            var items = EncashCompletions(list);
            var count = items.Count();
            if (count >= limit)
            {
                var o = items.Last();
                var filter = (o.NextFilter = Dynamic.Object);
                items = items.Take(count - 1).Concat(new[] { o });
                filter.BaseFilter = o.DateTransaction;
            }
            return items;
        }
        public static IEnumerable<dynamic> EncashCompletions(IEnumerable<dynamic> list)
        {
            if (list == null) return null;
            return list.Select(e => EncashCompletion(e));
        }

        public static IDictionary<string, object> EncashCompletion(IDictionary<string, object> data)
        {
            dynamic o = EncashOnProcess(data);
            string status = data["RQS_STAT"].Str().Trim();
            o.IsPending = false;
            if (status.Equals("1"))
            {
                var d = (o.Depositor = Dynamic.Object);
                d.Fullname = data["SNDR_NME"].Str();
                d.MobileNumber = data["SNDR_CNTCT_NO"].Str();
                d.ReferenceID = data["REF_NO"].Str();
                d.WinAmount = data["TOT_AMT"].Str().ToDecimalDouble();
                d.ServiceFee = data["SVC_CHG"].Str().ToDecimalDouble();
                d.TotalSent = data["TOT_SNT"].Str().ToDecimalDouble();
                o.DateTransaction = data["UPD_TRN_TS"];
                o.IsCompleted = true;
                try
                {
                    DateTime datetime = data["UPD_TRN_TS"].To<DateTime>();
                    o.DateCompleted = datetime.ToString("MMM dd, yyyy");
                    o.TimeCompleted = datetime.ToString("hh:mm:ss tt");
                    o.FulldateCompleted = $"{o.DateCompleted} {o.TimeCompleted}";
                }
                catch { }
            }
            else
            {
                o.IsCancelled = true;
            }
            return o;
        }


        public static IEnumerable<dynamic> Remittances(IEnumerable<dynamic> list)
        {
            if (list == null) return null;
            return list.Select(e => Remittance(e));
        }
        public static IDictionary<string, object> Remittance(IDictionary<string, object> data)
        {
            return ((object)Dynamic.Object).Ref<dynamic>(o =>
            {
                o.Code = data["REM_CD"].Str();
                o.Description = data["REM_NM"].Str();
                o.LogoUrl = data["REM_LOGO"].Str();
                o.Type = data["ENC_CD"].Str();
            });
        }
        public static IEnumerable<dynamic> EncashmentWinnings(IEnumerable<dynamic> list)
        {
            if (list == null) return null;
            return list.Select(e => EncashmentWinning(e));
        }
        public static IDictionary<string, object> EncashmentWinning(IDictionary<string, object> data)
        {
            dynamic o = Dynamic.Object;
            o.TransactionNo = data["TRN_NO"].Str();
            o.ReferenceID = data["REF_NO"].Str();
            o.RequestID = data["RQS_ID"].Str();
            //i.primaryid =  row["TRN_NO"].Str();

            o.AccountName = o.AccountNumber = "";
            o.ReceiverName = o.ReceiverMobileNumber = o.CompleteAddress = "";
            o.WithdrawalTypeID = o.WithdrawalType = "";
            o.RemittanceID = o.RemittanceName = "";
            o.IsEncashment = true;
            string type = data["ENC_TYP"].Str();
            if (type.Equals("01"))
            {
                o.WithdrawalTypeID = "BTB";
                o.WithdrawalType = "Bank to Bank Transfer";
                o.RemittanceName = data["REM_NM"].Str();
                o.AccountName = data["NME"].Str();
                o.AccountNumber = data["NMBER"].Str();
            }
            else if (type.Equals("02"))
            {
                o.WithdrawalTypeID = "CD";
                o.WithdrawalType = "Door to Door Delivery";
                o.RemittanceName = data["REM_NM"].Str();
                o.ReceiverName = data["NME"].Str();
                o.ReceiverMobileNumber = data["NMBER"].Str();
                o.CompleteAddress = data["ADDR"].Str();
            }
            else if (type.Equals("03"))
            {
                o.WithdrawalTypeID = "MRC";
                o.WithdrawalType = "Money Remittance Center";
                o.RemittanceName = data["REM_NM"].Str();
                o.ReceiverName = data["NME"].Str();
                o.ReceiverMobileNumber = data["NMBER"].Str();
            }
            else if (type.Equals("04"))
            {
                o.WithdrawalTypeID = "CGC";
                o.WithdrawalType = "Convert to Game Credit";
                o.RemittanceName = "Convert to Game Credit";
                o.IsEncashment = false;
                //i.RemittanceName = row["REM_NM"].Str();
                //i.ReceiverName = row["NME"].Str();
                //i.ReceiverMobileNumber = row["NMBER"].Str();
            }
            //
            o.RemittanceCode = data["ENC_COM"].Str();

            o.Fullname = data["USR_FLL_NM"].Str().Trim();
            o.AccountID = data["ACT_ID"].Str();
            o.SubscriberID = data["RGS_EMP_ID"].Str();
            o.BranchID = data["BR_CD"].Str();
            o.TotalAmount = data["TOT_AMT"].Str().ToDecimalDouble();
            o.DateRequest = data["RGS_TRN_TS"].To<DateTime>().ToString("MMMM dd, yyyy hh:mm:ss tt");
            o.DateProcess = "";

            o.IsConfirmed = (data["REQ_STAT"].Str().Equals("1"));
            o.Confirmed = (o.IsConfirmed ? "YES" : "NO");
            o.DateConfirmed = "";
            if (o.IsConfirmed)
            {
                o.DateConfirmed = data["UPD_TRN_TS"].To<DateTime>().ToString("MMMM dd, yyyy hh:mm:ss tt");
            }

            o.IsCancelled = false;
            o.Cancelled = "";
            o.CancelledRemarks = "";
            o.DateCancelled = "";

            o.ServiceCharge = data["SVC_CHG"].Str().ToDecimalDouble();
            o.SenderName = data["SNDR_NME"].Str();
            o.SenderMobileNumber = data["SNDR_CNTCT_NO"].Str();

            return o;
        }



    }
}