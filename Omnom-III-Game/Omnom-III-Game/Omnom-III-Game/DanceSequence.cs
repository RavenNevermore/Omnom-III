using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Omnom_III_Game {
    public class DanceSequence {

        public class Input {
            public InputState.Move handicap;
            public Song.MusicTime time;
            public long length;
            public long startPositionInSong;

            public Input(InputState.Move handicap, Song.MusicTime time) {
                this.handicap = handicap;
                this.time = time;
            }

            public void calcLength(Song song) {
                this.length = (long)(Song.MusicTimeInFractions(this.time) * song.measureTimeInMs);
            }
        }

        public long startPosition;
        public Input[] sequence;
        private int positionInSequence;
        public long endPosition;

        public DanceSequence(Song song, int startMeasure, params Input[] sequence) {
            this.startPosition = (long) (song.measureTimeInMs * (startMeasure - 1));
            this.positionInSequence = 0;

            this.endPosition = this.startPosition;

            this.sequence = sequence;
            long inputPos = this.startPosition;
            foreach (Input input in this.sequence){
                input.calcLength(song);
                input.startPositionInSong = inputPos;

                this.endPosition += input.length;
                inputPos += input.length;
            }
        }

        public bool isGone(Song song) {
            return song.timeRunningInMs > this.endPosition;
        }

        public Input nextInput(Song song) {
            if (song.timeRunningInMs < this.startPosition
                || song.timeRunningInMs > this.endPosition)
                return null;

            for (int i = 0; i < this.sequence.Length; i++) {
                Input current = this.sequence[i];
                if (current.startPositionInSong <= song.timeRunningInMs
                    && current.startPositionInSong + current.length >= song.timeRunningInMs) {
                        return current;
                }
            }
            return null;
        }

    }
}
