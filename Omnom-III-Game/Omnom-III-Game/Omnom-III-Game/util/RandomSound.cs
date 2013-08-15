using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Omnom_III_Game.util {
    public class RandomSound {

        private Sound[] sounds;

        public RandomSound(String name, int count, ContentUtil content) {
            this.sounds = new Sound[count];
            for (int i = 0; i < count; i++) {
                String countStr = "" + (i + 1);
                if (i < 9)
                    countStr = "0" + countStr;

                sounds[i] = new Sound(name + "_" + countStr, content);
            }

        }

        public void play() {
            int i = SystemRef.nextRandomInt(0, this.sounds.Length);
            this.sounds[i].play();
        }

    }
}
