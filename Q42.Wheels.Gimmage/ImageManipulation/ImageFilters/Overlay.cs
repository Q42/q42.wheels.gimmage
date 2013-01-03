using System.Drawing;
using System.Drawing.Drawing2D;

namespace Q42.Wheels.Gimmage.ImageManipulation
{
  public enum horizontalAlign { left, center, right }
  public enum verticalAlign { top, center, bottom }

  public partial class ImageFilters
  {
    /// <summary>
    /// Projects the Bitmap on a background
    /// </summary>
    /// <param name="backgroundWidth">Width of the background</param>
    /// <param name="backgroundHeight">Height of the background</param>
    /// <param name="backgroundHex">Hex value (#FF0000) of the color of the background</param>
    /// <param name="hAlign">Where to place the Bitmap on the background horizontally</param>
    /// <param name="vAlign">Where to place the Bitmap on the background vertically</param>
    /// <returns></returns>
    public static Filter Overlay(int backgroundWidth, int backgroundHeight, string backgroundHex, horizontalAlign hAlign, verticalAlign vAlign)
    {
      return delegate(Bitmap bmp)
      {
        Bitmap bmpBg = backgroundBitmap(ColorFromString(backgroundHex), backgroundWidth, backgroundHeight);
        return OverlayImage(bmpBg, bmp, hAlign, vAlign);
      };
    }

    public static Filter Overlay(string fileName)
    {
      return Overlay(fileName, horizontalAlign.left, verticalAlign.top);
    }

    public static Filter Overlay(string fileName, horizontalAlign hAlign, verticalAlign vAlign)
    {
      return delegate(Bitmap bmp)
      {
        Bitmap overlayImage = new Bitmap(fileName);
        return OverlayImage(bmp, overlayImage, hAlign, vAlign);
      };
    }

    #region privates

    private static Bitmap backgroundBitmap(Color bgColor, int width, int height)
    {
      Bitmap bmp = new Bitmap(width, height);
      Graphics bmpGraphic = Graphics.FromImage(bmp);

      Brush brush = new SolidBrush(bgColor);
      bmpGraphic.FillRectangle(brush, 0, 0, width, height);

      //Bitmap van zelfde kwaliteit maken als originele plaatje
      bmpGraphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
      bmpGraphic.SmoothingMode = SmoothingMode.HighQuality;
      bmpGraphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
      bmpGraphic.CompositingQuality = CompositingQuality.HighQuality;

      return bmp;
    }

    private static Color ColorFromString(string strColor)
    {
      Color color = Color.Empty;
      if (strColor.StartsWith("#"))
        color = ColorTranslator.FromHtml(strColor);
      else
        color = Color.FromName(strColor);
      return color;
    }


    private static Bitmap OverlayImage(Bitmap bottomLayer, Bitmap topLayer, horizontalAlign hAlign, verticalAlign vAlign)
    {
      Graphics bottomLayerGraphics = Graphics.FromImage(bottomLayer);

      int x, y;
      switch (hAlign)
      {
        case horizontalAlign.right:
          x = bottomLayer.Width - topLayer.Width;
          break;
        case horizontalAlign.center:
          x = (bottomLayer.Width - topLayer.Width) / 2;
          break;
        case horizontalAlign.left:
        default:
          x = 0;
          break;
      }
      switch (vAlign)
      {
        case verticalAlign.bottom:
          y = bottomLayer.Height - topLayer.Height;
          break;
        case verticalAlign.center:
          y = (bottomLayer.Height - topLayer.Height) / 2;
          break;
        case verticalAlign.top:
        default:
          y = 0;
          break;
      }

      //Bitmap van zelfde kwaliteit maken als originele plaatje
      bottomLayerGraphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
      bottomLayerGraphics.SmoothingMode = SmoothingMode.HighQuality;
      bottomLayerGraphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
      bottomLayerGraphics.CompositingQuality = CompositingQuality.HighQuality;

      // fix if the input bitmap is of another resolution than the generated graphic
      topLayer.SetResolution(bottomLayer.HorizontalResolution, bottomLayer.VerticalResolution);

      bottomLayerGraphics.DrawImage(topLayer, x, y);

      return bottomLayer;
    }

    #endregion
  }
}