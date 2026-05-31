using UnityEngine;

namespace UtilityAI.Considerations
{
    [CreateAssetMenu(fileName = "Cons_Constant", menuName = "UtilityAI/Considerations/Constant Baseline")]
    public class ConstantConsideration : Consideration
    {
        [Header("Baseline Settings")]
        [Tooltip("The flat utility value this consideration will always output.")]
        [Range(0f, 1f)] public float baselineUtility = 0.2f;

        protected override float GetRawInput()
        {
            return baselineUtility;
        }
    }
}