using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RPG.Entities
{
    public class PickupSpawnTile : ITile
    {
        public ItemType ItemType{get; set;}
        public ActiveTileType ActiveTileType { get; set; }
        public TileSpriteSheet Type { get; set; }
        public TileCollision Collision { get; set; }
        public bool IsActive { get; set; }

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

        public PickupSpawnTile(ItemType itemType, TileCollision collision,
            List<string> spriteKeys, ITile backgroundTile, ITile foregroundTitle)
        {
            this.ItemType = itemType;

            this.IsActive = true;
            this.Collision = collision;
            this.Type = TileSpriteSheet.Item;

            this.spriteKeys = spriteKeys;

            this.ActiveTileType = Entities.ActiveTileType.Pickup;

            this.backgroundTile = backgroundTile;
            this.foregroundTile = foregroundTile;
        }
    }
}
