using UnityEngine;

namespace UtilityAI.Traits
{
    [CreateAssetMenu(menuName = "UtilityAI/Traits/Trait")]
    public class Trait : ScriptableObject
    {
        [Header("Identification")]
        public string traitName = "New Trait";

        [Tooltip("The exact ID of the consideration it affects")]
        public string targetConsiderationID;

        [Header("Mathematical Modifiers")]
        [Tooltip("Shifts the curve on the X Axis")]
        [Range(-1f, 1f)]
        public float thresholdOffset = 0f;

        [Tooltip("Scales the curve on the Y Axis")]
        [Range(0f, 5f)]
        public float importanceMultiplier = 1f;
    }
}