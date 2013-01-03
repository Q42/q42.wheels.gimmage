using System.Web.Mvc;
using System.Web.Routing;

namespace Q42.Wheels.Gimmage
{
  public class GimmageRoute : Route
  {
    public GimmageRoute()
      : base("gimmage/{source}/{template}/{*filename}", new RouteValueDictionary(new { controller = "Gimmage", action = "Index" }), new MvcRouteHandler())
    {
    }

    /// <summary>
    /// use this specific gimmageroute to redesign your urls
    /// </summary>
    /// <param name="customRoute"></param>
    /// <param name="defaultSource"></param>
    /// <param name="defaultTemplate"></param>
    public GimmageRoute(string customRoute, string defaultSource, string defaultTemplate)
      : base(customRoute, new RouteValueDictionary(new { controller = "Gimmage", action = "Index", template = defaultTemplate, source = defaultSource }), new MvcRouteHandler())
    {
    }
  }
}
