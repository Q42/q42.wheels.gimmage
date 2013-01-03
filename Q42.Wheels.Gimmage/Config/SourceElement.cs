using System;
using System.Linq;
using System.Configuration;
using Q42.Wheels.Gimmage.Interfaces;
using System.Diagnostics;

namespace Q42.Wheels.Gimmage.Config
{
  public sealed class SourceElement : ConfigurationElement, ISource
  {
    [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
    public string Name
    {
      get
      {
        return this["name"] as string;
      }
    }
    
    /// <summary>
    /// If Type == File then exceptions will be thrown on misconfigured directory attributes
    /// </summary>
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

    [ConfigurationProperty("dir")]
    public string Directory
    {
      get
      {
        return this["dir"] as string;
      }
    }

    [ConfigurationProperty("connectionStringName")]
    public string DatabaseConnectionStringName
    {
      get
      {
        return this["connectionStringName"] as string;
      }
    }

    [ConfigurationProperty("type")]
    public string Sourcetype
    {
      get
      {
        return this["type"] as string;
      }
    }

    public SourceType Type
    {
      get
      {
        if (!string.IsNullOrWhiteSpace(Sourcetype))
          return (SourceType)Enum.Parse(typeof(SourceType), Sourcetype);

        //auto-detect type of this source
        if (Directory.ToLower().StartsWith("http://") || Directory.ToLower().StartsWith("https://"))
          return SourceType.http;
        if (Directory.ToLower().StartsWith("ftp://"))
          return SourceType.ftp;
        if (Directory.ToLower().StartsWith("\\"))
          return SourceType.share;

        //default unknown
        return SourceType.file;
      }
    }
  }
}
