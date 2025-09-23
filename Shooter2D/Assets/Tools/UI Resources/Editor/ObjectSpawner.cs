#if UNITY_EDITOR && UnityExtensions
using UnityEditor;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.EventSystems;

public class ObjectSpawner
{
    private const string PrefabsSubfolder = "UI Resources/Prefabs";
    private const string MenuRoot = "GameObject/Extensions/";

    [MenuItem(MenuRoot + "Image", false, 1)]
    private static void SpawnImage() => SpawnUIElement("Image");

    [MenuItem(MenuRoot + "Text", false, 2)]
    private static void SpawnText() => SpawnUIElement("Text");

    [MenuItem(MenuRoot + "Button", false, 3)]
    private static void SpawnButton() => SpawnUIElement("Button");

    private static void SpawnUIElement(string elementName)
    {
        string samplesPath = FindSamplesPath();
        if (string.IsNullOrEmpty(samplesPath))
        {
            Debug.LogError("Samples folder not found!");
            return;
        }

        string prefabPath = Path.Combine(samplesPath, $"{elementName}.prefab");
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

        if (prefab == null)
        {
            Debug.LogError($"Prefab {elementName} not found at path: {prefabPath}");
            return;
        }

        Transform parent = GetParentTransform();
        GameObject instance = GameObject.Instantiate(prefab, parent);
        instance.name = elementName;
        SetupNewInstance(instance);
    }

    private static string FindSamplesPath()
    {
        string[] possiblePaths = Directory
            .GetDirectories(Path.Combine("Assets", "Tools"), "*", SearchOption.TopDirectoryOnly)
            .Select(dir => Path.Combine(dir, PrefabsSubfolder))
            .Concat(
                new[]
                {
                    Path.Combine("Assets", "Tools", PrefabsSubfolder),
                    Path.Combine("Assets", PrefabsSubfolder),
                }
            )
            .ToArray();

        foreach (var path in possiblePaths)
        {
            if (Directory.Exists(path))
            {
                return path;
            }
        }

        return null;
    }

    private static Transform GetParentTransform()
    {
        Transform parent = Selection.activeTransform;
        if (parent != null && parent.GetComponentInParent<Canvas>())
            return parent;

        Canvas canvas = GetOrCreateCanvas();
        CreateEventSystemIfNeeded();
        return canvas.transform;
    }

    private static void SetupNewInstance(GameObject instance)
    {
        Undo.RegisterCreatedObjectUndo(instance, $"Create {instance.name}");
        Selection.activeGameObject = instance;

        RectTransform rt = instance.GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.anchoredPosition = Vector3.zero;
            rt.localScale = Vector3.one;
            rt.sizeDelta = Vector2.zero;
        }

        SceneView.lastActiveSceneView?.FrameSelected();
    }

    private static Canvas GetOrCreateCanvas()
    {
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasGo = new GameObject("Canvas");
            canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGo.AddComponent<CanvasScaler>();
            canvasGo.AddComponent<GraphicRaycaster>();
            Undo.RegisterCreatedObjectUndo(canvasGo, "Create Canvas");
        }
        return canvas;
    }

    private static void CreateEventSystemIfNeeded()
    {
        if (Object.FindFirstObjectByType<EventSystem>() == null)
        {
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<StandaloneInputModule>();
            Undo.RegisterCreatedObjectUndo(eventSystem, "Create EventSystem");
        }
    }

    [MenuItem(MenuRoot + "Image", true)]
    [MenuItem(MenuRoot + "Text", true)]
    [MenuItem(MenuRoot + "Button", true)]
    private static bool ValidateSpawnUIElement()
    {
        return FindSamplesPath() != null;
    }
}
#endif
