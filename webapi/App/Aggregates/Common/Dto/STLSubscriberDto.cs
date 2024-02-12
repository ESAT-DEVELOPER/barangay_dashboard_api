using Comm.Commons.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using webapi.App.Model.User;
using webapi.App.RequestModel.AppRecruiter;
using webapi.App.STLDashboardModel;

namespace webapi.App.Aggregates.Common.Dto
{
    public class STLSubscriberDto
    {
        public static IDictionary<string, object> GetPartyList(IDictionary<string,object> data, bool fullinfo = true)
        {
            dynamic o = Dynamic.Object;
            o.PL_ID = data["PL_ID"].Str();
            o.PL_NM = data["PL_NM"].Str();
            o.PL_ADD = data["PL_ADD"].Str();
            o.PL_TEL_NO = data["PL_TEL_NO"].Str();
            o.URL_STRMNG_NM = data["URL_STRMNG_NM"].Str();
            o.URL_STRMNG = data["URL_STRMNG"].Str();
            return o;
        }
        public static STLParty GetSTLParty(IDictionary<string,object> data)
        {
            STLParty o = new STLParty();
            o.PL_ID = data["PL_ID"].Str();
            return o;
        }
        public static IDictionary<string, object> GetGroup(IDictionary<string, object> data, bool fullinfo = true)
        {
            dynamic o = Dynamic.Object;
            o.PL_ID = data["PL_ID"].Str();
            o.PGRP_ID = data["PGRP_ID"].Str();
            //o.PSN_CD = data["PSN_CD"].Str();
            o.PLTCL_NM = data["PLTCL_NM"].Str();
            return o;
        }

        public static IDictionary<string, object> GetInbox(IDictionary<string, object> data, bool fullinfo = true)
        {
            dynamic o = Dynamic.Object;
            o.MobileNumber = data["MessageFrom"].Str();
            o.TotalUnRead = data["UnreadSMS"].Str();
            return o;
        }

        public static IEnumerable<dynamic> GetSMSInboxList(IEnumerable<dynamic> data, int limit = 100, bool fullinfo = true)
        {
            if (data == null) return null;
            var items = GetSMSInbox_List(data);
            var count = items.Count();
            //if (count >= limit)
            //{
            //    var o = items.Last();
            //    var filter = (o.NextFilter = Dynamic.Object);
            //    items = items.Take(count - 1).Concat(new[] { o });
            //    filter.NextFilter = o.num_row;
            //}
            return items;
        }
        public static IEnumerable<dynamic> GetSMSInbox_List(IEnumerable<dynamic> data, bool fullinfo = true)
        {
            if (data == null) return null;
            return data.Select(e => Get_SMSInbox_List(e));
        }
        public static IDictionary<string, object> Get_SMSInbox_List(IDictionary<string, object> data, bool fullinfo = true)
        {
            dynamic o = Dynamic.Object;
            o.Id = data["Id"].Str();
            o.SMS_ID = data["SMS_ID"].Str();
            o.SendTime = Convert.ToDateTime(data["SendTime"].Str()).ToString("MMM dd, yyyy hh:mm tt");
            o.MessageFrom = data["MessageFrom"].Str();
            o.MessageTo = data["MessageTo"].Str();
            o.MessageText = data["MessageText"].Str();
            o.MessageType = data["MessageType"].Str();
            o.IsRead = (data["IsRead"].Str() == "") ? false : Convert.ToBoolean(data["IsRead"]);
            return o;
        }

        public static IDictionary<string, object> GetGroupSA(IDictionary<string, object> data, string plid, string pgrpid, bool fullinfo = true)
        {
            dynamic o = Dynamic.Object;
            o.PL_ID = plid;
            o.PGRP_ID = pgrpid;
            o.PSN_CD = data["PSN_CD"].Str();
            o.PLTCL_NM = data["PLTCL_NM"].Str();
            return o;
        }
        public static IDictionary<string, object> GetMenuPage(IDictionary<string,object> data, string accttype)
        {
            string menubar = string.Empty;
            if (data!=null)
                menubar = (data["PGS"].Str().IsEmpty()) ? "" : data["PGS"].Str();
            dynamic o = Dynamic.Object;
            o.MenuPage = (accttype== "1" || accttype=="2") ? menubar : data["PGS"].Str();
            return o;
        }
        public static STLAccount STLUpdateMember(STLMembership data)
        {
            STLAccount o = new STLAccount();
            o.PL_ID = data.PLID;
            o.PGRP_ID = data.PGRPID;
            o.USR_ID = data.Userid;
            o.ACT_TYP = data.AccountType;

            o.FRST_NM = data.Firstname;
            o.LST_NM = data.Lastname;
            o.MDL_NM = data.Middlename;
            o.FLL_NM = $"{data.Lastname}, {data.Firstname} {data.Middlename}";
            o.NCK_NM = data.Nickname;

            o.MOB_NO = data.MobileNumber;
            o.EML_ADD = data.EmailAddress;
            o.HM_ADDR = data.HomeAddress;
            o.PRSNT_ADDR = data.PresentAddress;
            o.LOC_REG = data.Region;
            o.LOC_PROV = data.Province;
            o.LOC_MUN = data.Municipality;
            o.LOC_BRGY = data.Barangay;

            o.GNDR = data.Gender;
            o.MRTL_STAT = data.MaritalStatus;
            o.CTZNSHP = data.Citizenship;
            o.ImageUrl = data.ImageUrl;
            o.BRT_DT = data.BirthDate;
            o.BLD_TYP = data.BloodType;
            o.NATNLTY = data.Nationality;
            o.OCCPTN = data.Occupation;
            o.SKLLS = data.Skills;
            return o;
        }

        public static IEnumerable<dynamic> GetAttachementIssuesConcernList(IEnumerable<dynamic> data, int limit = 100, bool fullinfo = true)
        {
            if (data == null) return null;
            var items = GetAttachementIssuesConcern_List(data);
            var count = items.Count();
            if (count >= limit)
            {
                var o = items.Last();
                var filter = (o.NextFilter = Dynamic.Object);
                items = items.Take(count - 1).Concat(new[] { o });
                filter.NextFilter = o.num_row;
            }
            return items;
        }
        public static IEnumerable<dynamic> GetAttachementIssuesConcern_List(IEnumerable<dynamic> data, bool fullinfo = true)
        {
            if (data == null) return null;
            return data.Select(e => Get_AttachementIssuesConcern_List(e));
        }
        public static IDictionary<string, object> Get_AttachementIssuesConcern_List(IDictionary<string, object> data, bool fullinfo = true)
        {
            dynamic o = Dynamic.Object;
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            o.PLID = data["CompID"].Str();
            o.GroupID = data["GroupId"].Str();
            o.SequenceNo = data["SequenceNo"].Str();
            o.AttachmentNo = data["AttachmentNo"].Str();
            o.Attachment = data["Attachment"].Str();
            return o;
        }

        public static IEnumerable<dynamic> GetAttachementReqDocAttmList(IEnumerable<dynamic> data, int limit = 100, bool fullinfo = true)
        {
            if (data == null) return null;
            var items = GetAttachementReqDocAttm_List(data);
            var count = items.Count();
            if (count >= limit)
            {
                var o = items.Last();
                var filter = (o.NextFilter = Dynamic.Object);
                items = items.Take(count - 1).Concat(new[] { o });
                filter.NextFilter = o.num_row;
            }
            return items;
        }
        public static IEnumerable<dynamic> GetAttachementReqDocAttm_List(IEnumerable<dynamic> data, bool fullinfo = true)
        {
            if (data == null) return null;
            return data.Select(e => Get_AttachementReqDocAttm_List(e));
        }
        public static IDictionary<string, object> Get_AttachementReqDocAttm_List(IDictionary<string, object> data, bool fullinfo = true)
        {
            dynamic o = Dynamic.Object;
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            o.PLID = data["CompID"].Str();
            o.GroupID = data["GroupId"].Str();
            o.SequenceNo = data["SequenceNo"].Str();
            o.AttachmentNo = data["AttachmentNo"].Str();
            o.RequestDocumentID = data["RequestDocumentID"].Str();
            o.Attachment = data["Attachment"].Str();
            return o;
        }


