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

        static int DRAW_GAP_FOR_PRECOUNT = 25;

        class TransitPoint : IComparable<TransitPoint> {
            public long[] preCounts;
            public long transition;
            public VisualTransition parent;

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


        public void initialize(ContentUtil content, float beatTimeInMs) {
            this.preCountTexture = content.load<Texture2D>("circle");

            this.transitPoints = new List<TransitPoint>();
            this.beatTimeInMs = beatTimeInMs;
            this.flashTime = (long) (beatTimeInMs / 2);

            this.activeTransit = null;
            this.lastSongTime = 0;
        }

        public void addTransitPoint(float timeInSong) {
            float measureTimeInMs = this.beatTimeInMs * 4;

            TransitPoint transit = new TransitPoint();
            transit.parent = this;
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

            this.drawPreCount(sprites);

            this.drawFlash(sprites);
        }

        private void drawFlash(SpriteBatchWrapper sprites) {
            if (this.lastSongTime >= this.activeTransit.transition
                    && this.lastSongTime <= this.activeTransit.transition + this.flashTime) {

                float flashyness = ((float) (this.lastSongTime - this.activeTransit.transition)) / ((float) this.flashTime);
                sprites.fillWithColor(Color.LightGreen, 1.0f - flashyness);
            }
        }

        private void drawPreCount(SpriteBatchWrapper sprites) {
            int firstX = this.getFirstPreCountX();
            int i = 0;
            foreach (long preCount in this.activeTransit.preCounts) {
                if (preCount > this.lastSongTime)
                    break;

                sprites.drawFromCenter(
                    this.preCountTexture,
                    16, 16,
                    firstX + (DRAW_GAP_FOR_PRECOUNT * i), -150);

                i++;
            }
        }

        private int getFirstPreCountX() {
            return -1 * (DRAW_GAP_FOR_PRECOUNT * this.activeTransit.preCounts.Length / 2);
        }

    }
}
