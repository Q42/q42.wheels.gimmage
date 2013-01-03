using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Threading;
using Q42.Wheels.Gimmage.Config;
using Q42.Wheels.Gimmage.Interfaces;
using Q42.Wheels.Gimmage.Sources;
using Q42.Wheels.Gimmage.Templating;
using Q42.Wheels.Gimmage.Tooling;
using log4net;

namespace Q42.Wheels.Gimmage
{
  public class ImageServer : IImageServer
  {
    #region privates and constructor

    private static readonly ILog log = LogManager.GetLogger(typeof(ImageServer));

    private readonly IDictionary<string, IImageTemplate> templateCache = new Dictionary<string, IImageTemplate>();
    private readonly IDictionary<string, ISource> sourceCache = new Dictionary<string, ISource>();

    public ImageServer()
    {
      log.Debug("Initializing Gimmage engine");

      loadGimmageTemplates();

      //initialize sources cache dictionary
      foreach (SourceElement el in GimmageConfig.Config.Sources)
      {
        sourceCache.Add(el.Name.ToLowerInvariant(), el);
      }
      log.Debug("Gimmage Engine successfully initialized");
    }

    /// <summary>
    /// Loops through all types in all loaded assemblies to search for AbstractTemplates
    /// </summary>
    private void loadGimmageTemplates()
    {
      // use reflection to get the right image template
      Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
      foreach (Assembly assembly in assemblies)
      {
        try
        {
          Type[] types = assembly.GetTypes();
          foreach (Type type in types)
          {
            if (type.BaseType == typeof(AbstractTemplate))
            {
              AbstractTemplate template = System.Activator.CreateInstance(type) as AbstractTemplate;
              templateCache.Add(template.Name, template);
              log.DebugFormat("Found image template: {0}", template.GetType());
            }
          }
        }
        catch (Exception ex)
        {
          log.Error("Exception raised wihle searching Templates extending AbstractTemplate", ex);
        }
      }
    }
    #endregion

    #region Paths

    public void AddSource(string name, string source, SourceType type)
    {
      AddSource(new Source(name, source, type));
    }

    public void AddSource(ISource source)
    {
      sourceCache.Add(source.Name, source);
    }

    /// <summary>
    /// Returns the directory
    /// </summary>
    protected DirectoryInfo GetSourceDirectory(ISource source)
    {
      if (source.Type == SourceType.file)
        return ConfigTools.GetDirectory(source.Source, false);

      // create directory to save all original images
      if (source.Type == SourceType.http || source.Type == SourceType.share)
      {
        string sourceDir = Path.Combine(GimmageConfig.Config.Cache.SourceDir.FullName, source.Name + "-original");
        return ConfigTools.GetDirectory(sourceDir, true);
      }

      //all other types of sources have no 'original' folder, return the cache dir in these cases
      return ConfigTools.GetDirectory(GimmageConfig.Config.Cache.SourceDir.FullName, false);
    }

    /// <summary>
    /// Gets the reference to the original file
    /// </summary>
    /// <param name="source"></param>
    /// <param name="fileName"></param>
    /// <returns></returns>
    protected FileInfo GetOriginalImage(string source, string fileName)
    {
      return GetOriginalImage(GetSource(source), fileName);
    }

    /// <summary>
    /// Gets the reference to the original file
    /// </summary>
    /// <param name="source"></param>
    /// <param name="fileName"></param>
    /// <returns></returns>
    protected FileInfo GetOriginalImage(ISource source, string fileName)
    {
      FileInfo file = new FileInfo(Path.Combine(source.Source, fileName));
      return file;
    }


    /// <summary>
    /// Gets the reference to the cached file (doesn't have to exist!)
    /// </summary>
    protected FileInfo GetCachedImage(string sourceName, string fileName, string templateName)
    {
      ISource source = GetSource(sourceName);
      IImageTemplate template = GetTemplate(templateName);

      return GetCachedImage(source, fileName, template);
    }

    protected FileInfo GetCachedImage(ISource source, string fileName, IImageTemplate template)
    {
      DirectoryInfo dir = GetSourceDirectory(source);

      string cachedImagePath = Path.Combine(GimmageConfig.Config.Cache.SourceDir.FullName, source.Name);

      if (source.Name == "N2")
      {
        string fileNameExtensionless = fileName.Substring(0, fileName.IndexOf('.'));
        char[] chars = fileNameExtensionless.ToCharArray();
        string path = String.Empty;

        for (int i = 0; i < 3 || i < chars.Length; i++)
        {
          path += String.Format("{0}/", chars[i]);
        }       

        path = path + fileName;
      }

      DirectoryInfo cacheSubDir = new DirectoryInfo(cachedImagePath);

      //fileName = source.Type == SourceType.file ? fileName : fileName.Replace('/', '_').Replace('\\', '_');
      return GetImage(cacheSubDir, fileName, template);
    }

