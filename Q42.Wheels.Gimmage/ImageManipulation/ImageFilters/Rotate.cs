using System;
using System.Drawing;

namespace Q42.Wheels.Gimmage.ImageManipulation
{
  public partial class ImageFilters
  {
    /// <summary>
    /// Rotate the image. The result image will be larger than the original if the degrees is not dividable by 90.
    /// </summary>
    /// <param name="degrees">Amount of degrees to rotate the image</param>
    /// <returns></returns>
    public static Filter Rotate(int degrees)
    {
      return delegate(Bitmap bmp)
      {
        if (degrees == 0)
          return bmp;

        switch ((degrees % 360) / 90)
        {
          case 1:
          case -3:
            bmp.RotateFlip(RotateFlipType.Rotate90FlipNone);
            break;
          case 2:
          case -2:
            bmp.RotateFlip(RotateFlipType.Rotate180FlipNone);
            break;
          case 3:
          case -1:
            bmp.RotateFlip(RotateFlipType.Rotate270FlipNone);
            break;
          default:
            break;
        }

        double dblAngle = Convert.ToDouble(Math.Abs(degrees) % 90);

        double cosAlpha = Math.Cos(dblAngle / 180 * Math.PI);
        double sinAlpha = Math.Sin(dblAngle / 180 * Math.PI);

        double a = bmp.Width * sinAlpha;
        double b = bmp.Height * cosAlpha;
        double c = bmp.Width * cosAlpha;
        double d = bmp.Height * sinAlpha;

        int intHeight = Convert.ToInt32(a + b);
        int intWidth = Convert.ToInt32(c + d);

        int x = 0;
        int y = 0;
        if (degrees > 0)
          x = Convert.ToInt32(d);
        else
          y = Convert.ToInt32(a);

        Bitmap bmpNew = new Bitmap(intWidth, intHeight);
        bmpNew.MakeTransparent();

        Graphics g = Graphics.FromImage(bmpNew);
        g.TranslateTransform(x, y);
        g.RotateTransform(degrees % 90);
        g.DrawImage(bmp, 0, 0, bmp.Width, bmp.Height);

        g.Save();
        bmp.Dispose();
        g.Dispose();
        return bmpNew;
      };
    }
  }
}