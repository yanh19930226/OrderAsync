using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderAsyncWebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //DbHelper.CreateConnection("server=121.196.186.210;port=3309;user=root;password=123456;database=testerp3;charset=utf8mb4;Allow User Variables=True", "MySql.Data.MySqlClient");

            //DbHelper.CreateConnection("server=pc-bp1nd5wvp3w9pi9z9.rwlb.rds.aliyuncs.com;port=3306;user=xc1;password=X9t^WTPSSm@;database=test1;charset=utf8;Allow User Variables=True");

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
