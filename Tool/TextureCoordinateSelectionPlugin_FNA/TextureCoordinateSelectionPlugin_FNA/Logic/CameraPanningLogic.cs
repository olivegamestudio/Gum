using System;
using RenderingLibrary;

namespace FlatRedBall.SpecializedXnaControls.Input
{
    public class CameraPanningLogic
    {
        Camera mCamera;
        //GraphicsDeviceControl mControl;


        SystemManagers mManagers;

        public event Action Panning;

        public CameraPanningLogic(SystemManagers managers)
        {
            mManagers = managers;
            
            //mKeyboard = keyboard;

            //mCursor = cursor;
            mCamera = managers.Renderer.Camera;
            //mControl = graphicsControl;
            //graphicsControl.XnaUpdate += new Action(Activity);

        }

        public void Activity()
        {

            //if(mKeyboard != null)
            //{
            //    bool isCtrlDown = mKeyboard.KeyDown(Microsoft.Xna.Framework.Input.Keys.LeftControl) ||
            //        mKeyboard.KeyDown(Microsoft.Xna.Framework.Input.Keys.RightControl);

            //    if(isCtrlDown)
            //    {
            //        const int movementCoefficient = 20;
            //        if (mKeyboard.KeyPushed(Microsoft.Xna.Framework.Input.Keys.Up))
            //        {
            //            mCamera.Y -= movementCoefficient / mCamera.Zoom;
            //            if (Panning != null)
            //            {
            //                Panning();
            //            }
            //        }
            //        else if (mKeyboard.KeyPushed(Microsoft.Xna.Framework.Input.Keys.Down))
            //        {
            //            mCamera.Y += movementCoefficient / mCamera.Zoom;
            //            if (Panning != null)
            //            {
            //                Panning();
            //            }
            //        }
            //        else if (mKeyboard.KeyPushed(Microsoft.Xna.Framework.Input.Keys.Left))
            //        {
            //            mCamera.X -= movementCoefficient / mCamera.Zoom;
            //            if (Panning != null)
            //            {
            //                Panning();
            //            }
            //        }
            //        else if (mKeyboard.KeyPushed(Microsoft.Xna.Framework.Input.Keys.Right))
            //        {
            //            mCamera.X += movementCoefficient / mCamera.Zoom;
            //            if (Panning != null)
            //            {
            //                Panning();
            //            }
            //        }
            //        //else if (e.KeyCode == Keys.Oemplus || e.KeyCode == Keys.Add)
            //        //{
            //        //    mWireframeEditControl.ZoomIn();
            //        //}
            //        //else if (e.KeyCode == Keys.OemMinus || e.KeyCode == Keys.Subtract)
            //        //{
            //        //    mWireframeEditControl.ZoomOut();
            //        //}
            //    }
            //}
            
            //if (mCursor.MiddleDown && 
            //    mCursor.IsInWindow &&
            //    (mCursor.XChange != 0 || mCursor.YChange != 0)
            //    )
            //{


            //    mCamera.X -= mCursor.XChange / mManagers.Renderer.Camera.Zoom;
            //    mCamera.Y -= mCursor.YChange / mManagers.Renderer.Camera.Zoom;
            //    if (Panning != null)
            //    {
            //        Panning();
            //    }

            //}

        }


    }
}
