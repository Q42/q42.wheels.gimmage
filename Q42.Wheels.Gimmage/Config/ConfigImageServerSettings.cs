using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;
using Q42.Wheels.Gimmage.Templating;
using Q42.Wheels.Gimmage.Tooling;
using Q42.Wheels.Gimmage.Interfaces;

namespace Q42.Wheels.Gimmage.Config
{
  public class ConfigImageServerSettings : IImageServerSettings
  {

    public ConfigImageServerSettings(GimmageConfigurationSection gconfig)
    {
      cacheDir = gconfig.Cache.SourceDir;
      expiresDays = gconfig.Cache.ExpiresDays;
      if (gconfig.Backup.SourceDir != null)
        backupDir = gconfig.Backup.SourceDir;
      mimeTypeExtractionMethod = gconfig.MimeTypeExtractionMethod;

      sources = new Dictionary<string, SourceElement>();
      foreach (SourceElement element in gconfig.Sources)
      {
        // first verify that directory exists
        if (element.Type == SourceType.file)
          ConfigTools.GetDirectory(element.Source, false);
        sources.Add(element.Name, element);
      }
    }

    private int expiresDays;
    public int ExpiresDays
    {
      get
      {
        return expiresDays;
      }
    }

    private MimeType.MimeTypeExtractionMethod mimeTypeExtractionMethod;
    public MimeType.MimeTypeExtractionMethod MimeTypeExtractionMethod
    {
      get
      {
        return mimeTypeExtractionMethod;
      }
    }

    private Dictionary<string, SourceElement> sources;
    public Dictionary<string, SourceElement> Sources
    {
      get
      {
        return sources;
      }
    }

    private DirectoryInfo cacheDir;
    public DirectoryInfo CacheDir
    {
      get
      {
        return cacheDir;
      }
    }

    private DirectoryInfo backupDir;
    public DirectoryInfo BackupDir
    {
      get
      {
        return backupDir;
      }
    }
  }
}