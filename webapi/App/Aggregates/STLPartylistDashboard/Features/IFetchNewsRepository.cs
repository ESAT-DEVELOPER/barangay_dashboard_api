using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using webapi.Commons.AutoRegister;
using Microsoft.AspNetCore.Mvc;
using webapi.App.RequestModel.AppRecruiter;
using webapi.App.Aggregates.Common;
using Infrastructure.Repositories;
using Comm.Commons.Extensions;

namespace webapi.App.Aggregates.STLPartylistDashboard.Features
{
    [Service.ITransient(typeof(FetchNewsRepository))]
    public interface IFetchNewsRepository
    {
        Task<(Results result, String message)> FetchNewsAsync(FetchLoadNews req);
    }
    public class FetchNewsRepository : IFetchNewsRepository
    {
        private readonly IRepository _repo;
        public FetchNewsRepository(IRepository repo)
        {
            _repo = repo;
        }
        public async Task<(Results result, string message)> FetchNewsAsync([FromBody] FetchLoadNews req)
        {
            var result = _repo.DQuery<dynamic>($"spfn_BIMSSNEWS00B0B").FirstOrDefault();
            if (result != null)
            {
                var row = ((IDictionary<string, object>)result);
                var ResultCode = row["RESULT"].Str();
                if (ResultCode == "1")
                {
                    return (Results.Success, "Successfully Fetch!");
                }
                else if (ResultCode == "2")
                    return (Results.Failed, "License was not valid");
            }
            return (Results.Null, null);
        }
    }
}
