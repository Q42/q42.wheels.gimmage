using System.Web;
using System.Web.Mvc;
using System.Diagnostics;

namespace Q42.Wheels.Gimmage
{
  public class GimmageResult : ActionResult
  {
    private GimmageModel model;

    public GimmageResult(GimmageModel model)
      : base()
    {
      this.model = model;
    }

    // TODO: content-type
    [DebuggerStepThrough] // Ignore FileNotFound exceptions
    public override void ExecuteResult(ControllerContext context)
    {
      GimmageServer.Server.ServeImage(model.FileName, model.SourceStr, model.TemplateStr, HttpContext.Current.Response, HttpContext.Current.Request);
    }
  }
}
