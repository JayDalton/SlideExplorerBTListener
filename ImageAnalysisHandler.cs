using System;

using SharpAccessory.Resources;

using SharpAccessory.Imaging.Processors;
using SharpAccessory.Imaging.Segmentation;

using VMscope.VMSlideExplorer.VisualComponents;


namespace TestPlugin
{

  public class ImageAnalysisHandler : WsiHandler
  {
    private WsiToolButton wtbThreshold;


    public ImageAnalysisHandler(WsiComposite wsiComposite)
      : base(wsiComposite)
    {
      wtbThreshold = wsiComposite.Tile.ToolBar.CreateToolButton();
      wtbThreshold.Image = SharpAccessoryIconSet.LoadIcon(SharpAccessoryIcon.Objects_Filled);
      wtbThreshold.ToolTipText = "Perform Image Analysis";
      wtbThreshold.Click += delegate { PerformImageAnalysis(); };
    }


    private void PerformImageAnalysis()
    {
      if (WsiComposite.Tile.WsiBox.Image == null) return;

      GrayscaleProcessor p = new GrayscaleProcessor(WsiComposite.Tile.WsiBox.Image, RgbToGrayscaleConversion.Mean);
      p.WriteBack = false;

      ObjectLayer layer = new ThresholdSegmentation().Execute(p);

      p.Dispose();

      WsiComposite.Tile.WsiBox.ObjectLayer = layer;
    }

  }
}