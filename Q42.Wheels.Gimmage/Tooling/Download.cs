using System;
using System.Drawing;
using System.IO;
using System.Net;

namespace Q42.Wheels.Gimmage.Tooling
{
  /// <summary>
  /// Tools for downloading images
  /// </summary>
  public class Download
  {
    /// <summary>
    /// Downloads the file and saves it to disk
    /// </summary>
    /// <param name="fileUri">URI of the file to download</param>
    /// <returns></returns>
    public static Bitmap DownloadFromInternet(string fileUri)
    {
      Stream str = null;
      HttpWebRequest wReq = (HttpWebRequest)WebRequest.Create(fileUri);
      HttpWebResponse wRes = (HttpWebResponse)(wReq).GetResponse();
      str = wRes.GetResponseStream();

      return new Bitmap(str);
    }

    /// <summary>
    /// Downloads the file and saves it on disk
    /// </summary>
    /// <param name="fileUri">URI of the file to download</param>
    /// <param name="filePath">Path to save the image to</param>
    /// <returns></returns>
    public static FileInfo DownloadFromInternet(string fileUri, string filePath)
    {
      FileInfo file = new FileInfo(filePath);
      if (!file.Directory.Exists)
        file.Directory.Create();

      byte[] b;
      HttpWebRequest myReq = (HttpWebRequest)WebRequest.Create(fileUri);

      using (WebResponse myResp = myReq.GetResponse())
        b = ReadFully(myResp.GetResponseStream(), 32768);

      using (FileStream fs = new FileStream(filePath, FileMode.Create))
      using (BinaryWriter w = new BinaryWriter(fs))
        w.Write(b);

      if (!file.Exists)
        throw new FileNotFoundException("WebImage not saved to disk.");

      return file;
    }

    /// <summary>
    /// Reads data from a stream until the end is reached. The
    /// data is returned as a byte array. An IOException is
    /// thrown if any of the underlying IO calls fail.
    /// </summary>
    /// <param name="stream">The stream to read data from</param>
    /// <param name="initialLength">The initial buffer length</param>
    private static byte[] ReadFully(Stream stream, int initialLength)
    {
      // If we've been passed an unhelpful initial length, just
      // use 32K.
      if (initialLength < 1)
        initialLength = 32768;

      byte[] buffer = new byte[initialLength];
      int read = 0;

      int chunk;
      while ((chunk = stream.Read(buffer, read, buffer.Length - read)) > 0)
      {
        read += chunk;

        // If we've reached the end of our buffer, check to see if there's
        // any more information
        if (read == buffer.Length)
        {
          int nextByte = stream.ReadByte();

          // End of stream? If so, we're done
          if (nextByte == -1)
            return buffer;

          // Nope. Resize the buffer, put in the byte we've just
          // read, and continue
          byte[] newBuffer = new byte[buffer.Length * 2];
          Array.Copy(buffer, newBuffer, buffer.Length);
          newBuffer[read] = (byte)nextByte;
          buffer = newBuffer;
          read++;
        }
      }
      // Buffer is now too big. Shrink it.
      byte[] ret = new byte[read];
      Array.Copy(buffer, ret, read);
      return ret;
    }

  }

}