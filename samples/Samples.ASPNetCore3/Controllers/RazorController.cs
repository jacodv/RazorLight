using System;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Samples.ASPNetCore3.Models;
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
    public IActionResult Post([FromBody] RazorModel model)
    {

      try
      {
        var result = _razorService.Convert(JsonConvert.DeserializeObject<dynamic>(model.Data), model.Template);
        return new ContentResult() { ContentType = "text/plain", Content = result };
      }
      catch (Exception ex)
      {
        return new ContentResult() { ContentType = "text/plain", Content = ex.ToString() };
      }
    }
  }
}
