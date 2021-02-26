using Microsoft.AspNetCore.Mvc;
using Skinet.Infrastructure.Data;
using Skinet.WebApi.Errors;

namespace Skinet.WebApi.Controllers
{
    public class BuggyController : BaseController
    {
        private readonly StoreContext _context;

        public BuggyController(StoreContext context)
        {
            _context = context;
        }

        [HttpGet("notfound")]
        public ActionResult GetNotFoundRequest()
        {
            return NotFound(new ApiResponse(404));
        }

        [HttpGet("servererror")]
        public ActionResult GetServerErrorRequest()
        {
            var thing = _context.Products.Find(42);

            var thingToReturn = thing.ToString();

            return Ok();
        }

        [HttpGet("badrequest")]
        public ActionResult GetBadRequestRequest()
        {
            return BadRequest(new ApiResponse(400));
        }

        [HttpGet("badrequest/{id}")]
        public ActionResult GetValidationRequest(int id)
        {
            return Ok();
        }
    }
}
