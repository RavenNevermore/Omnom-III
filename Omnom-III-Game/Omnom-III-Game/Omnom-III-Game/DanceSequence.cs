using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Omnom_III_Game.util;

namespace Omnom_III_Game {
    public class DanceSequence {

        public class Input {
            public InputState.Move handicap;
            public Song.MusicTime time;
            public bool partOfTriole;
            public long length;
            public long startPositionInSong;

            public Input(InputState.Move handicap, Song.MusicTime time, bool partOfTriole) {
                this.handicap = handicap;
                this.partOfTriole = partOfTriole;
                this.time = time;
            }

            public Input(InputState.Move handicap, Song.MusicTime time) : this(handicap, time, false) {}

            public void calcLength(Song song) {
                this.length = (long)(Song.MusicTimeInFractions(this.time) * song.measureTimeInMs);
                if (this.partOfTriole) {
                    this.length = (long) (2.0f / 3.0f * (float)this.length);
                }
            }
        }

        public long startPosition;
        public Input[] sequence;
        private int positionInSequence;
        public long endPosition;
        private Song song;

        public DanceSequence(Song song, int startMeasure, params Input[] sequence) {
            this.startPosition = (long) (song.measureTimeInMs * (startMeasure - 1));
            this.positionInSequence = 0;
            this.song = song;

            this.endPosition = this.startPosition;

            this.addInputs(sequence);
            /*
            this.sequence = sequence;
            long inputPos = this.startPosition;
            foreach (Input input in this.sequence){
                input.calcLength(song);
                input.startPositionInSong = inputPos;

                this.endPosition += input.length;
                inputPos += input.length;
            }*/
        }

        public float playPosition {
            get {
                float pos = (float) (this.song.timeRunningInMs - this.startPosition);
                float length = (float) (this.endPosition - this.startPosition);
                return pos / length;
            }
        }

        public void addInputs(params Input[] sequence) {
            this.sequence = ParserUtil.addArrays(this.sequence, sequence);

            this.recalculatePositions();
        }

        private void recalculatePositions() {
            long inputPos = this.startPosition;
            this.endPosition = this.startPosition;
            foreach (Input input in this.sequence) {
                input.calcLength(this.song);
                input.startPositionInSong = inputPos;

                this.endPosition += input.length;
                inputPos += input.length;
            }
        }

        public bool isGone {
            get {
                return this.song.timeRunningInMs > this.endPosition;
            }
        }

        public Input nextInput() {
            if (this.song.timeRunningInMs < this.startPosition
                || this.song.timeRunningInMs > this.endPosition)
                return null;

            for (int i = 0; i < this.sequence.Length; i++) {
                Input current = this.sequence[i];
                if (current.startPositionInSong <= this.song.timeRunningInMs
                    && current.startPositionInSong + current.length >= this.song.timeRunningInMs) {
                        return current;
                }
            }
            return null;
        }

    }
}
