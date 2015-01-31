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
    public class Sprite
    {
        public string Name { private get; set; }
        public Texture2D Texture { private get; set; }


        public Sprite(string name, Texture2D texture)
        {
            this.Name = name;
            this.Texture = texture;
        }
    }
}
