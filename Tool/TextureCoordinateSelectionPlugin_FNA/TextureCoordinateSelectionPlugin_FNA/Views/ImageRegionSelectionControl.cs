using FlatRedBall.SpecializedXnaControls.Input;
using FlatRedBall.SpecializedXnaControls.RegionSelection;
using Gum.Wireframe;
using RenderingLibrary;
using RenderingLibrary.Content;
using RenderingLibrary.Graphics;
using RenderingLibrary.Math;
using SkiaGum.Renderables;
using SkiaGum.Wpf;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ToolsUtilities;

namespace TextureCoordinateSelectionPlugin.Views;
public class ImageRegionSelectionControl : GumSKElement
{
    #region Fields

    //ImageData maxAlphaImageData;

    SKBitmap mCurrentTexture;
    SKBitmap maxAlphaTexture;

    bool mRoundRectangleSelectorToUnit = true;
    List<RectangleSelector> mRectangleSelectors = new List<RectangleSelector>();

    CameraPanningLogic mCameraPanningLogic;

    //Cursor mCursor;
    //Keyboard mKeyboard;

    TimeManager mTimeManager;

    Sprite mCurrentTextureSprite;


    IList<int> mAvailableZoomLevels;

    bool showFullAlpha;

    #endregion


    public bool RoundRectangleSelectorToUnit
    {
        get { return mRoundRectangleSelectorToUnit; }
        set
        {
            mRoundRectangleSelectorToUnit = value;

            foreach (var item in mRectangleSelectors)
            {
                item.RoundToUnitCoordinates = mRoundRectangleSelectorToUnit;
            }
        }
    }

    int? snappingGridSize;
    public int? SnappingGridSize
    {
        get
        {
            return snappingGridSize;
        }
        set
        {
            snappingGridSize = value;
            foreach (var item in mRectangleSelectors)
            {
                item.SnappingGridSize = snappingGridSize;
            }
        }
    }

    Camera Camera
    {
        get
        {
            return SystemManagers.Renderer.Camera;
        }
    }

    public SystemManagers SystemManagers { get; set; }

    public RectangleSelector RectangleSelector
    {
        get
        {
            if (mRectangleSelectors.Count != 0)
            {
                return mRectangleSelectors[0];
            }
            else
            {
                return null;
            }
        }
    }

    public List<RectangleSelector> RectangleSelectors
    {
        get
        {
            return mRectangleSelectors;
        }
    }

    public SKBitmap CurrentTexture
    {
        get { return mCurrentTexture; }
        set
        {
            bool didChange = mCurrentTexture != value;

            if (didChange)
            {
                mCurrentTexture = value;
                if (SystemManagers != null)
                {
                    bool hasCreateVisuals = mCurrentTextureSprite != null;

                    if (!hasCreateVisuals)
                    {
                        CreateVisuals();
                    }
                    if (mCurrentTexture == null)
                    {
                        mCurrentTextureSprite.Visible = false;
                    }
                    else
                    {
                        mCurrentTextureSprite.Visible = true;
                        if (showFullAlpha)
                        {
                            mCurrentTextureSprite.Texture = maxAlphaTexture;
                        }
                        else
                        {
                            mCurrentTextureSprite.Texture = mCurrentTexture;
                        }
                        mCurrentTextureSprite.Width = mCurrentTexture.Width;
                        mCurrentTextureSprite.Height = mCurrentTexture.Height;

                    }
                }
            }
        }
    }


    private void CreateVisuals()
    {
        mCurrentTextureSprite = new Sprite();
        mCurrentTextureSprite.Texture = mCurrentTexture;
        mCurrentTextureSprite.Name = "Image Region Selection Main Sprite";
        //SystemManagers.SpriteManager.Add(mCurrentTextureSprite);
    }

    //public Cursor XnaCursor
    //{
    //    get { return mCursor; }
    //}

    public bool SelectorVisible
    {
        get
        {
            return mRectangleSelectors.Count != 0 && mRectangleSelectors[0].Visible;
        }
        set
        {
            // This causes problems in VS designer mode.
            if (mRectangleSelectors != null)
            {
                foreach (var selector in mRectangleSelectors)
                {
                    selector.Visible = value;
                }
            }
        }
    }

    public int ZoomValue
    {
        get
        {
            return MathFunctions.RoundToInt(SystemManagers.Renderer.Camera.Zoom * 100);
        }
        set
        {
            if (SystemManagers?.Renderer != null)
            {
                SystemManagers.Renderer.Camera.Zoom = value / 100.0f;
            }
        }
    }

