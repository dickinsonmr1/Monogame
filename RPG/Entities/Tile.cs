using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RPG.Entities
{
    public enum TileSpriteSheet
    {
        Tile,
        Item,
        Enemy
    }

    /// <summary>
    /// Controls the collision detection and response behavior of a tile.
    /// </summary>
    public enum TileCollision
    {
        /// <summary>
        /// A passable tile is one which does not hinder player motion at all.
        /// </summary>
        Passable = 0,

        /// <summary>
        /// An impassable tile is one which does not allow the player to move through
        /// it at all. It is completely solid.
        /// </summary>
        Impassable = 1,

        /// <summary>
        /// A platform tile is one which behaves like a passable tile except when the
        /// player is above it. A player can jump up through a platform as well as move
        /// past it to the left and right, but can not fall down through the top of it.
        /// </summary>
        Platform = 2,
    }
    //public enum TileType
    //{        
    //    PassableTile,
    //    ImpassableTile,
    //    ImpassableFromTopOnlyTile,
    //    LadderTile,
    //    PickupItem,
        
    //}

    public class Tile : ITile
    {
        public ActiveTileType ActiveTileType { get; set; }
        public bool IsActive { get; set; }
        public TileSpriteSheet Type { get; set; }
        public TileCollision Collision { get; set; }


        public List<string> GetSpriteKeys()
        {
            return new List<string>{this.spriteKey};
        }
        public string GetSpriteKey()
        {
            return this.spriteKey;
        }
        private string spriteKey;

        public ITile GetBackgroundTile()
        {
            return this.backgroundTile;
        }
        private ITile backgroundTile;

        public ITile GetForegroundTile()
        {
            return this.foregroundTile;
        }
        private ITile foregroundTile;

        public Tile(TileSpriteSheet type, TileCollision collision, string spriteKey, ActiveTileType activeTileType)
        {
            this.IsActive = true;

            this.Collision = collision;
            this.Type = type;
            this.spriteKey = spriteKey;

            this.ActiveTileType = activeTileType;
            this.backgroundTile = null;
        }
        public Tile(TileSpriteSheet type, TileCollision collision, string spriteKey, 
            ITile backgroundTile, ITile foregroundTile, ActiveTileType activeTileType)
        {
            this.IsActive = true;

            this.Collision = collision;
            this.Type = type;
            this.spriteKey = spriteKey;

            this.backgroundTile = backgroundTile;
            this.foregroundTile = foregroundTile;
        }
    }
}
