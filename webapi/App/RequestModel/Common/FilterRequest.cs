using System;
using System.ComponentModel.DataAnnotations;
using Comm.Commons.Extensions;

namespace webapi.App.RequestModel.Common
{
    public class FilterRequest
    {
        public bool IsAll;
        public String Transaction;
        public String From;
        public String To;
        public String Status;
        public String Method;
        public bool IsOverall;
        public bool IsPending;
        public bool IsCurrent;
        public bool IsActive;
        public String Search;
        public String BaseFilter;
        public String Page;
        public String Type;
        public int SubTyp;
        public String ID;
        public String Schedule;
        public string REF_GRP_ID;
        public string Userid;
        public string PL_ID;
        public string PGRP_ID;
        public string OTP;
        public string MOB_NO;
        public string DONO_ID;
        public string BrgyCode;
        public string num_row;
        public string DocTypeID;
        public string BusinessId;
        public string Gender;
        public string EmgyID;
        public string EmgyTypID;
        public string EmgyContactID;
        public string TickectNo;
        public string CommunityID;
        public string PostID;
        public int TypeLevel;

        public DateTime? TransactionDt;
        public DateTime? FromDt;
        public DateTime? ToDt;

        public static bool validity0a(FilterRequest filter, bool add1Day = true){
            if(filter == null) return false;
            DateTime today = DateTime.Now;
            try{ filter.TransactionDt = DateTime.Parse(filter.Transaction); } catch{ }
            try{ filter.FromDt = DateTime.Parse(filter.From); } catch{ filter.FromDt = today; }
            try{ filter.ToDt = DateTime.Parse(filter.To); } catch{ filter.ToDt = today; }

            if(!(filter.TransactionDt==null||(filter.TransactionDt??DateTime.Now).Year<2000))
                filter.Transaction = (filter.TransactionDt??DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss.fff");
            
            if(add1Day) 
                filter.ToDt = (filter.ToDt??today).AddDays(1);
                
            filter.From = (filter.FromDt??today).ToString("yyyy-MM-dd");
            filter.To = (filter.ToDt??today).ToString("yyyy-MM-dd");
            filter.IsOverall = (filter.IsAll || filter.IsOverall);
            return true;
        }

        public static bool validity0b(FilterRequest filter){
            if(filter == null) return false;
            filter.BaseFilter = ((int)filter.BaseFilter.Str().ToDecimalDouble()).ToString("0");
            return true;
        }

        public static bool validity0c(FilterRequest filter){
            if(filter == null) return false;
            validity0g(filter);
            if(!(filter.Status=="1"||filter.Status=="2"||filter.Status=="0")) 
                filter.Status = "";
            if(!(filter.Method=="1"||filter.Method=="2"))
                filter.Method = "";
            return true;
        }

        public static bool validity0d(FilterRequest filter){
            if(filter == null) return false;
            filter.Search = filter.Search.Str();
            filter.BaseFilter = filter.BaseFilter.Str();
            return true;
        }
        public static bool validity0e(FilterRequest filter){
            if(filter == null) return false;
            DateTime today = DateTime.Now;
            try{ filter.FromDt = DateTime.Parse(filter.From); } catch{ filter.FromDt = today; }
            try{ filter.ToDt = DateTime.Parse(filter.To); } catch{ filter.ToDt = today; }

            filter.From = (filter.FromDt??today).ToString("yyyy-MM-dd");
            filter.To = (filter.ToDt??today).ToString("yyyy-MM-dd");
            filter.IsOverall = (filter.IsAll || filter.IsOverall);
            return true;
        }

        public static bool validity0f(FilterRequest filter){
            if(filter == null) return false;
            validity0e(filter);
            string status = filter.Status.Str().ToLower();
            if(status.Equals("pending")) filter.Status = "0";
            else if(status.Equals("approved")||status.Equals("completed")) filter.Status = "1";
            else if(status.Equals("cancelled")) filter.Status = "2";
            else filter.Status = "";

            return (!filter.Status.IsEmpty());
        }
        public static bool validity0g(FilterRequest filter){
            if(filter == null) return false;
            filter.TransactionDt = null;
            try{ filter.TransactionDt = DateTime.Parse(filter.BaseFilter); } catch{ }
            filter.BaseFilter = "";
            if(!(filter.TransactionDt==null||(filter.TransactionDt??DateTime.Now).Year<2000))
                filter.BaseFilter = (filter.TransactionDt??DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss.fff");
            return true;
        }
        public static bool validity0h(FilterRequest filter){
            if(filter == null) return false;
            validity0g(filter);
            filter.IsOverall = (filter.IsAll || filter.Type.Str().IsEmpty());
            return true;
        }

    }
}