using System.Web.Routing;
using Q42.Wheels.Gimmage.Interfaces;

namespace Q42.Wheels.Gimmage
{
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

    public static void RegisterRoutes(RouteCollection routes)
    {
      routes.Add(new GimmageRoute());
    }
  }
}
