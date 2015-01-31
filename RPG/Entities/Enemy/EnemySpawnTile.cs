using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RPG.Entities
{
    public class EnemySpawnTile : ITile
    {
        public ActiveTileType ActiveTileType { get; set; }
        public TileSpriteSheet Type { get; set; }
        public TileCollision Collision { get; set; }
        public bool IsActive { get; set; }

        public string GetSpriteKey()
        {
            int numSprites = this.spriteKeys.Count();

            int index = (DateTime.Now.Millisecond < 500) ? 0 : 1;

            return this.spriteKeys[index];
        }

        private List<string> spriteKeys;
        public List<string> GetSpriteKeys()
        {
            return spriteKeys;
        }

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

        public EnemySpawnTile(TileCollision collision, 
            List<string> spriteKeys, ITile backgroundTile,
            ITile foregroundTile)
        {
            this.IsActive = true;
            this.Collision = collision;
            this.Type = TileSpriteSheet.Enemy;

            this.spriteKeys = spriteKeys;

            this.ActiveTileType = Entities.ActiveTileType.Static;
            this.backgroundTile = backgroundTile;
            this.foregroundTile = foregroundTile;
        }
    }
}
