using System;
using System.Windows.Forms;
using RenderingLibrary;
using RenderingLibrary.Graphics;
using Gum.Managers;
using MonoGameGum;
using WPFNA;
using System.Windows;

namespace Gum.Plugins.InternalPlugins.EditorTab.Services
{
    public class CameraController 
    {
        public Camera Camera
        {
            get;
            set;
        }

        //WireframeEditControl mWireframeEditControl;

        public event Action CameraChanged;
        HotkeyManager _hotkeyManager;

        public CameraController( HotkeyManager hotkeyManager)
        {
            _hotkeyManager = hotkeyManager;
        }

        public void Initialize(Camera camera, HotkeyManager hotkeyManager)
        {
            //mWireframeEditControl = wireframeEditControl;

            //Renderer.Self.Camera.X = defaultWidth / 2 - 30;
            //Renderer.Self.Camera.Y = defaultHeight / 2 - 30;
        }


        internal void HandleKeyPress(KeyEventArgs e)
        {
            if (_hotkeyManager.MoveCameraLeft.IsPressed(e))
            {
                SystemManagers.Default.Renderer.Camera.X -= 10 / SystemManagers.Default.Renderer.Camera.Zoom;
                CameraChanged?.Invoke();
            }
            if (_hotkeyManager.MoveCameraRight.IsPressed(e))
            {
                SystemManagers.Default.Renderer.Camera.X += 10 / SystemManagers.Default.Renderer.Camera.Zoom;
                CameraChanged?.Invoke();
            }
            if (_hotkeyManager.MoveCameraUp.IsPressed(e))
            {
                SystemManagers.Default.Renderer.Camera.Y -= 10 / SystemManagers.Default.Renderer.Camera.Zoom;
                CameraChanged?.Invoke();
            }
            if (_hotkeyManager.MoveCameraDown.IsPressed(e))
            {
                SystemManagers.Default.Renderer.Camera.Y += 10 / SystemManagers.Default.Renderer.Camera.Zoom;
                CameraChanged?.Invoke();
            }

            if (_hotkeyManager.ZoomCameraIn.IsPressed(e) || _hotkeyManager.ZoomCameraInAlternative.IsPressed(e))
            {
                //mWireframeEditControl.ZoomIn();
                CameraChanged?.Invoke();
            }

            if (_hotkeyManager.ZoomCameraOut.IsPressed(e) || _hotkeyManager.ZoomCameraOutAlternative.IsPressed(e))
            {
                //mWireframeEditControl.ZoomOut();
                CameraChanged?.Invoke();
            }
        }


    }
}
