using System;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace Q42.Wheels.Gimmage.Tooling
{
  public static class MimeType
  {
    /// <summary>
    /// Gets the imageformat by mimetype
    /// </summary>
    /// <param name="mimetype"></param>
    /// <returns></returns>
    public static ImageFormat GetImageFormat(string mimetype)
    {
      switch (mimetype)
      {
        case "image/jpeg":
        case "image/jpg":
        case "image/pjpeg":
        case "image/x-citrix-pjpeg":
          return ImageFormat.Jpeg;
        case "image/gif":
          return ImageFormat.Gif;
        case "image/png":
        case "image/x-png":
          return ImageFormat.Png;
        case "image/bmp":
          return ImageFormat.Jpeg;
        case "image/tiff":
          return ImageFormat.Tiff;
        default:
          throw new ArgumentException("mimetype " + mimetype + " unknown.");
      }
    }

    /// <summary>
    /// Way to extract mimetype from file. Internet Explorer also uses this dll
    /// </summary>
    /// <param name="pBC"></param>
    /// <param name="pwzUrl"></param>
    /// <param name="pBuffer"></param>
    /// <param name="cbSize"></param>
    /// <param name="pwzMimeProposed"></param>
    /// <param name="dwMimeFlags"></param>
    /// <param name="ppwzMimeOut"></param>
    /// <param name="dwReserverd"></param>
    /// <returns></returns>
    [DllImport(@"urlmon.dll", CharSet = CharSet.Auto)]
    private extern static System.UInt32 FindMimeFromData(
        UInt32 pBC,
        [MarshalAs(UnmanagedType.LPStr)] String pwzUrl,
        [MarshalAs(UnmanagedType.LPArray)] byte[] pBuffer,
        UInt32 cbSize,
        [MarshalAs(UnmanagedType.LPStr)] String pwzMimeProposed,
        UInt32 dwMimeFlags,
        out UInt32 ppwzMimeOut,
        UInt32 dwReserverd
    );

    public enum MimeTypeExtractionMethod { extension, bytes };

    public static string GetMimeTypeByExtension(FileInfo file)
    {
      string ext = file.Extension.ToLower();
      switch (ext)
      {
        case ".jpg":
        case ".jpeg":
          return "image/jpeg";
        case ".png":
          return "image/png";
        case ".gif":
          return "image/gif";
        case ".bmp":
          return "image/bmp";
        default:
          throw new NotSupportedException(string.Format("Extension {0} is not supported", ext));
      }
    }

    public static string GetMimeTypeByBytes(FileInfo file)
    {
      if (!file.Exists)
        throw new FileNotFoundException(file.FullName + " not found");

      byte[] buffer = new byte[256];
      using (FileStream fs = file.OpenRead())
      {
        if (fs.Length >= 256)
          fs.Read(buffer, 0, 256);
        else
          fs.Read(buffer, 0, (int)fs.Length);
      }
      try
      {
        UInt32 mimetype;
        FindMimeFromData(0, null, buffer, 256, null, 0, out mimetype, 0);
        IntPtr mimeTypePtr = new IntPtr(mimetype);
        string mime = Marshal.PtrToStringUni(mimeTypePtr);
        Marshal.FreeCoTaskMem(mimeTypePtr);
        return mime;
      }
      catch (Exception)
      {
        return "unknown/unknown";
      }
    }

    public static string GetMimeTypeByByteArray(byte[] data)
    {
      try
      {
        UInt32 mimetype;
        FindMimeFromData(0, null, data, 256, null, 0, out mimetype, 0);
        IntPtr mimeTypePtr = new IntPtr(mimetype);
        string mime = Marshal.PtrToStringUni(mimeTypePtr);
        Marshal.FreeCoTaskMem(mimeTypePtr);
        return mime;
      }
      catch (Exception)
      {
        return "unknown/unknown";
      }
    }

    public static ImageCodecInfo GetEncoderInfo(string mimeType)
    {
      ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
      foreach (ImageCodecInfo codec in codecs)
        if (codec.MimeType == mimeType)
          return codec;
      return null;
    }

    public static ImageCodecInfo GetEncoderInfo(ImageFormat format)
    {
      ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
      foreach (ImageCodecInfo codec in codecs)
        if (codec.FormatID == format.Guid)
          return codec;
      return null;
    }
  }
}
