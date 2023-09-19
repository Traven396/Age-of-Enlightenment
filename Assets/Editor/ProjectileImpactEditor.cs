using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ProjectileImpact))]
public class ProjectileImpactEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        ProjectileImpact myBehaviour = target as ProjectileImpact;
        EditorGUILayout.PropertyField(serializedObject.FindProperty("TypeOfImpact"), new GUIContent("Impact Type: "));

        if (myBehaviour.TypeOfImpact == ProjectileImpact.ImpactType.Tween)
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
