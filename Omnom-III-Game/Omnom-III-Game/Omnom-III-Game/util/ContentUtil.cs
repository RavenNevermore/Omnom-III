using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace Omnom_III_Game.util {
    public class ContentUtil {

        private ContentManager manager;

        public ContentUtil() { }

        public ContentUtil(ContentManager manager) {
            this.manager = manager;
            
        }

        public virtual T load<T>(String assetName) {
            return this.manager.Load<T>(assetName);
        }

    }
}
