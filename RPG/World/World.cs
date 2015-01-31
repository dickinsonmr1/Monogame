using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using RPG.Entities;
using Microsoft.Xna.Framework;
using RPG.Graphics;
using Microsoft.Xna.Framework.Graphics;
//using RPG.WorldTheme;

namespace RPG
{
    public class World
    {
        private ITile[,] tiles;

        public ITile[,] GetTiles()
        {
            return this.tiles;
        }


        public ITile GetTile(int x, int y)
        {
            if (x < this.Width && x >= 0 && y < this.Height && y >= 0)
            {
                return this.tiles[x, y];
            }
            else return null;
        }

        public ITile GetBackgroundTile(int x, int y)
        {
            return this.tiles[x, y].GetBackgroundTile();
        }

        public ITile GetForegroundTile(int x, int y)
        {
            return this.tiles[x, y].GetForegroundTile();
        }

        public List<Enemy> Enemies { get; set; }
        public List<PickupItem> Items { get; set; }
        //public List<Item> Items { get; set; }
        public Vector2 PlayerSpawnPoint { get; set; }
        public List<Vector2> Doorways { get; set; }

        public TileCollision GetCollision(int x, int y)
        {
            // Prevent escaping past the level ends.
            if (x < 0 || x >= Width)
                return TileCollision.Impassable;
            // Allow jumping past the level top and falling through the bottom.
            if (y < 0 || y >= Height)
                return TileCollision.Passable;

            return tiles[x, y].Collision;
        }

        /// <summary>
        /// Gets the bounding rectangle of a tile in world space.
        /// </summary>        
        public Rectangle GetBounds(int x, int y)
        {
            //return new Rectangle(x * Tile.Width, y * Tile.Height, Tile.Width, Tile.Height);
            return new Rectangle(x * tileSize, y * tileSize, tileSize, tileSize);
        }


        /// <summary>
        /// Width of level measured in tiles.
        /// </summary>
        public int Width
        {
            get { return tiles.GetLength(0); }
        }

        /// <summary>
        /// Height of level measured in tiles.
        /// </summary>
        public int Height
        {
            get { return tiles.GetLength(1); }
        }

        private int tileSize = 0;
        private SpriteSheetManager spriteSheetManager;
        public WorldTheme WorldTheme { get; set; }

        public World(int tileSize, SpriteSheetManager spriteSheetManager, WorldTheme WorldTheme)
        {
            this.tileSize = tileSize;
            this.spriteSheetManager = spriteSheetManager;
            this.WorldTheme = WorldTheme;

            tiles = null;

            //this.dummyTexture = new Texture2D(device, 1, 1);           
        }

        public void Load(Stream fileStream)
        {
            // Load the level and ensure all of the lines are the same length.
            int width;
            List<string> lines = new List<string>();
            using (StreamReader reader = new StreamReader(fileStream))
            {
                string line = reader.ReadLine();
                width = line.Length;
                while (line != null)
                {
                    lines.Add(line);
                    if (line.Length != width)
                        throw new Exception(String.Format("The length of line {0} is different from all preceding lines.", lines.Count));
                    line = reader.ReadLine();
                }
            }

            // Allocate the tile grid.
            tiles = new ITile[width, lines.Count];
            this.Enemies = new List<Enemy>();
            this.Items = new List<PickupItem>();
            this.Doorways = new List<Vector2>();

            // Loop over every tile position,
            for (int y = 0; y < Height; ++y)
            {
                for (int x = 0; x < Width; ++x)
                {
                    // to load each tile.
                    char tileType = lines[y][x];
                    tiles[x, y] = LoadTile(tileType, x, y);
                    if (tiles[x, y].GetType() == typeof(EnemySpawnTile))
                    {
                        bool isAffectedByGravity = true;
                        if (tiles[x, y].GetSpriteKey().Contains("fish") || tiles[x, y].GetSpriteKey().Contains("fly"))
                        {
                            isAffectedByGravity = false;
                        }
                        this.Enemies.Add(new Enemy(this, tiles[x,y].GetSpriteKeys(), GameObjects.CharacterType.One,
                            new Vector2(tileSize * x, tileSize * y), tileSize, this.spriteSheetManager.enemySprites, isAffectedByGravity));
                    }
                    if (tiles[x, y].GetType() == typeof(PickupSpawnTile))
                    {
                        var pickupType = ItemType.Gem;
                        if(tiles[x, y].GetSpriteKey().ToLower().Contains("key"))
                        {
                            pickupType = ItemType.Key;
                        }
                        this.Items.Add(new PickupItem(pickupType, this, tiles[x, y].GetSpriteKeys(), new Vector2(tileSize * x, tileSize * y), tileSize, this.spriteSheetManager.itemSprites));
                    }
                    if (tiles[x, y].GetType() == typeof(CheckpointTile))
                    {
                    }
                    if (tiles[x, y].GetType() == typeof(DoorwayTile))
                    {

                    }
                    if (tiles[x, y].GetType() == typeof(PlayerSpawnTile))
                    {
                        PlayerSpawnPoint = new Vector2(tileSize * x, tileSize * y);//, tileSize, this.spriteSheetManager.itemSprites);
                    }
                }
            }
        }

