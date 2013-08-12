using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Omnom_III_Game.highscore {
    public class HighscoreParams {

        public String stage;
        public String background;
        public PlayerProgress newScore;
        
        public HighscoreParams(String stageName, String background, PlayerProgress newScore) {
            this.stage = stageName;
            this.background = background;
            this.newScore = newScore;
        }

        public HighscoreParams(String stageName, String background) : this(stageName, background, null) { }

        
    }
}
