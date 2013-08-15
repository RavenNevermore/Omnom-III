using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Omnom_III_Game.util {
    public class SpriteBatchWrapper {

        public enum Direction { LEFT, RIGHT, CENTER }

        private SpriteBatch wrapped;
        private GraphicsDevice wrappedDevice;
        private Dictionary<string, SpriteFont> fonts;
        private ContentUtil content;
        private Rectangle viewport;

        public SpriteBatchWrapper() { }
        public SpriteBatchWrapper(SpriteBatch wrapped, GraphicsDevice device, ContentUtil content) {
            this.wrapped = wrapped;
            this.wrappedDevice = device;
            this.viewport = device.Viewport.Bounds;
            this.fonts = new Dictionary<string, SpriteFont>();
            this.fonts["default"] = content.load<SpriteFont>("default");
            this.content = content;
        }

        private SpriteFont getFont(String name) {
            if (null == name) {
                name = "default";
            }
            if (!this.fonts.ContainsKey(name)) {
                this.fonts[name] = this.content.load<SpriteFont>(name); ;
            }
            return this.fonts[name];
        }

        private SpriteFont defaultFont { get { return this.getFont("default"); } }

        public void drawDebugText(params object[] text) {
            /*String msg = "";
            foreach (object item in text){
                msg += null == item ? "null":item.ToString();
                msg += " ";
            }
            this.wrapped.DrawString(this.defaultFont, msg, new Vector2(5, 5), Color.DarkGray);*/
        }

        public void drawTextCentered(String text, String fontName, float scale, Color color) {
            int x = this.wrappedDevice.Viewport.Width / 2;
            int y = this.wrappedDevice.Viewport.Height / 2;
            SpriteFont font = this.getFont(fontName);

            Vector2 textSize = font.MeasureString(text) * scale;

            x -= (int) textSize.X / 2;
            y -= (int) textSize.Y / 2;

            this.wrapped.DrawString(font, text, new Vector2(x, y), color, 0.0f,
                Vector2.Zero, scale, SpriteEffects.None, 0);
        }

        public void drawTextCentered(String text, int lineOffset, Color color) {
            int x = 50;
            int y = this.getYForTextLine(lineOffset, null);

            this.wrapped.DrawString(this.defaultFont, text, new Vector2(x, y), color);
        }

        public void drawTextAt(String text, int x, int y, float scale, Color color, String fontName, 
                Direction measurementDirection) {

            if (null == text)
                return;

            SpriteFont font = this.defaultFont;
            if (null != fontName) {
                font = this.getFont(fontName);
            }

            Vector2 size = font.MeasureString(text);

            if (x < 0) {
                x = this.wrappedDevice.Viewport.Width + x;
                x -= (int) size.X;
            }
            if (y < 0) {
                y = this.wrappedDevice.Viewport.Height + y;
                y -= (int) size.Y;
            }

            if (Direction.CENTER == measurementDirection) {
                x += (int) size.X / 2;
                y += (int) size.Y / 2;
            } else if (Direction.RIGHT == measurementDirection){
                x -= (int) size.X;
                //y += (int) size.Y;
            }

            this.wrapped.DrawString(font, text, new Vector2(x, y), color, 0.0f,
                Vector2.Zero, scale, SpriteEffects.None, 0);
        }
        
        public void drawTextAt(String text, int x, int y, float scale, Color color, String fontName) {
            this.drawTextAt(text, x, y, scale, color, fontName, Direction.LEFT);
        }

        public void drawTextAt(String text, int x, int y, float scale, Color color) {
            this.drawTextAt(text, x, y, scale, color, null);
        }


        public int getYForTextLine(int line, String fontName){
            return (this.viewport.Height / 2) + this.getFont(fontName).LineSpacing * line;
        }

        public int getWidthOfText(String text, float scale, String fontName) {
            return (int) (this.getFont(fontName).MeasureString(text).X * scale);
        }

        public int getHeightOfText(String text, float scale, String fontName) {
            return (int) (this.getFont(fontName).MeasureString(text).Y * scale);
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

        public Rectangle calcMaxProportionalSize(Texture2D texture) {
            float imageAspect = (float) texture.Bounds.Width / (float) texture.Bounds.Height;
            int maxH = this.wrappedDevice.Viewport.Height;
            int maxW = this.wrappedDevice.Viewport.Width;

            int wAtMaxH = (int) (maxH * imageAspect);
            int hAtMaxW = (int) (maxW / imageAspect);

            if (wAtMaxH > maxW) {
                return new Rectangle(0, 0, maxW, hAtMaxW);
            } else {
                return new Rectangle(0, 0, wAtMaxH, maxH);
            }
        }

        public void drawTextureAt(Texture2D texture,
                int width, int height,
                int x, int y) {

            this.drawTextureAt(texture, 1.0f, width, height, x, y);
        }

        public void drawTextureAt(Texture2D texture, float alpha,
                int width, int height,
                int x, int y) {

            this.wrapped.Draw(
                texture,
                new Rectangle(
                    x,
                    y,
                    width,
                    height), 
                texture.Bounds,
                Color.White * alpha);
        }

        public void drawColorAt(Color color,
                float alpha,
                int width, int height,
                int x, int y) {

            //color = color * alpha;
            Texture2D texture = DrawingUtil.createTexture(wrappedDevice, color);
            Rectangle bounds = new Rectangle(x, y, width, height);
            this.wrapped.Draw(texture, bounds, Color.White * alpha);
        }

        internal void fillWithColor(Color color, float alpha) {
            Texture2D texture = DrawingUtil.createTexture(wrappedDevice, color);
            this.wrapped.Draw(texture, wrappedDevice.Viewport.Bounds, Color.White * alpha);
        }

        internal void drawBackground(Texture2D texture) {
            this.drawBackground(texture, 1.0f);
        }

        internal void drawBackground(Texture2D texture, float alpha) {
            if (null == texture) {
                texture = DrawingUtil.createTexture(wrappedDevice, Color.CornflowerBlue);
            }
            this.wrapped.Draw(texture, wrappedDevice.Viewport.Bounds, Color.White * alpha);
        }

        internal Vector2 getCenterOfScreen() {
            Vector2 center = new Vector2();
            center.Y = this.wrappedDevice.Viewport.Height / 2;
            center.X = this.wrappedDevice.Viewport.Width / 2;
            return center;
        }
    }
}
