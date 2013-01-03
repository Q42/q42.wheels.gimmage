using System.Web.Mvc;

namespace Q42.Wheels.Gimmage
{
  public class GimmageController : System.Web.Mvc.Controller
  {
    public ActionResult Index(string fileName, string source, string template)
    {
      GimmageModel model = new GimmageModel { FileName = fileName, SourceStr = source, TemplateStr = template };

      return new GimmageResult(model);
    }
  }
}
