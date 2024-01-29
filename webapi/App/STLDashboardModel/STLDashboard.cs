using Org.BouncyCastle.Bcpg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace webapi.App.STLDashboardModel
{
    public class STLDashboard
    {
    }
    public class STLParty
    {
        public String PL_ID;
        public String PL_NM;
        public String PL_ADD;
        public String PL_TEL_NO;
        public String URL_STRMNG_NM;
        public String URL_STRMNG;
        public String APP_SHRING_DESC;
    }
    public class STLRequestValidator
    {
        public string Host;
        public string Username;
        public string Password;
    }

    public class HeadOffice
    {
        //Party List Information
        public string parmplid { get; set; }
        public string parmplnm { get; set; }
        public string parmpladd { get; set; }
        public string parmtelno { get; set; }

        public string parmplemladd { get; set; }

        //Group Information
        public string parmpgrpid { get; set; }
        public string parmpsncd { get; set; }
        public string parmpltclid { get; set; }


    }
    public class AgentHeadOffice : HeadOffice
    {

        //User Information
        public string parmusrfnm { get; set; }
        public string parmusrlnm { get; set; }
        public string parmusrmnm { get; set; }
        public string parmusrnm { get; set; }
        public string parmusrpsswrd { get; set; }
        public string parmaddrss { get; set; }
        //6
        public string parmusrmobno { get; set; }
        public string parmemladd { get; set; }
        public string parmuserid { get; set; }
    }
    public class PCSCO : AgentHeadOffice
    {
        public string parmurlstrmngnm { get; set; }
        public string parmurlstrmng { get; set; }
        public string parmapkurl { get; set; }
        public string parmapkupdturl { get; set; }
    }
    public class GROUPEVENT
    {
        public string PL_ID;
        public string PGRP_ID;
        public string Title;
        public string Description;
        public string Date;
        public string Time;
        public string Location;
        public string Process;
        public string Event;
    }

    public class LiveStreaming
    {
        public string LiveStreamUrl;
        public string LiveStreamDescription;
        public string Group;
        public string PL_ID;
        public string PGRP_ID;
    }
    public class GroupList
    {
        public string PL_ID;
        public string PGRP_ID;
    }

    public class Donation
    {
        public string PL_ID;
        public string PGRP_ID;
        public double Amount;
        public string Purpose;
        public string DateReceived;
        public string iUSR_ID;
        public string DonoID;
        public string OTP;
        public string MobileNo;
    }
    public class Sitio
    {
        public string Region;
        public string Province;
        public string Municipality;
        public string Brgy;
        public string SitioID;
        public string SitioName;
    }
    public class Barangay
    {
        public string IslandCode;
        public string Region;
        public string Province;
        public string Municipality;
        public string BarangayCode;
        public string BarangayName;
        public string MunicipalityCode;
        public string Code;
        public string Name;
        public string ID;
    }
    public class AcctStatistic
    {
        public string PGRP_ID;
        public string Reg;
        public string Prov;
        public string Mun;
        public string Brgy;
        public string Sitio;
        public int AgeFrom;
        public int AgeTo;

    }
    public class TextBlasting
    {
        public string MobileNumber;
        public string TextMessage;
    }
    public class IndividualText
    {
        public string MobileNumber;
        public string TextMessage;
        public string Id;
        public string SMSType;
        public string SMS_ID;
    }
    public class GenerateBlasting
    {
        public string Prefix;
        public string TextMessage;
        public int FromNo;
        public int ToNo;
    }
    public class TextBlastingMemberAccount
    {
        public string AccntID;
        public string Row_Num;
        public string Row_Num_To;
        public string Search;
    }

    public class Memorandum
    {
        public string MemoId;
        public string BrgyCode;
        public string MemorandumNo;
        public string MemorandumURL;
        public string Attachment;
        public string Subject;
    }
    public class DocumentType
    {
        public string DocTypeID;
        public string DocTypeNM;
        public string BrgyCode;
        public int Category;
        public string DocContent;
        public string iDocRequirements;
    }
    public class BarangayOfficial
    {
        public string Userid;
        public string BarangayPositionID;
        public string Committee;
        public string TermStart;
        public string TermEnd;
        public string RankNo;
        public string BrgyOfficialID;
    }
    public class Profile_Picture
    {
        public string PL_ID;
        public string PGRP_ID;
        public string Userid;
        public string ImageURL;
        public string ImageBase64;
    }
    public class RequestDocument
    {
        public string ReqDocID;
        public string DoctypeID;
        public string BusinessID;
        public string BusinessName;
        public string BusinessAddress;
        public string Purpose;
        public string Type;
        public string BusinessOwnerAddress;
        public string BusinessOwnerName;
        public string RequestorNM;
        public string RequestorID;
        public List<String> Attachment;
        public String iAttachments;
        public string CTCNo;
        public string ORNO;
        public string Amount;
        public string STATUS;
        public string ApplicationDate;
        public string URLAttachment;
        public string URL_DocPath;
        public string CategoryID;
        public string Category_Document;
        public string BizReport;
        public string IssuedDate;
        public string ControlNo;
        public string AppointmentDate;
        public string OTRDocumentType;
        public string isPaid;
        public string isFree;

    }
    public class Blotter
    {
        public string PL_ID;
        public string PGRP_ID;
        public string BarangayCaseNo;
        public string JsonStringComplainant;
        public string ComplainantSitio;
        public string JsonRespondent;
        public string RespondentSitio;
        //public string RegionCode;
        //public string ProvinceCode;
        //public string MunicipalCode;
        public string BrgyCode;
        //public string PurokOrSitio;
        //public string BarangayCaptain;
        public string BarangaySecretary;
        public string Witness;
        public int CaseType;
        public string Accusations;
        public string PlaceOfIncident;
        //public string JsonStringAccomplice;
        public string DateOfIncident;
        public string TimeOfIncident;
        public string Statement;
        public string DateCreated;
        public string ModifiedBy;
        public string DTModified;
        public string JsonAttachment;
        public string Docname;
        public string TermsOfSettlement;
        public Decimal Award;
        public DateTime SummonDate = DateTime.Parse("01/01/1900 12:00:00");
        public string SummonTime;
        public string PangkatChairman;
        public string PangkatSecretary;
        public string PangkatFMember;
        public string PangkatSMember;
        public List<string> Attachments;
        public String iAttachments;
        public int Status;
        public string TemplateId;
        public string TemplateName;
        public string TemplateContent;
    }

    public class HouseDetails
    {
        public string HouseId;
        public string HouseholdId;
        public string FamilyId;
        public string HouseOwner;
        public string Householder;
        public string Household;
        public string NewFamilyHead;
        public string FamilyMember;
        public string NewFamilyMember;
        public string HouseholdClassification;
        public string HouseClassification;
        public string SitioId;
        public string Relationship;
        public string HomeAddress;
        public int LivingTheHouse;
        public int LivingWithFamily;
        public string CreatedBy;
        public string DateCreated;
        public string ModifiedBy;
        public string ModifiedDate;
    }

    public class PurposeDetails
    {
        public string PurposeID;
        public string PurposeDescription;
    }

    //public class Position
    //{
    //    public string JsonString;
    //}
    public class Position
    {
        public int PositionId;
        public string PL_ID;
        public string PGRP_ID;
        public string LOC_BRGY;
        public string Positionn;
        public int Category;
    }

    public class Report
    {
        public string XML = null;
        public string brgyCode;
        public int typeOfReport;
        public string columnBits;
        public string filterBits;
        public int? agefrom = null;
        public int? ageto = null;
        public string code = null;
        public int loctype;
    }

    public class ReportSettings
    {
        public int settingsID;
        public string plid
            , pgrpid
            , userid = "";
        public string description = "";
        public int layoutIndex;
        public string title;
        public string subtitle;
        public int dateFormat;
        //public int[] selectBits = new int[50];
        //public List<int[]> distinctionBits = new List<int[]>();
        public string selectBits = "";
        public string distinctionBits = "";
        public int? from;
        public int? to;
        //public bool[] chartIndex = new bool[] { true, true, true, true };
        //public bool[] signIndex = new bool[] { true, true };
        public string chartIndex = "";
        public string signIndex = "";
        public int isactive = 1;
    }

    public class StatisticalData
    {

        public string code = null;
        public int loctype;
        public string xml = null;
    }

    public class Sitioreport
    {
        public string json = null;
        public int type;
    }

    public class DocumentStatistics
    {
        public int type;
        public string dtfrom;
        public string dtto;
    }

    public class Religion
    {
        public string Code;
        public string Description;
    }

    public class CertificateTypeDetails
    {
        public string CertTypID;
        public string CertTypDescription;

    }
    public class BusinessOwnershipType
    {
        public string BusinessOwnershiptypeID;
        public string BusinessOwnershipDescription;
    }
    public class AppointDetails
    {
        public string JsonString;
        public string DateCreated;
    }

    public class BrgyCedula
    {
        public string CedulaId;
        public string RequestBy;
        public string RequestType;
        public string RequestPurpose;
        public string Birthdate;
        public string Gender;
        public string Address;
        public string Citizenship;
        public string CivilStatus;
        public decimal BCT;
        public decimal GEB;
        public decimal GEBTax;
        public decimal GEP;
        public decimal GEPTax;
        public decimal IRP;
        public decimal IRPTax;
        public decimal TotalTax;
        public decimal InterestRate;
        public decimal Interest;
        public decimal AmountPaid;
        public string RequestDate;
        public string ClaimDate;
        public string ProcessBy;
        public string ProcessDate;
        public string ReleaseDate;
    }

    public class BrgyClearance
    {
        public string PLID;
        public string PGRPID;
        public string ClearanceID;
        public string ClearanceNo;
        public string ControlNo;
        public string TypeofClearance;
        public string Purpose;
        public string ORNumber;
        public string AmountPaid;
        public string DocStamp;
        public int EnableCommunityTax;
        public string CTCNo;
        public string CTCIssuedAt;
        public string CTCIssuedOn;
        public string DocumentID;
        public string UserID;
        public string Requestor;

        public int exportDoc;

        public string IssuedDate;
        public string ExpiryDate;
        public int MosValidity;
        public int StatusRequest;
        public string AppointmentDate;
        public string ExportedDocument;
        public string URLDocument;
    }
    public class cCommunity
    {
        public string CommunityID;
        public string CommunityName;
        public string CommunityDescription;
        public int TypeLevel;
        public string ScopeLevelDescription;
        public string ScopeLevelDescriptionVal;
    }
    public class cPostCommunity
    {
        public string PostID;
        public string PostTitle;
        public string PostDescription;
        public string base64Attachment;
        public string URL;
        public string ImgURL;
        public string CommunityID;
    }
    public class cCommentPostCommunity
    {
        public string CommunityID;
        public string PostID;
        public string CommentID;
        public string Reason;
    }
    public class AcptCommunity
    {
        public string PL_ID;
        public string PGRP_ID;
        public string CommunityID;
        public int RequestStatus;
    }

    public class DOD
    {
        public string PLID;
        public string PGRPID;
        public string DCID;
        public string DeathCertificateNo;
        public string ControlNo;
        public string CauseofDeath;
        public string DiedDate;
        public string DiedTime;
        public string ORNumber;
        public string ORIssuedDate;
        public string AmountPaid;
        public string DocStamp;
        public int EnableCommunityTax;
        public string CTCNo;
        public string CTCIssuedAt;
        public string CTCIssuedOn;
        public string UserID;
        public string ResidenceName;
        public string VerifiedBy;
        public string CertifiedBy;

        public string IssuedDate;
        public string iTagline;
        public string ExportedDocument;
        public string URLDocument;
    }
    public class BrgyBusinessClearance
    {
        public string PLID;
        public string PGRPID;
        public string BusinessClearanceID;
        public string ControlNo;
        public string BusinessID;
        public string ORNumber;
        public string AmountPaid;
        public string DocStamp;
        public int EnableCommunityTax;
        public string CTCNo;
        public string CTCIssuedAt;
        public string CTCIssuedOn;
        public string OwnerID;
        public string IssuedDate;
        public string ExpiryDate;
        public int MosValidity;
        public string VerifiedBy;
        public string CertifiedBy;
        public string AppointmentDate;
        public int StatusRequest;
        public string ExportedDocument;
        public string URLDocument;
    }

    public class Fingerprint
    {
        public List<FingerImage> ImageList;
        public string UserId;
        public string Json;
        public string RegisterDate;
        public string RegisterTime;
        public DateTime RegisterTimeStamp;
        public string UpdateDate;
        public string UpdateTime;
        public DateTime UpdateTimeStamp;
        public string Operator;
        public class FingerImage
        {
            public int Id;
            public string Index;
            public string Image;
        }
    }

    public class VirtualID
    {
        public string UserId;
        public string FrontIdImageUrl;
        public string BackIdImageUrl;
    }
}

