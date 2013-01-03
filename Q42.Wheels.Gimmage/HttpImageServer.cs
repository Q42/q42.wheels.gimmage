using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web;
using Q42.Wheels.Gimmage.Config;
using Q42.Wheels.Gimmage.Interfaces;
using Q42.Wheels.Gimmage.Tooling;
using System.Diagnostics;
using System.Net;
using System.Data.SqlClient;
using System.Configuration;
using log4net;

namespace Q42.Wheels.Gimmage
{
  public class HttpImageServer : ImageServer, IHttpImageServer
  {
    private static readonly ILog log = LogManager.GetLogger(typeof(HttpImageServer));

    #region if modified since
    private static DateTime GetIfModSinceHeader(HttpRequest request)
    {
      DateTime result = DateTime.MinValue;
      if (string.IsNullOrEmpty(request.Headers["If-Modified-Since"]))
        return result;

      DateTime.TryParse(request.Headers["If-Modified-Since"], System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out result);
      return result;
    }

    private static void SendNotModified(HttpResponse response)
    {
      response.ClearHeaders();
      setCacheHeaders(response);
      response.StatusCode = 304; // not modified
      response.StatusDescription = "Not Modified";
      response.End();
    }

    private static void SendFileNotFound(HttpResponse response)
    {
      response.ClearHeaders();
      response.StatusCode = 404; //not found
      response.StatusDescription = "Not Found";
      response.End();
    }
    #endregion

    #region sending bitmap to client
    /// <summary>
    /// Sends the file to the response with content-length header. will check last-writetime for sending modified header
    /// </summary>
    /// <param name="Response"></param>
    /// <param name="file"></param>
    /// <param name="mimetype"></param>
    /// <remarks>Works with all types of files, not image specific</remarks>
    public void SendToResponse(HttpResponse Response, FileInfo file, string mimetype)
    {
      Response.ClearHeaders();
      setCacheHeaders(Response);   

      //Response.Cache.SetETag("\"999ed196eeb0cc1:9\"");
      Response.Cache.SetLastModified(file.LastWriteTime);
      Response.ContentType = mimetype;      
      Response.AppendHeader("Content-Length", file.Length.ToString());
      Response.WriteFile(file.FullName);
      Response.End();
    }

    /// <summary>
    /// Sends the file to the response with content-length header
    /// Works with all types of files, not image specific
    /// </summary>
    /// <param name="response"></param>
    /// <param name="file"></param>
    /// <param name="mimetype"></param>
    private static void SendToResponse(HttpResponse response, byte[] fileContent, DateTime lastWriteTime, string mimetype)
    {
      response.ClearHeaders();
      setCacheHeaders(response);
      
      response.Cache.SetLastModified(lastWriteTime); // to avoid rounding problems with header (no millisecs there)
      response.ContentType = mimetype;      
      response.AppendHeader("Content-Length", fileContent.Length.ToString());
      response.BinaryWrite(fileContent);
      response.End();
    }

    private static void setCacheHeaders(HttpResponse Response)
    {
      HttpCachePolicy cache = Response.Cache;
      cache.SetCacheability(HttpCacheability.Public);
      cache.SetMaxAge(new TimeSpan(48, 0, 0));
      cache.SetRevalidation(HttpCacheRevalidation.None);
    }

    public void ServeImage(string fileName, byte[] fileContent, DateTime lastWriteTime, string mimeType, ISource source, IImageTemplate template, HttpResponse response, HttpRequest request)
    {
      // throws error if template is not found, because then the backup image can't be served either
      FileInfo cachedFile;

      try
      {
        cachedFile = GetCachedImage(source, fileName, template);
      }
      catch
      {
        // something error happens, serve one of the backup images
        FileInfo originalFile = GetBackupImage();
        if (originalFile == null) throw;
        cachedFile = GetCachedBackupImage(originalFile, template);
      }

      // send not modified
      if (!IsModifiedSince(GetIfModSinceHeader(request), lastWriteTime))
      {
        SendNotModified(response);
        return;
      }

      // save to disk, and send to response
      if (template != null && ShouldCacheBeUpdated(lastWriteTime, cachedFile))
      {
        using (MemoryStream stream = new MemoryStream(fileContent))
        {
          using (Bitmap bmp = ApplyTemplate(new Bitmap(stream), template))
          {
            SaveToDisk(bmp, cachedFile, mimeType);
          }
        }
      }

      if (cachedFile.Exists)
        SendToResponse(response, cachedFile, mimeType);
      else
        SendToResponse(response, fileContent, lastWriteTime, mimeType);

    }

    
    /// <summary>
    /// Sends the bitmap to the response without content-length header
    /// </summary>
    /// <param name="Response"></param>
    /// <param name="bmp"></param>
    /// <param name="mimetype"></param>
    public void SendToResponse(HttpResponse Response, Bitmap bmp, string mimetype)
    {
      ImageFormat format = MimeType.GetImageFormat(mimetype);

      Response.Cache.SetCacheability(HttpCacheability.Public);
      Response.Cache.SetMaxAge(new TimeSpan(48, 0, 0));
      Response.ContentType = mimetype;
      //last modified is not known for a bmp image

      // Deze stuurt de content-length, maar kost ook veel rekenkracht. Is niet nodig denk ik!
      //Response.AppendHeader("Content-Length", ImageTools.BmpToBytes(bmp, format).Length.ToString());
      bmp.Save(Response.OutputStream, format);
    }
    public void SendAsAttachment(HttpResponse Response, string filename)
    {
      Response.AddHeader("Content-Disposition", "attachment; filename=" + filename);
      Response.ContentType = "application/octet-stream";
    }

