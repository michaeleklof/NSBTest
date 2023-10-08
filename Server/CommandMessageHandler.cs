using NServiceBus;
using System.Threading.Tasks;
using NServiceBus.Logging;
using System.Threading;

#region Handler

public class CommandMessageHandler :
    IHandleMessages<Command>
{
    static ILog log = LogManager.GetLogger<CommandMessageHandler>();

    public Task Handle(Command message, IMessageHandlerContext context)
    {
        log.Info("Hello from CommandMessageHandler");
        
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