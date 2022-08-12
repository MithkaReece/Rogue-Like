using UnityEngine;
using UnityEditor;

//[CustomEditor(typeof(ItemScriptableObject))]
public class ModifierGeneratorEditor : Editor
{
    int index = 0;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();
        GUILayout.Label("Modifiers");

        ItemScriptableObject item = (ItemScriptableObject)target;

        for (int i = 0; i < item.modifiers.Count; i++)
        {
            GUILayout.BeginHorizontal("BOX");
            GUILayout.BeginVertical("BOX");
            EditorGUILayout.PropertyField(serializedObject.FindProperty($"modifiers.Array.data[{i}].statType"));
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty($"modifiers.Array.data[{i}].modifierType"));
            //EditorGUILayout.PropertyField(modifier.FindProperty("statType"));
            index = EditorGUILayout.Popup(index, new string[] { "Range", "Preset" });
            EditorGUILayout.Space(); EditorGUILayout.Space(); EditorGUILayout.Space();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
        }
    }
}