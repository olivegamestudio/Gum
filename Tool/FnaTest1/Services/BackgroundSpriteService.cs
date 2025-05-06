using Gum;
using Microsoft.Xna.Framework.Graphics;
using RenderingLibrary;
using RenderingLibrary.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EditorTabPlugin_XNA.Services;
internal class BackgroundSpriteService
{
    public Sprite BackgroundSprite { get; private set; }

    public BackgroundSpriteService()
    {

    }

    public void Initialize(SystemManagers systemManagers)
    {
        // Create the Texture2D here
        ImageData imageData = new ImageData(2, 2, null);

        Microsoft.Xna.Framework.Color opaqueColor = Microsoft.Xna.Framework.Color.White;
        Microsoft.Xna.Framework.Color transparent = new Microsoft.Xna.Framework.Color(0, 0, 0, 0);

        for (int y = 0; y < 2; y++)
        {
            for (int x = 0; x < 2; x++)
            {
                bool isDark = ((x + y) % 2 == 0);
                if (isDark)
                {
                    imageData.SetPixel(x, y, transparent);

                }
                else
                {
                    imageData.SetPixel(x, y, opaqueColor);
                }
            }
        }

        Texture2D texture = imageData.ToTexture2D(false);
        texture.Name = "Background Checkerboard";

        BackgroundSprite = new Sprite(texture);
        BackgroundSprite.Name = "Background checkerboard Sprite";
        BackgroundSprite.Wrap = true;
        BackgroundSprite.X = -4096;
        BackgroundSprite.Y = -4096;
        BackgroundSprite.Width = 8192;
        BackgroundSprite.Height = 8192;
        BackgroundSprite.Color = System.Drawing.Color.FromArgb(255, 150, 150, 150);

        BackgroundSprite.Wrap = true;
        int timesToRepeat = 256;
        BackgroundSprite.SourceRectangle =
        new System.Drawing.Rectangle(0, 0, timesToRepeat * texture.Width, timesToRepeat * texture.Height);

        systemManagers.SpriteManager.Add(BackgroundSprite);
    }

    public void Activity()
    {
        if (ProjectManager.Self.GeneralSettingsFile != null)
        {
            BackgroundSprite.Color = System.Drawing.Color.FromArgb(255,
                ProjectManager.Self.GeneralSettingsFile.CheckerColor2R,
                ProjectManager.Self.GeneralSettingsFile.CheckerColor2G,
                ProjectManager.Self.GeneralSettingsFile.CheckerColor2B
            );

        }
    }
}