        public static IEnumerable<dynamic> GetPurposeList(IEnumerable<dynamic> data, int limit = 1000, bool fullinfo = true)
        {
            if (data == null) return null;
            var items = GetPurpose_List(data);
            var count = items.Count();
            //if (count >= limit)
            //{
            //    var o = items.Last();
            //    var filter = (o.NextFilter = Dynamic.Object);
            //    items = items.Take(count - 1).Concat(new[] { o });
            //    filter.NextFilter = o.num_row;
            //}
            return items;
        }
        public static IEnumerable<dynamic> GetPurpose_List(IEnumerable<dynamic> data, bool fullinfo = true)
        {
            if (data == null) return null;
            return data.Select(e => Get_Purpose_List(e));
        }
        public static IDictionary<string, object> Get_Purpose_List(IDictionary<string, object> data, bool fullinfo = true)
        {
            dynamic o = Dynamic.Object;
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            o.PLID = data["PL_ID"].Str();
            o.PGRPID = data["PGRP_ID"].Str();
            o.PurposeId = data["PURP_ID"].Str();
            o.Purpose = data["Purpose"].Str();
            return o;
        }



        public static IEnumerable<dynamic> GetBusinessOwnerTypeList(IEnumerable<dynamic> data, int limit = 1000, bool fullinfo = true)
        {
            if (data == null) return null;
            var items = GetBusinessOwnerType_List(data);
            var count = items.Count();
            //if (count >= limit)
            //{
            //    var o = items.Last();
            //    var filter = (o.NextFilter = Dynamic.Object);
            //    items = items.Take(count - 1).Concat(new[] { o });
            //    filter.NextFilter = o.num_row;
            //}
            return items;
        }
        public static IEnumerable<dynamic> GetBusinessOwnerType_List(IEnumerable<dynamic> data, bool fullinfo = true)
        {
            if (data == null) return null;
            return data.Select(e => Get_BusinessOwnerType_List(e));
        }
        public static IDictionary<string, object> Get_BusinessOwnerType_List(IDictionary<string, object> data, bool fullinfo = true)
        {
            dynamic o = Dynamic.Object;
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            o.PLID = data["PL_ID"].Str();
            o.PGRPID = data["PGRP_ID"].Str();
            o.BusinessOwnershiptypeID = data["BIZTYPOWNRSHP_ID"].Str();
            o.BusinessOwnershipDescription = textInfo.ToUpper(textInfo.ToLower(data["DESCRPTN"].Str()));
            return o;
        }

        public static IEnumerable<dynamic> GetCertificateTypeList(IEnumerable<dynamic> data, int limit = 1000, bool fullinfo = true)
        {
            if (data == null) return null;
            var items = GetCertificateType_List(data);
            var count = items.Count();
            //if (count >= limit)
            //{
            //    var o = items.Last();
            //    var filter = (o.NextFilter = Dynamic.Object);
            //    items = items.Take(count - 1).Concat(new[] { o });
            //    filter.NextFilter = o.num_row;
            //}
            return items;
        }
        public static IEnumerable<dynamic> GetCertificateType_List(IEnumerable<dynamic> data, bool fullinfo = true)
        {
            if (data == null) return null;
            return data.Select(e => Get_CertificateType_List(e));
        }
        public static IDictionary<string, object> Get_CertificateType_List(IDictionary<string, object> data, bool fullinfo = true)
        {
            dynamic o = Dynamic.Object;
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            o.PLID = data["PL_ID"].Str();
            o.PGRPID = data["PGRP_ID"].Str();
            o.CertificateTypeId = data["CERTTYP_ID"].Str();
            o.CertificateType = data["CertificatType"].Str();
            return o;
        }



        public static IEnumerable<dynamic> GetBusinessNameList(IEnumerable<dynamic> data, int limit = 1000, bool fullinfo = true)
        {
            if (data == null) return null;
            var items = GetBusinessName_List(data);
            var count = items.Count();
            //if (count >= limit)
            //{
            //    var o = items.Last();
            //    var filter = (o.NextFilter = Dynamic.Object);
            //    items = items.Take(count - 1).Concat(new[] { o });
            //    filter.NextFilter = o.num_row;
            //}
            return items;
        }
        public static IEnumerable<dynamic> GetBusinessName_List(IEnumerable<dynamic> data, bool fullinfo = true)
        {
            if (data == null) return null;
            return data.Select(e => Get_BusinessName_List(e));
        }
        public static IDictionary<string, object> Get_BusinessName_List(IDictionary<string, object> data, bool fullinfo = true)
        {
            dynamic o = Dynamic.Object;
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            o.PLID = data["PL_ID"].Str();
            o.PGRPID = data["PGRP_ID"].Str();
            o.Businessname = data["Businessname"].Str();
            return o;
        }

        public static IEnumerable<dynamic> GetBusinessOwnerList(IEnumerable<dynamic> data, int limit = 1000, bool fullinfo = true)
        {
            if (data == null) return null;
            var items = GetBusinessOwner_List(data);
            var count = items.Count();
            //if (count >= limit)
            //{
            //    var o = items.Last();
            //    var filter = (o.NextFilter = Dynamic.Object);
            //    items = items.Take(count - 1).Concat(new[] { o });
            //    filter.NextFilter = o.num_row;
            //}
            return items;
        }
        public static IEnumerable<dynamic> GetBusinessOwner_List(IEnumerable<dynamic> data, bool fullinfo = true)
        {
            if (data == null) return null;
            return data.Select(e => Get_BusinessOwner_List(e));
        }
        public static IDictionary<string, object> Get_BusinessOwner_List(IDictionary<string, object> data, bool fullinfo = true)
        {
            dynamic o = Dynamic.Object;
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            o.PLID = data["PL_ID"].Str();
            o.PGRPID = data["PGRP_ID"].Str();
            o.Businessowner = data["Businessowner"].Str();
            return o;
        }

        public static IEnumerable<dynamic> GetBusinessTypeList(IEnumerable<dynamic> data, int limit = 1000, bool fullinfo = true)
        {
            if (data == null) return null;
            var items = GetBusinessType_List(data);
            var count = items.Count();
            //if (count >= limit)
            //{
            //    var o = items.Last();
            //    var filter = (o.NextFilter = Dynamic.Object);
            //    items = items.Take(count - 1).Concat(new[] { o });
            //    filter.NextFilter = o.num_row;
            //}
            return items;
        }
        public static IEnumerable<dynamic> GetBusinessType_List(IEnumerable<dynamic> data, bool fullinfo = true)
        {
            if (data == null) return null;
            return data.Select(e => Get_BusinessType_List(e));
        }
        public static IDictionary<string, object> Get_BusinessType_List(IDictionary<string, object> data, bool fullinfo = true)
        {
            dynamic o = Dynamic.Object;
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            o.PLID = data["PL_ID"].Str();
            o.PGRPID = data["PGRP_ID"].Str();
            o.BusinessType = data["BIZ_TYP"].Str();
            o.BusinessAddress = data["BIZ_ADDRESS"].Str();
            o.BusinessOwner = data["BIZ_OWNER"].Str();
            o.BusinessOwnerAddress = data["BIZ_OWNER_ADDRESS"].Str();
            return o;
        }

        public static IEnumerable<dynamic> GetAllIssuesConcernList(IEnumerable<dynamic> data, string userid = "", int limit = 100, bool fullinfo = true)
        {
            if (data == null) return null;
            var items = GetAllIssuesConcern_List(data);
            var count = items.Count();
            if (count >= limit)
            {
                var o = items.Last();
                var filter = (o.NextFilter = Dynamic.Object);
                items = items.Take(count - 1).Concat(new[] { o });
                filter.NextFilter = o.num_row;
                filter.Userid = userid;
            }
            return items;
        }
        public static IEnumerable<dynamic> GetAllIssuesConcern_List(IEnumerable<dynamic> data, bool fullinfo = true)
        {
            if (data == null) return null;
            return data.Select(e => Get_AllIssuesConcern_List(e));
        }
        public static IDictionary<string, object> Get_AllIssuesConcern_List(IDictionary<string, object> data, bool fullinfo = true)
        {
            dynamic o = Dynamic.Object;
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            o.num_row = data["Num_Row"].Str();
            o.PL_ID = data["PL_ID"].Str();
            o.PGRP_ID = data["PGRP_ID"].Str();
            o.Userid = data["USR_ID"].Str();

            o.Firstname = textInfo.ToTitleCase(data["FRST_NM"].Str());
            o.Lastname = textInfo.ToTitleCase(data["LST_NM"].Str());
            o.Middlename = textInfo.ToTitleCase(data["MDL_NM"].Str());
            o.AccountName = textInfo.ToTitleCase(data["FLL_NM"].Str());
            o.MobileNumber = data["MOB_NO"].Str();
            o.ProfileImageUrl = data["IMG_URL"].Str();
            o.Sitio = data["LOC_SIT"].Str();
            o.SitioName = textInfo.ToTitleCase(data["SIT_NM"].Str());
            o.GeoLocationLat = data["GEO_LOC_LAT"].Str();
            o.GeoLocationLong = data["GEO_LOC_LONG"].Str();

            o.TransactionNo = data["TRN_NO"].Str();
            o.TicketNo = data["TCKT_NO"].Str();
            o.Subject = data["SBJCT"].Str();
            o.Body = data["BODY"].Str();
            o.Status = data["STAT"].Str();
            o.StatusName = data["STAT_NM"].Str();
            o.CorrectiveAction = data["COR_ACTION"].Str();
            o.DateReceived = (data["RGS_TRN_TS"].Str() == "") ? "" : Convert.ToDateTime(data["RGS_TRN_TS"].Str()).ToString("MMM, dd, yyyy");
            o.ProcessDate = (data["PRCS_TRN_TS"].Str() == "") ? "" : Convert.ToDateTime(data["PRCS_TRN_TS"].Str()).ToString("MMM dd, yyyy");
            o.ActionDate = (data["FXD_TRN_TS"].Str() == "") ? "" : Convert.ToDateTime(data["FXD_TRN_TS"].Str()).ToString("MMM, dd, yyyy");
            o.TotalAttachment = (data["TTL_ATTCHMNT"].Str()=="0") ? "" : data["TTL_ATTCHMNT"].Str();

            o.Attachment = (data["ATTCHMNT"].Str() == "0") ? "" : data["ATTCHMNT"].Str();
            //o.isLeader = Convert.ToBoolean(data["isLeader"].Str());
            return o;
        }


