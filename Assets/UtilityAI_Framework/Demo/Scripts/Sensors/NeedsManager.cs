using System.Collections;
using UnityEngine;

namespace UtilityAI.Sensors
{
    public class NeedsManager : MonoBehaviour
    {
        private DynamicBlackboard _blackboard;

        [Header("Basic Needs")]
        [Range(0f, 100f)] public float hunger = 100f;
        [Range(0f, 100f)] public float energy = 100f;
        [Range(0f, 100f)] public float social = 100f;

        [Header("Decay Rates")]
        public float hungerDecayRate = 2f;
        public float energyDecayRate = 1.5f;
        public float socialDecayRate = 1f;

        [Header("Decay Pauses")]
        public bool pauseHungerDecay = false;
        public bool pauseEnergyDecay = false;
        public bool pauseSocialDecay = false;

        [Header("Inventory & Economy")]
        public int money = 0;
        [Tooltip("The amount of money that this agent considers to be financial success.")]
        public int targetWealth = 50;
        [Range(0.1f, 1f)] public float spendingHabit = 1f;
        public int foodRations = 0;

        [Header("Perception")]
        [Tooltip("Time in seconds between each blackboard sync")]
        public float syncInterval = 0.2f;

        private void Start()
        {
            _blackboard = GetComponent<DynamicBlackboard>();

            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
                _blackboard.Put("OriginalColor", spriteRenderer.color);

            _blackboard.Put("TargetWealth", targetWealth);
            _blackboard.Put("SpendingHabit", spendingHabit);
            _blackboard.Put("FoodRations", foodRations);


            SyncBlackboard();
        }

        private void OnEnable() => StartCoroutine(SyncRoutine());
        private void OnDisable() => StopAllCoroutines();

        private void Update()
        {
            if (!pauseHungerDecay) hunger -= hungerDecayRate * Time.deltaTime;
            if (!pauseEnergyDecay) energy -= energyDecayRate * Time.deltaTime;
            if (!pauseSocialDecay) social -= socialDecayRate * Time.deltaTime;

            hunger = Mathf.Clamp(hunger, 0f, 100f);
            energy = Mathf.Clamp(energy, 0f, 100f);
            social = Mathf.Clamp(social, 0f, 100f);
        }

        private IEnumerator SyncRoutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(syncInterval);
                SyncBlackboard();
            }
        }

        public void Eat(float amount) { hunger = Mathf.Clamp(hunger + amount, 0f, 100f); }
        public void Rest(float amount) { energy = Mathf.Clamp(energy + amount, 0f, 100f); }
        public void Socialize(float amount) { social = Mathf.Clamp(social + amount, 0f, 100f); }

        public void EarnMoney(int amount) { money += amount; }
        public void SpendMoney(int amount) { money = Mathf.Max(0, money - amount); }
        public void AddFoodRations(int amount) { foodRations += amount; }
        public void ConsumeFoodRation() { foodRations = Mathf.Max(0, foodRations - 1); }

        private void SyncBlackboard()
        {
            _blackboard.Put("NormalizedHunger", hunger / 100f);
            _blackboard.Put("NormalizedEnergy", energy / 100f);
            _blackboard.Put("NormalizedSocial", social / 100f);

            _blackboard.Put("AgentPosition", (Vector2)transform.position);
            _blackboard.Put("Money", money);
            _blackboard.Put("FoodRations", foodRations);
        }
    }
}