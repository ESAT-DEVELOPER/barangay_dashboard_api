using Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using webapi.App.Aggregates.Common;
using Comm.Commons.Extensions;
using webapi.App.Aggregates.SubscriberAppAggregate.Common;
using webapi.App.Model.User;
using webapi.Commons.AutoRegister;
using Microsoft.AspNetCore.Mvc;
using webapi.App.STLDashboardModel;
using System.Runtime.CompilerServices;
using com.innovatrics.enrollment;

namespace webapi.App.Aggregates.STLPartylistDashboard.Features
{
    [Service.ITransient(typeof(NewsRepository))]
    public interface INewsRepository
    {
        Task<(Results result, string message)> uploadArticle(Article detail);
        Task<(Results result, object articles)> getArticles(string date);
    }
    public class NewsRepository : INewsRepository
    {
        private readonly ISubscriber _identity;
        public readonly IRepository _repo;
        public STLAccount account { get { return _identity.AccountIdentity(); } }
        public NewsRepository(ISubscriber identity, IRepository repo)
        {
            _identity = identity;
            _repo = repo;
        }

        public async Task<(Results result, string message)> uploadArticle(Article detail)
        {
            var result = _repo.DSpQuery<dynamic>($"dbo.spfn_BIMSSADDARTCL", new Dictionary<string, object>()
            {
                {"parmcategory", detail.Category},
                {"parmpublisher", detail.Publisher},
                {"parmauthor", detail.Author},
                {"parmtitle", detail.Title},
                {"parmdescription", detail.Description},
                {"parmurl", detail.Url},
                {"parmimgurl", detail.ImgUrl},
                {"parmdtpublished", detail.DateTimePublished},
                {"parmdatepublished", detail.DatePublished}
            }).FirstOrDefault();
            if (result != null)
            {
                var row = ((IDictionary<string, object>)result);
                string ResultCode = row["RESULT"].Str();
                if (ResultCode == "1")
                {
                    return (Results.Success, "Successfully posted");
                }
                else if (ResultCode == "0")
                {
                    return (Results.Failed, "Article already exist");
                }
                else if (ResultCode == "2")
                {
                    return (Results.Null, "System Error");
                }
            }
            return (Results.Null, "System Error");
        }



        public async Task<(Results result, object articles)> getArticles(string date)
        {
            var result = _repo.DSpQuery<dynamic>($"dbo.spfn_POSTEDARTCL", new Dictionary<string, object>()
                {
                    {"parmdt", date }
                });
            if (result != null)
                return (Results.Success, result);
            return (Results.Null, null);
        }
    }
}
