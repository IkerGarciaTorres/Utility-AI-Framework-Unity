using UnityEngine;
using UnityEngine.AI;
using UtilityAI.Sensors;

namespace UtilityAI.Actions
{
    [CreateAssetMenu(fileName = "AcceptChatAction", menuName = "UtilityAI/Actions/Accept Chat")]
    public class AcceptChatAction : Action
    {
        public float talkingDistance = 1.5f;
        public float socialRecoveryRate = 15f;
        public Color chattingColor = Color.magenta;

        private SpriteRenderer _spriteRenderer;
        private NeedsManager _needsManager;
        private NavMeshAgent _agent;
        private GameObject _chatPartner;

        public override void Contextualize(GameObject gm, Brain brainContext)
        {
            base.Contextualize(gm, brainContext);
            _needsManager = gm.GetComponent<NeedsManager>();
            _agent = gm.GetComponent<NavMeshAgent>();
            _spriteRenderer = gm.GetComponentInChildren<SpriteRenderer>();
        }

        public override void OnStart()
        {
            _chatPartner = blackboard.Exists("ChatOffer") ? blackboard.Get<GameObject>("ChatOffer") : null;

            if (_needsManager != null) 
                _needsManager.pauseSocialDecay = true;

            if (_spriteRenderer != null) 
                _spriteRenderer.color = chattingColor;

            blackboard.Put("PartnerAcceptedChat", true);

            if (_chatPartner != null && _agent != null && _agent.isOnNavMesh)
            {
                _agent.isStopped = false;
                _agent.SetDestination(_chatPartner.transform.position);
            }
        }

        public override void Execute()
        {
            if (_chatPartner == null || _needsManager == null || _agent == null) 
                return;

            if (!_agent.isOnNavMesh) 
                return;

            float distance = Vector2.Distance(_agent.transform.position, _chatPartner.transform.position);

            if (distance > talkingDistance)
            {
                if (_agent.isStopped ||
                    Vector2.Distance(_agent.destination, _chatPartner.transform.position) > 0.1f)
                {
                    _agent.isStopped = false;
                    _agent.SetDestination(_chatPartner.transform.position);
                }
                return;
            }

            if (!_agent.isStopped)
            {
                _agent.isStopped = true;
                _agent.ResetPath();
            }

            _needsManager.Socialize(socialRecoveryRate * Time.deltaTime);
        }

        public override void OnStop()
        {
            if (_needsManager != null) _needsManager.pauseSocialDecay = false;

            blackboard.Put("PartnerAcceptedChat", false);
            blackboard.Put("ChatOffer", (GameObject)null);
            _chatPartner = null;

            if (_spriteRenderer != null && blackboard.Exists("OriginalColor"))
                _spriteRenderer.color = blackboard.Get<Color>("OriginalColor");

            if (_agent != null && _agent.isOnNavMesh)
            {
                _agent.isStopped = true;
                _agent.ResetPath();
            }
        }
    }
}