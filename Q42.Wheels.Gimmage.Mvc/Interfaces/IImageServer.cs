using System.Collections.Generic;

namespace Q42.Wheels.Gimmage.Interfaces
{
  public interface IImageServer
  {
    IDictionary<string, ISource> GetSources();
    ISource GetSource(string sourceName);
    void AddSource(string name, string source, SourceType type);
    void AddSource(ISource source);
    IDictionary<string, IImageTemplate> GetTemplates();
    IImageTemplate GetTemplate(string templateName);
  }
}
