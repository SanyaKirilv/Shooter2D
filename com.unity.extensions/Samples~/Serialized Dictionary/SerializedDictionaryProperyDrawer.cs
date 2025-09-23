using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

[CustomPropertyDrawer(typeof(SerializedDictionary<,>))]
public class SerializedDictionaryProperyDrawer : PropertyDrawer
{
    private const float LineHeight = 20f;
    private const float Spacing = 5f;
    private const int ItemsPerPage = 10;

    private bool _isFoldoutOpen = false;
    private int _currentPage = 0;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        Rect _foldoutRect = new Rect(position.x, position.y, position.width, LineHeight);
        _isFoldoutOpen = EditorGUI.Foldout(_foldoutRect, _isFoldoutOpen, label);

        if (_isFoldoutOpen)
        {
            SerializedProperty _keys = property.FindPropertyRelative("keys");
            SerializedProperty _values = property.FindPropertyRelative("values");

            Rect _contentRect = new Rect(
                position.x,
                position.y + LineHeight + Spacing,
                position.width,
                LineHeight
            );

            int startIndex = _currentPage * ItemsPerPage;
            int endIndex = Mathf.Min(startIndex + ItemsPerPage, _keys.arraySize);

            if (_keys.arraySize > ItemsPerPage)
            {
                Rect _rect = new Rect(
                    _foldoutRect.x + _foldoutRect.width - (LineHeight * 4) - Spacing,
                    _foldoutRect.y,
                    LineHeight * 2,
                    LineHeight
                );
                EditorGUI.BeginDisabledGroup(_currentPage == 0);
                if (GUI.Button(new Rect(_rect.x, _rect.y, _rect.width, LineHeight), "<<"))
                {
                    _currentPage--;
                }
                EditorGUI.EndDisabledGroup();

                EditorGUI.BeginDisabledGroup(endIndex >= _keys.arraySize);
                if (
                    GUI.Button(
                        new Rect(_rect.x + _rect.width + Spacing, _rect.y, _rect.width, LineHeight),
                        ">>"
                    )
                )
                {
                    _currentPage++;
                }
                EditorGUI.EndDisabledGroup();
            }

            for (int i = startIndex; i < endIndex; i++)
            {
                Rect key = new Rect(
                    _contentRect.x,
                    _contentRect.y,
                    (_contentRect.width / 2) - Spacing,
                    _contentRect.height
                );
                DrawProperty(key, _keys.GetArrayElementAtIndex(i), $"index {i} Key", 112);

                Rect value = new Rect(
                    _contentRect.x + key.width + Spacing,
                    _contentRect.y,
                    (_contentRect.width / 2) - Spacing,
                    _contentRect.height
                );
                DrawProperty(value, _values.GetArrayElementAtIndex(i), "Value", 64);

                _contentRect.y += LineHeight + Spacing;
            }
        }

        EditorGUI.EndProperty();
    }

    private void DrawProperty(Rect rect, SerializedProperty prop, string name, float labelWidth)
    {
        Rect _labelRect = new Rect(rect.x, rect.y, labelWidth, rect.height);
        EditorGUI.LabelField(_labelRect, name);

        Rect _propRect = new Rect(
            rect.x + labelWidth,
            rect.y,
            rect.width - _labelRect.width,
            rect.height
        );

        if (prop.isArray && prop.propertyType == SerializedPropertyType.Generic)
        {
            prop.isExpanded = EditorGUI.Foldout(_propRect, prop.isExpanded, GUIContent.none);
            if (prop.isExpanded)
            {
                EditorGUI.indentLevel++;
                for (int i = 0; i < prop.arraySize; i++)
                {
                    Rect elementRect = new Rect(
                        _propRect.x,
                        _propRect.y + (i + 1) * (LineHeight + Spacing),
                        _propRect.width,
                        LineHeight
                    );
                    DrawProperty(
                        elementRect,
                        prop.GetArrayElementAtIndex(i),
                        $"Element {i}",
                        labelWidth
                    );
                }
                EditorGUI.indentLevel--;
            }
        }
        else if (prop.propertyType == SerializedPropertyType.ObjectReference)
        {
            EditorGUI.ObjectField(_propRect, prop, GUIContent.none);
        }
        else
        {
            EditorGUI.PropertyField(_propRect, prop, GUIContent.none);
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (!_isFoldoutOpen)
        {
            return LineHeight;
        }

        SerializedProperty keys = property.FindPropertyRelative("keys");
        SerializedProperty values = property.FindPropertyRelative("values");

        int visiblePairs = Mathf.Min(ItemsPerPage, keys.arraySize - _currentPage * ItemsPerPage);
        float height = LineHeight + (visiblePairs * (LineHeight + Spacing));

        for (int i = 0; i < visiblePairs; i++)
        {
            SerializedProperty valueProp = values.GetArrayElementAtIndex(
                i + _currentPage * ItemsPerPage
            );
            if (
                valueProp.isArray
                && valueProp.propertyType == SerializedPropertyType.Generic
                && valueProp.isExpanded
            )
            {
                height += (valueProp.arraySize + 1) * (LineHeight + Spacing);
            }
        }

        return height;
    }
}

#endif
