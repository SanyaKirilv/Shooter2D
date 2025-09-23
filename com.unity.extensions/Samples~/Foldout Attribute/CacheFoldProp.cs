using System.Collections.Generic;
using UnityEditor;

#if UNITY_EDITOR
public class CacheFoldProp
{
    public string Name;
    public bool Expanded;
    public List<SerializedProperty> Props = new List<SerializedProperty>();
    public List<CacheFoldProp> Children = new List<CacheFoldProp>();
    public CacheFoldProp Parent;

    public void Dispose()
    {
        Props.Clear();
        foreach (var child in Children)
        {
            child.Dispose();
        }
        Children.Clear();
    }
}
#endif