        public static IEnumerable<dynamic> GetAllIssuesConcernList1(IEnumerable<dynamic> data, string userid = "", int limit = 100, bool fullinfo = true)
        {
            if (data == null) return null;
            var items = GetAllIssuesConcern_List1(data);
            var count = items.Count();
            if (count >= limit)
            {
                var o = items.Last();
                var filter = (o.NextFilter = Dynamic.Object);
                items = items.Take(count - 1).Concat(new[] { o });
                filter.NextFilter = o.num_row;
                filter.Userid = userid;
            }
            return items;
        }
        public static IEnumerable<dynamic> GetAllIssuesConcern_List1(IEnumerable<dynamic> data, bool fullinfo = true)
        {
            if (data == null) return null;
            return data.Select(e => Get_AllIssuesConcern_List1(e));
        }
        public static IDictionary<string, object> Get_AllIssuesConcern_List1(IDictionary<string, object> data, bool fullinfo = true)
        {
            dynamic o = Dynamic.Object;
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            o.num_row = data["Num_Row"].Str();
            o.PL_ID = data["PL_ID"].Str();
            o.PGRP_ID = data["PGRP_ID"].Str();
            o.Userid = data["USR_ID"].Str();

            o.Firstname = textInfo.ToTitleCase(data["FRST_NM"].Str());
            o.Lastname = textInfo.ToTitleCase(data["LST_NM"].Str());
            o.Middlename = textInfo.ToTitleCase(data["MDL_NM"].Str());
            o.Fullname = textInfo.ToTitleCase(data["FLL_NM"].Str());
            o.MobileNumber = data["MOB_NO"].Str();
            o.Sitio = data["LOC_SIT"].Str();
            o.SitioName = textInfo.ToTitleCase(data["SIT_NM"].Str());
            o.GeoLocLat = data["GEO_LOC_LAT"].Str();
            o.GeoLocLong = data["GEO_LOC_LONG"].Str();
            o.ImageUrl = data["IMG_URL"].Str();

            o.TransactionNo = data["TRN_NO"].Str();
            o.TicketNo = data["TCKT_NO"].Str();
            o.Subject = data["SBJCT"].Str();
            o.Body = data["BODY"].Str();
            o.STAT = data["STAT"].Str();
            o.CorrectiveAction = data["COR_ACTION"].Str();
            o.IssuedDate = (data["RGS_TRN_TS"].Str() == "") ? "" : Convert.ToDateTime(data["RGS_TRN_TS"].Str()).ToString("MMM, dd, yyyy");
            o.ProcessDate = (data["PRCS_TRN_TS"].Str() == "") ? "" : Convert.ToDateTime(data["PRCS_TRN_TS"].Str()).ToString("MMM dd, yyyy");
            o.ActionDate = (data["FXD_TRN_TS"].Str() == "") ? "" : Convert.ToDateTime(data["FXD_TRN_TS"].Str()).ToString("MMM, dd, yyyy");
            o.TotalAttachment = (data["TTL_ATTCHMNT"].Str() == "0") ? "" : data["TTL_ATTCHMNT"].Str();

            o.Attachment = (data["ATTCHMNT"].Str() == "0") ? "" : data["ATTCHMNT"].Str();
            //o.isLeader = Convert.ToBoolean(data["isLeader"].Str());
            return o;
        }


        public static IEnumerable<dynamic> GetAllMemoList(IEnumerable<dynamic> data, string userid = "", int limit = 100, bool fullinfo = true)
        {
            if (data == null) return null;
            var items = GetAllMemo_List(data);
            var count = items.Count();
            if (count >= limit)
            {
                var o = items.Last();
                var filter = (o.NextFilter = Dynamic.Object);
                items = items.Take(count - 1).Concat(new[] { o });
                filter.NextFilter = o.num_row;
                filter.Userid = userid;
            }
            return items;
        }
        public static IEnumerable<dynamic> GetAllMemo_List(IEnumerable<dynamic> data, bool fullinfo = true)
        {
            if (data == null) return null;
            return data.Select(e => Get_AllMemo_List(e));
        }
        public static IDictionary<string, object> Get_AllMemo_List(IDictionary<string, object> data, bool fullinfo = true)
        {
            dynamic o = Dynamic.Object;
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            o.num_row = data["Num_Row"].Str();
            o.PL_ID = data["PL_ID"].Str();
            o.PGRP_ID = data["PGRP_ID"].Str();
            o.BrgyCode = data["LOC_BRGY"].Str();
            o.Brgy = data["BRGY"].Str();
            o.MemoID = data["MEMO_ID"].Str();
            o.MemorandumNo = data["MEMO_NO"].Str();
            o.Subject = textInfo.ToTitleCase(data["MEMO_SBJCT"].Str());
            o.MemoURL = data["MEMO_PATH"].Str();
            //o.isLeader = Convert.ToBoolean(data["isLeader"].Str());
            return o;
        }


        public static IEnumerable<dynamic> GetOfficialHeaderList(IEnumerable<dynamic> data, int limit = 100, bool fullinfo = true)
        {
            if (data == null) return null;
            var items = GetOfficialHeader_List(data);
            //var count = items.Count();
            //if (count >= limit)
            //{
            //    var o = items.Last();
            //    var filter = (o.NextFilter = Dynamic.Object);
            //    items = items.Take(count - 1).Concat(new[] { o });
            //    filter.NextFilter = o.num_row;
            //}
            return items;
        }
        public static IEnumerable<dynamic> GetOfficialHeader_List(IEnumerable<dynamic> data, bool fullinfo = true)
        {
            if (data == null) return null;
            return data.Select(e => Get_OfficialHeader_List(e));
        }
        public static IDictionary<string, object> Get_OfficialHeader_List(IDictionary<string, object> data, bool fullinfo = true)
        {
            dynamic o = Dynamic.Object;
            o.PL_ID = data["PL_ID"].Str();
            o.PGRP_ID = data["PGRP_ID"].Str();
            o.BrgyOfficialLogo = data["BRGY_LOGO"].Str();
            o.MunicipalLogo = data["MUNICIPAL_LOGO"].Str();
            o.Country = data["COUNTRYHEADER"].Str();
            o.Province = data["PROVINCEHEADER"].Str();
            o.Municipality = data["MUNICIPALHEADER"].Str();
            o.Barangay = data["BARANGAYHEADER"].Str();
            o.IssuedLocation = data["ISSUED_LOCATION"].Str();
            o.ContactNo = data["CNTCTNO"].Str();
            o.DefaultValidity = Convert.ToInt32(data["DF_VAL"].Str());
            o.MonthValidity = data["MOS_VAL"].Str();
            return o;
        }


        public static IEnumerable<dynamic> GetAllDocTypeList(IEnumerable<dynamic> data, string userid = "", int limit = 100, bool fullinfo = true)
        {
            if (data == null) return null;
            var items = GetAllDocType_List(data);
            var count = items.Count();
            if (count >= limit)
            {
                var o = items.Last();
                var filter = (o.NextFilter = Dynamic.Object);
                items = items.Take(count - 1).Concat(new[] { o });
                filter.NextFilter = o.num_row;
                filter.Userid = userid;
            }
            return items;
        }
        public static IEnumerable<dynamic> GetAllDocType_List(IEnumerable<dynamic> data, bool fullinfo = true)
        {
            if (data == null) return null;
            return data.Select(e => Get_AllDocType_List(e));
        }
        public static IDictionary<string, object> Get_AllDocType_List(IDictionary<string, object> data, bool fullinfo = true)
        {
            dynamic o = Dynamic.Object;
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            o.num_row = data["Num_Row"].Str();
            o.PL_ID = data["PL_ID"].Str();
            o.PGRP_ID = data["PGRP_ID"].Str();
            o.BrgyCode = data["LOC_BRGY"].Str();
            o.Brgy = data["BRGY"].Str();
            o.DocTypeID = data["DOC_TYP_ID"].Str();
            o.DocTypeNM = data["DOC_TYPE"].Str();
            o.Category = data["CATEGORY"].Str();
            o.CategoryNM = data["CATEGORY_NM"].Str();
            o.DocContent = data["DOC_CONTENT"].Str();
            //o.isLeader = Convert.ToBoolean(data["isLeader"].Str());
            return o;
        }

