using SharpAccessory.Resources;
using SharpAccessory.VisualComponents;
using System;
using System.Drawing;
using System.Windows;
using VMscope.VMSlideExplorer.VisualComponents;

namespace TestPlugin
{
  public class BTHandler : WsiHandler
  {

    //private double viewWidth;
    //public double ViewWidth
    //{
    //  get { return viewWidth; }
    //  set
    //  {
    //    if (value != viewWidth)
    //    {
    //      viewWidth = value;
    //    }
    //  }
    //}

    //private double viewHeight;
    //public double ViewHeight
    //{
    //  get { return viewHeight; }
    //  set
    //  {
    //    if (value != viewHeight)
    //    {
    //      viewHeight = value;
    //    }
    //  }
    //}

    public BTHandler(WsiComposite wsiComposite)
      : base(wsiComposite)
    {
      //WsiToolButton wtbThreshold;

      //wtbThreshold = wsiComposite.Tile.ToolBar.CreateToolButton();
      //wtbThreshold.Image = SharpAccessoryIconSet.LoadIcon(SharpAccessoryIcon.Cursor);
      //wtbThreshold.ToolTipText = "Connect by Select";
      //wtbThreshold.Click += delegate { ZoomIn(); };

      //wtbThreshold = wsiComposite.Tile.ToolBar.CreateToolButton();
      //wtbThreshold.Image = SharpAccessoryIconSet.LoadIcon(SharpAccessoryIcon.Cursor);
      //wtbThreshold.ToolTipText = "Connect by Address";
      //wtbThreshold.Click += delegate { ZoomIn(); };

      //wtbThreshold = wsiComposite.Tile.ToolBar.CreateToolButton();
      //wtbThreshold.Image = SharpAccessoryIconSet.LoadIcon(SharpAccessoryIcon.Cursor);
      //wtbThreshold.ToolTipText = "Perform Zoom In";
      //wtbThreshold.Click += delegate { ZoomIn(); };

      //wtbThreshold = wsiComposite.Tile.ToolBar.CreateToolButton();
      //wtbThreshold.Image = SharpAccessoryIconSet.LoadIcon(SharpAccessoryIcon.Cursor);
      //wtbThreshold.ToolTipText = "Perform Zoom Out";
      //wtbThreshold.Click += delegate { ZoomOut(); };

      //wsiComposite.Tile.WsiBox.WsiNavigation.Changed += OnWsiNavigationChanged;
    }

    private void OnWsiNavigationChanged(object sender, EventArgs e)
    {
      //ImageBoxNavigator nav = sender as ImageBoxNavigator;
      //viewWidth = nav.DstRectangle.Width;
      //viewHeight = nav.DstRectangle.Height;
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

    public void ZoomIn(PointF p)
    {
      ImageBoxNavigator nav = WsiComposite.Tile.WsiBox.WsiNavigation;

      //nav.Goto(nav.Zoom, new PointF(x, y));
      //nav.Goto(nav.zo, p);
    }

    private void ZoomOut(PointF p)
    {
      ImageBoxNavigator nav = WsiComposite.Tile.WsiBox.WsiNavigation;

      //nav.DstRectangle.X = 20;
      //nav.DstRectangle.Y =+ 20;
      //float x = nav.SrcRectangle.Y + (nav.SrcRectangle.Height - 1) / 2F;
      //float y = nav.SrcRectangle.X + (nav.SrcRectangle.Width - 1) / 2F;

      //nav.Goto(nav.Zoom, new PointF(x, y));
    }


}
}
