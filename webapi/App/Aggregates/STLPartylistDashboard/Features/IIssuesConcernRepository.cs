using Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using webapi.App.Aggregates.Common;
using webapi.App.Aggregates.SubscriberAppAggregate.Common;
using webapi.App.Model.User;
using webapi.App.RequestModel.Feature;
using webapi.Commons.AutoRegister;
using webapi.Services.Dependency;

using Comm.Commons.Extensions;
using System.Net.Mail;
using System.Net;
using webapi.App.RequestModel.Common;
using webapi.App.Aggregates.Common.Dto;

namespace webapi.App.Aggregates.STLPartylistDashboard.Features
{
    [Service.ITransient(typeof(IssuesConcernRepository))]
    public interface IIssuesConcernRepository
    {
        Task<(Results result, String message)> SendIssuesConcernAsync(ReportAProblemRequest request);
        Task<(Results result, String message)> UpdateIssuesConcernAsync(ReportAProblemRequest request);
        Task<(Results result, String message)> ProcessIssuesConcernAsync(ReportAProblemRequest request);
        Task<(Results result, object concern)> LoadIssuesConcern(FilterRequest request);
        Task<(Results result, object concern)> LoadIssuesConcernTickectNo(FilterRequest request);
        Task<(Results result, object concern)> LoadIssuesConcern1(FilterRequest request);
        Task<(Results result, string total_concern)> TotalIssuesConcernAsync(FilterRequest request);
        Task<(Results result, object concern)> LoadIssuesConcernAttachment(ReportAProblemRequest request);
        Task<(Results result, String message)> ClosedIssuesConcernAsync(ReportAProblemRequest request);
    }
    public class IssuesConcernRepository:IIssuesConcernRepository
    {
        private readonly ISubscriber _identity;
        private readonly IRepository _repo;
        private readonly IFileData _fd;
        public STLAccount account { get { return _identity.AccountIdentity(); } }
        public IssuesConcernRepository(ISubscriber identity, IRepository repo, IFileData fd)
        {
            _identity = identity;
            _repo = repo;
            _fd = fd;
        }
        public async Task<(Results result, String message)> SendIssuesConcernAsync(ReportAProblemRequest request)
        {
            string supportAccount = _fd.String("Company:Support");
            var split = supportAccount.Split(':');
            if (split.Length == 2)
            {
                string guser = split[0], gpass = split[1];
                if (!guser.IsEmpty() && !gpass.IsEmpty())
                {
                    var result = _repo.DSpQueryMultiple("dbo.spfn_BDB0D", new Dictionary<string, object>(){
                        { "parmplid", account.PL_ID },
                        { "parmpgrpid", account.PGRP_ID },
                        { "parmuserid", account.USR_ID },
                    }).ReadSingleOrDefault();
                    if (result != null)
                    {
                        var row = ((IDictionary<string, object>)result);
                        request.SenderAccount = row["ACT_ID"].Str();
                        request.SenderName = row["FLL_NM"].Str().Trim();
                        if (request.AddressLocation.IsEmpty())
                            request.AddressLocation = row["PRSNT_ADDR"].Str().Trim();
                        request.TicketNo = ((int)DateTime.Now.ToTimeMillisecond()).ToString("X");
                        var resAsync = await PrepareSendingToGmail(request, guser, gpass);
                        if (resAsync.result == Results.Success)
                        {
                            _repo.DSpQueryMultiple("dbo.spfn_AEARP0A", new Dictionary<string, object>(){
                                { "parmplid", account.PL_ID },
                                { "parmpgrpid", account.PGRP_ID },
                                { "parmuserid", account.USR_ID },
                                { "parmsitioid", request.Sitio },
                                { "parmsssid", account.SessionID },
                                { "parmtckt", request.TicketNo },
                                { "parmsbjct", request.Subject },
                                { "parmbody", request.Body },
                                { "parmxattchmnt", request.iAttachments },
                                { "parmdvcnm", request.DeviceName },
                                { "parmdvcmdlid", request.DeviceID },
                                { "parmdvcmnfctrr", request.Manufacturer },
                                { "parmdvcsrial", request.Serial },
                                { "parmdvcbnd", request.Brand },
                                { "parmdvcos", request.DeviceOS },
                                { "parmdvcvrsn", request.DeviceVersion },
                            }).ReadSingleOrDefault();
                        }
                        return resAsync;
                    }
                }
            }
            return (Results.Failed, "Support account not set, please contact to nearest office."); //branch
        }
        private async Task<(Results result, String message)> PrepareSendingToGmail(ReportAProblemRequest request, String gUser, String gPass)
        {
            MailMessage message = new MailMessage();
            message.From = new MailAddress(gUser);
            message.To.Add(gUser);
            message.Subject = "Abacos Report Problem (" + request.TicketNo + ")";
            message.IsBodyHtml = true;
            message.Body = getBodyFullMessageProblemRequest(request);
            return await TrySendToGmail(request, gUser, gPass, message);
        }
        private async Task<(Results result, String message)> TrySendToGmail(ReportAProblemRequest request, String gUser, String gPass, MailMessage message, int attemp = 5)
        {
            try
            {
                using (var stmp = new SmtpClient
                {
                    Host = "smtp.mail.yahoo.com",
                    Port = 587,
                    EnableSsl = true,
                    UseDefaultCredentials = false,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = new NetworkCredential(gUser, gPass),
                    Timeout = 20000,
                })
                {
                    stmp.Send(message);
                    //stmp.Send(gUser, gUser, "Abacos Report Problem (" + request.TicketNo + ")", getBodyFullMessageProblemRequest(request));
                    /*
			SendEmailExternal(GlobalEmailSupport,"Abacos Report Problem (" + ticketno + ")", htmlString);
			SendDirectSMS(userid,"Thank you for contacting us. This is an automated response confirming the receipt of your ticket ID: "+ticketno+". One of our agents will get back to you as soon as possible.");
			SendSMSDirectMobile(GlobalMobileSupport, "You have new reported problem of Abacos App with ticket ID: "+ticketno+". please check your admin support email to view full report details");
                    */
                    return (Results.Success, "Problem successfully reported");
                }
            }
            catch (Exception ex)
            {
                String exMessage = ex.Message;
            }
            if (attemp > 0)
                return await TrySendToGmail(request, gUser, gPass, message, attemp - 1);
            return (Results.Failed, "Cannot send right now, please try again later");
        }

