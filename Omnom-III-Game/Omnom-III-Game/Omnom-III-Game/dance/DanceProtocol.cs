using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Omnom_III_Game.dance {
    class DanceProtocol {

        DanceSequence[] sequences;
        int activeSequenceIndex;
        public Song song;

        public DanceProtocol() {

        }

        public DanceProtocol(String scriptname) {

        }


        public DanceSequence activeSequence {
            get { 
                return this.activeSequenceIndex < this.sequences.Length ? 
                        this.sequences[this.activeSequenceIndex] : null; 
            }
        }

        public long timeRunning {
            get {
                return this.song.timeRunningInMs;
            }
        }

        public int numberOfSequences {
            get { return this.sequences.Length; } 
        }

        public void reset() {
            this.activeSequenceIndex = 0;
        }

        public void startPlaying() {
            this.song.play();
        }

        public void update() {
            this.song.calculateMetaInfo();
            if (null != this.activeSequence && this.activeSequence.isGone(this.song)) {
                this.activeSequenceIndex++;
            }
        }

        public static DanceProtocol EyeOfTheTiger(FMOD.System soundsystem) {
            DanceProtocol protocol = new DanceProtocol();
            //this.song = new Song("MeasureTest", this.soundsystem, 60);//122.8f);
            //this.song = new Song("eattherich", this.soundsystem, 122.8f);
            //this.song = new Song("eyeofthetiger", this.soundsystem, 109f);
            //this.song.timeShift = this.song.beatTimeInMs * -1.5f;
            protocol.song = new Song("eyeofthetiger", soundsystem, 109f);
            protocol.song.timeShift = protocol.song.beatTimeInMs * -1.5f;
            protocol.sequences = new DanceSequence[]{
                new DanceSequence(protocol.song, 1,
                    new DanceSequence.Input(InputState.Move.LEFT, Song.MusicTime.QUARTER),
                    new DanceSequence.Input(InputState.Move.RIGHT, Song.MusicTime.QUARTER),
                    new DanceSequence.Input(InputState.Move.LEFT, Song.MusicTime.QUARTER),
                    new DanceSequence.Input(InputState.Move.RIGHT, Song.MusicTime.QUARTER)),

                new DanceSequence(protocol.song, 3,
                    new DanceSequence.Input(InputState.Move.LEFT, Song.MusicTime.QUARTER),
                    new DanceSequence.Input(InputState.Move.RIGHT, Song.MusicTime.QUARTER),
                    new DanceSequence.Input(InputState.Move.LEFT, Song.MusicTime.QUARTER),
                    new DanceSequence.Input(InputState.Move.RIGHT, Song.MusicTime.QUARTER)),

                new DanceSequence(protocol.song, 5,
                    new DanceSequence.Input(InputState.Move.UP, Song.MusicTime.HALF),
                    new DanceSequence.Input(InputState.Move.LEFT, Song.MusicTime.QUARTER, true),
                    new DanceSequence.Input(InputState.Move.RIGHT, Song.MusicTime.QUARTER, true),
                    new DanceSequence.Input(InputState.Move.UP, Song.MusicTime.QUARTER, true),

                    new DanceSequence.Input(InputState.Move.BREAK, Song.MusicTime.HALF),
                    new DanceSequence.Input(InputState.Move.LEFT, Song.MusicTime.QUARTER, true),
                    new DanceSequence.Input(InputState.Move.RIGHT, Song.MusicTime.QUARTER, true),
                    new DanceSequence.Input(InputState.Move.UP, Song.MusicTime.QUARTER, true),

                    new DanceSequence.Input(InputState.Move.BREAK, Song.MusicTime.HALF),
                    new DanceSequence.Input(InputState.Move.LEFT, Song.MusicTime.QUARTER, true),
                    new DanceSequence.Input(InputState.Move.RIGHT, Song.MusicTime.QUARTER, true),
                    new DanceSequence.Input(InputState.Move.DOWN, Song.MusicTime.QUARTER, true),

                    new DanceSequence.Input(InputState.Move.DOWN, Song.MusicTime.FULL)),

                new DanceSequence(protocol.song, 13,
                    new DanceSequence.Input(InputState.Move.UP, Song.MusicTime.QUARTER),
                    new DanceSequence.Input(InputState.Move.DOWN, Song.MusicTime.QUARTER),
                    new DanceSequence.Input(InputState.Move.LEFT, Song.MusicTime.QUARTER, true),
                    new DanceSequence.Input(InputState.Move.RIGHT, Song.MusicTime.QUARTER, true),
                    new DanceSequence.Input(InputState.Move.DOWN, Song.MusicTime.EIGTH, true),
                    new DanceSequence.Input(InputState.Move.UP, Song.MusicTime.EIGTH, true),

                    new DanceSequence.Input(InputState.Move.BREAK, Song.MusicTime.HALF),
                    new DanceSequence.Input(InputState.Move.LEFT, Song.MusicTime.QUARTER, true),
                    new DanceSequence.Input(InputState.Move.RIGHT, Song.MusicTime.QUARTER, true),
                    new DanceSequence.Input(InputState.Move.DOWN, Song.MusicTime.EIGTH, true),
                    new DanceSequence.Input(InputState.Move.UP, Song.MusicTime.EIGTH, true),

                    new DanceSequence.Input(InputState.Move.BREAK, Song.MusicTime.HALF),
                    new DanceSequence.Input(InputState.Move.LEFT, Song.MusicTime.QUARTER, true),
                    new DanceSequence.Input(InputState.Move.RIGHT, Song.MusicTime.QUARTER, true),
                    new DanceSequence.Input(InputState.Move.DOWN, Song.MusicTime.QUARTER, true),

                    new DanceSequence.Input(InputState.Move.DOWN, Song.MusicTime.FULL)),

                new DanceSequence(protocol.song, 21,
                    new DanceSequence.Input(InputState.Move.LEFT, Song.MusicTime.QUARTER),
                    new DanceSequence.Input(InputState.Move.RIGHT, Song.MusicTime.QUARTER),
                    new DanceSequence.Input(InputState.Move.LEFT, Song.MusicTime.QUARTER),
                    new DanceSequence.Input(InputState.Move.RIGHT, Song.MusicTime.QUARTER),
                    new DanceSequence.Input(InputState.Move.LEFT, Song.MusicTime.QUARTER),
                    new DanceSequence.Input(InputState.Move.RIGHT, Song.MusicTime.QUARTER),
                    new DanceSequence.Input(InputState.Move.LEFT, Song.MusicTime.QUARTER),
                    new DanceSequence.Input(InputState.Move.DOWN, Song.MusicTime.EIGTH),
                    new DanceSequence.Input(InputState.Move.RIGHT, Song.MusicTime.EIGTH)),

                new DanceSequence(protocol.song, 25,
                    new DanceSequence.Input(InputState.Move.UP, Song.MusicTime.QUARTER),
                    new DanceSequence.Input(InputState.Move.DOWN, Song.MusicTime.QUARTER),
                    new DanceSequence.Input(InputState.Move.UP, Song.MusicTime.QUARTER),
                    new DanceSequence.Input(InputState.Move.DOWN, Song.MusicTime.QUARTER),
                    new DanceSequence.Input(InputState.Move.UP, Song.MusicTime.QUARTER),
                    new DanceSequence.Input(InputState.Move.DOWN, Song.MusicTime.QUARTER),
                    new DanceSequence.Input(InputState.Move.LEFT, Song.MusicTime.QUARTER),
                    new DanceSequence.Input(InputState.Move.RIGHT, Song.MusicTime.EIGTH),
                    new DanceSequence.Input(InputState.Move.DOWN, Song.MusicTime.EIGTH)),

                new DanceSequence(protocol.song, 29,
                    new DanceSequence.Input(InputState.Move.LEFT, Song.MusicTime.QUARTER),
                    new DanceSequence.Input(InputState.Move.RIGHT, Song.MusicTime.QUARTER),
                    new DanceSequence.Input(InputState.Move.LEFT, Song.MusicTime.QUARTER),
                    new DanceSequence.Input(InputState.Move.RIGHT, Song.MusicTime.QUARTER),
                    new DanceSequence.Input(InputState.Move.LEFT, Song.MusicTime.QUARTER),
                    new DanceSequence.Input(InputState.Move.RIGHT, Song.MusicTime.QUARTER),
                    new DanceSequence.Input(InputState.Move.LEFT, Song.MusicTime.QUARTER),
                    new DanceSequence.Input(InputState.Move.DOWN, Song.MusicTime.EIGTH),
                    new DanceSequence.Input(InputState.Move.RIGHT, Song.MusicTime.EIGTH)),

                new DanceSequence(protocol.song, 33,
                    new DanceSequence.Input(InputState.Move.UP, Song.MusicTime.QUARTER),
                    new DanceSequence.Input(InputState.Move.DOWN, Song.MusicTime.QUARTER),
                    new DanceSequence.Input(InputState.Move.UP, Song.MusicTime.QUARTER),
                    new DanceSequence.Input(InputState.Move.DOWN, Song.MusicTime.QUARTER),
                    new DanceSequence.Input(InputState.Move.UP, Song.MusicTime.QUARTER),
                    new DanceSequence.Input(InputState.Move.DOWN, Song.MusicTime.QUARTER),
                    new DanceSequence.Input(InputState.Move.LEFT, Song.MusicTime.QUARTER),
                    new DanceSequence.Input(InputState.Move.RIGHT, Song.MusicTime.EIGTH),
                    new DanceSequence.Input(InputState.Move.DOWN, Song.MusicTime.EIGTH)),

                new DanceSequence(protocol.song, 37,
                    new DanceSequence.Input(InputState.Move.LEFT, Song.MusicTime.QUARTER),
                    new DanceSequence.Input(InputState.Move.RIGHT, Song.MusicTime.QUARTER),
                    new DanceSequence.Input(InputState.Move.UP, Song.MusicTime.QUARTER),
                    new DanceSequence.Input(InputState.Move.DOWN, Song.MusicTime.QUARTER)),

                new DanceSequence(protocol.song, 39,
                    new DanceSequence.Input(InputState.Move.UP, Song.MusicTime.HALF),
                    new DanceSequence.Input(InputState.Move.DOWN, Song.MusicTime.HALF),
                    new DanceSequence.Input(InputState.Move.LEFT, Song.MusicTime.HALF),
                    new DanceSequence.Input(InputState.Move.RIGHT, Song.MusicTime.HALF))
            };
            return protocol;
        }
    }
}
