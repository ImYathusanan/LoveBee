using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoveBeeApi.Helpers
{
    public static class Extentions
    {
        public static void AddApplicationError(this HttpResponse response, string message)
        {
            response.Headers.Add("Application-error", message);
            response.Headers.Add("Access-Control-Expose-Headers", "Application-error");
            response.Headers.Add("Access-Control-Allow-Origin", "*");
        }
    }
}
