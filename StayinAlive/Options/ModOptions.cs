using StardewValley;
using StardewModdingAPI;
using StayinAlive.Infrastructure.Extensions;
using StayinAlive.OverheadHealthbar;
using System;

namespace StayinAlive.Options
{
    internal record ModOptions
    {
        public bool ShowHealthbar { get; set; } = true;
        public bool ShowInPeace { get; set; } = false;
        public bool HideWhenFull { get; set; } = false;
        public Colors HealthbarColor { get; set; } = Colors.Red;
        public Colors HealthbarBackgroundColor { get; set; } = Colors.Black;
        public Colors TextureBorderColor { get; set; } = Colors.White;

        public bool EnableLowHealthAlarm { get; set; } = true;
    }
}
