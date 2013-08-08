using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Omnom_III_Game.graphics {
    public class ScaledTexture {

        public Texture2D texture;
        public float scale;

        public ScaledTexture(Texture2D texture, float scale) {
            this.texture = texture;
            this.scale = scale;
        }

        public Rectangle bounds {
            get {
                Rectangle bounds = new Rectangle();
                bounds.X = 0;
                bounds.Y = 0;
                bounds.Height = (int)((float)texture.Height * scale);
                bounds.Width = (int)((float)texture.Width * scale);
                return bounds;
            }
        }


        public int Width { get { return this.bounds.Width; } }

        public int Height { get { return this.bounds.Height; } }
    }
}
