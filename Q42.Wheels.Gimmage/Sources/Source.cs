using Q42.Wheels.Gimmage.Interfaces;

namespace Q42.Wheels.Gimmage.Sources
{
  internal class Source : ISource
  {
    public Source(string name, string source, SourceType type)
    {
      Name = name;
      SourcePath = source;
      Type = type;
    }

    /// <summary>
    /// only filled if Type == Type.db
    /// </summary>
    public string DatabaseConnectionStringName { get; set; }

    public string SourcePath { get; set; }

    public string Name { get; set; }

    string ISource.Source
    {
      get { return SourcePath; }
    }

    public SourceType Type { get; set; }
  }
}
