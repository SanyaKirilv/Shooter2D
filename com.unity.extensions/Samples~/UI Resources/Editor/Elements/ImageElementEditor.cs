#if UNITY_EDITOR && UnityExtensions
using UnityExtensions.UI;
using Nobi.UiRoundedCorners;
using UnityEditor;
using UnityEngine.UI;

[CustomEditor(typeof(ImageElement), true)]
[CanEditMultipleObjects]
public class ImageElementEditor : GraphicComponentEditor
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
        CreateLayout("Stretch", "Is stretch Image");
        base.DrawProperties();
    }
}
#endif
