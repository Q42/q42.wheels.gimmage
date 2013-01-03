using System;
using System.Text.RegularExpressions;

namespace Q42.Wheels.Gimmage
{
  public static class GimmageUrl
  {
    private static Regex ImageUrlRegex = new Regex("/upload/([^\"]*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    
    /// <summary>
    /// Convert the given <paramref name="rawHtml"/> with given <paramref name="template"/> to the appropiate Gimmage URL
    /// </summary>
    /// <param name="imageCmsUrl"></param>
    /// <param name="templateName"></param>
    /// <returns></returns>
    public static string Translate(string rawHtml, Type template)
    {
      if (rawHtml == null)
        return null;

      // Don't use standard Uri escaping, because there might be slashes that belong as part URL
      // Just escape spaces, because they are a problem with HTML validation
      var s = ImageUrlRegex.Replace(rawHtml, match => string.Format("/gimmage/N2/{0}/{1}", template.Name, match.Groups[1].Value.Replace(" ", "%20")));

      return s.Replace(@" border=""0""", "");
    }
  }
}