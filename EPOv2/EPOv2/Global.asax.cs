using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using EPOv2.Business.Interfaces;
using EPOv2.BusinessLayer;
using Hangfire;

namespace EPOv2
{
    using Serilog;
    using Serilog.Events;
    using Serilog.Exceptions;
    using Serilog.Sinks.Email;

    using SerilogWeb.Classic;
    using SerilogWeb.Classic.Enrichers;

    public class MvcApplication : System.Web.HttpApplication
    {
        
       
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            // WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            //RecurringJob.AddOrUpdate("MyJob", () => _data.ReApplySubstitutions(), "*/2 * * * *");
            ClsJobScheduler.Start();
            //RecurringJob.AddOrUpdate("MyJob1", () => _data.DeleteExpiredSubstitutionsEveryday(), "*/2 * * * *");
            log4net.Config.XmlConfigurator.Configure();

            var info = new EmailConnectionInfo()
            {
                EmailSubject = "EPOv2.Logging",
                FromEmail = "epo@oneharvest.com.au",
                MailServer = "mail",
                //NetworkCredentials = new NetworkCredential("mandrill_username", "mandrill_apikey"),
                Port = 25,
                ToEmail = "errorlogs@oneharvest.com.au",
            };
            ApplicationLifecycleModule.IsEnabled = false;
            ApplicationLifecycleModule.RequestLoggingLevel = LogEventLevel.Verbose;
            ApplicationLifecycleModule.LogPostedFormData = LogPostedFormDataOption.OnlyOnError;

            var logger = new LoggerConfiguration()
                    .WriteTo.Email(info, restrictedToMinimumLevel: LogEventLevel.Error)
                    .WriteTo.Seq(serverUrl: "http://tfs:5341/", apiKey: "adZNYzB8LDH297SAJSh4")
                    .Enrich.WithProperty("ApplicationName", "EPOv2")
                    //.Enrich.WithProperty("Environment", ConfigurationManager.AppSettings["Environment"])
                    .Enrich.FromLogContext()
                    .Enrich.With(new HttpRequestIdEnricher())
                    .Enrich.With<UserNameEnricher>()
                    //.Enrich.WithMachineName()
                    .Enrich.WithExceptionDetails()
                    .Enrich.With(new HttpRequestClientHostIPEnricher(true))
                    .Enrich.With(new HttpRequestClientHostNameEnricher())
                    .Enrich.With(new HttpRequestRawUrlEnricher())
                    .Enrich.With(new HttpRequestUrlReferrerEnricher())
                    .Enrich.With(new HttpRequestUserAgentEnricher())
                    .CreateLogger();


            Log.Logger = logger;
          
        }
    }
}
