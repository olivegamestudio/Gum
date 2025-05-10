using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EditorTabPlugin_FNA.LibraryFiles;

public class FnaGame : Game
{
    private GraphicsDeviceManager graphics;
    private SpriteBatch sb;

    public MouseState ForcedMouseState { get; set; }
    public KeyboardState ForcedKeyboardState { get; set; }

    private Point Resolution = new Point(800, 500);
    private bool doResize;
    private int resizeWidth;
    private int resizeHeight;

    public event Action Initialized;
    public event Action<GameTime> Updated;

    public DrawCallback DrawCalled;

    public delegate void DrawCallback(SpriteBatch spriteBatch);


    public FnaGame() : base()
    {
        graphics = new GraphicsDeviceManager(this);
        graphics.PreferredBackBufferWidth = Resolution.X;
        graphics.PreferredBackBufferHeight = Resolution.Y;


    }


    public void Resize(int width, int height)
    {
        doResize = true;
        resizeWidth = width;
        resizeHeight = height;
    }

    protected override void Initialize()
    {
        Initialized?.Invoke();
        base.Initialize();

    }

    protected override void Update(GameTime gameTime)
    {
        try
        {
            Updated?.Invoke(gameTime);

            if (doResize && GraphicsDevice.GraphicsDeviceStatus == GraphicsDeviceStatus.Normal)
            {
                doResize = false;
                Resolution.X = resizeWidth;
                Resolution.Y = resizeHeight;
                graphics.PreferredBackBufferHeight = resizeHeight;
                graphics.PreferredBackBufferWidth = resizeWidth;
                graphics.ApplyChanges();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        base.Update(gameTime);
    }



}
