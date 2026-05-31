using UnityEngine;
using UtilityAI.Math;

namespace UtilityAI
{
    public enum MathCurveType { Manual, Linear, Polynomial, Logistic, Gaussian }

    public abstract class Consideration : ScriptableObject, IUtilityNode
    {
        [Header("Base Configuration")]
        public string considerationName;
        public string traitID;

        [Header("Curve Type")]
        public MathCurveType curveType = MathCurveType.Manual;

        [Header("Linear Parameters")]
        [Tooltip("Slope (m) Determines the incline of the line")]
        public float m = 1f;
        [Tooltip("Y-Intercept (b) Point where the line crosses the Y axis when X is 0")]
        public float b = 0f;

        [Header("Polynomial Parameters")]
        [Tooltip("Exponent (k) Values > 1 create accelerating curves, < 1 create decelerating curves")]
        public float k = 2f;

        [Header("Logistic Parameters")]
        [Tooltip("Midpoint (x0) X value where the curve changes concavity")]
        public float midpoint = 0.5f;
        [Tooltip("Hill slope (k) Controls the aggressiveness of the transition at the center")]
        public float steepness = 10f;

        [Header("Gaussian Parameters")]
        [Tooltip("Mean (μ) The input value that grants maximum utility (1.0)")]
        public float mean = 0.5f;
        [Tooltip("Standard Deviation (σ) Controls the width or range of the 'sweet spot'")]
        public float stdDev = 0.15f;

        [Space(10)]
        public AnimationCurve responseCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

        public GameObject gameObject { get; private set; }
        protected DynamicBlackboard blackboard;
        protected Brain brainContext;

        public string Name => considerationName;

        public virtual void Contextualize(GameObject gm)
        {
            this.gameObject = gm;
            this.blackboard = gm.GetComponent<DynamicBlackboard>();
            this.brainContext = gm.GetComponent<Brain>();
        }

        public float Evaluate()
        {
            float rawInput = GetRawInput();
            float offset = 0f;
            float multiplier = 1f;

            if (brainContext != null && brainContext.HasPersonality())
            {
                brainContext.GetTraitModifiers(traitID, out offset, out multiplier);
            }

            return UtilityCalculator.EvaluateConsideration(responseCurve, rawInput, offset, multiplier);
        }

        protected abstract float GetRawInput();

        public virtual void Clear() { }
    }
}