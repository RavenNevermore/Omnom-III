using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Omnom_III_Game.util;
using Microsoft.Xna.Framework.Graphics;

namespace Omnom_III_Game {
    public interface IScene {
        void initialize(ContentUtil content);
        void update(InputState input);
        void draw(SpriteBatchWrapper sprites, GraphicsDevice device);
        void cleanup();

        String nextScene();
        bool wantsToExit();
    }
}
