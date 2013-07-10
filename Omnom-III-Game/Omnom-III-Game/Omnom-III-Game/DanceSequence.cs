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

            public float length { get { return Song.MusicTimeInFractions(
                this.musicLength,
                this.partOfTriole); } }

            public float positionInSong { get {
                return this.positionInSequence + this.parent.startMeasure;
            } }

            internal long startTime(float beatsPerMs) {
                return (long) ((this.positionInSong * 4) / beatsPerMs);
            }
        }

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
            Input input = new Input();
            input.handicap = move;
            input.musicLength = musicLength;
            input.partOfTriole = isTriplet;
            input.positionInSequence = this._length;
            input.parent = this;

            this.sequence.Add(input);
            this._length += input.length;
        }

        internal bool playerInputAllowed(float measures) {
            float playerStart = this.startMeasure + this.length;
            float playerEnd = playerStart + this.length;
            float fuzzyness = Song.MusicTimeInFractions(Song.MusicTime.SIXTEENTH);
            playerStart -= fuzzyness;
            playerStart += fuzzyness;
            return playerStart <= measures && measures <= playerEnd;
        }

        internal bool isEnemyActive(float measures) {
            return this.startMeasure <= measures && measures <= this.startMeasure + this.length;
        }
    }
}
