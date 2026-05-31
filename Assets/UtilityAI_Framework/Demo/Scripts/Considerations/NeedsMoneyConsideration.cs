using UnityEngine;

namespace UtilityAI.Considerations
{
    [CreateAssetMenu(fileName = "Cons_NeedsMoney", menuName = "UtilityAI/Considerations/Needs Money")]
    public class NeedsMoneyConsideration : Consideration
    {
        [Header("Fallback Settings")]
        [Tooltip("Safety value in case NeedsManager did not upload the TargetWealth to the board.")]
        public float defaultTargetWealth = 50f;

        protected override float GetRawInput()
        {
            if (blackboard == null) return 0f;

            float currentMoney = blackboard.Get<int>("Money");

            float targetWealth = defaultTargetWealth;
            if (blackboard.Exists("TargetWealth"))
            {
                targetWealth = blackboard.Get<int>("TargetWealth");
            }

            targetWealth = Mathf.Max(1f, targetWealth);

            float normalizedWealth = Mathf.Clamp01(currentMoney / targetWealth);

            return normalizedWealth;
        }
    }
}