    protected FileInfo GetCachedBackupImage(FileInfo backupImage, IImageTemplate template)
    {
      string cachedImagePath = Path.Combine(GimmageConfig.Config.Cache.SourceDir.FullName, "backup-images");
      DirectoryInfo cacheSubDir = new DirectoryInfo(cachedImagePath);

      return GetImage(cacheSubDir, backupImage.Name, template);
    }

    /// <summary>
    /// Gets a random image from the specified backup directory
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    protected FileInfo GetBackupImage()
    {
      if (GimmageConfig.Config.Backup.SourceDir == null)
        return null;

      if (GimmageConfig.Config.Backup.SourceImage != null)
        return GimmageConfig.Config.Backup.SourceImage;

      FileInfo[] files = GimmageConfig.Config.Backup.SourceDir.GetFiles();
      if (files.Length == 0)
        return null;

      //get any image from random map, if we use RandomImage
      int maxValue = files.Length;
      int filenr = new Random().Next(0, maxValue);
      return files[filenr];
    }

    /// <summary>
    /// Gets the file from the directory, and checks soms security
    /// </summary>
    /// <param name="dir"></param>
    /// <param name="filename"></param>
    /// <returns></returns>
    protected FileInfo GetImage(DirectoryInfo dir, string filename, IImageTemplate template)
    {
      if (string.IsNullOrEmpty(filename))
        throw new ArgumentNullException("No filename given.");

      string fullFilename = Path.Combine(dir.FullName, filename);
      if (!fullFilename.StartsWith(dir.FullName))
        throw new ArgumentException("Requested file " + fullFilename + " isn't within directory " + dir.FullName);

      FileInfo fi = new FileInfo(fullFilename);

      // plak templatename achter de file, en daarachter weer de extensie
      if (template != null)
      {
        string fileWithoutExtension = Path.GetFileNameWithoutExtension(fi.FullName);
        string extension = fi.Extension;
        fi = new FileInfo(
              Path.Combine(
                fi.Directory.FullName,
                string.Format("{0}_{1}{2}", fileWithoutExtension, template.Name, extension)));
      }

      return fi;
    }

    #endregion

    #region modified-since
    protected bool IsModifiedSince(DateTime refDate, FileInfo file)
    {
      if (!file.Exists)
        return true;//file does not even exist

      DateTime lastModified = file.LastWriteTime;
      return refDate.AddSeconds(1) < lastModified;
    }

    protected bool IsModifiedSince(DateTime refDate, DateTime lastWriteTime)
    {
      DateTime lastModified = lastWriteTime;
      return refDate.Ticks.CompareTo(lastModified.Ticks) <= 0;
    }
    #endregion

    #region should cache be updated
    /// <summary>
    /// Should the cache file be updated, looks at ExpiresDays, filewritetime and filecreatetime
    /// </summary>
    /// <param name="file"></param>
    /// <param name="cache"></param>
    /// <returns></returns>
    public bool ShouldCacheBeUpdated(FileInfo file, FileInfo cache)
    {
      if (!cache.Exists || !GimmageConfig.Config.Cache.Enabled)
        return true;

      DateTime cacheWriteTime = cache.LastWriteTime;
      DateTime fileWriteTime = file.LastWriteTime;
      DateTime fileCreateTime = file.CreationTime;

      // if cache is older than original, cache should be updated (true)
      return (cacheWriteTime < fileWriteTime || cacheWriteTime < fileCreateTime);
    }
    
    /// <summary>
    /// Should the cache file be updated, looks at ExpiresDays, filewritetime and filecreatetime
    /// </summary>
    /// <param name="file"></param>
    /// <param name="cache"></param>
    /// <returns></returns>
    public bool ShouldCacheBeUpdated(DateTime lastWriteTime, FileInfo cache)
    {
      if (!cache.Exists || !GimmageConfig.Config.Cache.Enabled)
        return true;

      DateTime cacheWriteTime = cache.LastWriteTime;


      // if cached file is older than max cached days, return true
      if (cacheWriteTime < DateTime.Now.AddDays(GimmageConfig.Config.Cache.ExpiresDays))
        return true;

      // if cache is older than original, cache should be updated (true)
      return (cacheWriteTime < lastWriteTime);
    }
    #endregion

    #region apply template
    /// <summary>
    /// manipulates the image and saves it to cachePath
    /// </summary>
    public Bitmap ApplyTemplate(Bitmap bmp, IImageTemplate template)
    {
      return ImageManipulation.Manipulate.Apply(bmp, template.Filters);
    }
    #endregion

    /// <summary>
    /// retrieves the mimetype using one of the extraction methods
    /// </summary>
    /// <param name="fileinfo"></param>
    /// <returns></returns>
    protected string GetMimeType(FileInfo fileinfo)
    {
      // first tries the extension one, if user wants it
      if (GimmageConfig.Config.MimeTypeExtractionMethod == MimeType.MimeTypeExtractionMethod.extension)
      {
        try { return MimeType.GetMimeTypeByExtension(fileinfo); }
        catch (Exception) { }
      }

      // fallback to getting from bytes
      return MimeType.GetMimeTypeByBytes(fileinfo);
    }

