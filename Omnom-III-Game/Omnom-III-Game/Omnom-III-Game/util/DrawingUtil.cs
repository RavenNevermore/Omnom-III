using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Omnom_III_Game.util {
    class DrawingUtil {

        public static Texture2D createTexture(GraphicsDevice graphicsDevice, Color color) {
            return createTexture(graphicsDevice, 1, 1, color);
        }

        public static Texture2D createTexture(GraphicsDevice graphicsDevice, int width, int height, Color color) {
            // create the rectangle texture without colors
            Texture2D texture = new Texture2D(
                graphicsDevice,
                width,
                height,
                false,
                SurfaceFormat.Color);

            // Create a color array for the pixels
            Color[] colors = new Color[width * height];
            for (int i = 0; i < colors.Length; i++) {
                colors[i] = new Color(color.ToVector3());
            }

            // Set the color data for the texture
            texture.SetData(colors);

            return texture;
        }
    }
}