    /// <summary>
    /// Sets the available zoom levels, where 100 is 100. These values must be set for zooming to be enabled.
    /// </summary>
    public IList<int> AvailableZoomLevels
    {
        set
        {
            mAvailableZoomLevels = value;
        }
    }

    public int ZoomIndex
    {
        get
        {
            if (mAvailableZoomLevels != null)
            {
                return mAvailableZoomLevels.IndexOf(ZoomValue);
            }
            return -1;
        }
    }

    /// <summary>
    /// Creates and destroys the internal rectangle selectors to match the desired count.
    /// </summary>
    public int DesiredSelectorCount
    {
        set
        {
            while (value > this.mRectangleSelectors.Count)
            {
                CreateNewSelector();
            }

            while (value < this.mRectangleSelectors.Count)
            {
                var selector = mRectangleSelectors.Last();

                selector.RemoveFromManagers();
                mRectangleSelectors.RemoveAt(mRectangleSelectors.Count - 1);
            }
        }
    }

    public bool ShowFullAlpha
    {
        get
        {
            return showFullAlpha;
        }
        set
        {
            showFullAlpha = value;

            if (mCurrentTextureSprite != null)
            {
                if (showFullAlpha)
                {
                    mCurrentTextureSprite.Texture = maxAlphaTexture;
                }
                else
                {
                    mCurrentTextureSprite.Texture = mCurrentTexture;
                }
            }
        }
    }


    #region Events

    public event EventHandler StartRegionChanged;
    public new event EventHandler RegionChanged;
    public event EventHandler EndRegionChanged;

    public event EventHandler MouseWheelZoom;
    public event Action Panning;
    #endregion

    public ImageRegionSelectionControl()
    {
    }

    #region Methods


    

    public void CreateDefaultZoomLevels()
    {

    }

    public void CustomInitialize()
    {
        if (!DesignerProperties.GetIsInDesignMode(this))
        {
            mTimeManager = new TimeManager();


            //SystemManagers.Name = "Image Region Selection";
            //Assembly assembly = Assembly.GetAssembly(typeof(GraphicsDeviceControl));// Assembly.GetCallingAssembly();

            //string targetFntFileName = FileManager.UserApplicationDataForThisApplication + "Font18Arial.fnt";
            //string targetPngFileName = FileManager.UserApplicationDataForThisApplication + "Font18Arial_0.png";
            //FileManager.SaveEmbeddedResource(
            //    assembly,
            //    "XnaAndWinforms.Content.Font18Arial.fnt",
            //    targetFntFileName);

            //FileManager.SaveEmbeddedResource(
            //    assembly,
            //    "XnaAndWinforms.Content.Font18Arial_0.png",
            //    targetPngFileName);



            //var contentLoader = new ContentLoader();
            //contentLoader.SystemManagers = SystemManagers;

            //LoaderManager.Self.ContentLoader = contentLoader;
            //LoaderManager.Self.Initialize("Content/InvalidTexture.png", targetFntFileName, Services, mManagers);

            CreateNewSelector();

            mCameraPanningLogic = new CameraPanningLogic(SystemManagers);
            mCameraPanningLogic.Panning += HandlePanning;



            //MouseWheelFNA += HandleMouseWheel;
            //ZoomNumbers = new Zooming.ZoomNumbers();
        }
    }

    private RectangleSelector CreateNewSelector()
    {
        var newSelector = new RectangleSelector(SystemManagers);
        newSelector.AddToManagers(SystemManagers);
        newSelector.Visible = false;
        newSelector.StartRegionChanged += HandleStartRegionChanged;
        newSelector.RegionChanged += new EventHandler(RegionChangedInternal);
        newSelector.EndRegionChanged += EndRegionChangedInternal;
        newSelector.SnappingGridSize = snappingGridSize;
        newSelector.RoundToUnitCoordinates = mRoundRectangleSelectorToUnit;

        mRectangleSelectors.Add(newSelector);

        return newSelector;
    }

    private void HandlePanning()
    {
        if (Panning != null)
        {
            Panning();
        }
    }

    private void HandleStartRegionChanged(object sender, EventArgs e)
    {
        StartRegionChanged?.Invoke(this, null);
    }

    void RegionChangedInternal(object sender, EventArgs e)
    {
        RegionChanged?.Invoke(this, null);
    }

    void EndRegionChangedInternal(object sender, EventArgs e)
    {
        EndRegionChanged?.Invoke(this, null);
    }

    void PerformActivity()
    {
        mTimeManager.Activity();

        
        foreach (var item in mRectangleSelectors)
        {
            //item.Activity(mCursor, mKeyboard);
        }
    }

