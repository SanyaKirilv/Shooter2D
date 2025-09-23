#if UNITY_EDITOR && UnityExtensions
using UnityExtensions.UI;
using UnityEditor;

[CustomEditor(typeof(ScrollElement), true)]
[CanEditMultipleObjects]
public class ScrollElementEditor : RectComponentEditor { }
#endif
