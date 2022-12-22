using Microsoft.AspNetCore.Mvc;

namespace trabalhoISI_G10.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RoomsController : ControllerBase
    {
        [HttpGet(Name = "GetRooms")]
    }
}