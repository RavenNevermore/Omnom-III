using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Omnom_III_Game.util;

namespace Omnom_III_Game {
    public class DanceSequence {
        public class Input {
            public InputState.Move handicap;
            public float positionInSequence;
            public Song.MusicTime musicLength;
            public bool partOfTriole;
            internal DanceSequence parent;

            public Input(DanceSequence parent) {
                this.parent = parent;
            }

            public float length { get { return Song.MusicTimeInFractions(
                this.musicLength,
                this.partOfTriole); } }

            public float positionInSong { 
                get {
                    return this.positionInSequence + this.parent.startMeasure;
                }
                set {
                    this.positionInSequence = value - this.parent.startMeasure;
                }
            }

            internal long startTime(float beatsPerMs) {
                return (long) ((this.positionInSong * 4) / beatsPerMs);
            }

            public bool isReachable(float songMeasure) {
                float templateMeasure = songMeasure - this.parent.length;

                float start = this.positionInSong - FUZZYNESS;
                float end = this.positionInSong + this.length + FUZZYNESS;

                return start <= templateMeasure && templateMeasure <= end;
            }

            public override bool Equals(object obj) {
                if (null == obj || !(obj is Input))
                    return false;
                Input other = (Input) obj;

                return other.handicap.Equals(this.handicap)
                    && other.positionInSequence == this.positionInSequence
                    && other.musicLength == this.musicLength
                    && other.partOfTriole == this.partOfTriole;
            }
        }

        public static float FUZZYNESS = Song.MusicTimeInFractions(Song.MusicTime.SIXTEENTH);

        public int startMeasure;
        public List<Input> sequence;
        private float _length;
        public float length { get { return this._length; } }
        

        public DanceSequence(int startMeasure) {
            this.startMeasure = startMeasure;
            this._length = 0f;

            this.sequence = new List<Input>();
        }

        public override string ToString() {
            return "Start: "+ this.startMeasure+" Length: "+this.length;
        }

        internal void addInput(InputState.Move move, Song.MusicTime musicLength, bool isTriplet) {
            Input input = new Input(this);
            input.handicap = move;
            input.musicLength = musicLength;
            input.partOfTriole = isTriplet;
            input.positionInSequence = this._length;

            this.sequence.Add(input);
            this._length += input.length;
        }

        internal bool playerInputAllowed(float measures) {
            float playerStart = this.startMeasure + this.length;
            float playerEnd = playerStart + this.length;
            
            playerStart -= FUZZYNESS;
            playerStart += FUZZYNESS;
            return playerStart <= measures && measures <= playerEnd;
        }

        internal bool isEnemyActive(float measures) {
            return this.startMeasure <= measures && measures <= this.startMeasure + this.length;
        }

        public List<Input> findReachableInputs(float measure) {
            List<Input> possibles = new List<Input>();
            foreach (Input input in this.sequence) {
                if (input.isReachable(measure)){
                    possibles.Add(input);
                }
            }
            return possibles;
        }
    }
}
