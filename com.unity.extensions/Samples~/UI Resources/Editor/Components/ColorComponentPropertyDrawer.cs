#if UNITY_EDITOR && UnityExtensions
using UnityExtensions.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ColorComponent))]
[CanEditMultipleObjects]
public class ColorComponentPropertyDrawer : PropertyDrawer
{
    private SerializedProperty _property;
    private bool _isCustom;
    private float _horizontalOffset = 2f;

    public override void OnGUI(Rect pos, SerializedProperty property, GUIContent label)
    {
        _property = property;

        EditorGUI.LabelField(pos, label);

        float labelWidth = EditorGUIUtility.labelWidth;
        float fieldWidth = pos.width - labelWidth;

        Rect colorRect = new Rect(
            pos.x + labelWidth,
            pos.y,
            (fieldWidth * 2f / 5f) - _horizontalOffset,
            pos.height
        );

        Rect popupRect = new Rect(
            colorRect.x + colorRect.width + _horizontalOffset,
            pos.y,
            (fieldWidth * 2f / 5f) - _horizontalOffset,
            pos.height
        );

        Rect alphaRect = new Rect(
            popupRect.x + popupRect.width + _horizontalOffset,
            pos.y,
            fieldWidth * 1f / 5f,
            pos.height
        );

        BeginPopupProperty(popupRect);
        BeginAlphaProperty(alphaRect);
        BeginColorProperty(colorRect);
    }

    private SerializedProperty Property(string name)
    {
        return _property.FindPropertyRelative(name);
    }

    private void BeginColorProperty(Rect rect)
    {
        SerializedProperty colorProp = Property("_color");
        SerializedProperty nameProp = Property("_name");
        SerializedProperty alphaProp = Property("_alpha");

        if (_isCustom)
        {
            colorProp.colorValue = EditorGUI.ColorField(rect, colorProp.colorValue);
            alphaProp.floatValue = colorProp.colorValue.a;
        }
        else
        {
            colorProp.colorValue = ColorComponent.GetColor(
                nameProp.stringValue,
                alphaProp.floatValue
            );

            EditorGUI.BeginDisabledGroup(true);
            EditorGUI.ColorField(rect, colorProp.colorValue);
            EditorGUI.EndDisabledGroup();
        }
    }

    private void BeginPopupProperty(Rect rect)
    {
        SerializedProperty property = Property("_name");
        List<string> keys = Colors.ColorMap.Keys.ToList();
        int selectedIndex = keys.IndexOf(property.stringValue);

        if (selectedIndex < 0)
        {
            selectedIndex = 0;
        }

        int newIndex = EditorGUI.Popup(rect, selectedIndex, keys.ToArray());
        if (newIndex != selectedIndex)
        {
            property.stringValue = keys[newIndex];
        }

        _isCustom = property.stringValue == "Custom";
    }

    private void BeginAlphaProperty(Rect rect)
    {
        Rect labelRect = new Rect(rect.x + rect.width + 2, rect.y, 12, rect.height);
        SerializedProperty alphaProp = Property("_alpha");

        if (_isCustom)
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUI.FloatField(rect, Mathf.RoundToInt(alphaProp.floatValue * 100));
            EditorGUI.EndDisabledGroup();
        }
        else
        {
            float alphaRaw = EditorGUI.FloatField(
                rect,
                Mathf.RoundToInt(alphaProp.floatValue * 100)
            );
            alphaProp.floatValue = alphaRaw / 100;
        }

        EditorGUI.LabelField(labelRect, "%");
    }
}
#endif
