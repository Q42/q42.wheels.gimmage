using System.Collections.Generic;
using System.Drawing;

namespace Q42.Wheels.Gimmage.ImageManipulation
{
  public class Manipulate
  {
    public static Bitmap Apply(Bitmap input, List<ImageFilters.Filter> filters)
    {
      foreach (ImageFilters.Filter filter in filters)
        input = filter.Invoke(input);
      return input;
    }

    public static Bitmap Apply(Bitmap input, ImageFilters.Filter filter)
    {
      List<ImageFilters.Filter> filters = new List<ImageFilters.Filter>(1);
      filters.Add(filter);
      return Apply(input, filters);
    }

    public static Bitmap Apply(string input, ImageFilters.Filter filter)
    {
      return Apply(new Bitmap(input), filter);
    }

    public static Bitmap Apply(string input, List<ImageFilters.Filter> filters)
    {
      return Apply(new Bitmap(input), filters);
    }
  }
}
