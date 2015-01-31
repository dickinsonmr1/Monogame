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
    public class Player
    {
        public Vector2 Location { get; private set; }
        public Player(Vector2 location)
        {
            this.Location = location;
        }
        public void Update()
        {
        }
    }
}
