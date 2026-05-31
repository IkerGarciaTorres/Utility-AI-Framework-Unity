using UnityEngine;
using UnityEngine.AI;

namespace UtilityAI.Actions
{
    [CreateAssetMenu(fileName = "WanderAction", menuName = "UtilityAI/Actions/Wander")]
    public class WanderAroundAction : Action
    {
        [Header("Wander Settings")]
        public float wanderRadius = 8f;
        public float minWaitTime = 2f;
        public float maxWaitTime = 6f;

        [Header("Center Gravity")]
        [Tooltip("The center point of the village")]
        public Vector3 mapCenter = Vector3.zero;

        [Range(0f, 1f)] public float centerBias = 0.2f;

        [Header("Visual Debug")]
        public Color actionColor = Color.gray;
        private SpriteRenderer _spriteRenderer;

        private NavMeshAgent _agent;
        private float _waitTimer;
        private bool _isWaiting;

        public override void Contextualize(GameObject gm, Brain brainContext)
        {
            base.Contextualize(gm, brainContext);
            _agent = gm.GetComponent<NavMeshAgent>();
            _spriteRenderer = gm.GetComponentInChildren<SpriteRenderer>();
        }

        public override void OnStart()
        {
            if (_spriteRenderer != null)
            {
                _spriteRenderer.color = actionColor;
            }

            SetNewRandomDestination();
        }

        public override void Execute()
        {
            if (_agent == null)
                return;

            if (!_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance)
            {
                if (!_isWaiting)
                {
                    _isWaiting = true;
                    _waitTimer = Random.Range(minWaitTime, maxWaitTime);
                }
                else
                {
                    _waitTimer -= Time.deltaTime;
                    if (_waitTimer <= 0f)
                    {
                        SetNewRandomDestination();
                    }
                }
            }
        }

        private void SetNewRandomDestination()
        {
            _isWaiting = false;

            Vector2 random2D = Random.insideUnitCircle.normalized;
            Vector3 randomDirection = new Vector3(random2D.x, random2D.y, 0f);

            Vector3 toCenterDirection = (mapCenter - _agent.transform.position).normalized;
            toCenterDirection.z = 0f;

            Vector3 finalDirection = Vector3.Lerp(randomDirection, toCenterDirection, centerBias).normalized;
            Vector3 targetPosition = _agent.transform.position + (finalDirection * wanderRadius);

            NavMeshHit hit;
            if (NavMesh.SamplePosition(targetPosition, out hit, wanderRadius, NavMesh.AllAreas))
            {
                _agent.SetDestination(hit.position);
                _agent.isStopped = false;
            }
        }

        public override void OnStop()
        {
            if (_spriteRenderer != null && blackboard.Exists("OriginalColor"))
            {
                _spriteRenderer.color = blackboard.Get<Color>("OriginalColor");
            }

            if (_agent != null && _agent.isOnNavMesh)
            {
                _agent.isStopped = true;
                _agent.ResetPath();
            }
        }
    }
}