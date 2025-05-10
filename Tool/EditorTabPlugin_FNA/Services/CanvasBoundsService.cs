using Gum;
using Gum.Plugins.InternalPlugins.EditorTab.Services;
using RenderingLibrary;
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

    public void Initialize(LayerService layerService, SystemManagers systemManagers)
    {
        mCanvasBounds = new LineRectangle(systemManagers);
        mCanvasBounds.IsDotted = true;
        mCanvasBounds.Name = "Gum Screen Bounds";
        mCanvasBounds.Width = 800;
        mCanvasBounds.Height = 600;
        mCanvasBounds.Color = ScreenBoundsColor;

        systemManagers.ShapeManager.Add(mCanvasBounds, layerService.OverlayLayer);

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
