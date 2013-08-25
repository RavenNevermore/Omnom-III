using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Omnom_III_Game.util {
    public class TextInput {

        static string INPUT_FONT_NAME = "hud/userinput";
        static string MESSAGE_FONT_NAME = "hud/message";

        string message;
        bool listeningStarted;
        bool listening;
        string text;
        Timer blickTimer;
        bool showCursor;
        bool ignoreInputs;

        Keys[] markedForInput;

        public TextInput() {
            this.blickTimer = new Timer();
        }

        public void setMessage(String message) {
            this.message = message;
        }

        public void startListening() {
            this.text = "";
            this.listening = true;
            this.listeningStarted = true;
            this.ignoreInputs = true;
            this.markedForInput = null;
        }

        public void update() {
            if (!this.listening)
                return;

            if (this.blickTimer.timeInSeconds >= .5f) {
                this.ignoreInputs = false;
                this.blickTimer.restart();
                this.showCursor = !this.showCursor;
            }

            if (this.ignoreInputs) {
                return;
            }

            if (null != this.markedForInput) {
                foreach (Keys key in this.markedForInput) {
                    if (Keyboard.GetState().IsKeyUp(key)) {
                        if (Keys.Enter == key) {
                            this.listening = false;
                            return;
                        } else if (Keys.Escape == key) {
                            this.text = null;
                            this.listening = false;
                        } else if (Keys.Back == key) {
                            if (null != this.text && this.text.Length > 0) {
                                this.text = this.text.Substring(0, this.text.Length - 1);
                            }
                        } else {
                            String keyTxt = this.getTextFromKey(key);
                            if (null != keyTxt) {
                                this.text += keyTxt;
                            }
                        }
                    }
                }
            }

            this.markedForInput = Keyboard.GetState().GetPressedKeys();
        }

        private string getTextFromKey(Keys key) {
            int ascii = (int) key;
            if ((48 <= ascii && 57 >= ascii) ||
                    (65 <= ascii && 90 >= ascii) ||
                    (97 <= ascii && 122 >= ascii) ||
                    95 == ascii || 32 == ascii) {

                return ("" + (char) ascii).ToLower();
            }
            return null;
        }

        public void draw(SpriteBatchWrapper sprites) {
            if (!this.listening)
                return;

            this.drawGlassPane(sprites);

            Vector2 center = sprites.getCenterOfScreen();
            int boxWidth = 500;
            int boxHeight = 150;
            int boxX = (int)(center.X - (boxWidth / 2));
            int boxY = (int)(center.Y - (boxHeight / 2));

            this.drawBox(sprites, boxWidth, boxHeight, boxX, boxY);

            if (null != this.message) {
                int msgHeight = sprites.getHeightOfText(this.message, 1.0f, MESSAGE_FONT_NAME);
                int msgWidth = sprites.getWidthOfText(this.message, 1.0f, MESSAGE_FONT_NAME);
                int msgY = boxY + (boxHeight / 4) - (msgHeight / 2);
                int msgX = (int) center.X - (msgWidth / 2);

                sprites.drawTextAt(this.message, msgX, msgY, 1.0f, Color.LightGray, MESSAGE_FONT_NAME);
            }

            this.drawTextBox(sprites, center, boxWidth);
        }

        private void drawTextBox(SpriteBatchWrapper sprites, Vector2 center, int boxWidth) {
            int textboxWidth = (boxWidth * 8) / 10;
            int textHeight = sprites.getHeightOfText("Foo", 1.0f, INPUT_FONT_NAME);
            int textBoxHeight = textHeight + (textHeight / 5);
            int textboxX = (int) center.X - ((boxWidth * 4) / 10);
            int textX = textboxX + 15;
            int textboxY = (int) center.Y;
            int textY = textboxY + (textHeight / 10);
            int visibleChars = 32;
            
            sprites.drawColorAt(Color.Black, 1.0f, textboxWidth, textBoxHeight, textboxX, textboxY);

            String textToDraw = this.text;
            
            bool cutoff = this.text.Length > visibleChars;
            if (cutoff) {
                textToDraw = this.text.Substring(
                    this.text.Length - visibleChars);
                sprites.drawColorAt(Color.Orange, 1.0f, 5, textBoxHeight, textboxX, textboxY);
            }

            if (this.showCursor) {
                textToDraw += "_";
            }
            sprites.drawTextAt(textToDraw, textX, textY, 1.0f, Color.Orange, INPUT_FONT_NAME);
        }

        private void drawBox(SpriteBatchWrapper sprites, int boxWidth, int boxHeight, int boxX, int boxY) {
            sprites.drawColorAt(new Color(.2f, .2f, .2f), 1.0f,
                boxWidth, boxHeight,
                boxX, boxY);
        }

        private void drawGlassPane(SpriteBatchWrapper sprites) {
            sprites.fillWithColor(Color.Black, .5f);
        }

        public bool hasFinishedListening() {
            if (this.listeningStarted && !this.listening) {
                this.listeningStarted = false;
                return true;
            }
            return false;
        }

        public String getResult() {
            return this.text;
        }

    }
}
