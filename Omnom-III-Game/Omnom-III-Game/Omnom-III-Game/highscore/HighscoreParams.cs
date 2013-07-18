using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Omnom_III_Game.highscore {
    public class HighscoreParams {
        public HighscoreParams(String stageName, PlayerProgress newScore) {
            this.stage = stageName;
            this.newScore = newScore;
        }

        public HighscoreParams(String stageName) : this(stageName, null) {}

        public String stage;
        public PlayerProgress newScore;
    }
}
