using System.Collections.Generic;
using Q42.Wheels.Gimmage.ImageManipulation;

namespace Q42.Wheels.Gimmage.Templating
{
  /// <summary>
  /// Default template which applies no filters on the original image
  /// </summary>
  public class DefaultTemplate : AbstractTemplate
  {
    /// <summary>
    /// The list of filters (Empty)
    /// </summary>
    public override List<ImageFilters.Filter> Filters
    {
      get
      {
        return new List<ImageFilters.Filter>();
      }
    }
  }
}