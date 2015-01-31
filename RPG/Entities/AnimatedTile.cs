using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RPG.Entities
{
    public class AnimatedTile : ITile
    {
        public ActiveTileType ActiveTileType { get; set; }
        public TileSpriteSheet Type { get; set; }
        public TileCollision Collision { get; set; }
        public bool IsActive { get; set; }

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

        public AnimatedTile(TileSpriteSheet type, TileCollision collision, List<string> spriteKeys, ActiveTileType activeTileType, 
            ITile backgroundTile, ITile foregroundTile)
        {
            this.IsActive = true;
            this.Collision = collision;
            this.Type = type;
            this.spriteKeys = spriteKeys;

            this.ActiveTileType = activeTileType;

            this.backgroundTile = backgroundTile;
            this.foregroundTile = foregroundTile;
        }

        public AnimatedTile(TileSpriteSheet type, TileCollision collision, List<string> spriteKeys)
        {
            this.IsActive = true;

            this.Collision = collision;
            this.Type = type;
            this.spriteKeys = spriteKeys;

            this.backgroundTile = null;
            this.foregroundTile = null;
        }
    }
}
