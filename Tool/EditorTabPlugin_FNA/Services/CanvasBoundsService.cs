using Gum;
using Gum.Plugins.InternalPlugins.EditorTab.Services;
using RenderingLibrary.Math.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EditorTabPlugin_FNA.Services;
internal class CanvasBoundsService
{
    LineRectangle mCanvasBounds;

    public bool CanvasBoundsVisible
    {
        get => mCanvasBounds.Visible;
        set => mCanvasBounds.Visible = value;
    }

    public Color ScreenBoundsColor = Color.LightBlue;


    public LineRectangle ScreenBounds
    {
        get { return mCanvasBounds; }
    }

    public void Initialize(LayerService layerService)
    {
        mCanvasBounds = new LineRectangle();
        mCanvasBounds.IsDotted = true;
        mCanvasBounds.Name = "Gum Screen Bounds";
        mCanvasBounds.Width = 800;
        mCanvasBounds.Height = 600;
        mCanvasBounds.Color = ScreenBoundsColor;

        ShapeManager.Self.Add(mCanvasBounds, layerService.OverlayLayer);

    }

    public void Activity()
    {
        var gumProject = ProjectManager.Self.GumProjectSave;
        if (mCanvasBounds != null && gumProject != null)
        {
            mCanvasBounds.Width = gumProject.DefaultCanvasWidth;
            mCanvasBounds.Height = gumProject.DefaultCanvasHeight;

            CanvasBoundsVisible = gumProject.ShowCanvasOutline;
        }
    }

}
