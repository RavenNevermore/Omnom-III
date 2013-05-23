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


namespace Omnom_III_Game {
    public class DanceScene {

        Dictionary<String, Texture2D> textures;
        FMOD.System soundsystem;
        Song song;

        InputState latestInput;
        List<DanceSequence> sequences;
        DanceSequence activeSequence;
        DanceSequence.Input activeSequenceInput;

        PlayerProgress progress;


        public void initialize(ContentUtil content) {
            this.textures = new Dictionary<String, Texture2D>();

            this.loadTextures(content, "player_character", "btn_up", "btn_down", "btn_left", "btn_right");


            this.progress = new PlayerProgress();
            this.sequences = new List<DanceSequence>();
            this.sequences.Add(
                new DanceSequence(2,
                    new DanceSequence.Input(InputState.Move.LEFT, Song.MusicTime.QUARTER),
                    new DanceSequence.Input(InputState.Move.UP, Song.MusicTime.QUARTER),
                    new DanceSequence.Input(InputState.Move.DOWN, Song.MusicTime.QUARTER),
                    new DanceSequence.Input(InputState.Move.RIGHT, Song.MusicTime.QUARTER)));

            this.sequences.Add(
                new DanceSequence(6,
                    new DanceSequence.Input(InputState.Move.UP, Song.MusicTime.QUARTER),
                    new DanceSequence.Input(InputState.Move.LEFT, Song.MusicTime.EIGTH),
                    new DanceSequence.Input(InputState.Move.RIGHT, Song.MusicTime.EIGTH),
                    new DanceSequence.Input(InputState.Move.DOWN, Song.MusicTime.QUARTER),
                    new DanceSequence.Input(InputState.Move.UP, Song.MusicTime.QUARTER)));

            this.createSoundSystem();
            this.song = new Song("eattherich", this.soundsystem, 122.8f);
            this.song.play();
        }

        private void createSoundSystem() {
            uint version = 0;
            FMOD.RESULT result = FMOD.Factory.System_Create(ref soundsystem);
            this.ERRCHECK(result);
            result = soundsystem.getVersion(ref version);
            ERRCHECK(result);
            result = soundsystem.init(32, FMOD.INITFLAGS.NORMAL, (IntPtr)null);
            ERRCHECK(result);
        }

        private void ERRCHECK(FMOD.RESULT result) {
            if (result != FMOD.RESULT.OK) {
                throw new SoundSystemException(result.ToString("X"));
            }
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
            this.latestInput = input;
            this.song.calculateMetaInfo();

            if (null != this.activeSequence && activeSequence.isGone(this.song)) {
                this.activeSequence = null;
                this.activeSequenceInput = null;
            }


            this.sequences.RemoveAll(x => x.isGone(this.song));


            if (null == this.activeSequence && 0 < this.sequences.Count) {
                this.activeSequence = this.sequences.ElementAt(0);
            }

            if (null != this.activeSequence) {
                this.activeSequenceInput = this.activeSequence.nextInput(this.song);
            }
        }

        public void draw(SpriteBatchWrapper sprites, GraphicsDevice device) {
            Rectangle viewport = device.Viewport.Bounds;
            sprites.drawFromCenter(this.textures["player_character"], viewport, 150, 150);

            sprites.drawFromCenter(this.textures["btn_up"], viewport, 30, 30, 0, -100, 
                getStateColor(InputState.Move.UP));
            sprites.drawFromCenter(this.textures["btn_down"], viewport, 30, 30, 0, 100,
                getStateColor(InputState.Move.DOWN));

            sprites.drawFromCenter(this.textures["btn_left"], viewport, 30, 30, -100, 0,
                getStateColor(InputState.Move.LEFT));
            sprites.drawFromCenter(this.textures["btn_right"], viewport, 30, 30, 100, 0,
                getStateColor(InputState.Move.RIGHT));

            int beats = this.song.timeRunningInBeats;
            float positionInBeat = song.positionInBeat;


            
            sprites.drawDebugText("Playback:", this.song.timeRunningInMs, 
                "|", beats, "(", this.song.timeRunningInMeasures, this.song.positionInMeasure, ")\n\rScore:", this.progress.score);

            
            if (0 == (beats - 1) % 4) {
                sprites.fillWithColor(Color.White, (1f - positionInBeat) * .5f);
            } else if (0 == (beats - 1) % 2) {
                sprites.fillWithColor(Color.White, (1f - positionInBeat) * .25f);
            }

            drawDebugSpectrum(sprites, device, viewport);
        }

        private void drawDebugSpectrum(
            SpriteBatchWrapper sprites,
            GraphicsDevice device, 
            Rectangle viewport) {

            drawSpectrum(sprites, device, viewport, this.song.spectrum.left, true, -225);
            drawSpectrum(sprites, device, viewport, this.song.spectrum.right, false, 225);
        }

        private void drawSpectrum(
            SpriteBatchWrapper sprites, 
            GraphicsDevice device, 
            Rectangle viewport, 
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
                        drawScaleDot(sprites, viewport,
                            i % 10 == 0 ? boxTexRed2 : boxTexRed,
                            i, startOffset, j, Color.White);
                    } else if (j <= maxScaledSpectrum[i]) {
                        drawScaleDot(sprites, viewport,
                            i % 10 == 0 ? boxTexGrey2 : boxTexGrey,
                            i, startOffset, j, Color.White);

                    }
                }
            }
        }

        private static void drawScaleDot(
            SpriteBatchWrapper sprites, 
            Rectangle viewport, 
            Texture2D boxTex, 
            int i,
            int startOffset, 
            int j, 
            Color color) {

            sprites.drawFromCenter(
                boxTex,
                viewport,
                5, 5,
                startOffset + 7 * i,
                100 - j * 7,
                color);
        }

        private Color getStateColor(InputState.Move move) {
            if (null != this.activeSequenceInput) {
                return move == this.activeSequenceInput.handicap ? Color.LightBlue : Color.Gray;
            }
            return this.latestInput.isActive(move) ? Color.Green : Color.Gray;
        }
    }
}
