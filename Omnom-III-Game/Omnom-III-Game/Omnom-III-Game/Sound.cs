using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FMOD;
using Omnom_III_Game.exceptions;
using Omnom_III_Game.util;

namespace Omnom_III_Game {
    public class Sound {
        protected String filename;
        protected FMOD.Sound content;
        protected FMOD.Channel channel;
        private bool paused;

        public Sound(String contentName) {
            this.setFilenameFromContentName(contentName);
            ERRCHECK(SystemRef.soundsystem.createSound(filename, FMOD.MODE.SOFTWARE, ref content));
        }

        protected void setFilenameFromContentName(String contentName) {
            this.filename = "Content/" + contentName + ".wma";
        }

        protected void ERRCHECK(FMOD.RESULT result) {
            if (result != FMOD.RESULT.OK) {
                throw new SoundSystemException(result.ToString("X"));
            }
        }

        public virtual void play() {
            if (this.paused) {
                this.paused = false;
                this.channel.setPaused(false);
            } else {
                SystemRef.soundsystem.playSound(FMOD.CHANNELINDEX.FREE, content, false, ref channel);
            }
        }

        public virtual void pause() {
            if (this.paused)
                return;
            this.channel.setPaused(true);
            this.paused = true;
        }

        public bool stoppedPlaying() {
            if (null == this.channel)
                return true;
            bool isPlaying = false;
            this.channel.isPlaying(ref isPlaying);
            return !isPlaying;
        }

        public void stop() {
            if (!this.stoppedPlaying()) {
                this.channel.stop();
            }
        }

        public void reset() {
            this.channel.setPosition(0, TIMEUNIT.MS);
        }
    }
}
