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
using RenderingLibrary;
using Gum;

namespace EditorTabPlugin_FNA;
internal class EditorGame : FnaGame
{
    public SystemManagers SystemManagers => Gum.SystemManagers;

    GumService Gum => GumService.Default;

    public EditorGame() : base ()
    {
        
    }

    protected override void Initialize()
    {
        var relativeDirectory = FileManager.RelativeDirectory;
        Gum.Initialize(this);
        FileManager.RelativeDirectory = relativeDirectory;
        //Gum.SystemManagers.Renderer.Camera.CameraCenterOnScreen = CameraCenterOnScreen.Center;


        RenderingLibrary.Graphics.Renderer.ApplyCameraZoomOnWorldTranslation = false;

        base.Initialize();
    }

    protected override void Update(GameTime gameTime)
    {
        
        Gum.Update(gameTime);

        var keyboard = Gum.Keyboard;
        var cursor = Gum.Cursor;
        cursor.Activity(gameTime.TotalGameTime.TotalSeconds, ForcedMouseState);
        keyboard.Activity(gameTime.TotalGameTime.TotalSeconds, this, ForcedKeyboardState);
        //_cameraController.Update();


        // todo - pull in DragDropManager.
        // Look at DragDropManager.Activity "todo - move this"

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        var clearColor = new Microsoft.Xna.Framework.Color(
        ProjectManager.Self.GeneralSettingsFile.CheckerColor1R,
                ProjectManager.Self.GeneralSettingsFile.CheckerColor1G,
                ProjectManager.Self.GeneralSettingsFile.CheckerColor1B);

        GraphicsDevice.Clear(clearColor);

        Gum.Draw();

        //sb.Begin();

        //if (OnDraw != null)
        //    OnDraw(sb);


        //sb.End();
    }
}
