using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using webapi.Commons.AutoRegister;
using webapi.App.STLDashboardModel;
using webapi.App.Aggregates.Common;
using webapi.App.Aggregates.SubscriberAppAggregate.Common;
using Infrastructure.Repositories;
using webapi.App.Model.User;
using Comm.Commons.Extensions;
using webapi.App.RequestModel.Common;
using webapi.App.Aggregates.Common.Dto;

namespace webapi.App.Aggregates.STLPartylistDashboard.Features
{
    [Service.ITransient(typeof(PostCommunityRepository))]
    public interface IPostCommunityRepository
    {
        Task<(Results result, String message)> PostCommunityAsync(cPostCommunity req);
        Task<(Results result, String message)> UpdatePostCommunityAsync(cPostCommunity req);
        Task<(Results result, String message)> RemoveCommentPostCommunityAsync(cCommentPostCommunity req);
        Task<(Results result, object comm)> LoadPostCommunityListAsync(FilterRequest req);
        Task<(Results result, object comment)> LoadCommentPostCommunityListAsync(FilterRequest req);
        Task<(Results result, String message)> AcceptRequestCommunityAsync(AcptCommunity req);
    }
    public class PostCommunityRepository:IPostCommunityRepository
    {
        private readonly ISubscriber _identity;
        private readonly IRepository _repo;
        public STLAccount account { get { return _identity.AccountIdentity(); } }
        public PostCommunityRepository(ISubscriber identity, IRepository repo)
        {
            _identity = identity;
            _repo = repo;
        }

        public async Task<(Results result, string message)> PostCommunityAsync(cPostCommunity req)
        {
            req.PostID = ((int)DateTime.Now.ToTimeMillisecond()).ToString("X");
            var results = _repo.DSpQueryMultiple($"dbo.spfn_BIMSRAC000P", new Dictionary<string, object>()
            {
                {"parmplid", account.PL_ID },
                {"parmpgrpid", account.PGRP_ID },
                {"parmcommid", req.CommunityID},
                {"parmpostid", req.PostID},
                {"parmposttitle", req.PostTitle},
                {"parmpostdescription", req.PostDescription},
                {"parmurl", req.URL},
                {"parmuserid", account.USR_ID},
            }).ReadSingleOrDefault();
            if (results != null)
            {
                var row = ((IDictionary<string, object>)results);
                string ResultCode = row["RESULT"].Str();
                if (ResultCode == "1")
                    return (Results.Success, "Successfully Save.");
                else if (ResultCode == "2")
                    return (Results.Failed, "Check your request, Please try again!");
            }
            return (Results.Null, "Please check your Internet Connection.");
        }

        public async Task<(Results result, object comm)> LoadPostCommunityListAsync(FilterRequest req)
        {
            var result = _repo.DSpQueryMultiple($"dbo.spfn_BIMSRAC000P2", new Dictionary<string, object>()
            {
                {"parmplid",account.PL_ID },
                {"parmpgrpid",account.PGRP_ID },
                {"parmcommid",req.CommunityID },
                {"parmrownum",req.num_row },
                {"parmsearch",req.Search }
            });
            if (result != null)
                return (Results.Success, STLSubscriberDto.GetAllPostCommunityList(result.Read<dynamic>(), req.Userid, 100));
            return (Results.Null, null);
        }

        public async Task<(Results result, string message)> UpdatePostCommunityAsync(cPostCommunity req)
        {
            var results = _repo.DSpQueryMultiple($"dbo.spfn_BIMSRAC000P1", new Dictionary<string, object>()
            {
                {"parmplid", account.PL_ID },
                {"parmpgrpid", account.PGRP_ID },
                {"parmcommid", req.CommunityID},
                {"parmpostid", req.PostID},
                {"parmposttitle", req.PostTitle},
                {"parmpostdescription", req.PostDescription},
                {"parmurl", req.ImgURL},
                {"parmuserid", account.USR_ID},
            }).ReadSingleOrDefault();
            if (results != null)
            {
                var row = ((IDictionary<string, object>)results);
                string ResultCode = row["RESULT"].Str();
                if (ResultCode == "1")
                    return (Results.Success, "Successfully Save.");
                else if (ResultCode == "2")
                    return (Results.Failed, "Check your request, Please try again!");
            }
            return (Results.Null, "Please check your Internet Connection.");
        }

        public async Task<(Results result, string message)>RemoveCommentPostCommunityAsync(cCommentPostCommunity req)
        {
            var results = _repo.DSpQueryMultiple($"dbo.spfn_BIMSRACCOM0005", new Dictionary<string, object>()
            {
                {"parmcommid", req.CommunityID},
                {"parmpostid", req.PostID},
                {"parmcommentid", req.CommentID},
                {"parmreason", req.Reason},
                {"parmuserid", account.USR_ID},
            }).ReadSingleOrDefault();
            if (results != null)
            {
                var row = ((IDictionary<string, object>)results);
                string ResultCode = row["RESULT"].Str();
                if (ResultCode == "1")
                    return (Results.Success, "Successfully Remove");
                else if (ResultCode == "2")
                    return (Results.Failed, "Check your request, Please try again!");
            }
            return (Results.Null, "Please check your Internet Connection.");
        }

        public async Task<(Results result, string message)> AcceptRequestCommunityAsync(AcptCommunity req)
        {
            var results = _repo.DSpQueryMultiple($"dbo.spfn_BIMSRAC000A2", new Dictionary<string, object>()
            {
                {"parmplid", req.PL_ID },
                {"parmpgrpid", req.PGRP_ID },
                {"parmcommid", req.CommunityID},
                {"parmreqstatus", req.RequestStatus },
                {"parmuserid", account.USR_ID},
            }).ReadSingleOrDefault();
            if (results != null)
            {
                var row = ((IDictionary<string, object>)results);
                string ResultCode = row["RESULT"].Str();
                if (ResultCode == "1")
                    return (Results.Success, " successfully accepted!");
                else if (ResultCode == "2")
                    return (Results.Failed, "Check your data, Please try again!");
                else if (ResultCode == "3")
                    return (Results.Failed, " successfully decline!");
            }
            return (Results.Null, "Please check your Internet Connection.");
        }

        public async Task<(Results result, object comment)> LoadCommentPostCommunityListAsync(FilterRequest req)
        {
            var result = _repo.DSpQueryMultiple($"dbo.spfn_BIMSRACCOM0004", new Dictionary<string, object>()
            {
                {"parmcommid",req.CommunityID },
                {"parmpostid",req.PostID },
                {"parmrownum",req.num_row }
            });
            if (result != null)
                return (Results.Success, STLSubscriberDto.GetAllCommentPostCommunityList(result.Read<dynamic>(), req.Userid, 100));
            return (Results.Null, null);
        }
    }
}
