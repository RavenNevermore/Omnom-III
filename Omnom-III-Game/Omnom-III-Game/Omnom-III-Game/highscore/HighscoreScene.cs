using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Omnom_III_Game.util;
using Omnom_III_Game.graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Omnom_III_Game.highscore {
    class HighscoreScene : IScene {
        private String stageName;

        private PlayerProgress nextScore;
        private TextInput nameInput;
        private Sound showInputSound;
        private Sound createSound;

        private HighscoreList scores;
        private bool exit;

        private int initTime;

        private Texture2D background;
        private ScaledTexture lineBackground;

        public void initialize(ContentUtil content, SceneActivationParameters parameters) {
            HighscoreParams stageParams = (HighscoreParams) parameters.parameters;
            this.stageName = stageParams.stage;
            this.nextScore = stageParams.newScore;

            this.showInputSound = new Sound("menu/select", content);
            this.createSound = new Sound("menu/click", content);

            this.background = content.load<Texture2D>(null == stageParams.background ? 
                "bgr_highscore" : stageParams.background);

            this.lineBackground = new ScaledTexture(content.load<Texture2D>(
                null == stageParams.background ? "hud/highscore_line" : stageParams.background+"_line"), 
                .4f);
            
            this.scores = new HighscoreList();
            this.scores.loadForScene(stageName);
            this.exit = false;

            this.nameInput = new TextInput();

            /*if (null == this.nextScore) {
                this.nextScore = new PlayerProgress();
                this.nextScore.score = 4321;
            }*/
            if (null != this.nextScore) {
                this.nameInput.setMessage("You scored "
                        + this.nextScore.score 
                        + ". Please enter your name.");
                this.showInputSound.play();
                this.nameInput.startListening();
            }

            //this.scores.add(new Scoring("Batman", this.nextScore.score, true));
            this.initTime = Environment.TickCount;
        }

        public void update(InputState input) {
            int ticks = Environment.TickCount - this.initTime;
            if (ticks > 1000 && input.isActive(InputState.Control.EXIT)) {
                this.showInputSound.play();
                this.exit = true;
                return;
            }

            if (this.nameInput.hasFinishedListening()){
                this.createSound.play();
                String username = this.nameInput.getResult();
                if (null != username && !"".Equals(username.Trim())) {
                    this.scores.add(
                        new Scoring(username, this.nextScore.score, true));
                }
            }

            this.nameInput.update();
        }

        public void draw(SpriteBatchWrapper sprites, GraphicsDevice device) {
            sprites.drawBackground(this.background);
            sprites.drawTextAt("Highscore: " + this.stageName, 50, 30, 1.0f, Color.White, "hud/ingametext");
            //sprites.drawTextCentered(this.stageName + " - Highscore", -9, Color.Black);

            this.drawScores(sprites);
            this.nameInput.draw(sprites);
        }

        private void drawScores(SpriteBatchWrapper sprites) {
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
