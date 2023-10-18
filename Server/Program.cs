using System;
using System.Threading.Tasks;
using OpenTelemetry;
using OpenTelemetry.Trace;
using NServiceBus;
using OpenTelemetry.Resources;
using OpenTelemetry.Extensions.Hosting;
using System.Collections.Generic;
using System.Diagnostics;

class Program
{

    const string EndpointName = "NSBTest.Endpoint.Server";
    public static async Task Main()
    {
        var tracerProvider = Sdk.CreateTracerProviderBuilder()
        .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(EndpointName))
        //.SetResourceBuilder(ResourceBuilder.CreateDefault())
        .AddSource("NServiceBus.Core") //Tested "NServiceBus.*", "NServiceBus.Core", "*"
        .AddOtlpExporter()
        .Build();

        //Sdk.SetDefaultTextMapPropagator(new BaggagePropagator());

        var endpointConfiguration = new EndpointConfiguration(Console.Title = EndpointName);
        endpointConfiguration.EnableCallbacks(makesRequests: false);
        endpointConfiguration.UseTransport(new LearningTransport());
        //var transport = endpointConfiguration.UseTransport<RabbitMQTransport>();
        //transport.UseConventionalRoutingTopology(QueueType.Quorum);
        //transport.ConnectionString("host=10.20.10.30;username=guest;password=guest");
        endpointConfiguration.EnableOpenTelemetry();
        endpointConfiguration.EnableInstallers();


        var endpointInstance = await Endpoint.Start(endpointConfiguration);
        Console.WriteLine("Press any key to exit");
        Console.ReadKey();
        await endpointInstance.Stop();
    }
}
