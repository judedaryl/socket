using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace PRSMessaging
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            var port = Environment.GetEnvironmentVariable("PRS_MESS_PORT");
            if (string.IsNullOrWhiteSpace(port)) port = "5002";
            return WebHost.CreateDefaultBuilder(args)
            .UseStartup<Startup>()
            .UseUrls($"https://localhost:{port}");
        }
    }
}
