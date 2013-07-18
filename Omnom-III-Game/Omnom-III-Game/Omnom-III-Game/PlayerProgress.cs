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
            }

            public List<DanceSequence.Input> perfect;
            public List<DanceSequence.Input> good;
            public List<DanceSequence.Input> ok;
            public List<DanceSequence.Input> missed;
            public List<DanceSequence.Input> wrong;

            internal void addWrongMove(DanceSequence sequence, float measure, InputState.Move move) {
                DanceSequence.Input input = new DanceSequence.Input(sequence);
                input.positionInSong = measure;
                input.handicap = move;
                this.missed.Add(input);
            }
        }

        
        public int score;
        private float ratedUntil;
        private List<DanceSequence.Input> notYetRated;

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
                if (!possibleMiss.isReachable(measure)) {
                    rating.missed.Add(possibleMiss);
                    rated.Add(possibleMiss);
                }
            }
            ListUtil.removeAllFromList(this.notYetRated, rating.missed);

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

        
    }
}
