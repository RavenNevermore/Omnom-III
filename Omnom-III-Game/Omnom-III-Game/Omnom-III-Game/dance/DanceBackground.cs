using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Omnom_III_Game.util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Omnom_III_Game.dance {
    class DanceBackground {

        private Texture2D bakground;
        private Texture2D beatSign;
        private Texture2D[] countSigns;

        private float beatAlpha;
        private int countsShown;

        public void initialize(ContentUtil content, ContentScript script) { 
            this.bakground = content.load<Texture2D>(script.get("background"));

            String beatSignTexture = script.get("beatlayer");
            if (null != beatSignTexture) {
                this.beatSign = content.load<Texture2D>(beatSignTexture);
            }

            if (script.contains("countlayers")) {
                this.countSigns = new Texture2D[script["countlayers"].Count];
                for (int i = 0; i < this.countSigns.Length; i++) {
                    this.countSigns[i] = content.load<Texture2D>(script["countlayers"][i]);
                }
            }
        }

        public void update(float songTimeInMeasures, float sequenceTimeNormal) {
            float beats = songTimeInMeasures * 4;
            this.beatAlpha = 1.0f - (beats - ((int)beats));

            if (null != this.countSigns){
                if (sequenceTimeNormal > 1.0f)
                    sequenceTimeNormal -= 1.0f;

                if (0.0f <= sequenceTimeNormal && sequenceTimeNormal <= 1.0f) {
                    this.countsShown = (int)(sequenceTimeNormal * this.countSigns.Length);
                    this.countsShown += 1;
                } else {
                    this.countsShown = -1;
                }
            }
        }

        public void draw(SpriteBatchWrapper sprites) {
            sprites.drawBackground(this.bakground);
            if (null != this.beatSign) {
                sprites.drawBackground(this.beatSign, this.beatAlpha);
            }
            if (null != this.countSigns) {
                for (int i = 0; i < this.countsShown; i++) {
                    sprites.drawBackground(this.countSigns[i]);
                }
            }
        }
    }
}
