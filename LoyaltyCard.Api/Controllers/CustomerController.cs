using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using LoyaltyCard.Domain.Contracts.Customer;
using LoyaltyCard.Domain.Interfaces;

namespace LoyaltyCard.Api.Controllers
{
    [ApiController]
    [Route("Customer")]
    public class CustomerController : ControllerBase
    {
        private readonly IQueueService _queueService;

        public CustomerController(ILogger<CustomerController> logger, IQueueService queueService)
        {
            _queueService = queueService;
        }

        [HttpPost]
        public Task<IActionResult> Create([FromBody] CreateCustomer cmd) => Handle(cmd);

        [HttpPut]
        public Task<IActionResult> Update([FromBody] ChangeCustomerName cmd) => Handle(cmd);

        [HttpDelete]
        public Task<IActionResult> Delete([FromBody] Delete cmd) => Handle(cmd);

        async Task<IActionResult> Handle<T>(T cmd) where T : class
        {
            await _queueService.Enqueue(cmd);

            //Log.Information(cmd.ToString());
           // await _service.Handle(cmd);
            return Ok();
        }
    }
}
