#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using System.Xml;
using System.Xml.Linq;
using GameObjects;
using RPG.Graphics;
using System.IO;
using RPG.Entities;
using Microsoft.Xna.Framework.Audio;
#endregion

namespace RPG
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    /// 
    //

    /*
     * todo
     * camera
     * level editor
     * layers
     * parallax
     * items spritesheet
     * player/enemy animations
     * player movement
     * */
    public class Game1 : Game
    {


        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D picture;

        Texture2D light;
        Texture2D healthBar;
        Texture2D healthBarOutline;
        Texture2D shield;

        Player player = null;//new Player(Vector2.Zero, 1);

        //SpriteSheet hudItems = new SpriteSheet();
        //SpriteSheet enemies = new SpriteSheet();
        //SpriteSheet items = new SpriteSheet();
        //SpriteSheet tiles = new SpriteSheet();
        //SpriteSheet playerSheet = new SpriteSheet();

        SpriteSheetManager spriteSheetManager = new SpriteSheetManager();

        World world;

        bool isInitialized = false;

        bool isDebugVisuals = false;
        bool drawShield = false;

        public int TileSize = 48;

        Camera2D camera = new Camera2D();

        private MouseState mouseState;
        private KeyboardState keyboardState;
        private GamePadState gamePadState;
        List<Layer> layers;

        Sound sound;

        public static AudioManager AudioManager = new AudioManager();

        public Game1()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = 960;
            graphics.PreferredBackBufferHeight = 640;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            var tiles = new SpriteSheet();
            var playerSheet1 = new SpriteSheet();
            var playerSheet2 = new SpriteSheet();
            var playerSheet3 = new SpriteSheet();
            var playerSheet4 = new SpriteSheet();

            tiles.LoadContentFromXml(@"Content\Artwork\hexagon\complete.xml",
                Content.Load<Texture2D>(@"Artwork\hexagon\complete.png"));

            spriteSheetManager.tileSprites = tiles;

            isInitialized = true;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            //this.ProcessInput(ref this.mouseState, ref this.keyboardState, ref this.gamePadState);

            //this.player.ProcessInput(ref this.mouseState, ref this.keyboardState, ref this.gamePadState);
            
            //this.player.Update(gameTime, this.world);

            //foreach (var enemy in world.Enemies)
            //{
            //    enemy.Update(gameTime, this.world);
            //}

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            if (isInitialized)
            {
                GraphicsDevice.Clear(Color.AliceBlue);

                //var spriteBatch = new SpriteBatch(this.GraphicsDevice);

                //camera.Pos = this.player.Position;// -new Vector2(0, this.player.GetJumpTime() * 100);// new Vector2(200.0f, 200.0f);

                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

                int Width = 10;
                int Height = 10;
                
                int tileSize = 32;

                int tileWidth = 55; //65
                int tileHeight = 57; //89

                for (int j = 0; j < Height; j++)
                {

                    for (int i = 0; i < Width; i++)
                    {
                        //var rect = new Rectangle( tileSize * 20, tileSize * 20, 65, 89);

                        Color color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                        var offset = (j % 2) * tileSize;
                        //var offsetY = (j % 2) * 40;
                        string tileName = j % 2 == 0 ? "tileGrass" : "tileWater";
                        this.spriteSheetManager.Draw(spriteBatch, null, new Vector2(tileWidth * i + offset, tileHeight * j), true, TileSize, color, tileName);
                    }
                }
                spriteBatch.End();

                // TODO: Add your drawing code here

                base.Draw(gameTime);
            }
        }

        public void ProcessInput(ref MouseState originalMouseState, ref KeyboardState originalKeyboardState, ref GamePadState originalGamePadState)
        {
            KeyboardState currentKeyboardState = Keyboard.GetState();

            if (currentKeyboardState.IsKeyDown(Keys.Q))
            {
                camera.Zoom += 0.1f;
            }

            if (currentKeyboardState.IsKeyDown(Keys.Z))
            {
                camera.Zoom -= 0.1f;
            }


            if (currentKeyboardState.IsKeyDown(Keys.F2) && originalKeyboardState.IsKeyUp(Keys.F2))
            {
                this.isDebugVisuals = !this.isDebugVisuals;
            }

            if (currentKeyboardState.IsKeyDown(Keys.F3) && originalKeyboardState.IsKeyUp(Keys.F3))
            {
                this.drawShield = !this.drawShield;
                //this.sound.Play();
            }

            if (currentKeyboardState.IsKeyDown(Keys.F9) && originalKeyboardState.IsKeyUp(Keys.F9))
            {
                this.player.Respawn(this.world.PlayerSpawnPoint);
            }

            originalKeyboardState = currentKeyboardState;

        }
    }
}
