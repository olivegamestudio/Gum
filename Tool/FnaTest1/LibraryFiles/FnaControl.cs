//© 2021-2025 Bit Kid, Inc.
//Use at your own risk. No warranty expressed or implied!

using FnaTest1;
using SDL3;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace WPFNA
{
    /// <summary>
    /// Useful limits of interop
    /// https://docs.microsoft.com/en-us/dotnet/desktop/wpf/advanced/technology-regions-overview?view=netframeworkdesktop-4.8
    /// and more good info
    /// https://docs.microsoft.com/en-us/dotnet/desktop/wpf/advanced/wpf-and-win32-interoperation?view=netframeworkdesktop-4.8#hosting-a-microsoft-win32-window-in-wpf
    /// </summary>
    public class FnaControl : HwndHost
    {
        public ExampleGame Game { get; private set; }

        private IntPtr gameHandle = IntPtr.Zero;
        private readonly CancellationTokenSource gameShutingDown = new CancellationTokenSource();

        #region Input Events
        public Action<Microsoft.Xna.Framework.Point> MouseMoveFNA;
        public Action MouseUpFNA;
        public Action<MouseButton> MouseDownFNA;
        public Action MouseEnterFNA;
        public Action MouseLeaveFNA;
        public Action<object, Microsoft.Xna.Framework.Point> DropFNA;
        #endregion

        #region Mouse Vars
        public Microsoft.Xna.Framework.Point MousePosition { get; private set; }
        public Point MousePositionScreen { get; private set; }
        public bool IsMouseInside { get; private set; }
        public bool WasMouseInside {  get; private set; }
        private Rect windowRect;
        private DpiScale dpiScale;
        bool wasDragging;
        #endregion

        public FnaControl()
        {
            //force graphics driver: "SDL_GPU" (default, uses D3D12), "D3D11" or "Vulkan"
            //Environment.SetEnvironmentVariable("FNA3D_FORCE_DRIVER", "D3D11");

            //init SDL
            bool ret = (bool)SDL.SDL_Init(SDL.SDL_InitFlags.SDL_INIT_VIDEO);
            Debug.WriteLine($"SDL_Init {ret}");

            //create game and initialize
            Game = new ExampleGame();
            Game.RunOneFrame();

            //spin up game update thread
            var gameThread = new Thread(GameExecution);
            gameThread.SetApartmentState(ApartmentState.STA);
            gameThread.Start();
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

            return new HandleRef(this, gameHandle);
        }

        protected override void DestroyWindowCore(HandleRef hwnd)
        {
            gameShutingDown.Cancel();
            Game.OnUpdate = null;
            Game.OnDraw = null;
            Game?.Exit();
            Game = null;
        }

        // Run the Game in its own thread, to prevent GL ugliness
        private void GameExecution()
        {
            Debug.WriteLine("Game Loop started");
            var shutdownToken = gameShutingDown.Token;

            while (Game != null && !shutdownToken.IsCancellationRequested)
            {
                Game?.RunOneFrame();
                UpdateMouseLocation();
                UpdateDragDrop();
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
            var windowOrigin = this.PointToScreen(new Point(0,0));
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

        //Handle Mouse and Keyboard Events
        //Note: this seems to swallow key events eventually, not sure why but
        //will cause problems if you want to use Keyboard.GetState in Game class
        protected override IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case 0x0200://mouse move
                    handled = true;
                    int x = unchecked((short)(long)lParam);
                    int y = unchecked((short)((long)lParam >> 16));
                    var pos = new Microsoft.Xna.Framework.Point(x, y);

                    if (MouseMoveFNA != null)
                        MouseMoveFNA(pos);
                    break;
                case 0x201: //left mouse down
                    this.Focus();
                    handled = true;

                    if (MouseDownFNA != null)
                        MouseDownFNA(MouseButton.Left);
                    break;
                case 0x202: //left mouse up
                    handled = true;

                    if (MouseUpFNA != null)
                        MouseUpFNA();
                    break;
                //case 0x203: //left mouse double click
                //    handled = true;
                //    // this.Focus();
                //    break;
                case 0x0204: //right mouse down
                    handled = true;

                    if (MouseDownFNA != null)
                        MouseDownFNA(MouseButton.Right);
                    break;
                case 0x0205: //right mouse up
                    handled = true;

                    if (MouseUpFNA != null)
                        MouseUpFNA();
                    break;
                //case 0x0206: //right mouse double click
                //    handled = true;
                //    break;
                case 0x0207: //middle mouse down
                    handled = true;

                    if (MouseDownFNA != null)
                        MouseDownFNA(MouseButton.Middle);
                    break;
                case 0x0208: //middle mouse up
                    handled = true;

                    if (MouseUpFNA != null)
                        MouseUpFNA();
                    break;
            }

            return base.WndProc(hwnd, msg, wParam, lParam, ref handled);
        }
    }
}