using System;
using System.Threading.Tasks;
using OpenTelemetry;
using OpenTelemetry.Trace;
using NServiceBus;
using OpenTelemetry.Resources;
using OpenTelemetry.Extensions.Hosting;

class Program
{

    const string EndpointName = "Samples.AsyncPages.Server";
    public static async Task Main()
    {
        var tracerProvider = Sdk.CreateTracerProviderBuilder()
        .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(EndpointName))
        .AddSource("*") //Also tested "NServiceBus.*", "NServiceBus.Core"
        .AddOtlpExporter()
        .Build();

        var endpointConfiguration = new EndpointConfiguration(Console.Title = EndpointName);
        endpointConfiguration.EnableCallbacks(makesRequests: false);
        endpointConfiguration.UseTransport(new LearningTransport());
        endpointConfiguration.EnableOpenTelemetry();



        var endpointInstance = await Endpoint.Start(endpointConfiguration);
        Console.WriteLine("Press any key to exit");
        Console.ReadKey();
        await endpointInstance.Stop();
    }
}
