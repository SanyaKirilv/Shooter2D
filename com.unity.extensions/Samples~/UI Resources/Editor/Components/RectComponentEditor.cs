#if UNITY_EDITOR && UnityExtensions
using UnityExtensions.UI;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RectComponent), false)]
[CanEditMultipleObjects]
public class RectComponentEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.BeginHorizontal();
        DrawComponents();
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space(6);

        DrawProperties();

        serializedObject.ApplyModifiedProperties();
    }

    public virtual void DrawComponents() { }

    public virtual void DrawProperties() { }

    public void CreateLayout(string name, string label, bool editable = true)
    {
        SerializedProperty property = Property(name);

        if (property != null)
        {
            EditorGUI.BeginDisabledGroup(!editable);
            EditorGUILayout.PropertyField(property, new GUIContent(label));
            EditorGUI.EndDisabledGroup();
        }
        else
        {
            EditorGUILayout.HelpBox($"{name} property not found!", MessageType.Error);
        }
    }

    public void NextLine()
    {
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
    }

    public SerializedProperty Property(string name)
    {
        return serializedObject.FindProperty(name);
    }

    public void AddComponent<T>()
        where T : MonoBehaviour
    {
        T component = serializedObject.targetObject.GetComponent<T>();

        if (component == null)
        {
            if (GUILayout.Button($"+ {typeof(T).Name}"))
            {
                serializedObject.targetObject.AddComponent<T>();
            }
        }
        else if (serializedObject.targetObject.GetComponent<T>() != null)
        {
            if (GUILayout.Button($"- {typeof(T).Name}"))
            {
                DestroyImmediate(component);
            }
        }
    }
}
#endif
