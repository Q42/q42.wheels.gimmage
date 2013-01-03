namespace Q42.Wheels.Gimmage.Interfaces
{
  public interface IImageTemplate
  {
    System.Collections.Generic.List<Q42.Wheels.Gimmage.ImageManipulation.ImageFilters.Filter> Filters { get; }
    string Name { get; }
  }
}
