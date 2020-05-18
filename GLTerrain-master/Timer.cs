using System;

namespace GLTerrain {
    public class Timer {
        public double Period { get; private set; }
        public bool IsLooping { get; private set; }
        public bool IsFrameBased { get; private set; }
        public long FiredCount { get; private set; }

        private Action<double> timeFunc = null;
        private double elapsed = 0.0;
        private int framesElapsed = 0;
        private int framesToSkip = 0;

        public Timer(double period, bool loop, Action<double> func) {
            Period = period;
            IsLooping = loop;
            IsFrameBased = false;
            timeFunc = func;
        }

        public Timer(int framesPerTick, bool loop, Action<double> func) {
            Period = (double) framesPerTick;
            framesToSkip = framesPerTick;

            IsLooping = loop;
            IsFrameBased = true;
            timeFunc = func;
        }

        public void Update(double lastFrameTime) {
            if (timeFunc == null || !IsLooping && FiredCount > 0) {
                return;
            }

            elapsed += lastFrameTime;
            if (IsFrameBased) {

                framesElapsed++;
                if (framesElapsed >= framesToSkip) {
                    FiredCount++;
                    timeFunc(elapsed);
                    framesElapsed -= framesToSkip;
                }

            } else {

                if (elapsed > Period) {
                    FiredCount++;
                    timeFunc(elapsed);
                    elapsed -= Period;
                }

            }
        }
    }
}
