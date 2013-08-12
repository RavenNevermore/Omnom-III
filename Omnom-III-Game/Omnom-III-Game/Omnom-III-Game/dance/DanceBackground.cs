using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Omnom_III_Game.util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Omnom_III_Game.dance {
    public class DanceBackground {
        class BeatLayer {
            public Texture2D texture;
            public float alpha;
            public float startInMeasure;
            public float updateLengthInBeats;

            public BeatLayer(ContentUtil content, String loadString) {
                String[] cfg = loadString.Split(' ');
                this.texture = content.load<Texture2D>(cfg[0]);
                this.alpha = 0.0f;

                this.updateLengthInBeats = cfg.Length > 1 ? ParserUtil.toFloat(cfg[1]) : 1.0f;
                this.startInMeasure = cfg.Length > 2 ? ParserUtil.toFloat(cfg[2]) : 0.0f;
                
            }

            public void updateAlpha(float songTimeInMeasures) {
                songTimeInMeasures -= this.startInMeasure;
                float alphaUpdates = songTimeInMeasures * (4 / this.updateLengthInBeats);
                float positionInUpdate = alphaUpdates - ((int)alphaUpdates);
                
                this.alpha = 1.0f - positionInUpdate;
                this.alpha *= this.updateLengthInBeats;
                this.alpha -= (this.updateLengthInBeats - 1);
                if (this.alpha < 0.0f)
                    this.alpha = 0.0f;
            }
        }

        private Texture2D bakground;
        private BeatLayer[] beatSigns;
        private Texture2D[] countSigns;

        private int countsShown;

        public void initialize(ContentUtil content, ContentScript script) { 
            this.bakground = content.load<Texture2D>(script.get("background"));

            if (script.contains("beatlayers")){
                this.beatSigns = new BeatLayer[script["beatlayers"].Count];
                for (int i = 0; i < this.beatSigns.Length; i++){
                    this.beatSigns[i] = new BeatLayer(content, script["beatlayers"][i]);
                }
            }

            if (script.contains("countlayers")) {
                this.countSigns = new Texture2D[script["countlayers"].Count];
                for (int i = 0; i < this.countSigns.Length; i++) {
                    this.countSigns[i] = content.load<Texture2D>(script["countlayers"][i]);
                }
            }
        }

        

        public void update(float songTimeInMeasures, float sequenceTimeNormal) {
            if (null != this.beatSigns) {
                foreach (BeatLayer beatsign in this.beatSigns) {
                    beatsign.updateAlpha(songTimeInMeasures);
                }
            }

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
            if (null != this.beatSigns) {
                foreach (BeatLayer beatsign in this.beatSigns) {
                    sprites.drawBackground(beatsign.texture, beatsign.alpha);
                }
            }
            if (null != this.countSigns) {
                for (int i = 0; i < this.countsShown; i++) {
                    sprites.drawBackground(this.countSigns[i]);
                }
            }
        }
    }
}
