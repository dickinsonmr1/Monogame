using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace RPG.Graphics
{
    public class SpriteSheet
    {
        public Dictionary<string, GameObjects.Sprite> Items { get; private set; }

        public Texture2D spriteSheet{get; private set;}

        public SpriteSheet()//SheetType type)
        {
            this.Items = new Dictionary<string, GameObjects.Sprite>();
        }
        public void LoadContentFromXml(string spriteSheetXmlPath, Texture2D spriteSheet)
        {
            this.spriteSheet = spriteSheet;

            XDocument document = XDocument.Load(spriteSheetXmlPath, LoadOptions.None);

            foreach (XElement item in document.Descendants("SubTexture"))
            {
                var name = item.Attribute("name").Value;
                var x = int.Parse(item.Attribute("x").Value);
                var y = int.Parse(item.Attribute("y").Value);
                var width = int.Parse(item.Attribute("width").Value);
                var height = int.Parse(item.Attribute("height").Value);

                Rectangle sourceRectangle = new Rectangle(x, y, width, height);
                this.Items.Add(name.Substring(0, name.Length - 4), new GameObjects.Sprite(name, new Vector2(x, y), width, height));
            }
        }

        public void LoadContentFromTxt(string spriteSheetTxtPath, Texture2D spriteSheet)
        {
            this.spriteSheet = spriteSheet;

            List<string> lines = new List<string>();
            using (StreamReader reader = new StreamReader(spriteSheetTxtPath))
            {
                string line;
	            while ((line = reader.ReadLine()) != null)
                {
                    string[] words = line.Split(' ');

                    var name = words[0];
                    var x = int.Parse(words[2]);
                    var y = int.Parse(words[3]);
                    var width = int.Parse(words[4]);
                    var height = int.Parse(words[5]);

                    Rectangle sourceRectangle = new Rectangle(x, y, width, height);
                    this.Items.Add(name, new GameObjects.Sprite(name, new Vector2(x, y), width, height));
                }
            }
        }

        public void DrawItem(SpriteBatch spriteBatch, string key,Vector2 location, Rectangle rectangle,
            bool facingRight, Color color)
        {
            if (!string.IsNullOrEmpty(key))
            {
                var sprite = this.Items[key];

                Rectangle sourceRectangle = new Rectangle((int)sprite.Location.X, (int)sprite.Location.Y, sprite.Width, sprite.Height);

                float ratio = 1.0f;
                Rectangle destinationRectangle = new Rectangle((int)location.X, (int)location.Y, rectangle.Width, rectangle.Height);

                if (sprite.Shape == GameObjects.SpriteShape.TallAndSkinny)
                {
                    ratio = (float)sprite.Height / (float)sprite.Width;
                    destinationRectangle = new Rectangle((int)location.X, (int)location.Y, rectangle.Width, rectangle.Height);
                }
                if (sprite.Shape == GameObjects.SpriteShape.ShortAndFat)
                {
                    ratio = (float)sprite.Height / (float)sprite.Width;
                    
                    //sourceRectangle = new Rectangle((int)sprite.Location.X, (int)sprite.Location.Y + sprite.Height, sprite.Width, sprite.Height);
                    //destinationRectangle = new Rectangle((int)location.X, (int)location.Y + sprite.Height, rectangle.Width, rectangle.Height);
                    destinationRectangle = new Rectangle((int)location.X, (int)location.Y, rectangle.Width, rectangle.Height);
                    
                }

                SpriteEffects spriteFlipEffects = facingRight == true ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

                spriteBatch.Draw(spriteSheet, destinationRectangle, sourceRectangle, color, 0, Vector2.Zero, spriteFlipEffects, 0);
            }
        }

        public void DrawItem(SpriteBatch spriteBatch, string key, Vector2 location, 
            bool facingRight, int tileSize, Color color)
        {
            if(!string.IsNullOrEmpty(key))
            {
                var sprite = this.Items[key];

                Rectangle sourceRectangle = new Rectangle((int)sprite.Location.X, (int)sprite.Location.Y, sprite.Width, sprite.Height);

                float ratio = 1.0f;
                Rectangle destinationRectangle = new Rectangle((int)(location.X - 0.5 * sprite.Width), (int)(location.Y - 0.5 * sprite.Height), sprite.Width, sprite.Height);
                var spriteFlipEffects = facingRight == true ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

                spriteBatch.Draw(spriteSheet, destinationRectangle, sourceRectangle, color, 0, Vector2.Zero, spriteFlipEffects, 0);
            }
        }
    }
}
