using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using Omnom_III_Game.exceptions;
using Omnom_III_Game.util;

namespace Omnom_III_Game {
    public class Sound {
        protected String filename;
        protected SoundEffect sound;
        protected SoundEffectInstance instance;

        public Sound(String contentName, ContentUtil content) {
            this.sound = content.load<SoundEffect>(contentName);
        }

        public virtual void play() {
            if (null == this.instance) {
                this.instance = sound.CreateInstance();
            }
            if (SoundState.Paused ==  this.instance.State) {
                this.instance.Resume();
            } else {
                this.instance.Play();
            }
        }

        public virtual void pause() {
            if (SoundState.Paused == this.instance.State)
                return;
            this.instance.Pause();
        }

        public bool stoppedPlaying() {
            return null == this.instance || 
                SoundState.Stopped == this.instance.State;
        }

        public void stop() {
            if (!this.stoppedPlaying()) {
                this.instance.Stop();
            }
        }

        public void reset() {
            this.stop();
            this.instance = null;
        }
    }
}
