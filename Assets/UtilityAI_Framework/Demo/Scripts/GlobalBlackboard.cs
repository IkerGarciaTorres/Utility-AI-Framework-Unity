using System.Collections.Generic;
using UnityEngine;

namespace UtilityAI.Environment
{
    public class GlobalBlackboard : MonoBehaviour
    {
        public static GlobalBlackboard Instance { get; private set; }

        private List<ResourceStation> _allStations = new List<ResourceStation>();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                Instance = this;
            }
        }

        public void RegisterStation(ResourceStation station)
        {
            if (!_allStations.Contains(station))
            {
                _allStations.Add(station);
            }
        }

        public void UnregisterStation(ResourceStation station)
        {
            if (_allStations.Contains(station))
            {
                _allStations.Remove(station);
            }
        }

        public ResourceStation GetClosestAvailableStation(ResourceType type, Vector2 agentPosition)
        {
            ResourceStation bestStation = null;
            float closestDistance = float.MaxValue;

            foreach (var station in _allStations)
            {
                if (station.stationType == type && !station.isOccupied)
                {
                    float distance = Vector2.Distance(agentPosition, station.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        bestStation = station;
                    }
                }
            }

            return bestStation;
        }

        public void ClaimStation(ResourceStation station)
        {
            if (station != null)
            {
                station.isOccupied = true;
            }
        }

        public void ReleaseStation(ResourceStation station)
        {
            if (station != null)
            {
                station.isOccupied = false;
            }
        }
    }
}