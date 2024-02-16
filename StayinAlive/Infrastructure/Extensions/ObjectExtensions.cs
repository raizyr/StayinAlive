using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StayinAlive.Infrastructure.Extensions
{
    public static class ObjectExtensions
    {
        public static string SafeGetString(this IModHelper helper, string key)
        {
            var result = string.Empty;

            if (!string.IsNullOrEmpty(key) &&
                helper != null)
            {
                result = helper.Translation.Get(key);
            }

            return result;
        }
    }
}
