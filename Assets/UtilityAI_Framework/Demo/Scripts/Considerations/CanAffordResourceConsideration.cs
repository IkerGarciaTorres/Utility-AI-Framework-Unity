using UnityEngine;
using UtilityAI.Environment;

namespace UtilityAI.Considerations
{
    [CreateAssetMenu(fileName = "Cons_CanAffordResource", menuName = "UtilityAI/Considerations/Can Afford Resource")]
    public class CanAffordResourceConsideration : Consideration
    {
        public ResourceType targetResource = ResourceType.FoodShop;

        protected override float GetRawInput()
        {
            if (blackboard == null)
                return 0f;

            int currentMoney = blackboard.Get<int>("Money");

            ResourceStation targetStation = null;
            string actualKey = "TargetStation_" + targetResource.ToString();
            string sensorKey = "ClosestAvailable_" + targetResource.ToString();

            if (blackboard.Exists(actualKey) && blackboard.Get<ResourceStation>(actualKey) != null)
            {
                targetStation = blackboard.Get<ResourceStation>(actualKey);
            }
            else if (blackboard.Exists(sensorKey) && blackboard.Get<ResourceStation>(sensorKey) != null)
            {
                targetStation = blackboard.Get<ResourceStation>(sensorKey);
            }

            if (targetStation != null && currentMoney >= targetStation.pricePerUnit) 
                return 1f;

            return 0f;
        }
    }
}