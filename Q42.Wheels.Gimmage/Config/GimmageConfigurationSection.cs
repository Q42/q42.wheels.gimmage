using System;
using System.Configuration;
using Q42.Wheels.Gimmage.Tooling;

namespace Q42.Wheels.Gimmage.Config
{
  public class GimmageConfigurationSection : ConfigurationSection
  {
    [ConfigurationProperty("defaultTemplate", IsRequired = false, DefaultValue = "default")]
    public string DefaultTemplate
    {
      get
      {
        return (string)this["defaultTemplate"];
      }
    }

    [ConfigurationProperty("mimeTypeExtractionMethod", IsRequired = false, DefaultValue = "extension")]
    public string MimeTypeExtractionMethodStr
    {
      get
      {
        return this["mimeTypeExtractionMethod"] as string;
      }
    }

    public MimeType.MimeTypeExtractionMethod MimeTypeExtractionMethod
    {
      get
      {
        return (MimeType.MimeTypeExtractionMethod)Enum.Parse(typeof(MimeType.MimeTypeExtractionMethod), MimeTypeExtractionMethodStr, true);
      }
    }

    [ConfigurationProperty("sources")]
    public SourceCollection Sources
    {
      get
      {
        return this["sources"] as SourceCollection ?? new SourceCollection();
      }
    }

    [ConfigurationProperty("cache", IsRequired = false)]
    public CacheElement Cache
    {
      get
      {
        return this["cache"] as CacheElement ?? new CacheElement();
      }
    }

    [ConfigurationProperty("backup", IsRequired = false)]
    public BackupElement Backup
    {
      get
      {
        return this["backup"] as BackupElement ?? new BackupElement();
      }
    }
  }
}
