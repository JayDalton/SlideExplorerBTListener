using BTControl;
using SharpAccessory.Resources;
using SharpAccessory.VisualComponents;
using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using VMscope.VMSlideExplorer.VisualComponents;

namespace TestPlugin
{
  public class BTHandler : WsiHandler
  {

    private ConcurrentQueue<BTContent> input;
    private float barrierdScale = 0;
    private float receivedScale = 1;

    
    public BTHandler(WsiComposite wsiComposite)
      : base(wsiComposite)
    {
      input = new ConcurrentQueue<BTContent>();
      ImageBoxNavigator nav = WsiComposite.Tile.WsiBox.WsiNavigation;
      nav.Changed += OnWsiNavigationChanged;
      barrierdScale = nav.Zoom;
      //startInputProcessor();
    }

    private void OnWsiNavigationChanged(object sender, EventArgs e)
    {
      ImageBoxNavigator nav = sender as ImageBoxNavigator;
      if (barrierdScale != nav.Zoom)
      {
        barrierdScale = nav.Zoom;
        receivedScale = nav.Zoom;
      }
    }

    public void ScaleView(Vector p)
    {
      if (WsiComposite.IsSelected)
      {
        ImageBoxNavigator nav = WsiComposite.Tile.WsiBox.WsiNavigation;

        // Änderung zuweisen
        receivedScale *= ((float)p.X + (float)p.Y) / 2F;

        // Grenze nach oben
        if (receivedScale > nav.Zoom * nav.ZoomInOutFactor)
        {
          if (!nav.IsMinimumZoom)
          {
            nav.ZoomIn();
          }
        }

        // Grenze nach unten
        if (receivedScale < nav.Zoom / nav.ZoomInOutFactor)
        {
          if (!nav.IsMaximumZoom)
          {
            nav.ZoomOut();
          }
        }

      }
    }

    public void TranslateView(Vector vec)
    {
      if (WsiComposite.IsSelected)
      {
        ImageBoxNavigator nav = WsiComposite.Tile.WsiBox.WsiNavigation;

        // Skalierung ermitteln
        float scale1 = nav.SrcRectangle.Height / nav.DstRectangle.Height;
        float scale2 = nav.SrcRectangle.Width / nav.DstRectangle.Width;

        // aktuellen Mittelpunkt ermitteln
        float bitmapMidY = nav.SrcRectangle.Y + (nav.SrcRectangle.Height - 1) / 2F;
        float bitmapMidX = nav.SrcRectangle.X + (nav.SrcRectangle.Width - 1) / 2F;

        // neuen Mittelpunkt ermitteln
        Vector v = Vector.Add(new Vector(bitmapMidX, bitmapMidY), Vector.Multiply(scale1, vec));
        float neuMidY = bitmapMidY - scale1 * (float)vec.Y;
        float neuMidX = bitmapMidX - scale1 * (float)vec.X;

        // Translation zun neuem Mittelpunkt
        nav.Goto(nav.Zoom, new PointF(neuMidX, neuMidY));
      }
    }

//#region Test

//    public void addInput(BTContent content)
//    {
//      input.Enqueue(content);
//    }

//    private void startInputProcessor()
//    {
//      Thread worker = new Thread(new ThreadStart(Process_Runner));
//      worker.Name = "InputProcessor";
//      worker.IsBackground = true;
//      worker.Priority = ThreadPriority.Lowest;
//      worker.Start();
//    }

//    private void Process_Runner()
//    {
//      BTContent result;
//      while (true)
//      {
//        if (input.TryDequeue(out result))
//        {
//          handleInput(result.Data);
//        }
//      }
//    }

//    /*
// * M:Hello World!
// * T:12.345678;43.212313 
// * P:12.345678;43.213123
// * S:12.345678;43.213123
// * C: Composite (ID)
// * */

//    private void handleInput(string input)
//    {
//      char[] splitter1 = new char[] { ':' };
//      char[] splitter2 = new char[] { ';' };
//      string[] part = input.Split(splitter1, StringSplitOptions.RemoveEmptyEntries);
//      if (0 < part.Length)
//      {
//        string[] value = part[1].Split(splitter2, StringSplitOptions.RemoveEmptyEntries);
//        if (0 < value.Length)
//        {
//          switch (part[0])
//          {
//            case "S": // Scale
//              ScaleView(new Vector(
//                float.Parse(value[0], CultureInfo.InvariantCulture),
//                float.Parse(value[1], CultureInfo.InvariantCulture)
//              ));
//              break;
//            case "T": // Trans
//              TranslateView(new Vector(
//                float.Parse(value[0], CultureInfo.InvariantCulture),
//                float.Parse(value[1], CultureInfo.InvariantCulture)
//              ));
//              break;
//          }
//        }
//      }
//    }
//#endregion
  }


}
