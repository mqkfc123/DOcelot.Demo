using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DOcelot.APIGateway
{
    public class Program
    {
        public static string IP;
        public static int Port;

        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        //public static IWebHost BuildWebHost(string[] args) =>

        //    WebHost.CreateDefaultBuilder(args)
        //        .UseStartup<Startup>()
        //        .UseUrls($"http://{IP}:{Port}")
        //        .ConfigureAppConfiguration((hostingContext, builder) =>
        //                    {
        //                        builder.AddJsonFile("configuration.json", false, true);
        //                    })
        //        .Build();


        public static IWebHost BuildWebHost(string[] args)
        {
            var config = new ConfigurationBuilder()
                   .AddCommandLine(args)
                   .Build();

            IP = config["ip"];
            Port = Convert.ToInt32(config["port"]);
            if (string.IsNullOrEmpty(IP))
            {
                IP = "127.0.0.1";
            }
            if (Port == 0)
            {
                Port = 80;
            }

            return WebHost.CreateDefaultBuilder(args)
                          .UseStartup<Startup>()
                          .UseUrls($"http://{IP}:{Port}")
                          .ConfigureAppConfiguration((hostingContext, builder) =>
                          {
                              builder.AddJsonFile("configuration.json", false, true);
                          })
                          .Build();
        }
         
    }
}
