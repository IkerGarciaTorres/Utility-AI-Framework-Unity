using UnityEngine;

namespace UtilityAI.Environment
{
    public enum ResourceType
    {
        Bed,
        Workstation,
        FoodShop
    }

    public class ResourceStation : MonoBehaviour
    {
        [Header("Station Configuration")]
        public ResourceType stationType;

        [Tooltip("Where the agent should stand to use this resource")]
        public Transform interactionPoint;

        [Header("Runtime State")]
        public bool isOccupied = false;

        [Header("Economy Settings")]
        public int pricePerUnit = 2;

        private void Start()
        {
            if (GlobalBlackboard.Instance != null)
            {
                GlobalBlackboard.Instance.RegisterStation(this);
            }

            if (interactionPoint == null)
            {
                interactionPoint = this.transform;
            }
        }

        private void OnDestroy()
        {
            if (GlobalBlackboard.Instance != null)
            {
                GlobalBlackboard.Instance.UnregisterStation(this);
            }
        }
    }
}