#if UNITY_EDITOR && UnityExtensions
using UnityExtensions.UI;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LayoutComponent), true)]
[CanEditMultipleObjects]
public class LayoutEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawProperties();
        serializedObject.ApplyModifiedProperties();
    }

    protected virtual void DrawProperties()
    {
        if (GUILayout.Button("Update", GUILayout.Width(75)))
        {
            _ = serializedObject.targetObject.GetComponent<LayoutComponent>().UpdateLayout();
        }

        EditorGUILayout.Space(2);

        EditorGUILayout.BeginHorizontal();

        CreateLayout("_isVertical", "Is Vertical", true);
        CreateLayout("_isHorizontal", "Is Horizontal", true);

        EditorGUILayout.EndHorizontal();

        CreateLayout("_offset", "Offset", true);
        CreateLayout("_padding", "Padding", true);
        CreateLayout("_minSize", "Min Size", true);
    }

    public void CreateLayout(string name, string label, bool editable = true)
    {
        SerializedProperty property = Property(name);

        if (property != null)
        {
            if (!editable)
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.PropertyField(property, new GUIContent(label));
                EditorGUI.EndDisabledGroup();
            }
            else
            {
                EditorGUILayout.PropertyField(property, new GUIContent(label));
            }
        }
        else
        {
            EditorGUILayout.HelpBox($"{name} property not found!", MessageType.Error);
        }
    }

    public SerializedProperty Property(string name)
    {
        return serializedObject.FindProperty(name);
    }
}
#endif
