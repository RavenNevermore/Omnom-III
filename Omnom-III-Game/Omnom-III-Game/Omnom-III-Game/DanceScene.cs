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


namespace Omnom_III_Game {
    public class DanceScene {

        Dictionary<String, Texture2D> textures;
        InputState latestInput;

        public void initialize(ContentUtil content) {
            this.textures = new Dictionary<String, Texture2D>();

            loadTextures(content, "player_character", "up", "down", "left", "right");
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
        }

        public void draw(SpriteBatchWrapper sprites, Rectangle viewport) {
            sprites.drawFromCenter(this.textures["player_character"], viewport, 50, 150);

            sprites.drawFromCenter(this.textures["up"], viewport, 30, 30, 0, -100, 
                getStateColor(InputState.Move.UP));
            sprites.drawFromCenter(this.textures["down"], viewport, 30, 30, 0, 100,
                getStateColor(InputState.Move.DOWN));

            sprites.drawFromCenter(this.textures["left"], viewport, 30, 30, -100, 0,
                getStateColor(InputState.Move.LEFT));
            sprites.drawFromCenter(this.textures["right"], viewport, 30, 30, 100, 0,
                getStateColor(InputState.Move.RIGHT));
        }

        private Color getStateColor(InputState.Move move) {
            return this.latestInput.isActive(move) ? Color.Green : Color.Gray;
        }
    }
}
