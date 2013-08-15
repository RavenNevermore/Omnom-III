using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Omnom_III_Game.util;
using Omnom_III_Game.dance;
using Omnom_III_Game.graphics;

namespace Omnom_III_Game {
    public class IngameUI {

        VisualTransition transitions;
        Dictionary<InputState.Move, DirectionalIndicator> hud;
        float beatTimeInMs;

        Sound errorSound;
        Sound successSound;
        RandomSound seqSuccessSound;

        float flash = 0.0f;

        public IngameUI() {
            this.transitions = new VisualTransition();
            this.hud = new Dictionary<InputState.Move, DirectionalIndicator>();
        }

        public void initialize(
                ContentUtil content, 
                DanceSequenceProtocol protocol, 
                float beatTimeInMS,
                float uiSpeed) {

            this.beatTimeInMs = beatTimeInMS;
            this.initHud(content, uiSpeed);

            this.errorSound = new Sound("hud/error_move", content);
            this.successSound = null;// new Sound("hud/success_move");
            this.seqSuccessSound = new RandomSound("hud/perfect", 10, content);

            float bpms = 1 / beatTimeInMS;
            foreach (DanceSequence.Input input in protocol.handicaps) {
                if (!this.hud.ContainsKey(input.handicap))
                    continue;

                this.hud[input.handicap].prerecord(input.startTime(bpms), Color.Orange);
            }

            this.transitions.initialize(content, protocol, beatTimeInMS);
        }

        private void initHud(ContentUtil content, float uiSpeed) {
            this.hud.Clear();
            this.hud[InputState.Move.UP] = new DirectionalIndicator(content, InputState.Move.UP, this.beatTimeInMs, uiSpeed);
            this.hud[InputState.Move.DOWN] = new DirectionalIndicator(content, InputState.Move.DOWN, this.beatTimeInMs, uiSpeed);
            this.hud[InputState.Move.LEFT] = new DirectionalIndicator(content, InputState.Move.LEFT, this.beatTimeInMs, uiSpeed);
            this.hud[InputState.Move.RIGHT] = new DirectionalIndicator(content, InputState.Move.RIGHT, this.beatTimeInMs, uiSpeed);
        }


        public void update(PlayerProgress.RatedMoves rated, bool ratingComplete, long time, long deltaT) {
            this.transitions.update(time);

            this.flash = 1.0f - ((time % this.beatTimeInMs) / this.beatTimeInMs);
            this.flash = this.flash / 16;

            if (ratingComplete)
                this.seqSuccessSound.play();

            if (null != rated) {
                foreach (DanceSequence.Input ratedInput in rated.allFromUserInput) {
                    if (!this.hud.ContainsKey(ratedInput.handicap))
                        continue;

                    Sound sound = getSoundForRating(rated, ratedInput);
                    if (null != sound) {
                        sound.play();
                    }

                    Color color = getColorForRating(rated, ratedInput);
                    this.hud[ratedInput.handicap].start(color);
                }
            }

            foreach (KeyValuePair<InputState.Move, DirectionalIndicator> hudElement in this.hud) {
                hudElement.Value.update(time, deltaT);
            }
        }

        private Sound getSoundForRating(PlayerProgress.RatedMoves rated, DanceSequence.Input ratedInput) {
            
            Sound sound;            
            if (rated.ok.Contains(ratedInput)) {
                sound = this.successSound;
            } else if (rated.good.Contains(ratedInput)) {
                sound = this.successSound;
            } else if (rated.perfect.Contains(ratedInput)) {
                sound = this.successSound;
            } else {
                sound = this.errorSound;
            }
            return sound;
        }

        private static Color getColorForRating(PlayerProgress.RatedMoves rated, DanceSequence.Input ratedInput) {
            Color color;
            if (rated.ok.Contains(ratedInput)) {
                color = Color.Yellow;
            } else if (rated.good.Contains(ratedInput)) {
                color = Color.GreenYellow;
            } else if (rated.perfect.Contains(ratedInput)) {
                color = Color.Lime;
            } else {
                color = Color.Red;
            }
            return color;
        }

        public void draw(SpriteBatchWrapper sprites) {
            sprites.fillWithColor(Color.White, this.flash);

            this.transitions.draw(sprites);

            foreach (KeyValuePair<InputState.Move, DirectionalIndicator> hudElement in this.hud) {
                hudElement.Value.draw(sprites);
            }
        }
    }
}
