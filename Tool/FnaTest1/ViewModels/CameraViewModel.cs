using Gum.Managers;
using Gum.Mvvm;
using Gum.Plugins;
using RenderingLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EditorTabPlugin_FNA.ViewModels;
public class CameraViewModel : ViewModel
{
    List<int> _availableZoomLevels = new List<int>();
    Camera _camera;
    private readonly HotkeyManager _hotkeyManager;
    Microsoft.Xna.Framework.Point mLastMouseLocation;

    public List<int> AvailableZoomLevels =>
        _availableZoomLevels;

    public int SelectedZoomIndex
    {
        get => Get<int>();
        set
        {
            if (Set(value) && value >= 0 &&
                value < _availableZoomLevels.Count)
            {
                var zoomLevel = _availableZoomLevels[value];

                _camera.Zoom = zoomLevel / 100f;

                PluginManager.Self.CameraChanged();
            }
        }
    }

    public CameraViewModel(Camera camera, HotkeyManager hotkeyManager)
    {
        _camera = camera;
        _hotkeyManager = hotkeyManager;

        _availableZoomLevels.Add(1600);
        _availableZoomLevels.Add(1200);
        _availableZoomLevels.Add(1000);
        _availableZoomLevels.Add(800);
        _availableZoomLevels.Add(700);
        _availableZoomLevels.Add(600);
        _availableZoomLevels.Add(500);
        _availableZoomLevels.Add(400);
        _availableZoomLevels.Add(350);
        _availableZoomLevels.Add(300);
        _availableZoomLevels.Add(250);
        _availableZoomLevels.Add(200);
        _availableZoomLevels.Add(175);
        _availableZoomLevels.Add(150);
        _availableZoomLevels.Add(125);
        _availableZoomLevels.Add(100);
        _availableZoomLevels.Add(87);
        _availableZoomLevels.Add(75);
        _availableZoomLevels.Add(63);
        _availableZoomLevels.Add(50);
        _availableZoomLevels.Add(33);
        _availableZoomLevels.Add(25);
        _availableZoomLevels.Add(10);
        _availableZoomLevels.Add(5);

        //SelectedZoomIndex = _availableZoomLevels.IndexOf(100);
        // Don't set the property because doing so raises plugin events
        // but the plugin system isn't yet running:
        Set(_availableZoomLevels.IndexOf(100), nameof(SelectedZoomIndex));
    }


    public void ZoomOut()
    {
        int index = SelectedZoomIndex;

        if (index < _availableZoomLevels.Count - 1)
        {
            SelectedZoomIndex++;
        }

    }

    public void ZoomIn()
    {
        int index = SelectedZoomIndex;

        if (index > 0)
        {
            SelectedZoomIndex--;
        }
    }

    public void HandleMouseDown(System.Windows.Input.MouseButton e, Microsoft.Xna.Framework.Point position)
    {
        mLastMouseLocation = position;

    }

    public bool HandleKeyPress(System.Windows.Forms.KeyEventArgs e)
    {
        if (_hotkeyManager.MoveCameraLeft.IsPressed(e))
        {
            SystemManagers.Default.Renderer.Camera.X -= 10 / SystemManagers.Default.Renderer.Camera.Zoom;
            PluginManager.Self.CameraChanged();

            return true;
        }
        if (_hotkeyManager.MoveCameraRight.IsPressed(e))
        {
            SystemManagers.Default.Renderer.Camera.X += 10 / SystemManagers.Default.Renderer.Camera.Zoom;
            PluginManager.Self.CameraChanged();
            return true;
        }
        if (_hotkeyManager.MoveCameraUp.IsPressed(e))
        {
            SystemManagers.Default.Renderer.Camera.Y -= 10 / SystemManagers.Default.Renderer.Camera.Zoom;
            PluginManager.Self.CameraChanged();
            return true;
        }
        if (_hotkeyManager.MoveCameraDown.IsPressed(e))
        {
            SystemManagers.Default.Renderer.Camera.Y += 10 / SystemManagers.Default.Renderer.Camera.Zoom;
            PluginManager.Self.CameraChanged();
            return true;
        }

        if (_hotkeyManager.ZoomCameraIn.IsPressed(e) || _hotkeyManager.ZoomCameraInAlternative.IsPressed(e))
        {
            ZoomIn();
            return true;
        }

        if (_hotkeyManager.ZoomCameraOut.IsPressed(e) || _hotkeyManager.ZoomCameraOutAlternative.IsPressed(e))
        {
            ZoomOut();
            return true;
        }
        return false;
    }

    internal void HandleMouseWheel(int delta, Microsoft.Xna.Framework.Point position)
    {
        float worldX, worldY;
        _camera.ScreenToWorld(position.X, position.Y, out worldX, out worldY);
        float differenceX = _camera.X - worldX;
        float differenceY = _camera.Y - worldY;

        float oldZoom = _camera.Zoom;

        if (delta < 0)
        {
            ZoomOut();

        }
        else
        {
            ZoomIn();
        }

        float newDifferenceX = differenceX * oldZoom / _camera.Zoom;
        float newDifferenceY = differenceY * oldZoom / _camera.Zoom;

        _camera.X = worldX + newDifferenceX;
        _camera.Y = worldY + newDifferenceY;

        //CameraChanged?.Invoke();

        //var asHandleable = e as HandledMouseEventArgs;
        //if (asHandleable != null)
        //{
        //    asHandleable.Handled = true;
        //}
    }

    internal void HandleMouseMove(bool isMiddleMouseDown, Microsoft.Xna.Framework.Point position)
    {
        if (isMiddleMouseDown)
        {
            var newPosition = position;
            int xChange = (int)(newPosition.X - mLastMouseLocation.X);
            int yChange = (int)(newPosition.Y - mLastMouseLocation.Y);

            _camera.Position.X -= xChange / _camera.Zoom;
            _camera.Position.Y -= yChange / _camera.Zoom;

            if (xChange != 0 || yChange != 0)
            {
                PluginManager.Self.CameraChanged();
            }

            mLastMouseLocation = newPosition;
        }
    }
}
