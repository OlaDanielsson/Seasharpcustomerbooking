using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Seasharpcustomerbooking
{
    public static class ApiConnection
    {
        public static HttpClient ApiClient = new HttpClient();

        static ApiConnection()
        {
            ApiClient.BaseAddress = new Uri("http://193.10.202.81/ConnectingAPI/");
            ApiClient.DefaultRequestHeaders.Clear();
            ApiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
    }
}
