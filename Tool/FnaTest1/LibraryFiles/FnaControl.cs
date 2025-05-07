//© 2021-2025 Bit Kid, Inc.
//Use at your own risk. No warranty expressed or implied!

using EditorTabPlugin_FNA.LibraryFiles;
using Gum.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SDL3;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using static WPFNA.Win32Api;

namespace WPFNA;

/// <summary>
/// Useful limits of interop
/// https://docs.microsoft.com/en-us/dotnet/desktop/wpf/advanced/technology-regions-overview?view=netframeworkdesktop-4.8
/// and more good info
/// https://docs.microsoft.com/en-us/dotnet/desktop/wpf/advanced/wpf-and-win32-interoperation?view=netframeworkdesktop-4.8#hosting-a-microsoft-win32-window-in-wpf
/// </summary>
public class FnaControl : HwndHost
{
    public FnaGame Game { get; private set; }

    private IntPtr gameHandle = IntPtr.Zero;
    private readonly CancellationTokenSource gameShutingDown = new CancellationTokenSource();

    #region Input Events
    public event Action<bool, Microsoft.Xna.Framework.Point> MouseMoveFNA;
    public event Action<MouseButton> MouseUpFNA;
    public event Action<MouseButton, Microsoft.Xna.Framework.Point> MouseDownFNA;
    public event Action<int, Microsoft.Xna.Framework.Point> MouseWheelFNA;
    public event Action MouseEnterFNA;
    public event Action MouseLeaveFNA;
    public event Action<object, Microsoft.Xna.Framework.Point> DropFNA;
    #endregion

    #region Mouse Vars
    public Microsoft.Xna.Framework.Point MousePosition { get; private set; }
    public System.Windows.Point MousePositionScreen { get; private set; }
    ButtonState LeftMouseButton;
    ButtonState RightMouseButton;
    ButtonState MiddleMouseButton;

    HashSet<Microsoft.Xna.Framework.Input.Keys> Keys =
        new HashSet<Keys>();

    Keys[] KeysArray = new Keys[0];

    public bool IsMouseInside { get; private set; }
    public bool WasMouseInside {  get; private set; }
    private Rect windowRect;
    private DpiScale dpiScale;
    bool wasDragging;
    #endregion

    public FnaControl(FnaGame fnaGame)
    {
        //force graphics driver: "SDL_GPU" (default, uses D3D12), "D3D11" or "Vulkan"
        //Environment.SetEnvironmentVariable("FNA3D_FORCE_DRIVER", "D3D11");

        //init SDL
        bool ret = (bool)SDL.SDL_Init(SDL.SDL_InitFlags.SDL_INIT_VIDEO);
        Debug.WriteLine($"SDL_Init {ret}");

        KeyDown += (s, e) =>
        {
            var wpfKey = e.Key;

            var xnaKey = (Keys)System.Windows.Input.KeyInterop.VirtualKeyFromKey(wpfKey);

            var winformsKey = (System.Windows.Forms.Keys)xnaKey;
            if (Keys.Contains(Microsoft.Xna.Framework.Input.Keys.LeftShift) || Keys.Contains(Microsoft.Xna.Framework.Input.Keys.RightShift))
            {
                winformsKey = winformsKey | System.Windows.Forms.Keys.Shift;
            }
            if(Keys.Contains(Microsoft.Xna.Framework.Input.Keys.LeftAlt) || Keys.Contains(Microsoft.Xna.Framework.Input.Keys.RightAlt))
            {
                winformsKey = winformsKey | System.Windows.Forms.Keys.Alt;
            }
            if(Keys.Contains(Microsoft.Xna.Framework.Input.Keys.LeftControl) || Keys.Contains(Microsoft.Xna.Framework.Input.Keys.RightControl))
            {
                winformsKey = winformsKey | System.Windows.Forms.Keys.Control;
            }

            var msg = new System.Windows.Forms.Message();
            bool handled = HotkeyManager.Self.ProcessCmdKeyWireframe(ref msg, winformsKey);

            if (Keys.Add(xnaKey))
            {
                KeysArray = Keys.ToArray();
                ShowKeys();
            }

            // handle the arrow keys otherwise tab is shifted around
            if (xnaKey == Microsoft.Xna.Framework.Input.Keys.Left || xnaKey == Microsoft.Xna.Framework.Input.Keys.Right ||
                xnaKey == Microsoft.Xna.Framework.Input.Keys.Up || xnaKey == Microsoft.Xna.Framework.Input.Keys.Down)
            {
                handled = true;
            }

            e.Handled = handled;

        };
        KeyUp += (s, e) =>
        {
            var wpfKey = e.Key;
            var xnaKey = (Keys)System.Windows.Input.KeyInterop.VirtualKeyFromKey(wpfKey);
            Keys.Remove(xnaKey);
            KeysArray = Keys.ToArray();
            ShowKeys();
            e.Handled = true;

        };

        void ShowKeys()
        {
            KeysArray = Keys.ToArray();
            Debug.WriteLine($"Key: {string.Join(",", KeysArray)}");
        }

            //create game and initialize
            Game = fnaGame;
        Game.RunOneFrame();

        //spin up game update thread
        //var gameThread = new Thread(GameExecution);
        //gameThread.SetApartmentState(ApartmentState.STA);
        //gameThread.Start();
        // Update 
        // If we do
        // it in its
        // own therad, 
        // then accessing
        // any UI properties
        // requires some way of
        // communicating across the
        // threads or else we get a crash.
        // Instead, I just run an async loop
        GameExecution();

    }

