using StayinAlive.OverheadHealthbar;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using Sickhead.Engine.Util;

namespace StayinAlive.Infrastructure.Extensions
{
    public static class EnumExtensions
    {
        public static Color ToXnaColor(this Colors value)
        {
            return new Color((uint)value);
        }



        private static readonly List<string> _colors = new(Enum.GetNames(typeof(Colors)));

        public static int IndexOf(this Colors value)
        {
            string? _name = Enum.GetName(typeof(Colors), value);
            if (_name == null) return -1;
            return _colors.IndexOf(_name);
        }

        public static string Name(this Colors value) => value.ToString();
        public static Colors FromName(this Colors _, string name)
        {
            return Enum.Parse<Colors>(name);
        }

        public static List<string> Options(this Colors value)
        {
            var names = value.GetType().GetFields()
                .Where(m => m.GetCustomAttribute<DisplayAttribute>() != null)
                .Select(m => m.GetValue(value)!.ToString());
            return new List<string>(names!.ToArray<string>());
        }

        public static List<string> DisplayOptions(this Colors value)
        {
            var names = value.GetType().GetFields()
                .Where(m => m.GetCustomAttribute<DisplayAttribute>() != null)
                .Select(m => m.GetCustomAttribute<DisplayAttribute>()!.GetName());
            return new List<string>(names!.ToArray<string>());
        }
    }
}
