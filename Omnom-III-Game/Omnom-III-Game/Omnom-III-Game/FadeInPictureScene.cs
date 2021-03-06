﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Omnom_III_Game.util;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Omnom_III_Game {
    public class FadeInPictureScene : IScene {

        private long fadeInTime = 1500;
        private long fadeOutTime = 200;

        private String pictureName;
        private Texture2D picture;
        private Color backgroundColor;
        private Rectangle targetSize;

        private float fadeAmount;
        private Stopwatch runningTime;
        private bool exit;

        public FadeInPictureScene(String pictureName, Color background, long fadeTime) {
            this.pictureName = pictureName;
            this.backgroundColor = background;
            this.fadeInTime = fadeTime * 7 / 8;
            this.fadeOutTime = fadeTime * 1 / 8;
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
            if (deltaT > this.fadeInTime + this.fadeOutTime) {
                this.fadeAmount = 0.0f;
            } else if (deltaT > this.fadeInTime) {
                this.fadeAmount = 1.0f - (float) (deltaT - fadeInTime) / (float) fadeOutTime;
            } else {
                this.fadeAmount = (float)deltaT / (float)fadeInTime;
            }
        }

        public void draw(SpriteBatchWrapper sprites, GraphicsDevice device) {
            if (null != backgroundColor)
                sprites.fillWithColor(backgroundColor, 1.0f);
            if (!targetSizeIsValid()) {
                this.targetSize = sprites.calcMaxProportionalSize(this.picture);
            }

            sprites.drawFromCenter(
                this.picture, 
                this.targetSize.Width, 
                this.targetSize.Height, 
                0, 0, 
                Color.White * this.fadeAmount);

        }

        private bool targetSizeIsValid() {
            return this.targetSize.Width > 0 && this.targetSize.Height > 0;
        }

        
        public void cleanup() {
            this.picture = null;
        }

        public SceneActivationParameters nextScene() {
            return null;
        }

        public bool wantsToExit() {
            return this.exit ||
                this.runningTime.ElapsedMilliseconds > this.fadeInTime + this.fadeOutTime + 100;
        }
    }
}
