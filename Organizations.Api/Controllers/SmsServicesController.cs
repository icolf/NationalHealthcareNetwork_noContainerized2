using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Organizations.Api.SmsServices.SmsMessageValidation;
using Organizations.Api.SmsServices.SmsModels;
using Organizations.Api.SmsServices.SmsServices;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Organizations.Api.Controllers
{
    [Route("api/organizations")]
    public class SmsServicesController : ControllerBase
    {
        private readonly ISmsService _smsService;

        public SmsServicesController(ISmsService smsService)
        {
            this._smsService = smsService;
        }

        [HttpPost("sendMessage")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Send([FromBody]SmsMessage model)
        {
            var errors = ValidationService.Validate(out bool isValid, model);

            if (isValid)
            {
                var result = await _smsService.Send(model);

                return Ok(new GenericApiResponse(result));
            }
            else
            {
                return BadRequest(new GenericApiResponse(errors));
            }
        }
    }
}
