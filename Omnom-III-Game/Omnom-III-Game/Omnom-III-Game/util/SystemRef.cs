using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Omnom_III_Game.exceptions;

namespace Omnom_III_Game.util {
    class SystemRef {

        private static Random random = new Random();

        

        public static int nextRandomInt(int min, int max) {
            return random.Next(min, max);
        }
    }
}
