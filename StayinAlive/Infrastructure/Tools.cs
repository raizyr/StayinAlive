using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace StayinAlive.Infrastructure
{
    public static class Tools
    {
        public static void DrawMouseCursor()
        {
            if (!Game1.options.hardwareCursor)
            {
                int mouseCursorToRender = Game1.options.gamepadControls ? Game1.mouseCursor + 44 : Game1.mouseCursor;
                var what = Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, mouseCursorToRender, 16, 16);

                Game1.spriteBatch.Draw(
                    Game1.mouseCursors,
                    new Vector2(Game1.getMouseX(), Game1.getMouseY()),
                    what,
                    Color.White,
                    0.0f,
                    Vector2.Zero,
                    Game1.pixelZoom + (Game1.dialogueButtonScale / 150.0f),
                    SpriteEffects.None,
                    1f);
            }
        }

        public static int GetWidthInPlayArea()
        {
            if (Game1.isOutdoorMapSmallerThanViewport())
            {
                int right = Game1.graphics.GraphicsDevice.Viewport.TitleSafeArea.Right;
                int totalWidth = Game1.currentLocation.map.Layers[0].LayerWidth * Game1.tileSize;
                int someOtherWidth = Game1.graphics.GraphicsDevice.Viewport.TitleSafeArea.Right - totalWidth;

                return right - someOtherWidth / 2;
            }
            else
            {
                return Game1.graphics.GraphicsDevice.Viewport.TitleSafeArea.Right;
            }
        }
    }
}