    /// <summary>
    /// TODO: Make cached
    /// </summary>
    public void ServeImage(string path, string templateName, HttpResponse Response)
    {
      IImageTemplate template = GetTemplate(templateName);

      string dirName;
      string fileName;

      if (path.StartsWith("~/"))
      {
        dirName = VirtualPathUtility.GetDirectory(path);
        fileName = VirtualPathUtility.GetFileName(path);
      }
      else
      {
        dirName = Path.GetDirectoryName(path);
        fileName = Path.GetFileName(path);
      }

      DirectoryInfo dir = ConfigTools.GetDirectory(dirName, false);
      if (dir == null)
        return;

      FileInfo originalFile = File.Exists(dir.FullName + fileName) ? new FileInfo(dir.FullName + fileName) : null;
      if (originalFile == null)
        return;

      using (Bitmap bmp = ApplyTemplate(new Bitmap(originalFile.FullName), template))
      {
        SendToResponse(Response, bmp, GetMimeType(originalFile));
      }
    }

    /// <summary>
    /// for websites; gets source, id and template from querystring, uses the configuration from web.config
    /// </summary>
    /// <param name="Response"></param>
    /// <param name="Request">Gets source, id and template from querystring</param>
    public void ServeImage(HttpResponse Response, HttpRequest Request)
    {
      //get querystring variables
      string sourceStr = Request.QueryString["source"] ?? GimmageConfig.Config.Sources.DefaultSource.Name;
      string id = (Request.QueryString["id"] != null) ? Request.QueryString["id"].ToString() : string.Empty;
      string fileName = Request.QueryString["filename"] ?? string.Empty;
      string templateStr = Request.QueryString["template"] ?? string.Empty;

      if (!String.IsNullOrEmpty(id))
        ServeImage(fileName, sourceStr, templateStr, Response, Request);
      else if (!String.IsNullOrEmpty(fileName))
        ServeImage(fileName, templateStr, Response);
    }

    /// <summary>
    /// for websites; uses the configuration from web.config, saves to response
    /// </summary>
    /// <param name="fileName">name of the file to serve</param>
    /// <param name="sourceStr">name of the source where file is located</param>
    /// <param name="templateStr">name of the template that should be used to transform</param>
    /// <param name="response">sends the transformed image incl content length and mimetype to outputstream</param>
    /// <param name="request">gets ifmodifiedsince header from</param>
    //[DebuggerStepThrough] // Ignore FileNotFound exceptions
    public void ServeImage(string fileName, string sourceStr, string templateStr, HttpResponse response, HttpRequest request)
    {
      ISource source = GetSource(sourceStr);
      if (source == null)
      {
        if (log.IsDebugEnabled) log.DebugFormat("Requested image source '{0}' does not exist", sourceStr);
        SendFileNotFound(response);
        return;
      }
      IImageTemplate template = GetTemplate(templateStr);

      ServeImage(fileName, source, template, response, request);
    }

   // [DebuggerStepThrough] // Ignore FileNotFound exceptions
    public void ServeImage(string fileName, ISource source, IImageTemplate template, HttpResponse response, HttpRequest request)
    {
      if (source == null)
      {
        SendFileNotFound(response);
        return;
      }      

      try
      {
        switch (source.Type)
        {
          case SourceType.db:
            serveImageFromDatabase(fileName, source, template, response, request);
            break;
          case SourceType.share:
          case SourceType.file:
            serveImageFromFile(fileName, source, template, response, request);
            break;
          default:
            throw new NotImplementedException(string.Format("No imageServer implemented for sourceType '{0}'", source.Type));
        }
      }
      catch (FileNotFoundException)
      {
        SendFileNotFound(response);
      }
      catch (Exception ex)
      {
        log.Error("Exception raised while serving immage!", ex);
        throw;
      }
    }

