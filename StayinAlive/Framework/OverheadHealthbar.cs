using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Text.Json;


namespace StayinAlive.Framework
{
    internal sealed class OverheadHealthbar
    {

        const int TextureBorderPixels = 3;
        const int TextureBorderPadding = 6;

        private Texture2D Pixel => this._pixelLazy.Value;
        private static Color BorderColor = new Color(255, 255, 255, 255);

        internal static Texture2D Texture;
        internal static int HeightAboveHead = 8;

        internal Mod Mod;

        public OverheadHealthbar(Mod mod)
        {
            this.Mod = mod;
        }

        private void DrawTexture(Farmer player)
        {
            var _playerPos = player.getLocalPosition(Game1.viewport);

            // Draw
            Game1.spriteBatch.Draw(
                Texture,
                new Rectangle(
                    // player.X - Texture.scaledWidth / 2 + FarmerSprite.scaledWidth / 2
                    (int)_playerPos.X - Texture.Width * Game1.pixelZoom / 2 + player.FarmerSprite.SpriteWidth * Game1.pixelZoom / 2,
                    // player.Y - FarmerSprite.scaledHeight - scaledHeightAbovePlayerHead
                    (int)_playerPos.Y - player.FarmerSprite.SpriteHeight * Game1.pixelZoom - HeightAboveHead * Game1.pixelZoom,
                    Texture.Width * Game1.pixelZoom,
                    Texture.Height * Game1.pixelZoom),
                this.Mod.Config.TextureBorderColor);
            this.Mod.Monitor.Log("OverheadHealthbar Texture drawn", LogLevel.Trace);
        }

        private void DrawBar(Farmer player)
        {
            var _playerPos = player.getLocalPosition(Game1.viewport);
            var _healthRec = new Rectangle(
                   (int)_playerPos.X - Texture.Width * Game1.pixelZoom / 2 + player.FarmerSprite.SpriteWidth * Game1.pixelZoom / 2 + this.TextureBorderPixels * Game1.pixelZoom,
                   (int)_playerPos.Y - player.FarmerSprite.SpriteHeight * Game1.pixelZoom - (HeightAboveHead - this.TextureBorderPixels) * Game1.pixelZoom,
                   (int)((Pixel.Width * Game1.pixelZoom * Texture.Width - this.TextureBorderPadding * Game1.pixelZoom) * this.Mod.healthmonitor.HealthPercent),
                   Pixel.Height * Game1.pixelZoom * this.TextureBorderPadding);

            // Draw current health bar
            Game1.spriteBatch.Draw(Pixel, _healthRec, this.Mod.Config.HealthbarColor);
            this.Mod.Monitor.Log("OverheadHealthbar Bar drawn", LogLevel.Trace);
        }

        private void DrawBarBackground(Farmer player)
        {
            var _playerPos = player.getLocalPosition(Game1.viewport);
            var _healthBgRec = new Rectangle(
                   (int)_playerPos.X - Texture.Width * Game1.pixelZoom / 2 + player.FarmerSprite.SpriteWidth * Game1.pixelZoom / 2 + this.TextureBorderPixels * Game1.pixelZoom,
                   (int)_playerPos.Y - player.FarmerSprite.SpriteHeight * Game1.pixelZoom - (HeightAboveHead - this.TextureBorderPixels) * Game1.pixelZoom,
                   Pixel.Width * Game1.pixelZoom * Texture.Width - this.TextureBorderPadding * Game1.pixelZoom,
                   Pixel.Height * Game1.pixelZoom * this.TextureBorderPadding);

            // Draw health bar background
            Game1.spriteBatch.Draw(Pixel, _healthBgRec, this.Mod.Config.HealthbarBackgroundColor);
            this.Mod.Monitor.Log("OverheadHealthbar BarBackground drawn", LogLevel.Trace);
        }


        private readonly Lazy<Texture2D> _pixelLazy = new(() =>
        {
            Texture2D pixel = new Texture2D(Game1.graphics.GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });
            return pixel;
        });

        internal async void Display_RenderedWorld(object sender, RenderedWorldEventArgs e)
        {
            if (!Context.IsWorldReady) return;
            if (!Game1.showingHealth && !this.Mod.Config.AlwaysShowOverheadHealthbar) return;
            if (!Game1.showingHealth && !this.Mod.Config.HideOverheadHealthbarWhenFull) return;

            await Task.WhenAll(
                this.DrawBarBackground(Game1.player),
                this.DrawBar(Game1.player),
                this.DrawTexture(Game1.player),
            );
            this.Mod.Monitor.Log("OverheadHealthbar drawn", LogLevel.Trace);
        }


    }
}

