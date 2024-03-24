using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Reflection;
using StayinAlive.Infrastructure;
using StayinAlive.Infrastructure.Extensions;
using StayinAlive.OverheadHealthbar;
using StayinAlive.LifeAlert;
using Netcode;


namespace StayinAlive.Options
{
    internal class ModOptionsPageHandler : IDisposable
    {
        private readonly IModHelper _helper;
        private readonly bool _showPersonalConfigButton;

        private List<ModOptionsElement> _optionsElements = new();
        private readonly List<IDisposable> _elementsToDispose;

        private ModOptionsPage? _modOptionsPage;
        private ModOptionsPageButton? _modOptionsPageButton;
        private int _modOptionsTabPageNumber;

        private PerScreen<IClickableMenu> _lastMenu = new();
        private List<int> _instancesWithOptionsPageOpen = new();
        private bool _windowResizing = false;

        public ModOptionsPageHandler(IModHelper helper, ModOptions options, HealthMonitor? healthMonitor, bool? showPersonalConfigButton)
        {
            _helper = helper;
            if (showPersonalConfigButton != null) _showPersonalConfigButton = (bool)showPersonalConfigButton;

            if (_showPersonalConfigButton)
            {
                helper.Events.Display.RenderingActiveMenu += OnRenderingMenu;
                helper.Events.Display.RenderedActiveMenu += OnRenderedMenu;
                GameRunner.instance.Window.ClientSizeChanged += OnWindowClientSizeChanged;
                helper.Events.Display.WindowResized += OnWindowResized;
            }

            _elementsToDispose = new List<IDisposable>();

            if (healthMonitor != null)
            {
                var healthbar = new Healthbar(helper, ref options, healthMonitor);
                var lowHealthAlarm = new LowHealthAlarm(helper, ref options, healthMonitor);

                _elementsToDispose = new List<IDisposable>()
                {
                    healthbar,
                    lowHealthAlarm
                };

                int whichOption = 1;
                Version thisVersion = Assembly.GetAssembly(GetType())!.GetName().Version;
                _optionsElements.Add(new ModOptionsElement("StayinAlive v" + thisVersion!.Major + "." + thisVersion.Minor + "." + thisVersion.Build));

                var healthbarCheckbox = new ModOptionsCheckbox(_helper.SafeGetString(nameof(options.ShowHealthbar)), whichOption++, healthbar.ToggleOption, () => options.ShowHealthbar, v => options.ShowHealthbar = v);
                _optionsElements.Add(healthbarCheckbox);
                _optionsElements.Add(new ModOptionsCheckbox(_helper.SafeGetString(nameof(options.ShowInPeace)), whichOption++, healthbar.ToggleShowInPeace, () => options.ShowInPeace, v => { options.ShowInPeace = v; options.HideWhenFull = !v; }, healthbarCheckbox));
                _optionsElements.Add(new ModOptionsCheckbox(_helper.SafeGetString(nameof(options.HideWhenFull)), whichOption++, healthbar.ToggleHideWhenFull, () => options.HideWhenFull, v => { options.HideWhenFull = v; options.ShowInPeace = !v; }, healthbarCheckbox));
                _optionsElements.Add(new ModOptionsDropDown(_helper.SafeGetString(nameof(options.HealthbarColor)), whichOption++, () => options.HealthbarColor.Options(), () => options.HealthbarColor.DisplayOptions(), () => options.HealthbarColor.ToString() , v => { options.HealthbarColor = Enum.Parse<Colors>(v); }, healthbarCheckbox));
                _optionsElements.Add(new ModOptionsDropDown(_helper.SafeGetString(nameof(options.HealthbarBackgroundColor)), whichOption++, () => options.HealthbarBackgroundColor.Options(), () => options.HealthbarBackgroundColor.DisplayOptions(), () => options.HealthbarBackgroundColor.ToString(), v => { options.HealthbarBackgroundColor = Enum.Parse<Colors>(v); }, healthbarCheckbox));

                var lowHealthAlarmCheckbox = new ModOptionsCheckbox(_helper.SafeGetString(nameof(options.EnableLowHealthAlarm)), whichOption++, lowHealthAlarm.ToggleOption, () => options.EnableLowHealthAlarm, v => options.EnableLowHealthAlarm = v);
                _optionsElements.Add(lowHealthAlarmCheckbox);
            }
        }

        public void Dispose()
        {
            foreach (var item in _elementsToDispose)
                item.Dispose();
        }

        private void OnButtonLeftClicked(object? sender, EventArgs e)
        {
            // Do not activate when an action is being remapped
            if (Game1.activeClickableMenu is GameMenu gameMenu && gameMenu.readyToClose())
            {
                gameMenu.currentTab = _modOptionsTabPageNumber;
                Game1.playSound("smallSelect");
            }
        }

