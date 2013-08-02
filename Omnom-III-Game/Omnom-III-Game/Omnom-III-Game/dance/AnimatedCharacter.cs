using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Omnom_III_Game.graphics;
using Omnom_III_Game.util;

namespace Omnom_III_Game.dance {
    public class AnimatedCharacter {
        private AnimatedTexture idleTexture;
        private AnimatedTexture upTexture;
        private AnimatedTexture leftTexture;
        private AnimatedTexture rightTexture;
        private AnimatedTexture downTexture;

        private AnimatedTexture activeTexture;

        public AnimatedCharacter(String name, ContentUtil content, long beatTimeInMs) {

            this.idleTexture  = this.loadTexture(content, name + "_idle",  beatTimeInMs);
            this.upTexture    = this.loadTexture(content, name + "_up",    beatTimeInMs / 2);
            this.upTexture.repeat = false;
            this.leftTexture  = this.loadTexture(content, name + "_left",  beatTimeInMs / 2);
            this.leftTexture.repeat = false;
            this.rightTexture = this.loadTexture(content, name + "_right", beatTimeInMs / 2);
            this.rightTexture.repeat = false;
            this.downTexture  = this.loadTexture(content, name + "_down",  beatTimeInMs / 2);
            this.downTexture.repeat = false;

            this.reset();
        }

        public void reset() {
            this.idleTexture.reset();
            this.leftTexture.reset();
            this.rightTexture.reset();
            this.upTexture.reset();
            this.downTexture.reset();
            this.activeTexture = this.idleTexture;
        }

        private AnimatedTexture loadTexture(ContentUtil content, String name, long time) {
            Texture2D tex = content.load<Texture2D>(name);
            AnimatedTexture anim = new AnimatedTexture(tex, time);
            return anim;
        }

        public void update(long deltaT) {
            if (null == this.activeTexture)
                return;

            this.activeTexture.update(deltaT);
            if (this.activeTexture.stopped) {
                this.activate(InputState.Move.BREAK);
            }
        }

        public void draw(SpriteBatchWrapper sprites) {
            if (null == this.activeTexture)
                return;

            this.activeTexture.drawCentered(sprites, 450, 450, 75);
        }

        public void activate(InputState.Move move) {
            AnimatedTexture activate = this.idleTexture;
            if (InputState.Move.UP == move) {
                activate = this.upTexture;
            } else if (InputState.Move.DOWN == move) {
                activate = this.downTexture;
            } else if (InputState.Move.LEFT == move) {
                activate = this.leftTexture;
            } else if (InputState.Move.RIGHT == move) {
                activate = this.rightTexture;
            }

            if (this.activeTexture == activate && !this.activeTexture.stopped) {
                return;
            }
            this.activeTexture = activate;
            this.activeTexture.reset();
        }
    }
}
