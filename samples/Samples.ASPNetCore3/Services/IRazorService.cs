using System;
using System.Dynamic;
using System.IO;
using System.Reflection;
using log4net;
using Newtonsoft.Json;
using RazorLight;
using RazorLight.Razor;

namespace Samples.ASPNetCore3.Services
{
  public interface IRazorService
  {
    void Convert(dynamic data, Stream template, TextWriter stringWriter);
    string Convert(dynamic data, string templateData);
  }

  public class RazorService : IRazorService
  {
    private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    private readonly RazorLightEngine engine;

    public RazorService()
    {
      RazorLightProject project = new EmbeddedRazorProject(Assembly.GetExecutingAssembly());

      engine = new RazorLightEngineBuilder()
        .UseProject(project)
        .UseMemoryCachingProvider()
        .Build();
    }

    #region Implementation of ITemplateEngine

    public void Convert(dynamic data, Stream template, TextWriter stringWriter)
    {
      string readToEnd = template.ReadToString();
      try
      {
        stringWriter.Write(Convert(data, readToEnd));
        stringWriter.Flush();
      }
      catch (Exception e)
      {
        _log.ErrorFormat("Invalid JSON for STREAM template: {0}\n\n{1}", JsonConvert.SerializeObject(data), readToEnd);
        throw;
      }
    }

    public string Convert(dynamic data, string templateData)
    {
      try
      {
        if (templateData == null || data == null) return string.Empty;
        templateData = $"@using System;@using System.Linq;@using System.Collections.Generic\n{templateData}";
        var result = engine.CompileRenderStringAsync(GetTemplateCachedId(templateData), templateData, data, (ExpandoObject)null).Result;
        return result;
      }
      catch (Exception e)
      {
        _log.Debug(e);
        _log.ErrorFormat("Invalid JSON for STRING template: {0}\n\n{1}", JsonConvert.SerializeObject(data), templateData);
        throw;
      }
    }

    #endregion

    #region Private Methods

    private string GetTemplateCachedId(string templateData)
    {
      return "CH" + templateData.GetHashCode();
    }

    #endregion
  }

  public static class Helpers
  {
    public static string ReadToString(this Stream stream)
    {
      if (stream == null) return null;
      var streamReader = new StreamReader(stream);
      var readToString = streamReader.ReadToEnd();
      return readToString;
    }
  }

}
