using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Omnom_III_Game.util {
    public class TextureContext {
        Dictionary<String, Texture2D> textures;

        public TextureContext() {
            this.textures = new Dictionary<string, Texture2D>();
        }

        internal void clear() {
            this.textures.Clear();
        }

        internal void loadTextures(ContentUtil content, params String[] names) {
            foreach (String name in names) {
                if (null == name)
                    continue;
                try {
                    this.textures[name] = content.load<Texture2D>(name);
                } catch (ContentLoadException e) {
                    this.textures[name] = content.load<Texture2D>("missing_texture");
                }
            }
        }

        internal Texture2D getRaw(String name) {
            return  this.textures[name];
        }

        internal void drawAsBackground(String name, SpriteBatchWrapper sprites) {
            if (null != name && this.textures.ContainsKey(name)){
                sprites.drawBackground(this.textures[name]);
            }
        }
    }
}
