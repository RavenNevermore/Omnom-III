using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Omnom_III_Game;
using Omnom_III_Game.util;

namespace Omnom_III_Game.dance {
    class DanceSceneAnimationBundle {
        public Dictionary<InputState.Move, ButtonAnimation> opponent;
        public Dictionary<InputState.Move, ButtonAnimation> player;

        public DanceSceneAnimationBundle(Dictionary<String, Texture2D> textures, Song song) {
            this.opponent = new Dictionary<InputState.Move, ButtonAnimation>();
            this.player = new Dictionary<InputState.Move, ButtonAnimation>();

            long length = (long)song.beatTimeInMs;
            this.setup(this.opponent, textures, Color.LightBlue, length);
            this.setup(this.player, textures, Color.Green, length);
        }

        private void setup(Dictionary<InputState.Move, ButtonAnimation> animationSet, 
                Dictionary<String, Texture2D> textures, Color color, long length) {

            animationSet[InputState.Move.UP] = new ButtonAnimation(
                textures["btn_up"], new Vector2(0, -100), color, length);
            animationSet[InputState.Move.LEFT] = new ButtonAnimation(
                textures["btn_left"], new Vector2(-100, 0), color, length);
            animationSet[InputState.Move.RIGHT] = new ButtonAnimation(
                textures["btn_right"], new Vector2(100, 0), color, length);
            animationSet[InputState.Move.DOWN] = new ButtonAnimation(
                textures["btn_down"], new Vector2(0, 100), color, length);
        }

        public void startOpponentAnimation(InputState.Move move, long startPoint) {
            this.startAnimation(this.opponent, move, startPoint);
        }

        public void startPlayerAnimation(InputState.Move move, long startPoint) {
            this.startAnimation(this.player, move, startPoint);
        }

        private void startAnimation(Dictionary<InputState.Move, ButtonAnimation> bundle,
                InputState.Move move, long startPoint) {

            if (!bundle.ContainsKey(move))
                return;
            bundle[move].addStartPoint(startPoint);
        }

        public void update(long time) {
            this.performOnAll(x => x.update(time));
        }

        public void draw(SpriteBatchWrapper sprites) {
            this.performOnAll(x => x.draw(sprites));
        }

        delegate void func(ButtonAnimation x);
        private void performOnAll(func myFunc) {
            this.performOnDict(this.opponent, myFunc);
            this.performOnDict(this.player, myFunc);
        }

        private void performOnDict(Dictionary<InputState.Move, ButtonAnimation> animations,
                func myFunc) {
            foreach (KeyValuePair<InputState.Move, ButtonAnimation> anim in animations) {
                myFunc(anim.Value);
            }
        }
    }
}
