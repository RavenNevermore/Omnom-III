using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Omnom_III_Game {
    public class SceneActivationParameters {
        public SceneActivationParameters() : this(null, null) {}

        public SceneActivationParameters(String sceneName, Object parameters) {
            this.sceneName = sceneName;
            this.parameters = parameters;
        }

        public String sceneName;
        public Object parameters;
    }
}
