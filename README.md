# q42.wheels.gimmage

This repository contains an outdated image scaler for c# MVC. :warning: This code is no longer maintained! :warning: Here be dragons.

﻿GET IT RUNNING
=======
To initialize Gimmage in your MVC application append the following route to your global.asax:

```
protected void Application_Start(){
	GimmageServer.RegisterRoutes(routes);
}
```

This will initialize Gimmage and register the default route /gimmage/source/template/filename for use. Alternatively you can use this one to modify the url:
```
protected void Application_Start(){
  routes.MapRoute("Q42.Wheels.Gimmage", "gimmage/{source}/{template}/{*filename}",
    new { controller = "Gimmage", action = "Index" },
    new[] { typeof(GimmageController).Namespace });
}
```
But the Gimmage system will then be initialized upon first retrieval of an image instead of app-start

DEPENDENCYS
=======
Gimmage depends on log4net 1.2.10+

EXTEND IT
=======
To create your own Image scaling templates create a class (public) which extends Q42.Wheels.Gimmage.Templating.AbstractTemplate

Example code:

```
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
```
