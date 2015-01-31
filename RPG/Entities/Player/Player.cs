using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using RPG;
using RPG.Entities;
using RPG.Graphics;

namespace GameObjects
{
    public enum CharacterType
    {
        One,
        Two,
        Three
    }

    public class Player
    {
        public Vector2 Position { get; set; }
        
        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }
        Vector2 velocity;

        // Constants for controling horizontal movement
        private const float MoveAcceleration = 13000.0f;
        private const float MaxMoveSpeed = 1750.0f;
        private const float GroundDragFactor = 0.48f;
        private const float AirDragFactor = 0.58f;

        // Constants for controlling vertical movement
        private const float MaxJumpTime = 0.40f;
        private const float MaxSpringBoardJumpTime = 0.80f;
        private const float JumpLaunchVelocity = -3500.0f;
        private const float SpringboardJumpLaunchVelocity = -3500.0f;
        private const float GravityAcceleration = 3400.0f;
        //private const float GravityAcceleration = 1800.0f;
        private const float MaxFallSpeed = 550.0f;
        private const float JumpControlPower = 0.14f;

        // Input configuration
        private const float MoveStickScale = 1.0f;
        private const float AccelerometerScale = 1.5f;
        private const Buttons JumpButton = Buttons.A;

        private MouseState mouseState;
        private KeyboardState keyboardState;
        private GamePadState gamePadState;

        public float CurrentWalkFrame = 1;

        /// <summary>
        /// Current user movement input.
        /// </summary>
        private float movement;

        // Jumping state
        private bool isJumping;
        private bool wasJumping;
        private float jumpTime;

        public float GetJumpTime()
        {
            return this.jumpTime;
        }

        private bool isSpringBoardJumping;
        private bool wasSpringBoardJumping;
        private float springBoardJumpTime;

        float hurtTime;
        float maxHurtTime = 1.5f;

        public bool IsMovingRight = true;

        private Rectangle localBounds;

        public bool isOnGround { get; set; }

        private float ratio = 1.393939f;

        public int MaxHealth = 3;
        public int CurrentHealth { get; set; }
        public float GetHurtTime()
        {
            return this.hurtTime;
        }

        public int Gems { get; set; }

        public bool hasKey { get; set; }


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

        //SpriteSheet spriteSheetPlayer1;// = new SpriteSheet();
        //SpriteSheet spriteSheetPlayer2;// = new SpriteSheet();
        //SpriteSheet spriteSheetPlayer3;// = new SpriteSheet();

        //private World world;

        public CharacterType Character { get; private set; }

        public Player(CharacterType character, Vector2 location, int tileWidth, 
            SpriteSheet spriteSheetPlayer1,
            SpriteSheet spriteSheetPlayer2,
            SpriteSheet spriteSheetPlayer3)
        {
            //this.world = world;
            this.Character = character;
            switch (this.Character)
            {
                case CharacterType.One:
                    this.spriteSheet = spriteSheetPlayer1;
                    break;
                case CharacterType.Two:
                    this.spriteSheet = spriteSheetPlayer2;
                    break;
                case CharacterType.Three:
                    this.spriteSheet = spriteSheetPlayer3;
                    break;
            }
            
            this.Position = location;
            this.tileWidth = tileWidth;
            var height = tileHeight = (int)(tileWidth * ratio);

            localBounds = new Rectangle(tileWidth / 2, (int) height / 2, tileWidth, (int) height);

            this.CurrentHealth = this.MaxHealth;

            this.Gems = 0;
            this.hasKey = false;
        }
        public void Update(GameTime gameTime, World world)
        {
            //this.Position += velocity;
            this.ProcessInput(ref this.mouseState, ref this.keyboardState, ref this.gamePadState);

            this.ApplyPhysics(gameTime, world);

            if (this.isOnGround)
            {
                this.CurrentWalkFrame += 0.5f;
                if (this.CurrentWalkFrame > 11) this.CurrentWalkFrame = 1;
            }
            // Clear input.
            movement = 0.0f;
            isJumping = false;
            if (hurtTime > 0.0f)
            {
                hurtTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                //Game1.AudioManager.PlaySound("hurt");
            }
            else hurtTime = 0.0f;
            //isSpringBoardJumping = false;
        }

