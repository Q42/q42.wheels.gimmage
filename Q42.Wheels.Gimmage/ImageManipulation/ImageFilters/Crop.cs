using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Q42.Wheels.Gimmage.ImageManipulation
{
  public partial class ImageFilters
  {
    /// <summary>
    /// Crops a part out of an image
    /// </summary>
    /// <param name="x">Where to start cropping</param>
    /// <param name="y">Where to start cropping</param>
    /// <param name="width">Width of the result image</param>
    /// <param name="height">Height of the result image</param>
    /// <returns></returns>
    public static Filter Crop(int x, int y, int width, int height)
    {
      return delegate(Bitmap bmp)
      {
        // als hij niet resized hoeft te worden, return origineel
        if ((height + y > bmp.Height) || (width + x > bmp.Width))
          return bmp;

        // Transform image.
        Bitmap bmpNew = new Bitmap(width, height);
        Graphics g = Graphics.FromImage(bmpNew);

        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        g.SmoothingMode = SmoothingMode.HighQuality;
        g.PixelOffsetMode = PixelOffsetMode.HighQuality;
        g.CompositingQuality = CompositingQuality.HighQuality;

        g.DrawImage(bmp, -x, -y);
        g.Save();

        // Clean up.
        bmp.Dispose();
        g.Dispose();

        return bmpNew;
      };
    }

    /// <summary>
    /// Chops the top/bottom or left/right of the image off so it fits in the desired canvas
    /// </summary>
    /// <param name="width">Width of the result image</param>
    /// <param name="height">Height of the result image</param>
    /// <returns></returns>
    public static Filter Crop(int width, int height)
    {
      return delegate(Bitmap bmp)
      {
        // als hij niet resized hoeft te worden, return origineel
        if ((height > bmp.Height) || (width > bmp.Width))
          return bmp;

        double dblHeightDivider = Convert.ToDouble(bmp.Height) / Convert.ToDouble(height);
        double dblWidthDivider = Convert.ToDouble(bmp.Width) / Convert.ToDouble(width);
        int intImageHeight = height;
        int intImageWidth = width;
        int y = 0;
        int x = 0;

        if (dblHeightDivider > dblWidthDivider)
        {
          intImageHeight = Convert.ToInt32(bmp.Height / dblWidthDivider);
          y = (height - intImageHeight) / 2;
        }
        else
        {
          intImageWidth = Convert.ToInt32(bmp.Width / dblHeightDivider);
          x = (width - intImageWidth) / 2;
        }

        // Transform image.
        Bitmap bmpNew = new Bitmap(width, height);
        Graphics g = Graphics.FromImage(bmpNew);

        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        g.SmoothingMode = SmoothingMode.HighQuality;
        g.PixelOffsetMode = PixelOffsetMode.HighQuality;
        g.CompositingQuality = CompositingQuality.HighQuality;

        g.DrawImage(bmp, x, y, intImageWidth, intImageHeight);
        g.Save();

        // Clean up.
        bmp.Dispose();
        g.Dispose();

        return bmpNew;
      };
    }
  }
}