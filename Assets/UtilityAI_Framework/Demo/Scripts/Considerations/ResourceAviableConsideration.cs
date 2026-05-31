using UnityEngine;
using UtilityAI.Environment;

namespace UtilityAI.Considerations
{
    [CreateAssetMenu(fileName = "Cons_ResourceAviable", menuName = "UtilityAI/Considerations/Resource Aviable")]
    public class ResourceAviableConsideration : Consideration
    {
        public ResourceType targetResource;

        protected override float GetRawInput()
        {
            if (blackboard == null) return 0f;

            string actualKey = "TargetStation_" + targetResource.ToString();
            if (blackboard.Exists(actualKey) && blackboard.Get<ResourceStation>(actualKey) != null) 
                return 1f;

            string sensorKey = "ClosestAvailable_" + targetResource.ToString();
            if (blackboard.Exists(sensorKey) && blackboard.Get<ResourceStation>(sensorKey) != null) 
                return 1f;

            return 0f;
        }
    }
}