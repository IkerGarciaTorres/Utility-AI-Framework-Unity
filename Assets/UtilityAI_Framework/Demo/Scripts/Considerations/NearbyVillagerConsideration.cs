using UnityEngine;

namespace UtilityAI.Considerations
{
    [CreateAssetMenu(fileName = "Cons_NearbyVillager", menuName = "UtilityAI/Considerations/Nearby Villager")]
    public class NearbyVillagerConsideration : Consideration
    {
        protected override float GetRawInput()
        {
            if (blackboard == null)
                return 0f;

            if (blackboard.Exists("ChatOffer") && blackboard.Get<GameObject>("ChatOffer") != null)
                return 0f;

            if (blackboard.Exists("ClosestFreeVillager") && blackboard.Get<GameObject>("ClosestFreeVillager") != null)
                return 1f;

            return 0f;
        }
    }
}