        public static IEnumerable<dynamic> GetAllBrgyOfficialList(IEnumerable<dynamic> data, string userid = "", int limit = 100, bool fullinfo = true)
        {
            if (data == null) return null;
            var items = GetAllBrgyOfficial_List(data);
            var count = items.Count();
            if (count >= limit)
            {
                var o = items.Last();
                var filter = (o.NextFilter = Dynamic.Object);
                items = items.Take(count - 1).Concat(new[] { o });
                filter.NextFilter = o.num_row;
                filter.Userid = userid;
            }
            return items;
        }
        public static IEnumerable<dynamic> GetAllBrgyOfficial_List(IEnumerable<dynamic> data, bool fullinfo = true)
        {
            if (data == null) return null;
            return data.Select(e => Get_AllBrgyOfficial_List(e));
        }
        public static IDictionary<string, object> Get_AllBrgyOfficial_List(IDictionary<string, object> data, bool fullinfo = true)
        {
            dynamic o = Dynamic.Object;
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            o.num_row = data["Num_Row"].Str();
            o.BrgyOfficialID = data["BRGY_OFL_ID"].Str();
            o.PL_ID = data["PL_ID"].Str();
            o.PGRP_ID = data["PGRP_ID"].Str();
            o.BrgyCode = data["LOC_BRGY"].Str();
            o.Brgy = data["BRGY"].Str();
            o.SitioCode = data["LOC_SIT"].Str();
            o.Sitio = data["SIT_NM"].Str();
            o.BarangayPositionID = data["BRGY_PSTN_ID"].Str();
            o.BarangayPosition = data["POSITION"].Str();
            o.Category = data["CATEGORY"].Str();
            o.Sub_Category = data["SUB_CAT"].Str();
            o.ElectedOfficial = data["OFCL_NM"].Str();
            o.ResidentName = data["FLL_NM"].Str();
            o.Userid = data["USR_ID"].Str();
            o.RankNo = data["RNK_NO"].Str();
            o.Committee = data["CMTE"].Str();
            o.TermStart = (data["EF_DATE"].Str().IsEmpty()) ? "" : Convert.ToDateTime(data["EF_DATE"].Str()).ToString("MMM dd, yyyy");
            o.TermEnd = (data["EOT_DATE"].Str().IsEmpty()) ? "" : Convert.ToDateTime(data["EOT_DATE"].Str()).ToString("MMM dd, yyyy");
            o.SignatureURL = data["SIGNATUREID"].Str();
            //o.isLeader = Convert.ToBoolean(data["isLeader"].Str());
            return o;
        }

        public static IEnumerable<dynamic> GetAllBarangaySignatoryList(IEnumerable<dynamic> data, string userid = "", int limit = 100, bool fullinfo = true)
        {
            if (data == null) return null;
            var items = GetAllBarangaySignatory_List(data);
            var count = items.Count();
            //if (count >= limit)
            //{
            //    var o = items.Last();
            //    var filter = (o.NextFilter = Dynamic.Object);
            //    items = items.Take(count - 1).Concat(new[] { o });
            //    filter.NextFilter = o.num_row;
            //    filter.Userid = userid;
            //}
            return items;
        }
        public static IEnumerable<dynamic> GetAllBarangaySignatory_List(IEnumerable<dynamic> data, bool fullinfo = true)
        {
            if (data == null) return null;
            return data.Select(e => Get_AllBarangaySignatory_List(e));
        }
        public static IDictionary<string, object> Get_AllBarangaySignatory_List(IDictionary<string, object> data, bool fullinfo = true)
        {
            dynamic o = Dynamic.Object;
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            o.Position_ID = data["ID"].Str();
            o.PL_ID = data["PL_ID"].Str();
            o.PGRP_ID = data["PGRP_ID"].Str();
            o.Userid = data["USR_ID"].Str();
            o.Brgy_Official = data["BRGY_OFFICIAL"].Str();
            o.Committee = data["CMTE"].Str();
            o.Position = data["POSITION"].Str();
            o.Tagline = data["TAGLINE"].Str();
            o.Tagline_Sig = data["TAGLINE_SIG"].Str();
            o.Brgy_Official_Sig = data["SIGNATURE_URL"].Str();
            return o;
        }

        public static IEnumerable<dynamic> GetAllRequestDocumentList(IEnumerable<dynamic> data, string userid = "", int limit = 100, bool fullinfo = true)
        {
            if (data == null) return null;
            var items = GetAllRequestDocument_List(data);
            var count = items.Count();
            //if (count >= limit)
            //{
            //    var o = items.Last();
            //    var filter = (o.NextFilter = Dynamic.Object);
            //    items = items.Take(count - 1).Concat(new[] { o });
            //    filter.NextFilter = o.num_row;
            //    filter.Userid = userid;
            //}
            return items;
        }
        public static IEnumerable<dynamic> GetAllRequestDocument_List(IEnumerable<dynamic> data, bool fullinfo = true)
        {
            if (data == null) return null;
            return data.Select(e => Get_AllRequestDocument_List(e));
        }
        public static IDictionary<string, object> Get_AllRequestDocument_List(IDictionary<string, object> data, bool fullinfo = true)
        {
            dynamic o = Dynamic.Object;
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            o.num_row = data["Num_Row"].Str();
            o.PL_ID = data["PL_ID"].Str();
            o.PGRP_ID = data["PGRP_ID"].Str();
            o.ReqDocID = data["REQ_DOC_ID"].Str();
            o.DoctypeID = data["DOCTYP_ID"].Str();
            o.DoctypeNM = data["DOCTYP_NM"].Str();
            o.CategoryID = data["CATEGORY"].Str();
            o.Category_Doc_ID = data["CAT_DOC"].Str();
            o.Category_Document = data["CAT_DOC_NM"].Str();
            o.ApplicationDate = (data["APP_DATE"].Str()=="") ? "" : Convert.ToDateTime(data["APP_DATE"].Str()).ToString("MMM dd, yyyy");
            o.BusinessName = data["BIZ_NM"].Str();
            o.BusinessAddress = data["BIZ_ADDRESS"].Str();
            o.BusinessOwnerName = data["BIZ_OWNER"].Str();
            o.BusinessOwnerAddress = data["BIZ_OWNER_ADDRESS"].Str();
            o.Type = data["BIZ_TYP"].Str();
            o.RequestorID = data["REQTR_ID"].Str();
            o.RequestorNM = data["REQTR_NM"].Str();
            o.STATUS = data["STATUS"].Str();
            o.STATUS_NM = data["STATUS_NM"].Str();
            o.CategoryID = data["CATEGORY"].Str();
            o.CategoryNM = data["CATEGORY_NM"].Str();
            o.Purpose = data["PURPOSE"].Str();
            o.ORNO = data["OR_NO"].Str();
            o.CTCNo = data["CTC_NO"].Str();
            o.IssuedDate = (data["DATE_ISSUED"].Str()=="") ? "" : Convert.ToDateTime(data["DATE_ISSUED"].Str()).ToString("MMM dd, yyyy");
            o.Amount = data["AMOUNT"].Str();
            o.BrgyCaptain = data["BRGY_CAPTAIN"].Str();
            o.BrgyCaptainSignature = data["BRGY_CAPTAIN_SIGNATURE"].Str();
            o.URL_DocPath = data["URL_DOCPATH"].Str();
            o.ControlNo = data["CNTRL_NO"].Str();
            o.Gender = data["GNDR"].Str();
            o.Gender_NM = data["GNDR_NM"].Str();
            o.MaritalStatus = data["MRTL_STAT"].Str();
            o.MaritalStatus_NM = data["MRTL_STAT_NM"].Str();
            o.Birthdate = data["BRT_DT"].Str();
            o.PurokSitio = data["SIT_NM"].Str();
            o.ProfilePicture = data["PRF_PIC"].Str();
            o.PresentAddress = textInfo.ToUpper(data["PRSNT_ADDR"].Str());
            o.URLAttachment = data["URL_ATTACHMENT"].Str();
            o.OTR_Document_ID = data["OTR_DOC_ID"].Str();
            o.OTR_Document_Type = data["OTR_DOC_TYPE"].Str();
            o.OTRDocType = data["DOC_TYPE"].Str();
            o.OTRDocContent = data["DOC_Content"].Str();
            o.OTRCategory = data["OTR_CATEGORY"].Str();
            o.OTRCategoryNM = data["OTR_CATEGORY_NM"].Str();
            o.isFree = data["isFREE"].Str();
            o.isFreeNM = data["isFREE_NM"].Str();
            o.AppointmentDate = (data["APPT_DATE"].Str() == "") ? "" : Convert.ToDateTime(data["APPT_DATE"].Str()).ToString("MMM dd, yyyy");

            //o.isLeader = Convert.ToBoolean(data["isLeader"].Str());
            return o;
        }


