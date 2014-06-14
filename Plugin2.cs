using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;

using SharpAccessory.Resources;
using SharpAccessory.VisualComponents;

using SharpAccessory.GenericBusinessClient.Plugging;

using SharpAccessory.GenericBusinessClient.DefaultPlugins;

using VMscope.VMSlideExplorer;
using VMscope.VMSlideExplorer.VisualComponents;


namespace TestPlugin
{

  public class Plugin2 : Plugin
  {
    private WsiToolButton wtbShowMessage;
    private WsiToolButton wtbDock;
    private Microscope microscope;
    private TextBox tb;

    protected override void OnBroadcastContext(BroadcastContextEventArgs e)
    {
      base.OnBroadcastContext(e);

      if (e.Context is Microscope)
      {
        microscope = e.Context as Microscope;
        microscope.WsiCompositesChanged += OnWsiCompositesChanged;
        microscope.WsiResolve += OnWsiResolve;
        //microscope.WsiComposites[0].Tile.WsiBox.WsiNavigation.Goto()

        wtbShowMessage = microscope.ToolBar.CreateToolButton();
        wtbShowMessage.Image = TangoIconSet.LoadIcon(TangoIcon.Preferences_System_Windows);
        wtbShowMessage.ToolTipText = "Show message";
        wtbShowMessage.Click += delegate { ShowMessage("Hallo Welt."); };

        wtbDock = microscope.ToolBar.CreateToolButton();
        wtbDock.Image = TangoIconSet.LoadIcon(TangoIcon.Window_New);
        wtbDock.ToolTipText = "Show dock";
        wtbDock.Checked = true;
        wtbDock.Click += delegate { ToggleDock(); };

        tb = new TextBox();
        tb.Parent = microscope.DockAreas.Bottom;
        tb.ScrollBars = ScrollBars.Both;
        tb.Dock = DockStyle.Bottom;
        tb.BackColor = Color.Red;
        tb.WordWrap = false;
        tb.Multiline = true;
        tb.Visible = true;
        tb.Height = 100;
      }
    }

    private void OnWsiCompositesChanged(object sender, EventArgs e)
    {
      for (int i = 0; i < microscope.WsiComposites.Count; i++)
      {
        WsiComposite composite = microscope.WsiComposites[i];

        new ImageAnalysisHandler(composite);
        new WsiDockSample(composite);
      }
    }


    private void OnWsiResolve(object sender, WsiResolveEventArgs e)
    {
      string filename = new FileInfo(e.Url).Name;

      if (e.Wsi != null)
      {
        filename += "...good.";
      }
      else filename += "...bad.";

      tb.AppendText(filename + "\r\n");
    }


    private void ShowMessage(string message)
    {
      MessageDialogParameters dialog = MessageDialogParameters.Default;

      dialog.Buttons = DialogButtons.Ok;
      dialog.Message = message;
      dialog.FollowUp += delegate { ShowPlugin("Microscope"); };

      ShowPlugin("MessageDialog", dialog);
    }

    private void ToggleDock()
    {
      wtbDock.Checked = !wtbDock.Checked;

      tb.Visible = wtbDock.Checked;

      microscope.HideOverlay();
    }

  }
}