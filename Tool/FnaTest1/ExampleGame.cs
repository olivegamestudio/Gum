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

namespace FnaTest1;

public class ExampleGame : Game
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


    GumService Gum => GumService.Default;

    public ExampleGame() : base()
    {
        graphics = new GraphicsDeviceManager(this);
        graphics.PreferredBackBufferWidth = Resolution.X;
        graphics.PreferredBackBufferHeight = Resolution.Y;
    }

    protected override void Initialize()
    {
        var relativeDirectory = FileManager.RelativeDirectory;
        Gum.Initialize(this);
        FileManager.RelativeDirectory = relativeDirectory;

        var rectangle = new ColoredRectangleRuntime();
        rectangle.AddToRoot();


        base.Initialize();
    }

    public void Resize(int width, int height)
    {
        doResize = true;
        resizeWidth = width;
        resizeHeight = height;
    }

    protected override void Update(GameTime gameTime)
    {
        Gum.Update(gameTime);

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



    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        Gum.Draw();

        //sb.Begin();

        //if (OnDraw != null)
        //    OnDraw(sb);


        //sb.End();
    }



}
