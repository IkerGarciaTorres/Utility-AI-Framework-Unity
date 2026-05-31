using UnityEngine;
using System.Collections.Generic;

namespace UtilityAI
{
    [RequireComponent(typeof(DynamicBlackboard))]
    public class Brain : MonoBehaviour, ITickableUtility
    {
        [Header("Available Behaviors")]
        [Tooltip("Drag and drop the Action ScriptableObjects")]
        public Action[] availableActions;

        [Header("Personality")]
        public Traits.PersonalityProfile personalityProfile;

        private List<Action> runtimeActions = new List<Action>();
        private Action currentAction;

        public Action CurrentAction => currentAction;
        public List<Action> RuntimeActions => runtimeActions;

        private void Start()
        {
            foreach (Action originalAction in availableActions)
            {
                if (originalAction == null) continue;

                Action clonedAction = Instantiate(originalAction);
                clonedAction.Contextualize(this.gameObject, this);
                runtimeActions.Add(clonedAction);
            }

            if (TickScheduler.Instance != null)
            {
                TickScheduler.Instance.Register(this);
            }
 
        }

        private void OnDestroy()
        {
            if (TickScheduler.Instance != null)
            {
                TickScheduler.Instance.Unregister(this);
            }
        }

        public void TickUtility()
        {
            if (runtimeActions.Count == 0) return;

            Action bestAction = null;
            float bestUtility = -1f;

            foreach (Action action in runtimeActions)
            {
                float utility = action.GetActionUtility();

                if (action == currentAction)
                {
                    utility = Math.UtilityCalculator.ApplyInertia(utility, action.inertiaBonus);
                }

                if (utility > bestUtility)
                {
                    bestUtility = utility;
                    bestAction = action;
                }
            }

            if (bestAction != currentAction && bestAction != null)
            {
                if (currentAction != null)
                {
                    currentAction.OnStop();
                }

                currentAction = bestAction;
                currentAction.OnStart();
            }
        }

        private void Update()
        {
            if (currentAction != null)
            {
                currentAction.Execute();
            }
        }

        public bool HasPersonality()
        {
            return personalityProfile != null;
        }

        public void GetTraitModifiers(string traitID, out float offset, out float multiplier)
        {
            if (HasPersonality())
            {
                personalityProfile.TryGetModifiers(traitID, out offset, out multiplier);
            }
            else
            {
                offset = 0f;
                multiplier = 1f;
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (Application.isPlaying && currentAction != null)
            {
                Vector3 textPosition = transform.position + Vector3.up * 2f;
                UnityEditor.Handles.color = Color.white;
                UnityEditor.Handles.Label(textPosition, $"Current Action:\n{currentAction.Name}");
            }
        }
#endif
    }
}