    protected override HandleRef BuildWindowCore(HandleRef hwndParent)
    {
        Debug.WriteLine($"hwndParent {hwndParent.Handle}");

        //fna window handle
        gameHandle = SDL.SDL_GetPointerProperty(SDL.SDL_GetWindowProperties(Game.Window.Handle), SDL.SDL_PROP_WINDOW_WIN32_HWND_POINTER, new IntPtr());
        Debug.WriteLine($"SDL window handle {gameHandle:X4}");

        Game.IsMouseVisible = true;
        Game.Window.IsBorderlessEXT = true;
        Game.Window.AllowUserResizing = true;

        //make the window a child window instead of top-level application window and give it focus
        var oldStyle = (Win32Api.WindowStyles)Win32Api.SetWindowLong(gameHandle, Win32Api.GWL_STYLE, 
            (uint)(Win32Api.WindowStyles.WS_CHILD | Win32Api.WindowStyles.WS_TABSTOP));
        Win32Api.SetParent(gameHandle, hwndParent.Handle);
        Win32Api.SetFocus(gameHandle);

        // this allows us to read mouse events outside of the control, but it
        // seems to make the mouse move super slow when Gum starts initially
        //hook_mouse();

        return new HandleRef(this, gameHandle);
    }

    protected override void DestroyWindowCore(HandleRef hwnd)
    {
        //unhook_mouse();
        gameShutingDown.Cancel();
        Game?.Exit();
        Game = null;
    }

    // Run the Game in its own thread, to prevent GL ugliness
    static System.Diagnostics.Stopwatch stopWatch;
    double lastPrint;
    private async void GameExecution()
    {
        stopWatch = new System.Diagnostics.Stopwatch();
        stopWatch.Start();

        var desiredFps = 60;
        var desiredMs = 1000f / desiredFps;

        Debug.WriteLine("Game Loop started");
        var shutdownToken = gameShutingDown.Token;

        while (Game != null && !shutdownToken.IsCancellationRequested)
        {
            var start = stopWatch.Elapsed.TotalSeconds;

            Game.ForcedMouseState = new Microsoft.Xna.Framework.Input.MouseState(
                (int)MousePosition.X, (int)MousePosition.Y, 0,
                LeftMouseButton,
                MiddleMouseButton,
                RightMouseButton,
                ButtonState.Released,
                ButtonState.Released
                );
            var keyboardState = new Microsoft.Xna.Framework.Input.KeyboardState(KeysArray);
            Game.ForcedKeyboardState = keyboardState;

            Game?.RunOneFrame();
            UpdateMouseLocation();
            UpdateDragDrop();
            var afterRunOneFrame = stopWatch.Elapsed.TotalSeconds;

            var msToSleep = desiredMs - (afterRunOneFrame - start);
            // testing
            await Task.Delay((int)System.Math.Max(1,msToSleep));

            var afterSleep = stopWatch.Elapsed.TotalSeconds;

            if(stopWatch.Elapsed.TotalSeconds > lastPrint + 1)
            {
                lastPrint = stopWatch.Elapsed.TotalSeconds;
                System.Diagnostics.Debug.WriteLine($"Game Logic: {((afterRunOneFrame - start) * 1000):0.00}, " +
                    $"Slept for: {msToSleep:0.00}, Total loop: {((afterSleep - start)*1000):0.00})");
            }
        }

        Debug.WriteLine("Game stopped");
    }

