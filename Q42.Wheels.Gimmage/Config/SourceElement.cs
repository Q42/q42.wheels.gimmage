using System.Configuration;
using Q42.Wheels.Gimmage.Interfaces;

namespace Q42.Wheels.Gimmage.Config
{
  public class SourceElement : ConfigurationElement, ISource
  {
    [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
    public string Name
    {
      get
      {
        return this["name"] as string;
      }
    }
    
    public string Source
    {
      get
      {
        if (Type == SourceType.file)
        {
          return ConfigTools.GetDirectory(Directory, true).FullName;
        }
        return Directory;
      }
    }

    [ConfigurationProperty("dir", IsRequired = true)]
    public string Directory
    {
      get
      {
        return this["dir"] as string;
      }
    }

    public SourceType Type
    {
      get
      {
        if (Directory.ToLower().StartsWith("http://") || Directory.ToLower().StartsWith("https://"))
          return SourceType.http;
        if (Directory.ToLower().StartsWith("ftp://"))
          return SourceType.ftp;
        if (Directory.ToLower().StartsWith("\\"))
          return SourceType.share;
        return SourceType.file;
      }
    }
  }
}
