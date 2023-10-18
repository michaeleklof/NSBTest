using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus;
using OpenTelemetry;
using OpenTelemetry.Trace;
using OpenTelemetry.Resources;
using OpenTelemetry.Context.Propagation;
using OpenTelemetry.Extensions.Hosting;
using NServiceBus.Transport.RabbitMQ;

class Program
{

    const string EndpointName = "NSBTest.Endpoint.WebApp";
    public static void Main()
    {
        #region ApplicationStart
        var builder = WebApplication.CreateBuilder();

        var tracerProvider = Sdk.CreateTracerProviderBuilder()
        //.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(EndpointName))
        .SetResourceBuilder(ResourceBuilder.CreateDefault())
        .AddSource("NServiceBus.Core") //Tested "NServiceBus.*", "NServiceBus.Core", "*"
        .AddOtlpExporter()
        .Build();

        //Sdk.SetDefaultTextMapPropagator(new BaggagePropagator());

        builder.Host.UseNServiceBus(context =>
        {
            var endpointConfiguration = new EndpointConfiguration(EndpointName);
            endpointConfiguration.MakeInstanceUniquelyAddressable("1");
            endpointConfiguration.EnableCallbacks();
            endpointConfiguration.UseTransport(new LearningTransport());
            //var transport = endpointConfiguration.UseTransport<RabbitMQTransport>();
            //transport.UseConventionalRoutingTopology(QueueType.Quorum);
            //transport.ConnectionString("host=10.20.10.30;username=guest;password=guest");
            endpointConfiguration.EnableOpenTelemetry();
            endpointConfiguration.EnableInstallers();
            return endpointConfiguration;
        });
        #endregion

        builder.Services.AddRazorPages();

        var app = builder.Build();

        app.MapRazorPages();

        app.Run();

    }
}