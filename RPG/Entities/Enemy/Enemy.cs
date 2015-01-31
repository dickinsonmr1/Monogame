using GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RPG.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RPG.Entities
{
    public class Enemy
    {
        /// <summary>
        /// Facing direction along the X axis.
        /// </summary>
        enum FaceDirection
        {
            Left = -1,
            Right = 1,
        }

        public Vector2 Position { get; set; }

        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }
        Vector2 velocity;

        /// <summary>
        /// The direction this enemy is facing and moving along the X axis.
        /// </summary>
        private FaceDirection direction = FaceDirection.Left;

        /// <summary>
        /// How long this enemy has been waiting before turning around.
        /// </summary>
        private float waitTime;
        /// <summary>
        /// How long to wait before turning around.
        /// </summary>
        private const float MaxWaitTime = 0.5f;

        /// <summary>
        /// The speed at which this enemy moves along the X axis.
        /// </summary>
        private const float MoveSpeed = 64.0f;

        private float ratio = 1.393939f;

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
        
        private int tileWidth;
        private int tileHeight;

        private float previousBottom;

        SpriteSheet spriteSheet;// = new SpriteSheet();

        public CharacterType Character { get; private set; }

        private World world;
        //private string spriteName;

        public List<string> GetSpriteKeys()
        {
            return this.spriteKeys;
        }
        public string GetSpriteKey()
        {
            int numSprites = this.spriteKeys.Count();

            int index = (DateTime.Now.Millisecond < 500) ? 0 : 1;

            return this.spriteKeys[index];
        }
        private List<string> spriteKeys;


        bool isAffectedByGravity;

        public Enemy(World world, List<string> spriteKeys, CharacterType character,
            Vector2 location, int tileWidth, SpriteSheet spriteSheet, bool isAffectedByGravity)
        {
            this.world = world;
            this.spriteKeys = spriteKeys;

            //this.spriteName = spriteName;
            this.Character = character;
            this.Position = location;
            this.tileWidth = tileWidth;
            this.spriteSheet = spriteSheet;

            string spriteName = this.GetSpriteKey();

            var fatness = tileWidth;// this.spriteSheet.Items[spriteName].Width;
            var tallness = tileWidth;// this.spriteSheet.Items[spriteName].Height;
            //var fatness = this.spriteSheet.Items[spriteName].Width;
            //var tallness = this.spriteSheet.Items[spriteName].Height;
            this.isAffectedByGravity = isAffectedByGravity;

            this.ratio = 1.2f;// (int)(tallness / fatness);

            var height = tileHeight = tallness;// (int)(tileWidth * 1.393939);

            localBounds = new Rectangle(tileWidth / 2, (int)height / 2, fatness, tallness);
            //localBounds = new Rectangle(tileWidth / 2, (int)height / 2 + this.spriteSheet.Items[spriteName].Height, fatness, tallness);
        }

        /// <summary>
        /// Paces back and forth along a platform, waiting at either end.
        /// </summary>
        public void Update(GameTime gameTime, World world)
        {

            //this.ApplyPhysics(gameTime, world);

            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Calculate tile position based on the side we are walking towards.
            float posX = Position.X + localBounds.Width / 2 * (int)direction;
            float posY = Position.Y + localBounds.Width / 2;// *(int)direction;
            int tileX = (int)Math.Floor(posX / tileWidth) - (int)direction;
            int tileY = (int)Math.Floor(posY / tileHeight);

            if (waitTime > 0)
            {
                // Wait for some amount of time.
                waitTime = Math.Max(0.0f, waitTime - (float)gameTime.ElapsedGameTime.TotalSeconds);
                if (waitTime <= 0.0f)
                {
                    // Then turn around.
                    direction = (FaceDirection)(-(int)direction);
                }
            }
            else
            {
                // If we are about to run into a wall or off a cliff, start waiting.
                if ((this.isAffectedByGravity && world.GetCollision(tileX + (int)direction, tileY + 1) == TileCollision.Passable) ||
                    world.GetCollision(tileX + (int)direction, tileY) == TileCollision.Impassable)
                {
                    waitTime = MaxWaitTime;
                }
                else
                {
                    // Move in the current direction.
                    Vector2 velocity = new Vector2((int)direction * MoveSpeed * elapsed, 0.0f);
                    Position = Position + velocity;
                }
            }
        }

        //public void ApplyPhysics(GameTime gameTime, World world)
        //{
        //    this.HandleCollisions(world, gameTime);
        //}

        //private void HandleCollisions(World world, GameTime gameTime)
        //{
        //     // Get the player's bounding rectangle and find neighboring tiles.
        //    Rectangle bounds = BoundingRectangle;
        //    int leftTile = (int)Math.Floor((float)bounds.Left / tileWidth);
        //    int rightTile = (int)Math.Ceiling(((float)bounds.Right / tileWidth)) - 1;
        //    int topTile = (int)Math.Floor((float)bounds.Top / tileWidth);
        //    int bottomTile = (int)Math.Ceiling(((float)bounds.Bottom / tileWidth)) - 1;

        //    // Reset flag to search for ground collision.
        //    bool isOnGround = false;

        //    // For each potentially colliding tile,
        //    for (int y = topTile; y <= bottomTile; ++y)
        //    {
        //        for (int x = leftTile; x <= rightTile; ++x)
        //        {
        //        }
        //    }
        //}

        public void Draw(GameTime gameTime, GraphicsDevice device, SpriteBatch spriteBatch, Camera2D camera, bool isDebugVisuals)
        {
            string spriteName = this.GetSpriteKey();

            var fatness = this.spriteSheet.Items[spriteName].Width;
            var tallness = this.spriteSheet.Items[spriteName].Height;


            float posX = Position.X + localBounds.Width / 2 * (int)direction;
            float posY = Position.Y + localBounds.Width / 2;// *(int)direction;
            int tileX = (int)Math.Floor(posX / tileWidth) - (int)direction;
            int tileY = (int)Math.Floor(posY / tileHeight);
            //ITile tile1 = world.GetTile((int)Position.X + (int)direction, (int)Position.Y - 1);
            //ITile tile2 = world.GetTile((int)Position.X + (int)direction, (int)Position.Y);
            //world.GetCollision(tileX + (int)direction, tileY - 1) == TileCollision.Impassable ||
            //world.GetCollision(tileX + (int)direction, tileY)

            this.spriteSheet.DrawItem(spriteBatch, spriteName, this.Position, new Rectangle((int)Position.X, (int)Position.Y + tallness, fatness, tallness), 
                //true,
                (direction == FaceDirection.Left), 
                Color.White);

            if (isDebugVisuals)
            {
                this.spriteSheet.DrawItem(spriteBatch, spriteName, new Vector2((tileX  + (int)direction) * tileWidth, (tileY + 1) * tileWidth), true, tileWidth, Color.Black);
                this.spriteSheet.DrawItem(spriteBatch, spriteName, new Vector2((tileX + (int)direction) * tileWidth + (int)direction, tileY * tileWidth), true, tileWidth, Color.Black);
            }

        }
    }
}
