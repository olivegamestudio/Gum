using EditorTabPlugin_FNA.LibraryFiles;
using EditorTabPlugin_FNA.ViewModels;
using Gum.Plugins.InternalPlugins.EditorTab.Services;
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

    // to do- make this not use the default, instead give it its own
    // instance:
    public Camera Camera => SystemManagers.Default.Renderer.Camera;

    CameraViewModel ViewModel => DataContext as CameraViewModel;

    public EditorWindow(CameraController cameraController, FnaGame fnaGame) : base(fnaGame)
    {
        _cameraController = cameraController;
        _cameraController.Camera = Camera;

        MouseDownFNA += HandleMouseDown;
        MouseMoveFNA += HandleMouseMove;
        MouseWheelFNA += HandleMouseWheel;
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