        // Early because it is called during Display.RenderingActiveMenu instead of later during Display.MenuChanged,
        private void EarlyOnMenuChanged(IClickableMenu? oldMenu, IClickableMenu? newMenu)
        {
            if (_showPersonalConfigButton)
            {
                // Remove from old menu
                if (oldMenu is GameMenu oldGameMenu)
                {
                    if (_modOptionsPage != null)
                    {
                        oldGameMenu.pages.Remove(_modOptionsPage);
                        _modOptionsPage = null;
                    }
                    if (_modOptionsPageButton != null)
                    {
                        _modOptionsPageButton.OnLeftClicked -= OnButtonLeftClicked;
                        _modOptionsPageButton = null;
                    }
                }

                // Add to new menu
                if (newMenu is GameMenu newGameMenu)
                {
                    // Both modOptions variables require Game1.activeClickableMenu to not be null.
                    if (_modOptionsPage == null)
                        _modOptionsPage = new ModOptionsPage(_optionsElements, _helper.Events);
                    if (_modOptionsPageButton == null)
                        _modOptionsPageButton = new ModOptionsPageButton(_helper.Events);

                    _modOptionsPageButton.OnLeftClicked += OnButtonLeftClicked;
                    List<IClickableMenu> tabPages = newGameMenu.pages;
                    _modOptionsTabPageNumber = tabPages.Count;
                    tabPages.Add(_modOptionsPage);
                }
            }
        }

        private void OnRenderingMenu(object? sender, RenderingActiveMenuEventArgs e)
        {
            if (_showPersonalConfigButton)
            {
                // Trigger the "EarlyOnMenuChanged" event
                if (_lastMenu.Value != Game1.activeClickableMenu)
                {
                    EarlyOnMenuChanged(_lastMenu.Value, Game1.activeClickableMenu);
                    _lastMenu.Value = Game1.activeClickableMenu;
                }
                if (Game1.activeClickableMenu is GameMenu gameMenu)
                {
                    // Draw our tab icon behind the menu even if it is dimmed by the menu's transparent background,
                    // so that it still displays during transitions eg. when a letter is viewed in the collections tab
                    DrawButton(gameMenu);
                }
            }
        }

        private void OnRenderedMenu(object? sender, RenderedActiveMenuEventArgs e)
        {
            if (_showPersonalConfigButton
                && Game1.activeClickableMenu is GameMenu gameMenu
                // But don't render when the map is displayed...
                && !(gameMenu.currentTab == GameMenu.mapTab
                    // ...or when a letter is opened in the collection's page
                    || gameMenu.GetCurrentPage() is CollectionsPage cPage && cPage.letterviewerSubMenu != null
                ))
            {
                DrawButton(gameMenu);

                // Draw the game menu's hover text again so it displays above our tab
                if (!gameMenu.hoverText.Equals(""))
                    IClickableMenu.drawHoverText(Game1.spriteBatch, gameMenu.hoverText, Game1.smallFont);
            }
        }

        private void OnWindowClientSizeChanged(object? sender, EventArgs e)
        {
            if (_showPersonalConfigButton)
            {
                _windowResizing = true;
                GameRunner.instance.ExecuteForInstances((Game1 instance) => {
                    if (Game1.activeClickableMenu is GameMenu gameMenu && gameMenu.currentTab == _modOptionsTabPageNumber)
                    {
                        // Temporarily change all open mod options pages to the game's options page
                        // because the GameMenu is recreated when the window is resized, before we can add
                        // our mod options page to GameMenu#pages.
                        gameMenu.currentTab = GameMenu.optionsTab;
                        _instancesWithOptionsPageOpen.Add(instance.instanceId);
                    }
                });
            }
        }

        private void OnWindowResized(object? sender, EventArgs e)
        {
            if (_windowResizing)
            {
                _windowResizing = false;
                GameRunner.instance.ExecuteForInstances((Game1 instance) => {
                    if (_instancesWithOptionsPageOpen.Remove(instance.instanceId))
                    {
                        if (Game1.activeClickableMenu is GameMenu gameMenu)
                        {
                            gameMenu.currentTab = _modOptionsTabPageNumber;
                        }
                    }
                });
            }
        }

        private void DrawButton(GameMenu gameMenu)
        {
            _modOptionsPageButton!.yPositionOnScreen = gameMenu.yPositionOnScreen + (gameMenu.currentTab == _modOptionsTabPageNumber ? 24 : 16);
            _modOptionsPageButton.draw(Game1.spriteBatch);
        }
    }
}
