using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Omnom_III_Game.util;

namespace Omnom_III_Game {
    class MusicalSceneChain : SceneChain {

        private String songName;
        private float bpm;
        private Song song;

        public MusicalSceneChain(String songname, float bpm) : base() {
            this.songName = songname;
            this.bpm = bpm;
        }

        public override void initialize(ContentUtil content, SceneActivationParameters parameters) {
            base.initialize(content, parameters);
            this.song = new Song(this.songName, content, this.bpm);
        }

        public override void update(InputState input) {
            if (this.song.stoppedPlaying())
                this.song.play();
            base.update(input);
        }

        public override void cleanup() {
            base.cleanup();
            this.song.stop();
            this.song.reset();
            this.song = null;
        }
    }
}
