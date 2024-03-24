using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.Monsters;
using StayinAlive.Compatibility;
using StayinAlive.Infrastructure;
using StayinAlive.LifeAlert;
using StayinAlive.Options;


namespace StayinAlive
{
    internal sealed class ModEntry : Mod
    {
        public static IMonitor? MonitorObject { get; private set; }

        private ModConfig? _modConfig;

        private ModOptions? _modOptions;
        private ModOptionsPageHandler? _modOptionsPageHandler;
        private HealthMonitor? _healthMonitor;

        public override void Entry(IModHelper helper)
        {
            MonitorObject = Monitor;
            _modConfig = Helper.ReadConfig<ModConfig>();
            _healthMonitor = new HealthMonitor();
            _healthMonitor.Entry(helper);


            helper.Events.GameLoop.ReturnedToTitle += OnReturnedToTitle;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.GameLoop.Saved += OnSaved;
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        }

        private void OnReturnedToTitle(object sender, ReturnedToTitleEventArgs e)
        {
            // Unload if the main player quits.
            if (Context.ScreenId != 0) return;

            _modOptionsPageHandler?.Dispose();
            _modOptionsPageHandler = null;
        }

        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            if (Context.ScreenId != 0) return;

            _modOptions = Helper.Data.ReadJsonFile<ModOptions>($"data/{Constants.SaveFolderName}.json") 
                ?? new ModOptions();

            _modOptionsPageHandler = new ModOptionsPageHandler(Helper, _modOptions, _healthMonitor, _modConfig.ShowOptionsTabInMenu);
        }

        private void OnSaved(object sender, SavedEventArgs e)
        {
            if (Context.ScreenId != 0) return;

            Helper.Data.WriteJsonFile($"data/{Constants.SaveFolderName}.json", _modOptions);
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            var modVersion = Helper.ModRegistry.Get("spacechase0.GenericModConfigMenu")?.Manifest?.Version;
            var minModVersion = "1.6.0";
            if (modVersion?.IsOlderThan(minModVersion) == true)
            {
                Monitor.Log($"Detected Generic Mod Config menue {modVersion} but expected {minModVersion} or newer. Disabling integration with th at mod.", LogLevel.Warn);
                return;
            }

            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            configMenu.Register(
                mod: ModManifest,
                reset: () => _modConfig = new ModConfig(),
                save: () => Helper.WriteConfig(_modConfig)
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Show option in in-game configMenu",
                tooltip: () => "Enables an extra tab in the in-game configMenu where you can configure every options for this mod.",
                getValue: () => _modConfig.ShowOptionsTabInMenu,
                setValue: value => _modConfig.ShowOptionsTabInMenu = value
            );
        }
    }
}
