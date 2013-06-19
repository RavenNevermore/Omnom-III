using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using Omnom_III_Game.util;

namespace Omnom_III_Game.dance {
    public class DanceProtocol {

        public String title;
        public String enemyTexture;
        private String scriptname;
        ContentScript script;
        DanceSequence[] sequences;
        int activeSequenceIndex;
        public Song song;

        public DanceProtocol() {
            this.sequences = new DanceSequence[0];
        }

        public DanceProtocol(String scriptname): this() {
            this.scriptname = scriptname;
            this.script = ContentScript.FromFile(scriptname);
            this.title = script.title;
        }

        public void initialize() {
            this.script = ContentScript.FromFile(scriptname);
            this.title = script.title;
            this.enemyTexture = script["enemy"][0];

            this.song = new Song(script["song"][0], script.asFloat["tempo"][0]);

            if (null != script["startshift"])
                this.song.timeShift = this.song.beatTimeInMs * script.asFloat["startshift"][0];


            Regex regexStep = new Regex(@"(?:(\d+)/(\d+)(\.)?)?(\s*triplet\s*\()?\s*(\w+)\s*(\))?");
            foreach (String sequenceSource in script["sequences"]) {
                String[] startAndScript = sequenceSource.Split('>');
                int startMeasure = int.Parse(startAndScript[0].Trim());
                String[] scriptSource = startAndScript[1].Split(',');
                DanceSequence sequence = this.addEmptySequenceAt(startMeasure);

                Song.MusicTime lastMusicTime = Song.MusicTime.QUARTER;
                bool isTriplet = false;
                foreach (String _step in scriptSource) {
                    String step = _step.Trim();
                    Match m = regexStep.Match(step);
                    if (m.Success) {
                        String mtPart = m.Groups[1].Value.Trim();
                        String mtFraction = m.Groups[2].Value.Trim();
                        String mtDotted = m.Groups[3].Value.Trim();
                        String tripletStart = m.Groups[4].Value.Trim();
                        String move = m.Groups[5].Value.Trim();
                        String tripletEnd = m.Groups[6].Value.Trim();

                        if (!"".Equals(mtPart) && !"".Equals(mtFraction)) {
                            float fraction = ParserUtil.toFloat(mtPart) / ParserUtil.toFloat(mtFraction);
                            bool dotted = !"".Equals(mtDotted);
                            lastMusicTime = Song.FractionsInMusicTime(fraction, dotted);
                        }

                        if (!"".Equals(tripletStart))
                            isTriplet = true;

                        sequence.addInputs(
                            new DanceSequence.Input(
                                InputState.moveFromString(move),
                                lastMusicTime,
                                isTriplet));

                        if (!"".Equals(tripletEnd))
                            isTriplet = false;
                    }
                }
            }
        }


        public DanceSequence activeSequence {
            get { 
                return this.activeSequenceIndex < this.sequences.Length ? 
                        this.sequences[this.activeSequenceIndex] : null; 
            }
        }

        public DanceSequence.Input nextSequenceInput() {
            return this.activeSequence.nextInput();
        }

        public float activeSequencePlayPosition {
            get {
                return null == this.activeSequence ? -1 : this.activeSequence.playPosition;
            }
        }

        public bool isEnemyActive {
            get {
                return null != this.activeSequence && 0 < this.activeSequencePlayPosition;
            }
        }

        public void addSequence(DanceSequence sequence) {
            List<DanceSequence> sequenceList = this.sequences.ToList();
            sequenceList.Add(sequence);
            this.sequences = sequenceList.ToArray();
        }

        public DanceSequence addEmptySequenceAt(int startMeasure) {
            DanceSequence seq = new DanceSequence(this.song, startMeasure);
            this.addSequence(seq);
            return seq;
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

        public bool stoppedPlaying() {
            return this.song.stoppedPlaying();
        }

        public void update() {
            this.song.calculateMetaInfo();
            if (null != this.activeSequence && this.activeSequence.isGone) {
                this.activeSequenceIndex++;
            }
        }


        internal void stop() {
            this.song.stop();
        }
    }
}
