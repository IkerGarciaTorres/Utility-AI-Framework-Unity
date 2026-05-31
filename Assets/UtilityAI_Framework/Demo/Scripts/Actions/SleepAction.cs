using UnityEngine;
using UnityEngine.AI;
using UtilityAI.Environment;
using UtilityAI.Sensors;

namespace UtilityAI.Actions
{
    [CreateAssetMenu(fileName = "SleepAction", menuName = "UtilityAI/Actions/Sleep")]
    public class SleepAction : Action
    {
        [Header("Sleep Settings")]
        public float sleepRecoveryRate = 20f;
        public float stoppingDistance = 0.5f;

        [Tooltip("Multiplier applied to hunger decay while sleeping")]
        [Range(0f, 1f)] public float sleepHungerMultiplier = 0.3f;

        [Header("Visual Setting")]
        public Color actionColor = Color.blue;

        private SpriteRenderer _spriteRenderer;
        private NeedsManager _needsManager;
        private NavMeshAgent _agent;
        private ResourceStation _targetBed;
        private bool _isSleeping;
        private float _originalHungerDecay;

        public override void Contextualize(GameObject gm, Brain brainContext)
        {
            base.Contextualize(gm, brainContext);
            _needsManager = gm.GetComponent<NeedsManager>();
            _agent = gm.GetComponent<NavMeshAgent>();
            _spriteRenderer = gm.GetComponent<SpriteRenderer>();
        }

        public override void OnStart()
        {
            _targetBed = blackboard.Get<ResourceStation>("ClosestAvailable_Bed");
            _isSleeping = false;

            if (_targetBed != null && _agent != null)
            {
                if (_targetBed.isOccupied)
                {
                    _targetBed = null;
                    blackboard.Put("ClosestAvailable_Bed", (ResourceStation)null);
                    return;
                }

                blackboard.Put("TargetStation_Bed", _targetBed);
                GlobalBlackboard.Instance.ClaimStation(_targetBed);
                _agent.isStopped = false;
                _agent.SetDestination(_targetBed.interactionPoint.position);
            }
        }

        public override void Execute()
        {
            if (_needsManager == null || _agent == null)
                return;

            if (_targetBed == null)
            {
                ResourceStation newTarget = blackboard.Get<ResourceStation>("ClosestAvailable_Bed");
                if (newTarget != null && !newTarget.isOccupied)
                {
                    _targetBed = newTarget;
                    blackboard.Put("TargetStation_Bed", _targetBed);
                    GlobalBlackboard.Instance.ClaimStation(_targetBed);

                    _agent.isStopped = false;
                    _agent.SetDestination(_targetBed.interactionPoint.position);
                }
                return;
            }

            if (_agent.pathPending || _agent.remainingDistance > stoppingDistance)
                return;

            if (!_agent.isStopped)
            {
                _agent.isStopped = true;
                _agent.ResetPath();
            }

            if (!_isSleeping)
            {
                _isSleeping = true;
                if (_spriteRenderer != null) _spriteRenderer.color = actionColor;

                _needsManager.pauseEnergyDecay = true;
                _needsManager.pauseSocialDecay = true;

                _originalHungerDecay = _needsManager.hungerDecayRate;
                _needsManager.hungerDecayRate = _originalHungerDecay * sleepHungerMultiplier;
            }

            _needsManager.Rest(sleepRecoveryRate * Time.deltaTime);
        }

        public override void OnStop()
        {
            if (_needsManager != null)
            {
                _needsManager.pauseEnergyDecay = false;
                _needsManager.pauseSocialDecay = false;

                if (_isSleeping)
                {
                    _needsManager.hungerDecayRate = _originalHungerDecay;
                }
            }

            if (_targetBed != null)
            {
                GlobalBlackboard.Instance.ReleaseStation(_targetBed);
                _targetBed = null;
                blackboard.Put("TargetStation_Bed", null);
            }

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