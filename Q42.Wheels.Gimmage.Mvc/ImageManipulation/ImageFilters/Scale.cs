using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Q42.Wheels.Gimmage.ImageManipulation
{
  public partial class ImageFilters
  {
    /// <summary>
    /// Scales the image to match maxwidth or maxheight
    /// </summary>
    /// <param name="maxWidth">Maximum width, on a landscape image this will be used</param>
    /// <param name="maxHeight">Maximum height, on a portrait image this will be used</param>
    /// <param name="enlarge">If true, smaller images enlarge to match either maxWidth or maxHeight. Otherwise they stay their original size.</param>
    /// <returns></returns>
    public static Filter Scale(int maxWidth, int maxHeight, bool enlarge)
    {
      return delegate(Bitmap bmp)
      {
        // als hij niet resized hoeft te worden, return origineel
        if (!(((maxHeight < bmp.Height || maxWidth < bmp.Width) && !enlarge) || (enlarge && (maxHeight != bmp.Height || maxWidth != bmp.Width))))
          return bmp;

        double dblHeightDivider = Convert.ToDouble(bmp.Height) / Convert.ToDouble(maxHeight);
        double dblWidthDivider = Convert.ToDouble(bmp.Width) / Convert.ToDouble(maxWidth);
        int intImageHeight;
        int intImageWidth;
        int x = 0;
        int y = 0;
        // Calculate scale.
        if (dblHeightDivider > dblWidthDivider)
          maxWidth = Convert.ToInt32(bmp.Width / dblHeightDivider);
        else
          maxHeight = Convert.ToInt32(bmp.Height / dblWidthDivider);

        intImageHeight = maxHeight;
        intImageWidth = maxWidth;

        // Transform image.
        Bitmap bmpNew = new Bitmap(maxWidth, maxHeight);
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