        public static IEnumerable<dynamic> GetAllEmergencyTpeList(IEnumerable<dynamic> data, string userid = "", int limit = 100, bool fullinfo = true)
        {
            if (data == null) return null;
            var items = GetAllEmergencyTpe_List(data);
            var count = items.Count();
            //if (count >= limit)
            //{
            //    var o = items.Last();
            //    var filter = (o.NextFilter = Dynamic.Object);
            //    items = items.Take(count - 1).Concat(new[] { o });
            //    filter.NextFilter = o.num_row;
            //    filter.Userid = userid;
            //}
            return items;
        }
        public static IEnumerable<dynamic> GetAllEmergencyTpe_List(IEnumerable<dynamic> data, bool fullinfo = true)
        {
            if (data == null) return null;
            return data.Select(e => Get_AllEmergencyTpe_List(e));
        }
        public static IDictionary<string, object> Get_AllEmergencyTpe_List(IDictionary<string, object> data, bool fullinfo = true)
        {
            dynamic o = Dynamic.Object;
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            o.num_row = data["Num_Row"].Str();
            o.PL_ID = data["PL_ID"].Str();
            o.PGRP_ID = data["PGRP_ID"].Str();
            o.EmgyTypID = data["EMGY_TYP_ID"].Str();
            o.EmgyType = data["EMERGENCY_TYPE"].Str();
            o.Message = data["TEXT_MESSAGE"].Str();

            //o.isLeader = Convert.ToBoolean(data["isLeader"].Str());
            return o;
        }


        public static IEnumerable<dynamic> GetAllCommunityList(IEnumerable<dynamic> data, string userid = "", int limit = 100, bool fullinfo = true)
        {
            if (data == null) return null;
            var items = GetAllCommunity_List(data);
            var count = items.Count();
            //if (count >= limit)
            //{
            //    var o = items.Last();
            //    var filter = (o.NextFilter = Dynamic.Object);
            //    items = items.Take(count - 1).Concat(new[] { o });
            //    filter.NextFilter = o.num_row;
            //    filter.Userid = userid;
            //}
            return items;
        }
        public static IEnumerable<dynamic> GetAllCommunity_List(IEnumerable<dynamic> data, bool fullinfo = true)
        {
            if (data == null) return null;
            return data.Select(e => Get_AllCommunity_List(e));
        }
        public static IDictionary<string, object> Get_AllCommunity_List(IDictionary<string, object> data, bool fullinfo = true)
        {
            dynamic o = Dynamic.Object;
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            o.num_row = data["Num_Row"].Str();
            o.Subscriber = data["PLTCL_NM"].Str();
            o.PL_ID = data["PL_ID"].Str();
            o.PGRP_ID = data["PGRP_ID"].Str();
            o.CommunityID = data["COMM_ID"].Str();
            o.CommunityName = data["COMM_NM"].Str();
            o.CommunityDescription = data["COMM_DESC"].Str();
            o.TypeLevel = data["TYP_LVL"].Str();
            o.ScopeLevelDescription = data["SCOPE_LVL_DESCR"].Str();
            o.isStatus = Convert.ToBoolean(data["isSTATUS"]);
            o.isActive = Convert.ToBoolean(data["isActive"]);
            o.RequestStatus = Convert.ToBoolean(data["REQ_STATUS"]);
            o.RequestDate = data["RequestDate"].Str();
            o.Requestor = data["FLL_NM"].Str();
            o.ApprovedDate = data["APPROVED_DT"].Str();
            o.RegionCode = data["LOC_REG"].Str();
            o.ProvinceCode = data["LOC_PROV"].Str();
            o.MunicipalityCode = data["LOC_MUN"].Str();
            o.BarangayCode = data["LOC_BRGY"].Str();
            o.SitioCode = data["LOC_SIT"].Str();
            if (data["PostCount"].Str() == "0")
                o.PostCount = "";
            else
            {
                o.PostCount = (data["PostCount"].Str() == "1") ? data["PostCount"].Str() + " post" : data["PostCount"].Str() + " posts";
            }
            
            //o.isLeader = Convert.ToBoolean(data["isLeader"].Str());
            return o;
        }

        public static IEnumerable<dynamic> GetAllPostCommunityList(IEnumerable<dynamic> data, string userid = "", int limit = 100, bool fullinfo = true)
        {
            if (data == null) return null;
            var items = GetAllPostCommunity_List(data);
            var count = items.Count();
            //if (count >= limit)
            //{
            //    var o = items.Last();
            //    var filter = (o.NextFilter = Dynamic.Object);
            //    items = items.Take(count - 1).Concat(new[] { o });
            //    filter.NextFilter = o.num_row;
            //    filter.Userid = userid;
            //}
            return items;
        }
        public static IEnumerable<dynamic> GetAllPostCommunity_List(IEnumerable<dynamic> data, bool fullinfo = true)
        {
            if (data == null) return null;
            return data.Select(e => Get_AllPostCommunity_List(e));
        }
        public static IDictionary<string, object> Get_AllPostCommunity_List(IDictionary<string, object> data, bool fullinfo = true)
        {
            dynamic o = Dynamic.Object;
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            o.num_row = data["Num_Row"].Str();
            o.PL_ID = data["PL_ID"].Str();
            o.PGRP_ID = data["PGRP_ID"].Str();
            o.CommunityID = data["COMM_ID"].Str();
            o.CommunityName = data["COMM_NM"].Str();
            o.CommunityDescription = data["COMM_DESC"].Str();
            o.PostID = data["POST_ID"].Str();
            o.PostTitle = data["POST_TTL"].Str();
            o.PostDescription = data["POST_DESC"].Str();
            o.URL = data["IMG_CONTENT"].Str();
            o.isInActive = Convert.ToBoolean(data["isInActive"]);
            o.CreatedDate = data["CreatedDate"].Str();
            o.TypeLevel = data["TYP_LVL"].Str();
            o.TypeLevelDescription = data["Typ_Level_Desc"].Str();
            o.Total_Comment = (data["Total_Comment"].Str() == "0") ? "" : (Convert.ToInt32(data["Total_Comment"]) > 1) ? data["Total_Comment"].Str() + " " + "Comment" : data["Total_Comment"].Str() + " " + "Comments";
            o.Total_Like = (Convert.ToInt32(data["Total_Like"]) == 0) ? "" : data["Total_Like"].Str();
            o.Total_disLike = (Convert.ToInt32(data["Total_disLike"]) == 0) ? "" : data["Total_disLike"].Str();
            return o;
        }


        public static IEnumerable<dynamic> GetAllCommentPostCommunityList(IEnumerable<dynamic> data, string userid = "", int limit = 100, bool fullinfo = true)
        {
            if (data == null) return null;
            var items = GetAllCommentPostCommunity_List(data);
            var count = items.Count();
            //if (count >= limit)
            //{
            //    var o = items.Last();
            //    var filter = (o.NextFilter = Dynamic.Object);
            //    items = items.Take(count - 1).Concat(new[] { o });
            //    filter.NextFilter = o.num_row;
            //    filter.Userid = userid;
            //}
            return items;
        }
        public static IEnumerable<dynamic> GetAllCommentPostCommunity_List(IEnumerable<dynamic> data, bool fullinfo = true)
        {
            if (data == null) return null;
            return data.Select(e => Get_AllCommentPostCommunity_List(e));
        }
        public static IDictionary<string, object> Get_AllCommentPostCommunity_List(IDictionary<string, object> data, bool fullinfo = true)
        {
            dynamic o = Dynamic.Object;
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            o.num_row = data["Num_Row"].Str();
            o.CommenterName = data["CommenterName"].Str();
            o.CommenterMobileNumber = data["CommenterMobileNumber"].Str();
            o.CommenterImage = data["CommenterImage"].Str();
            o.CommunityID = data["COMM_ID"].Str();
            o.PostID = data["POST_ID"].Str();
            o.USR_ID = data["USR_ID"].Str();
            o.CommentID = data["CMN_ID"].Str();
            o.CommentLevel = data["COMN_LVL"].Str();
            o.CommentDescription = data["CMN_DESCRIPTION"].Str();
            o.Total_Like = (Convert.ToInt32(data["Total_Like"]) > 0) ? data["Total_Like"].Str(): "";
            o.Total_disLike = (Convert.ToInt32(data["Total_disLike"]) > 0) ? data["Total_disLike"].Str() : "";
            o.Post_Date = data["Post_Date"].Str();
            o.Post_Time = Get_Time(Convert.ToDateTime(data["Post_Date"]));
            o.Reason = data["Reason"].Str();
            o.isRemove = Convert.ToBoolean(data["isREMOVE"]);
            return o;
        }

