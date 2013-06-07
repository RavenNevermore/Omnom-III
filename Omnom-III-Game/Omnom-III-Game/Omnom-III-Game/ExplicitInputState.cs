using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Omnom_III_Game {
    class ExplicitInputState : InputState {

        Dictionary<object, Boolean> lastStates;

        public ExplicitInputState() : base() {
            this.lastStates = new Dictionary<object, bool>();
        }

        public void update(InputState input) {
            foreach (KeyValuePair<Move, Boolean> state in this.moveStates) {
                this.lastStates[state.Key] = state.Value;
            }
            foreach (KeyValuePair<Control, Boolean> state in this.controlStates) {
                this.lastStates[state.Key] = state.Value;
            }

            this.moveStates = input.moveStates;
            this.controlStates = input.controlStates;
        }

        public override Boolean isActive(Move move) {
            return this.isActive(move, base.isActive(move));
        }

        public override Boolean isActive(Control control) {
            return this.isActive(control, base.isActive(control));
        }

        private Boolean isActive(object key, Boolean current){
            Boolean last = this.lastStates.ContainsKey(key) && this.lastStates[key];

            this.lastStates[key] = current;

            return !last && current;
        }

    }
}
