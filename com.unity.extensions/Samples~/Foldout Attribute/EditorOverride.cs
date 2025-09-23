using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

[CustomEditor(typeof(System.Object), true, isFallback = true)]
[CanEditMultipleObjects]
public class EditorOverride : Editor
{
    private Dictionary<string, CacheFoldProp> cacheFolds = new Dictionary<string, CacheFoldProp>();
    private List<SerializedProperty> props = new List<SerializedProperty>();
    private bool initialized;
    private CacheFoldProp _currentFoldEverything;

    private void OnEnable()
    {
        initialized = false;
    }

    private void OnDisable()
    {
        if (target != null)
        {
            foreach (var c in cacheFolds)
            {
                EditorPrefs.SetBool($"{c.Value.Name}{target.name}", c.Value.Expanded);
                c.Value.Dispose();
            }
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        Setup();

        if (props.Count == 0 && cacheFolds.Count == 0)
        {
            DrawDefaultInspector();
            return;
        }

        foreach (var foldout in cacheFolds.Values)
        {
            DrawFoldout(foldout);
        }

        for (var i = 0; i < props.Count; i++)
        {
            EditorGUILayout.PropertyField(props[i], true);
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawFoldout(CacheFoldProp foldout)
    {
        foldout.Expanded = EditorGUILayout.Foldout(
            foldout.Expanded,
            foldout.Name,
            true,
            EditorStyles.foldout
        );

        if (foldout.Expanded)
        {
            EditorGUI.indentLevel++;

            foreach (var prop in foldout.Props)
            {
                EditorGUILayout.PropertyField(prop, true);
            }

            foreach (var child in foldout.Children)
            {
                DrawFoldout(child);
            }

            EditorGUI.indentLevel--;
        }
    }

    private void Setup()
    {
        if (!initialized)
        {
            cacheFolds.Clear();
            props.Clear();
            _currentFoldEverything = null;

            var property = serializedObject.GetIterator();
            if (property.NextVisible(true))
            {
                do
                {
                    HandleFoldProp(property);
                } while (property.NextVisible(false));
            }

            initialized = true;
        }
    }

    private void HandleFoldProp(SerializedProperty prop)
    {
        var fieldInfo = EditorTypes.GetFieldInfo(target.GetType(), prop.name);
        var foldoutAttribute = fieldInfo?.GetCustomAttribute<FoldoutAttribute>();

        if (prop.name == "m_Script")
        {
            return;
        }

        if (foldoutAttribute != null)
        {
            var foldoutPath = foldoutAttribute.Name.Split('/');
            CacheFoldProp currentFoldout = null;

            foreach (var foldoutName in foldoutPath)
            {
                var foldout = FindOrCreateFoldout(foldoutName, currentFoldout);
                currentFoldout = foldout;
            }

            if (currentFoldout != null)
            {
                currentFoldout.Props.Add(prop.Copy());
            }

            if (foldoutAttribute.FoldEverything)
            {
                _currentFoldEverything = currentFoldout;
            }
            else
            {
                _currentFoldEverything = null;
            }
        }
        else if (_currentFoldEverything != null)
        {
            _currentFoldEverything.Props.Add(prop.Copy());
        }
        else
        {
            props.Add(prop.Copy());
        }
    }

    private CacheFoldProp FindOrCreateFoldout(string name, CacheFoldProp parent)
    {
        var foldoutList = parent == null ? cacheFolds.Values.ToList() : parent.Children;
        var foldout = foldoutList.FirstOrDefault(f => f.Name == name);

        if (foldout == null)
        {
            foldout = new CacheFoldProp
            {
                Name = name,
                Expanded = EditorPrefs.GetBool($"{name}{target.name}", false),
                Parent = parent,
            };

            if (parent == null)
            {
                cacheFolds.Add(name, foldout);
            }
            else
            {
                parent.Children.Add(foldout);
            }
        }

        return foldout;
    }
}
#endif
