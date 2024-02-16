using StayinAlive.OverheadHealthbar;
using Microsoft.Xna.Framework;

namespace StayinAlive.Infrastructure.Extensions
{
    public static class EnumExtensions
    {
        public static Color ToXnaColor(this Colors value)
        {
            return new Color((uint)value);
        }
    }
}
