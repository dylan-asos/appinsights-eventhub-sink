using System;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using AppInsights.EventHub.Sink.NetFramework.Telemetry.Configuration;
using AppInsights.EventHub.Sink.NetFramework.Telemetry.Sink;
using Microsoft.ApplicationInsights.Extensibility;

namespace AppInsights.EventHub.Sink.NetFramework
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            ConfigureApplicationInsights();
        }

        private static void ConfigureApplicationInsights()
        {
            var config = TelemetryConfiguration.Active;

            var builder = config.TelemetryProcessorChainBuilder;

            var eventHubSinkSettings = new EventHubSinkSettings
            {
                KeyName = "your-key-name",
                AccessKey = "your-access-key",                
                EventHubUri = new Uri("https://eventhub-namespace.servicebus.windows.net/event-hub-name"),
                BatchBufferSettings = new BatchBufferConfig()
                {
                    MaxBacklog = 1000,
                    MaxWindowCount = 1000,
                    BatchFlushInterval = TimeSpan.FromSeconds(10)
                }                
            };

            builder.Use((next) => new EventHubsSink(eventHubSinkSettings, next)).Build();
        }
    }
}
