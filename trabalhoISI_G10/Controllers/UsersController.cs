using Microsoft.AspNetCore.Mvc;

namespace trabalhoISI_G10.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        [HttpGet(Name = "GetUser")]

        [HttpPost(Name = "CreateUser")]
    }
}