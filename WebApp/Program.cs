using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus;
using OpenTelemetry;
using OpenTelemetry.Trace;
using OpenTelemetry.Resources;
using OpenTelemetry.Extensions.Hosting;

class Program
{

    const string EndpointName = "Samples.AsyncPages.WebApplication";
    public static void Main()
    {
        #region ApplicationStart
        var builder = WebApplication.CreateBuilder();

        var tracerProvider = Sdk.CreateTracerProviderBuilder()
.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(EndpointName))
.AddSource("NServiceBus.*")
.AddOtlpExporter()
.Build();

        builder.Host.UseNServiceBus(context =>
        {
            var endpointConfiguration = new EndpointConfiguration(EndpointName);
            endpointConfiguration.MakeInstanceUniquelyAddressable("1");
            endpointConfiguration.EnableCallbacks();
            endpointConfiguration.UseTransport(new LearningTransport());
            endpointConfiguration.EnableOpenTelemetry();
            return endpointConfiguration;
        });
        #endregion

        builder.Services.AddRazorPages();

        var app = builder.Build();

        app.MapRazorPages();

        app.Run();
    }
}