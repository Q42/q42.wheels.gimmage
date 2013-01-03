using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using ImageQuantization;

namespace Q42.Wheels.Gimmage.Tooling
{
  /// <summary>
  /// Utils used for editing images
  /// </summary>
  public class ImageTools
  {
    /// <summary>
    /// Gets quantizer
    /// </summary>
    public static Bitmap QuantizeBmp(Bitmap bmp, ImageFormat imageformat)
    {
      return imageformat == ImageFormat.Gif ? new OctreeQuantizer(255, 8).Quantize(bmp) : bmp;
    }

    public static byte[] BmpToBytes(Bitmap bmp, ImageFormat format)
    {
      byte[] data;
      using (MemoryStream ms = new MemoryStream())
      {
        bmp.Save(ms, format);
        ms.Seek(0, 0);
        data = ms.ToArray();
      }
      return data;
    }
  }
}
