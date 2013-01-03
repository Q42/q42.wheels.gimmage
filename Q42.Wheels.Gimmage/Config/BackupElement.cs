using System.Configuration;
using System.IO;

namespace Q42.Wheels.Gimmage.Config
{
  public class BackupElement : ConfigurationElement
  {
    [ConfigurationProperty("enabled", IsRequired = false, DefaultValue = false)]
    public bool Enabled
    {
      get
      {
        return (bool)this["enabled"];
      }
    }

    [ConfigurationProperty("dir", IsRequired = false, DefaultValue = "~/backup")]
    protected string Directory
    {
      get
      {
        return this["dir"] as string;
      }
    }

    [ConfigurationProperty("image", IsRequired = false, DefaultValue = null)]
    protected string Image
    {
      get
      {
        return this["image"] as string;
      }
    }

    public DirectoryInfo SourceDir
    {
      get
      {
        return ConfigTools.GetDirectory(this.Directory, true);
      }
    }

    public FileInfo SourceImage
    {
      get
      {
        if (string.IsNullOrEmpty(Image))
          return null;
        return new FileInfo(Path.Combine(SourceDir.FullName, Image));
      }
    }
  }
}
