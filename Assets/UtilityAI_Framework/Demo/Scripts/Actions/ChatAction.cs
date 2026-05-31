using UnityEngine;
using UnityEngine.AI;
using UtilityAI.Sensors;

namespace UtilityAI.Actions
{
    [CreateAssetMenu(fileName = "ChatAction", menuName = "UtilityAI/Actions/Chat (Initiate)")]
    public class ChatAction : Action
    {
        public float patienceDuration = 4f;
        public float socialRecoveryRate = 15f;
        public float talkingDistance = 1.5f;
        public Color requestColor = Color.cyan;
        public Color chattingColor = Color.magenta;

        private float _waitTimer;
        private GameObject _chatPartner;
        private NavMeshAgent _agent;
        private SpriteRenderer _spriteRenderer;
        private NeedsManager _needsManager;

        public override void Contextualize(GameObject gm, Brain brainContext)
        {
            base.Contextualize(gm, brainContext);
            _agent = gm.GetComponent<NavMeshAgent>();
            _needsManager = gm.GetComponent<NeedsManager>();
            _spriteRenderer = gm.GetComponentInChildren<SpriteRenderer>();
        }

        public override void OnStart()
        {
            _waitTimer = 0f;

            _chatPartner = blackboard.Exists("ClosestFreeVillager")
                ? blackboard.Get<GameObject>("ClosestFreeVillager")
                : null;

            if (_chatPartner == null) 
                return;

            DynamicBlackboard partnerBoard = _chatPartner.GetComponent<DynamicBlackboard>();
            if (partnerBoard == null)
            {
                _chatPartner = null;
                return;
            }

            bool hasOffer   = partnerBoard.Exists("ChatOffer") && partnerBoard.Get<GameObject>("ChatOffer") != null;
            bool isChatting = partnerBoard.Exists("PartnerAcceptedChat") && partnerBoard.Get<bool>("PartnerAcceptedChat");

            if (hasOffer || isChatting)
            {
                _chatPartner = null;
                blackboard.Put("ClosestFreeVillager", (GameObject)null);
                return;
            }

            partnerBoard.Put("ChatOffer", this.gameObject);

            if (_needsManager != null) _needsManager.pauseSocialDecay = true;
            if (_spriteRenderer != null) _spriteRenderer.color = requestColor;

            if (_agent != null && _agent.isOnNavMesh)
            {
                _agent.isStopped = false;
                _agent.SetDestination(_chatPartner.transform.position);
            }
        }

        public override void Execute()
        {
            if (_agent == null || !_agent.isOnNavMesh) 
                return;

            if(_chatPartner == null)
            {
                GameObject newPartner = blackboard.Get<GameObject>("ClosestFreeVillager");
                if (newPartner != null)
                {
                    DynamicBlackboard pb = newPartner.GetComponent<DynamicBlackboard>();
                    bool hasOffer = pb.Exists("ChatOffer") && pb.Get<GameObject>("ChatOffer") != null;
                    bool isChatting = pb.Exists("PartnerAcceptedChat") && pb.Get<bool>("PartnerAcceptedChat");

                    if (!hasOffer && !isChatting)
                    {
                        _chatPartner = newPartner;
                        pb.Put("ChatOffer", this.gameObject);

                        _agent.isStopped = false;
                        _agent.SetDestination(_chatPartner.transform.position);
                    }
                }
                return;
            }

            float distance = Vector2.Distance(_agent.transform.position, _chatPartner.transform.position);

            if (distance > talkingDistance)
            {
                _agent.isStopped = false;
                _agent.SetDestination(_chatPartner.transform.position);
                return;
            }

            if (!_agent.isStopped)
            {
                _agent.isStopped = true;
                _agent.ResetPath();
            }

            DynamicBlackboard partnerBoard = _chatPartner.GetComponent<DynamicBlackboard>();
            if (partnerBoard == null)
                return;

            bool hasAccepted = partnerBoard.Exists("PartnerAcceptedChat") && partnerBoard.Get<bool>("PartnerAcceptedChat");

            if (hasAccepted)
            {
                if (_spriteRenderer != null && _spriteRenderer.color != chattingColor)
                    _spriteRenderer.color = chattingColor;

                _needsManager?.Socialize(socialRecoveryRate * Time.deltaTime);
            }
            else
            {
                _waitTimer += Time.deltaTime;
                if (_waitTimer >= patienceDuration)
                {
                    partnerBoard.Put("ChatOffer", (GameObject)null);
                    _chatPartner = null;
                }
            }
        }

        public override void OnStop()
        {
            if (_needsManager != null) _needsManager.pauseSocialDecay = false;

            if (_chatPartner != null)
            {
                DynamicBlackboard partnerBoard = _chatPartner.GetComponent<DynamicBlackboard>();
                if (partnerBoard != null &&
                    partnerBoard.Exists("ChatOffer") &&
                    partnerBoard.Get<GameObject>("ChatOffer") == gameObject)
                {
                    partnerBoard.Put("ChatOffer", (GameObject)null);
                }
            }

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