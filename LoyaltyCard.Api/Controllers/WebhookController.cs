using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using LoyaltyCard.Domain.Contracts.Customer;
using LoyaltyCard.Domain.Interfaces;
using Newtonsoft.Json.Linq;

namespace LoyaltyCard.Api.Controllers
{
    [ApiController]
    [Route("Webhook")]
    public class WebhookController : Controller
    {
        private readonly IQueueService _queueService;

        public WebhookController(IQueueService queueService)
        {
            _queueService = queueService;
        }

        [HttpPost]
        public void Handle([FromBody] string request)
        {
           
        }
    }
}
