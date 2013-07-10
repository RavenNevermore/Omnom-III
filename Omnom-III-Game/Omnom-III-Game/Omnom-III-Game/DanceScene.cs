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


namespace Omnom_III_Game {
    public class DanceScene : IScene {

        
        ContentScript script;
        Song song;
        DanceSequenceProtocol sequences;
        PlayerProgress progress;


        TextureContext textures;


        bool exit;

        

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

            script.reload();
            this.textures.loadTextures(content, 
                "player_character", "btn_up", "btn_down", "btn_left", "btn_right", "btn_fail");
            this.textures.loadTextures(content, 
                this.script.get("enemy"),
                this.script.get("background"));

            this.song = new Song(this.script);
            this.sequences.initialize(this.script);
            this.animations = new DanceSceneAnimationBundle(this.textures, this.song);
            this.progress = new PlayerProgress();

            foreach (DanceSequence.Input handicap in this.sequences.handicaps) {
                this.animations.startOpponentAnimation(
                    handicap.handicap,
                    handicap.startTime(song.bpms));
            }

            this.song.play();
        }

        public void update(InputState input) {
            if (this.hasExitState(input))
                return;

            this.song.calculateMetaInfo();
            long time = this.song.timeRunningInMs;
            
            float measures = this.song.timeRunningInMeasures;

            DanceSequence currentSequence = sequences.atMeasure((int)measures);
            if (null != currentSequence && currentSequence.playerInputAllowed(measures)) {
                foreach (InputState.Move move in input.activeStates) {
                    this.animations.startPlayerAnimation(move, time);
                }
            }
            
            
            this.animations.update(time);
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

            this.textures.drawAsBackground(this.script.get("background"), sprites);

            //String characterTex = this.protocol.isEnemyActive ? 
            //    this.protocol.enemyTexture : "player_character";
            //sprites.drawFromCenter(this.textures[characterTex], 350, 350, 0, 50);
            
            this.animations.draw(sprites);
            
        }


        public void cleanup() {
            this.progress.reset();
            this.song.stop();
            this.song.reset();
        }
    }
}
