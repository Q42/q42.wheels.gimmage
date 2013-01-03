using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace Q42.Wheels.Gimmage.Tooling
{
  public class WebPreviewBuilder
  {
    private string _url = String.Empty;
    private string _filename = String.Empty;

    public WebPreviewBuilder(string url, string filename)
    {
      if (!filename.EndsWith(".png", StringComparison.CurrentCultureIgnoreCase))
        throw new ArgumentException("Filename must have png as extension");

      FileInfo file = new FileInfo(filename);
      if (!file.Directory.Exists)
        file.Directory.Create();

      _url = url;
      _filename = filename;
      // Path.Combine(Environment.CurrentDirectory, "output.bmp");
    }

    public void CreatePreview()
    {
      ThreadStart ts = new ThreadStart(this.doWork);
      Thread t = new Thread(ts);
      t.SetApartmentState(ApartmentState.STA);
      t.Start();

      // TODO find the proper way to wait for a thread
      // wait for the thread
      while (t.IsAlive)
        Thread.Sleep(25);
    }

    private void doWork()
    {
      Bitmap bitmap = getPreviewBitmap();

      bitmap.Save(_filename, ImageFormat.Png);
      bitmap.Dispose();
    }

    /// <summary>
    /// Deze kan niet public gemaakt worden, maar moet in een eigen thread draaien, helaas
    /// </summary>
    /// <returns></returns>
    private Bitmap getPreviewBitmap()
    {
      WebBrowser wb = new WebBrowser();
      wb.ScrollBarsEnabled = false;
      wb.Size = new Size(Width, Height);
      wb.ScriptErrorsSuppressed = true;
      wb.NewWindow += new System.ComponentModel.CancelEventHandler(wb_NewWindow);
      wb.Navigate(_url);
      // wait for it to load
      while (wb.ReadyState != WebBrowserReadyState.Complete)
        Application.DoEvents();
      Bitmap bitmap = new Bitmap(Width, Height);
      Rectangle rect = new Rectangle(0, 0, Width, Height);
      wb.DrawToBitmap(bitmap, rect);
      return bitmap;
    }

    void wb_NewWindow(object sender, CancelEventArgs e)
    {
      e.Cancel = true;
    }

    private int _width = 1024;

    /// <summary>
    /// Width of the screenshot, default 1024
    /// </summary>
    public int Width
    {
      get { return _width; }
      set { _width = value; }
    }

    private int _height = 768;

    /// <summary>
    /// Height of the screenshot, default 768
    /// </summary>
    public int Height
    {
      get { return _height; }
      set { _height = value; }
    }
  }
}