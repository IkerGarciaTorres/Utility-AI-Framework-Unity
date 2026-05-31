using UnityEngine;
using UnityEngine.AI;

namespace UtilityAI.Navigation
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class NavMesh2DSetup : MonoBehaviour
    {
        private void Awake()
        {
            NavMeshAgent agent = GetComponent<NavMeshAgent>();

            agent.updateRotation = false;
            agent.updateUpAxis = false;
        }
    }
}