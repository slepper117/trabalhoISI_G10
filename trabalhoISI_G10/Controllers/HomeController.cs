using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace trabalhoISI_G10.Controllers
{
    /// <summary>
    /// Home Controller
    /// </summary>
    [ApiController]
    [Route("/")]
    public class HomeController : ControllerBase
    {
        /// <summary>
        /// Index Root
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(200)]
        public ActionResult<string> Index()
        {
            return Ok("A API RESTfull está a funcionar...");
        }
    }
}
