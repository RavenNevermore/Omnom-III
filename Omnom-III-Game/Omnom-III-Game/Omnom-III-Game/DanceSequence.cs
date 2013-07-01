using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Omnom_III_Game.util;

namespace Omnom_III_Game {
    public class DanceSequence {
        public class BasicInput {
            public InputState.Move handicap;
            public long startPositionInSong;
            public long length;

            public long endPositionInSong {
                get {
                    return this.startPositionInSong + this.length;
                }
            }

            public long getDeltaTime(long positionInSong) {
                long delta = this.startPositionInSong - positionInSong;
                if (0 > delta)
                    delta = delta * -1;
                return delta;
            }

            public BasicInput copyShifted(long deltaTime) {
                BasicInput copy = new BasicInput();
                copy.startPositionInSong = this.startPositionInSong + deltaTime;
                copy.handicap = this.handicap;
                copy.length = this.length;
                return copy;
            }
        }

        public class Input : BasicInput {
            public Song.MusicTime time;
            public bool partOfTriole;
            

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

        public float getTimeDeltaInBeats(long timeDelta) {
            return timeDelta / this.song.beatTimeInMs;
        }

        public long length {
            get {
                return this.endPosition - this.startPosition;
            }
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

        public bool isGoneAt(long time) {
            return time > this.endPosition;
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


        public BasicInput findClosestInput(long sequenceTime) {
            Input closest = null;
            long deltaTime = 0;
            foreach (Input input in this.sequence) {
                if (null == closest) {
                    closest = input;
                    deltaTime = closest.getDeltaTime(sequenceTime);
                } else {
                    long nextDelta = input.getDeltaTime(sequenceTime);
                    if (nextDelta < deltaTime) {
                        closest = input;
                        deltaTime = nextDelta;
                    }
                }
            }
            return closest;
        }
    }
}