    #region saving final bitmap
    /// <summary>
    /// Wrapper for SaveToDisk, which retries this function
    /// Use when it is possible that file to write to is locked by other read / write action
    /// </summary>
    /// <param name="bmp"></param>
    /// <param name="cache"></param>
    /// <param name="mimeType">mimetype as string, will be converted to ImageFormat</param>
    protected void SaveToDisk(Bitmap bmp, FileInfo cache, string mimeType)
    {
      SaveToDisk(bmp, cache, MimeType.GetImageFormat(mimeType));
    }
    
    /// <summary>
    /// Wrapper for SaveToDisk, which retries this function
    /// Use when it is possible that file to write to is locked by other read / write action
    /// </summary>
    /// <param name="bmp"></param>
    /// <param name="cache"></param>
    /// <param name="mimeType">mimetype as string, will be converted to ImageFormat</param>
    /// <param name="retries">number of retries, usually 2 or 3</param>
    /// <param name="timeout">timeout between retries, usually 100-200ms</param>
    protected void SaveToDisk(Bitmap bmp, FileInfo cache, string mimeType, int retries, int timeout)
    {
      SaveToDisk(bmp, cache, MimeType.GetImageFormat(mimeType), retries, timeout);
    }

    /// <summary>
    /// Saves the bmp to disk, first quantizes
    /// </summary>
    protected void SaveToDisk(Bitmap bmp, FileInfo cache, ImageFormat imageformat)
    {
      if (!cache.Directory.Exists)
        cache.Directory.Create();

      if (imageformat == ImageFormat.Gif)
        bmp = ImageTools.QuantizeBmp(bmp, imageformat);

      try
      {
        if (imageformat == ImageFormat.Jpeg)
        {
          EncoderParameters encoderParams = new EncoderParameters(1);
          encoderParams.Param[0] = new EncoderParameter(Encoder.Quality, 85L);
          ImageCodecInfo imageEncoder = MimeType.GetEncoderInfo(imageformat);

          // if error = general GDI+ error, set your directory permissions straight!
          // give IUSR permission to the cache folder
          bmp.Save(cache.FullName, imageEncoder, encoderParams);
        }
        else
        {
          bmp.Save(cache.FullName, imageformat);
        }
      }
      catch (System.Runtime.InteropServices.ExternalException e) // System.Runtime.InteropServices.ExternalException: A generic error occurred in GDI+.
      // discard this useless exception (just do some logging) and throw a new IOException with a proper description
      {
        throw new IOException("Could not write " + cache + " to disk, maybe the file is in use or not enough permissions", e);
      }
      cache.Refresh();
    }

    /// <summary>
    /// Wrapper for SaveToDisk, which retries this function
    /// Use when it is possible that file to write to is locked by other read / write action
    /// </summary>
    /// <param name="bmp"></param>
    /// <param name="cache"></param>
    /// <param name="imageformat"></param>
    /// <param name="retries">number of retries, usually 2 or 3</param>
    /// <param name="timeout">timeout between retries, usually 100-200ms</param>
    protected void SaveToDisk(Bitmap bmp, FileInfo cache, ImageFormat imageformat, int retries, int timeout)
    {
      try
      {
        SaveToDisk(bmp, cache, imageformat);
      }
      catch (IOException)
      {
        while (retries > 0)
        {
          Thread.Sleep(timeout);
          try
          {
            SaveToDisk(bmp, cache, imageformat);
            return;
          }
          catch (IOException)
          {
            // do nothing
          }
          retries--;
        }
        throw;
      }
    }
    #endregion

    #region IImageServer

    public ISource GetSource(string sourceName)
    {
      return sourceCache.ContainsKey(sourceName.ToLowerInvariant()) ? sourceCache[sourceName.ToLowerInvariant()] : null;
    }

    public IDictionary<string, ISource> GetSources()
    {
      return sourceCache;
    }

    /// <summary>
    /// Gets the template that should be used from Q42.Wheels.Gimmage.Templating
    /// </summary>
    /// <param name="templateStr">Fully qualified classname incl namespaces of the template</param>
    /// <returns></returns>
    public IImageTemplate GetTemplate(string templateStr)
    {
      templateStr = templateStr.ToLower();

      if (string.IsNullOrEmpty(templateStr) || !templateCache.ContainsKey(templateStr))
      {
        if (log.IsDebugEnabled) log.DebugFormat("given templatestr '{0}' is not found in Gimmage, returning defaultTemplate", templateStr);
        return templateCache[GimmageConfig.Config.DefaultTemplate];
      }

      return templateCache[templateStr];
    }

    public IDictionary<string, IImageTemplate> GetTemplates()
    {
      IDictionary<string, IImageTemplate> result = new Dictionary<string, IImageTemplate>();

      foreach (IImageTemplate template in templateCache.Values)
      {
        result.Add(template.Name, template);
      }

      return result;
    }

    #endregion
  }
}