        private ITile LoadTile(char tileType, int x, int y)
        {
            switch (tileType)
            {
                // Blank space
                case '.':
                    return new Tile(TileSpriteSheet.Item, TileCollision.Passable, string.Empty, ActiveTileType.Static);
                    //return "stoneHalf";

                // box
                case 'B':
                    return new Tile(TileSpriteSheet.Tile, TileCollision.Impassable, "box", ActiveTileType.Static);

                // gem
                case '+':
                    return new PlayerSpawnTile(TileCollision.Passable, new List<string> { "fireball" }, null, null);

                // gem
                case 'G':
                    return new PickupSpawnTile(ItemType.Gem, TileCollision.Passable, new List<string> { this.WorldTheme.Gem1Tile }, null, null);
                // gem
                case 'g':
                    return new PickupSpawnTile(ItemType.Gem, TileCollision.Passable, new List<string> { this.WorldTheme.Gem2Tile }, null, null);

                // star
                case '*':
                    return new PickupSpawnTile(ItemType.Star, TileCollision.Passable, new List<string> { "star" }, null, null);
                    //return new Tile(TileSpriteSheet.Item, TileCollision.Passable, "star", ActiveTileType.Pickup);

                // star
                case 'k':
                    return new PickupSpawnTile(ItemType.Key, TileCollision.Passable, new List<string> { "keyBlue" }, null, null);
                //return new Tile(TileSpriteSheet.Item, TileCollision.Passable, "star", ActiveTileType.Pickup);
                // star
                case 'L':
                    return new Tile(TileSpriteSheet.Tile, TileCollision.Impassable, "lock_blue", ActiveTileType.Static);
                //return new Tile(TileSpriteSheet.Item, TileCollision.Passable, "star", ActiveTileType.Pickup);

                case '&':

                    Random rand = new Random();
                    int cloudNum = rand.Next() % 3 + 1;

                    return new Tile(TileSpriteSheet.Item, TileCollision.Passable, string.Empty,
                        null,
                        new Tile(TileSpriteSheet.Item, TileCollision.Passable, string.Format("cloud{0}", cloudNum), ActiveTileType.Static),
                        ActiveTileType.Static);

                // springboard
                case 'z':
                    return new AnimatedTile(TileSpriteSheet.Item, TileCollision.Passable, new List<string> { "springboardDown", "springboardUp" }, ActiveTileType.Springboard, null, null);

                // springboard 
                case 'Z':
                    return new AnimatedTile(TileSpriteSheet.Item, TileCollision.Passable, new List<string> { "springboardDown", "springboardUp" }, ActiveTileType.Springboard, 
                        new Tile(TileSpriteSheet.Tile, TileCollision.Passable, WorldTheme.InsideBackgroundTile, ActiveTileType.Static), null);


                //case 'c':
                //    return new Tile(TileSpriteSheet.Item, TileCollision.Passable, "cactus", ActiveTileType.Static);
                //case 'm':
                //    return new Tile(TileSpriteSheet.Item, TileCollision.Passable, "mushroomRed", ActiveTileType.Static);
                case 'p':
                    return new Tile(TileSpriteSheet.Item, TileCollision.Passable, WorldTheme.Plant1Tile, ActiveTileType.Static);
                case 'P':
                    return new Tile(TileSpriteSheet.Item, TileCollision.Passable, WorldTheme.Plant2Tile, ActiveTileType.Static);

                case 'b':
                    return new Tile(TileSpriteSheet.Item, TileCollision.Passable, "bush", ActiveTileType.Static);

                case 'r':
                    return new Tile(TileSpriteSheet.Item, TileCollision.Passable, "rock", ActiveTileType.Static);


                case 'h':
                    return new Tile(TileSpriteSheet.Tile, TileCollision.Passable, string.Empty,
                        new Tile(TileSpriteSheet.Tile, TileCollision.Passable, "hill_smallAlt", ActiveTileType.Static),
                        null,
                        ActiveTileType.Static);

                case 'H':
                    return new Tile(TileSpriteSheet.Tile, TileCollision.Passable, string.Empty,
                        new Tile(TileSpriteSheet.Tile, TileCollision.Passable, "hill_largeAlt", ActiveTileType.Static),
                        null,
                        ActiveTileType.Static);


                case '@':
                    return new Tile(TileSpriteSheet.Item, TileCollision.Passable, "bomb", ActiveTileType.Static);

                case 'f':
                    return new CheckpointTile(TileCollision.Passable, 
                        "flagBlueHanging", new List<string> {"flagBlue", "flagBlue2" }, null, null);

                case '?':
                    return new AnimatedTile(TileSpriteSheet.Item, TileCollision.Passable, new List<string> { "switchLeft", "switchMid", "switchRight" }, ActiveTileType.Static, null, null);

                case '|':
                    return new Tile(TileSpriteSheet.Item, TileCollision.Passable, "chain", ActiveTileType.Static);
                case 'W':
                    return new Tile(TileSpriteSheet.Item, TileCollision.Impassable, "weightChained", ActiveTileType.Static);


                case '!':
                    return new AnimatedTile(TileSpriteSheet.Tile, TileCollision.Passable, new List<string> { "tochLit", "tochLit2" }, ActiveTileType.Static,
                        new Tile(TileSpriteSheet.Tile, TileCollision.Passable, WorldTheme.InsideBackgroundTile, ActiveTileType.Static), null);
                case 'd':
                    return new Tile(TileSpriteSheet.Tile, TileCollision.Passable, "door_closedTop",
                        new Tile(TileSpriteSheet.Tile, TileCollision.Passable, WorldTheme.InsideBackgroundTile, ActiveTileType.Static),
                        null, 
                        ActiveTileType.Static);
                case 'D':
                    return new Tile(TileSpriteSheet.Tile, TileCollision.Passable, "door_closedMid",
                        new Tile(TileSpriteSheet.Tile, TileCollision.Passable, WorldTheme.InsideBackgroundTile, ActiveTileType.Static),
                        null, 
                        ActiveTileType.Static);

                case 'o':
                    return new Tile(TileSpriteSheet.Tile, TileCollision.Passable, "door_openTop",
                        new Tile(TileSpriteSheet.Tile, TileCollision.Passable, WorldTheme.InsideBackgroundTile, ActiveTileType.Static),
                        null, 
                        ActiveTileType.Static);
                case 'O':
                    return new DoorwayTile(TileCollision.Passable,
                        "door_openMid", "door_openMid", null, null);
                    //return new Tile(TileSpriteSheet.Tile, TileCollision.Passable, "door_openMid",
                        //new Tile(TileSpriteSheet.Tile, TileCollision.Passable, WorldTheme.InsideBackgroundTile, ActiveTileType.Static),
                        //null, 
                        //ActiveTileType.Static);

                case 'X':
                    return new Tile(TileSpriteSheet.Tile, TileCollision.Passable, "signExit", ActiveTileType.Static);


                // slime
                case '1':
                    return new EnemySpawnTile(TileCollision.Passable, new List<string> { "slimeWalk1", "slimeWalk2" }, null, null);

                // poker
                case '2':
                    return new EnemySpawnTile(TileCollision.Passable, new List<string> { "pokerMad", "pokerSad" }, null, null);

                // blocker
                case '3':
                    return new EnemySpawnTile(TileCollision.Passable, new List<string> { "blockerMad", "blockerSad" }, null, null);

                // fish
                case '4':
                    return new EnemySpawnTile(TileCollision.Passable, new List<string> { "fishSwim1", "fishSwim2" },
                        null,
                        new Tile(TileSpriteSheet.Tile, TileCollision.Passable, "liquidWater", ActiveTileType.Static));

                // fly
                case '5':
                    return new EnemySpawnTile(TileCollision.Passable, new List<string> { "flyFly1", "flyFly2" }, null, null);

                // snail
                case '6':
                    return new EnemySpawnTile(TileCollision.Passable, new List<string> { "snailWalk1", "snailWalk2" }, null, null);

                // passable castle background
                case ',':
                    return new Tile(TileSpriteSheet.Tile, TileCollision.Passable, WorldTheme.InsideBackgroundTile, ActiveTileType.Static);

                // Impassable block
                case '<':
                    return new Tile(TileSpriteSheet.Tile, TileCollision.Platform, WorldTheme.GroundTileLeft, ActiveTileType.Static);

                // cliff left
                case '{':
                    return new Tile(TileSpriteSheet.Tile, TileCollision.Platform, WorldTheme.GroundTileCliffLeft, ActiveTileType.Static);

                // Impassable block
                case '"':
                    return new Tile(TileSpriteSheet.Tile, TileCollision.Platform, WorldTheme.GroundTileMid, ActiveTileType.Static);

                // Impassable block
                case '#':
                    return new Tile(TileSpriteSheet.Tile, TileCollision.Impassable, WorldTheme.GroundTileCenter, ActiveTileType.Static);

                // Impassable block
                case '>':
                    return new Tile(TileSpriteSheet.Tile, TileCollision.Platform, WorldTheme.GroundTileRight, ActiveTileType.Static);

                    
                // cliff right
                case '}':
                    return new Tile(TileSpriteSheet.Tile, TileCollision.Platform, WorldTheme.GroundTileCliffRight, ActiveTileType.Static);

                // Platform block (can jump from below)
                case '=':
                    return new Tile(TileSpriteSheet.Tile, TileCollision.Platform, WorldTheme.GroundTileHalfMid, ActiveTileType.Static);

                // hill left
                case '/':
                    return new Tile(TileSpriteSheet.Tile, TileCollision.Platform, WorldTheme.GroundTileHillLeft, ActiveTileType.Static);
                // hill right
                case '\\':
                    return new Tile(TileSpriteSheet.Tile, TileCollision.Platform, WorldTheme.GroundTileHillRight, ActiveTileType.Static);

                // hill left 2
                case '[':
                    return new Tile(TileSpriteSheet.Tile, TileCollision.Platform, WorldTheme.GroundTileHillLeft2, ActiveTileType.Static);
                // hill right 2
                case ']':
                    return new Tile(TileSpriteSheet.Tile, TileCollision.Platform, WorldTheme.GroundTileHillRight2, ActiveTileType.Static);


                // water top
                case '~':
                    return new Tile(TileSpriteSheet.Tile, TileCollision.Passable, string.Empty, 
                        null,
                        new Tile(TileSpriteSheet.Tile, TileCollision.Passable, "liquidWaterTop_mid", ActiveTileType.Static),
                        ActiveTileType.Static);

                // water
                case '-':
                    return new Tile(TileSpriteSheet.Tile, TileCollision.Passable, string.Empty, 
                        null,
                        new Tile(TileSpriteSheet.Tile, TileCollision.Passable, "liquidWater", ActiveTileType.Static),
                        ActiveTileType.Static);

                case '^':
                    return new Tile(TileSpriteSheet.Item, TileCollision.Impassable, "spikes",
                        null,
                        new Tile(TileSpriteSheet.Tile, TileCollision.Passable, "liquidWater", ActiveTileType.Static),
                        ActiveTileType.Static);


                // lava top
                case '%':
                    return new Tile(TileSpriteSheet.Tile, TileCollision.Passable, string.Empty,
                   null,
                   new Tile(TileSpriteSheet.Tile, TileCollision.Passable, "liquidLavaTop_mid", ActiveTileType.Static),
                    ActiveTileType.Static);
                    //return new Tile(TileSpriteSheet.Tile, TileCollision.Passable, "liquidLavaTop_mid", ActiveTileType.Static);

                // lava
                case '_':
                    return new Tile(TileSpriteSheet.Tile, TileCollision.Passable, string.Empty,
                    null,
                    new Tile(TileSpriteSheet.Tile, TileCollision.Passable, "liquidLava", ActiveTileType.Static),
                     ActiveTileType.Static);

                // Unknown tile type character
                default:
                    throw new NotSupportedException(String.Format("Unsupported tile type character '{0}' at position {1}, {2}.", tileType, x, y));
            }
        }

