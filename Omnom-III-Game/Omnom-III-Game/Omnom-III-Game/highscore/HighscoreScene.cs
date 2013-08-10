using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Omnom_III_Game.util;
using Omnom_III_Game.graphics;
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

        private Texture2D background;
        private ScaledTexture lineBackground;

        public void initialize(ContentUtil content, SceneActivationParameters parameters) {
            HighscoreParams stageParams = (HighscoreParams) parameters.parameters;
            this.stageName = stageParams.stage;
            this.nextScore = stageParams.newScore;
            this.listeningForInput = false;

            this.background = content.load<Texture2D>("bgr_highscore");
            this.lineBackground = new ScaledTexture(content.load<Texture2D>("hud/highscore_line"), .4f);
            
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
            sprites.drawBackground(this.background);
            sprites.drawTextAt("Highscore: " + this.stageName, 50, 30, 1.0f, Color.White, "hud/ingametext");
            //sprites.drawTextCentered(this.stageName + " - Highscore", -9, Color.Black);

            int i = 0;
            foreach (Scoring score in this.scores.getSortedRange(0, 8)) {
                int baseline = 120 + (60 * i++);

                sprites.drawTextureAt(
                    this.lineBackground.texture,
                    this.lineBackground.Width,
                    this.lineBackground.Height,
                    220, baseline);

                Color textColor = score.isNew ? Color.AliceBlue : Color.Black;
                int textbase = baseline + 18;

                sprites.drawTextAt("" + i, 270, textbase, 1.0f, textColor, 
                    "hud/highscoreline", SpriteBatchWrapper.Direction.RIGHT);

                sprites.drawTextAt(score.name, 300, textbase, 1.0f, textColor,
                    "hud/highscoreline", SpriteBatchWrapper.Direction.LEFT);

                sprites.drawTextAt("" + score.score, 970, textbase, 1.0f, textColor,
                    "hud/highscoreline", SpriteBatchWrapper.Direction.RIGHT);
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
