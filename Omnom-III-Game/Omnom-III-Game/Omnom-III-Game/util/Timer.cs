using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Omnom_III_Game.util {
    public class Timer {

        private long startTicks = 0;

        public Timer() {
            this.restart();
        }

        public void restart() {
            this.startTicks = Environment.TickCount;
        }

        public long timeInMillis {
            get {
                return Environment.TickCount - this.startTicks;
            }
        }

        public float timeInSeconds {
            get {
                return (float)this.timeInMillis / 1000.0f;
            }
        }
    }
}