        public void Respawn(Vector2 location)
        {
            this.Position = location;
            this.CurrentHealth = this.MaxHealth;
        }
        public void ProcessInput(ref MouseState originalMouseState, ref KeyboardState originalKeyboardState, ref GamePadState originalGamePadState)
        {
            KeyboardState currentKeyboardState = Keyboard.GetState();

            if (currentKeyboardState.IsKeyDown(Keys.A) || currentKeyboardState.IsKeyDown(Keys.Left))
            {
                this.movement = -1.5f;
                IsMovingRight = false;
                //this.Update(new Vector2(-2.5f, 0), this.world);
            }
            if (currentKeyboardState.IsKeyDown(Keys.D) || currentKeyboardState.IsKeyDown(Keys.Right))
            {
                this.movement = 1.5f;
                IsMovingRight = true;
                //this.Update(new Vector2(2.5f, 0), this.world);
            }
            if (currentKeyboardState.IsKeyDown(Keys.W) || currentKeyboardState.IsKeyDown(Keys.Up))
            {
                //this.Update(new Vector2(0, -2.5f), this.world);
            }
            if (currentKeyboardState.IsKeyDown(Keys.S) || currentKeyboardState.IsKeyDown(Keys.Down))
            {
                //this.Update(new Vector2(0, 2.5f), this.world);
            }

            if (currentKeyboardState.IsKeyDown(Keys.E) || currentKeyboardState.IsKeyDown(Keys.F))
            {
                //this.Update(new Vector2(0, 2.5f), this.world);
            }

            isJumping = currentKeyboardState.IsKeyDown(Keys.Space)
                || currentKeyboardState.IsKeyDown(Keys.W)
                || currentKeyboardState.IsKeyDown(Keys.Up);
        }

        public void Draw(GameTime gameTime, GraphicsDevice device, SpriteBatch spriteBatch, Camera2D camera, bool isDebugVisuals)
        {
            string playerNumber = "1";
            switch (this.Character)
            {
                case CharacterType.One:
                    playerNumber = "1";
                    break;
                case CharacterType.Two:
                    playerNumber = "2";
                    break;
                case CharacterType.Three:
                    playerNumber = "3";
                    break;
            }
            string playerSpriteName = string.Empty;
            //bool facingRight = true;
            if(hurtTime > 0.0f && jumpTime > 0.0f)
            {
                playerSpriteName = string.Format("p{0}_hurt", playerNumber);
            }
            else if (!this.isOnGround)
            {
                playerSpriteName = string.Format("p{0}_jump", playerNumber);
            }
            else
            {
                if (this.Velocity.X != 0)
                {
                    playerSpriteName = string.Format("p{0}_walk{1:00}", playerNumber, this.CurrentWalkFrame);
                }
                else
                {
                    playerSpriteName = string.Format("p{0}_stand", playerNumber);
                }

            }
            var fatness = this.spriteSheet.Items[playerSpriteName].Width;
            var tallness = this.spriteSheet.Items[playerSpriteName].Height;

            var drawColor = Color.White;            
            if (isDebugVisuals || hurtTime > 0.0f)
            {
                float transparency = (DateTime.Now.Millisecond < 500) ? 0.3f : 1.0f;
                drawColor = new Color(1.0f, 1.0f, 1.0f, transparency);
            }

            this.spriteSheet.DrawItem(spriteBatch, playerSpriteName, this.Position, this.IsMovingRight, (int)(fatness / ratio), drawColor);

        }
        public void ApplyPhysics(GameTime gameTime, World world)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Vector2 previousPosition = Position;

            // Base velocity is a combination of horizontal movement control and
            // acceleration downward due to gravity.
            velocity.X += movement * MoveAcceleration * elapsed;
            velocity.Y = MathHelper.Clamp(velocity.Y + GravityAcceleration * elapsed, -MaxFallSpeed, MaxFallSpeed);

            //if (isJumping)
            {

                if (isSpringBoardJumping && springBoardJumpTime < MaxSpringBoardJumpTime)
                {
                    velocity.Y = DoSpringBoardJump(velocity.Y, gameTime);
                    
                }
                else
                {
                    isSpringBoardJumping = false; 
                    velocity.Y = DoJump(velocity.Y, gameTime);
                }
//                this.isSpringBoardJumping = false;isSpringBoardJumping
            }
            //else
            //{
            //    velocity.Y = DoJump(velocity.Y, gameTime);
            //}

            // Apply pseudo-drag horizontally.
            if (isOnGround)
                velocity.X *= GroundDragFactor;
            else
                velocity.X *= AirDragFactor;

