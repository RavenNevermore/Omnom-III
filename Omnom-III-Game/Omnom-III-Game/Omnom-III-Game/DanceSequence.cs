using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Omnom_III_Game {
    public class DanceSequence {

        public class Input {
            public InputState.Move handicap;
            public Song.MusicTime time;

            public Input(InputState.Move handicap, Song.MusicTime time) {
                this.handicap = handicap;
                this.time = time;
            }
        }

        public int startMeasure;
        public Input[]/*List<Input>*/ sequence;
        private float endMeasure;

        public DanceSequence(int startMeasure, params Input[] sequence) {
            this.startMeasure = startMeasure;
            float end = startMeasure;
            this.sequence = sequence;// new List<Input>();
            foreach (Input input in sequence){
                //this.sequence.Add(input);
                end += Song.MusicTimeInFractions(input.time);
            }
            this.endMeasure = end;
        }

        public bool isGone(Song song) {
            int songPos = song.timeRunningInMeasures;
            int endMeasure = (int)this.endMeasure;
            
            return songPos > endMeasure;
        }

        public Input nextInput(Song song) {
            float currentMeasure = song.timeRunningInMeasures;
            currentMeasure += song.positionInMeasure;

            if (currentMeasure < this.startMeasure
                || currentMeasure > this.endMeasure) {
                    return null;
            }
            float measure = (float) startMeasure;
            
            Input lastInput = null;
            foreach (Input input in sequence) {
                lastInput = input;
                measure += Song.MusicTimeInFractions(input.time);
                if (measure > currentMeasure) {
                    break;
                }
            }

            return lastInput;
        }
    }
}
