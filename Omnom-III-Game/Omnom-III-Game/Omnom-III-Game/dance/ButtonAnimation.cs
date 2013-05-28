using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Omnom_III_Game.util;

namespace Omnom_III_Game.dance {
    public class ButtonAnimation {
        static int  DefaultBoxSize = 30;

        class State {
            public long startPoint;
            public float alpha;
            public float scale;

            public State(long startPoint) {
                this.startPoint = startPoint;
                this.alpha = 1.0f;
                this.scale = 1.0f;
            }
        }

        private Texture2D texture;
        private Color color;
        private Vector2 centerOffset;
        private long length;

        private Queue<long> startPoints;

        private State current;

        public ButtonAnimation(Texture2D texture, Vector2 centerOffset, Color color, long length) {
            this.texture = texture;
            this.centerOffset = centerOffset;
            this.color = color;
            this.length = length;

            this.startPoints = new Queue<long>();
        }

        public void clearStartPoints() {
            this.startPoints.Clear();
        }

        public int getNumberofStartPoints() {
            return this.startPoints.Count;
        }

        public void addStartPoint(long time) {
            this.startPoints.Enqueue(time);
        }

        public void update(long time){
            if (this.startPoints.Count > 0 && this.startPoints.Peek() < time) {
                this.current = new State(
                    this.startPoints.Dequeue());
            }

            if (null != this.current) {
                if (this.current.startPoint + this.length < time) {
                    this.current = null;
                } else {
                    float delta_lt = this.length - (time - this.current.startPoint);
                    this.current.alpha = delta_lt / (float) this.length;
                    this.current.scale = 2f - this.current.alpha;
                }
            }
        }

        public void draw(SpriteBatchWrapper sprites) {
            if (null == current) {
                return;
            } else {
                sprites.drawFromCenter(
                    this.texture,
                    this.getScaledSize(),
                    this.getScaledSize(), 
                    (int)this.centerOffset.X, 
                    (int)this.centerOffset.Y, 
                    this.color * this.current.alpha);
            }
        }

        private int getScaledSize() {
            return (int) (DefaultBoxSize * this.current.scale);
        }
    }
}
