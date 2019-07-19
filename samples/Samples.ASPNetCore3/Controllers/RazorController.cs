using Microsoft.AspNetCore.Mvc;
using Samples.ASPNetCore3.Services;

namespace Samples.ASPNetCore3.Controllers
{
  [ApiController]
  public class RazorController: ControllerBase
  {
    private readonly IRazorService _razorService;

    public RazorController(IRazorService razorService)
    {
      _razorService = razorService;
    }

    [HttpPost]
    [Route("/api/razor")]
    public IActionResult Post([FromBody] dynamic model)
    {
      var result = _razorService.Convert(model, "Item name: @Model.name");
      return new JsonResult(new {Result=result});
    }
  }
}
