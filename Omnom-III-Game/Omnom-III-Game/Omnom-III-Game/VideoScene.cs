using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Omnom_III_Game.util;

namespace Omnom_III_Game {
    class VideoScene : IScene {

        private Video video;
        private VideoPlayer player;
        private Rectangle size;
        private String videoName;
        private bool forceExit;

        public VideoScene(String videoName) {
            this.videoName = videoName;
        }

        public void initialize(ContentUtil content, SceneActivationParameters parameters) {
            this.video = content.load<Video>(this.videoName);
            this.player = null;
            this.size = new Rectangle(0, 0, 0, 0);
            this.forceExit = false;
        }

        private ExplicitInputState input;
        public void update(InputState inputState) {
            if (this.wantsToExit())
                return;


            if (null == this.input) {
                this.input = new ExplicitInputState();
            }
            this.input.update(inputState);
            
            if (null == this.player) {
                this.player = new VideoPlayer();
                this.player.Play(this.video);
            }

            if (this.input.isActive(InputState.Control.BACK) ||
                this.input.isActive(InputState.Control.EXIT)) {
                    this.forceExit = true;
            }
        }

        public void draw(SpriteBatchWrapper sprites, GraphicsDevice device) {
            sprites.fillWithColor(Color.Black, 1.0f);
            
            if (null == this.player || MediaState.Stopped == this.player.State)
                return;

            Texture2D videoTexture = player.GetTexture();
            if (null != videoTexture) {
                if (0 == this.size.Width)
                    this.size = sprites.calcMaxProportionalSize(videoTexture);

                sprites.drawFromCenter(videoTexture, this.size.Width, this.size.Height);
            }
        }

        public void cleanup() {
            if (null != this.player && 
                    MediaState.Stopped != this.player.State &&
                    this.video.Equals(this.player.Video)) {
                
                this.player.Stop();
            }
            this.video = null;
            this.player = null;
        }

        public SceneActivationParameters nextScene() {
            return null;
        }

        public bool wantsToExit() {
            return this.forceExit || (null != this.player &&
                    MediaState.Stopped == this.player.State);
        }
    }
}
