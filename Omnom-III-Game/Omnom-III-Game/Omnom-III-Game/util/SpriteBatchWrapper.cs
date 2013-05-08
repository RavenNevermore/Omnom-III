using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Omnom_III_Game.util {
    public class SpriteBatchWrapper {

        private SpriteBatch wrapped;

        public SpriteBatchWrapper() { }
        public SpriteBatchWrapper(SpriteBatch wrapped) {
            this.wrapped = wrapped;
        }

        public virtual void Draw(Texture2D texture2D, Rectangle rectangle, Color color) {
            this.wrapped.Draw(texture2D, rectangle, color);
        }
    }
}
