using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Omnom_III_Game.util;

namespace Omnom_III_Game {
    public class DanceSequenceProtocol {

        static Regex regexStep = new Regex(@"(?:(\d+)/(\d+)(\.)?)?(\s*triplet\s*\()?\s*(\w+)\s*(\))?");

        List<DanceSequence> sequences;

        public DanceSequenceProtocol() {
            this.sequences = new List<DanceSequence>();
        }

        internal void initialize(ContentScript script) {
            this.sequences.Clear();

            foreach (String sequenceSource in script["sequences"]) {
                String[] startAndScript = sequenceSource.Split('>');
                int startMeasure = int.Parse(startAndScript[0].Trim()) - 1;
                String[] scriptSource = startAndScript[1].Split(',');

                DanceSequence sequence = this.readSequenceDetails(startMeasure, scriptSource);

                this.sequences.Add(sequence);
            }
        }

        private DanceSequence readSequenceDetails(int startMeasure, String[] scriptSource) {
            DanceSequence sequence = new DanceSequence(startMeasure);

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

                    sequence.addInput(
                            InputState.moveFromString(move),
                            lastMusicTime,
                            isTriplet);

                    if (!"".Equals(tripletEnd))
                        isTriplet = false;
                }
            }
            return sequence;
        }

        public IEnumerable<DanceSequence.Input> handicaps { get {
            List<DanceSequence.Input> inputs = new List<DanceSequence.Input>();
            foreach (DanceSequence sequence in this.sequences) {
                inputs.AddRange(sequence.sequence);
            }
            return inputs;
        } }

        internal DanceSequence atMeasure(int measure) {
            foreach (DanceSequence seq in this.sequences) {
                if (measure >= seq.startMeasure
                    && measure <= seq.startMeasure + 2*seq.length) {
                        return seq;
                }
            }
            return null;
        }
    }
}
