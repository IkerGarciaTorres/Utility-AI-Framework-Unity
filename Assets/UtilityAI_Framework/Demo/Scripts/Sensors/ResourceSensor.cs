using System.Collections;
using UnityEngine;
using UtilityAI.Environment;

namespace UtilityAI.Sensors
{
    public class ResourceSensor : MonoBehaviour
    {
        [Tooltip("Time in seconds between each resource scan")]
        public float scanInterval = 0.5f;

        private DynamicBlackboard _blackboard;

        private void Awake()
        {
            _blackboard = GetComponent<DynamicBlackboard>();
        }

        private void OnEnable() => StartCoroutine(ScanRoutine());
        private void OnDisable() => StopAllCoroutines();

        private IEnumerator ScanRoutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(scanInterval);
                Scan();
            }
        }

        private void Scan()
        {
            UpdateResourceTarget(ResourceType.Bed, "ClosestAvailable_Bed");
            UpdateResourceTarget(ResourceType.Workstation, "ClosestAvailable_Workstation");
            UpdateResourceTarget(ResourceType.FoodShop, "ClosestAvailable_FoodShop");
        }

        private void UpdateResourceTarget(ResourceType type, string blackboardKey)
        {
            string officialKey = "TargetStation_" + type.ToString();
            if (_blackboard.Exists(officialKey) && _blackboard.Get<ResourceStation>(officialKey) != null)
                return;

            ResourceStation closest = GlobalBlackboard.Instance.GetClosestAvailableStation(type, transform.position);
            _blackboard.Put(blackboardKey, closest);
        }
    }
}