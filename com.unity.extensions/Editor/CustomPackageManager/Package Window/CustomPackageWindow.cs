#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
internal class StartupHelper
{
    static StartupHelper()
    {
        EditorApplication.update += Startup;
    }

    private static void Startup()
    {
        EditorApplication.update -= Startup;

        if (!CustomPackageWindow.WasButtonPressed())
        {
            CustomPackageWindow.ShowWindow();
        }
    }
}

public partial class CustomPackageWindow : EditorWindow
{
    private Vector2 _scrollPos;
    private const string _packageName = "com.unity.extensions";
    private static CustomPackageWindow _window = null;
    private static CustomPackageData _packageData = new CustomPackageData();

    private static readonly string _dontShow = "DontShowAgain";

    [MenuItem("Window/Package Manager", false, 1)]
    public static void ShowWindow()
    {
        LoadPackageData();
        ManageWindows();
    }

    private void Update()
    {
        Repaint();
    }

    private void OnGUI()
    {
        if (_window == null)
        {
            return;
        }

        DrawPackageData();
    }

    protected void OnEnable()
    {
        LoadPackageData();
        EditorApplication.hierarchyChanged += OnHierarchyChanged;
    }

    protected void OnDisable()
    {
        EditorApplication.hierarchyChanged -= OnHierarchyChanged;
    }

    protected void OnFocus()
    {
        LoadPackageData();
    }

    protected void OnDestroy()
    {
        _window = null;
    }

    protected void OnHierarchyChanged()
    {
        LoadPackageData();
        Repaint();
    }

    public static bool WasButtonPressed()
    {
        return EditorPrefs.GetBool(_dontShow, false);
    }

    private static void ManageWindows()
    {
        CustomPackageWindow[] windows = Resources.FindObjectsOfTypeAll<CustomPackageWindow>();
        if (windows.Length == 1)
        {
            windows[0].Focus();
            return;
        }
        else if (windows.Length > 1)
        {
            for (int i = 1; i < windows.Length; i++)
            {
                windows[i].Close();
            }

            windows[0].Focus();
            return;
        }

        _window = GetWindow<CustomPackageWindow>(true, "Custom package", true);
        _window.maxSize = _window.minSize = WindowRect;
    }

    private static void LoadPackageData()
    {
        string filePath = $"Packages/{_packageName}/package.json";
        if (!File.Exists(filePath))
        {
            _packageData = new CustomPackageData();
            Debug.LogError("Package not found!");
            return;
        }

        string data = File.ReadAllText(filePath);
        _packageData = JsonUtility.FromJson<CustomPackageData>(data);
    }

    private void DrawPackageData()
    {
        Vector2 logo = DrawLogo(Vector2.zero);

        if (_packageData == null)
        {
            _ = DrawNoPackage(logo);
            return;
        }

        Vector2 packageMainData = DrawPackageMainData(logo);
        Vector2 dependencyButtons = DrawDependencyButtons(packageMainData);
        _ = DrawSamples(dependencyButtons);

        ButtonData dontShow = new ButtonData()
        {
            Text = "Dont show on next launch",
            IsAvailable = true,
            IsLinkButton = false,
            IsVisible = !WasButtonPressed(),
            Width = 160,
            Height = 20,
            xOffset = WindowRect.x - 160 - 6,
            yOffset = WindowRect.y - 20 - 6,
            BackgroundColor = Color.clear,
        };
        dontShow.OnClick += static () => EditorPrefs.SetBool(_dontShow, true);
        dontShow.Draw();
    }

    private Vector2 DrawLogo(Vector2 offset)
    {
        LogoRect logo = new LogoRect()
        {
            Texture2D = Resources.Load<Texture2D>("logo"),
            Width = WindowRect.x,
            Height = 128f,
            xOffset = 0,
            yOffset = 0,
            ImageSize = new Vector2(256f, 93f),
            BackgroundColor = Color.white,
        };
        logo.Draw();

        return logo.Offset;
    }

    private Vector2 DrawNoPackage(Vector2 offset)
    {
        MessageData message = new MessageData()
        {
            Text = "Package info not loaded.",
            Type = MessageType.Warning,
            xOffset = 8,
            yOffset = offset.y + 8,
            BackgroundColor = Color.clear,
        };
        message.Draw();

        ButtonData reload = new ButtonData()
        {
            Text = "Reload",
            IsAvailable = true,
            IsLinkButton = false,
            IsVisible = true,
            Width = 88,
            Height = 20,
            xOffset = (WindowRect.x - 88) / 2,
            yOffset = message.Offset.y + 6,
            BackgroundColor = Color.clear,
        };
        reload.OnClick += static () => LoadPackageData();
        reload.Draw();

        _window.maxSize = _window.minSize = new Vector2(WindowRect.x, reload.Offset.y + 8);

        return reload.Offset;
    }

