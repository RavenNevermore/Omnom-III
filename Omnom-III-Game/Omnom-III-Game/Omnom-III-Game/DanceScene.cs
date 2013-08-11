using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Omnom_III_Game.util;
using Omnom_III_Game.exceptions;
using Omnom_III_Game.dance;
using Omnom_III_Game.graphics;
using Omnom_III_Game.highscore;


namespace Omnom_III_Game {
    public class DanceScene : IScene {

        
        ContentScript script;
        Song song;
        DanceSequenceProtocol sequences;
        DanceSequence currentSequence;
        PlayerProgress progress;
        
        AnimatedCharacter enemy;
        AnimatedCharacter player;
        
        String backgroundTexture;
        TextureContext textures;

        long lastTime;

        bool exit;
        bool paused;

        InputState.Move lastMove;

        IngameUI ui;
        
        private ExplicitInputState input;

        public DanceScene(String scriptname) {
            this.script = ContentScript.FromFile(scriptname);
            this.textures = new TextureContext();
            this.sequences = new DanceSequenceProtocol();
            
            this.ui = new IngameUI();
        }

        public String title { get { return this.script.title; } }

        public SceneActivationParameters nextScene() {
            if (0 == this.progress.score) {
                return null;
            } else {
                return new SceneActivationParameters(
                    "highscore",
                    new HighscoreParams(this.title, this.progress.clone()));
            }
        }

        public bool wantsToExit() { return exit; }

        public void initialize(ContentUtil content, SceneActivationParameters parameters) {
            this.exit = false;
            this.paused = false;
            this.textures.clear();
            this.currentSequence = null;
            this.input = new ExplicitInputState();

            script.reload();
            this.textures.loadTextures(content, 
                "player_character", "btn_up", "btn_down", "btn_left", "btn_right", "btn_fail");
            
            this.backgroundTexture = this.script.get("background");
            this.textures.loadTextures(content, this.backgroundTexture);

            this.song = new Song(this.script);
            
            
            this.progress = new PlayerProgress();

            this.sequences.initialize(this.script);

            float uiSpeed = 1.0f;
            if (this.script.contains("ui_speed"))
                uiSpeed = this.script.asFloat["ui_speed"][0];
            this.ui.initialize(content, this.sequences, this.song.beatTimeInMs, uiSpeed);
            

            long beatTimeMs = (long) this.song.beatTimeInMs;
            this.enemy = new AnimatedCharacter(this.script.get("enemy"), content, beatTimeMs);
            this.player = new AnimatedCharacter("toasty/toasty", content, beatTimeMs);
            
            this.lastTime = 0;
            this.song.play();
        }

        
        public void update(InputState input) {
            this.input.update(input);
            this.updateInternal(this.input);
        }
            
        private void updateInternal(ExplicitInputState input) {
            if (this.hasExitState(input))
                return;

            updatePauseState(input);

            this.song.calculateMetaInfo();
            long time = this.song.timeRunningInMs;
            long deltaT = time - this.lastTime;
            this.lastTime = time;
            
            float measures = this.song.timeRunningInMeasures;

            this.currentSequence = sequences.atMeasure(measures);
            if (null != this.currentSequence && this.currentSequence.isEnemyActive(measures)) {
                InputState.Move move = this.currentSequence.getActiveMoveAt(measures);
                if (this.lastMove != move)
                    this.enemy.activate(move);
                this.lastMove = move;
            }

            PlayerProgress.RatedMoves rated = null;

            if ((null == this.currentSequence ||
                    this.currentSequence.playerInputAllowed(measures)) &&
                    input.lastMoveIsNew()) {
                
                this.player.activate(input.lastMove);
                rated = this.progress.nextRating(measures, this.currentSequence, input);
            }
            this.ui.update(rated, time, deltaT);


            this.player.update(deltaT);
            this.enemy.update(deltaT);
        }

        private void updatePauseState(ExplicitInputState input) {
            if (input.isActive(InputState.Control.PAUSE)) {
                if (this.paused) {
                    this.song.play();
                } else {
                    this.song.pause();
                }
                this.paused = !this.paused;
            }
        }


        private bool hasExitState(InputState input) {
            if (this.exit) {
                return true;
            }

            if (this.song.stoppedPlaying() ||
                    input.isActive(InputState.Control.EXIT)) {
                exit = true;
                return true;
            }

            return false;
        }

        public void draw(SpriteBatchWrapper sprites, GraphicsDevice device) {
            if (this.exit)
                return;

            this.textures.drawAsBackground(this.backgroundTexture, sprites);

            if (null != this.currentSequence) {
                if (this.currentSequence.isEnemyActive(this.song.timeRunningInMeasures)) {
                    this.enemy.draw(sprites);
                } else {
                    this.player.draw(sprites);
                }
                
            }

            /*sprites.drawDebugText(
                "Measures: ", this.song.timeRunningInMeasures,
                "Current Seq: ", this.currentSequence, "Last Move:", this.lastMove,
                "\n\rScore: ", this.progress.score);*/

            sprites.drawTextAt(
                "Score: " + this.progress.score,
                -225, 15, 1.05f, Color.Black, "hud/ingametext", SpriteBatchWrapper.Direction.CENTER);
            sprites.drawTextAt(
                "Score: " + this.progress.score,
                -225, 15, 1.0f, Color.Orange, "hud/ingametext", SpriteBatchWrapper.Direction.CENTER);
            
            this.ui.draw(sprites);
        }


        public void cleanup() {
            this.progress.reset();
            this.song.stop();
            this.song.reset();
        }
    }
}
