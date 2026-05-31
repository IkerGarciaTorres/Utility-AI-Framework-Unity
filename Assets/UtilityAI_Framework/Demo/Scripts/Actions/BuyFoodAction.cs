using UnityEngine;
using UnityEngine.AI;
using UtilityAI.Environment;
using UtilityAI.Sensors;

namespace UtilityAI.Actions
{
    [CreateAssetMenu(fileName = "BuyFoodAction", menuName = "UtilityAI/Actions/Buy Food")]
    public class BuyFoodAction : Action
    {
        [Header("Shopping Settings")]
        public float stoppingDistance = 0.5f;
        public int foodPrice = 2;
        public int maxFoodToBuy = 5;

        private NeedsManager _needsManager;
        private NavMeshAgent _agent;
        private ResourceStation _targetShop;
        private bool _hasPurchased;

        public override void Contextualize(GameObject gm, Brain brainContext)
        {
            base.Contextualize(gm, brainContext);
            _needsManager = gm.GetComponent<NeedsManager>();
            _agent = gm.GetComponent<NavMeshAgent>();
        }

        public override void OnStart()
        {
            _targetShop = blackboard.Get<ResourceStation>("ClosestAvailable_FoodShop");
            _hasPurchased = false;

            if (_targetShop != null && _agent != null)
            {
                if (_targetShop.isOccupied)
                {
                    _targetShop = null;
                    blackboard.Put("ClosestAvailable_FoodShop", (ResourceStation)null);
                    return;
                }

                blackboard.Put("TargetStation_FoodShop", _targetShop);
                GlobalBlackboard.Instance.ClaimStation(_targetShop);
                _agent.isStopped = false;
                _agent.SetDestination(_targetShop.interactionPoint.position);
            }
        }

        public override void Execute()
        {
            if (_needsManager == null || _agent == null)
                return;

            if (_targetShop == null)
            {
                ResourceStation newTarget = blackboard.Get<ResourceStation>("ClosestAvailable_FoodShop");
                if (newTarget != null && !newTarget.isOccupied)
                {
                    _targetShop = newTarget;
                    blackboard.Put("TargetStation_FoodShop", _targetShop);
                    GlobalBlackboard.Instance.ClaimStation(_targetShop);

                    _agent.isStopped = false;
                    _agent.SetDestination(_targetShop.interactionPoint.position);
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

            if (!_hasPurchased)
            {
                ExecutePurchase();
                _hasPurchased = true;
            }
        }

        private void ExecutePurchase()
        {
            int currentPrice = _targetShop.pricePerUnit;

            float spendingHabit = 1f;
            if (blackboard.Exists("SpendingHabit"))
            {
                spendingHabit = blackboard.Get<float>("SpendingHabit");
            }

            int desiredAmount = Mathf.CeilToInt(spendingHabit * maxFoodToBuy);
            desiredAmount = Mathf.Max(1, desiredAmount);

            int affordableAmount = _needsManager.money / currentPrice;
            int actualAmountToBuy = Mathf.Min(desiredAmount, affordableAmount);

            if (actualAmountToBuy > 0)
            {
                _needsManager.SpendMoney(actualAmountToBuy * currentPrice);
                _needsManager.AddFoodRations(actualAmountToBuy);

                blackboard.Put("Money", _needsManager.money);
                blackboard.Put("FoodRations", _needsManager.foodRations);
            }
        }

        public override void OnStop()
        {
            if (_targetShop != null)
            {
                GlobalBlackboard.Instance.ReleaseStation(_targetShop);
                _targetShop = null;
                blackboard.Put("TargetStation_FoodShop", null);
            }

            if (_agent != null && _agent.isOnNavMesh)
            {
                _agent.isStopped = true;
                _agent.ResetPath();
            }
        }
    }
}