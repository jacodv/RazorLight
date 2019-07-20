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

      //action
      var result = await _engine.CompileRenderStringAsync(GetTemplateCachedId(template), template, model, (ExpandoObject)null);

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