        private string getBodyFullMessageProblemRequest(ReportAProblemRequest request)
        {
            string htmlAttachment = "";
            if (!request.iAttachments.IsEmpty())
            {
                foreach (var attachment in request.Attachments)
                    htmlAttachment += (htmlAttachment.IsEmpty() ? "" : "<br/>") + ($"<a href='{ attachment }' target='_blank'>{ attachment }</a>");
                htmlAttachment = $"<tr><td><b>Attachment(s): </b></td><td>{ htmlAttachment }</td></tr>";
            }
            return $@"
<!DOCTYPE html>
<html><head>
<style type='text/css'>
body{{ font-family: Helvetica, Verdana; font-size:2vw; color: #4d4c4c; margin: 0; }}
table{{ border-collapse: collapse; border: 1px solid #d1d1d1; width: 100% }}
td{{ border: 1px solid #d1d1d1; padding: 5px 10px; border: 1px solid #d1d1d1; }}
tr{{ padding: 2px }}
h1,h2,h3,h4{{ margin: 2px; vertical-align: bottom; }}
</style>
</head>
<body>
<div style='margin: 10px' align='center'>
    <table cellspacing='0' cellpadding='0'>
        <tr><td colspan='2' align='center'><h3>Account Information</h3></td></tr>
        <tr><td><b>Account #:</b></td><td>{ request.SenderAccount }</td></tr>
        <tr><td><b>Account Name:</b></td><td>{ request.SenderName }</td></tr>
        <tr><td><b>Address: </b></td><td>{ request.AddressLocation }</td></tr>
        <tr><td colspan='2'><h3>Device Information</h3></td></tr>
        <tr><td><b>Device ID: </b></td><td>{ request.DeviceID }</td></tr>
        <tr><td><b>Model: </b></td><td>{ request.DeviceName }</td></tr>
        <tr><td><b>Manufacturer: </b></td><td>{ request.Manufacturer }</td></tr>
        <tr><td><b>Serial: </b></td><td>{ request.Serial }</td></tr>
        <tr><td><b>Brand: </b></td><td>{ request.Brand }</td></tr>
        <tr><td colspan='2'><h3>Android OS</h3></td></tr>
        <tr><td><b>OS: </b></td><td>{ request.DeviceOS }</td></tr>
        <tr><td><b>Version: </b></td><td>{ request.DeviceVersion }</td></tr>
        <tr><td colspan='2'><h3>Reported Problem</h3></td></tr>
        <tr><td><b>Subject: </b></td><td>{ request.Subject }</td></tr>
        <tr><td><b>Problem: </b></td><td>{ request.Body }</td></tr>
        { htmlAttachment }
    </table>
</div>
</body>
</html>";
        }


        public async Task<(Results result, object concern)> LoadIssuesConcernAttachment(ReportAProblemRequest request)
        {
            var result = _repo.DSpQueryMultiple($"dbo.spfn_AEB0A", new Dictionary<string, object>()
            {
                {"parmplid",account.PL_ID },
                {"parmpgrpid",account.PGRP_ID },
                {"parmtransactionno", request.TransactionNo}
            });
            if (result != null)
                return (Results.Success, STLSubscriberDto.GetAttachementIssuesConcernList(result.Read<dynamic>(), 100));
            return (Results.Null, null);
        }
        public async Task<(Results result, object concern)> LoadIssuesConcern(FilterRequest request)
        {
            var result = _repo.DSpQueryMultiple($"dbo.spfn_AEABDB0C", new Dictionary<string, object>()
            {
                {"parmplid",account.PL_ID },
                {"parmpgrpid",account.PGRP_ID },
                {"parmrownum", request.num_row},
                {"parmsrch", request.Search}
            });
            if (result != null)
                return (Results.Success, STLSubscriberDto.GetAllIssuesConcernList(result.Read<dynamic>(), request.Userid, 100));
            return (Results.Null, null);
        }

        public async Task<(Results result, object concern)> LoadIssuesConcernTickectNo(FilterRequest request)
        {
            var result = _repo.DSpQueryMultiple($"dbo.spfn_AEABDB0C3", new Dictionary<string, object>()
            {
                {"parmplid",request.PL_ID },
                {"parmpgrpid",request.PGRP_ID },
                {"@parmticketno", request.TickectNo}
            });
            if (result != null)
                return (Results.Success, STLSubscriberDto.GetAllIssuesConcernList(result.Read<dynamic>(), request.Userid, 100));
            return (Results.Null, null);
        }
        public async Task<(Results result, object concern)> LoadIssuesConcern1(FilterRequest request)
        {
            var result = _repo.DSpQueryMultiple($"dbo.spfn_AEABDB0C1", new Dictionary<string, object>()
            {
                {"parmplid",request.PL_ID },
                {"parmpgrpid",request.PGRP_ID },
                {"parmrownum", request.num_row},
                {"parmstatus", request.Status},
                {"parmsrch", request.Search}
            });
            if (result != null)
                return (Results.Success, STLSubscriberDto.GetAllIssuesConcernList(result.Read<dynamic>(), request.Userid, 100));
            return (Results.Null, null);
        }

        public async Task<(Results result, string total_concern)> TotalIssuesConcernAsync(FilterRequest req)
        {
            var result = _repo.DSpQuery<dynamic>($"dbo.spfn_AEABDB0C2", new Dictionary<string, object>()
            {
                {"parmplid",req.PL_ID },
                {"parmpgrpid",req.PGRP_ID }
            }).FirstOrDefault();
            if (result != null)
            {
                var row = ((IDictionary<string, object>)result);
                var ResultCode = row["RESULT"].Str();
                if (ResultCode == "1")
                {
                    return (Results.Success, row["TTL_IC"].Str());
                }
            }
            return (Results.Null, null);
        }

        public async Task<(Results result, string message)> UpdateIssuesConcernAsync(ReportAProblemRequest request)
        {
            string supportAccount = _fd.String("Company:Support");
            var split = supportAccount.Split(':');
            if (split.Length == 2)
            {
                string guser = split[0], gpass = split[1];
                if (!guser.IsEmpty() && !gpass.IsEmpty())
                {
                    var result = _repo.DSpQueryMultiple("dbo.spfn_BDB0D", new Dictionary<string, object>(){
                        { "parmplid", account.PL_ID },
                        { "parmpgrpid", account.PGRP_ID },
                        { "parmuserid", account.USR_ID },
                    }).ReadSingleOrDefault();
                    if (result != null)
                    {
                        var row = ((IDictionary<string, object>)result);
                        request.SenderAccount = row["ACT_ID"].Str();
                        request.SenderName = row["FLL_NM"].Str().Trim();
                        if (request.AddressLocation.IsEmpty())
                            request.AddressLocation = row["PRSNT_ADDR"].Str().Trim();
                        //request.TicketNo = ((int)DateTime.Now.ToTimeMillisecond()).ToString("X");
                        var resAsync = await PrepareSendingToGmail(request, guser, gpass);
                        if (resAsync.result == Results.Success)
                        {
                            _repo.DSpQueryMultiple("dbo.spfn_AEARP0B", new Dictionary<string, object>(){
                                //5
                                { "parmplid", account.PL_ID },
                                { "parmpgrpid", account.PGRP_ID },
                                { "parmuserid", account.USR_ID },
                                { "parmsssid", account.SessionID },
                                { "parmsitioid", request.Sitio },

                                //5
                                { "parmtran_no", request.TransactionNo },
                                { "parmtckt", request.TicketNo },
                                { "parmsbjct", request.Subject },
                                { "parmbody", request.Body },
                                { "parmxattchmnt", request.iAttachments },

                                //5
                                { "parmdvcnm", request.DeviceName },
                                { "parmdvcmdlid", request.DeviceID },
                                { "parmdvcmnfctrr", request.Manufacturer },
                                { "parmdvcsrial", request.Serial },
                                { "parmdvcbnd", request.Brand },
                                //2
                                { "parmdvcos", request.DeviceOS },
                                { "parmdvcvrsn", request.DeviceVersion },
                            }).ReadSingleOrDefault();
                        }
                        return resAsync;
                    }
                }
            }
            return (Results.Failed, "Support account not set, please contact to nearest office."); //branch
        }

        public async Task<(Results result, string message)> ProcessIssuesConcernAsync(ReportAProblemRequest request)
        {
            var results = _repo.DSpQueryMultiple($"dbo.spfn_AEARP0C", new Dictionary<string, object>()
            {
                {"parmplid",account.PL_ID },
                {"parmpgrpid",account.PGRP_ID },
                {"parmuserid", account.USR_ID},
                {"parmtran_no", request.TransactionNo},
                {"parmtckt", request.TicketNo},
                {"parmcorrectiveaction", request.CorrectiveAction}
            }).ReadSingleOrDefault();
            if (results != null)
            {
                var row = ((IDictionary<string, object>)results);
                string ResultCode = row["RESULT"].Str();
                if (ResultCode == "1")
                    return (Results.Success, "Succesfull save");
            }
            return (Results.Null, null);
        }

        public async Task<(Results result, string message)> ClosedIssuesConcernAsync(ReportAProblemRequest request)
        {
            var results = _repo.DSpQueryMultiple($"dbo.spfn_AEARP0D", new Dictionary<string, object>()
            {
                {"parmplid",account.PL_ID },
                {"parmpgrpid",account.PGRP_ID },
                {"parmuserid", account.USR_ID},
                {"parmtran_no", request.TransactionNo},
                {"parmcorrectiveaction", request.CorrectiveAction },
                {"parmtckt", request.TicketNo},
                {"parmxattchmnt", request.iAttachments }
            }).ReadSingleOrDefault();
            if (results != null)
            {
                var row = ((IDictionary<string, object>)results);
                string ResultCode = row["RESULT"].Str();
                if (ResultCode == "1")
                    return (Results.Success, $"Ticket # {request.TicketNo} already Closed.");
            }
            return (Results.Null, null);
        }
    }
}
