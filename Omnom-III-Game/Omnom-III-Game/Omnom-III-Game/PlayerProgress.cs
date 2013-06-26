using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Omnom_III_Game {
    public class PlayerProgress {
        public enum Rating {
            MISSED, PERFECT, GOOD, OK, BAD, NONE
        }

        public class PlayerMove {
            public DanceSequence.BasicInput script;
            public Rating rating = Rating.NONE;

            internal bool hasEnded(long positionInSong) {
                return positionInSong >= this.script.endPositionInSong;
            }

            internal bool isCounted() {
                return Rating.NONE != this.rating;
            }

            public override string ToString() {
                String str = "";
                if (null == this.script) {
                    str += "NULL --- | ---";
                } else {
                    str += this.script.handicap;
                    str += " ";
                    str += this.script.startPositionInSong;
                    str += " | ";
                    str += this.script.endPositionInSong;
                }
                str += " (";
                str += this.rating;
                str += ")";
                return str;
            }

            internal bool isBreak() {
                return null != this.script &&
                    this.script.handicap == InputState.Move.BREAK;
            }
        }

        public int score;
        public int lifes;

        public PlayerMove activeMove;

        public PlayerProgress() {
            this.score = 0;
            this.lifes = 10;
        }

        public void reset() {
            this.score = 0;
            this.lifes = 10;
            this.activeMove = null;
        }

        public void activateNextMove(DanceSequence sequence, long currentTime) {
            long sequenceTime = currentTime - sequence.length;
            if (sequence.isGoneAt(sequenceTime))
                return;

            this.activeMove = new PlayerMove();
            DanceSequence.BasicInput sequenceInput = 
                sequence.findClosestInput(sequenceTime);

            if (null != sequenceInput) {
                this.activeMove.script = sequenceInput.copyShifted(sequence.length);
            }
        }

        /**
         *  returns whether this update changed ratings or the move
         */
        public bool update(long positionInSong, DanceSequence activeSequence,
                List<InputState.Move> activeMoves) {

            bool hasNextMove = false;

            if (null == activeSequence)
                return false;
            if (null == this.activeMove) {
                this.activateNextMove(activeSequence, positionInSong);
                hasNextMove = true;
            }
            if (null != this.activeMove && !this.activeMove.isCounted()) {
                if (this.activeMove.hasEnded(positionInSong) && ! this.activeMove.isBreak()) {
                    this.activeMove.rating = Rating.MISSED;
                    this.lifes--;
                    return true;
                } else {
                    if (activeMoves.Contains(this.activeMove.script.handicap)){
                        this.activeMove.rating = Rating.GOOD;
                        return true;
                    }
                }
            }

            return hasNextMove;
        }

        public bool activeMoveIsMissed() {
            return null != this.activeMove && 
                Rating.MISSED == this.activeMove.rating;
        }

        public InputState.Move getActiveHandicap() {
            if (null == this.activeMove)
                return InputState.Move.BREAK;
            return this.activeMove.script.handicap;
        }

        public void cleanup(long positionInSong) {
            if (null != this.activeMove && this.activeMove.hasEnded(positionInSong)) {
                this.activeMove = null;
            }
        }



        public Rating getActiveMoveRating() {
            if (null == this.activeMove)
                return Rating.NONE;
            return this.activeMove.rating;
        }
    }
}
