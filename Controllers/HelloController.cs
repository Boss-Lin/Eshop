using Microsoft.AspNetCore.Mvc;

namespace EShop.Controllers;

[ApiController]
[Route("[controller]")]
public class HelloController:ControllerBase
{
    [HttpGet]
    public IActionResult GetHelloWorld()
    {
        return Ok(new { 
            data = "Hello World from Controller!",
            timestamp = DateTime.Now 
        });
    }
}