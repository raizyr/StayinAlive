using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using StayinAlive.Options;
using StayinAlive.Infrastructure.Extensions;
using StardewValley.Locations;
using StayinAlive.Infrastructure;
using System.Linq;
using System.Reflection;


namespace StayinAlive.OverheadHealthbar
{
    internal sealed class Healthbar : IDisposable
    {
        private const int TextureBorderPixels = 3;
        private const int TextureBorderPadding = 6;

        private readonly ModOptions _options;
        private readonly IModHelper _helper;
        private readonly HealthMonitor _healthMonitor;

        private bool Enabled {  get; set; }
        private bool ShowInPeace {  get; set; }
        private bool HideWhenFull {  get; set; }

        private Texture2D Pixel => _pixelLazy.Value;

        internal Texture2D Texture;
        internal static int HeightAboveHead = 8;

        public Healthbar(IModHelper helper, ref ModOptions options, HealthMonitor healthMonitor)
        {
            _helper = helper;
            _options = options;
            _healthMonitor = healthMonitor;

            Texture = helper.ModContent.Load<Texture2D>("assets/healthbar.png");

            helper.Events.Display.RenderedWorld += OnRenderedWorld;
        }
        public void Dispose()
        {
            ToggleOption(false);
        }

        public void ToggleOption(bool showHealthbar)
        {
            Enabled = showHealthbar;

            _helper.Events.Display.RenderedWorld -= OnRenderedWorld;

            if (showHealthbar )
            {
                _helper.Events.Display.RenderedWorld += OnRenderedWorld;
            }
        }

        public void ToggleShowInPeace(bool showInPeace)
        {
            ShowInPeace = showInPeace;
            HideWhenFull = !showInPeace;
            ToggleOption(Enabled);
        }

        public void ToggleHideWhenFull(bool hideWhenFull)
        {
            HideWhenFull = hideWhenFull;
            ShowInPeace = !hideWhenFull;
            ToggleOption(Enabled);
        }

        private void DrawTexture(Farmer player)
        {
            var _playerPos = player.getLocalPosition(Game1.viewport);

            // Draw
            Game1.spriteBatch.Draw(
                Texture,
                new Rectangle(
                    (int)_playerPos.X - Texture.Width * Game1.pixelZoom / 2 + player.FarmerSprite.SpriteWidth * Game1.pixelZoom / 2,
                    (int)_playerPos.Y - player.FarmerSprite.SpriteHeight * Game1.pixelZoom - HeightAboveHead * Game1.pixelZoom,
                    Texture.Width * Game1.pixelZoom,
                    Texture.Height * Game1.pixelZoom),
                _options.TextureBorderColor.ToXnaColor());
        }

        private void DrawBar(Farmer player)
        {
            var _playerPos = player.getLocalPosition(Game1.viewport);
            var _healthRec = new Rectangle(
                   (int)_playerPos.X - Texture.Width * Game1.pixelZoom / 2 + player.FarmerSprite.SpriteWidth * Game1.pixelZoom / 2 + TextureBorderPixels * Game1.pixelZoom,
                   (int)_playerPos.Y - player.FarmerSprite.SpriteHeight * Game1.pixelZoom - (HeightAboveHead - TextureBorderPixels) * Game1.pixelZoom,
                   (int)((Pixel.Width * Game1.pixelZoom * Texture.Width - TextureBorderPadding * Game1.pixelZoom) * _healthMonitor.PercentOfMax),
                   Pixel.Height * Game1.pixelZoom * TextureBorderPadding);

            // Draw current health bar
            Game1.spriteBatch.Draw(Pixel, _healthRec, _options.HealthbarColor.ToXnaColor());
        }

        private void DrawBarBackground(Farmer player)
        {
            var _playerPos = player.getLocalPosition(Game1.viewport);
            var _healthBgRec = new Rectangle(
                   (int)_playerPos.X - Texture.Width * Game1.pixelZoom / 2 + player.FarmerSprite.SpriteWidth * Game1.pixelZoom / 2 + TextureBorderPixels * Game1.pixelZoom,
                   (int)_playerPos.Y - player.FarmerSprite.SpriteHeight * Game1.pixelZoom - (HeightAboveHead - TextureBorderPixels) * Game1.pixelZoom,
                   Pixel.Width * Game1.pixelZoom * Texture.Width - TextureBorderPadding * Game1.pixelZoom,
                   Pixel.Height * Game1.pixelZoom * TextureBorderPadding);

            // Draw health bar background
            Game1.spriteBatch.Draw(Pixel, _healthBgRec, _options.HealthbarBackgroundColor.ToXnaColor());
        }


        private readonly Lazy<Texture2D> _pixelLazy = new(() =>
        {
            Texture2D pixel = new(Game1.graphics.GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });
            return pixel;
        });

        internal void OnRenderedWorld(object? sender, RenderedWorldEventArgs e)
        {
            if (!Context.IsWorldReady) return;
            if (HideWhenFull && _healthMonitor.IsMax) return;
            if (ShowInPeace || Game1.currentLocation is MineShaft || Game1.currentLocation is Woods || Game1.currentLocation is SlimeHutch || Game1.currentLocation is VolcanoDungeon || !_healthMonitor.IsMax)
            { 
                DrawBarBackground(Game1.player);
                DrawBar(Game1.player);
                DrawTexture(Game1.player);
            }
        }
    }
}

