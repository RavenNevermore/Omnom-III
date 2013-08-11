using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Omnom_III_Game.exceptions;
using Omnom_III_Game.util;

namespace Omnom_III_Game {
    public class Song {
        public enum MusicTime { 
            FULL, 
            HALF, HALF_DOTTED, 
            QUARTER, QUARTER_DOTTED, 
            EIGTH, EIGTH_DOTTED, 
            SIXTEENTH, SIXTEENTH_DOTTED }

        public static float MusicTimeInFractions(MusicTime time, Boolean asPartOfTriole) {
            float t = Song.MusicTimeInFractions(time);
            if (asPartOfTriole) {
                t = 2f / 3f * t;
            }
            return t;
        }

        public static float MusicTimeInFractions(MusicTime time) {
            if (MusicTime.FULL == time)
                return 1.0f;
            else if (MusicTime.HALF == time)
                return .5f;
            else if (MusicTime.HALF_DOTTED == time)
                return .75f;
            else if (MusicTime.QUARTER == time)
                return .25f;
            else if (MusicTime.QUARTER_DOTTED == time)
                return .375f;
            else if (MusicTime.EIGTH == time)
                return .125f;
            else if (MusicTime.EIGTH_DOTTED == time)
                return .1875f;
            else if (MusicTime.SIXTEENTH == time)
                return .0625f;
            else if (MusicTime.SIXTEENTH_DOTTED == time)
                return .09375f;
            return 0;
        }

        public static MusicTime FractionsInMusicTime(float fraction, bool dotted) {
            if (dotted) {
                fraction += fraction / 2;
            }

            if (1.0f == fraction)
                return MusicTime.FULL;
            else if (.75f == fraction)
                return MusicTime.HALF_DOTTED;
            else if (.5f == fraction)
                return MusicTime.HALF;
            else if (.375f == fraction)
                return MusicTime.QUARTER_DOTTED;
            else if (.25f == fraction)
                return MusicTime.QUARTER;
            else if (.1875f == fraction)
                return MusicTime.EIGTH_DOTTED;
            else if (.125f == fraction)
                return MusicTime.EIGTH;
            else if (.09375f == fraction)
                return MusicTime.SIXTEENTH_DOTTED;
            else if (.0625f == fraction)
                return MusicTime.SIXTEENTH;

            throw new ArgumentOutOfRangeException("Fraction is not a valid musictime fraction.");
        }


        private Microsoft.Xna.Framework.Media.Song content;
        private float beatsPerMillisecond;
        public float timeShift;

        public float bpm {
            get {
                return this.beatsPerMillisecond * 1000 * 60;
            }

            set {
                this.beatsPerMillisecond = (value / 60) / 1000;
            }
        }

        public float bpms { get { return this.beatsPerMillisecond; } }

        public float beatTimeInMs {
            get { return 1 / this.beatsPerMillisecond; }
            set { }
        }

        public float measureTimeInMs {
            get { return this.beatTimeInMs * 4; }
            set { }
        }

        public float positionInBeat {
            get { 
                float beatMs = this.timeRunningInMs - ((this.timeRunningInBeats - 1) * this.beatTimeInMs);
                return beatMs / this.beatTimeInMs;
            }
            set { }
        }

        private long _timeRunningInMs;
        public long timeRunningInMs { 
            get {
                return this._timeRunningInMs;
            } 
        }

        public int timeRunningInBeats {
            get {
                return 1 + (int)(this.timeRunningInMs * this.beatsPerMillisecond);
            }
        }

        public float timeRunningInMeasures {
            get {
                return (float)this.timeRunningInMs * (this.beatsPerMillisecond / 4.0f);
            }
        }

        public float positionInMeasure {
            get {
                int measure = this.timeRunningInBeats / 4;
                float measureTime = this.beatTimeInMs * 4;
                float measureMs = this.timeRunningInMs - (measure * measureTime);
                return measureMs / measureTime;
            }
        }

        
        public Song(String contentName, ContentUtil content, float bpm) {
            this.bpm = bpm;
            this.content = content.load<Microsoft.Xna.Framework.Media.Song>(contentName);
        }

        public Song(ContentScript script, ContentUtil content)
            : this(script["song"][0], content, script.asFloat["tempo"][0]) {
            
            if (null != script["startshift"])
                this.timeShift = this.beatTimeInMs * script.asFloat["startshift"][0];
        }

        public void play() {
            if (this.isActive() && MediaState.Paused == MediaPlayer.State) {
                MediaPlayer.Resume();
            } else {
                MediaPlayer.Play(this.content);
            }
        }

        public void calculateMetaInfo() {
            uint position = this.isActive() ? 
                (uint) MediaPlayer.PlayPosition.TotalMilliseconds : 0;
            
            this._timeRunningInMs = (long) position;
            this._timeRunningInMs += (long)this.timeShift;
        }



        internal bool stoppedPlaying() {
            return !this.isActive() ||
                MediaState.Stopped == MediaPlayer.State;
        }

        private bool isActive() {
            Microsoft.Xna.Framework.Media.Song activeSong = MediaPlayer.Queue.ActiveSong;
            return null != activeSong && activeSong.Equals(this.content);
        }

        internal void stop() {
            if (this.isActive())
                MediaPlayer.Stop();
        }

        internal void reset() {
            this.stop();
        }

        internal void pause() {
            if (!this.isActive())
                return;
            if (MediaState.Playing == MediaPlayer.State) {
                MediaPlayer.Pause();
            }
        }
    }
}
