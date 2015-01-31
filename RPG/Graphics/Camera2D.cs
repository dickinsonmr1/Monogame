using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;

namespace RPG
{
    // http://www.david-amador.com/2009/10/xna-camera-2d-with-zoom-and-rotation/
     public class Camera2D
     {
        protected float          _zoom; // Camera Zoom
        public Matrix             Transform; // Matrix Transform
        public Vector2          Position; // Camera Position
        protected float         Rotation2D; // Camera Rotation
 

         // Sets and gets zoom
        public float Zoom
        {
            get { return _zoom; }
            set { _zoom = value; if (_zoom < 0.1f) _zoom = 0.1f; } // Negative zoom will flip image
        }
 
        public float Rotation
        {
            get {return Rotation2D; }
            set { Rotation2D = value; }
        }
 
        // Auxiliary function to move the camera
        public void Move(Vector2 amount)
        {
           Position += amount;
        }
       // Get set position
        public Vector2 Pos
        {
             get{ return  Position; }
             set{ Position = value; }
        }

        public Camera2D()
        {
            _zoom = 1.0f;
            Rotation2D = 0.0f;
            Position = Vector2.Zero;
        }
         
        public Matrix GetViewMatrix(GraphicsDevice graphicsDevice, Vector2 parallax)
        {
            Transform =       // Thanks to o KB o for this solution
                Matrix.CreateTranslation(new Vector3(-Position * parallax, 0)) *
                    Matrix.CreateRotationZ(Rotation) *
                    Matrix.CreateScale(new Vector3(Zoom, Zoom, 1)) *
                    Matrix.CreateTranslation(
                        new Vector3(graphicsDevice.Viewport.Width * 0.5f,
                        graphicsDevice.Viewport.Height * 0.5f, 0));
            return Transform;
        }
     }
}
