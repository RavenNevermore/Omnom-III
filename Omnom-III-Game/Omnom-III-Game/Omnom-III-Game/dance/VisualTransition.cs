using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Omnom_III_Game.util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Omnom_III_Game.dance {
    public class VisualTransition {

        static int DRAW_GAP_FOR_PRECOUNT = 32;
        static int CIRCLE_SIZE = 24;

        class TransitPoint : IComparable<TransitPoint> {
            public long[] preCounts;
            public long transition;
            public VisualTransition parent;
            public bool transitFromEnemy;

            public Color flashColor { get { return this.transitFromEnemy ? Color.LightGreen : Color.LightSkyBlue; } }

            public int CompareTo(TransitPoint other) {
                return this.transition.CompareTo(other.transition);
            }

            public bool isActive(long time) {
                return this.preCounts[0] <= time && 
                    this.transition + this.parent.flashTime >= time;
            }
        }

        long flashTime;

        float beatTimeInMs;
        List<TransitPoint> transitPoints;

        TransitPoint activeTransit;
        long  lastSongTime;

        Texture2D preCountTexture;
        Texture2D preCountShadowTexture;

        public void initialize(ContentUtil content,
                DanceSequenceProtocol protocol, float beatTimeInMs) {

            this.preCountTexture = content.load<Texture2D>("circle");
            this.preCountShadowTexture = content.load<Texture2D>("circle_shadow");

            this.transitPoints = new List<TransitPoint>();
            this.beatTimeInMs = beatTimeInMs;
            this.flashTime = (long) (beatTimeInMs / 2);

            this.activeTransit = null;
            this.lastSongTime = 0;

            this.initWithProtocol(protocol);
        }

        private void initWithProtocol(DanceSequenceProtocol protocol) {
            foreach (DanceSequence sequence in protocol.asList()) {
                this.addTransitPoint(sequence.startMeasure, false);

                this.addTransitPoint(
                    sequence.startMeasure + sequence.length, true);
            }
        }

        public void addTransitPoint(float timeInSong, bool transitFromEnemy) {
            float measureTimeInMs = this.beatTimeInMs * 4;

            TransitPoint transit = new TransitPoint();
            transit.parent = this;
            transit.transitFromEnemy = transitFromEnemy;
            
            transit.transition = (long) (timeInSong * measureTimeInMs);
            transit.preCounts = new long[4];
            transit.preCounts[0] = transit.transition - (long) (this.beatTimeInMs * 4);
            transit.preCounts[1] = transit.transition - (long) (this.beatTimeInMs * 3);
            transit.preCounts[2] = transit.transition - (long) (this.beatTimeInMs * 2);
            transit.preCounts[3] = transit.transition - (long) (this.beatTimeInMs * 1);

            this.transitPoints.Add(transit);
            this.transitPoints.Sort();
        }

        public void update(long songTime) {
            this.lastSongTime = songTime;

            if (null != this.activeTransit && this.activeTransit.isActive(songTime))
                return;

            this.activeTransit = null;

            foreach (TransitPoint transit in this.transitPoints) {
                if (transit.isActive(songTime)) {
                    this.activeTransit = transit;
                    break;
                }
            }
        }

        public void draw(SpriteBatchWrapper sprites) {
            if (null == this.activeTransit)
                return;

            if (this.lastSongTime < this.activeTransit.transition)
                this.drawPreCount(sprites);

            this.drawFlash(sprites);
        }

        private float calcFlashyness(float flashPosition) {
            return 1.0f - ((float) (this.lastSongTime - flashPosition)) / ((float) this.flashTime);
        }

        private void drawFlash(SpriteBatchWrapper sprites) {
            if (this.lastSongTime >= this.activeTransit.transition
                    && this.lastSongTime <= this.activeTransit.transition + this.flashTime) {

                sprites.fillWithColor(
                    this.activeTransit.flashColor,
                    this.calcFlashyness(this.activeTransit.transition));
            }
        }

        private void drawPreCount(SpriteBatchWrapper sprites) {
            int firstX = this.getFirstPreCountX();
            int i = -1;
            int lastCount = 0;
            foreach (long preCount in this.activeTransit.preCounts) {
                i++;
                if (preCount < 0)
                    continue;

                drawPreCountIndicator(sprites, this.preCountShadowTexture, firstX, i);

                if (preCount > this.lastSongTime) {
                    continue;
                }
                
                sprites.fillWithColor(
                    this.activeTransit.flashColor,
                    this.calcFlashyness(preCount) / 8);

                drawPreCountIndicator(sprites, this.preCountTexture, firstX, i);
                lastCount = i + 1;
            }

            //this.drawNumbers(sprites, lastCount);
        }

        private void drawNumbers(SpriteBatchWrapper sprites, int lastCount) {
            if (this.activeTransit.transitFromEnemy && 0 < lastCount && lastCount <= 4) {
                float deltaT = (float) (this.lastSongTime - this.activeTransit.preCounts[lastCount - 1]);

                float alpha = 1.0f - (deltaT / this.beatTimeInMs);
                float blackAlpha = 2 * alpha - alpha;
                if (0.0f > blackAlpha)
                    blackAlpha = 0.0f;

                sprites.drawTextCentered("" + lastCount, "hud/bignumbers", 1.1f, Color.Black * blackAlpha);
                sprites.drawTextCentered("" + lastCount, "hud/bignumbers", 1.0f, Color.Orange * (alpha * 2.0f));
            }
        }

        private void drawPreCountIndicator(SpriteBatchWrapper sprites, Texture2D texture, int firstX, int xOffset) {
            sprites.drawFromCenter(
                texture,
                CIRCLE_SIZE, CIRCLE_SIZE,
                firstX + (DRAW_GAP_FOR_PRECOUNT * xOffset), -150);
        }

        private int getFirstPreCountX() {
            return -1 * (DRAW_GAP_FOR_PRECOUNT * this.activeTransit.preCounts.Length / 2);
        }

    }
}
