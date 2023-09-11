using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Infrastructure;
using tp24TT.Models.Request;
using tp24TT.Models.Response;
using tp24TT.Services;

namespace tp24TT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReceivablesController : ControllerBase
    {
        private readonly IReceivablesService _receivablesService;
        public ReceivablesController(IReceivablesService receivablesService)
        {
            _receivablesService = receivablesService;
        }


        [HttpPost]
        public async Task<IActionResult> Payload(ReceivableRequest request)
        {
            if (request == null)
            {
                return BadRequest("Invalid request object, you done goofed");
            }

            try
            {
                var payloadResponse = await _receivablesService.SendPayload(request);

                if (!payloadResponse.Errors.Any())
                {
                    // Return a 201 Created status code and the payloadResponse in the response
                    return Created("payload stored", payloadResponse);
                }

                // If there are errors in the payloadResponse, return a BadRequest with the payloadResponse
                return BadRequest(payloadResponse);
            }
            catch (Exception)
            {
                return BadRequest("An error occurred please scream at your machine until it does what you want it to. ");
            }
        }



        [HttpGet("summary")]
        public async Task<ReceivablesSummaryResponse> Summary()
        {
            return await _receivablesService.GetReceivablesSummary();
        }
    }
}