        public void Draw(GameTime gameTime, GraphicsDevice device, SpriteBatch spriteBatch, Camera2D camera, int TileSize, bool isDebugVisuals)
        {

            //ITile[,] worldTiles = this.GetTiles();
            this.DrawBackgroundPass(gameTime, device, spriteBatch, camera, tileSize, isDebugVisuals);
                       
            //this.Window.Title = string.Format("Platformer: Player ({0:0.00}, {1:0.00}) CameraZoom({2:0.00}x)", player.Position.X, player.Position.Y, camera.Zoom);
            //spriteBatch.Draw(picture, new Rectangle(0, 0, this.GraphicsDevice.Viewport.Width, this.GraphicsDevice.Viewport.Height), Color.White);
            //this.tiles.DrawItem(spriteBatch, "boxAlt", player1.Location, tileSize);
            if (isDebugVisuals)
            {
                //spriteBatch.Draw(picture, new Rectangle(player.BoundingRectangle.X, player.BoundingRectangle.Y, player.BoundingRectangle.Width, player.BoundingRectangle.Height), Color.Purple);
                //spriteBatch.Draw(player1Picture, new Rectangle(player1.BoundingRectangle.X, player1.BoundingRectangle.Y, player1.BoundingRectangle.Width, player1.BoundingRectangle.Height), Color.White);

                foreach (var enemy in this.Enemies)
                {
                    spriteBatch.Draw(this.spriteSheetManager.DummyTexture, new Rectangle(enemy.BoundingRectangle.X, enemy.BoundingRectangle.Y, enemy.BoundingRectangle.Width, enemy.BoundingRectangle.Height), Color.Purple);

                    //enemy.Draw(gameTime, this.GraphicsDevice, spriteBatch, camera, Color.Purple);
                }

                foreach (var item in this.Items)
                {
                    spriteBatch.Draw(this.spriteSheetManager.DummyTexture, new Rectangle(item.BoundingRectangle.X, item.BoundingRectangle.Y, item.BoundingRectangle.Width, item.BoundingRectangle.Height), Color.Purple);

                    //enemy.Draw(gameTime, this.GraphicsDevice, spriteBatch, camera, Color.Purple);
                }
            }

            foreach (var enemy in this.Enemies)
            {
                enemy.Draw(gameTime, device, spriteBatch, camera, isDebugVisuals);
            }

            this.DrawMainPass(gameTime, device, spriteBatch, camera, tileSize, isDebugVisuals);
          
            foreach (var item in this.Items)
            {
                item.Draw(gameTime, device, spriteBatch, camera, isDebugVisuals);
            }

            this.DrawForegroundPass(gameTime, device, spriteBatch, camera, tileSize, isDebugVisuals);
        }

