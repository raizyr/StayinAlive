using Microsoft.Xna.Framework.Audio;
using StardewModdingAPI.Events;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace StayinAlive.Framework
{
    internal class LifeAlert
    {
        internal CueDefinition cue;


        internal Mod Mod;
        internal LifeAlert(Mod mod)
        {
            this.Mod = mod;
            SoundEffect audio;
            string filePathCombined = Path.Combine(this.Mod.Helper.DirectoryPath, "assets/low-health-warning.wav");
            using (var stream = new System.IO.FileStream(filePathCombined, FileMode.Open))
            {
                audio = SoundEffect.FromStream(stream);
            }

            this.cue = new CueDefinition("low_health_warning", audio, Game1.audioEngine.GetCategoryIndex("Sound"), false)
            {
                instanceLimit = 1,
                limitBehavior = CueDefinition.LimitBehavior.FailToPlay
            };
            Game1.soundBank.AddCue(this.cue);
            this.Mod.Monitor.Log("Sound effects loaded", LogLevel.Debug);
        }

        internal void GameLoop_UpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady) return;
            var _maxHealth = Math.Max(Game1.player.health, Game1.player.maxHealth);
            var CurrentHealthPercent = (float)Game1.player.health / _maxHealth;

            if (CurrentHealthPercent < 0.5 && Game1.showingHealth)
            {
                Game1.playSound(cue.name);
            }
        }
    }
}
