using NServiceBus;
using System.Threading.Tasks;
using NServiceBus.Logging;
using System.Threading;
using OpenTelemetry;
using System.Diagnostics;


#region Handler
public class CommandMessageHandler :
    IHandleMessages<Command>
{
    static ILog log = LogManager.GetLogger<CommandMessageHandler>();

    public Task Handle(Command message, IMessageHandlerContext context)
    {
        log.Info("Hello from CommandMessageHandler: " + message.Id);

        foreach (var item in Baggage.GetBaggage())
        {
            log.Info("Baggage mottaget:");
            log.Info(item.Key);
            log.Info(item.Value);
        }

        foreach (var item in Activity.Current.Baggage)
        {
            log.Info("ActivityBaggage mottaget:");
            log.Info(item.Key);
            log.Info(item.Value);
        }

        // sleep for a random amount between 50 and 500 ms
        Thread.Sleep(new System.Random().Next(50, 500));

        Task reply;
        if (message.Id%2 == 0)
        {
            reply = context.Reply(ErrorCodes.Fail);
        }
        else
        {
            reply = context.Reply(ErrorCodes.None);
        }
        return reply;
    }
}

#endregion