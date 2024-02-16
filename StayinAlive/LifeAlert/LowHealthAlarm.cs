using Microsoft.Xna.Framework.Audio;
using StardewModdingAPI.Events;
using StardewModdingAPI;
using StardewValley;
using System.IO;
using StayinAlive;
using StayinAlive.Options;
using System;
using System.Collections.Generic;
using StayinAlive.Infrastructure;

namespace StayinAlive.LifeAlert
{
    internal class LowHealthAlarm : IDisposable
    {
        private ICue Cue;

        private readonly IModHelper _helper;
        private readonly ModOptions _options;
        private readonly HealthMonitor _healthMonitor;
        private bool Enabled { get; set; }

        internal LowHealthAlarm(IModHelper helper, ModOptions options, HealthMonitor healthMonitor)
        {
            _helper = helper;
            _options = options;
            _healthMonitor = healthMonitor;

            SoundEffect alert;
            string filePathCombined = Path.Combine(helper.DirectoryPath, "assets/totk_low_health_combined.wav");
            var bytes = File.ReadAllBytes(filePathCombined);
            alert = new SoundEffect(bytes, 0, bytes.Length, 44100, AudioChannels.Stereo, 88480, 90390);

            var cue = new CueDefinition("low_health_warning", alert, Game1.audioEngine.GetCategoryIndex("Sound"), true)
            {
                instanceLimit = 1,
                limitBehavior = CueDefinition.LimitBehavior.FailToPlay
            };

            Game1.soundBank.AddCue(cue);
            Cue = Game1.soundBank.GetCue(cue.name);
        }

        public void Dispose()
        {
            ToggleOption(false);
            if (!Cue.IsStopped) Cue.Stop(AudioStopOptions.Immediate);
            Cue.Dispose();
        }

        public void ToggleOption(bool enableLowHealthAlarm)
        {
            Enabled = enableLowHealthAlarm;

            _helper.Events.GameLoop.UpdateTicked -= OnUpdateTicked;
            _helper.Events.Player.Warped -= OnWarped;

            if (enableLowHealthAlarm)
            {
                _helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
                _helper.Events.Player.Warped += OnWarped;
            }
        }

        internal void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady) return;
            if (!Enabled) return;

            if (_healthMonitor.IsHalf)
            {
                if (!Cue.IsPlaying) PlayCue();
                return;
            }

            Cue.Stop(AudioStopOptions.Immediate);
        }

        internal void OnWarped(object? sender, WarpedEventArgs e)
        {
            if (!Context.IsWorldReady) return;
            if (!Enabled) return;
        }

        internal void PlayCue()
        {
            if (Cue.IsPaused)
            {
                Cue.Resume();
            }
            else
            {
                Cue = Game1.soundBank.GetCue(Cue.Name);
                Cue.Play();
            }
        }
        internal void PauseCue()
        {
            if (Cue.IsPlaying)
            {
                Cue.Pause();
            }
        }
    }
}
