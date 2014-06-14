using SharpAccessory.Resources;
using SharpAccessory.VisualComponents;
using System;
using System.Drawing;
using VMscope.VMSlideExplorer.VisualComponents;

namespace TestPlugin
{
  public class BTHandler : WsiHandler
  {

    private double viewWidth;
    public double ViewWidth
    {
      get { return viewWidth; }
      set
      {
        if (value != viewWidth)
        {
          viewWidth = value;
        }
      }
    }

    private double viewHeight;
    public double ViewHeight
    {
      get { return viewHeight; }
      set
      {
        if (value != viewHeight)
        {
          viewHeight = value;
        }
      }
    }

    public BTHandler(WsiComposite wsiComposite)
      : base(wsiComposite)
    {
      WsiToolButton wtbThreshold;

      //wtbThreshold = wsiComposite.Tile.ToolBar.CreateToolButton();
      //wtbThreshold.Image = SharpAccessoryIconSet.LoadIcon(SharpAccessoryIcon.Cursor);
      //wtbThreshold.ToolTipText = "Connect by Select";
      //wtbThreshold.Click += delegate { ZoomIn(); };

      //wtbThreshold = wsiComposite.Tile.ToolBar.CreateToolButton();
      //wtbThreshold.Image = SharpAccessoryIconSet.LoadIcon(SharpAccessoryIcon.Cursor);
      //wtbThreshold.ToolTipText = "Connect by Address";
      //wtbThreshold.Click += delegate { ZoomIn(); };

      wtbThreshold = wsiComposite.Tile.ToolBar.CreateToolButton();
      wtbThreshold.Image = SharpAccessoryIconSet.LoadIcon(SharpAccessoryIcon.Cursor);
      wtbThreshold.ToolTipText = "Perform Zoom In";
      wtbThreshold.Click += delegate { ZoomIn(); };

      wtbThreshold = wsiComposite.Tile.ToolBar.CreateToolButton();
      wtbThreshold.Image = SharpAccessoryIconSet.LoadIcon(SharpAccessoryIcon.Cursor);
      wtbThreshold.ToolTipText = "Perform Zoom Out";
      wtbThreshold.Click += delegate { ZoomOut(); };

      //wsiComposite.Tile.WsiBox.WsiNavigation.Changed += OnWsiNavigationChanged;
    }

    private void OnWsiNavigationChanged(object sender, EventArgs e)
    {
      ImageBoxNavigator nav = sender as ImageBoxNavigator;
      //viewWidth = nav.DstRectangle.Width;
      //viewHeight = nav.DstRectangle.Height;
    }

    public void TranslateView(System.Drawing.PointF p)
    {
      if (WsiComposite.IsSelected)
      {
        ImageBoxNavigator nav = WsiComposite.Tile.WsiBox.WsiNavigation;

        float scale1 = nav.SrcRectangle.Height / nav.DstRectangle.Height;
        float scale2 = nav.SrcRectangle.Width / nav.DstRectangle.Width;

        //WsiComposite.Wsi.Size;
        //// P2 in Screen
        //float pNewY = nav.DstRectangle.Y + (nav.DstRectangle.Height * p.Y);
        //float pNewX = nav.DstRectangle.X + (nav.DstRectangle.Width * p.X);
        //// Mid in Screen
        //float screenMidY = (nav.DstRectangle.Height - 1) / 2F;
        //float screenMidX = (nav.DstRectangle.Width - 1) / 2F;
        // Mid in Source
        float bitmapMidY = nav.SrcRectangle.Y + (nav.SrcRectangle.Height - 1) / 2F;
        float bitmapMidX = nav.SrcRectangle.X + (nav.SrcRectangle.Width - 1) / 2F;
        //// Vector to SrcMid
        //float dstVecY = pNewY - screenMidY;
        //float dstVecX = pNewX - screenMidX;
        // Vector to BmpMid
        float neuMidY = bitmapMidY + scale1 * p.Y;
        float neuMidX = bitmapMidX + scale1 * p.X;

        //MessageBox.Show(
        //  string.Format(
        //    "Old:{0} {1}\nNew:{2} {3}\nBMP:{4} {5}\nSCR:{6} {7}\nNEU:{8} {9}", 
        //    pOldX, pOldY, pNewX, pNewY, bitmapMidX, bitmapMidY, 
        //    screenMidX, screenMidY, neuMidX, neuMidY
        //  )
        //);

        nav.Goto(nav.Zoom, new PointF(neuMidX, neuMidY));
      }
    }

    private void ZoomIn()
    {
      ImageBoxNavigator nav = WsiComposite.Tile.WsiBox.WsiNavigation;

      float y = nav.SrcRectangle.Y + (nav.SrcRectangle.Height - 1) / 2F;
      float x = nav.SrcRectangle.X + (nav.SrcRectangle.Width - 1) / 2F;

      nav.Goto(nav.Zoom, new PointF(x, y));
    }

    private void ZoomOut()
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