        public static string Get_Time(DateTime Post_Date)
        {
            //string strtime = string.Empty;
            var milliseconds = DateTime.Now.Subtract(Post_Date);
            var d = milliseconds.Days;
            var h = milliseconds.Hours;
            var mi = milliseconds.Minutes;
            var ss = milliseconds.Seconds;
            var mls = milliseconds.Milliseconds;
            if(d == 0 && h == 0 && mi == 0 && ss > 0)
            {
                if (ss == 1)
                    return ss + " second ago";
                return ss + " seconds ago";
            }
            else if(d == 0 && h == 0 && mi < 60)
            {
                if (mi == 1)
                    return mi + " minute ago";
                return mi + " minutes ago";
            }
            else if(d == 0 && h < 24)
            {
                if (h == 1)
                    return h + " hour ago";
                return h + " hours ago";
            }
            else if (d >= 1) //days
            {
                if (d == 1)
                    return d + " day ago";
                else if(d > 1 && d < 365)
                {
                    if (d < 30)
                        return d + " days ago";
                    var dd = d / 30;
                    if(dd == 1)
                    {
                        return "1 month ago";
                    }
                    else if(dd > 1 && dd <= 12)
                    {
                        return dd + " months ago";
                    }
                }
                else
                {
                    var dd = d / 365;
                    if (dd == 1)
                        return "1 year ago";
                    else
                        return dd + " years ago";
                }
            }
            return "1 second ago";

            /*
            //var dtime = milliseconds;
            decimal dtime = Convert.ToDecimal(milliseconds.Ticks);
            //double today_date = DateTime.Now.ToOADate();
            //double d_postdate = Post_Date.ToOADate();
            //double dtime = double.Parse(milliseconds);
            //double dtime = today_date - d_postdate;
            if(Math.Floor(dtime / 1000) < 60) //second
            {
                if(Math.Floor(dtime / 1000) == 1)
                {
                    strtime = "1 second ago";
                    return strtime;
                }
                strtime = (Math.Floor(dtime / 1000)).Str() + " seconds ago";
                return strtime;
            }
            else if (Math.Floor(dtime / 1000 / 60) < 60) //minute
            {
                if (Math.Floor(dtime / 1000 / 60) == 1)
                {
                    strtime = "1 minute ago";
                    return strtime;
                }
                strtime = (Math.Floor(dtime / 1000 / 60)).Str() + " minutes ago";
                return strtime;
            }
            else if (Math.Floor(dtime / 1000 / 60 / 60) < 24) //hours
            {
                if (Math.Floor(dtime / 1000 / 60 / 60) == 1)
                {
                    strtime = "1 hour ago";
                    return strtime;
                }
                strtime = (Math.Floor(dtime / 1000 / 60 / 60)).Str() + " hours ago";
                return strtime;
            }
            else if (Math.Floor(dtime / 1000 / 60 / 60 /24) > 1 || Math.Floor(dtime / 1000 / 60 / 60 / 24) < 30) //days
            {
                if (Math.Floor(dtime / 1000 / 60 / 60 /24) == 1)
                {
                    strtime = "1 day ago";
                    return strtime;
                }
                strtime = (Math.Floor(dtime / 1000 / 60 / 60 / 24)).Str() + " days ago";
                return strtime;
            }
            else if (Math.Floor(dtime / 1000 / 60 / 60 / 24 / 30) < 12) //month
            {
                if (Math.Floor(dtime / 1000 / 60 / 60 / 24 / 30) == 1)
                {
                    strtime = "1 month ago";
                    return strtime;
                }
                strtime = (Math.Floor(dtime / 1000 / 60 / 60 / 24 /30)).Str() + " months ago";
                return strtime;
            }
            else //year
            {
                if (Math.Floor(dtime / 1000 / 60 / 60 / 24 / 30 /12) == 1)
                {
                    strtime = "1 year ago";
                    return strtime;
                }
                strtime = (Math.Floor(dtime / 1000 / 60 / 60 / 24 / 30 /12)).Str() + " years ago";
                return strtime;
            }
            */
        }

        public static IEnumerable<dynamic> GetAllEmergencyContactPersonList(IEnumerable<dynamic> data, string userid = "", int limit = 100, bool fullinfo = true)
        {
            if (data == null) return null;
            var items = GetAllEmergencyContactPerson_List(data);
            var count = items.Count();
            //if (count >= limit)
            //{
            //    var o = items.Last();
            //    var filter = (o.NextFilter = Dynamic.Object);
            //    items = items.Take(count - 1).Concat(new[] { o });
            //    filter.NextFilter = o.num_row;
            //    filter.Userid = userid;
            //}
            return items;
        }
        public static IEnumerable<dynamic> GetAllEmergencyContactPerson_List(IEnumerable<dynamic> data, bool fullinfo = true)
        {
            if (data == null) return null;
            return data.Select(e => Get_AllEmergencyContactPerson_List(e));
        }
        public static IDictionary<string, object> Get_AllEmergencyContactPerson_List(IDictionary<string, object> data, bool fullinfo = true)
        {
            dynamic o = Dynamic.Object;
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            o.num_row = data["Num_Row"].Str();
            o.PL_ID = data["PL_ID"].Str();
            o.PGRP_ID = data["PGRP_ID"].Str();
            o.EmgyContactID = data["EMGY_CNTCT_ID"].Str();
            o.EmgyTypID = data["EMGY_TYP_ID"].Str();
            o.EmgyType = data["EMERGENCY_TYPE"].Str();
            o.Message = data["TEXT_MESSAGE"].Str();
            o.ContactPerson = data["CONTACT_PERSON"].Str();
            o.MobileNumber = data["MOB_NUMBER"].Str();
            o.Status = Convert.ToBoolean(data["STATUS"]);
            o.StatusName = data["STATUS_NM"].Str();
            return o;
        }


        public static IEnumerable<dynamic> GetAllEmergencyAlertRequestList(IEnumerable<dynamic> data, string userid = "", int limit = 100, bool fullinfo = true)
        {
            if (data == null) return null;
            var items = GetAllEmergencyAlertRequest_List(data);
            var count = items.Count();
            //if (count >= limit)
            //{
            //    var o = items.Last();
            //    var filter = (o.NextFilter = Dynamic.Object);
            //    items = items.Take(count - 1).Concat(new[] { o });
            //    filter.NextFilter = o.num_row;
            //    filter.Userid = userid;
            //}
            return items;
        }
        public static IEnumerable<dynamic> GetAllEmergencyAlertRequest_List(IEnumerable<dynamic> data, bool fullinfo = true)
        {
            if (data == null) return null;
            return data.Select(e => Get_AllEmergencyAlertRequest_List(e));
        }
        public static IDictionary<string, object> Get_AllEmergencyAlertRequest_List(IDictionary<string, object> data, bool fullinfo = true)
        {
            dynamic o = Dynamic.Object;
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            o.Num_Row = data["Num_Row"].Str();
            o.PL_ID = data["PL_ID"].Str();
            o.PGRP_ID = data["PGRP_ID"].Str();
            o.UserID = data["USR_ID"].Str();
            o.AccountName = data["FLL_NM"].Str();
            o.ProfileImageUrl = data["IMG_URL"].Str();
            o.MobileNo = data["MOB_NO"].Str();
            o.EmergencyID = data["EMGY_ID"].Str();
            o.EmergencyTypeID = data["EMGY_TYP_ID"].Str();
            o.EmergencyTypeNM = data["EMERGENCY_TYPE"].Str();
            o.GeoLocationLat = data["GEO_LOC_LAT"].Str();
            o.GeoLocationLong = data["GEO_LOC_LONG"].Str();
            o.DateReceived = Convert.ToDateTime(data["RGS_TRN_TS"].Str()).ToString("MMM dd, yyyy");
            o.Count = data["CNT_REQUEST"].Str();
            return o;
        }


