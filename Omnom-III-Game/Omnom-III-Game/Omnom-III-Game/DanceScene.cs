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

        public void initialize(ContentUtil content) {
            this.textures = new Dictionary<String, Texture2D>();

            this.loadTextures(content, "player_character", "up", "down", "left", "right");

            

            this.createSoundSystem();
            this.song = new Song("eattherich", this.soundsystem);
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
        }

        public void draw(SpriteBatchWrapper sprites, GraphicsDevice device) {
            Rectangle viewport = device.Viewport.Bounds;
            sprites.drawFromCenter(this.textures["player_character"], viewport, 50, 150);

            sprites.drawFromCenter(this.textures["up"], viewport, 30, 30, 0, -100, 
                getStateColor(InputState.Move.UP));
            sprites.drawFromCenter(this.textures["down"], viewport, 30, 30, 0, 100,
                getStateColor(InputState.Move.DOWN));

            sprites.drawFromCenter(this.textures["left"], viewport, 30, 30, -100, 0,
                getStateColor(InputState.Move.LEFT));
            sprites.drawFromCenter(this.textures["right"], viewport, 30, 30, 100, 0,
                getStateColor(InputState.Move.RIGHT));


            drawDebugSpectrum(sprites, device, viewport);
        }

        private void drawDebugSpectrum(
            SpriteBatchWrapper sprites,
            GraphicsDevice device, 
            Rectangle viewport) {

            drawSpectrum(sprites, device, viewport, this.song.spectrum.left, true, -25);
            drawSpectrum(sprites, device, viewport, this.song.spectrum.right, false, 25);
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
            return this.latestInput.isActive(move) ? Color.Green : Color.Gray;
        }
    }
}
