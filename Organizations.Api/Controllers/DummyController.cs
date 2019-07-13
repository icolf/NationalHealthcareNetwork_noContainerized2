using Microsoft.AspNetCore.Mvc;
using Organizations.Api.Persistence;

namespace Organizations.Api.Controllers 
{
    public class DummyController : ControllerBase
    {
        private readonly OrganizationsContext _ctx;

        public DummyController(OrganizationsContext ctx)
        {
            _ctx = ctx;
        }

        [HttpGet]
        [Route("api/testDatabase")]
        public IActionResult testDatabase()
        {
            return Ok();
        }
    }
}