    private void UpdateMouseLocation()
    {
        Win32Api.POINT mousePoint;
        var got = Win32Api.GetCursorPos(out mousePoint);
        MousePositionScreen = new System.Windows.Point(mousePoint.X, mousePoint.Y);

        //translate to control area and apply dpi
        MousePosition = new Microsoft.Xna.Framework.Point((int)MousePositionScreen.X - (int)windowRect.X, (int)MousePositionScreen.Y - (int)windowRect.Y);
        MousePosition = new Microsoft.Xna.Framework.Point((int)(MousePosition.X / dpiScale.DpiScaleX), (int)(MousePosition.Y / dpiScale.DpiScaleY));

        WasMouseInside = IsMouseInside;
        IsMouseInside = windowRect.Contains(MousePositionScreen);

        if (!WasMouseInside && IsMouseInside)
            MouseEnterFNA?.Invoke();
        else if (WasMouseInside && !IsMouseInside)
            MouseLeaveFNA?.Invoke();
    }

    private void UpdateWindowLocation()
    {
        var source = PresentationSource.FromVisual(this);
        var transformToDevice = source.CompositionTarget.TransformToDevice;
        var pixelSize = (Size)transformToDevice.Transform((Vector)RenderSize);
        var windowOrigin = this.PointToScreen(new System.Windows.Point(0,0));
        windowRect = new Rect(windowOrigin, pixelSize);
        dpiScale = VisualTreeHelper.GetDpi(this);
    }

    private void UpdateDragDrop()
    {
        //bool dragging = GongSolutions.Wpf.DragDrop.DragDrop.IsDragging;

        //if (!dragging && wasDragging && IsMouseInside)
        //{
        //    if (DropFNA != null && GongSolutions.Wpf.DragDrop.DragDrop.DragDopObject != null)
        //    {
        //        Dispatcher.Invoke(new Action(() =>
        //        {
        //            object o = GongSolutions.Wpf.DragDrop.DragDrop.DragDopObject.GetData("GongSolutions.Wpf.DragDrop");
        //            DropFNA.Invoke(o, MousePosition);
        //        }));
        //    }
        //}

        //wasDragging = dragging;
    }

    protected override void OnWindowPositionChanged(Rect rcBoundingBox)
    {
        base.OnWindowPositionChanged(rcBoundingBox);
        UpdateWindowLocation();
    }

    protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
    {
        base.OnRenderSizeChanged(sizeInfo);

        if (Game != null)
        {
            int width = (int)Math.Round(sizeInfo.NewSize.Width, MidpointRounding.AwayFromZero);
            int height = (int)Math.Round(sizeInfo.NewSize.Height, MidpointRounding.AwayFromZero);
            Game.Resize(width, height);
        }

        UpdateWindowLocation();
    }

    private HookProc delMouseProc = null;
    private IntPtr hooked;

    private void hook_mouse()
    {
        // initialize our delegate
        this.delMouseProc = new HookProc(this.MouseProc);
        hooked = SetWindowsHookEx(HookType.WH_MOUSE_LL, this.delMouseProc, IntPtr.Zero, 0);
    }

    private void unhook_mouse()
    {
        UnhookWindowsHookEx(hooked);
        hooked = IntPtr.Zero;
    }

    /// <summary>
    /// Watch for left mouse button up and determine if the mouse is within our window.
    /// This is a chain of mouse watches that may not be broken.
    /// This must process very fast
    /// </summary>
    /// <remarks>
    /// WARNING
    ///   This runs for all mouse action regularless of the executing application or
    ///   mouse location. It run in Visual Studio when the XAML is edited.
    /// </remarks>
    /// <param name="code"></param>
    /// <param name="wParam"></param>
    /// <param name="lParam"></param>
    /// <returns></returns>
    private int MouseProc(int code, IntPtr wParam, IntPtr lParam)
    {

        if (code < 0)
        {
            //you need to call CallNextHookEx without further processing
            //and return the value returned by CallNextHookEx
            return CallNextHookEx(IntPtr.Zero, code, wParam, lParam);
        }
        WM wm = (WM)wParam.ToInt32();
        if (wm == WM.LBUTTONUP)
        {
            LeftMouseButton = ButtonState.Released;
            MSLLHOOKSTRUCT msll = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
            MousePositionScreen = new System.Windows.Point(msll.pt.X, msll.pt.Y);

            //this is WPF screen scale, not XNA screen scale
            MousePosition = new Microsoft.Xna.Framework.Point((int)MousePositionScreen.X - (int)windowRect.X,
                (int)MousePositionScreen.Y - (int)windowRect.Y);
            var contains = windowRect.Contains(MousePositionScreen);
            MousePosition = new Microsoft.Xna.Framework.Point((int)(MousePosition.X / dpiScale.DpiScaleX), (int)(MousePosition.Y / dpiScale.DpiScaleY));

            if (contains)
                MouseUpFNA?.Invoke(MouseButton.Left);
        }
        else if(wm == WM.MBUTTONUP)
        {
            MiddleMouseButton = ButtonState.Released;
            MSLLHOOKSTRUCT msll = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
            MousePositionScreen = new System.Windows.Point(msll.pt.X, msll.pt.Y);

            //this is WPF screen scale, not XNA screen scale
            MousePosition = new Microsoft.Xna.Framework.Point((int)MousePositionScreen.X - (int)windowRect.X,
                (int)MousePositionScreen.Y - (int)windowRect.Y);
            var contains = windowRect.Contains(MousePositionScreen);
            MousePosition = new Microsoft.Xna.Framework.Point((int)(MousePosition.X / dpiScale.DpiScaleX), (int)(MousePosition.Y / dpiScale.DpiScaleY));

            if (contains || isMiddleMouseDown)
            {
                isMiddleMouseDown = false;
                MouseUpFNA?.Invoke(MouseButton.Middle);

            }
        }

        //return the value returned by CallNextHookEx
        return CallNextHookEx(IntPtr.Zero, code, wParam, lParam);
    }

