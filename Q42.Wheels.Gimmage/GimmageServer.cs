using System.Web.Routing;
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
    public static void RegisterRoutes(RouteCollection routes)
    {
      routes.Add(new GimmageRoute());
    }
  }
}