    private Vector2 DrawFileButtons(Vector2 offset)
    {
        ButtonData documentation = new ButtonData()
        {
            Text = "Documentation",
            IsAvailable = false,
            IsLinkButton = true,
            IsVisible = true,
            Width = 88,
            Height = 20,
            xOffset = 6,
            yOffset = offset.y,
            BackgroundColor = Color.clear,
        };
        documentation.OnClick += static () => SelectFile("Changelog.md");
        documentation.Draw();

        ButtonData changelog = new ButtonData()
        {
            Text = "Changelog",
            IsAvailable = false,
            IsLinkButton = true,
            IsVisible = true,
            Width = 64,
            Height = 20,
            xOffset = documentation.Offset.x + 6,
            yOffset = offset.y,
            BackgroundColor = Color.clear,
        };
        changelog.OnClick += static () => SelectFile("Changelog.md");
        changelog.Draw();

        ButtonData license = new ButtonData()
        {
            Text = "License",
            IsAvailable = true,
            IsLinkButton = true,
            IsVisible = true,
            Width = 46,
            Height = 20,
            xOffset = changelog.Offset.x + 6,
            yOffset = offset.y,
            BackgroundColor = Color.clear,
        };
        license.OnClick += static () => SelectFile("LICENSE");
        license.Draw();

        ButtonData contact = new ButtonData()
        {
            Text = "Contact",
            IsAvailable = true,
            IsLinkButton = true,
            IsVisible = true,
            Width = 52,
            Height = 20,
            xOffset = license.Offset.x + 6,
            yOffset = offset.y,
            BackgroundColor = Color.clear,
        };
        contact.OnClick += static () => OpenMailApp();
        contact.Draw();

        return contact.Offset;
    }

    private static void SelectFile(string fileName)
    {
        string path = $"Packages/{_packageName}/{fileName}";
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(path);
        EditorGUIUtility.PingObject(Selection.activeObject);
    }

    private static void OpenMailApp()
    {
        string default_subject = "Support Request";
        string default_body = $"Dear {_packageData.author.name},\n\n";
        string url =
            $"mailto:{_packageData.author.email}?"
            + $"subject={EscapeUrl(default_subject)}"
            + $"&body={EscapeUrl(default_body)}";

        Application.OpenURL(url);
    }

    private static string EscapeUrl(string url)
    {
        return UnityEngine
            .Networking.UnityWebRequest.EscapeURL(url)
            .Replace("+", "%20")
            .Replace("%0A", "%0D%0A");
    }

    private Vector2 DrawPackageMainData(Vector2 offset)
    {
        TextData title = new TextData()
        {
            Text = _packageData.displayName,
            Width = WindowRect.x,
            Height = 32,
            xOffset = 8,
            yOffset = offset.y,
            FontSize = 18,
            FontStyle = FontStyle.Bold,
            Aligment = TextAnchor.MiddleLeft,
            Wrapped = false,
            FontColor = new Color(200, 200, 200, .75f),
            BackgroundColor = Color.clear,
        };
        title.Draw();

        TextData version = new TextData()
        {
            Text = $"Version: {_packageData.version}",
            Width = WindowRect.x,
            Height = 20,
            xOffset = 8,
            yOffset = title.Offset.y,
            FontSize = 12,
            FontStyle = FontStyle.Bold,
            Aligment = TextAnchor.MiddleLeft,
            Wrapped = false,
            FontColor = new Color(200, 200, 200, .75f),
            BackgroundColor = Color.clear,
        };
        version.Draw();

        TextData autor = new TextData()
        {
            Text = $"By {_packageData.author.name}",
            Width = WindowRect.x,
            Height = 20,
            xOffset = 8,
            yOffset = version.Offset.y,
            FontSize = 12,
            FontStyle = FontStyle.Normal,
            Aligment = TextAnchor.MiddleLeft,
            Wrapped = false,
            FontColor = new Color(200, 200, 200, .75f),
            BackgroundColor = Color.clear,
        };
        autor.Draw();

        TextData package = new TextData()
        {
            Text = _packageData.name,
            Width = WindowRect.x,
            Height = 20,
            xOffset = 8,
            yOffset = autor.Offset.y,
            FontSize = 12,
            FontStyle = FontStyle.Italic,
            Aligment = TextAnchor.MiddleLeft,
            Wrapped = false,
            FontColor = new Color(200, 200, 200, .75f),
            BackgroundColor = Color.clear,
        };
        package.Draw();

        Vector2 fileButtons = DrawFileButtons(package.Offset);

        MessageData description = new MessageData()
        {
            Text = _packageData.description,
            Type = MessageType.Info,
            xOffset = 8,
            yOffset = fileButtons.y + 8,
            BackgroundColor = Color.clear,
        };
        description.Draw();

        return description.Offset;
    }

