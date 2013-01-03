using Q42.Wheels.Gimmage.Interfaces;

namespace Q42.Wheels.Gimmage.Sources
{
  public class Source : ISource
  {
    public Source(string name, string source, SourceType type)
    {
      Name = name;
      SourcePath = source;
      Type = type;
    }

    public string SourcePath { get; set; }

    public string Name { get; set; }

    string ISource.Source
    {
      get { return SourcePath; }
    }

    public SourceType Type { get; set; }
  }
}
