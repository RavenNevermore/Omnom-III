using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Omnom_III_Game {
    class ExplicitInputState : InputState {

        Dictionary<Move, Boolean> lastMoveState;
        Dictionary<Control, Boolean> lastControlState;

        public ExplicitInputState() : base() {
            this.lastMoveState = new Dictionary<Move,bool>();
            this.lastControlState = new Dictionary<Control, bool>();
        }

        public void update(InputState input) {
            this.moveStates = input.moveStates;
            this.controlStates = input.controlStates;
        }

        public virtual Boolean isActive(Move move) {
            Boolean current = base.isActive(move);
            Boolean last = this.lastMoveState.ContainsKey(move)
                && this.lastMoveState[move];

            if (current != last)
                this.lastMoveState[move] = current;

            return current && !last;
        }

        public virtual Boolean isActive(Control control) {
            Boolean current = base.isActive(control);
            Boolean last = this.lastControlState.ContainsKey(control)
                && this.lastControlState[control];

            if (current != last)
                this.lastControlState[control] = current;

            return current && !last;
        }

    }
}