    private Vector2 DrawDependencyButtons(Vector2 offset)
    {
        bool state = PackageLoader.CheckDependencies();

        ButtonData add = new ButtonData()
        {
            Text = "Add Dependencies",
            IsAvailable = true,
            IsLinkButton = false,
            IsVisible = !state,
            Width = 144,
            Height = 20,
            xOffset = WindowRect.x - 144 - 8,
            yOffset = offset.y + 8,
            BackgroundColor = Color.clear,
        };
        add.OnClick += static () => PackageLoader.Load();
        add.Draw();

        ButtonData remove = new ButtonData()
        {
            Text = "Remove Dependencies",
            IsAvailable = true,
            IsLinkButton = false,
            IsVisible = state,
            Width = 144,
            Height = 20,
            xOffset = WindowRect.x - 144 - 8,
            yOffset = offset.y + 8,
            BackgroundColor = Color.clear,
        };
        remove.OnClick += static () => PackageLoader.Remove();
        remove.Draw();

        Vector2 currentOffset = add.IsVisible ? add.Offset : remove.Offset;

        return currentOffset;
    }

    private Vector2 DrawSamples(Vector2 offset)
    {
        if (_packageData.samples?.Length == 0)
        {
            _window.maxSize = _window.minSize = new Vector2(WindowRect.x, offset.y + 8);
            return offset;
        }

        TextData title = new TextData()
        {
            Text = "Samples:",
            Width = WindowRect.x,
            Height = 16,
            xOffset = 8,
            yOffset = offset.y + 8,
            FontSize = 16,
            FontStyle = FontStyle.Bold,
            Aligment = TextAnchor.MiddleLeft,
            Wrapped = false,
            FontColor = new Color(200, 200, 200, .75f),
            BackgroundColor = Color.clear,
        };
        title.Draw();

        PanelData background = new PanelData()
        {
            Height = WindowRect.y - offset.y,
            Width = WindowRect.x,
            xOffset = 0,
            yOffset = title.Offset.y + 8,
            BackgroundColor = new Color32(43, 41, 43, 255),
        };
        background.Draw();

        GUILayout.Space(title.Offset.y + 8);

        using (EditorGUILayout.VerticalScope v = new EditorGUILayout.VerticalScope())
        {
            using EditorGUILayout.ScrollViewScope scrollView = new EditorGUILayout.ScrollViewScope(
                _scrollPos,
                GUILayout.Width(WindowRect.x),
                GUILayout.Height(WindowRect.y - title.Offset.y)
            );
            _scrollPos = scrollView.scrollPosition;

            Vector2 sampleOffset = new Vector2(0, 8);

            foreach (SampleData sample in _packageData.samples)
            {
                sampleOffset = DrawSample(sample, sampleOffset);
                sampleOffset += new Vector2(0, 12);
            }
        }

        return title.Offset;
    }

