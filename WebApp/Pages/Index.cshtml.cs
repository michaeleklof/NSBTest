using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NServiceBus;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;

namespace WebApp.Pages
{
    [IgnoreAntiforgeryToken]
    public class IndexModel : PageModel
    {
        IMessageSession messageSession;

        public string ResponseText { get; set; }

        public IndexModel(IMessageSession messageSession)
        {
            this.messageSession = messageSession;
        }

        public async Task<IActionResult> OnPostAsync(string textField)
        {
            if (string.IsNullOrWhiteSpace(textField))
            {
                return Page();
            }

            #region ActionHandling

            if (!int.TryParse(textField, out var number))
            {
                return Page();
            }
            var command = new Command
            {
                Id = number
            };

            
            //Baggage.Current.SetBaggage("testbaggage", "testvalue");
            Activity.Current.AddBaggage("activitytestbaggage", "activitytestvalue");

            var sendOptions = new SendOptions();
            sendOptions.SetDestination("NSBTest.Endpoint.Server");

            //Call RESTAPI
            var client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync("http://localhost:5050/API");
            
            
            

            var code = await messageSession.Request<ErrorCodes>(command, sendOptions);
            ResponseText = Enum.GetName(typeof(ErrorCodes), code);

            foreach (var item in Baggage.GetBaggage())
            {
                Console.WriteLine("Baggage efter det kommer tillbaka:");
                Console.WriteLine(item.Key);
                Console.WriteLine(item.Value);
            }


            return Page();
            #endregion
        }
    }
}
