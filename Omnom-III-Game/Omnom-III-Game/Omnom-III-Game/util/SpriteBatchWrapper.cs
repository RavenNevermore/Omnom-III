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
        private Rectangle viewport;

        public SpriteBatchWrapper() { }
        public SpriteBatchWrapper(SpriteBatch wrapped, GraphicsDevice device, SpriteFont defaultFont) {
            this.wrapped = wrapped;
            this.wrappedDevice = device;
            this.viewport = device.Viewport.Bounds;
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

        public void drawTextCentered(String text, int lineOffset, Color color) {
            int x = 50;
            int y = this.getYForTextLine(lineOffset);

            this.wrapped.DrawString(this.font, text, new Vector2(x, y), color);
        }

        public void drawTextAt(String text, int x, int y, float scale, Color color) {
            this.wrapped.DrawString(this.font, text, new Vector2(x, y), color, 0.0f, 
                Vector2.Zero, scale, SpriteEffects.None, 0);
            //this.wrapped.DrawString(this.font, text, new Vector2(x, y), color);
        }

        public int getYForTextLine(int line) {
            return (this.viewport.Height / 2) + this.font.LineSpacing * line;
        }

        public int getWidthOfText(String text, float scale) {
            return (int) (this.font.MeasureString(text).X * scale);
        }

        public void drawFromCenter(Texture2D texture, int width, int height) {
            this.drawFromCenter(texture, width, height, 0, 0, Color.White);
        }

        public void drawFromCenter(Texture2D texture, Rectangle sourceRect, int width, int height) {
            this.drawFromCenter(texture, sourceRect, width, height, 0, 0, Color.White);
        }

        public void drawFromCenter(Texture2D texture,
            int width, int height,
            int offsetX, int offsetY) {

                this.drawFromCenter(texture, width, height, offsetX, offsetY, Color.White);
        }

        public void drawFromCenter(Texture2D texture, Rectangle sourceRect,
            int width, int height,
            int offsetX, int offsetY) {

            this.drawFromCenter(texture, sourceRect, width, height, offsetX, offsetY, Color.White);
        }

        public void drawFromCenter(Texture2D texture,
                int width, int height,
                int offsetX, int offsetY, 
                Color color) {

            Rectangle sourceRect = texture.Bounds;

            this.drawFromCenter(texture, sourceRect, width, height, offsetX, offsetY, color);
        }

        public void drawFromCenter(Texture2D texture,
            Rectangle sourceRect,
            int width, int height,
            int offsetX, int offsetY,
            Color color) {

            int centerX = (this.viewport.Width / 2) + offsetX;
            int centerY = (this.viewport.Height / 2) + offsetY;

            this.wrapped.Draw(
                texture,
                new Rectangle(
                    centerX - width / 2,
                    centerY - height / 2,
                    width,
                    height), 
                sourceRect,
                color);
        }

        internal void fillWithColor(Color color, float alpha) {
            Texture2D texture = DrawingUtil.createTexture(wrappedDevice, color);
            this.wrapped.Draw(texture, wrappedDevice.Viewport.Bounds, Color.White * alpha);
        }

        internal void drawBackground(Texture2D texture) {
            if (null == texture) {
                texture = DrawingUtil.createTexture(wrappedDevice, Color.CornflowerBlue);
            }
            this.wrapped.Draw(texture, wrappedDevice.Viewport.Bounds, Color.White);
        }
    }
}
