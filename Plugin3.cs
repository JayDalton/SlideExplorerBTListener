using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

using SharpAccessory.Resources;
using SharpAccessory.VisualComponents;

using SharpAccessory.GenericBusinessClient.Plugging;

using VMscope.VMSlideExplorer;
using VMscope.VMSlideExplorer.VisualComponents;


namespace TestPlugin
{

  public class Plugin3 : Plugin
  {
    private Dictionary<ImageBoxNavigator, PointF> offset;
    private bool ignoreNavigation = false;
    private WsiToolButton wtbConnect;
    private WsiToolButton wtbZoomOut;
    private WsiToolButton wtbZoomIn;
    private Microscope microscope;

    protected override void OnBroadcastContext(BroadcastContextEventArgs e)
    {
      base.OnBroadcastContext(e);

      if (e.Context is Microscope)
      {
        microscope = e.Context as Microscope;
        microscope.WsiCompositesChanged += OnWsiCompositesChanged;

        wtbConnect = microscope.ToolBar.CreateToolButton();
        wtbConnect.Image = TangoIconSet.LoadIcon(TangoIcon.Input_Gaming);
        wtbConnect.ToolTipText = "Connect navigation";
        wtbConnect.ShortcutKeys = Keys.T;
        wtbConnect.Click += delegate { ToggleConnect(); };

        wtbZoomIn = microscope.ToolBar.CreateToolButton();
        wtbZoomIn.ToolTipText = "Zoom In";

        wtbZoomOut = microscope.ToolBar.CreateToolButton();
        wtbZoomOut.ToolTipText = "Zoom Out";
      }
    }

    private void OnWsiCompositesChanged(object sender, EventArgs e)
    {
      wtbConnect.Checked = false;

      for (int i = 0; i < microscope.WsiComposites.Count; i++)
      {
        microscope.WsiComposites[i].Tile.WsiBox.WsiNavigation.Changed += OnWsiNavigationChanged;
      }
    }


    private void OnWsiNavigationChanged(object sender, EventArgs e)
    {
      if (ignoreNavigation) return;

      if (!wtbConnect.Checked) return;

      ImageBoxNavigator nav = sender as ImageBoxNavigator;

      float y = nav.SrcRectangle.Y + (nav.SrcRectangle.Height - 1) / 2F;
      float x = nav.SrcRectangle.X + (nav.SrcRectangle.Width - 1) / 2F;

      float dx = x - offset[nav].X;
      float dy = y - offset[nav].Y;

      ignoreNavigation = true;

      for (int i = 0; i < microscope.WsiComposites.Count; i++)
      {
        if (!microscope.WsiComposites[i].IsSelected) continue;

        if (microscope.WsiComposites[i].Tile.WsiBox.WsiNavigation == nav) continue;

        PointF off = offset[microscope.WsiComposites[i].Tile.WsiBox.WsiNavigation];

        microscope.WsiComposites[i].Tile.WsiBox.WsiNavigation.Goto(nav.Zoom, new PointF(off.X + dx, off.Y + dy));
      }

      ignoreNavigation = false;
    }


    private void ToggleConnect()
    {
      wtbConnect.Checked = !wtbConnect.Checked;

      offset = new Dictionary<ImageBoxNavigator, PointF>();

      for (int i = 0; i < microscope.WsiComposites.Count; i++)
      {
        ImageBoxNavigator nav = microscope.WsiComposites[i].Tile.WsiBox.WsiNavigation;

        float y = nav.SrcRectangle.Y + (nav.SrcRectangle.Height - 1) / 2F;
        float x = nav.SrcRectangle.X + (nav.SrcRectangle.Width - 1) / 2F;

        offset.Add(nav, new PointF(x, y));
      }
    }

  }
}