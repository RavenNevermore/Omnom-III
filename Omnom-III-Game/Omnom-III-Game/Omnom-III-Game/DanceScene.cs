using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace Omnom_III_Game {
    class DanceScene {

        Dictionary<String, Texture2D> textures;

        public void initialize(ContentManager content) {
            textures = new Dictionary<String, Texture2D>();

            //textures["player_character"] = content.Load<Texture2D>("missing_texture");
        }

        public void update() {
        }

        public void draw(SpriteBatch sprites, Rectangle viewport) {
            int centerX = viewport.Width / 2;
            int centerY = viewport.Height / 2;

            int characterWidth = 50;
            int characterHeight = 150;
            /*
            sprites.Draw(
                this.textures["player_character"],
                new Rectangle(
                    centerX - characterWidth / 2, 
                    centerY - characterHeight / 2, 
                    characterWidth, 
                    characterHeight),
                Color.White);*/
        }

    }
}
