using EditorTabPlugin_FNA.LibraryFiles;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using MonoGameGum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolsUtilities;
using Color = Microsoft.Xna.Framework.Color;
using Gum.Plugins.InternalPlugins.EditorTab.Services;

namespace EditorTabPlugin_FNA;
internal class EditorGame : FnaGame
{
    private readonly CameraController _cameraController;

    GumService Gum => GumService.Default;

    public EditorGame(CameraController cameraController) : base ()
    {
        _cameraController = cameraController;
    }

    protected override void Initialize()
    {
        var relativeDirectory = FileManager.RelativeDirectory;
        Gum.Initialize(this);
        FileManager.RelativeDirectory = relativeDirectory;

        base.Initialize();
    }

    protected override void Update(GameTime gameTime)
    {
        Gum.Update(gameTime);

        var keyboard = Gum.Keyboard;
        var cursor = Gum.Cursor;

        //_cameraController.Update();


        // todo - pull in DragDropManager.
        // Look at DragDropManager.Activity "todo - move this"

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
