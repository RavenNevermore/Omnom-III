using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Omnom_III_Game {
    public class InputState {

        public enum Move { UP, DOWN, LEFT, RIGHT, BREAK };

        public static Move moveFromString(String input) {
            input = input.Trim().ToUpper();
            if ("UP".Equals(input))
                return Move.UP;
            else if ("DOWN".Equals(input))
                return Move.DOWN;
            else if ("LEFT".Equals(input))
                return Move.LEFT;
            else if ("RIGHT".Equals(input))
                return Move.RIGHT;
            else if ("BREAK".Equals(input) || "---".Equals(input))
                return Move.BREAK;

            throw new ArgumentOutOfRangeException("No move defined for "+input);
        }

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

        public List<Move> activeStates { 
            get {
                List<Move> active = new List<Move>();
                foreach (KeyValuePair<Move, Boolean> move in this.moveStates) {
                    if (move.Value) {
                        active.Add(move.Key);
                    }
                }
                return active;
            }
        }

        public bool Equals(InputState other) {
            return null != other
                && other.moveStates.Equals(this.moveStates);
        }
    }
}
