using System;
using System.Dynamic;
using System.Reflection;
using System.Threading.Tasks;
using NUnit.Framework;
using RazorLight.Razor;

namespace RazorLight.Tests.NetCore3
{
  [TestFixture]
  public class SimpleRazorAsStringTests
  {
    private readonly RazorLightEngine _engine;

    public SimpleRazorAsStringTests()
    {
      RazorLightProject project = new EmbeddedRazorProject(Assembly.GetExecutingAssembly());
      _engine = new RazorLightEngineBuilder()
        .UseProject(project)
        .UseMemoryCachingProvider()
        .Build();
    }

    [Test]
    public async Task CompileAsync_GivenSimpleTemplateAndModel_ShouldCompileAndReturnValidResult()
    {
      //setup
      dynamic model = new {name = "TheName", date = DateTime.Now, number=999};
      var template = "Name=@Model.name";
      var expected = $"Name={model.name}";
      var cacheKey = GetTemplateCachedId(template);
      string result;
      //action
      try
      {
        result = await _engine.CompileRenderAsync(cacheKey, model, (ExpandoObject) null);
      }
      catch (Exception e)
      {
        Console.WriteLine(e);
        try
        {
          result = await _engine.CompileRenderStringAsync(cacheKey, template, model, (ExpandoObject)null);
        }
        catch (Exception exception)
        {
          Console.WriteLine(exception);
          throw;
        }
      }

      //assert
      Assert.AreEqual(result,expected);
    }

    #region Private
    private string GetTemplateCachedId(string templateData)
    {
      return "CH" + templateData.GetHashCode();
    }
    #endregion
  }
}
