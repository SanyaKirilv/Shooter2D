#if UNITY_EDITOR && UnityExtensions
using UnityExtensions.UI;
using UnityEditor;

[CustomEditor(typeof(GraphicComponent), true)]
[CanEditMultipleObjects]
public class GraphicComponentEditor : RectComponentEditor
{
    public override void DrawProperties()
    {
        EditorGUILayout.Space(4);
        CreateLayout("_graphic", "Target Graphics", false);
        CreateLayout("_color", "Default Color");
    }
}
#endif
