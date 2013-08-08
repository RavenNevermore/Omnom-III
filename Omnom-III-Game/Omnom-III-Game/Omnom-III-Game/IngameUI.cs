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

        public IngameUI() {
            this.transitions = new VisualTransition();
            this.hud = new Dictionary<InputState.Move, DirectionalIndicator>();
        }

        public void initialize(
                ContentUtil content, 
                DanceSequenceProtocol protocol, 
                float beatTimeInMS) {

            this.initTextures(content, beatTimeInMS);

            float bpms = 1 / beatTimeInMS;
            foreach (DanceSequence.Input input in protocol.handicaps) {
                if (!this.hud.ContainsKey(input.handicap))
                    continue;

                this.hud[input.handicap].prerecord(input.startTime(bpms), Color.White);
            }

            this.transitions.initialize(content, protocol, beatTimeInMS);
        }

        private void initTextures(ContentUtil content, float beatTimeInMS) {
            this.hud.Clear();
            this.hud[InputState.Move.UP] = new DirectionalIndicator(content, InputState.Move.UP, beatTimeInMS);
            this.hud[InputState.Move.DOWN] = new DirectionalIndicator(content, InputState.Move.DOWN, beatTimeInMS);
            this.hud[InputState.Move.LEFT] = new DirectionalIndicator(content, InputState.Move.LEFT, beatTimeInMS);
            this.hud[InputState.Move.RIGHT] = new DirectionalIndicator(content, InputState.Move.RIGHT, beatTimeInMS);
        }


        public void update(PlayerProgress.RatedMoves rated, long time, long deltaT) {
            this.transitions.update(time);

            if (null != rated) {
                foreach (DanceSequence.Input ratedInput in rated.allFromUserInput) {
                    if (!this.hud.ContainsKey(ratedInput.handicap))
                        continue;

                    Color color = getColorForRating(rated, ratedInput);
                    this.hud[ratedInput.handicap].start(color);
                }
            }

            foreach (KeyValuePair<InputState.Move, DirectionalIndicator> hudElement in this.hud) {
                hudElement.Value.update(time, deltaT);
            }
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
            this.transitions.draw(sprites);

            foreach (KeyValuePair<InputState.Move, DirectionalIndicator> hudElement in this.hud) {
                hudElement.Value.draw(sprites);
            }
        }
    }
}
