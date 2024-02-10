using GenericModConfigMenu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StayinAlive
{
    public sealed class ModConfig
    {
        /*
         * AlwaysShowOverheadHealthbar: bool
         * if true, the healthbar will be show even in peaceful areas
         */
        public bool AlwaysShowOverheadHealthbar { get; set; }
        /*
         * HideOverheadHealthbarWhenFull: bool
         * if true, the healthbar will be hidden when the player's health is full
         */
        public bool HideOverheadHealthbarWhenFull { get; set; }
        /*
         * HealthbarColor: Color
         * The color of the healthbar
         */
        public Color HealthbarColor { get; set; }
        /*
         * HealthbarBackgroundColor: Color
         * The color of the healthbar's background, visible when the healthbar is not full
         */
        public Color HealthbarBackgroundColor { get; set; }
        /*
         * TextureBorderColor: Color
         * The color of the border around the healthbar texture, seen only if issues loading textures
         */
        public Color TextureBorderColor { get; set; }

        public ModConfig()
        {
            this.AlwaysShowOverheadHealthbar = false;
            this.HideOverheadHealthbarWhenFull = false;
            this.HealthbarColor = new Color(226, 64, 64, 255);
            this.HealthbarBackgroundColor = new Color(0, 0, 0, 135);
            this.TextureBorderColor = new Color(255, 255, 255, 255);
        }
    }
}
