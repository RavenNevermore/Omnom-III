using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Omnom_III_Game.highscore {
    public class Scoring {
        static char SERIAL_SEPARATOR = '|';

        public static void sort(List<Scoring> scores) {
            scores.Sort(delegate(Scoring a, Scoring b) {
                return a.score.CompareTo(b.score) * -1;
            });
        }

        public Scoring(String name, int score) : this(name, score, false) {}

        public Scoring(String name, int score, bool isNew) {
            this.name = name;
            this.score = score;
            this.isNew = isNew;
        }

        public String name;
        public int score;
        public bool isNew;

        public String serialize() {
            return this.name + SERIAL_SEPARATOR + this.score;
        }

        public static Scoring deserialize(String serialized) {
            String[] src = serialized.Split(SERIAL_SEPARATOR);
            String name = src[0].Trim();
            int score = Convert.ToInt32(src[1].Trim());
            return new Scoring(name, score);
        }
    }
}
