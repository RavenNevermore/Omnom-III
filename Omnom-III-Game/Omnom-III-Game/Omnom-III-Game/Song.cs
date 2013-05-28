using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FMOD;
using Omnom_III_Game.exceptions;

namespace Omnom_III_Game {
    public class Song {
        public enum MusicTime { 
            FULL, 
            HALF, HALF_DOTTED, 
            QUARTER, QUARTER_DOTTED, 
            EIGTH, EIGTH_DOTTED, 
            SIXTEENTH, SIXTEENTH_DOTTED }

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

        public class Spectrum {
            public static int  DEFAULT_SAMPLESIZE = 64;

            public float[] data;

            private Spectrum(float[] data) {
                this.data = data;
            }

            public Spectrum(int sampleSize) {
                this.data = new float[sampleSize];
               
            }

            public Spectrum(FMOD.Channel channel,int channelOffset) {

                int sampleSize = DEFAULT_SAMPLESIZE;

                float[] specs = new float[sampleSize];

                channel.getSpectrum(specs, sampleSize, channelOffset, FMOD.DSP_FFT_WINDOW.RECT);
                this.data = specs;
            }

            public Spectrum highCut(int cuttedSamples) {
                float[] cutdata = new float[this.data.Length - cuttedSamples];
                for (int i = 0; i < cutdata.Length; i++) {
                    cutdata[i] = this.data[i];
                }
                return new Spectrum(cutdata);
            }

            public Spectrum downSample(int toSize) {
                float[] newdata = new float[toSize];
                double factor = this.data.Length / toSize;
                int floorFactor = (int) Math.Floor(factor);
                int ceilFactor = (int) Math.Ceiling(factor);
                for (int i = 0; i < newdata.Length; i++) {
                    float v = 0;

                    int min = i * floorFactor;
                    int  max = min + ceilFactor;
                    for (int j = min; j < max; j++) {
                        v += this.data[j];
                    }

                    newdata[i] = v / ceilFactor;
                }
                return new Spectrum(newdata);
            }

            public float[] scaledData(float scale) {
                float[] scaled = new float[this.data.Length];
                for (int i = 0; i < this.data.Length; i++) {
                    scaled[i] = normalize(this.data[i]) * scale;
                }
                return scaled;
            }

            private float normalize(float data) {
                return (float) Math.Pow(data, 1 / Math.E);
            }

            public int sampleSize {
                get {
                    return data.Length;
                }
            }

            
        }

        public class SpectralData {
            public Spectrum current;
            public Spectrum max;

            FMOD.Channel channel;
            int offset;

            public SpectralData(FMOD.Channel channel, int offset) {
                this.channel = channel;
                this.offset = offset;
                this.max = new Spectrum(Spectrum.DEFAULT_SAMPLESIZE);
            }

            public void update() {
                this.current = new Spectrum(this.channel, offset);
                for (int i = 0; i < this.current.sampleSize; i++) {
                    float vol = this.current.data[i];
                    if (vol > this.max.data[i]){
                        this.max.data[i] = vol;
                    }
                }
            }
        }

        public class ExtendedSpectralData {
            public SpectralData left;
            public SpectralData right;


            public ExtendedSpectralData(FMOD.Channel channel) {
                this.left = new SpectralData(channel, 0);
                this.right = new SpectralData(channel, 1);
            }

            public void update() {
                this.left.update();
                this.right.update();
            }

        }

        String filename;
        FMOD.System soundsystem;
        FMOD.Sound content;
        FMOD.Channel channel;
        public ExtendedSpectralData spectrum;
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

        public int timeRunningInMeasures {
            get {
                return ((this.timeRunningInBeats - 1) / 4) + 1;
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

        public Song() {
        }

        public Song(String contentName, FMOD.System soundSystem, float bpm) {
            this.bpm = bpm;
            this.soundsystem = soundSystem;
            this.filename = "Content/" + contentName + ".wma";
            ERRCHECK(soundSystem.createSound(filename, FMOD.MODE.SOFTWARE, ref content));
        }

        private void ERRCHECK(FMOD.RESULT result) {
            if (result != FMOD.RESULT.OK) {
                throw new SoundSystemException(result.ToString("X"));
            }
        }

        public void play() {
            this.soundsystem.playSound(FMOD.CHANNELINDEX.FREE, content, false, ref channel);
            this.spectrum = new ExtendedSpectralData(this.channel);
            
        }

        public void calculateMetaInfo() {
            this.soundsystem.update();
            this.spectrum.update();

            uint position = 0;
            this.channel.getPosition(ref position, TIMEUNIT.MS);
            this._timeRunningInMs = (long) position;
            this._timeRunningInMs += (long)this.timeShift;
        }

        
    }
}
