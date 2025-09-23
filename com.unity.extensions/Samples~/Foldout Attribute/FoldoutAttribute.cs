using UnityEngine;

public class FoldoutAttribute : PropertyAttribute
{
    public string Name;
    public bool FoldEverything;

    public FoldoutAttribute(string name, bool foldEverything = false)
    {
        Name = name;
        FoldEverything = foldEverything;
    }
}