    //void HandleMouseWheel(int delta, Microsoft.Xna.Framework.Point position)
    //{
    //    if (mAvailableZoomLevels != null)
    //    {
    //        int index = ZoomIndex;
    //        if (index != -1)
    //        {
    //            float value = delta;

    //            float worldX = mCursor.GetWorldX(SystemManagers);
    //            float worldY = mCursor.GetWorldY(SystemManagers);

    //            float oldCameraX = Camera.X;
    //            float oldCameraY = Camera.Y;

    //            float oldZoom = ZoomValue / 100.0f;

    //            bool didZoom = false;

    //            if (value < 0 && index < mAvailableZoomLevels.Count - 1)
    //            {
    //                ZoomValue = mAvailableZoomLevels[index + 1];

    //                didZoom = true;
    //            }
    //            else if (value > 0 && index > 0)
    //            {
    //                ZoomValue = mAvailableZoomLevels[index - 1];

    //                didZoom = true;
    //            }


    //            if (didZoom)
    //            {
    //                AdjustCameraPositionAfterZoom(worldX, worldY,
    //                    oldCameraX, oldCameraY, oldZoom, ZoomValue, Camera);

    //                if (MouseWheelZoom != null)
    //                {
    //                    MouseWheelZoom(this, null);
    //                }
    //            }
    //        }
    //    }
    //}

    public static void AdjustCameraPositionAfterZoom(float oldCursorWorldX, float oldCursorWorldY,
        float oldCameraX, float oldCameraY, float oldZoom, float newZoom, Camera camera)
    {
        float differenceX = oldCameraX - oldCursorWorldX;
        float differenceY = oldCameraY - oldCursorWorldY;

        float zoomAsFloat = newZoom / 100.0f;

        float modifiedDifferenceX = differenceX * oldZoom / zoomAsFloat;
        float modifiedDifferenceY = differenceY * oldZoom / zoomAsFloat;

        camera.X = oldCursorWorldX + modifiedDifferenceX;
        camera.Y = oldCursorWorldY + modifiedDifferenceY;

        // This makes the zooming behavior feel weird.  We'll do this when the user selects a new 
        // AnimationChain, but not when zooming.
        //BringSpriteInView();
    }

    public void BringSpriteInView()
    {
        if (mCurrentTexture != null)
        {
            const float pixelBorder = 10;

            bool isAbove = mCurrentTextureSprite.Y + mCurrentTexture.Height < Camera.AbsoluteTop;
            bool isBelow = mCurrentTextureSprite.Y > Camera.AbsoluteBottom;

            bool isLeft = mCurrentTextureSprite.X + mCurrentTexture.Width < Camera.AbsoluteLeft;
            bool isRight = mCurrentTextureSprite.X > Camera.AbsoluteRight;

            // If it's both above and below, that means the user has zoomed in a lot so that the Sprite is bigger than
            // the camera view.  
            // If it's neither, then the entire Sprite is in view.
            // If it's only one or the other, that means that part of the Sprite is hanging off the edge, and we can adjust.
            bool adjustY = (isAbove || isBelow) && !(isAbove && isBelow);
            bool adjustX = (isLeft || isRight) && !(isLeft && isRight);

            if (adjustY)
            {
                bool isTallerThanCamera = mCurrentTexture.Height * Camera.Zoom > Camera.ClientHeight;

                if ((isTallerThanCamera && isAbove) || (!isTallerThanCamera && isBelow))
                {
                    // Move Camera so Sprite is on bottom
                    Camera.Y = mCurrentTextureSprite.Y + mCurrentTexture.Height - (Camera.ClientHeight / 2.0f - pixelBorder) / Camera.Zoom;
                }
                else
                {
                    // Move Camera so Sprite is on top
                    Camera.Y = mCurrentTextureSprite.Y + (Camera.ClientHeight / 2.0f - pixelBorder) / Camera.Zoom;
                }
            }

            if (adjustX)
            {
                bool isWiderThanCamera = mCurrentTexture.Width * Camera.Zoom > Camera.ClientWidth;

                if ((isWiderThanCamera && isLeft) || (!isWiderThanCamera && isRight))
                {
                    Camera.X = mCurrentTextureSprite.X + mCurrentTexture.Width - (Camera.ClientWidth / 2.0f - pixelBorder) / Camera.Zoom;
                }
                else
                {
                    Camera.X = mCurrentTextureSprite.X + (Camera.ClientWidth / 2.0f - pixelBorder) / Camera.Zoom;
                }
            }
        }
    }

    #endregion

}
