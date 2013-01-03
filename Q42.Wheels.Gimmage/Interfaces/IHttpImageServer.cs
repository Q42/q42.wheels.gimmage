using System.Web;
using System;

namespace Q42.Wheels.Gimmage.Interfaces
{
  public interface IHttpImageServer : IImageServer
  {
    void ServeImage(HttpResponse Response, HttpRequest request);
    void ServeImage(string fileName, string templateName, HttpResponse response);    
    void ServeImage(string fileName, string sourceStr, string templateStr, HttpResponse response, HttpRequest request);
    void ServeImage(string fileName, ISource source, IImageTemplate template, HttpResponse response, HttpRequest request);
    void ServeImage(string fileName, byte[] fileContent, DateTime lastWriteTime, string mimetype, ISource source, IImageTemplate template, HttpResponse response, HttpRequest request);
    void DeleteImage(string source, string filename);
  }
}
