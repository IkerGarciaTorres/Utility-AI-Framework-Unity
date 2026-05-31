using UnityEngine;
using UnityEngine.AI;
using UtilityAI.Environment;
using UtilityAI.Sensors;

namespace UtilityAI.Actions
{
    [CreateAssetMenu(fileName = "WorkAction", menuName = "UtilityAI/Actions/Work")]
    public class WorkAction : Action
    {
        [Header("Work Settings")]
        public float incomeRate = 5f;
        public float stoppingDistance = 0.5f;

        [Tooltip("Multiplier applied to energy decay while working")]
        [Range(1f, 5f)] public float workFatigueMultiplier = 1.5f;

        [Header("Visual Setting")]
        public Color actionColor = new Color(0.8f, 0.4f, 0f);

        private SpriteRenderer _spriteRenderer;
        private NeedsManager _needsManager;
        private NavMeshAgent _agent;
        private ResourceStation _targetWorkstation;
        private bool _isWorking;
        private float _originalEnergyDecay;
        private float _moneyAccumulator;

        public override void Contextualize(GameObject gm, Brain brainContext)
        {
            base.Contextualize(gm, brainContext);
            _needsManager = gm.GetComponent<NeedsManager>();
            _agent = gm.GetComponent<NavMeshAgent>();
            _spriteRenderer = gm.GetComponent<SpriteRenderer>();
        }

        public override void OnStart()
        {
            _targetWorkstation = blackboard.Get<ResourceStation>("ClosestAvailable_Workstation");
            _isWorking = false;
            _moneyAccumulator = 0f;

            if (_targetWorkstation != null && _agent != null)
            {
                if (_targetWorkstation.isOccupied)
                {
                    _targetWorkstation = null;
                    blackboard.Put("ClosestAvailable_Workstation", (ResourceStation)null);
                    return;
                }

                blackboard.Put("TargetStation_Workstation", _targetWorkstation);
                GlobalBlackboard.Instance.ClaimStation(_targetWorkstation);
                _agent.isStopped = false;
                _agent.SetDestination(_targetWorkstation.interactionPoint.position);
            }
        }

        public override void Execute()
        {
            if (_needsManager == null || _agent == null)
                return;

            if (_targetWorkstation == null)
            {
                ResourceStation newTarget = blackboard.Get<ResourceStation>("ClosestAvailable_Workstation");
                if (newTarget != null && !newTarget.isOccupied)
                {
                    _targetWorkstation = newTarget;
                    blackboard.Put("TargetStation_Workstation", _targetWorkstation);
                    GlobalBlackboard.Instance.ClaimStation(_targetWorkstation);

                    _agent.isStopped = false;
                    _agent.SetDestination(_targetWorkstation.interactionPoint.position);
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

            if (!_isWorking)
            {
                _isWorking = true;
                if (_spriteRenderer != null) _spriteRenderer.color = actionColor;

                _originalEnergyDecay = _needsManager.energyDecayRate;
                _needsManager.energyDecayRate = _originalEnergyDecay * workFatigueMultiplier;
            }

            _moneyAccumulator += incomeRate * Time.deltaTime;

            if (_moneyAccumulator >= 1f)
            {
                int moneyToAdd = Mathf.FloorToInt(_moneyAccumulator);
                _needsManager.EarnMoney(moneyToAdd);
                _moneyAccumulator -= moneyToAdd;
            }
        }

        public override void OnStop()
        {
            if (_needsManager != null && _isWorking)
            {
                _needsManager.energyDecayRate = _originalEnergyDecay;
            }

            if (_targetWorkstation != null)
            {
                GlobalBlackboard.Instance.ReleaseStation(_targetWorkstation);
                _targetWorkstation = null;
                blackboard.Put("TargetStation_Workstation", null);
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