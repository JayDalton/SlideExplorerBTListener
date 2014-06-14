using System;
using System.Drawing;
using System.Windows.Forms;

using SharpAccessory.Resources;

using SharpAccessory.Imaging.Processors;
using SharpAccessory.Imaging.Segmentation;

using VMscope.VMSlideExplorer.VisualComponents;

namespace TestPlugin
{

  public class WsiDockSample
  {
    private WsiToolButton wtbDock;
    private WsiComposite composite;
    private TreeView tv;

    public WsiDockSample(WsiComposite composite)
    {
      this.composite = composite;

      tv = new TreeView();
      tv.Parent = composite.Tile.DockAreas.Left;
      tv.BackColor = Color.LightGray;
      tv.Dock = DockStyle.Left;
      tv.Visible = true;
      tv.Width = 150;
      tv.Nodes.Add("Node 1");
      tv.Nodes.Add("Node 2");


      wtbDock = composite.Tile.ToolBar.CreateToolButton();
      wtbDock.Image = TangoIconSet.LoadIcon(TangoIcon.Window_New);
      wtbDock.ToolTipText = "Show dock";
      wtbDock.Click += delegate { ToggleDock(); };
    }


    private void ToggleDock()
    {
      wtbDock.Checked = !wtbDock.Checked;

      tv.Visible = wtbDock.Checked;
    }

  }
}