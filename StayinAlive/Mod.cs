using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Monsters;
using StayinAlive.Framework;
using GenericModConfigMenu;


namespace StayinAlive
{
    internal sealed class Mod : StardewModdingAPI.Mod
    {
        private LifeAlert lifeAlert;
        private OverheadHealthbar healthbar;

        private ModConfig Config;

        public override void Entry(IModHelper helper)
        {
            this.Config = this.Helper.ReadConfig<ModConfig>();

            this.healthbar = new OverheadHealthbar(this);
            this.lifeAlert = new LifeAlert(this);

            this.Helper.Events.Display.RenderedWorld += Display_RenderedWorld;
            this.Helper.Events.GameLoop.UpdateTicked += GameLoop_UpdateTicked;
            this.Helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunched;
        }

        private void GameLoop_GameLaunched(object sender, GameLaunchedEventArgs e)
        {
            OverheadHealthbar.Texture = this.Helper.ModContent.Load<Texture2D>("assets/healthbar.png");
            this.Monitor.Log("Textures loaded", LogLevel.Debug);

            

            

            // get Generic Mod Config Menu's API (if it's installed)
            var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            this.RegisterConfigMenu(configMenu);


        }

        private void Display_RenderedWorld(object sender, RenderedWorldEventArgs e)
        {
            OverheadHealthbar.Display_RenderedWorld(sender, e);
        }

        private void GameLoop_UpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            this.lifeAlert.GameLoop_UpdateTicked(sender, e);

        }

        private void RegisterConfigMenu(IGenericModConfigMenuApi menu)
        {
            menu.Register(
                mod: this.ModManifest,
                reset: () => this.Config = new ModConfig(),
                save: () => this.Helper.WriteConfig(this.Config)
            );

            menu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Always Show Overhead Healthbar",
                tooltip: () => "If checked, Overhead Healthbar shows even in peaceful areas.  If unchecked, the Overhead Healthbar will only show in combat areas.",
                getValue: () => this.Config.AlwaysShowOverheadHealthbar,
                setValue: value => this.Config.AlwaysShowOverheadHealthbar = value
            );

            menu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Hide Overhead Healthbar When Full",
                tooltip: () => "If checked, Overhead Healthbar shows only when hurt.",
                getValue: () => this.Config.HideOverheadHealthbarWhenFull,
                setValue: value => this.Config.HideOverheadHealthbarWhenFull = value
            );

        }
    }
}
