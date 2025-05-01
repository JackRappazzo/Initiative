using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Initiative.Api.Controllers
{
    [Route("api/admin")]
    [ApiController]
    public class AdminController : ControllerBase
    {

        public AdminController() { }


        [HttpPost]
        public IActionResult Register([FromBody] object data)
        {
            return NotFound();
        }

        [HttpPost]
        public IActionResult Login([FromBody] object data)
        {
            return NotFound();
        }
    }
}
