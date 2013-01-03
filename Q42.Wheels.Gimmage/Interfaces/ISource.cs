
namespace Q42.Wheels.Gimmage.Interfaces
{
  public interface ISource
  {
    string Name { get; }
    string Source { get; }
    SourceType Type { get; }
  }
}