        public void DrawForegroundPass(GameTime gameTime, GraphicsDevice device, SpriteBatch spriteBatch, Camera2D camera, int TileSize, bool isDebugVisuals)
        {
            //for (int i = 0; i < this.Width; i++)
            //{
            //    for (int j = 0; j < this.Height; j++)
            //    {
            //        Rectangle rect = new Rectangle((int)camera.Position.X - tileSize * 20, (int)camera.Position.Y - tileSize * 20, tileSize * 40, tileSize * 40);

            //        if (rect.Intersects(new Rectangle(TileSize * i, TileSize * j, tileSize, tileSize)))
            //        {
            //            var tile = this.GetForegroundTile(i, j);

            //            if (tile != null)
            //            {
            //                //Color color = new Color(1.0f, 1.0f, 1.0f, 0.3f);
            //                Color color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            //                if (isDebugVisuals)
            //                {
            //                    switch (tile.Collision)
            //                    {
            //                        case TileCollision.Impassable:
            //                            color = Color.Red;
            //                            break;
            //                        case TileCollision.Passable:
            //                            color = Color.Green;
            //                            break;
            //                        case TileCollision.Platform:
            //                            color = Color.Yellow;
            //                            break;
            //                    }
            //                }
            //                this.spriteSheetManager.Draw(spriteBatch, tile, new Vector2(TileSize * i, TileSize * j), true, TileSize, color);
            //            }
            //        }
            //    }
            //}
        }