    private Vector2 DrawSample(SampleData sample, Vector2 offset)
    {
        TextData title = new TextData()
        {
            Text = sample.displayName,
            Width = WindowRect.x,
            Height = 20,
            xOffset = 16,
            yOffset = offset.y,
            FontSize = 12,
            FontStyle = FontStyle.Bold,
            Aligment = TextAnchor.MiddleLeft,
            Wrapped = false,
            FontColor = new Color(200, 200, 200, .75f),
            BackgroundColor = Color.clear,
        };
        title.Draw();

        TextData weight = new TextData()
        {
            Text = SampleSize(sample),
            Width = WindowRect.x,
            Height = 20,
            xOffset = title.WrappedWidth + title.xOffset + 4,
            yOffset = offset.y,
            FontSize = 10,
            FontStyle = FontStyle.Italic,
            Aligment = TextAnchor.MiddleLeft,
            Wrapped = false,
            FontColor = new Color(200, 200, 200, .75f),
            BackgroundColor = Color.clear,
        };
        weight.Draw();

        TextData description = new TextData()
        {
            Text = Trim(sample.description, 112),
            Width = WindowRect.x - 144,
            Height = 16,
            xOffset = 16,
            yOffset = title.Offset.y + 2,
            FontSize = 12,
            FontStyle = FontStyle.Normal,
            Aligment = TextAnchor.UpperLeft,
            Wrapped = true,
            FontColor = new Color(200, 200, 200, .75f),
            BackgroundColor = Color.clear,
        };
        description.Draw();

        bool isImported = IsSampleImported(sample);

        ButtonData add = new ButtonData()
        {
            Text = "Import",
            IsAvailable = true,
            IsLinkButton = false,
            IsVisible = !isImported,
            Width = 56,
            Height = 20,
            xOffset = WindowRect.x - 56 - 8,
            yOffset = offset.y,
            BackgroundColor = Color.clear,
        };
        add.OnClick += () => ImportSample(sample);
        add.Draw();

        ButtonData remove = new ButtonData()
        {
            Text = "Remove",
            IsAvailable = true,
            IsLinkButton = false,
            IsVisible = isImported,
            Width = 64,
            Height = 20,
            xOffset = WindowRect.x - 64 - 8,
            yOffset = offset.y,
            BackgroundColor = Color.clear,
        };
        remove.OnClick += () => RemoveSample(sample);
        remove.Draw();

        return description.Offset;
    }

    private string Trim(string source, int length)
    {
        if (source.Length <= length)
        {
            return source;
        }

        int lastSpace = source.LastIndexOf(' ', length);
        int trimPosition = lastSpace > 0 ? lastSpace : length;

        return source[..trimPosition] + "...";
    }

    private string SampleSize(SampleData sample)
    {
        string path = $"Packages/{_packageName}/{sample.path}";
        long sizeBytes = GetDirectorySize(path);

        string[] units = { "B", "KB", "MB", "GB", "TB" };
        int unitIndex = 0;
        float size = sizeBytes;

        while (size >= 1024f && unitIndex < units.Length - 1)
        {
            size /= 1024f;
            unitIndex++;
        }

        return $"{size:F2} {units[unitIndex]}";
    }

    private long GetDirectorySize(string path)
    {
        if (!Directory.Exists(path))
        {
            return 0;
        }

        long size = 0;
        string[] files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
        foreach (string file in files)
        {
            FileInfo info = new FileInfo(file);
            size += info.Length;
        }
        return size;
    }

    private bool IsSampleImported(SampleData sample)
    {
        string targetPath = Path.Combine("Assets/Tools", Path.GetFileName(sample.path));
        return Directory.Exists(targetPath);
    }

    private void ImportSample(SampleData sample)
    {
        string sourcePath = $"Packages/{_packageName}/{sample.path}";
        string targetPath = Path.Combine("Assets/Tools", Path.GetFileName(sample.path));

        if (!Directory.Exists(sourcePath))
        {
            Debug.LogError("Sample source folder not found: " + sourcePath);
            return;
        }

        CopyDirectory(sourcePath, targetPath);
        AssetDatabase.Refresh();
        Debug.Log($"Sample {sample.displayName} added.");
    }

    private void RemoveSample(SampleData sample)
    {
        string targetPath = Path.Combine("Assets/Tools", Path.GetFileName(sample.path));
        if (Directory.Exists(targetPath))
        {
            Directory.Delete(targetPath, true);
            File.Delete(targetPath + ".meta");
            AssetDatabase.Refresh();
            Debug.Log($"Sample {sample.displayName} removed.");
        }
    }

    private void CopyDirectory(string sourceDir, string destinationDir)
    {
        if (!Directory.Exists(destinationDir))
        {
            Directory.CreateDirectory(destinationDir);
        }

        foreach (string file in Directory.GetFiles(sourceDir))
        {
            string destFile = Path.Combine(destinationDir, Path.GetFileName(file));
            File.Copy(file, destFile, true);
        }

        foreach (string directory in Directory.GetDirectories(sourceDir))
        {
            string destDir = Path.Combine(destinationDir, Path.GetFileName(directory));
            CopyDirectory(directory, destDir);
        }
    }
}
#endif
