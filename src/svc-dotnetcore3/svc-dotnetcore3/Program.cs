using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using Serilog;

namespace Web.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateLogger();
            try
            {
                Log.Logger.Here().Information("Starting web host");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception err)
            {
                Log.Logger.Here().Fatal(err, "Host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog() // <-- Add this line;
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        public static void CreateLogger() {
            var outputTemplate = "[{Timestamp:HH:mm:ss} {Level}]:{NewLine}  {SourceContext}{NewLine}  {Message}{NewLine}  Method: [{MemberName}] at [{FilePath}:{LineNumber}]:{NewLine}  {Exception}{NewLine}";
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft.AspNetCore", Serilog.Events.LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.File(@"Logs\log_.txt", rollingInterval: RollingInterval.Day, outputTemplate: outputTemplate)
                .WriteTo.Console(restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information, outputTemplate: outputTemplate)
                // .WriteTo.MSSqlServer(
                //     loggingConnectionStringBuilder.ConnectionString, 
                //     "Ibex", 
                //     Serilog.Events.LogEventLevel.Information)
                // .WriteTo.AzureAnalytics(
                //     workspaceId: Configuration["AZUREANALYTICS_WORKSPACEID"],
                //     authenticationId: Configuration["AZUREANALYTICS_AUTHENTICATIONID"],
                //     logName: "ibex",
                //     restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Debug, 
                //     batchSize: 10)
                // .WriteTo.Email( 
                //     //notice that we should only email the most urgent messages
                //     restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Fatal, 
                //     connectionInfo: new EmailConnectionInfo()
                //     {
                //         MailServer = "mercury.nbsuply.com",
                //         FromEmail = "serilog@hmwallace.com",
                //         ToEmail = "",
                //         EmailSubject = "Serilog subject",
                //     })
                .CreateLogger();
        }
    }
}
