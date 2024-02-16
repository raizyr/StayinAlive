using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Threading.Tasks;

namespace StayinAlive.Infrastructure
{
    internal sealed class HealthMonitor
    {
        internal float PercentOfMax { get; private set; } = 1;
        internal bool IsAlive { get; private set; } = true;
        internal bool IsMax { get; private set; } = true;
        internal bool IsHalf { get; private set; } = false;
        internal bool IsQuarter { get; private set; } = false;
        internal bool IsPassedOut { get; private set; } = false;

        internal HealthMonitor() { }
    
        internal void Entry(IModHelper helper) 
        { 
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
        }

        internal void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            PercentOfMax = (float)Game1.player.health / Math.Max(Game1.player.health, Game1.player.maxHealth);
            IsAlive = PercentOfMax > 0;
            IsMax = PercentOfMax >= 1;
            IsHalf = PercentOfMax <= 0.5;
            IsQuarter = PercentOfMax <= 0.25;
            IsPassedOut = Game1.player.health <= 10;
        }
    }
}
