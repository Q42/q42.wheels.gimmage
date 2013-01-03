using System.IO;
using Q42.Wheels.Gimmage.Templating;

namespace Q42.Wheels.Gimmage.Tooling
{
  public class Cache
  {
    public static void RemoveCachedFiles(string cachePath, AbstractTemplate template)
    {
      if (!Directory.Exists(cachePath))
        throw new DirectoryNotFoundException("Cache directory not found.\n" + cachePath);

      string fileFilter = "*.*_*";
      if (template.Name != null)
        fileFilter = "*.*_" + template.Name;

      DirectoryInfo dir = new DirectoryInfo(cachePath);
      foreach (FileInfo file in dir.GetFiles(fileFilter, SearchOption.AllDirectories))
        file.Delete();
    }
  }
}
