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
        public String backgroundTexture;
        private String scriptname;
        ContentScript script;
        DanceSequence[] sequences;
        int activeSequenceIndex;
        

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
            this.backgroundTexture = null == script["background"] ? null : script["background"][0];

            


            
        }


        public DanceSequence activeSequence {
            get { 
                return this.activeSequenceIndex < this.sequences.Length ? 
                        this.sequences[this.activeSequenceIndex] : null; 
            }
        }

        public DanceSequence lastSequence {
            get {
                int i = this.activeSequenceIndex - 1;
                return this.activeSequenceIndex > 0 ?
                        this.sequences[i] : null; 
            }
        }

        public void addSequence(DanceSequence sequence) {
            List<DanceSequence> sequenceList = this.sequences.ToList();
            sequenceList.Add(sequence);
            this.sequences = sequenceList.ToArray();
        }

        public int numberOfSequences {
            get { return this.sequences.Length; } 
        }
    }
}
