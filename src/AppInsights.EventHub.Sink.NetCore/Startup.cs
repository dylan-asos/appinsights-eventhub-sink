using System;
using AppInsights.EventHub.Sink.NetCore.Telemetry.Configuration;
using AppInsights.EventHub.Sink.NetCore.Telemetry.Sink;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AppInsights.EventHub.Sink.NetCore
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            ConfigureApplicationInsights(services);
        }

        private static void ConfigureApplicationInsights(IServiceCollection services)
        {
            var options = new ApplicationInsightsServiceOptions
            {
                InstrumentationKey = "your-app-insights-key",
                EnableAdaptiveSampling = false
            };
            services.AddApplicationInsightsTelemetry(options);

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

            var processorBuilder = TelemetryConfiguration.Active.TelemetryProcessorChainBuilder;
            processorBuilder.Use((next) => new EventHubsSink(eventHubSinkSettings, next));
            processorBuilder.Build();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
                app.UseHsts();

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}