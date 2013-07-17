using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Omnom_III_Game.util;

namespace Omnom_III_Game.graphics {
    public class AnimatedTexture {
        private Texture2D texture;
        private int height;
        private int viewportWidth;
        private int textureWidth;
        private int frames;
        public long time;
        private long elapsedTime;
        private long frameTime;
        private Rectangle activeFrame;

        public AnimatedTexture(Texture2D texture) : this(texture, 0L) {}

        public AnimatedTexture(Texture2D texture, long time) {
            this.texture = texture;
            Rectangle textureDimensions = texture.Bounds;
            this.height = textureDimensions.Height;
            this.textureWidth = textureDimensions.Width;
            this.viewportWidth = this.height;
            this.frames = this.textureWidth / this.viewportWidth;
            this.time = time;
            this.frameTime = time / frames;
            this.reset();
        }



        public void reset(){
            this.activeFrame = new Rectangle(0, 0, this.viewportWidth, this.height);
            this.elapsedTime = 0;

        }

        public void update(long deltaT) {
            this.elapsedTime += deltaT;

            while (this.elapsedTime >= this.time) {
                this.elapsedTime -= this.time;
            }

            int frame = (int) (this.elapsedTime / this.frameTime);

            this.activeFrame = new Rectangle(
                frame * this.viewportWidth, 0, this.viewportWidth, this.height);
        }

        public void drawCentered(SpriteBatchWrapper sprites, int width, int height, int offsetY) {
            sprites.drawFromCenter(this.texture, this.activeFrame, width, height, 0, offsetY);
        }
    }
}
