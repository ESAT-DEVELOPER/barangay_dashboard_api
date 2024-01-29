using System;
using System.Collections.Generic;
using Comm.Commons.Extensions;
using System.Linq;

namespace webapi.App.Aggregates.Common
{
    public class NotificationDto
    {
        public static IEnumerable<dynamic> FilterNotifications(IEnumerable<dynamic> list, int limit = 50)
        {
            if (list == null) return null;
            var items = Notifications(list);
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
        public static IEnumerable<dynamic> Notifications(IEnumerable<dynamic> list)
        {
            if (list == null) return null;
            return list.Select(e => Notification(e));
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
            bool IsWinning = data["S_WNNG"].To<bool>(false);
            bool IsReceivedAmount = data["S_RCVNG_AMT"].To<bool>(false);
            if (IsWinning || IsReceivedAmount)
            {
                if (IsWinning) o.IsWinning = IsWinning;
                else if (IsReceivedAmount) o.IsReceivedAmount = IsReceivedAmount;
                o.Amount = data["AMT"].Str().ToDecimalDouble();
            }
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