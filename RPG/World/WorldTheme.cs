using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RPG
{
 

    public class WorldTheme
    {
        public string GroundTileHalfMid { get; set; }       //     =
        public string GroundTileMid { get; set; }           //     "
        public string GroundTileLeft { get; set; }          //     <
        public string GroundTileRight { get; set; }         //     >
        public string GroundTileCenter { get; set; }        //     #
        public string GroundTileCliffLeft { get; set; }     //     {
        public string GroundTileCliffRight { get; set; }    //     }
        public string GroundTileHillLeft { get; set; }      //     /
        public string GroundTileHillRight { get; set; }     //     \
        public string GroundTileHillLeft2 { get; set; }     //     [
        public string GroundTileHillRight2 { get; set; }    //     ]

        public string AirBackgroundTile { get; set; }       //     .
        public string InsideBackgroundTile { get; set; }    //     ,
        public string Plant1Tile { get; set; }              //     p
        public string Plant2Tile { get; set; }              //     P
        public Texture2D ParallaxBackground { get; set; }
        public Texture2D BackgroundSky { get; set; }

        public string Gem1Tile { get; set; }
        public string Gem2Tile { get; set; }
        public Color BackgroundColorTint { get; set; }

        public WorldTheme()
        {
        }
    }
}
