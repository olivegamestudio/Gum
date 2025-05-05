using Gum.Plugins.InternalPlugins.EditorTab.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameGum;
using MonoGameGum.GueDeriving;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolsUtilities;

namespace EditorTabPlugin_FNA.LibraryFiles;

public class FnaGame : Game
{
    private GraphicsDeviceManager graphics;
    private SpriteBatch sb;

    private Point Resolution = new Point(800, 500);
    private bool doResize;
    private int resizeWidth;
    private int resizeHeight;

    public UpdateCallback OnUpdate;
    public DrawCallback OnDraw;

    public delegate void UpdateCallback(GameTime gameTime);
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

    protected override void Update(GameTime gameTime)
    {
        try
        {
            if (OnUpdate != null)
                OnUpdate(gameTime);

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
