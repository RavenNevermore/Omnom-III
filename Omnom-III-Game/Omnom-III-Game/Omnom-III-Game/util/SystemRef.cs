using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Omnom_III_Game.exceptions;

namespace Omnom_III_Game.util {
    class SystemRef {

        private static FMOD.System _soundsystem;
        private static Random random = new Random();

        public static FMOD.System soundsystem {
            get {
                if (null == _soundsystem) {
                    createSoundSystem();
                }
                return _soundsystem;
            }
        }

        private static void createSoundSystem() {
            uint version = 0;
            FMOD.RESULT result = FMOD.Factory.System_Create(ref _soundsystem);
            ERRCHECK(result);
            result = soundsystem.getVersion(ref version);
            ERRCHECK(result);
            result = soundsystem.init(32, FMOD.INITFLAGS.NORMAL, (IntPtr) null);
            ERRCHECK(result);
        }

        private static void ERRCHECK(FMOD.RESULT result) {
            if (result != FMOD.RESULT.OK) {
                throw new SoundSystemException(result.ToString("X"));
            }
        }

        public static int nextRandomInt(int min, int max) {
            return random.Next(min, max);
        }
    }
}
