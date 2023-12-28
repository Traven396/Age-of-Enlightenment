using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SpellImpact))]
public class SpellImpactEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        SpellImpact myBehaviour = target as SpellImpact;
        EditorGUILayout.PropertyField(serializedObject.FindProperty("TypeOfImpact"), new GUIContent("Impact Type: "));

        if (myBehaviour.TypeOfImpact == SpellImpact.ImpactType.Tween)
        {
            EditorGUILayout.Separator();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("StartTween"), new GUIContent("Starting Tween Type: "));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("StartTweenSpeed"), new GUIContent("Starting Tween Time: "));

            EditorGUILayout.Separator();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("EndTween"), new GUIContent("Ending Tween Type: "));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("EndTweenSpeed"), new GUIContent("Ending Tween Time: "));
        }

        EditorGUILayout.Separator();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("AutoCalculateLifetime"));

        if (!myBehaviour.AutoCalculateLifetime)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_maxLifetime"), new GUIContent("Lifetime: "));
        }

        serializedObject.ApplyModifiedProperties();
    }
}
