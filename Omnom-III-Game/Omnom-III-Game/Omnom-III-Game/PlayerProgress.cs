using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Omnom_III_Game.util;

namespace Omnom_III_Game {
    public class PlayerProgress {
        public enum Rating {
            MISSED, PERFECT, GOOD, OK, NONE
        }

        public class RatedMoves {
            public RatedMoves() {
                this.perfect = new List<DanceSequence.Input>();
                this.good = new List<DanceSequence.Input>();
                this.ok = new List<DanceSequence.Input>();
                this.missed = new List<DanceSequence.Input>();
                this.wrong = new List<DanceSequence.Input>();
                this.allFromUserInput = new List<DanceSequence.Input>();
            }

            public List<DanceSequence.Input> perfect;
            public List<DanceSequence.Input> good;
            public List<DanceSequence.Input> ok;
            public List<DanceSequence.Input> missed;
            public List<DanceSequence.Input> wrong;

            public List<DanceSequence.Input> allFromUserInput;

            internal void validate() {
                this.allFromUserInput.Clear();
                this.validateList(this.perfect);
                this.validateList(this.good);
                this.validateList(this.ok);
                //this.validateList(this.missed);
                this.validateList(this.wrong);
            }

            private void validateList(List<DanceSequence.Input> inputs) {
                foreach (DanceSequence.Input input in inputs) {
                    if (!this.allFromUserInput.Contains(input)){
                        this.allFromUserInput.Add(input);
                    }
                }
            }

            internal bool contains(DanceSequence.Input input) {
                if (null == input)
                    return false;
                return this.allFromUserInput.Contains(input) ||
                    this.missed.Contains(input);
            }

            internal void addWrongMove(DanceSequence sequence, float measure, InputState.Move move) {
                DanceSequence.Input input = new DanceSequence.Input(sequence);
                input.positionInSong = measure;
                input.handicap = move;
                this.wrong.Add(input);
            }

            internal bool hasErrors() {
                return this.missed.Count > 0 ||
                    this.wrong.Count > 0;
            }
        }

        
        public int score;
        private float ratedUntil;
        private List<DanceSequence.Input> notYetRated;
        public bool errorInLastSequence;

        public PlayerProgress() {
            this.reset();
        }

        public RatedMoves nextRating(float measure, DanceSequence sequence, InputState input) {
            RatedMoves rating = new RatedMoves();
            if (null == sequence)
                return rating;

            List<DanceSequence.Input> possibleMatches = sequence.findReachableInputs(measure);
            List<DanceSequence.Input> rated = new List<DanceSequence.Input>();

            this.addNotYetRated(possibleMatches);

            foreach (InputState.Move move in input.activeStates) {
                DanceSequence.Input matching = this.getMatchingInput(possibleMatches, move);
                if (null != matching && matching.positionInSong <= this.ratedUntil)
                    continue;

                if (null != matching) {
                    float accuracy = matching.getAccuracy(measure);
                    if (accuracy <= Song.MusicTimeInFractions(Song.MusicTime.SIXTEENTH) / 2.0f) {
                        this.score += 25;
                        rating.perfect.Add(matching);
                    } else if (accuracy <= Song.MusicTimeInFractions(Song.MusicTime.SIXTEENTH)) {
                        this.score += 15;
                        rating.good.Add(matching);
                    } else {
                        this.score += 5;
                        rating.ok.Add(matching);
                    }

                    rated.Add(matching);
                    this.notYetRated.Remove(matching);
                    if (measure > this.ratedUntil) {
                        this.ratedUntil = matching.positionInSong;
                    }
                } else {
                    rating.addWrongMove(sequence, measure, move);
                }
            }
            
            foreach (DanceSequence.Input possibleMiss in this.notYetRated) {
                if (!possibleMiss.isReachable(measure) || 
                        this.ratedUntil > possibleMiss.positionInSong) {

                    rating.missed.Add(possibleMiss);
                    rated.Add(possibleMiss);
                }
            }
            ListUtil.removeAllFromList(this.notYetRated, rating.missed);

            rating.validate();
            return rating;
        }

        private DanceSequence.Input getMatchingInput(List<DanceSequence.Input> possibleMatches, InputState.Move move) {
            foreach (DanceSequence.Input input in possibleMatches) {
                if (input.handicap.Equals(move)) {
                    return input;
                }
            }
            return null;
        }

        private void addNotYetRated(List<DanceSequence.Input> possibleMatches) {
            foreach (DanceSequence.Input input in possibleMatches) {
                if (input.positionInSong <= this.ratedUntil)
                    continue;

                if (!this.notYetRated.Contains(input))
                    this.notYetRated.Add(input);
            }
        }

        public void reset() {
            this.score = 0;
            this.ratedUntil = -1f;
            this.notYetRated = new List<DanceSequence.Input>();
        }



        internal PlayerProgress clone() {
            PlayerProgress progress = new PlayerProgress();
            progress.score = this.score;
            progress.ratedUntil = this.ratedUntil;
            progress.notYetRated = this.notYetRated;
            return progress;
        }
    }
}
