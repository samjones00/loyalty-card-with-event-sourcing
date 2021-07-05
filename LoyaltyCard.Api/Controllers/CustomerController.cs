using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using LoyaltyCard.Domain.Contracts.User;
using LoyaltyCard.Domain.Interfaces;

namespace LoyaltyCard.Api.Controllers
{
    [ApiController]
    [Route("Customer")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _service;

        public CustomerController(ILogger<CustomerController> logger, ICustomerService service)
        {
            _service = service;
        }

        [HttpPost]
        public Task<IActionResult> Create([FromBody] Create cmd) => Handle(cmd);

        [HttpPut]
        public Task<IActionResult> Update([FromBody] ChangeName cmd) => Handle(cmd);

        [HttpDelete]
        public Task<IActionResult> Delete([FromBody] Delete cmd) => Handle(cmd);

        async Task<IActionResult> Handle<T>(T cmd) where T : class
        {
            //Log.Information(cmd.ToString());
            await _service.Handle(cmd);
            return Ok();
        }
    }
}
