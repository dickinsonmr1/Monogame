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

namespace GameObjects
{

    public enum SpriteShape
    {
        Square,
        TallAndSkinny,
        ShortAndFat,
        RetainSize
    }
    public class Sprite
    {
        public string Name { get; private set; }
        public string SheetName { get; private set; }
        public Vector2 Location { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public SpriteShape Shape { get; private set; }

        public Sprite(string name, Vector2 location, int width, int height)//, SpriteShape shape)
        {
            this.Name = name;
            this.Location = location;
            this.Width = width;
            this.Height = height;
            if (width == height)
            {
                this.Shape = SpriteShape.Square;
            }
            else
            {
                this.Shape = (width > height) ? SpriteShape.ShortAndFat : SpriteShape.TallAndSkinny;
            }
        }
    }
}
