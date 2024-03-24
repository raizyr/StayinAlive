using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StayinAlive.Options
{
    internal class ModOptionsDropDown : ModOptionsElement
    {
        [InstancedStatic]
        public static ModOptionsDropDown? selected;

        public List<string> dropDownOptions = new List<string>();
        public List<string> dropDownDisplayOptions = new List<string>();
        public int recentSlotY;
        public int startingSelected;

        public Rectangle dropDownBounds;

        public static Rectangle dropDownBGSource = new Rectangle(433, 451, 3, 3);
        public static Rectangle dropDownButtonSource = new Rectangle(437, 450, 10, 11);

        private int selectedOption;
        private readonly Action<string> setOption;
        private bool _canClick => _parent! is not ModOptionsCheckbox || ((ModOptionsCheckbox)_parent!)._isChecked;
        private bool clicked;

        public ModOptionsDropDown(
            string label,
            int whichOption,
            Func<List<string>> getOptions,
            Func<List<string>> getDisplayOptions,
            Func<string> getOption,
            Action<string> setOption,
            ModOptionsElement? parent = null)
            : base(label, -1, -1, (int)Game1.smallFont.MeasureString("Windowed Borderless Mode   ").X + 48, 44, whichOption, parent)
        {
            dropDownOptions = getOptions();
            dropDownDisplayOptions = getDisplayOptions();

            this.setOption = setOption;

            selectedOption = dropDownOptions.IndexOf(getOption());
            RecalculateBounds();
        }

        public virtual void RecalculateBounds()
        {
            foreach (string dropDownDisplayOption in dropDownDisplayOptions)
            {
                float x = Game1.smallFont.MeasureString(dropDownDisplayOption).X;
                if (x >= (float)(bounds.Width - 48))
                {
                    bounds.Width = (int)(x + 64f);
                }
            }

            dropDownBounds = new Rectangle(bounds.X, bounds.Y, bounds.Width - 48, bounds.Height * dropDownOptions.Count);
        }

        public override void ReceiveLeftClick(int x, int y)
        {
            if (!greyedOut)
            {
                base.ReceiveLeftClick(x, y);
                startingSelected = selectedOption;
                if (!clicked)
                {
                    Game1.playSound("shwip");
                }

                LeftClickHeld(x, y);
                selected = this;
            }
        }

        public override void LeftClickHeld(int x, int y)
        {
            if (!greyedOut)
            {
                base.LeftClickHeld(x, y);
                clicked = true;
                dropDownBounds.Y = Math.Min(dropDownBounds.Y, Game1.uiViewport.Height - dropDownBounds.Height - recentSlotY);
                if (!Game1.options.SnappyMenus)
                {
                    selectedOption = (int)Math.Max(Math.Min((float)(y - dropDownBounds.Y) / (float)bounds.Height, dropDownOptions.Count - 1), 0f);
                }
            }
        }

        public override void LeftClickReleased(int x, int y)
        {
            if (!greyedOut && dropDownOptions.Count > 0)
            {
                base.LeftClickReleased(x, y);
                if (clicked)
                {
                    Game1.playSound("drumkit6");
                }

                clicked = false;
                selected = this;
                if (dropDownBounds.Contains(x, y) || (Game1.options.gamepadControls && !Game1.lastCursorMotionWasMouse))
                {
                    setOption(dropDownOptions[selectedOption]);
                }
                else
                {
                    selectedOption = startingSelected;
                }

                selected = null;
            }
        }

        public override void Draw(SpriteBatch batch, int slotX, int slotY, IClickableMenu? context = null)
        {
            recentSlotY = slotY;
            base.Draw(batch, slotX, slotY, context);
            float num = (greyedOut ? 0.33f : 1f);
            if (clicked)
            {
                IClickableMenu.drawTextureBox(batch, Game1.mouseCursors, dropDownBGSource, slotX + dropDownBounds.X, slotY + dropDownBounds.Y, dropDownBounds.Width, dropDownBounds.Height, Color.White * num, 4f, drawShadow: false, 0.97f);
                for (int i = 0; i < dropDownDisplayOptions.Count; i++)
                {
                    if (i == selectedOption)
                    {
                        batch.Draw(Game1.staminaRect, new Rectangle(slotX + dropDownBounds.X, slotY + dropDownBounds.Y + i * bounds.Height, dropDownBounds.Width, bounds.Height), new Rectangle(0, 0, 1, 1), Color.Wheat, 0f, Vector2.Zero, SpriteEffects.None, 0.975f);
                    }

                    batch.DrawString(Game1.smallFont, dropDownDisplayOptions[i], new Vector2(slotX + dropDownBounds.X + 4, slotY + dropDownBounds.Y + 8 + bounds.Height * i), Game1.textColor * num, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.98f);
                }
            }
            else
            {
                IClickableMenu.drawTextureBox(batch, Game1.mouseCursors, dropDownBGSource, slotX + bounds.X, slotY + bounds.Y, bounds.Width - 48, bounds.Height, Color.White * num, 4f, drawShadow: false);
                batch.DrawString(Game1.smallFont, (selectedOption < dropDownDisplayOptions.Count && selectedOption >= 0) ? dropDownDisplayOptions[selectedOption] : "", new Vector2(slotX + bounds.X + 4, slotY + bounds.Y + 8), Game1.textColor * num, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.88f);
                batch.Draw(Game1.mouseCursors, new Vector2(slotX + bounds.X + bounds.Width - 48, slotY + bounds.Y), dropDownButtonSource, Color.White * num, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.88f);
            }
        }
    }
}
