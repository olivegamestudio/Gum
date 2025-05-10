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

using RenderingLibrary;
using Gum;

namespace EditorTabPlugin_FNA;
public class EditorGame : FnaGame
{
    public SystemManagers SystemManagers => Gum.SystemManagers;

    GumService Gum;

    bool _isDefault;

    public Microsoft.Xna.Framework.Color ClearColor { get; set; }

    public EditorGame(bool isDefault) : base ()
    {
        _isDefault = isDefault;
    }

    protected override void Initialize()
    {
        var relativeDirectory = FileManager.RelativeDirectory;

        //if(_isDefault)
        //{
        //    GumService.Default.Initialize(this);
        //    Gum = GumService.Default;
        //}
        //else
        //{
            Gum = new GumService();
            var systemManagers = new SystemManagers();
            Gum.Initialize(this, systemManagers);
        //}
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
        GraphicsDevice.Clear(ClearColor);

        Gum.Draw();
    }
}
