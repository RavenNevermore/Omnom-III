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


namespace Omnom_III_Game {
    public class DanceScene : IScene {

        
        ContentScript script;
        Song song;
        DanceSequenceProtocol sequences;
        DanceSequence currentSequence;
        PlayerProgress progress;
        
        AnimatedCharacter enemy;
        
        String backgroundTexture;
        TextureContext textures;

        long lastTime;

        bool exit;

        InputState.Move lastMove;

        DanceSceneAnimationBundle animations;

        public DanceScene(String scriptname) {
            this.script = ContentScript.FromFile(scriptname);
            this.textures = new TextureContext();
            this.sequences = new DanceSequenceProtocol();
        }

        public String title { get { return this.script.title; } }

        public String nextScene() { return null; }

        public bool wantsToExit() { return exit; }

        public void initialize(ContentUtil content) {
            this.exit = false;
            this.textures.clear();
            this.currentSequence = null;

            script.reload();
            this.textures.loadTextures(content, 
                "player_character", "btn_up", "btn_down", "btn_left", "btn_right", "btn_fail");

            String enemyTextureName = this.script.get("enemy");
            //this.textures.loadTextures(content, enemyTextureName+"_idle");

            this.backgroundTexture = this.script.get("background");
            this.textures.loadTextures(content, 
                //this.enemyTexture,
                this.backgroundTexture);

            this.song = new Song(this.script);
            this.sequences.initialize(this.script);
            this.animations = new DanceSceneAnimationBundle(this.textures, this.song);
            this.progress = new PlayerProgress();

            foreach (DanceSequence.Input handicap in this.sequences.handicaps) {
                this.animations.startOpponentAnimation(
                    handicap.handicap,
                    handicap.startTime(song.bpms));
            }

            this.enemy = new AnimatedCharacter(enemyTextureName, content, (long)this.song.beatTimeInMs);
            //this.enemyTexture = new AnimatedTexture(textures.getRaw(enemyTextureName + "_idle"), (long)(this.song.beatTimeInMs));
            //this.enemyTexture.reset();
            this.lastTime = 0;
            this.song.play();
        }

        public void update(InputState input) {
            if (this.hasExitState(input))
                return;

            this.song.calculateMetaInfo();
            long time = this.song.timeRunningInMs;
            long deltaT = time - this.lastTime;
            this.lastTime = time;
            
            float measures = this.song.timeRunningInMeasures;

            this.currentSequence = sequences.atMeasure(measures);
            if (this.currentSequence.isEnemyActive(measures)) {
                InputState.Move move = this.currentSequence.getActiveMoveAt(measures);
                if (this.lastMove != move)
                    this.enemy.activate(move);
                this.lastMove = move;
            }

            PlayerProgress.RatedMoves rated = this.progress.nextRating(measures, this.currentSequence, input);
            foreach (DanceSequence.Input move in rated.good) {
                this.animations.startPlayerAnimation(move.handicap, time);
            }
            foreach (DanceSequence.Input move in rated.missed) {
                this.animations.startFailAnimation(move.handicap, time);
            }
            foreach (DanceSequence.Input move in rated.wrong) {
                this.animations.startFailAnimation(move.handicap, time);
            }
            
            
            this.animations.update(time);
            this.enemy.update(deltaT);
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
                    sprites.drawFromCenter(this.textures.getRaw("player_character"), 350, 350, 0, 50);
                }
                
            }

            sprites.drawDebugText(
                "Measures: ", this.song.timeRunningInMeasures,
                "Current Seq: ", this.currentSequence, "Last Move:", this.lastMove,
                "\n\rScore: ", this.progress.score);
            
            this.animations.draw(sprites);
            
        }


        public void cleanup() {
            this.progress.reset();
            this.song.stop();
            this.song.reset();
        }
    }
}
