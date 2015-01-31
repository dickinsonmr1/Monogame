using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RPG.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RPG.Graphics
{
    public enum DrawLayer
    {
        Foreground,
        Tile,
        Background,
    }
    public class SpriteSheetManager
    {
        public SpriteSheet hudSprites = new SpriteSheet();
        public SpriteSheet enemySprites = new SpriteSheet();
        public SpriteSheet itemSprites = new SpriteSheet();
        public SpriteSheet tileSprites = new SpriteSheet();
        public SpriteSheet playerSprites = new SpriteSheet();

        public Texture2D DummyTexture { get; set; }

        public SpriteSheetManager()
        {
        }

        public void LoadContent(
            SpriteSheet player, SpriteSheet hud, 
                            SpriteSheet tiles,
                            SpriteSheet items,
                            SpriteSheet enemies,
            Texture2D dummyTexture)
        {
            this.playerSprites = player;
            this.hudSprites = hud;
            this.tileSprites = tiles;
            this.itemSprites = items;
            this.enemySprites = enemies;
            this.DummyTexture = dummyTexture;
        }

        public void Draw(SpriteBatch spriteBatch, ITile tile, Vector2 location,
            bool facingRight, int tileSize, Color color, string tileName)
        {
            this.tileSprites.DrawItem(spriteBatch, tileName, location, true, tileSize, color);
        }
    }
}