        public static IEnumerable<dynamic> GetAllEmergencyAlertList(IEnumerable<dynamic> data, string userid = "", int limit = 100, bool fullinfo = true)
        {
            if (data == null) return null;
            var items = GetAllEmergencyAlert_List(data);
            var count = items.Count();
            //if (count >= limit)
            //{
            //    var o = items.Last();
            //    var filter = (o.NextFilter = Dynamic.Object);
            //    items = items.Take(count - 1).Concat(new[] { o });
            //    filter.NextFilter = o.num_row;
            //    filter.Userid = userid;
            //}
            return items;
        }
        public static IEnumerable<dynamic> GetAllEmergencyAlert_List(IEnumerable<dynamic> data, bool fullinfo = true)
        {
            if (data == null) return null;
            return data.Select(e => Get_AllEmergencyAlert_List(e));
        }
        public static IDictionary<string, object> Get_AllEmergencyAlert_List(IDictionary<string, object> data, bool fullinfo = true)
        {
            dynamic o = Dynamic.Object;
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            o.num_row = data["Num_Row"].Str();
            o.PL_ID = data["PL_ID"].Str();
            o.PGRP_ID = data["PGRP_ID"].Str();
            o.EmgyID = data["EMGY_ID"].Str();
            o.EmgyTypID = data["EMGY_TYP_ID"].Str();
            o.EmgyType = data["EMERGENCY_TYPE"].Str();
            o.Message = data["TEXT_MESSAGE"].Str();
            o.UserID = data["USR_ID"].Str();
            o.FUllName = data["FLL_NM"].Str();
            o.ProfileImageUrl = data["IMG_URL"].Str();
            o.MobileNo = data["MOB_NO"].Str();
            o.AlertDate = data["ALRT_DATE"].Str();
            o.GeolocationLat = data["GEO_LOC_LAT"].Str();
            o.GeolocationLong = data["GEO_LOC_LONG"].Str();
            o.Closed_Details = data["CLOSED_DETAILS"].Str();
            o.ClosedDate = data["CLOSED_DATE"].Str();
            o.Closed_Type = Convert.ToInt32(data["CLOSED_TYP"]);
            o.Closed_TypeName = data["CLOSED_TYP_NM"].Str();
            return o;
        }

        public static IEnumerable<dynamic> GetAllRegisterBusinessList(IEnumerable<dynamic> data, string userid = "", int limit = 100, bool fullinfo = true)
        {
            if (data == null) return null;
            var items = GetAllRegisterBusiness_List(data);
            var count = items.Count();
            //if (count >= limit)
            //{
            //    var o = items.Last();
            //    var filter = (o.NextFilter = Dynamic.Object);
            //    items = items.Take(count - 1).Concat(new[] { o });
            //    filter.NextFilter = o.num_row;
            //    filter.Userid = userid;
            //}
            return items;
        }
        public static IEnumerable<dynamic> GetAllRegisterBusiness_List(IEnumerable<dynamic> data, bool fullinfo = true)
        {
            if (data == null) return null;
            return data.Select(e => Get_AllRegisterBusiness_List(e));
        }
        public static IDictionary<string, object> Get_AllRegisterBusiness_List(IDictionary<string, object> data, bool fullinfo = true)
        {
            dynamic o = Dynamic.Object;
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            o.num_row = data["Num_Row"].Str();
            o.PL_ID = data["PL_ID"].Str();
            o.PGRP_ID = data["PGRP_ID"].Str();
            o.BussinessID = data["BIZ_ID"].Str();
            //o.Category = data["CATEGORY"].Str();
            o.BusinessControlNo = data["CTRL_NO"].Str();
            //o.CTCNo = data["CTC_NO"].Str();
            //o.InitialCapital = data["INIT_CAP"].Str();
            o.RegisteredNo = textInfo.ToUpper(textInfo.ToLower(data["REG_NO"].Str()));
            o.NatureofBusiness = textInfo.ToUpper(textInfo.ToLower(data["NTREBIZ"].Str()));
            o.BusinessName = textInfo.ToUpper(textInfo.ToLower(data["BIZ_NM"].Str()));
            o.BusinessEmail = data["BIZ_EMAIL"].Str();
            o.BusinessContactNo = data["CT_NO"].Str();
            o.BusinessAddress = textInfo.ToUpper(textInfo.ToLower(data["BIZ_ADDR"].Str()));
            o.BusinessDateOperate = data["DATE_OPRT"].Str();
            o.BusinessOwnershipTypeID = data["OWNRSHP_TYP"].Str();
            o.BusinessOwnershipTypeNM = textInfo.ToUpper(textInfo.ToLower(data["OWNRSHP_TYP_NM"].Str()));
            o.isInActive = Convert.ToBoolean(data["nSTATUS"].Str());
            o.Owner_ID = data["OWN_ID"].Str();
            o.Owner_Title = data["OWNR_TTL"].Str();
            o.Owner_NM = textInfo.ToUpper(textInfo.ToLower(data["OWNR_NM"].Str()));
            o.OwnerAddres = textInfo.ToUpper(textInfo.ToLower(data["OWNR_ADDRESS"].Str()));
            o.Owner_ContactNo = data["MOB_NO"].Str();
            o.Owner_Email = data["EML_ADD"].Str();

            //o.FirstName = data["OWN_FRST_NM"].Str();
            //o.MiddleInitial = data["OWN_MI_NM"].Str();
            //o.LastName = data["OWN_LST_NM"].Str();
            //o.FullName = data["OWN_FLL_NM"].Str();
            //o.OwnerAddress = data["OWN_ADDR"].Str();
            //o.OwnerEmail = data["OWN_EMAIL"].Str();
            //o.OwnerContactNo = data["OWN_CT_NO"].Str();
            o.Request = (data["REQ_DOC"].Str()=="") ? 0 : Convert.ToInt32(data["REQ_DOC"].Str());

            //o.isLeader = Convert.ToBoolean(data["isLeader"].Str());
            return o;
        }


        public static IEnumerable<dynamic> GetAllOrganizationList(IEnumerable<dynamic> data, bool fullinfo = true)
        {
            if (data == null) return null;
            var items = GetAllOrganization_List(data);
            var count = items.Count();
            //if (count >= limit)
            //{
            //    var o = items.Last();
            //    var filter = (o.NextFilter = Dynamic.Object);
            //    items = items.Take(count - 1).Concat(new[] { o });
            //    filter.NextFilter = o.num_row;
            //    filter.Userid = userid;
            //}
            return items;
        }
        public static IEnumerable<dynamic> GetAllOrganization_List(IEnumerable<dynamic> data, bool fullinfo = true)
        {
            if (data == null) return null;
            return data.Select(e => Get_AllOrganization_List(e));
        }
        public static IDictionary<string, object> Get_AllOrganization_List(IDictionary<string, object> data, bool fullinfo = true)
        {
            dynamic o = Dynamic.Object;
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            o.PL_ID = data["PL_ID"].Str();
            o.PGRP_ID = data["PGRP_ID"].Str();
            o.OrganizationID = data["ORG_ID"].Str();
            o.OrganizationNM = data["ORG_NM"].Str();
            o.OrganizatioAbbr = data["ORG_ABBR"].Str();
            o.Estabalished = data["ORG_EST"].Str();

            //o.isLeader = Convert.ToBoolean(data["isLeader"].Str());
            return o;
        }

        public static IEnumerable<dynamic> GetAllChatSenderList(IEnumerable<dynamic> data, bool fullinfo = true)
        {
            if (data == null) return null;
            var items = GetAllChatSender_List(data);
            var count = items.Count();
            //if (count >= limit)
            //{
            //    var o = items.Last();
            //    var filter = (o.NextFilter = Dynamic.Object);
            //    items = items.Take(count - 1).Concat(new[] { o });
            //    filter.NextFilter = o.num_row;
            //    filter.Userid = userid;
            //}
            return items;
        }
        public static IEnumerable<dynamic> GetAllChatSender_List(IEnumerable<dynamic> data, bool fullinfo = true)
        {
            if (data == null) return null;
            return data.Select(e => Get_AllChatSender_List(e));
        }
        public static IDictionary<string, object> Get_AllChatSender_List(IDictionary<string, object> data, bool fullinfo = true)
        {
            dynamic o = Dynamic.Object;
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            o.num_row = data["Num_Row"].Str();
            o.PL_ID = data["PL_ID"].Str();
            o.PGRP_ID = data["PGRP_ID"].Str();
            o.ChatID = data["ID"].Str();
            o.ChatKey = data["CHT_CD"].Str();
            o.IsPersonal = data["S_PRSNL"].Str();
            o.IsGroup = data["S_GRP"].Str();
            o.IsPublic = data["S_PBLC"].Str();
            o.IsAllowInvatiation = data["S_ALLW_INVT"].Str();
            o.SenderID = data["MMBR_ID"].Str();
            o.DisplayName = data["FLL_NM"].Str();
            o.ProfileImageUrl = data["IMG_URL"].Str();
            //o.MobileNo = data["MOB_NO"].Str();
            o.DateSend = data["RGS_TRN_TS"].Str();
            o.Count = (data["Unread"].Str() == "0") ? "" :  data["Unread"].Str();

            //o.isLeader = Convert.ToBoolean(data["isLeader"].Str());
            return o;
        }