    private void serveImageFromFile(string fileName, ISource source, IImageTemplate template, HttpResponse response, HttpRequest request)
    {
      // throws error if template is not found, because then the backup image can't be served either
      FileInfo cachedFile;
      FileInfo originalFile;

      try
      {
        cachedFile = GetCachedImage(source, fileName, template);
        originalFile = GetOriginalImage(source, fileName);
        if (!originalFile.Exists)
          throw new FileNotFoundException("Not found: " + originalFile);
      }
      catch
      {
        // something error happens, serve one of the backup images
        originalFile = GetBackupImage();
        if (originalFile == null) throw;
        cachedFile = GetCachedBackupImage(originalFile, template);
      }

      FileInfo fileToServe = template == null ? originalFile : cachedFile;

      // send not modified
      if (originalFile.Exists && !IsModifiedSince(GetIfModSinceHeader(request), fileToServe))
      {
        SendNotModified(response);
      }
      else
      {
        // save to disk, and send to response
        if (template != null && ShouldCacheBeUpdated(originalFile, cachedFile))
        {
          using (Bitmap bmp = ApplyTemplate(new Bitmap(originalFile.FullName), template))
          {
            string mimeType = GetMimeType(cachedFile);
            SaveToDisk(bmp, cachedFile, mimeType);
          }
        }

        SendToResponse(response, fileToServe, GetMimeType(fileToServe));
      }
    }

    /// <summary>
    /// hack for n2 / 9292 - server image from n2 database instead of file if possible
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="source"></param>
    /// <param name="template"></param>
    /// <param name="response"></param>
    /// <param name="request"></param>
    private void serveImageFromDatabase(string fileName, ISource source, IImageTemplate template, HttpResponse response, HttpRequest request)
    {
      // Hack: treat PDF files differently
      // Todo: store in local cache
      if (fileName.EndsWith(".pdf"))
      {
        SendToResponse(response, GetImageStreamFromDatabase(source, fileName), DateTime.MinValue, "application/pdf");
        return;
      }

      //check if the templated image exists => if not => check if the original file exists on disk => if not => retrieve from database

      //check for templated image
      var cachedTemplateFile = getImageCacheFileName(source, fileName, template);
      if(cachedTemplateFile == null)
        throw new ArgumentNullException("getImageCacheFileName returned NULL, should not be");

      if (!cachedTemplateFile.Exists)
      {
        //lock for thread safety, double check existance of template file before actually saving shit
        lock (String.Intern(cachedTemplateFile.ToString()))
        {
          if (!cachedTemplateFile.Exists)
          {
            //check if original file exists on disk or retrieve it from the database
            string path = getDBImageCachePath(source, fileName);
            if (!Directory.Exists(path))
              Directory.CreateDirectory(path);

            byte[] originalImgData = null;
            FileInfo originalFile = new FileInfo(Path.Combine(path, fileName));
            
            if (log.IsDebugEnabled) log.DebugFormat("Going to render template '{0}' for original '{1}' since it did not exist on disk ({2})", template.Name, originalFile.ToString(), cachedTemplateFile.ToString());
            
            if (!originalFile.Exists)
            {
              lock (String.Intern(originalFile.ToString()))
              {
                if (!originalFile.Exists)
                {
                  //retrieve original from database and save it to disk
                  originalImgData = GetImageStreamFromDatabase(source, fileName);
                  try{
                    using (FileStream fs = new FileStream(originalFile.ToString(), FileMode.Create, FileAccess.Write))
                    {
                      fs.Write(originalImgData, 0, originalImgData.Length);
                    }
                  }
                  catch(Exception e){
                    //save to disk failed, but we have the image stream! rejoice!
                    log.Error(string.Format("Could not save image '{0}' from database to disk '{1}', raw-image-stream from database will be used. (PERFORMANCE PENALTY!)", fileName, originalFile.ToString()), e);
                  }
                }
              }
            }

            //the original is available as image on disk or (fastest) memory stream
            try
            {
              if (originalImgData != null)
              {
                //save original file stream as templated file to disk
                using (Bitmap img = ApplyTemplate((Bitmap)Bitmap.FromStream(new MemoryStream(originalImgData)), template))
                {
                  SaveToDisk(img, cachedTemplateFile, MimeType.GetMimeTypeByByteArray(originalImgData));
                }
                SendToResponse(response, cachedTemplateFile, MimeType.GetMimeTypeByByteArray(originalImgData));//it is new
              }
              else
              {
                //originalFile.existed so we use that one as the base for our template transformations
                using (Bitmap img = ApplyTemplate(new Bitmap(originalFile.FullName), template))
                {
                  SaveToDisk(img, cachedTemplateFile, GetMimeType(originalFile));
                }
                SendToResponse(response, cachedTemplateFile, GetMimeType(originalFile));//it is new
              }
              return;//we are done! :D
            }
            catch (Exception e)
            {
              //saving of the cached templated version failed, damnit
              log.Error(string.Format("Could not save cached template image to location '{0}'", cachedTemplateFile), e);
              throw;//return 500 error
            }
          }
        }
      }

      //verify if the images are the same, return 304 if so
      if (!IsModifiedSince(GetIfModSinceHeader(request), cachedTemplateFile))
      {
        SendNotModified(response);
      }
      else
      {
        //not the same, return image
        SendToResponse(response, cachedTemplateFile, GetMimeType(cachedTemplateFile));
      }
    }

