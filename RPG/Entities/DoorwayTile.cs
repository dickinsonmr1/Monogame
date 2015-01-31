using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RPG.Entities
{
    public class DoorwayTile : ITile
    {
        public ItemType ItemType { get; set; }
        public ActiveTileType ActiveTileType { get; set; }
        public TileSpriteSheet Type { get; set; }
        public TileCollision Collision { get; set; }
        public bool IsActive { get; set; }

        public string GetSpriteKey()
        {
            return spriteKeys[0];
            //int numSprites = this.spriteKeys.Count();

            //if (!IsActive)
            //{
            //    return this.inactiveSpriteKey;
            //}
            //else
            //{
            //    if (numSprites > 1)
            //    {
            //        int index = (DateTime.Now.Millisecond < 500) ? 0 : 1;

            //        return this.spriteKeys[index];
            //    }
            //    else return spriteKeys[0];
            //}
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

        private string inactiveSpriteKey;

        public DoorwayTile(TileCollision collision,
            string inactiveSpriteKey, string spriteKey, ITile backgroundTile, ITile foregroundTile)
        {
            this.inactiveSpriteKey = inactiveSpriteKey;

            this.IsActive = true;
            this.Collision = collision;
            this.Type = TileSpriteSheet.Tile;

            this.spriteKeys = new List<string>{spriteKey};

            this.ActiveTileType = Entities.ActiveTileType.Doorway;

            this.backgroundTile = backgroundTile;
            this.foregroundTile = foregroundTile;

            this.IsActive = true;
        }
    }
}
