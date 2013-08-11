using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Omnom_III_Game.util;
using Omnom_III_Game.dance;

namespace Omnom_III_Game.graphics {
    public class DirectionalIndicator {
        static Vector2 FREEDOM = new Vector2(250f, 100f);
        static int CENTER_OFFSET = 180;
        static float TEXTURE_SCALE = .55f;

        class RecordedStart {
            public RecordedStart(Color color, long time) {
                this.color = color;
                this.time = time;
            }

            public Color color;
            public long time;
        }

        InputState.Move move;
        ScaledTexture texture;
        Vector2 direction;
        float beatTimeInMs;
        float travelSpeed;
        Color color;

        float travel;
        bool isMoving;

        List<RecordedStart> recordings;


        public DirectionalIndicator(
                ContentUtil content, 
                InputState.Move move,
                float beatTimeInMs,
                float travelSpeed) {

            this.recordings = new List<RecordedStart>();
            this.color = Color.White;
            this.move = move;
            this.beatTimeInMs = beatTimeInMs;
            this.travelSpeed = travelSpeed;
            String texName = "hud/arrow_" + move.ToString().ToLower();
            this.texture = new ScaledTexture(
                content.load<Texture2D>(texName), TEXTURE_SCALE);
            switch (move) {
                case InputState.Move.UP:
                    this.direction = new Vector2(0, 1);
                    break;
                case InputState.Move.DOWN:
                    this.direction = new Vector2(0, -1);
                    break;
                case InputState.Move.LEFT:
                    this.direction = new Vector2(1, 0);
                    break;
                case InputState.Move.RIGHT:
                    this.direction = new Vector2(-1, 0);
                    break;
            }

            this.isMoving = false;
        }

        public void prerecord(long time, Color color) {
            recordings.Add(new RecordedStart(color, time));

            recordings = recordings.OrderBy(x => x.time).ToList();
        }

        public void start(Color color) {
            if (this.isMoving)
                return;
            this.restart(color);
        }
        private void restart(Color color) {
            this.color = color;
            this.travel = 0f;
            this.isMoving = true;
        }

        public void update(long time, long deltaT) {
            this.updateRecoordings(time);

            this.updateMovement(deltaT);
        }

        private void updateRecoordings(long time) {
            if (this.recordings.Count > 0) {
                RecordedStart rec = this.recordings.ElementAt(0);
                if (rec.time <= time) {
                    this.recordings.RemoveAt(0);
                    this.restart(rec.color);

                    this.updateMovement(time - rec.time);
                }
            }
        }

        private void updateMovement(long deltaT) {
            if (!this.isMoving)
                return;

            //float normalizedTravel = this.beatTimeInMs * (deltaT / 1000f);
            float normalizedTravel = deltaT / this.beatTimeInMs;
            this.travel += normalizedTravel * 2f * travelSpeed;

            if (this.travel > 1) {
                this.travel = 1;
                this.isMoving = false;
            }
        }

        public void draw(SpriteBatchWrapper sprites) {
            if (!this.isMoving)
                return;

            //TODO add a fade in, for negative travel values

            Vector2 startingPosition = this.direction * CENTER_OFFSET;
            Vector2 position = this.direction * this.travel;
            position *= DirectionalIndicator.FREEDOM;
            position += startingPosition;

            int undergroth = 10 + (int)(this.travel * 50);
            sprites.drawFromCenter(
                this.texture.texture,
                this.texture.Width + undergroth,
                this.texture.Height + undergroth,
                (int) position.X * -1,
                (int) position.Y * -1,
                Color.Black * (1f - this.travel));

            sprites.drawFromCenter(
                this.texture.texture,
                this.texture.Width,
                this.texture.Height,
                (int) position.X * -1,
                (int) position.Y * -1,
                this.color);
        }
    }
}
