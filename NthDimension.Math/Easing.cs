﻿using System;

namespace NthDimension.Algebra
{
    public static class Easing
    {
        // Motion Easing :: http://theinstructionlimit.com/?m=200907

        // Adapted from source : http://www.robertpenner.com/easing/

        public static float Ease(double linearStep, float acceleration, EasingType type)
        {
            float easedStep = acceleration > 0 ? EaseIn(linearStep, type) :
                              acceleration < 0 ? EaseOut(linearStep, type) :
                              (float)linearStep;

            return MathFunc.Lerp(linearStep, easedStep, System.Math.Abs(acceleration));
        }

        public static float EaseIn(double linearStep, EasingType type)
        {
            switch (type)
            {
                case EasingType.Step: return linearStep < 0.5 ? 0 : 1;
                case EasingType.Linear: return (float)linearStep;
                case EasingType.Sine: return Sine.EaseIn(linearStep);
                case EasingType.Quadratic: return Power.EaseIn(linearStep, 2);
                case EasingType.Cubic: return Power.EaseIn(linearStep, 3);
                case EasingType.Quartic: return Power.EaseIn(linearStep, 4);
                case EasingType.Quintic: return Power.EaseIn(linearStep, 5);
            }
            throw new NotImplementedException();
        }

        public static float EaseOut(double linearStep, EasingType type)
        {
            switch (type)
            {
                case EasingType.Step: return linearStep < 0.5 ? 0 : 1;
                case EasingType.Linear: return (float)linearStep;
                case EasingType.Sine: return Sine.EaseOut(linearStep);
                case EasingType.Quadratic: return Power.EaseOut(linearStep, 2);
                case EasingType.Cubic: return Power.EaseOut(linearStep, 3);
                case EasingType.Quartic: return Power.EaseOut(linearStep, 4);
                case EasingType.Quintic: return Power.EaseOut(linearStep, 5);
            }
            throw new NotImplementedException();
        }

        public static float EaseInOut(double linearStep, EasingType easeInType, EasingType easeOutType)
        {
            return linearStep < 0.5 ? EaseInOut(linearStep, easeInType) : EaseInOut(linearStep, easeOutType);
        }
        public static float EaseInOut(double linearStep, EasingType type)
        {
            switch (type)
            {
                case EasingType.Step: return linearStep < 0.5 ? 0 : 1;
                case EasingType.Linear: return (float)linearStep;
                case EasingType.Sine: return Sine.EaseInOut(linearStep);
                case EasingType.Quadratic: return Power.EaseInOut(linearStep, 2);
                case EasingType.Cubic: return Power.EaseInOut(linearStep, 3);
                case EasingType.Quartic: return Power.EaseInOut(linearStep, 4);
                case EasingType.Quintic: return Power.EaseInOut(linearStep, 5);
            }
            throw new NotImplementedException();
        }

        static class Sine
        {
            public static float EaseIn(double s)
            {
                return (float)System.Math.Sin(s * MathFunc.HalfPi - MathFunc.HalfPi) + 1;
            }
            public static float EaseOut(double s)
            {
                return (float)System.Math.Sin(s * MathFunc.HalfPi);
            }
            public static float EaseInOut(double s)
            {
                return (float)(System.Math.Sin(s * MathFunc.Pi - MathFunc.HalfPi) + 1) / 2;
            }
        }

        static class Power
        {
            public static float EaseIn(double s, int power)
            {
                return (float)System.Math.Pow(s, power);
            }
            public static float EaseOut(double s, int power)
            {
                var sign = power % 2 == 0 ? -1 : 1;
                return (float)(sign * (System.Math.Pow(s - 1, power) + sign));
            }
            public static float EaseInOut(double s, int power)
            {
                s *= 2;
                if (s < 1) return EaseIn(s, power) / 2;
                var sign = power % 2 == 0 ? -1 : 1;
                return (float)(sign / 2.0 * (System.Math.Pow(s - 2, power) + sign * 2));
            }
        }
    }

    public enum EasingType
    {
        Step,
        Linear,
        Sine,
        Quadratic,
        Cubic,
        Quartic,
        Quintic
    }


}
