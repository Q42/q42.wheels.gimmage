using System.Collections.Generic;
using Q42.Wheels.Gimmage.Interfaces;

namespace Q42.Wheels.Gimmage.Templating
{
  /// <summary>
  /// Base abstract template to extend all templates from
  /// </summary>
  public abstract class AbstractTemplate : IImageTemplate
  {
    /// <summary>
    /// empty constructor
    /// </summary>
    protected AbstractTemplate() { }

    /// <summary>
    /// The list of filters to apply to the original image
    /// </summary>
    public abstract List<ImageManipulation.ImageFilters.Filter> Filters { get; }

    /// <summary>
    /// Name of this template
    /// </summary>
    public virtual string Name { get { return this.GetType().Name.Replace("Template", "").ToLower(); } }
  }
}
