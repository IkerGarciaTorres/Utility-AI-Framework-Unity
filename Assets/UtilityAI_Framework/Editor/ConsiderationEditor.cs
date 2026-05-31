using UnityEngine;
using UnityEditor;
using UtilityAI;
using UtilityAI.Math;

namespace UtilityAI.EditorTools
{
    [CustomEditor(typeof(Consideration), true)]
    public class ConsiderationEditor : Editor
    {
        private static readonly string[] excludedProperties = new string[]
        {
            "m_Script", "considerationName", "traitID", "curveType",
            "m", "b", "k", "midpoint", "steepness", "mean", "stdDev", "responseCurve"
        };

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            Consideration cons = (Consideration)target;

            EditorGUILayout.PropertyField(serializedObject.FindProperty("considerationName"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("traitID"));

            EditorGUILayout.Space();

            DrawPropertiesExcluding(serializedObject, excludedProperties);

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Curve Generator", EditorStyles.boldLabel);
            SerializedProperty curveTypeProp = serializedObject.FindProperty("curveType");
            EditorGUILayout.PropertyField(curveTypeProp);

            MathCurveType type = (MathCurveType)curveTypeProp.enumValueIndex;

            switch (type)
            {
                case MathCurveType.Linear:
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("m"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("b"));
                    break;
                case MathCurveType.Polynomial:
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("k"));
                    break;
                case MathCurveType.Logistic:
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("midpoint"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("steepness"));
                    break;
                case MathCurveType.Gaussian:
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("mean"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("stdDev"));
                    break;
            }

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("responseCurve"));

            if (type != MathCurveType.Manual)
            {
                EditorGUILayout.Space();
                GUI.backgroundColor = Color.green;
                if (GUILayout.Button("Bake Mathematical Curve", GUILayout.Height(30)))
                {
                    BakeCurve(cons);
                }
                GUI.backgroundColor = Color.white;
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void BakeCurve(Consideration cons)
        {
            int resolution = 25;
            Keyframe[] keys = new Keyframe[resolution];
            for (int i = 0; i < resolution; i++)
            {
                float x = i / (float)(resolution - 1);
                float y = 0f;
                switch (cons.curveType)
                {
                    case MathCurveType.Linear: 
                        y = UtilityCalculator.EvaluateLinear(x, cons.m, cons.b);
                        break;
                    case MathCurveType.Polynomial: 
                        y = UtilityCalculator.EvaluatePolynomial(x, cons.k);
                        break;
                    case MathCurveType.Logistic:
                        y = UtilityCalculator.EvaluateLogistic(x, cons.midpoint, cons.steepness); 
                        break;
                    case MathCurveType.Gaussian:
                        y = UtilityCalculator.EvaluateGaussian(x, cons.mean, cons.stdDev); 
                        break;
                }
                keys[i] = new Keyframe(x, Mathf.Clamp01(y));
            }
            cons.responseCurve = new AnimationCurve(keys);
            for (int i = 0; i < resolution; i++)
            {
                AnimationUtility.SetKeyLeftTangentMode(cons.responseCurve, i, AnimationUtility.TangentMode.Auto);
                AnimationUtility.SetKeyRightTangentMode(cons.responseCurve, i, AnimationUtility.TangentMode.Auto);
            }
            EditorUtility.SetDirty(cons);
            AssetDatabase.SaveAssets();
        }
    }
}