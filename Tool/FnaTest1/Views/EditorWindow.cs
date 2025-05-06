using EditorTabPlugin_FNA.LibraryFiles;
using EditorTabPlugin_FNA.Services;
using EditorTabPlugin_FNA.ViewModels;
using EditorTabPlugin_XNA.Services;
using Gum.Managers;
using Gum.Plugins.InternalPlugins.EditorTab.Services;
using Gum.Wireframe;
using Microsoft.Xna.Framework;
using RenderingLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolsUtilities;
using WPFNA;

namespace EditorTabPlugin_FNA.Views;
internal class EditorWindow : FnaControl
{
    bool mouseHasEntered = false;

    private readonly CameraController _cameraController;
    private readonly EditorGame _game;
    private readonly BackgroundSpriteService _backgroundSpriteService;
    private readonly CanvasBoundsService _canvasBoundsService;
    private readonly RulerService _rulerService;
    private readonly SelectionManager _selectionManager;

    CameraViewModel ViewModel => DataContext as CameraViewModel;

    public EditorWindow(
        CameraController cameraController, 
        BackgroundSpriteService backgroundSpriteService, 
        EditorGame game,
        CanvasBoundsService canvasBoundsService,
        RulerService rulerService,
        SelectionManager selectionManager
        ) : base(game)
    {
        _cameraController = cameraController;
        _game = game;
        _backgroundSpriteService = backgroundSpriteService;
        _canvasBoundsService = canvasBoundsService;
        _rulerService = rulerService;
        _selectionManager = selectionManager;

        _cameraController.Camera = _game.SystemManagers.Renderer.Camera;
        _game.Updated += HandleEveryFrameUpdate;
        MouseDownFNA += HandleMouseDown;
        MouseMoveFNA += HandleMouseMove;
        MouseWheelFNA += HandleMouseWheel;

        MouseEnterFNA += () =>
        {
            mouseHasEntered = true;
        };
        MouseLeaveFNA += () =>
        {
            mouseHasEntered = false;
        };
    }

    private void HandleEveryFrameUpdate(GameTime time)
    {
        try
        {
            _backgroundSpriteService.Activity();
            _canvasBoundsService.Activity();
            _rulerService.Activity();

            if (_rulerService.IsCursorOverRulers == false)
            {
                var doesTreeViewHaveMouseOver = ElementTreeViewManager.Self.HasMouseOver;
                var shouldForceNoHighlight = mouseHasEntered == false
                    // If the mouse is over the element tree view, we don't want to force unhlighlights since they can highlight when over the tree view items
                    && !doesTreeViewHaveMouseOver
                    ;
                _selectionManager.Activity(shouldForceNoHighlight);

                _selectionManager.LateActivity();
            }
        }

        catch
        {
            int m = 3;
            throw;
        }
    }

    internal void HandleMouseDown(System.Windows.Input.MouseButton e, Microsoft.Xna.Framework.Point position)
    {
        ViewModel.HandleMouseDown(e, position);
    }

    internal void HandleMouseWheel(int delta, Microsoft.Xna.Framework.Point position)
    {
        ViewModel.HandleMouseWheel(delta, position);
    }


    internal void HandleMouseMove(bool isMiddleMouseDown, Microsoft.Xna.Framework.Point position)
    {
        ViewModel.HandleMouseMove(isMiddleMouseDown, position);
    }
}
