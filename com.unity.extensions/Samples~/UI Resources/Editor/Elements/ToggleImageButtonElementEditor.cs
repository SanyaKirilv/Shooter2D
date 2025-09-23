#if UNITY_EDITOR && UnityExtensions
using UnityExtensions.UI;
using Nobi.UiRoundedCorners;
using UnityEditor;
using UnityEngine.UI;

[CustomEditor(typeof(ToggleImageButtonElement), true)]
[CanEditMultipleObjects]
public class ToggleImageButtonElementEditor : ButtonElementEditor
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
        CreateLayout("Child", "Image Element");
        CreateLayout("OnSprite", "On Sprite");
        CreateLayout("OffSprite", "Off Sprite");
        base.DrawProperties();
    }
}
#endif
