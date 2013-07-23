using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Omnom_III_Game.highscore {
    public class HighscoreList {

        private List<Scoring> scores;

        public HighscoreList() {
            this.scores = new List<Scoring>();
            //this.fillWithRandomData();
        }

        public void loadForScene(String sceneName) {
            String filepath = this.getFilenameFromScenename(sceneName);
            this.scores.Clear();
            if (!File.Exists(filepath)) {
                return;
            }
            StreamReader reader = File.OpenText(filepath);
            try {
                String line = null;
                while (null != (line = reader.ReadLine())) {
                    line = line.Trim();
                    if ("".Equals(line)) {
                        continue;
                    }
                    this.add(Scoring.deserialize(line));
                }
            } finally {
                reader.Close();
            }
        }

        public void storeForScene(String sceneName) {
            String filepath = this.getFilenameFromScenename(sceneName);
            StreamWriter writer = null;
            if (File.Exists(filepath)) {
                File.Delete(filepath);
            }
            writer = File.CreateText(filepath);

            try {
                foreach (Scoring score in this.scores) {
                    writer.Write(score.serialize());
                    writer.Write("\n");
                }
            } finally {
                writer.Flush();
                writer.Close();
            }
        }

        private String getFilenameFromScenename(String sceneName) {
            String filepath = sceneName.ToLower();
            filepath = filepath.Replace(" ", "_");
            filepath = filepath.Replace("\t", "_");
            filepath += ".score";
            return filepath;
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
            for (int i = start; i < end && i < this.scores.Count; i++) {
                result.Add(this.scores.ElementAt(i));
            }
            return result;
        }
    }
}
