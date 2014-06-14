using System;
using System.Drawing;
using System.Windows.Forms;

using SharpAccessory.Resources;

using SharpAccessory.GenericBusinessClient.Plugging;
using SharpAccessory.GenericBusinessClient.VisualComponents;


namespace TestPlugin
{

  public class Plugin1 : Plugin
  {
    private ToolButton tbMicroscope;
    private ToolButton tbDock;
    private Panel pnlLeft;
    private Panel pnlTop;

    //private SharpAccessory.GenericBusinessClient.VisualComponents

    public Plugin1()
    {
      base.Text = "Demo Plugin 1";

      pnlLeft = new Panel();
      pnlLeft.BackColor = Color.Green;
      pnlLeft.Dock = DockStyle.Left;
      pnlLeft.Visible = false;
      pnlLeft.Width = 150;

      pnlTop = new Panel();
      pnlTop.BackColor = Color.Blue;
      pnlTop.Dock = DockStyle.Top;
      pnlTop.Visible = false;
      pnlTop.Height = 50;

      tbDock = new ToolButton();
      tbDock.Image = TangoIconSet.LoadIcon(TangoIcon.Window_New);
      tbDock.Text = "Show dock";
      tbDock.Click += delegate { ToggleDock(); };

      tbMicroscope = new ToolButton();
      tbMicroscope.Image = TangoIconSet.LoadIcon(TangoIcon.Network_Transmit);
      tbMicroscope.Text = "Mikroskop";
      tbMicroscope.Click += delegate { ShowPlugin("Microscope"); };

      TextBox tb = new TextBox();
      tb.BackColor = Color.Yellow;
      tb.Dock = DockStyle.Fill;
      tb.Multiline = true;
      tb.Parent = Control;
    }


    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);

      pnlLeft.Parent = DockAreas.Left;
      pnlTop.Parent = DockAreas.Top;
    }


    protected override ToolButton[] GetToolButtons()
    {
      return new ToolButton[] { tbDock, tbMicroscope };
    }


    protected override HomepageInfo GetHomepageInfo()
    {
      HomepageInfo info = HomepageInfo.Empty;

      info.ToolTipText = "Demo Plugin 1 tut nichts.";
      info.Name = "Demo Plugin 1";
      info.Index = 100;

      return info;
    }


    private void ToggleDock()
    {
      tbDock.Checked = !tbDock.Checked;

      pnlLeft.Visible = tbDock.Checked;
      pnlTop.Visible = tbDock.Checked;
    }

  }
}