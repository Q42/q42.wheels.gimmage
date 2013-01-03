using Q42.Wheels.Gimmage.Interfaces;

namespace Q42.Wheels.Gimmage
{
  public class GimmageModel
  {
    public string FileName { get; set; }
    public string TemplateStr { get; set; }
    public string SourceStr { get; set; }
    public IImageTemplate Template 
    { 
      get 
      { 
        return GimmageServer.Server.GetTemplate(TemplateStr); 
      } 
    }
    public ISource Source 
    { 
      get 
      {
        return GimmageServer.Server.GetSource(SourceStr);
      } 
    }
  }
}
