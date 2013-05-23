using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Omnom_III_Game.util {
    public class SpriteBatchWrapper {

        private SpriteBatch wrapped;
        private GraphicsDevice wrappedDevice;
        private SpriteFont font;

        public SpriteBatchWrapper() { }
        public SpriteBatchWrapper(SpriteBatch wrapped, GraphicsDevice device, SpriteFont defaultFont) {
            this.wrapped = wrapped;
            this.wrappedDevice = device;
            this.font = defaultFont;
        }

        public void drawDebugText(params object[] text) {
            String msg = "";
            foreach (object item in text){
                msg += null == item ? "null":item.ToString();
                msg += " ";
            }
            this.wrapped.DrawString(font, msg, new Vector2(5, 5), Color.DarkGray);
        }

        public void drawFromCenter(Texture2D texture, Rectangle viewport,
            int width, int height) {

            this.drawFromCenter(texture, viewport, width, height, 0, 0, Color.White);
        }

        public void drawFromCenter(Texture2D texture, Rectangle viewport,
            int width, int height,
            int offsetX, int offsetY) {

                this.drawFromCenter(texture, viewport, width, height, offsetX, offsetY, Color.White);
        }

        public void drawFromCenter(Texture2D texture, Rectangle viewport, 
            int width, int height,
            int offsetX, int offsetY, 
            Color color) {

            int centerX = (viewport.Width / 2) + offsetX;
            int centerY = (viewport.Height / 2) + offsetY;

            this.wrapped.Draw(
                texture,
                new Rectangle(
                    centerX - width / 2,
                    centerY - height / 2,
                    width,
                    height),
                color);
        }

        internal void fillWithColor(Color color, float alpha) {
            Texture2D texture = DrawingUtil.createTexture(wrappedDevice, color);
            this.wrapped.Draw(texture, wrappedDevice.Viewport.Bounds, Color.White * alpha);
        }
    }
}
