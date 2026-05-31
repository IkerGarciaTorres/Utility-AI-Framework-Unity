using UnityEngine;
using System.Collections.Generic;

namespace UtilityAI
{
    public class TickScheduler : MonoBehaviour
    {
        public static TickScheduler Instance { get; private set; }

        private List<ITickableUtility> registeredAgents = new List<ITickableUtility>();
        private int currentIndex = 0;

        [Header("Execution Mode")]
        [Tooltip("Enable to distribute AI load across multiple frames. Disable for all agents to think every frame")]
        public bool useTimeSlicing = true;

        [Header("Performance Configuration")]
        [Tooltip("Time in seconds between each reasoning cycle for the same agent")]
        public float tickInterval = 0.2f;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
                return;
            }
            Instance = this;
        }

        public void Register(ITickableUtility agent)
        {
            if (!registeredAgents.Contains(agent))
            {
                registeredAgents.Add(agent);
            }
        }

        public void Unregister(ITickableUtility agent)
        {
            registeredAgents.Remove(agent);
        }

        private void Update()
        {
            int totalAgents = registeredAgents.Count;
            if (totalAgents == 0) return;

            if (!useTimeSlicing || tickInterval <= 0f)
            {
                for (int i = 0; i < totalAgents; i++)
                {
                    registeredAgents[i].TickUtility();
                }
                return;
            }

            float frameDuration = Time.deltaTime;
            int agentsToTickThisFrame = Mathf.CeilToInt((totalAgents * frameDuration) / tickInterval);

            agentsToTickThisFrame = Mathf.Min(agentsToTickThisFrame, totalAgents);

            for (int i = 0; i < agentsToTickThisFrame; i++)
            {
                if (currentIndex >= totalAgents)
                {
                    currentIndex = 0;
                }

                registeredAgents[currentIndex].TickUtility();
                currentIndex++;
            }
        }
    }
}