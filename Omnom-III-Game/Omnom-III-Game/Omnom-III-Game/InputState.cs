using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Omnom_III_Game {
    public class InputState {

        public enum Move { UP, DOWN, LEFT, RIGHT };

        private Dictionary<Move, Boolean> moveStates;

        public InputState() {
            this.moveStates = new Dictionary<Move, Boolean>();
        }

        public void activate(Move move) { 
            this.set(move, true);
        }

        public void deactivate(Move move) {
            this.set(move, false);
        }

        public void set(Move move, Boolean value) {
            this.moveStates[move] = value;
        }

        public Boolean isActive(Move move) {
            return this.moveStates.ContainsKey(move) &&
                this.moveStates[move];
        }

    }
}
