using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Omnom_III_Game.util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.GamerServices;

namespace Omnom_III_Game.highscore {
    class HighscoreScene : IScene {
        private String stageName;

        private PlayerProgress nextScore;
        private bool listeningForInput;

        private HighscoreList scores;
        private bool exit;

        private int initTime;

        public void initialize(ContentUtil content, SceneActivationParameters parameters) {
            HighscoreParams stageParams = (HighscoreParams) parameters.parameters;
            this.stageName = stageParams.stage;
            this.nextScore = stageParams.newScore;
            this.listeningForInput = false;
            
            this.scores = new HighscoreList();
            this.scores.loadForScene(stageName);
            this.exit = false;

            /*if (null == this.nextScore) {
                this.nextScore = new PlayerProgress();
                this.nextScore.score = 4321;
            }*/

            //this.scores.add(new Scoring("Batman", this.nextScore.score, true));
            this.initTime = Environment.TickCount;
        }

        public void update(InputState input) {
            int ticks = Environment.TickCount - this.initTime;
            if (ticks > 1000 && input.isActive(InputState.Control.EXIT)) {
                this.exit = true;
                return;
            }
            if (this.listeningForInput) {
                return;
            }

            if (null != this.nextScore) {
                this.listenForUserName();
            }
        }

        public void listenForUserName() {
            this.listeningForInput = true;
            Guide.BeginShowKeyboardInput(
                PlayerIndex.One,
                "Enter your Name", "You scored " + this.nextScore.score, "",
                this.setUserNameFromInput,
                null);
        }

        private void setUserNameFromInput(IAsyncResult result) {
            String username = Guide.EndShowKeyboardInput(result);
            this.scores.add(new Scoring(username, this.nextScore.score, true));
            this.listeningForInput = false;
            this.nextScore = null;
        }

        public void draw(SpriteBatchWrapper sprites, GraphicsDevice device) {
            sprites.fillWithColor(Color.CornflowerBlue, 1.0f);
            sprites.drawTextCentered(this.stageName + " - Highscore", -9, Color.Black);

            int i = 0;
            foreach (Scoring score in this.scores.getSortedRange(0, 15)) {
                sprites.drawTextCentered(
                    score.name + " ........ " + score.score,
                    -7 + i++,
                    score.isNew ? Color.Black : Color.DarkGray);
            }
        }

        public void cleanup() {
            this.scores.storeForScene(this.stageName);
        }

        public SceneActivationParameters nextScene() {
            return null;
        }

        public bool wantsToExit() {
            return this.exit;
        }
    }
}
