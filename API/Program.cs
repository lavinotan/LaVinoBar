using System;
using API.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace API
{
    public class Program
    {
        public static void Main(string[] args)
        {
           var host = CreateHostBuilder(args).Build();
           using var scope = host.Services.CreateScope(); // dispose any garbage collections by using
           var context = scope.ServiceProvider.GetRequiredService<StoreContext>(); // initialize database
           var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>(); // log exception on terminal
        
           try
           {
               context.Database.Migrate(); // create table of database
               DbInitializer.Initialzize(context); // add default products
           }
           catch (Exception ex)
           {
               logger.LogError(ex, "Problem migrating data");
           }
           
           host.Run();
        }   

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
