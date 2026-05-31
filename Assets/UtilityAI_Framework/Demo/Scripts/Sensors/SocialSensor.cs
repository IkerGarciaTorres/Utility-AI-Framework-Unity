using System.Collections;
using UnityEngine;

namespace UtilityAI.Sensors
{
    public class SocialSensor : MonoBehaviour
    {
        public float detectionRadius = 4f;
        public string villagerTag = "Villager";

        [Tooltip("Time in seconds between each social scan")]
        public float scanInterval = 0.25f;

        private DynamicBlackboard _blackboard;

        private void Awake() => _blackboard = GetComponent<DynamicBlackboard>();

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
            if (_blackboard == null) return;

            if (_blackboard.Exists("ChatOffer") && _blackboard.Get<GameObject>("ChatOffer") != null)
            {
                _blackboard.Put("ClosestFreeVillager", (GameObject)null);
                return;
            }

            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, detectionRadius);
            GameObject bestTarget = null;
            float minDistance = float.MaxValue;

            GameObject currentTarget = _blackboard.Exists("ClosestFreeVillager")
                ? _blackboard.Get<GameObject>("ClosestFreeVillager")
                : null;

            foreach (var hit in hitColliders)
            {
                if (!hit.CompareTag(villagerTag) || hit.gameObject == this.gameObject)
                    continue;

                DynamicBlackboard targetBoard = hit.GetComponent<DynamicBlackboard>();
                if (targetBoard == null)
                    continue;

                bool isFree = !targetBoard.Exists("ChatOffer") || targetBoard.Get<GameObject>("ChatOffer") == null;
                bool isOfferingToMe = targetBoard.Exists("ChatOffer") && targetBoard.Get<GameObject>("ChatOffer") == this.gameObject;

                if (!isFree && !isOfferingToMe)
                    continue;

                if (hit.gameObject == currentTarget)
                {
                    bestTarget = hit.gameObject;
                    break;
                }

                float distance = Vector2.Distance(transform.position, hit.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    bestTarget = hit.gameObject;
                }
            }

            _blackboard.Put("ClosestFreeVillager", bestTarget);
        }
    }
}