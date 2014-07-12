using SharpAccessory.Resources;
using SharpAccessory.VisualComponents;
using System;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using VMscope.VMSlideExplorer.VisualComponents;

namespace TestPlugin
{
  public class BTHandler : WsiHandler
  {

    private float receivedScale = 1;

    public BTHandler(WsiComposite wsiComposite)
      : base(wsiComposite)
    {
      wsiComposite.Tile.WsiBox.WsiNavigation.Changed += OnWsiNavigationChanged;
    }

    private void OnWsiNavigationChanged(object sender, EventArgs e)
    {
      ImageBoxNavigator nav = sender as ImageBoxNavigator;
      receivedScale = nav.Zoom;
    }

    public void ScaleView(Vector p)
    {
      if (WsiComposite.IsSelected)
      {
        ImageBoxNavigator nav = WsiComposite.Tile.WsiBox.WsiNavigation;

        receivedScale *= (float)p.X;

        //MessageBox.Show("receivedScale: " + receivedScale);

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

        //MessageBox.Show(
        //  "IsMaximumZoom: " + nav.IsMaximumZoom + "\n" +
        //  "IsMinimumZoom: " + nav.IsMinimumZoom + "\n" +
        //  "MaximumZoom: " + nav.MaximumZoom + "\n" +
        //  "MinimumZoom: " + nav.MinimumZoom + "\n" +
        //  "Zoom: " + nav.Zoom + "\n" +
        //  "ZoomInOutFactor: " + nav.ZoomInOutFactor
        //);
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
        float neuMidY = bitmapMidY + scale1 * (float)vec.Y;
        float neuMidX = bitmapMidX + scale1 * (float)vec.X;

        // Translation zun neuem Mittelpunkt
        nav.Goto(nav.Zoom, new PointF(neuMidX, neuMidY));
      }
    }
  }
}
