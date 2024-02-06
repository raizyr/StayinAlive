using GenericModConfigMenu;
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
        public bool AlwaysShowOverheadHealthbar { get; set; }
        public bool HideOverheadHealthbarWhenFull { get; set; }

        public ModConfig()
        {
            this.AlwaysShowOverheadHealthbar = false;
            this.HideOverheadHealthbarWhenFull = false;
        }
    }
}
