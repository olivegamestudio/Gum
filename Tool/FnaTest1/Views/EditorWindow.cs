using EditorTabPlugin_FNA.LibraryFiles;
using EditorTabPlugin_FNA.Services;
using EditorTabPlugin_FNA.ViewModels;
using EditorTabPlugin_XNA.Services;
using Gum.Plugins.InternalPlugins.EditorTab.Services;
using Microsoft.Xna.Framework;
using RenderingLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFNA;

namespace EditorTabPlugin_FNA.Views;
internal class EditorWindow : FnaControl
{
    private readonly CameraController _cameraController;
    private readonly EditorGame _game;
    private readonly BackgroundSpriteService _backgroundSpriteService;
    private readonly CanvasBoundsService _canvasBoundsService;

    CameraViewModel ViewModel => DataContext as CameraViewModel;

    public EditorWindow(
        CameraController cameraController, 
        BackgroundSpriteService backgroundSpriteService, 
        EditorGame game,
        CanvasBoundsService canvasBoundsService
        ) : base(game)
    {
        _cameraController = cameraController;
        _game = game;
        _backgroundSpriteService = backgroundSpriteService;
        _canvasBoundsService = canvasBoundsService;

        _cameraController.Camera = _game.SystemManagers.Renderer.Camera;
        _game.Updated += HandleEveryFrameUpdate;
        MouseDownFNA += HandleMouseDown;
        MouseMoveFNA += HandleMouseMove;
        MouseWheelFNA += HandleMouseWheel;
    }

    private void HandleEveryFrameUpdate(GameTime time)
    {
        _backgroundSpriteService.Activity();
        _canvasBoundsService.Activity();
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
