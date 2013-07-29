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

        InputState.Move lastMove;

        DanceSceneAnimationBundle animations;
        VisualTransition transitions;

        public DanceScene(String scriptname) {
            this.script = ContentScript.FromFile(scriptname);
            this.textures = new TextureContext();
            this.sequences = new DanceSequenceProtocol();
            this.transitions = new VisualTransition();
        }

        public String title { get { return this.script.title; } }

        public SceneActivationParameters nextScene() { 
            return new SceneActivationParameters(
                "highscore",
                new HighscoreParams(this.title, this.progress.clone())); 
        }

        public bool wantsToExit() { return exit; }

        public void initialize(ContentUtil content, SceneActivationParameters parameters) {
            this.exit = false;
            this.textures.clear();
            this.currentSequence = null;

            script.reload();
            this.textures.loadTextures(content, 
                "player_character", "btn_up", "btn_down", "btn_left", "btn_right", "btn_fail");
            
            this.backgroundTexture = this.script.get("background");
            this.textures.loadTextures(content, this.backgroundTexture);

            this.song = new Song(this.script);
            
            this.animations = new DanceSceneAnimationBundle(this.textures, this.song);
            this.progress = new PlayerProgress();

            this.initOpponent();

            this.initTransitions(content);


            long beatTimeMs = (long) this.song.beatTimeInMs;
            this.enemy = new AnimatedCharacter(this.script.get("enemy"), content, beatTimeMs);
            this.player = new AnimatedCharacter("toasty/toasty", content, beatTimeMs);
            
            this.lastTime = 0;
            this.song.play();
        }

        private void initTransitions(ContentUtil content) {
            this.transitions.initialize(content, this.song.beatTimeInMs);
            foreach (DanceSequence sequence in this.sequences.asList()) {
                this.transitions.addTransitPoint(
                    sequence.startMeasure + sequence.length);
            }
        }

        private void initOpponent() {
            this.sequences.initialize(this.script);
            foreach (DanceSequence.Input handicap in this.sequences.handicaps) {
                this.animations.startOpponentAnimation(
                    handicap.handicap,
                    handicap.startTime(song.bpms));
            }
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
            if (null != this.currentSequence && this.currentSequence.isEnemyActive(measures)) {
                InputState.Move move = this.currentSequence.getActiveMoveAt(measures);
                if (this.lastMove != move)
                    this.enemy.activate(move);
                this.lastMove = move;
            }

            //if (!input.lastMove.Equals(InputState.Move.BREAK)) {
            if (input.lastMoveIsNew()) {
                this.player.activate(input.lastMove);
            }
            updatePlayerRating(input, time, measures);
            
            this.animations.update(time);
            this.player.update(deltaT);
            this.enemy.update(deltaT);
            this.transitions.update(time);
        }

        private void updatePlayerRating(InputState input, long time, float measures) {
            PlayerProgress.RatedMoves rated = this.progress.nextRating(measures, this.currentSequence, input);
            foreach (DanceSequence.Input move in rated.ok) {
                this.animations.startPlayerAnimation(move.handicap, time, PlayerProgress.Rating.OK);
            }
            foreach (DanceSequence.Input move in rated.good) {
                this.animations.startPlayerAnimation(move.handicap, time, PlayerProgress.Rating.GOOD);
            }
            foreach (DanceSequence.Input move in rated.perfect) {
                this.animations.startPlayerAnimation(move.handicap, time, PlayerProgress.Rating.PERFECT);
            }
            foreach (DanceSequence.Input move in rated.missed) {
                this.animations.startFailAnimation(move.handicap, time);
            }
            foreach (DanceSequence.Input move in rated.wrong) {
                this.animations.startFailAnimation(move.handicap, time);
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
                    //sprites.drawFromCenter(this.textures.getRaw("player_character"), 350, 350, 0, 50);
                }
                
            }

            sprites.drawDebugText(
                "Measures: ", this.song.timeRunningInMeasures,
                "Current Seq: ", this.currentSequence, "Last Move:", this.lastMove,
                "\n\rScore: ", this.progress.score);
            
            this.animations.draw(sprites);
            this.transitions.draw(sprites);
        }


        public void cleanup() {
            this.progress.reset();
            this.song.stop();
            this.song.reset();
        }
    }
}
