#if UNITY_EDITOR && UnityExtensions
using UnityExtensions.UI;
using UnityEditor;
using UnityEngine.UI;

[CustomEditor(typeof(TextElement), true)]
[CanEditMultipleObjects]
public class TextElementEditor : GraphicComponentEditor
{
    public override void DrawComponents()
    {
        AddComponent<ContentSizeFitter>();
        AddComponent<LayoutElement>();
    }

    public override void DrawProperties()
    {
        CreateLayout("_defaultText", "Default Text");
        base.DrawProperties();
    }
}
#endif
