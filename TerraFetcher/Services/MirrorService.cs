using RestSharp;
using System;
using TerraFetcher.Models;

namespace TerraFetcher.Services
{
    public class MirrorService
    {
        private string baseUrl = "https://graph.mirror.finance";

        public MirrorResponse GetCurrentPrice()
        {
            var timestamp = GetTimestamp();
            return GetPrice(timestamp);
        }

        public MirrorResponse GetPrice(int timestamp)
        {
            var client = new RestClient(baseUrl);
            var request = new RestRequest("/graphql", Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody("{\"query\": \"{ assets { symbol name prices{ priceAt(timestamp: " + timestamp + "000 ) oraclePriceAt(timestamp: " + timestamp + "000 ) } } }\"}");

            var response = client.Execute<MirrorResponse>(request);
            return response.Data;
        }

        public int GetTimestamp()
        {
            return (int)((DateTime.Now.Subtract(new DateTime(1970, 1, 1))).TotalSeconds);
        }
    }
}