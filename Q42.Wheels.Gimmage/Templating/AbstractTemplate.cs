using System.Collections.Generic;
using Q42.Wheels.Gimmage.Interfaces;

namespace Q42.Wheels.Gimmage.Templating
{
  public abstract class AbstractTemplate : IImageTemplate
  {
    protected AbstractTemplate() { }

    public abstract List<ImageManipulation.ImageFilters.Filter> Filters { get; }

    public virtual string Name { get { return this.GetType().Name.Replace("Template", "").ToLower(); } }
  }
}
