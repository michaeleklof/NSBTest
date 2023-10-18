using System;
using System.Diagnostics;
using OpenTelemetry;

public class Program
{

    public static string testClass()
    {
        foreach (var item in Baggage.Current)
        {
            Console.WriteLine("RESTAPI Baggage mottaget:");
            Console.WriteLine(item.Key);
            Console.WriteLine(item.Value);
        }

        foreach (var item in Activity.Current.Baggage)
        {
            Console.WriteLine("RESTAPI ActivityBaggage mottaget:");
            Console.WriteLine(item.Key);
            Console.WriteLine(item.Value);
        }

        return "Call method executed successfully";
    }

    static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var app = builder.Build();

        app.MapGet("/API", testClass);

        app.Run();
    }
}
