using System.Configuration;
using System.IO;

namespace Q42.Wheels.Gimmage.Config
{
  public sealed class CacheElement : ConfigurationElement
  {
    [ConfigurationProperty("enabled", IsRequired = false, DefaultValue = false)]
    public bool Enabled
    {
      get
      {
        return (bool)this["enabled"];
      }
    }

    [ConfigurationProperty("dir", IsRequired = false, DefaultValue = "~/cache")]
    protected string Directory
    {
      get
      {
        return this["dir"] as string;
      }
    }

    public DirectoryInfo SourceDir
    {
      get
      {
        return ConfigTools.GetDirectory(this.Directory, true);
      }
    }

    [ConfigurationProperty("expiresdays", IsRequired = false, DefaultValue = 0)]
    public int ExpiresDays
    {
      get
      {
        return (int)this["expiresdays"];
      }
    }

  }
}
