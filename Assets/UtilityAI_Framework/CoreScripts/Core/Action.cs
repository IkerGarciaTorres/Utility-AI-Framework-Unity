using UnityEngine;
using System.Collections.Generic;
using UtilityAI.Math;

namespace UtilityAI
{
    public abstract class Action : ScriptableObject
    {
        [Header("Action Configuration")]
        public string actionName;

        [Tooltip("Bonus added if this action is already executing to prevent flickering")]
        public float inertiaBonus = 0.05f;

        [Header("Considerations")]
        public List<Consideration> considerations = new List<Consideration>();

        protected GameObject gameObject;
        protected Brain brain;
        protected DynamicBlackboard blackboard;

        public string Name => string.IsNullOrEmpty(actionName) ? this.name : actionName;

        public virtual void Contextualize(GameObject gm, Brain brainContext)
        {
            this.gameObject = gm;
            this.brain = brainContext;
            this.blackboard = gm.GetComponent<DynamicBlackboard>();

            List<Consideration> clonedConsiderations = new List<Consideration>();

            foreach (var cons in considerations)
            {
                if (cons == null) continue;

                Consideration clonedCons = Instantiate(cons);

                clonedCons.Contextualize(gm);

                clonedConsiderations.Add(clonedCons);
            }

            this.considerations = clonedConsiderations;
        }

        public float GetActionUtility()
        {
            if (considerations.Count == 0) return 0f;

            List<float> scores = new List<float>();

            foreach (var cons in considerations)
            {
                float s = cons.Evaluate();

                if (s <= 0f)
                    return 0f;

                scores.Add(s);
            }

            return UtilityCalculator.CalculateActionUtility(scores);
        }

        public abstract void Execute();
        public virtual void OnStart() { }
        public virtual void OnStop() { }
    }
}