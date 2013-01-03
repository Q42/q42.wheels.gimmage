using System.Collections.Generic;
using Q42.Wheels.Gimmage.ImageManipulation;

namespace Q42.Wheels.Gimmage.Templating
{
  public class DefaultTemplate : AbstractTemplate
  {
    public override List<ImageFilters.Filter> Filters
    {
      get
      {
        return new List<ImageFilters.Filter>();
      }
    }
  }
}