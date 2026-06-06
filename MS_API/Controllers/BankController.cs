using Microsoft.AspNetCore.Mvc;
using MS_Application.Services.Interfaces;

namespace MS_API.Controllers
{
    [ApiController]
    [Route("api/bank")]
    public class BankController : BaseController
    {
        private readonly IBankService _bankService;

        public BankController(IBankService bankService) 
        {
            _bankService = bankService;
        }

        [HttpGet]
        public async Task<IActionResult> GetBanks()
        {
            var result = await _bankService.GetBanks();
            return Ok(result);
        }
    }
}
