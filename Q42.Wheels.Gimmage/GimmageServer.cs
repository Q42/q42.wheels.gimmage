using System.Web.Routing;
using System.Web.Mvc;
using Q42.Wheels.Gimmage.Interfaces;

namespace Q42.Wheels.Gimmage
{
  /// <summary>
  /// This class is the MVC implementation singleton for MVC
  /// </summary>
  public static class GimmageServer
  {
    private static readonly IHttpImageServer imageServer = new HttpImageServer();
    
    public static IHttpImageServer Server 
    {
      get
      {
        return imageServer;
      }
    }

    /// <summary>
    /// Register the default gimmage routes in your applications MVC route table
    /// </summary>
    /// <param name="routes"></param>
    /// http://codeclimber.net.nz/archive/2008/11/14/how-to-call-controllers-in-external-assemblies-in-an-asp.net.aspx
    public static void RegisterRoutes(RouteCollection routes)
    {
      routes.MapRoute("Q42.Wheels.Gimmage", "gimmage/{source}/{template}/{*filename}",
        new { controller = "Gimmage", action = "Index" },
        new[] { typeof(GimmageController).Namespace });
    }
  }
}
