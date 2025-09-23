#if UnityExtensions
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace UnityExtensions
{
    public static class MemoryUtility
    {
        #region Save Methods

        public static class Save
        {
            public static void Text(string fileName, string filePath, string value)
            {
                string path = Path.Combine(filePath, fileName);

                EnsureDirectory(path);

                File.WriteAllText(path, value);
            }

            public static void Texture2D(string fileName, string filePath, Texture2D value)
            {
                string path = Path.Combine(filePath, fileName);

                EnsureDirectory(path);

                byte[] bytes = TextureUtility.TextureToFile(value);
                File.WriteAllBytes(path, bytes);
            }

            public static void Data(string fileName, string filePath, byte[] value)
            {
                string path = Path.Combine(filePath, fileName);

                EnsureDirectory(path);

                File.WriteAllBytes(path, value);
            }

            public static void Bundle(string fileName, string filePath, byte[] bundleData)
            {
                string path = Path.Combine(filePath, fileName);
                EnsureDirectory(path);
                File.WriteAllBytes(path, bundleData);
            }

            public static class Async
            {
                public static async UniTask Text(string fileName, string filePath, string value)
                {
                    string path = Path.Combine(filePath, fileName);

                    EnsureDirectory(path);

                    await File.WriteAllTextAsync(path, value);
                }

                public static async UniTask Texture2D(
                    string fileName,
                    string filePath,
                    Texture2D value
                )
                {
                    string path = Path.Combine(filePath, fileName);

                    EnsureDirectory(path);

                    byte[] bytes = TextureUtility.TextureToFile(value);
                    await File.WriteAllBytesAsync(path, bytes);
                }

                public static async UniTask Data(string fileName, string filePath, byte[] value)
                {
                    string path = Path.Combine(filePath, fileName);

                    EnsureDirectory(path);

                    await File.WriteAllBytesAsync(path, value);
                }

                public static async UniTask Bundle(
                    string fileName,
                    string filePath,
                    byte[] bundleData
                )
                {
                    string path = Path.Combine(filePath, fileName);
                    EnsureDirectory(path);
                    await File.WriteAllBytesAsync(path, bundleData);
                }
            }
        }

        #endregion

        #region Load Methods

        public static class Load
        {
            public static string Text(string fileName, string filePath)
            {
                string path = Path.Combine(filePath, fileName);
                bool isExsist = File.Exists(path);

                if (!isExsist)
                {
                    DebugLog.Error($"File {fileName} not exsists at path {filePath}");
                    return string.Empty;
                }

                string value = File.ReadAllText(path, System.Text.Encoding.UTF8);
                DebugLog.Info($"File {fileName} exsists!");

                return value;
            }

            public static Texture2D Texture2D(string fileName, string filePath)
            {
                string path = Path.Combine(filePath, fileName);
                bool isExsist = File.Exists(path);

                if (!isExsist)
                {
                    DebugLog.Error($"File {fileName} not exsists at path {filePath}");
                    return new Texture2D(1, 1);
                }

                byte[] bytes = File.ReadAllBytes(path);
                Texture2D value = TextureUtility.TextureFromFile(bytes);
                DebugLog.Info($"File {fileName} exsists!");

                return value;
            }

            public static byte[] Data(string fileName, string filePath)
            {
                string path = Path.Combine(filePath, fileName);
                bool isExsist = File.Exists(path);

                if (!isExsist)
                {
                    DebugLog.Error($"File {fileName} not exsists at path {filePath}");
                    return new byte[0];
                }

                byte[] value = File.ReadAllBytes(path);
                DebugLog.Info($"File {fileName} exsists!");

                return value;
            }

            public static AssetBundle Bundle(string fileName, string filePath)
            {
                string path = Path.Combine(filePath, fileName);
                if (!File.Exists(path))
                {
                    DebugLog.Error($"AssetBundle {fileName} not exists at path {filePath}");
                    return null;
                }

                var bundle = UnityEngine.AssetBundle.LoadFromFile(path);
                DebugLog.Info($"AssetBundle {fileName} loaded!");
                return bundle;
            }

            public static class Async
            {
                public static async UniTask<string> Text(string fileName, string filePath)
                {
                    string path = Path.Combine(filePath, fileName);
                    bool isExsist = File.Exists(path);

                    if (!isExsist)
                    {
                        DebugLog.Error($"File {fileName} not exsists at path {filePath}");
                        return string.Empty;
                    }

                    string value = await File.ReadAllTextAsync(path, System.Text.Encoding.UTF8);
                    DebugLog.Info($"File {fileName} exsists!");

                    return value;
                }

                public static async UniTask<Texture2D> Texture2D(string fileName, string filePath)
                {
                    string path = Path.Combine(filePath, fileName);
                    bool isExsist = File.Exists(path);

                    if (!isExsist)
                    {
                        DebugLog.Error($"File {fileName} not exsists at path {filePath}");
                        return new Texture2D(1, 1);
                    }

                    byte[] bytes = await File.ReadAllBytesAsync(path);
                    Texture2D value = TextureUtility.TextureFromFile(bytes);
                    DebugLog.Info($"File {fileName} exsists!");

                    return value;
                }

                public static async UniTask<byte[]> Data(string fileName, string filePath)
                {
                    string path = Path.Combine(filePath, fileName);
                    bool isExsist = File.Exists(path);

                    if (!isExsist)
                    {
                        DebugLog.Error($"File {fileName} not exsists at path {filePath}");
                        return new byte[0];
                    }

                    byte[] value = await File.ReadAllBytesAsync(path);
                    DebugLog.Info($"File {fileName} exsists!");

                    return value;
                }

                public static async UniTask<AssetBundle> Bundle(string fileName, string filePath)
                {
                    string path = Path.Combine(filePath, fileName);
                    if (!File.Exists(path))
                    {
                        DebugLog.Error($"AssetBundle {fileName} not exists at path {filePath}");
                        return null;
                    }

                    var request = UnityEngine.AssetBundle.LoadFromFileAsync(path);
                    await request;
                    DebugLog.Info($"AssetBundle {fileName} loaded async!");
                    return request.assetBundle;
                }
            }
        }

        #endregion

        #region Helpful Methods

        public static void DeleteDirectory(params string[] filePath)
        {
            string path = Path.Combine(Path.Combine(filePath));
            string dir = Path.GetDirectoryName(path);
            if (Directory.Exists(dir))
            {
                Directory.Delete(path, true);
            }
        }

        public static long GetDirectorySize(params string[] filePath)
        {
            string path = Path.Combine(Path.Combine(filePath));
            EnsureDirectory(path);
            long size = 0;

            foreach (string file in Directory.GetFiles(path, "*", SearchOption.AllDirectories))
            {
                size += new FileInfo(file).Length;
            }

            return size;
        }

        public static void EnsureDirectory(string filePath)
        {
            string dir = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }

        #endregion
    }
}
#endif
