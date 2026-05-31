using UnityEngine;
using UnityEditor;
using UtilityAI;

namespace UtilityAI.EditorTools
{
    [CustomEditor(typeof(Brain))]
    public class BrainEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (!Application.isPlaying) return;

            Brain brain = (Brain)target;

            EditorGUILayout.Space(15);
            EditorGUILayout.LabelField("Utility Monitor", EditorStyles.boldLabel);

            if (brain.RuntimeActions == null || brain.RuntimeActions.Count == 0)
            {
                EditorGUILayout.HelpBox("Inactive brain or no actions", MessageType.Warning);
                return;
            }

            string currentActionName = brain.CurrentAction != null ? brain.CurrentAction.Name : "Evaluating...";
            EditorGUILayout.HelpBox($"Current Action: {currentActionName}", MessageType.Info);

            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("Continuous Utility (Geometric Mean):", EditorStyles.miniBoldLabel);

            foreach (Action action in brain.RuntimeActions)
            {
                float utility = action.GetActionUtility();

                Rect rect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight);

                EditorGUI.ProgressBar(rect, utility, $"{action.Name} ({utility:F2})");
            }

            Repaint();
        }
    }
}