        public void DrawMainPass(GameTime gameTime, GraphicsDevice device, SpriteBatch spriteBatch, Camera2D camera, int TileSize, bool isDebugVisuals)
        {
            //for (int i = 0; i < this.Width; i++)
            //{
            //    for (int j = 0; j < this.Height; j++)
            //    {
            //        Rectangle rect = new Rectangle((int)camera.Position.X - tileSize * 20, (int)camera.Position.Y - tileSize * 20, tileSize * 40, tileSize * 40);

            //        if (rect.Intersects(new Rectangle(TileSize * i, TileSize * j, tileSize, tileSize)))
            //        {

            //            var tile = this.GetTile(i, j);

            //            if (tile != null)
            //            {
            //                //Color color = new Color(1.0f, 1.0f, 1.0f, 0.3f);
            //                Color color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            //                if (isDebugVisuals)
            //                {
            //                    switch (tile.Collision)
            //                    {
            //                        case TileCollision.Impassable:
            //                            color = Color.Red;
            //                            break;
            //                        case TileCollision.Passable:
            //                            color = Color.Green;
            //                            break;
            //                        case TileCollision.Platform:
            //                            color = Color.Yellow;
            //                            break;
            //                    }
            //                }
            //                var offset = (i % 2)*Width;
            //                this.spriteSheetManager.Draw(spriteBatch, tile, new Vector2(TileSize * i + offset, TileSize * j), true, TileSize, color);
            //            }
            //        }
            //    }
            //}
        }

        public void DrawBackgroundPass(GameTime gameTime, GraphicsDevice device, SpriteBatch spriteBatch, Camera2D camera, int TileSize, bool isDebugVisuals)
        {
            //for (int i = 0; i < this.Width; i++)
            //{
            //    for (int j = 0; j < this.Height; j++)
            //    {
            //        Rectangle rect = new Rectangle((int)camera.Position.X - tileSize * 20, (int)camera.Position.Y - tileSize * 20, tileSize * 40, tileSize * 40);

            //        if(rect.Intersects(new Rectangle(TileSize * i, TileSize * j, tileSize, tileSize)))
            //        {
            //            var tile = this.GetBackgroundTile(i, j);
            //            if (tile != null)
            //            {
            //                Color color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            //                this.spriteSheetManager.Draw(spriteBatch, tile, new Vector2(TileSize * i, TileSize * j), true, TileSize, color);
            //            }
            //        }
            //    }
            //}
        }        
    }
}
