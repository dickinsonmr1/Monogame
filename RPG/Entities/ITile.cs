using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RPG.Entities
{
    public enum ActiveTileType
    {
        Static,
        Pickup,
        Checkpoint,
        Springboard,
        Doorway
    }
    public interface ITile
    {
        ActiveTileType ActiveTileType { get; set; }
        TileSpriteSheet Type { get; set; }        
        TileCollision Collision { get; set; }

        bool IsActive { get; set; }

        List<string> GetSpriteKeys();
        string GetSpriteKey();
        ITile GetBackgroundTile();
        ITile GetForegroundTile();

    }
}
