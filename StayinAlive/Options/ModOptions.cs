using StayinAlive.OverheadHealthbar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StayinAlive.Options
{
    internal record ModOptions
    {
        public bool ShowHealthbar { get; set; } = true;
        public bool ShowInPeace { get; set; } = false;
        public bool HideWhenFull { get; set; } = false;
        public Colors HealthbarColor { get; set; } = Colors.Crimson;
        public Colors HealthbarBackgroundColor { get; set; } = Colors.Black;
        public Colors TextureBorderColor { get; set; } = Colors.White;

        public bool EnableLowHealthAlarm { get; set; } = true;
    }
}