    Microsoft.Xna.Framework.Point lastMousePosition = new Microsoft.Xna.Framework.Point(0, 0);
    bool isMiddleMouseDown = false;
    //Handle Mouse and Keyboard Events
    //Note: this seems to swallow key events eventually, not sure why but
    //will cause problems if you want to use Keyboard.GetState in Game class
    protected override IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, 
        IntPtr lParam, ref bool handled)
    {
        var windowsMessage = (WM)msg;
        switch (windowsMessage)
        {
            case WM.MOUSEMOVE://mouse move
                handled = true;
                int x = unchecked((short)(long)lParam);
                int y = unchecked((short)((long)lParam >> 16));
                lastMousePosition = new Microsoft.Xna.Framework.Point(x, y);

                if (MouseMoveFNA != null)
                    MouseMoveFNA(isMiddleMouseDown, lastMousePosition);
                break;
            case WM.LBUTTONDOWN: //left mouse down
                LeftMouseButton = ButtonState.Pressed;
                this.Focus();
                handled = true;

                if (MouseDownFNA != null)
                    MouseDownFNA(MouseButton.Left, lastMousePosition);
                break;
            case WM.LBUTTONUP: //left mouse up
                LeftMouseButton = ButtonState.Released;
                handled = true;

                if (MouseUpFNA != null)
                    MouseUpFNA(MouseButton.Left);
                break;
            //case 0x203: //left mouse double click
            //    handled = true;
            //    // this.Focus();
            //    break;
            case WM.RBUTTONDOWN: //right mouse down
                RightMouseButton = ButtonState.Pressed;
                handled = true;

                if (MouseDownFNA != null)
                    MouseDownFNA(MouseButton.Right, lastMousePosition);
                break;
            case WM.RBUTTONUP: //right mouse up
                RightMouseButton = ButtonState.Released;
                handled = true;

                if (MouseUpFNA != null)
                    MouseUpFNA(MouseButton.Right);
                break;
            //case 0x0206: //right mouse double click
            //    handled = true;
            //    break;
            case WM.MBUTTONDOWN: //middle mouse down
                MiddleMouseButton = ButtonState.Pressed;
                handled = true;
                isMiddleMouseDown = true;
                if (MouseDownFNA != null)
                    MouseDownFNA(MouseButton.Middle, lastMousePosition);
                break;
            case WM.MBUTTONUP: //middle mouse up
                MiddleMouseButton = ButtonState.Released;
                handled = true;
                isMiddleMouseDown = false;

                if (MouseUpFNA != null)
                    MouseUpFNA(MouseButton.Middle);
                break;
            case WM.MOUSEWHEEL:
                handled = true;

                short delta = (short)((wParam.ToInt64() >> 16) & 0xFFFF);

                System.Diagnostics.Debug.WriteLine($"Mouse wheel: {lParam} delta: {delta}");

                MouseWheelFNA?.Invoke(delta, lastMousePosition);


                break;
        }

        //if(!handled)
        //{
        //    var asVm = (WM)msg;

        //    System.Diagnostics.Debug.WriteLine($"Not processed: {asVm} " + msg.ToString("X"));
        //}

        return base.WndProc(hwnd, msg, wParam, lParam, ref handled);
    }
}