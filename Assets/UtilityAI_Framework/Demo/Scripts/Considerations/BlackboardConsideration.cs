using UnityEngine;

namespace UtilityAI.Considerations
{
    [CreateAssetMenu(menuName = "UtilityAI/Considerations/Blackboard Reader")]
    public class BlackboardConsideration : Consideration
    {
        [Header("Reading Settings")]
        [Tooltip("The exact name of the variable on the board")]
        public string blackboardKey;

        [Tooltip("Value returned if the sensor has not yet written anything to the board")]
        public float fallbackValue = 0f;

        protected override float GetRawInput()
        {
            if (blackboard != null && blackboard.Exists(blackboardKey))
            {
                return blackboard.Get<float>(blackboardKey);
            }

            return fallbackValue;
        }
    }
}