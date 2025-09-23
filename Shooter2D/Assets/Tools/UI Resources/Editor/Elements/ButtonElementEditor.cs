#if UNITY_EDITOR && UnityExtensions
using UnityExtensions.UI;
using Nobi.UiRoundedCorners;
using UnityEditor;
using UnityEngine.UI;

[CustomEditor(typeof(ButtonElement), true)]
[CanEditMultipleObjects]
public class ButtonElementEditor : GraphicComponentEditor
{
    public override void DrawComponents()
    {
        AddComponent<ImageWithRoundedCorners>();
        AddComponent<ImageWithIndependentRoundedCorners>();
        NextLine();
        AddComponent<LayoutComponent>();
        AddComponent<LayoutElement>();
    }

    public override void DrawProperties()
    {
        CreateLayout("_state", "Button State");
        CreateLayout("_hasVisualFeedback", "Transition");
        base.DrawProperties();
    }
}
#endif
