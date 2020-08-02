using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace DragonchainSDK.Framework.Redisearch
{
    public static class RedisearchHelper
    {
        public static string GetRedisearchParams(string transactionType, string query, bool verbatim, int offset, int limit, string sort, bool sortAscending, bool idsOnly)
        {
            var @params = new Dictionary<string, string>();

            @params.Add("transaction_type", transactionType);

            if (!string.IsNullOrWhiteSpace(query)) { @params.Add("q", query); }

            @params.Add("verbatim", verbatim ? "1" : "0");

            if (!string.IsNullOrWhiteSpace(sort)) { 
                @params.Add("sort", sort);
                @params.Add("sortAscending", sortAscending ? "1" : "0");
            }

            @params.Add("offset", offset.ToString());
            @params.Add("limit", limit.ToString());

            @params.Add("idsOnly", idsOnly ? "1" : "0");

            return GenerateQueryString(@params);
        }

        private static string GenerateQueryString(Dictionary<string, string> queryObject)
        {
            const string query = "?";
            var array = queryObject.Select(i => $"{HttpUtility.UrlEncode(i.Key)}={HttpUtility.UrlEncode(i.Value)}").ToArray();
            return $"{query}{string.Join("&", array)}";
        }
    }
}
