using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Omnom_III_Game.highscore {
    public class HighscoreList {

        private List<Scoring> scores;

        public HighscoreList() {
            this.scores = new List<Scoring>();
            this.fillWithRandomData();
        }

        private void fillWithRandomData() {
            Random r = new Random();
            int max = 5000;
            this.scores.Add(new Scoring("Foo", r.Next(max)));
            this.scores.Add(new Scoring("Bar", r.Next(max)));
            this.scores.Add(new Scoring("Baz", r.Next(max)));
            this.scores.Add(new Scoring("Bear", r.Next(max)));
            this.scores.Add(new Scoring("Shark", r.Next(max)));

            this.scores.Add(new Scoring("Flash", r.Next(max)));
            this.scores.Add(new Scoring("Foo Man Sho", r.Next(max)));
            this.scores.Add(new Scoring("Goldfinger", r.Next(max)));
            this.scores.Add(new Scoring("Starsky", r.Next(max)));
            this.scores.Add(new Scoring("Hutch", r.Next(max)));

            this.scores.Add(new Scoring("Kim Jong Ill", r.Next(max)));
            this.scores.Add(new Scoring("Adelheim", r.Next(max)));
            this.scores.Add(new Scoring("Uschi", r.Next(max)));
            this.scores.Add(new Scoring("Bollocks", r.Next(max)));


            this.scores.Add(new Scoring("Starscream", r.Next(max)));
            this.scores.Add(new Scoring("Icecreme", r.Next(max)));
            this.scores.Add(new Scoring("Dalek", r.Next(max)));
            this.scores.Add(new Scoring("The Dr.", r.Next(max)));
            this.scores.Add(new Scoring("Kirk", r.Next(max)));
        }

        public void add(Scoring scoring) {
            this.scores.Add(scoring);
        }

        public List<Scoring> getSortedRange(int start, int length) {
            Scoring.sort(this.scores);
            List<Scoring> result = new List<Scoring>();
            int end = start + length;
            for (int i = start; i < end; i++) {
                result.Add(this.scores.ElementAt(i));
            }
            return result;
        }
    }
}
