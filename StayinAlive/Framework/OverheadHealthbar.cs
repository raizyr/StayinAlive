using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
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

        private static Texture2D Pixel => _pixelLazy.Value;
        private static Color BorderColor = new Color(255, 255, 255, 255);
        private static float CurrentHealthPercent = 1; // Current player health percentage as decimal


        internal static Texture2D Texture;
        internal static int HeightAboveHead = 8;

        internal Mod Mod;

        public OverheadHealthbar(Mod mod)
        {
            this.Mod = mod;
        }

        private static void DrawTexture(Farmer player)
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
                BorderColor);

        }

        private static void DrawBar(Farmer player) 
        {
            var _playerPos = player.getLocalPosition(Game1.viewport);
            var _healthRec = new Rectangle(
                   (int)_playerPos.X - Texture.Width * Game1.pixelZoom / 2 + player.FarmerSprite.SpriteWidth * Game1.pixelZoom / 2 + 3 * Game1.pixelZoom,
                   (int)_playerPos.Y - player.FarmerSprite.SpriteHeight * Game1.pixelZoom - (HeightAboveHead - 3) * Game1.pixelZoom,
                   (int)((Pixel.Width * Game1.pixelZoom * Texture.Width - 6 * Game1.pixelZoom) * CurrentHealthPercent),
                   Pixel.Height * Game1.pixelZoom * 6);

            // Draw current health bar
            Game1.spriteBatch.Draw(Pixel, _healthRec, new Color(226, 64, 64, 255));
        }

        private static void DrawBarBackground(Farmer player)
        {
            var _playerPos = player.getLocalPosition(Game1.viewport);
            var _healthBgRec = new Rectangle(
                   (int)_playerPos.X - Texture.Width * Game1.pixelZoom / 2 + player.FarmerSprite.SpriteWidth * Game1.pixelZoom / 2 + 3 * Game1.pixelZoom,
                   (int)_playerPos.Y - player.FarmerSprite.SpriteHeight * Game1.pixelZoom - (HeightAboveHead - 3) * Game1.pixelZoom,
                   Pixel.Width * Game1.pixelZoom * Texture.Width - 6 * Game1.pixelZoom,
                   Pixel.Height * Game1.pixelZoom * 6);

            // Draw health bar background
            Game1.spriteBatch.Draw(Pixel, _healthBgRec, new Color(0, 0, 0, 135));
        }


        private static readonly Lazy<Texture2D> _pixelLazy = new(() =>
        {
            Texture2D pixel = new Texture2D(Game1.graphics.GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });
            return pixel;
        });

        internal static void Display_RenderedWorld(object sender, RenderedWorldEventArgs e)
        {
            if (!Context.IsWorldReady) return;
            if (!Game1.showingHealth) return;

            DrawBarBackground(Game1.player);
            DrawBar(Game1.player);
            DrawTexture(Game1.player);
        }

        
    }
}

