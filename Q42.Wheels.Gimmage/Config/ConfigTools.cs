using System;
using System.IO;

namespace Q42.Wheels.Gimmage.Config
{
  internal static class ConfigTools
  {
    public static DirectoryInfo GetDirectory(string path, bool createDirectory)
    {
      if (string.IsNullOrEmpty(path))
        throw new ArgumentNullException(path + " has no path");

      if (path.StartsWith("~/"))
        path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path.Remove(0, 2));

      if (!Path.IsPathRooted(path))
        throw new ArgumentException(path + " path should be rooted");

      DirectoryInfo pathDir = new DirectoryInfo(path);
      if (!pathDir.Exists)
      {
        if (createDirectory)
          pathDir.Create();
        else
          throw new DirectoryNotFoundException(path + " directory not found");
      }
      return pathDir;
    }

  }
}
