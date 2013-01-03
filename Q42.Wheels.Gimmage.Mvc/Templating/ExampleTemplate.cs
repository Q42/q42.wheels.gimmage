using System.Collections.Generic;
using Q42.Wheels.Gimmage.ImageManipulation;

namespace Q42.Wheels.Gimmage.Templating
{
  /// <summary>
  /// example template with some filters 
  /// </summary>
  public class ExampleTemplate : AbstractTemplate
  {
    public override List<ImageFilters.Filter> Filters
    {
      get
      {
        List<ImageFilters.Filter> filters = new List<ImageFilters.Filter>();

        filters.Add(ImageFilters.Scale(200, 200, true));
        filters.Add(ImageFilters.Rotate(20));
        filters.Add(ImageFilters.Overlay(300, 300, "#ff0000", horizontalAlign.center, verticalAlign.center));

        return filters;
      }
    }
  }
}