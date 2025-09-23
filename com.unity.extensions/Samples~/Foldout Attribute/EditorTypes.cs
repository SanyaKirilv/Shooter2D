using System;
using System.Reflection;

static class EditorTypes
{
    public static FieldInfo GetFieldInfo(Type type, string fieldName)
    {
        return type.GetField(
            fieldName,
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
        );
    }
}