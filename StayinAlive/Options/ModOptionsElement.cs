using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using System.Reflection.Emit;

namespace StayinAlive.Options
{
    internal class ModOptionsElement
    {
        public enum Style
        {
            Default,
            OptionLabel
        }
        public Style style;
        public bool greyedOut;
        public Vector2 labelOffset = Vector2.Zero;


        protected const int DefaultX = 8;
        protected const int DefaultY = 4;
        protected const int DefaultPixelSize = 9;

        internal Rectangle bounds;
        internal string label;
        internal int whichOption;

        protected readonly ModOptionsElement? _parent;

        public Rectangle Bounds => bounds;

        public ModOptionsElement(string label, int x, int y, int width, int height, int whichOption = -1, ModOptionsElement? parent = null)
        {
            if (x == -1) x = 32;
            if (y == -1) y = 16;

            bounds = new Rectangle(x, y, width, height);
            this.label = label;
            this.whichOption = whichOption;
            _parent = parent;
        }

        public ModOptionsElement(string label, int whichOption = -1, ModOptionsElement? parent = null)
        {
            int x = DefaultX * Game1.pixelZoom;
            int y = DefaultY * Game1.pixelZoom;
            int width = DefaultPixelSize * Game1.pixelZoom;
            int height = DefaultPixelSize * Game1.pixelZoom;

            if (parent != null)
                x += DefaultX * 2 * Game1.pixelZoom;

            bounds = new Rectangle(x, y, width, height);
            this.label = label;
            this.whichOption = whichOption;
            _parent = parent;
        }

        public virtual void ReceiveLeftClick(int x, int y)
        {

        }

        public virtual void LeftClickHeld(int x, int y)
        {

        }

        public virtual void LeftClickReleased(int x, int y)
        {

        }

        public virtual void ReceiveKeyPress(Keys key)
        {

        }

        public virtual void Draw(SpriteBatch batch, int slotX, int slotY, IClickableMenu? context = null)
        {
            if (style == Style.OptionLabel)
            {
                Utility.drawTextWithShadow(batch, label, Game1.dialogueFont, new Vector2(slotX + bounds.X + (int)labelOffset.X, slotY + bounds.Y + (int)labelOffset.Y + 12), greyedOut ? (Game1.textColor * 0.33f) : Game1.textColor, 1f, 0.1f);
                return;
            }

            if (whichOption < 0)
            {
                SpriteText.drawString(batch, label, slotX + bounds.X, slotY + bounds.Y + Game1.pixelZoom * 3, 999, -1, 999, 1, 0.1f);
                return;
            }
            int num = slotX + bounds.X + bounds.Width + 8 + (int)labelOffset.X;
            int num2 = slotY + bounds.Y + (int)labelOffset.Y;
            string text = label;
            SpriteFont spriteFont = Game1.dialogueFont;
            if (context != null)
            {
                int num3 = context.width - 64;
                int xPositionOnScreen = context.xPositionOnScreen;
                if (spriteFont.MeasureString(label).X + (float)num > (float)(num3 + xPositionOnScreen))
                {
                    int width = num3 + xPositionOnScreen - num;
                    spriteFont = Game1.smallFont;
                    text = Game1.parseText(label, spriteFont, width);
                    num2 -= (int)((spriteFont.MeasureString(text).Y - spriteFont.MeasureString("T").Y) / 2f);
                }
            }

            Utility.drawTextWithShadow(batch, text, spriteFont, new Vector2(num, num2), greyedOut ? (Game1.textColor * 0.33f) : Game1.textColor, 1f, 0.1f);
        }
    }
}
