using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using NUnit.Framework;
using Samples.ASPNetCore3;
using Samples.ASPNetCore3.Controllers;
using Samples.ASPNetCore3.Models;
using Samples.ASPNetCore3.Services;

namespace RazorLight.Tests.AspNetCore3
{
  [TestFixture]
  public class SimpleWebCallTests
  {
    private TestServer _testServer;
    private HttpClient _testClient;

    public void Setup()
    {
      var webHostBuilder = new WebHostBuilder()
        .UseEnvironment("Development")
        .UseStartup(typeof(Startup));

      _testServer = new TestServer(webHostBuilder);
      _testClient = _testServer.CreateClient();
    }

    [Test]
    public async Task PostToRazorController_GivenValidModelAndTemplate_ShouldReturnValidResult()
    {
      //setup
      Setup();
      var data = new {name = "TheName", date = DateTime.Now, number = 99};
      var model = new RazorModel()
      {
        Data = JsonConvert.SerializeObject(data),
        Template = "Name=@Model.name"
      };
      var expected = $"Name={data.name}";

      //action
      var response = await _testClient.PostAsync("api/razor", new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json"));

      //assert
      Assert.IsTrue(response.IsSuccessStatusCode);
      Assert.AreEqual(await response.Content.ReadAsStringAsync(), expected);
    }

    [Test]
    public async Task CallRazorController__GivenValidModelAndTemplate_ShouldReturnValidResult()
    {
      //setup
      var service = new RazorService();
      var controller = new RazorController(service);
      var data = new { name = "TheName", date = DateTime.Now, number = 99 };
      var model = new RazorModel()
      {
        Data = JsonConvert.SerializeObject(data),
        Template = "Name=@Model.name"
      };
      var expected = $"Name={data.name}";

      //action
      var response = controller.Post(model) as ContentResult;
      var result = response.Content;

      //assert
      Assert.AreEqual(expected, result);
    }
  }
}
