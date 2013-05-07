using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Omnom_III_Game.util;


namespace Omnom_III_Game {
    public class DanceScene {

        Dictionary<String, Texture2D> textures;

        public void initialize(ContentUtil content) {
            this.textures = new Dictionary<String, Texture2D>();

            this.textures["player_character"] = content.load<Texture2D>("missing_texture");
        }

        public void update() {
        }

        public void draw(SpriteBatch sprites, Rectangle viewport) {

            int characterWidth = 50;
            int characterHeight = 150;

            int centerX = viewport.Width / 2;
            int centerY = viewport.Height / 2;

            sprites.Draw(
                this.textures["player_character"],
                new Rectangle(
                    centerX - characterWidth / 2, 
                    centerY - characterHeight / 2, 
                    characterWidth, 
                    characterHeight),
                Color.White);
        }

    }
}
