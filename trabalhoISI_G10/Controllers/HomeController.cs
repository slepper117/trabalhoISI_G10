using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace trabalhoISI_G10.Controllers
{
    /// <summary>
    /// Class for Root Routes
    /// </summary>
    [ApiController]
    [Route("/")]
    public class HomeController : ControllerBase
    {
        /// <summary>
        /// 
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