            // Prevent the player from running faster than his top speed.            
            velocity.X = MathHelper.Clamp(Velocity.X, -MaxMoveSpeed, MaxMoveSpeed);

            // Apply velocity.
            Position += Velocity * elapsed;
            Position = new Vector2((float)Math.Round(Position.X), (float)Math.Round(Position.Y));

            // If the player is now colliding with the level, separate them.
            this.HandleCollisions(world, gameTime);

            // If the collision stopped us from moving, reset the velocity to zero.
            if (Position.X == previousPosition.X)
                velocity.X = 0;

            if (Position.Y == previousPosition.Y)
                velocity.Y = 0;

            foreach (var item in world.Items)
            {
                if (this.BoundingRectangle.Intersects(item.BoundingRectangle))
                {
                    if (item.IsActive)
                    {
                        item.IsActive = false;
                        if (item.ItemType == ItemType.Gem)
                        {
                            Game1.AudioManager.PlaySound("coin");
                            this.Gems++;
                        }
                        if (item.ItemType == ItemType.Key)
                        {
                            Game1.AudioManager.PlaySound("key");
                            this.hasKey = true;
                        }
                    }
                }
            }

            foreach (var enemy in world.Enemies)
            {
                if (this.BoundingRectangle.Intersects(enemy.BoundingRectangle))
                {
                    if (hurtTime == 0.0f)
                    {
                        hurtTime = maxHurtTime;
                        //item.IsActive = false;
                        //Game1.AudioManager.PlaySound("coin");
                        Game1.AudioManager.PlaySound("hurt");
                        this.CurrentHealth--;
                    }
                }
            }
        }

        private void HandleCollisions(World world, GameTime gameTime)
        {
            // Get the player's bounding rectangle and find neighboring tiles.
            Rectangle bounds = BoundingRectangle;
            int leftTile = (int)Math.Floor((float)bounds.Left / tileWidth);
            int rightTile = (int)Math.Ceiling(((float)bounds.Right / tileWidth)) - 1;
            int topTile = (int)Math.Floor((float)bounds.Top / tileWidth);
            int bottomTile = (int)Math.Ceiling(((float)bounds.Bottom / tileWidth)) - 1;

            // Reset flag to search for ground collision.
            isOnGround = false;

            // For each potentially colliding tile,
            for (int y = topTile; y <= bottomTile; ++y)
            {
                for (int x = leftTile; x <= rightTile; ++x)
                {
                    // If this tile is collidable,
                    TileCollision collision = world.GetCollision(x, y);
                    if (collision != TileCollision.Passable)
                    {
                        // Determine collision depth (with direction) and magnitude.
                        Rectangle tileBounds = world.GetBounds(x, y);
                        Vector2 depth = RectangleExtensions.GetIntersectionDepth(bounds, tileBounds);
                        if (depth != Vector2.Zero)
                        {
                            float absDepthX = Math.Abs(depth.X);
                            float absDepthY = Math.Abs(depth.Y);

                            // Resolve the collision along the shallow axis.
                            if (absDepthY < absDepthX || collision == TileCollision.Platform)
                            {
                                // If we crossed the top of a tile, we are on the ground.
                                if (previousBottom <= tileBounds.Top)
                                    isOnGround = true;

                                // Ignore platforms, unless we are on the ground.
                                if (collision == TileCollision.Impassable || isOnGround)
                                {
                                    // Resolve the collision along the Y axis.
                                    Position = new Vector2(Position.X, Position.Y + depth.Y);

                                    // Perform further collisions with the new bounds.
                                    bounds = BoundingRectangle;
                                }
                            }
                            else if (collision == TileCollision.Impassable) // Ignore platforms.
                            {
                                // Resolve the collision along the X axis.
                                Position = new Vector2(Position.X + depth.X, Position.Y);

                                // Perform further collisions with the new bounds.
                                bounds = BoundingRectangle;
                            }
                        }
                    }
                    else
                    {
                        //Rectangle tileBounds = world.GetBounds(x, y);
                        //Vector2 depth = RectangleExtensions.GetIntersectionDepth(bounds, tileBounds);

                        //if(Math.Abs(depth) 
                        ITile collidedTile = world.GetTile(x, y);

                        //if(Math.Abs(this.Position - collidedTile.p
                        //e = false;

                        //if (collidedTile.ActiveTileType == ActiveTileType.Pickup)
                        //{
                        //    collidedTile.IsAcve = false;
                        //}
                        if (collidedTile != null && collidedTile.ActiveTileType == ActiveTileType.Springboard)// || (this.isSpringBoardJumping && springBoardJumpTime < MaxSpringBoardJumpTime))
                        {
                            //this.DoSpringBoardJump(gameTime);
                            this.isSpringBoardJumping = true;
                            //this.velocity.Y = JumpLaunchVelocity * 100;
                        }
                        if (collidedTile != null && collidedTile.ActiveTileType == ActiveTileType.Checkpoint && !collidedTile.IsActive)// || (this.isSpringBoardJumping && springBoardJumpTime < MaxSpringBoardJumpTime))
                        {
                            collidedTile.IsActive = true;
                            Game1.AudioManager.PlaySound("checkpoint");
                            world.PlayerSpawnPoint = new Vector2(world.GetBounds(x, y).Left, world.GetBounds(x, y).Top - tileHeight);
                        }
                        if (collidedTile != null && collidedTile.ActiveTileType == ActiveTileType.Doorway)// || (this.isSpringBoardJumping && springBoardJumpTime < MaxSpringBoardJumpTime))
                        {
                            collidedTile.IsActive = true;
                            Game1.AudioManager.PlaySound("checkpoint");
                            //world.PlayerSpawnPoint = new Vector2(world.GetBounds(x, y).Left, world.GetBounds(x, y).Top - tileHeight);
                        }
                    }
                }
            }

            // Save the new bounds bottom.
            previousBottom = bounds.Bottom;
        }

        private float DoSpringBoardJump(float velocityY, GameTime gameTime)
        {
            //velocityY = 0.0f;
            // If the player wants to jump
            if (isSpringBoardJumping)
            {
                //Begin or continue a jump
                if ((!wasSpringBoardJumping) || springBoardJumpTime > 0.0f)
                //if ((!wasJumping && isOnGround))// || jumpTime > 0.0f)
                {
                    this.CurrentWalkFrame = 1;

                    if (springBoardJumpTime == 0.0f)
                    {
                        Game1.AudioManager.PlaySound("spring");
                    }

                    springBoardJumpTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    //sprite.PlayAnimation(jumpAnimation);
                }

                //// If we are in the ascent of the jump
                if (0.0f < springBoardJumpTime && springBoardJumpTime <= MaxSpringBoardJumpTime)
                {
                    // Fully override the vertical velocity with a power curve that gives players more control over the top of the jump
                    velocityY = SpringboardJumpLaunchVelocity * (1.0f - (float)Math.Pow(springBoardJumpTime / MaxSpringBoardJumpTime, JumpControlPower));
                }
                else
                {
                    // Reached the apex of the jump
                    springBoardJumpTime = 0.0f;
                    isSpringBoardJumping = false;
                }
            }
            else
            {
                // Continues not jumping or cancels a jump in progress
                springBoardJumpTime = 0.0f;
            }
            wasSpringBoardJumping = isSpringBoardJumping;

            return velocityY;
        }
        private float DoJump(float velocityY, GameTime gameTime)
        {
            //isSpringBoardJumping;
            //wasSpringBoardJumping;
            //springBoardJumpTime;

            // If the player wants to jump
            if (isJumping)
            {
                // Begin or continue a jump
                if ((!wasJumping && isOnGround) || jumpTime > 0.0f)
                    //&& (!isSpringBoardJumping || springBoardJumpTime == 0.0f ))
                {
                    this.CurrentWalkFrame = 1;

                    if (jumpTime == 0.0f)
                    {
                        Game1.AudioManager.PlaySound("jump");
                    }

                    jumpTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    //sprite.PlayAnimation(jumpAnimation);
                }

                // If we are in the ascent of the jump
                if (0.0f < jumpTime && jumpTime <= MaxJumpTime)
                {
                    // Fully override the vertical velocity with a power curve that gives players more control over the top of the jump
                    velocityY = JumpLaunchVelocity * (1.0f - (float)Math.Pow(jumpTime / MaxJumpTime, JumpControlPower));
                }
                else
                {
                    // Reached the apex of the jump
                     jumpTime = 0.0f;
                }
            }
            else
            {
                // Continues not jumping or cancels a jump in progress
                jumpTime = 0.0f;
            }
            wasJumping = isJumping;

            return velocityY;
        }
    }
}
