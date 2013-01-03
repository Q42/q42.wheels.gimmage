using System;
using System.Linq;

namespace System.Web.Mvc.Html
{
  public static class GimmageHelper
  {
    public static MvcHtmlString Gimmage(this HtmlHelper helper, string source, string filename, string template)
    {
      TagBuilder builder = new TagBuilder("img");

      // TODO: check params: source, template, filename
      builder.Attributes.Add("src", GimmageUrl(source, template, filename));

      return MvcHtmlString.Create(builder.ToString(TagRenderMode.SelfClosing));
    }

    public static string GimmageUrl(string source, string template, string filename)
    {
      return "";
    }

  }
}
