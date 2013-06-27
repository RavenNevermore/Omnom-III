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

        public String title {get {return this.protocol.title;}}
        String scriptname;
        Dictionary<String, Texture2D> textures;
        //FMOD.System soundsystem;
        DanceProtocol protocol;
        List<InputState.Move> activePlayerInputs;
        DanceSequence.Input activeSequenceInput;

        bool exit;

        PlayerProgress progress;

        DanceSceneAnimationBundle animations;

        public DanceScene(String scriptname) {
            this.scriptname = scriptname;
            //this.createSoundSystem();
            this.protocol = new DanceProtocol(this.scriptname);
        }

        public String nextScene() { return null; }

        public bool wantsToExit() { return exit; }

        public void initialize(ContentUtil content) {
            this.exit = false;
            this.activePlayerInputs = new List<InputState.Move>();
            this.textures = new Dictionary<String, Texture2D>();

            this.loadTextures(content, "player_character", "btn_up", "btn_down", "btn_left", "btn_right", "btn_fail");
            
            //this.createSoundSystem();
            //this.protocol = new DanceProtocol(this.scriptname, this.soundsystem);
            this.protocol.initialize();

            this.loadTextures(content, this.protocol.enemyTexture);

            this.animations = new DanceSceneAnimationBundle(this.textures, this.protocol.song);
            this.progress = new PlayerProgress();

            
            this.protocol.startPlaying();
        }

        private void loadTextures(ContentUtil content, params String[] names) {
            foreach (String name in names){
                try {
                    this.textures[name] = content.load<Texture2D>(name);
                } catch (ContentLoadException e) {
                    this.textures[name] = content.load<Texture2D>("missing_texture");
                }
            }
        }

        public void update(InputState input) {
            this.protocol.update();
            if (this.hasExitState(input))
                return;
            long time = this.protocol.timeRunning;

            if (this.protocol.isEnemyActive) {
                DanceSequence.Input nextInput = this.protocol.nextSequenceInput();
                if (null != nextInput && !nextInput.Equals(this.activeSequenceInput)) {
                    this.animations.startOpponentAnimation(
                        nextInput.handicap, time);
                }
                this.activeSequenceInput = nextInput;
            }
            if (this.progress.update(time, this.protocol.lastSequence, input.activeStates)) {
                PlayerProgress.Rating rating = this.progress.getActiveMoveRating();
                InputState.Move handicap = this.progress.getActiveHandicap();

                if (PlayerProgress.Rating.MISSED == rating) {
                    this.animations.startFailAnimation(handicap, time);
                } else if (PlayerProgress.Rating.NONE != rating) {
                    this.animations.startPlayerAnimation(handicap, time);
                }
                
            }
            this.progress.cleanup(time);
            
            this.animations.update(this.protocol.timeRunning);

            if (this.progress.isDead()) {
                this.exit = true;
            }
        }

        private bool hasExitState(InputState input) {
            if (this.exit) {
                return true;
            }

            if (this.protocol.stoppedPlaying() ||
                    input.isActive(InputState.Control.EXIT)) {
                exit = true;
                return true;
            }

            return false;
        }

        public void draw(SpriteBatchWrapper sprites, GraphicsDevice device) {
            if (this.exit)
                return;

            String characterTex = this.protocol.isEnemyActive ? 
                this.protocol.enemyTexture : "player_character";
            sprites.drawFromCenter(this.textures[characterTex], 150, 150);
            
            this.animations.draw(sprites);
            int beats = this.protocol.song.timeRunningInBeats;
            float positionInBeat = this.protocol.song.positionInBeat;

            DanceSequence seq = this.protocol.activeSequence;
            
            sprites.drawDebugText("Playback:", this.protocol.timeRunning,
                "|", beats, "(", this.protocol.song.timeRunningInMeasures, this.protocol.song.positionInMeasure, 
                ")\n\rScore:", this.progress.score, "  Lives: ", this.progress.lifes, "  Active Move: ", this.progress.activeMove, 
                "\n\rSequences:", this.protocol.numberOfSequences, null == seq ? -1 : seq.startPosition, null == seq ? -1 : seq.endPosition,
                "\n\rPos in Active Sequence:", this.protocol.activeSequencePlayPosition);

            
            if (0 == (beats - 1) % 4) {
                sprites.fillWithColor(Color.White, (1f - positionInBeat) * .125f);
            } else if (0 == (beats - 1) % 2) {
                sprites.fillWithColor(Color.White, (1f - positionInBeat) * .075f);
            }

            drawDebugSpectrum(sprites, device);
        }


        public void cleanup() {
            this.protocol.stop();
            this.protocol.reset();
            this.progress.reset();
        }
        
        private void drawDebugSpectrum(
            SpriteBatchWrapper sprites,
            GraphicsDevice device) {

            drawSpectrum(sprites, device, this.protocol.song.spectrum.left, true, -225);
            drawSpectrum(sprites, device, this.protocol.song.spectrum.right, false, 225);
        }

        private void drawSpectrum(
            SpriteBatchWrapper sprites, 
            GraphicsDevice device, 
            Song.SpectralData spectrum,
            bool flipDirection,
            int xOffset) {

            Texture2D boxTexRed = DrawingUtil.createTexture(device, Color.Red);
            Texture2D boxTexRed2 = DrawingUtil.createTexture(device, Color.DarkRed);
            Texture2D boxTexGrey = DrawingUtil.createTexture(device, Color.LightGray);
            Texture2D boxTexGrey2 = DrawingUtil.createTexture(device, Color.DarkGray);

            float spectralScale = 30;
            float[] scaledSpectrum = spectrum.current.highCut(32).downSample(10).scaledData(spectralScale);
            float[] maxScaledSpectrum = spectrum.max.highCut(32).downSample(10).scaledData(spectralScale);


            int startOffset = xOffset;// -7 * scaledSpectrum.Length;
            if (flipDirection) {
                startOffset -= 7 * scaledSpectrum.Length;
                Array.Reverse(scaledSpectrum);
                Array.Reverse(maxScaledSpectrum);
            }

            for (int i = 0; i < scaledSpectrum.Length; i++) {
                
                float scaledVolume = scaledSpectrum[i];// *spectralScale;

                for (int j = 0; j < spectralScale; j++) {
                    if (j <= scaledVolume) {
                        drawScaleDot(sprites,
                            i % 10 == 0 ? boxTexRed2 : boxTexRed,
                            i, startOffset, j, Color.White);
                    } else if (j <= maxScaledSpectrum[i]) {
                        drawScaleDot(sprites,
                            i % 10 == 0 ? boxTexGrey2 : boxTexGrey,
                            i, startOffset, j, Color.White);

                    }
                }
            }
        }

        private static void drawScaleDot(
            SpriteBatchWrapper sprites, 
            Texture2D boxTex, 
            int i,
            int startOffset, 
            int j, 
            Color color) {

            sprites.drawFromCenter(
                boxTex,
                5, 5,
                startOffset + 7 * i,
                100 - j * 7,
                color);
        }
    }
}
