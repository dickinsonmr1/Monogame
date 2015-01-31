using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RPG.Graphics
{
    public class Layer
    {
        public Layer(Camera2D camera, Texture2D texture, Vector2 parallax)
        {
            _camera = camera;
            this.texture = texture;
            this.Parallax = parallax;
            //Sprites = new List<Sprite>();
        }

        public Vector2 Parallax { get; set; }
        public Texture2D texture {get; set;}
        //public List<Sprite> Sprites { get; private set; }

        public void Draw(GraphicsDevice device, SpriteBatch spriteBatch, Color color)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, _camera.GetViewMatrix(device, Parallax));

            //spriteBatch.Draw(texture, new Rectangle((int) -texture.Width, 0, device.Viewport.Width, device.Viewport.Height), Color.White);
            spriteBatch.Draw(texture, new Rectangle((int) -texture.Width / 2, 0, texture.Width, texture.Height), color);
            //foreach (Sprite sprite in Sprites)
                //sprite.Draw(spriteBatch);
            spriteBatch.End();
        }

        private readonly Camera2D _camera;
    }
}
