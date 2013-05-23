using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Omnom_III_Game {
    public class InputState {

        public enum Move { UP, DOWN, LEFT, RIGHT, BREAK };

        private Dictionary<Move, Boolean> moveStates;

        public InputState() {
            this.moveStates = new Dictionary<Move, Boolean>();
        }

        public InputState(params Move[] activeInputs) {
            this.moveStates = new Dictionary<Move, Boolean>();
            foreach (Move move in activeInputs){
                this.activate(move);
            }
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

        public bool Equals(InputState other) {
            return null != other
                && other.moveStates.Equals(this.moveStates);
        }
    }
}
