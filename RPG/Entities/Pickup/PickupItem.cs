using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RPG.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RPG.Entities
{
    public enum ItemType
    {
        Gem,
        Key,
        Star
    }

    public class PickupItem
    {
        public bool IsActive { get; set; }

        public ItemType ItemType { get; set; }
        SpriteSheet spriteSheet;
        private World world;
        private int tileWidth;
        private int tileHeight;
        private float ratio = 1.393939f;

        public Vector2 Position { get; set; }

        public List<string> GetSpriteKeys()
        {
            return this.spriteKeys;
        }
        public string GetSpriteKey()
        {
            int numSprites = this.spriteKeys.Count();

            if (numSprites > 1)
            {
                int index = (DateTime.Now.Millisecond < 500) ? 0 : 1;

                return this.spriteKeys[index];
            }
            else return spriteKeys[0];
        }
        private List<string> spriteKeys;

        private Rectangle localBounds;
        /// <summary>
        /// Gets a rectangle which bounds this enemy in world space.
        /// </summary>
        public Rectangle BoundingRectangle
        {
            get
            {
                //int left = (int)Math.Round(Position.X - sprite.Origin.X) + localBounds.X;
                //int top = (int)Math.Round(Position.Y - sprite.Origin.Y) + localBounds.Y;
                int left = (int)Math.Round(Position.X - localBounds.Width / 2) + localBounds.X;
                int top = (int)Math.Round(Position.Y - localBounds.Height / 2) + localBounds.Y;
                //int left = (int)Math.Round(Position.X - 50) + localBounds.X;
                //int top = (int)Math.Round(Position.Y - 50) + localBounds.Y;

                return new Rectangle(left, top, localBounds.Width, localBounds.Height);
            }
        }

        public PickupItem(ItemType itemType, World world, List<string> spriteKeys, Vector2 location, int tileWidth, SpriteSheet spriteSheet)
        {
            this.ItemType = itemType;
            this.IsActive = true;
            this.world = world;
            this.spriteKeys = spriteKeys;

            this.Position = location;
            this.tileWidth = tileWidth;
            this.spriteSheet = spriteSheet;

            string spriteName = this.GetSpriteKey();

            var fatness = tileWidth;// this.spriteSheet.Items[spriteName].Width;
            var tallness = tileWidth;// this.spriteSheet.Items[spriteName].Height;

            this.ratio = 1.0f;// (int)(tallness / fatness);

            var height = tileHeight = tallness;// (int)(tileWidth * 1.393939);

            localBounds = new Rectangle(tileWidth / 2, (int)height / 2, fatness, tallness);
        }

        public void Draw(GameTime gameTime, GraphicsDevice device, SpriteBatch spriteBatch, Camera2D camera, bool isDebugVisuals)
        {
            if (this.IsActive)
            {
                string spriteName = this.GetSpriteKey();

                //var fatness = this.spriteSheet.Items[spriteName].Width;
                //var tallness = this.spriteSheet.Items[spriteName].Height;

                this.spriteSheet.DrawItem(spriteBatch, spriteName, this.Position, 
                    BoundingRectangle, true,
                    //new Rectangle((int)Position.X, (int)Position.Y, fatness, tallness), true,
                    //                (direction == FaceDirection.Left), 
                    Color.White);
            }

        }
    }
}
