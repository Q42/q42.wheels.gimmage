GET IT RUNNING
=======
To initialize Gimmage in your MVC application append the following route to your global.asax:

protected void Application_Start(){
	GimmageServer.RegisterRoutes(routes);
}

This will initialize Gimmage and register the default route /gimmage/source/template/filename for use

DEPENDENCYS
=======
Gimmage depends on log4net 1.2.10+

EXTEND IT
=======
To create your own Image scaling templates create a class (public) which extends Q42.Wheels.Gimmage.Templating.AbstractTemplate

Example code
public class FourCols : AbstractTemplate
{
  public override List<Q42.Wheels.Gimmage.ImageManipulation.ImageFilters.Filter> Filters
  {
    get
    {
      return new List<Q42.Wheels.Gimmage.ImageManipulation.ImageFilters.Filter>
      {
        ImageFilters.Scale(300, 800, false)
      };
    }
  }
}