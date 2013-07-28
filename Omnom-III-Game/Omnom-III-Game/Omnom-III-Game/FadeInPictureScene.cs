using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Omnom_III_Game.util;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Omnom_III_Game {
    public class FadeInPictureScene : IScene {

        private long fadeTime = 1000;

        private String pictureName;
        private Texture2D picture;
        private Color backgroundColor;
        private Rectangle targetSize;

        private float fadeAmount;
        private Stopwatch runningTime;
        private bool exit;

        public FadeInPictureScene(String pictureName, Color background) {
            this.pictureName = pictureName;
            this.backgroundColor = background;
        }

        public void setTargetSize(int width, int height) {
            this.targetSize = new Rectangle(0, 0, width, height);
        }

        public void initialize(ContentUtil content, SceneActivationParameters parameters) {
            this.picture = content.load<Texture2D>(this.pictureName);
            this.fadeAmount = 0.0f;
            this.runningTime = null;
        }

        public void update(InputState input) {
            if (input.isActive(InputState.Control.EXIT)) {
                this.exit = true;
                return;
            }
            if (null == this.runningTime)
                this.runningTime = Stopwatch.StartNew();

            long deltaT = this.runningTime.ElapsedMilliseconds;
            if (deltaT >= this.fadeTime) {
                this.fadeTime = 1;
            } else {
                this.fadeAmount = (float)deltaT / (float)fadeTime;
            }
        }

        public void draw(SpriteBatchWrapper sprites, GraphicsDevice device) {
            if (null != backgroundColor)
                sprites.fillWithColor(backgroundColor, 1.0f);
            if (!targetSizeIsValid()) {
                this.calculateTargetSize(device);
            }

            sprites.drawFromCenter(
                this.picture, 
                this.targetSize.Width, 
                this.targetSize.Height, 
                0, 0, 
                Color.White * (1.0f - this.fadeAmount));

        }

        private bool targetSizeIsValid() {
            return this.targetSize.Width > 0 && this.targetSize.Height > 0;
        }

        private void calculateTargetSize(GraphicsDevice device) {
            float imageAspect = (float) this.picture.Bounds.Width / (float) this.picture.Bounds.Height;
            int maxH = device.Viewport.Height;
            int maxW = device.Viewport.Width;

            int wAtMaxH = (int) (maxH * imageAspect);
            int hAtMaxW = (int) (maxW / imageAspect);

            if (wAtMaxH > maxW) {
                this.setTargetSize(maxW, hAtMaxW);
            } else {
                this.setTargetSize(wAtMaxH, maxH);
            }
        }

        public void cleanup() {
            this.picture = null;
        }

        public SceneActivationParameters nextScene() {
            return null;
        }

        public bool wantsToExit() {
            return this.exit ||
                this.runningTime.ElapsedMilliseconds > this.fadeTime + 100;
        }
    }
}