    /// <summary>
    /// Retrieve the given filename from the n2cms database and return the complete byte array of data of it
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    private static byte[] GetImageStreamFromDatabase(ISource source, string fileName)
    {
      if(log.IsDebugEnabled) log.DebugFormat("Going to retrieve image data from database for filename '{0}'", fileName);

      if (source.Type != SourceType.db)
        throw new ArgumentException(string.Format("Source {0} is not of type 'db' but '{1}', can not serve image from database", source.Name, source.Type));

      if (ConfigurationManager.ConnectionStrings[source.DatabaseConnectionStringName] == null)
        throw new ConfigurationErrorsException(string.Format("Could not find connectionstringName '{0}' in the configurations ConnectionStrings section", source.DatabaseConnectionStringName));

      byte[] imgData = null;
      using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings[source.DatabaseConnectionStringName].ConnectionString))
      {
        conn.Open();
        using (SqlCommand comm = conn.CreateCommand())
        {
          int fileId;
          if (int.TryParse(fileName, out fileId))
          {
            comm.CommandText = "select length, data from n2filesystemitem where id = @p0";
            comm.Parameters.Add(new SqlParameter("@p0", fileId.ToString()));
          }
          else
          {
            comm.CommandText = "select length, data from n2filesystemitem where parent + name = '/upload/' + @p0";
            comm.Parameters.Add(new SqlParameter("@p0", fileName));
          }
          var datareader = comm.ExecuteReader();
          datareader.Read();
          if (!datareader.HasRows)
          {
            if (log.IsDebugEnabled) log.Debug("File not found in database");
            throw new FileNotFoundException("Not found: " + fileName); ;
          }

          long filesize = datareader.GetInt64(0);
          imgData = new byte[filesize];

          datareader.GetBytes(datareader.GetOrdinal("data"), 0, imgData, 0, (int)filesize);
        }
      }
      return imgData;
    }

    /// <summary>
    /// given the source filename and template return the complete filepath to which to save image
    /// </summary>
    /// <param name="source"></param>
    /// <param name="fileName"></param>
    /// <param name="template"></param>
    /// <returns></returns>
    private FileInfo getImageCacheFileName(ISource source, string fileName, IImageTemplate template)
    {
      DirectoryInfo dir = GetSourceDirectory(source);

      string cachedImagePath = Path.Combine(GimmageConfig.Config.Cache.SourceDir.FullName, source.Name);

      string path = getDBImageCachePath(source, fileName);
      DirectoryInfo cacheSubDir = new DirectoryInfo(path);

      return GetImage(cacheSubDir, fileName, template);
    }

    private string getDBImageCachePath(ISource source, string fileName)
    {
      DirectoryInfo dir = GetSourceDirectory(source);

      string path = String.Empty;
      string cachedImagePath = Path.Combine(GimmageConfig.Config.Cache.SourceDir.FullName, source.Name);
            
      //string fileNameExtensionless = fileName.Substring(0, fileName.IndexOf('.'));
      char[] chars = fileName.ToCharArray();

      for (int i = 0; i < 3 && i < chars.Length; i++)
      {
        path = Path.Combine(path, chars[i].ToString());
      }

      //path = Path.Combine(path, fileName);
      path = Path.Combine(cachedImagePath, path);
      return path;
    }


    public void DeleteImage(string source, string filename)
    {
      string path = Path.Combine(filename.Substring(2, 1), filename);
      path = Path.Combine(filename.Substring(1, 1), path);
      path = Path.Combine(filename.Substring(0, 1), path);
      path = Path.Combine(GimmageConfig.Config.Sources[source].Directory, path);
      
      File.Delete(path);
    }
    #endregion
  }
}