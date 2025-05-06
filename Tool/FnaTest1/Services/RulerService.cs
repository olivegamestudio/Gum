using Gum;
using Gum.Managers;
using Gum.Plugins.InternalPlugins.EditorTab.Services;
using Gum.Plugins.InternalPlugins.EditorTab.Views;
using MonoGameGum.Input;
using RenderingLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EditorTabPlugin_FNA.Services;
internal class RulerService
{
    Ruler mTopRuler;
    Ruler mLeftRuler;
    private SystemManagers _systemManagers;
    public bool IsCursorOverRulers
    {
        get; private set;
    }
    public bool RulersVisible
    {
        get => mLeftRuler.Visible;
        set
        {
            mLeftRuler.Visible = value;
            mTopRuler.Visible = value;
        }
    }

    public RulerService()
    {
    }

    public void Initialize(LayerService layerService,
        SystemManagers systemManagers,
        Cursor cursor,
        Keyboard keyboard,
        ToolFontService toolFontService,
        ToolLayerService toolLayerService)
    { 
        mTopRuler = new Ruler(systemManagers,
            cursor,
            keyboard,
            toolFontService,
            toolLayerService,
            layerService);
        mLeftRuler = new Ruler(systemManagers,
            cursor,
            keyboard,
            toolFontService,
            toolLayerService,
            layerService);
        mLeftRuler.RulerSide = RulerSide.Left;

        _systemManagers = systemManagers;
    }

    public void Activity()
    {
        mLeftRuler.ZoomValue = _systemManagers.Renderer.Camera.Zoom;
        mTopRuler.ZoomValue = _systemManagers.Renderer.Camera.Zoom;

        IsCursorOverRulers = mTopRuler.HandleXnaUpdate() ||
            mLeftRuler.HandleXnaUpdate();

        var gumProject = ProjectManager.Self.GumProjectSave;
        if(gumProject != null)
        {
            RulersVisible = gumProject.ShowRuler;
        }

    }

    public void RefreshGuides()
    {
        // setting GuideValues forces a refresh
        mTopRuler.GuideValues = mTopRuler.GuideValues.ToArray();

        mLeftRuler.GuideValues = mLeftRuler.GuideValues.ToArray();
    }
}
