#if UNITY_EDITOR
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

public static class PackageLoader
{
    private const string MAIN_DEFINE = "UnityExtensions";
    private static string ManifestPath =>
        Path.Combine(Application.dataPath, "..", "Packages", "manifest.json");

    [MenuItem("Window/Load Dependencies")]
    public static void Load()
    {
        if (!File.Exists(ManifestPath))
        {
            Debug.LogError("manifest.json not found!");
            return;
        }

        string manifestText = File.ReadAllText(ManifestPath);

        foreach (var package in DependencyData.dependencies)
        {
            if (!manifestText.Contains($"\"{package.name}\":"))
            {
                manifestText = InsertDependency(manifestText, package.name, package.version);
                Debug.Log($"Added package: {package.name}@{package.version}");
            }
        }

        foreach (var registry in DependencyData.scopedRegistries)
        {
            if (!manifestText.Contains($"\"name\": \"{registry.name}\""))
            {
                manifestText = InsertScopedRegistry(manifestText, registry);
                Debug.Log($"Added registry: {registry.name}");
            }
        }

        File.WriteAllText(ManifestPath, manifestText);
        AddDefineSymbol();
        AssetDatabase.Refresh();
    }

    [MenuItem("Window/Remove Dependencies")]
    public static void Remove()
    {
        if (!File.Exists(ManifestPath))
            return;

        string manifestText = File.ReadAllText(ManifestPath);

        foreach (var package in DependencyData.dependencies)
        {
            manifestText = RemoveDependency(manifestText, package.name);
        }

        foreach (var registry in DependencyData.scopedRegistries)
        {
            manifestText = RemoveScopedRegistry(manifestText, registry.name);
        }

        File.WriteAllText(ManifestPath, manifestText);
        RemoveDefineSymbol();
        AssetDatabase.Refresh();
    }

    public static bool CheckDependencies()
    {
        if (!File.Exists(ManifestPath))
        {
            Debug.LogError("manifest.json not found!");
            return false;
        }

        try
        {
            string manifestText = File.ReadAllText(ManifestPath);
            bool allPresent = true;

            foreach (var package in DependencyData.dependencies)
            {
                if (!manifestText.Contains($"\"{package.name}\":"))
                {
                    Debug.Log($"Package missing: {package.name}");
                    allPresent = false;
                }
            }

            foreach (var registry in DependencyData.scopedRegistries)
            {
                if (!manifestText.Contains($"\"name\": \"{registry.name}\""))
                {
                    Debug.Log($"Registry missing: {registry.name}");
                    allPresent = false;
                }
            }

            return allPresent;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error checking manifest: {e.Message}");
            return false;
        }
    }

    private static string InsertDependency(string manifest, string packageName, string version)
    {
        var match = Regex.Match(manifest, "\"dependencies\"\\s*:\\s*\\{([^}]*)\\}");
        if (match.Success)
        {
            string depsContent = match.Groups[1].Value.Trim();
            string newDep = $"\"{packageName}\": \"{version}\"";

            string newDepsContent = depsContent;
            if (!string.IsNullOrEmpty(depsContent))
                newDepsContent += ",\n    ";
            newDepsContent += newDep;

            return manifest.Replace(
                match.Groups[0].Value,
                $"\"dependencies\": {{\n    {newDepsContent}\n  }}"
            );
        }
        return manifest;
    }

    private static string RemoveDependency(string manifest, string packageName)
    {
        string pattern = $"\"{packageName}\"\\s*:\\s*\"[^\"]+\"(,?\\n?\\s*)";
        return Regex.Replace(manifest, pattern, string.Empty);
    }

    private static string InsertScopedRegistry(
        string manifest,
        RegistryData registry,
        bool isLast = true
    )
    {
        string registryJson =
            $@"
    {{
      ""name"": ""{registry.name}"",
      ""url"": ""{registry.url}"",
      ""scopes"": [{(string.Join(", ", registry.scopes.Select(s => $"\"{s}\"")))}]
    }}";

        if (manifest.Contains("\"scopedRegistries\":"))
        {
            return manifest.Replace(
                "\"scopedRegistries\": [",
                $"\"scopedRegistries\": [{registryJson},"
            );
        }
        else
        {
            string insertionPoint = "\"dependencies\": {";
            return manifest.Replace(
                insertionPoint,
                $"\"scopedRegistries\": [{registryJson}]{(isLast ? "" : ",")}\n  {insertionPoint}"
            );
        }
    }

    private static string RemoveScopedRegistry(string manifest, string registryName)
    {
        string pattern = $"\\{{\\s*\"name\"\\s*:\\s*\"{registryName}\".*?\\}}(,?\\n?\\s*)";
        return Regex.Replace(manifest, pattern, string.Empty, RegexOptions.Singleline);
    }

    private static void AddDefineSymbol()
    {
        NamedBuildTarget namedBuildTarget = NamedBuildTarget.FromBuildTargetGroup(
            EditorUserBuildSettings.selectedBuildTargetGroup
        );
        string defines = PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget);

        if (!defines.Contains(MAIN_DEFINE))
        {
            PlayerSettings.SetScriptingDefineSymbols(
                namedBuildTarget,
                string.IsNullOrEmpty(defines) ? MAIN_DEFINE : $"{defines};{MAIN_DEFINE}"
            );
        }
    }

    private static void RemoveDefineSymbol()
    {
        NamedBuildTarget namedBuildTarget = NamedBuildTarget.FromBuildTargetGroup(
            EditorUserBuildSettings.selectedBuildTargetGroup
        );
        string defines = PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget);

        if (defines.Contains(MAIN_DEFINE))
        {
            string[] newDefines = defines.Split(';').Where(static d => d != MAIN_DEFINE).ToArray();

            PlayerSettings.SetScriptingDefineSymbols(
                namedBuildTarget,
                string.Join(";", newDefines)
            );
        }
    }
}
#endif