        public static IEnumerable<dynamic> GetAllEstablishmentList(IEnumerable<dynamic> data, bool fullinfo = true)
        {
            if (data == null) return null;
            var items = GetAllEstablishment_List(data);
            var count = items.Count();
            //if (count >= limit)
            //{
            //    var o = items.Last();
            //    var filter = (o.NextFilter = Dynamic.Object);
            //    items = items.Take(count - 1).Concat(new[] { o });
            //    filter.NextFilter = o.num_row;
            //    filter.Userid = userid;
            //}
            return items;
        }
        public static IEnumerable<dynamic> GetAllEstablishment_List(IEnumerable<dynamic> data, bool fullinfo = true)
        {
            if (data == null) return null;
            return data.Select(e => Get_AllEstablishment_List(e));
        }
        public static IDictionary<string, object> Get_AllEstablishment_List(IDictionary<string, object> data, bool fullinfo = true)
        {
            dynamic o = Dynamic.Object;
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            o.num_row = data["Num_Row"].Str();
            o.PL_ID = data["PL_ID"].Str();
            o.PGRP_ID = data["PGRP_ID"].Str();
            o.Est_ID = data["ESTID"].Str();
            o.Est_Name = data["EST_NM"].Str();
            o.Est_Type = data["EST_TYP"].Str();
            o.ContactDetails = data["CON_DET"].Str();
            o.Address = data["EST_ADR"].Str();
            o.EmailAddress = data["EMAIL_ADR"].Str();
            o.CompanyLogo = data["LOGO"].Str();
            o.EstablishmentLocation = data["URL_LINKED"].Str();

            //o.isLeader = Convert.ToBoolean(data["isLeader"].Str());
            return o;
        }


        public static IEnumerable<dynamic> GetAllBusinessDocumentRequestList(IEnumerable<dynamic> data, string userid = "", int limit = 100, bool fullinfo = true)
        {
            if (data == null) return null;
            var items = GetAllBusinessDocumentRequest_List(data);
            var count = items.Count();
            if (count >= limit)
            {
                var o = items.Last();
                var filter = (o.NextFilter = Dynamic.Object);
                items = items.Take(count - 1).Concat(new[] { o });
                filter.NextFilter = o.num_row;
                filter.Userid = userid;
            }
            return items;
        }
        public static IEnumerable<dynamic> GetAllBusinessDocumentRequest_List(IEnumerable<dynamic> data, bool fullinfo = true)
        {
            if (data == null) return null;
            return data.Select(e => Get_AllBusinessDocumentRequest_List(e));
        }
        public static IDictionary<string, object> Get_AllBusinessDocumentRequest_List(IDictionary<string, object> data, bool fullinfo = true)
        {
            dynamic o = Dynamic.Object;
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            o.num_row = data["Num_Row"].Str();
            o.PL_ID = data["PL_ID"].Str();
            o.PGRP_ID = data["PGRP_ID"].Str();
            o.BussinessID = data["BIZ_ID"].Str();
            o.Category = data["CATEGORY"].Str();
            o.ControlNo = data["CTRL_NO"].Str();
            o.CTCNo = data["CTC_NO"].Str();
            o.InitialCapital = data["INIT_CAP"].Str();
            o.RegisteredNo = data["REG_NO"].Str();
            o.Type = data["TYP"].Str();
            o.BusinessName = data["BIZ_NM"].Str();
            o.BusinessAddress = data["BIZ_ADDR"].Str();
            o.Email = data["BIZ_EMAIL"].Str();
            o.ContactNo = data["CT_NO"].Str();
            o.FirstName = data["OWN_FRST_NM"].Str();
            o.MiddleInitial = data["OWN_MI_NM"].Str();
            o.LastName = data["OWN_LST_NM"].Str();
            o.FullName = data["OWN_FLL_NM"].Str();
            o.OwnerAddress = data["OWN_ADDR"].Str();
            o.OwnerEmail = data["OWN_EMAIL"].Str();
            o.OwnerContactNo = data["OWN_CT_NO"].Str();

            o.ReqDocID = data["REQ_DOC_ID"].Str();
            o.DoctypeID = data["DOCTYP_ID"].Str();
            o.DoctypeNM = data["DOCTYP_NM"].Str();
            o.Category_Doc_ID = data["CAT_DOC"].Str();
            o.OTRDocumentType = data["OTR_DOC_ID"].Str();
            o.ApplicationDate = (data["APP_DATE"].Str() == "") ? "" : Convert.ToDateTime(data["APP_DATE"].Str()).ToString("MMM dd, yyyy");
            o.AppointmentDate = (data["APPT_DATE"].Str() == "") ? "" : Convert.ToDateTime(data["APPT_DATE"].Str()).ToString("MMM dd, yyyy");

            o.Status = data["STATUS"].Str();
            o.Status_NM = data["STATUS_NM"].Str();

            o.ORNO = data["OR_NO"].Str();
            o.CTCNo = data["CTC_NO"].Str();
            o.IssuedDate = (data["DATE_ISSUED"].Str() == "") ? "" : Convert.ToDateTime(data["DATE_ISSUED"].Str()).ToString("MMM dd, yyyy");
            o.URL_DocPath = data["URL_DOCPATH"].Str();
            o.BrgyCaptain = data["BRGY_CAPTAIN"].Str();
            o.BrgyCaptainSignature = data["BRGY_CAPTAIN_SIGNATURE"].Str();
            o.Amount = data["AMOUNT"].Str();
            
            o.OTRDocType = data["DOC_TYPE"].Str();
            o.OTRDocContent = data["DOC_Content"].Str();
            o.OTRCategory = data["OTR_CATEGORY"].Str();
            o.OTRCategoryNM = data["OTR_CATEGORY_NM"].Str();
            

            //o.isLeader = Convert.ToBoolean(data["isLeader"].Str());
            return o;
        }


        public static IEnumerable<dynamic> GetAllDocRequirementList(IEnumerable<dynamic> data, string userid = "", int limit = 100, bool fullinfo = true)
        {
            if (data == null) return null;
            var items = GetAllDocRequirement_List(data);
            var count = items.Count();
            if (count >= limit)
            {
                var o = items.Last();
                var filter = (o.NextFilter = Dynamic.Object);
                items = items.Take(count - 1).Concat(new[] { o });
                filter.NextFilter = o.num_row;
                filter.Userid = userid;
            }
            return items;
        }
        public static IEnumerable<dynamic> GetAllDocRequirement_List(IEnumerable<dynamic> data, bool fullinfo = true)
        {
            if (data == null) return null;
            return data.Select(e => Get_AllDocRequirement_List(e));
        }
        public static IDictionary<string, object> Get_AllDocRequirement_List(IDictionary<string, object> data, bool fullinfo = true)
        {
            dynamic o = Dynamic.Object;
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            o.num_row = data["RW_NO"].Str();
            o.Category = data["CATEGORY"].Str();
            o.CategoryNM = data["CATEGORY_NM"].Str();
            o.SEQ_NO = data["SEQ_NO"].Str();
            o.DOC_REQRD = data["REQ_NM"].Str();
            //o.isLeader = Convert.ToBoolean(data["isLeader"].Str());
            return o;
        }


        public static IEnumerable<dynamic> Notifications(IDictionary<string, object> data)
        {
            List<IDictionary<string, object>> list = new List<IDictionary<string, object>>();
            list.Add(Notification(data));
            return list;
        }
        public static IDictionary<string, object> Notification(IDictionary<string, object> data)
        {
            dynamic o = Dynamic.Object;
            o.NotificationID = ((int)data["NOTIF_ID"].Str().ToDecimalDouble()).ToString("X");
            o.DateTransaction = data["RGS_TRN_TS"];
            o.Title = data["NOTIF_TTL"].Str();
            o.Description = data["NOTIF_DESC"].Str();
            o.IsCompany = data["S_COMP"].To<bool>(false);
            o.IsOpen = data["S_OPN"].To<bool>(false);
            string type = data["TYP"].Str();
            if (!type.IsEmpty()) o.Type = type;

            try
            {
                DateTime datetime = data["RGS_TRN_TS"].To<DateTime>();
                o.DateDisplay = datetime.ToString("MMM dd, yyyy");
                o.TimeDisplay = datetime.ToString("hh:mm:ss tt");
                o.FulldateDisplay = $"{o.DateDisplay} {o.TimeDisplay}";
            }
            catch { }
            return o;
        }


        

    }
}
