#if UNITY_EDITOR && UnityExtensions
using UnityExtensions.UI;
using Nobi.UiRoundedCorners;
using UnityEditor;

[CustomEditor(typeof(InputElement), true)]
[CanEditMultipleObjects]
public class InputElementEditor : GraphicComponentEditor
{
    public override void DrawComponents()
    {
        AddComponent<ImageWithRoundedCorners>();
        AddComponent<ImageWithIndependentRoundedCorners>();
    }

    public override void DrawProperties()
    {
        CreateLayout("Label", "Input Label");
        CreateLayout("Type", "InputField Type");
        base.DrawProperties();
    }
}
#endif
