using UnityEngine;
using UtilityAI.Sensors;

namespace UtilityAI.Actions
{
    [CreateAssetMenu(fileName = "EatAction", menuName = "UtilityAI/Actions/Eat")]
    public class EatAction : Action
    {
        [Header("Eating Settings")]
        [Tooltip("How much hunger is restored per food ration consumed")]
        public float nutritionValue = 30f;

        [Tooltip("Time in seconds it takes to eat one ration")]
        public float eatDuration = 2f;

        [Header("Visual Setting")]
        public Color actionColor = Color.green;

        private SpriteRenderer _spriteRenderer;
        private NeedsManager _needsManager;
        private float _timer;

        public override void Contextualize(GameObject gm, Brain brainContext)
        {
            base.Contextualize(gm, brainContext);
            _needsManager = gm.GetComponent<NeedsManager>();
            _spriteRenderer = gm.GetComponent<SpriteRenderer>();
        }

        public override void OnStart()
        {
            _timer = 0f;

            if (_spriteRenderer != null)
            {
                _spriteRenderer.color = actionColor;
            }

            if (_needsManager != null)
            {
                _needsManager.pauseHungerDecay = true;
            }
        }

        public override void Execute()
        {
            if (_needsManager == null || _needsManager.foodRations <= 0) 
                return;

            _timer += Time.deltaTime;

            if (_timer >= eatDuration)
            {
                _needsManager.ConsumeFoodRation();
                _needsManager.Eat(nutritionValue);

                blackboard.Put("FoodRations", _needsManager.foodRations);

                _timer = 0f;
            }
        }

        public override void OnStop()
        {
            if (_needsManager != null)
            {
                _needsManager.pauseHungerDecay = false;
            }

            if (_spriteRenderer != null && blackboard.Exists("OriginalColor"))
            {
                _spriteRenderer.color = blackboard.Get<Color>("OriginalColor");
            }
        }
    }
}