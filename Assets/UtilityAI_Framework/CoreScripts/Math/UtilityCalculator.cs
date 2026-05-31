using UnityEngine;
using System.Collections.Generic;

namespace UtilityAI.Math
{
    public static class UtilityCalculator
    {
        public static float EvaluateConsideration(AnimationCurve curve, float input, float offset = 0f, float multiplier = 1f)
        {
            float adjustedInput = Mathf.Clamp01(input + offset);
            float rawUtility = curve.Evaluate(adjustedInput);
            return Mathf.Clamp01(rawUtility * multiplier);
        }

        public static float CalculateActionUtility(List<float> considerationScores)
        {
            int count = considerationScores.Count;

            if (count == 0)
                return 0f;

            float product = 1f;
            for (int i = 0; i < count; i++)
            {
                float score = considerationScores[i];
                if (score <= 0f)
                    return 0f;

                product *= score;
            }
            return Mathf.Pow(product, 1f / count);
        }

        public static float ApplyInertia(float currentUtility, float inertiaBonus)
        {
            return Mathf.Clamp01(currentUtility + inertiaBonus);
        }

        public static float EvaluateLinear(float x, float m, float b)
        {
            return (m * x) + b;
        }

        public static float EvaluatePolynomial(float x, float k)
        {
            if (x < 0)
                x = 0;
            return Mathf.Pow(x, k);
        }

        public static float EvaluateLogistic(float x, float midpoint, float steepness)
        {
            return 1f / (1f + Mathf.Exp(-steepness * (x - midpoint)));
        }

        public static float EvaluateGaussian(float x, float mean, float stdDev)
        {
            float numerator = -Mathf.Pow(x - mean, 2);
            float denominator = 2f * Mathf.Pow(stdDev, 2);
            return Mathf.Exp(numerator / denominator);
        }
    }
}