using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Omnom_III_Game.util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Omnom_III_Game.highscore {
    class HighscoreScene : IScene {
        private String stageName;

        private PlayerProgress nextScore;
        private String nextName;
        private bool listeningForInput;

        private List<Scoring> scores;
        private bool exit;

        private int initTime;

        public void initialize(ContentUtil content, SceneActivationParameters parameters) {
            HighscoreParams stageParams = (HighscoreParams) parameters.parameters;
            this.stageName = stageParams.stage;
            this.nextScore = stageParams.newScore;
            this.nextName = null;
            this.scores = new List<Scoring>();
            this.exit = false;

            if (null == this.nextScore) {
                this.nextScore = new PlayerProgress();
                this.nextScore.score = 4321;
                this.listeningForInput = true;
            }

            this.scores.Add(new Scoring("Batman", this.nextScore.score, true));

            Random r = new Random();
            int max = 5000;
            this.scores.Add(new Scoring("Foo", r.Next(max)));
            this.scores.Add(new Scoring("Bar", r.Next(max)));
            this.scores.Add(new Scoring("Baz", r.Next(max)));
            this.scores.Add(new Scoring("Bear", r.Next(max)));
            this.scores.Add(new Scoring("Shark", r.Next(max)));

            this.scores.Add(new Scoring("Flash", r.Next(max)));
            this.scores.Add(new Scoring("Foo Man Sho", r.Next(max)));
            this.scores.Add(new Scoring("Goldfinger", r.Next(max)));
            this.scores.Add(new Scoring("Starsky", r.Next(max)));
            this.scores.Add(new Scoring("Hutch", r.Next(max)));

            this.scores.Add(new Scoring("Kim Jong Ill", r.Next(max)));
            this.scores.Add(new Scoring("Adelheim", r.Next(max)));
            this.scores.Add(new Scoring("Uschi", r.Next(max)));
            this.scores.Add(new Scoring("Bollocks", r.Next(max)));


            this.scores.Add(new Scoring("Starscream", r.Next(max)));
            this.scores.Add(new Scoring("Icecreme", r.Next(max)));
            this.scores.Add(new Scoring("Dalek", r.Next(max)));
            this.scores.Add(new Scoring("The Dr.", r.Next(max)));
            this.scores.Add(new Scoring("Kirk", r.Next(max)));

            Scoring.sort(this.scores);

            this.initTime = Environment.TickCount;
        }

        public void update(InputState input) {
            int ticks = Environment.TickCount - this.initTime;
            if (ticks > 1000 && input.isActive(InputState.Control.EXIT)) {
                this.exit = true;
                return;
            }


        }

        public void draw(SpriteBatchWrapper sprites, GraphicsDevice device) {
            sprites.fillWithColor(Color.CornflowerBlue, 1.0f);
            sprites.drawTextCentered(this.stageName + " - Highscore", -9, Color.Black);

            for (int i = 0; i < 15 && i < this.scores.Count; i++) {
                Scoring score = this.scores.ElementAt(i);
                sprites.drawTextCentered(
                    score.name + " ........ " + score.score, 
                    -7 + i, 
                    score.isNew ? Color.Black : Color.DarkGray);
            }
        }

        public void cleanup() {
        }

        public SceneActivationParameters nextScene() {
            return null;
        }

        public bool wantsToExit() {
            return this.exit;
        }
    }
}
