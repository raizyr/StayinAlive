using StardewModdingAPI;
using StardewValley;

namespace StayinAlive.Framework
{
  internal class HealthMonitor
  {
    private Mod Mod;

    internal float HealthPercent { get; private set; }

    public HealthMonitor(Mod mod)
    {
      this.Mod = mod;
      this.HealthPercent = (float)1;
    }

    public void GameLoop_UpdateTicked()
    {
      if (!Context.IsWorldReady)
        return;

      this.HealthPercent = (float)Game1.player.health / Math.Max(Game1.player.health, Game1.player.maxHealth);
    }
  }
}
