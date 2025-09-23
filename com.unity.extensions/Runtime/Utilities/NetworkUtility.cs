#if UnityExtensions
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Extensions.Unity.ImageLoader;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace UnityExtensions
{
    #region Data Structure

    [Serializable]
    public class Response
    {
        public string JSON = string.Empty;
        public bool Success = false;

        public Response(UnityWebRequest request, bool skip = false)
        {
            if (skip)
            {
                Success = true;
                return;
            }

            JSON = request.downloadHandler.text;
            Success = request.result == UnityWebRequest.Result.Success;
        }

        public T Convert<T>(bool standart = true)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(JSON))
                {
                    throw new Exception(
                        "Failed to deserialize JSON. Reason - Is Null or WhiteSpace"
                    );
                }

                return standart
                    ? JsonConvert.DeserializeObject<T>(JSON)
                    : JsonUtility.FromJson<T>(JSON);
            }
            catch (Exception exception)
            {
                DebugLog.Exception(exception);
                return default;
            }
        }

        public ErrorData Error
        {
            get
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(JSON))
                    {
                        return new ErrorData("Request", "Unknown request error");
                    }

                    return JsonConvert.DeserializeObject<ErrorData>(JSON);
                }
                catch (Exception exception)
                {
                    DebugLog.Exception(exception);
                    return default;
                }
            }
        }
    }

    [Serializable]
    public class ErrorData
    {
        public string Type = string.Empty;
        public string Message = string.Empty;

        public bool IsNull
        {
            get => string.IsNullOrWhiteSpace(Type);
        }

        public static ErrorData Empty
        {
            get => new ErrorData();
        }

        public ErrorData(string type = null, string message = null)
        {
            Type = type ?? string.Empty;
            Message = message ?? string.Empty;
        }

        public ErrorData()
            : this(null, null) { }

        public override string ToString()
        {
            return $"Error: {Type} - {Message}";
        }
    }

    #endregion

    #region Main Class

    public static partial class NetworkUtility
    {
        public static class Request
        {
            public static class Get
            {
                public static async UniTask<Response> Data(
                    string url,
                    CancellationToken cancellationToken = default,
                    Action<float> progress = null,
                    int timeout = 20
                )
                {
                    return await ProcessRequest(
                        url: url,
                        method: UnityWebRequest.kHttpVerbGET,
                        json: null,
                        token: null,
                        cancellationToken: cancellationToken,
                        progress: progress,
                        timeout: timeout
                    );
                }

                public static async UniTask<Response> Data(
                    string url,
                    string token,
                    CancellationToken cancellationToken = default,
                    Action<float> progress = null,
                    int timeout = 20
                )
                {
                    return await ProcessRequest(
                        url: url,
                        method: UnityWebRequest.kHttpVerbGET,
                        json: null,
                        token: token,
                        cancellationToken: cancellationToken,
                        progress: progress,
                        timeout: timeout
                    );
                }

                public static async UniTask<Response> Data(
                    string url,
                    string token,
                    object data,
                    CancellationToken cancellationToken = default,
                    Action<float> progress = null,
                    int timeout = 20
                )
                {
                    return await ProcessRequest(
                        url: url,
                        method: UnityWebRequest.kHttpVerbGET,
                        json: JsonConvert.SerializeObject(data),
                        token: token,
                        cancellationToken: cancellationToken,
                        progress: progress,
                        timeout: timeout
                    );
                }

                public static async UniTask<Response> Data(
                    string url,
                    string token,
                    bool isAuthenticated,
                    CancellationToken cancellationToken = default,
                    Action<float> progress = null,
                    int timeout = 20
                )
                {
                    return await ProcessRequest(
                        url: url,
                        method: UnityWebRequest.kHttpVerbGET,
                        json: null,
                        token: isAuthenticated ? token : null,
                        cancellationToken: cancellationToken,
                        progress: progress,
                        timeout: timeout
                    );
                }

                public static async UniTask<Response> PrivateData(
                    string url,
                    string token,
                    bool isAuthenticated,
                    CancellationToken cancellationToken = default,
                    Action<float> progress = null,
                    int timeout = 20
                )
                {
                    if (!isAuthenticated)
                    {
                        return new Response(null, true);
                    }

                    return await ProcessRequest(
                        url: url,
                        method: UnityWebRequest.kHttpVerbGET,
                        json: null,
                        token: token,
                        cancellationToken: cancellationToken,
                        progress: progress,
                        timeout: timeout
                    );
                }
            }

            public static class Post
            {
                public static async UniTask<Response> Data(
                    string url,
                    object data,
                    CancellationToken cancellationToken = default,
                    Action<float> progress = null,
                    int timeout = 20
                )
                {
                    return await ProcessRequest(
                        url: url,
                        method: UnityWebRequest.kHttpVerbPOST,
                        json: JsonConvert.SerializeObject(data),
                        token: null,
                        cancellationToken: cancellationToken,
                        progress: progress,
                        timeout: timeout
                    );
                }

                public static async UniTask<Response> Data(
                    string url,
                    string token,
                    object data,
                    CancellationToken cancellationToken = default,
                    Action<float> progress = null,
                    int timeout = 20
                )
                {
                    return await ProcessRequest(
                        url: url,
                        method: UnityWebRequest.kHttpVerbPOST,
                        json: JsonConvert.SerializeObject(data),
                        token: token,
                        cancellationToken: cancellationToken,
                        progress: progress,
                        timeout: timeout
                    );
                }

                public static async UniTask<Response> Data(
                    string url,
                    string token,
                    bool isAuthenticated,
                    object data,
                    CancellationToken cancellationToken = default,
                    Action<float> progress = null,
                    int timeout = 20
                )
                {
                    return await ProcessRequest(
                        url: url,
                        method: UnityWebRequest.kHttpVerbPOST,
                        json: JsonConvert.SerializeObject(data),
                        token: isAuthenticated ? token : null,
                        cancellationToken: cancellationToken,
                        progress: progress,
                        timeout: timeout
                    );
                }
            }

            public static class Put
            {
                public static async UniTask<Response> Data(
                    string url,
                    string token,
                    object data,
                    CancellationToken cancellationToken = default,
                    Action<float> progress = null,
                    int timeout = 20
                )
                {
                    return await ProcessRequest(
                        url: url,
                        method: UnityWebRequest.kHttpVerbPUT,
                        json: JsonConvert.SerializeObject(data),
                        token: token,
                        cancellationToken: cancellationToken,
                        progress: progress,
                        timeout: timeout
                    );
                }
            }

            #region Process Methods

            private static async UniTask<Response> ProcessRequest(
                string url,
                string method,
                string json = null,
                string token = null,
                CancellationToken cancellationToken = default,
                Action<float> progress = null,
                int timeout = 20
            )
            {
                using UnityWebRequest request = new UnityWebRequest(url, method);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.timeout = timeout;
                request.SetRequestHeader("Content-Type", "application/json");

                string sizeStr = request.GetResponseHeader("Content-Length");
                if (long.TryParse(sizeStr, out long size))
                {
                    Debug.Log($"Bundle size: {size / 1024f / 1024f:F2} MB");
                }

                if (!string.IsNullOrEmpty(token))
                {
                    request.SetRequestHeader("Authorization", $"Bearer {token}");
                }

                if (!string.IsNullOrEmpty(json))
                {
                    request.uploadHandler = new UploadHandlerRaw(
                        System.Text.Encoding.UTF8.GetBytes(json)
                    );
                }

                Progress<float> requestProgress =
                    progress != null ? new Progress<float>(progress) : null;

                try
                {
                    await request
                        .SendWebRequest()
                        .ToUniTask(cancellationToken: cancellationToken, progress: requestProgress);
                    return new Response(request);
                }
                catch (OperationCanceledException)
                {
                    DebugLog.Warning($"Request {method} canceled: {url}");
                    return new Response(request);
                }
                catch (Exception exception)
                {
                    DebugLog.Exception(exception);
                    DebugLog.Error(url);
                    return new Response(request);
                }
                finally
                {
                    request.Dispose();
                    DebugLog.Info($"Request {method} completed!");
                }
            }

            #endregion
        }

        public static class Load
        {
            #region Text Loading

            public static async UniTask<string> Text(string url)
            {
                return (string)await ProcessLoading<string>(url, default, null, default);
            }

            public static async UniTask<string> Text(
                string url,
                CancellationToken cancellationToken
            )
            {
                return (string)await ProcessLoading<string>(url, cancellationToken, null, default);
            }

            public static async UniTask<string> Text(string url, Action<float> progress)
            {
                return (string)await ProcessLoading<string>(url, default, progress, default);
            }

            public static async UniTask<string> Text(string url, int timeout)
            {
                return (string)await ProcessLoading<string>(url, default, null, timeout);
            }

            public static async UniTask<string> Text(
                string url,
                Action<float> progress,
                int timeout
            )
            {
                return (string)await ProcessLoading<string>(url, default, progress, timeout);
            }

            public static async UniTask<string> Text(
                string url,
                CancellationToken cancellationToken = default,
                Action<float> progress = null,
                int timeout = 20
            )
            {
                return (string)
                    await ProcessLoading<string>(url, cancellationToken, progress, timeout);
            }

            #endregion

            #region Texture2D Loading

            public static async UniTask<Texture2D> Texture2D(string url)
            {
                return (Texture2D)await ProcessLoading<Texture2D>(url, default, null, default);
            }

            public static async UniTask<Texture2D> Texture2D(
                string url,
                CancellationToken cancellationToken
            )
            {
                return (Texture2D)
                    await ProcessLoading<Texture2D>(url, cancellationToken, null, default);
            }

            public static async UniTask<Texture2D> Texture2D(string url, Action<float> progress)
            {
                return (Texture2D)await ProcessLoading<Texture2D>(url, default, progress, default);
            }

            public static async UniTask<Texture2D> Texture2D(string url, int timeout)
            {
                return (Texture2D)await ProcessLoading<Texture2D>(url, default, null, timeout);
            }

            public static async UniTask<Texture2D> Texture2D(
                string url,
                Action<float> progress,
                int timeout
            )
            {
                return (Texture2D)await ProcessLoading<Texture2D>(url, default, progress, timeout);
            }

            public static async UniTask<Texture2D> Texture2D(
                string url,
                CancellationToken cancellationToken = default,
                Action<float> progress = null,
                int timeout = 20
            )
            {
                return (Texture2D)
                    await ProcessLoading<Texture2D>(url, cancellationToken, progress, timeout);
            }

            #endregion

            #region Bundle Loading

            public static async UniTask<AssetBundle> Bundle(string url)
            {
                return (AssetBundle)await ProcessLoading<AssetBundle>(url, default, null, default);
            }

            public static async UniTask<AssetBundle> Bundle(
                string url,
                CancellationToken cancellationToken
            )
            {
                return (AssetBundle)
                    await ProcessLoading<AssetBundle>(url, cancellationToken, null, default);
            }

            public static async UniTask<AssetBundle> Bundle(string url, Action<float> progress)
            {
                return (AssetBundle)
                    await ProcessLoading<AssetBundle>(url, default, progress, default);
            }

            public static async UniTask<AssetBundle> Bundle(string url, int timeout)
            {
                return (AssetBundle)await ProcessLoading<AssetBundle>(url, default, null, timeout);
            }

            public static async UniTask<AssetBundle> Bundle(
                string url,
                Action<float> progress,
                int timeout
            )
            {
                return (AssetBundle)
                    await ProcessLoading<AssetBundle>(url, default, progress, timeout);
            }

            public static async UniTask<AssetBundle> Bundle(
                string url,
                CancellationToken cancellationToken = default,
                Action<float> progress = null,
                int timeout = 20
            )
            {
                return (AssetBundle)
                    await ProcessLoading<AssetBundle>(url, cancellationToken, progress, timeout);
            }

            #endregion

            #region Byte Loading

            public static async UniTask<byte[]> Byte(string url)
            {
                return (byte[])await ProcessLoading<byte[]>(url, default, null, default);
            }

            public static async UniTask<byte[]> Byte(
                string url,
                CancellationToken cancellationToken
            )
            {
                return (byte[])await ProcessLoading<byte[]>(url, cancellationToken, null, default);
            }

            public static async UniTask<byte[]> Byte(string url, Action<float> progress)
            {
                return (byte[])await ProcessLoading<byte[]>(url, default, progress, default);
            }

            public static async UniTask<byte[]> Byte(string url, int timeout)
            {
                return (byte[])await ProcessLoading<byte[]>(url, default, null, timeout);
            }

            public static async UniTask<byte[]> Byte(
                string url,
                Action<float> progress,
                int timeout
            )
            {
                return (byte[])await ProcessLoading<byte[]>(url, default, progress, timeout);
            }

            public static async UniTask<byte[]> Byte(
                string url,
                CancellationToken cancellationToken = default,
                Action<float> progress = null,
                int timeout = 20
            )
            {
                return (byte[])
                    await ProcessLoading<byte[]>(url, cancellationToken, progress, timeout);
            }

            #endregion

            #region Sprite Loading

            public static async UniTask Image(Image image, GameObject background, string url)
            {
                if (string.IsNullOrWhiteSpace(url))
                {
                    DebugLog.Warning($"URL is not valid {image?.name}");
                    return;
                }

                try
                {
                    Sprite sprite = await ImageLoader
                        .LoadSprite(url)
                        .Loaded(arg =>
                        {
                            if (background != null)
                            {
                                background.SetActive(false);
                            }
                        });

                    if (image != null)
                    {
                        image.sprite = sprite;
                    }
                }
                catch
                {
                    try
                    {
                        string fallbackUrl = url.Replace("staging/", "");
                        Sprite sprite = await ImageLoader
                            .LoadSprite(fallbackUrl)
                            .Loaded(arg =>
                            {
                                if (background != null)
                                {
                                    background.SetActive(false);
                                }
                            });

                        if (image != null)
                        {
                            image.sprite = sprite;
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogWarning($"Failed to load fallback image from {url}: {e.Message}");
                    }
                }
            }

            public static async UniTask RawImage(
                RawImage rawImage,
                GameObject background,
                string url
            )
            {
                if (string.IsNullOrWhiteSpace(url))
                {
                    DebugLog.Warning($"URL is not valid {rawImage?.name}");
                    return;
                }

                try
                {
                    Texture2D texture = await ImageLoader
                        .LoadTexture(url)
                        .Loaded(arg =>
                        {
                            if (background != null)
                            {
                                background.SetActive(false);
                            }
                        });

                    if (rawImage != null)
                    {
                        rawImage.texture = texture;
                    }
                }
                catch
                {
                    try
                    {
                        string fallbackUrl = url.Replace("staging/", "");
                        Texture2D texture = await ImageLoader
                            .LoadTexture(fallbackUrl, ignoreImageNotFoundError: true)
                            .Loaded(arg =>
                            {
                                if (background != null)
                                {
                                    background.SetActive(false);
                                }
                            });

                        if (rawImage != null)
                        {
                            rawImage.texture = texture;
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogWarning(
                            $"Failed to load fallback texture from {url}: {e.Message}"
                        );
                    }
                }
            }

            #endregion

            #region Process Methods

            private static async UniTask<object> ProcessLoading<T>(
                string url,
                CancellationToken cancellationToken = default,
                Action<float> progress = null,
                int timeout = 20
            )
            {
                UnityWebRequest request;

                if (typeof(T) == typeof(Texture2D))
                {
                    request = UnityWebRequestTexture.GetTexture(url);
                }
                else if (typeof(T) == typeof(AssetBundle))
                {
                    request = UnityWebRequestAssetBundle.GetAssetBundle(url);
                }
                else
                {
                    request = UnityWebRequest.Get(url);
                }

                try
                {
                    request.timeout = timeout;

                    Progress<float> requestProgress =
                        progress != null ? new Progress<float>(progress) : null;

                    await request
                        .SendWebRequest()
                        .ToUniTask(cancellationToken: cancellationToken, progress: requestProgress);

                    return ProcessResult<T>(request);
                }
                catch (OperationCanceledException)
                {
                    DebugLog.Warning($"Download {typeof(T).Name} canceled: {url}");
                    return default;
                }
                catch (Exception exception)
                {
                    DebugLog.Exception(exception);
                    DebugLog.Error(exception.Source);
                    DebugLog.Error(exception.StackTrace);
                    return default;
                }
                finally
                {
                    DebugLog.Info($"Loading {typeof(T).Name} completed!");
                }
            }

            private static object ProcessResult<T>(UnityWebRequest request)
            {
                if (request.result != UnityWebRequest.Result.Success)
                {
                    DebugLog.Error(request.error);
                    return default;
                }

                if (typeof(T) == typeof(string))
                {
                    return request.downloadHandler.text;
                }
                else if (typeof(T) == typeof(Texture2D))
                {
                    return DownloadHandlerTexture.GetContent(request);
                }
                else if (typeof(T) == typeof(AssetBundle))
                {
                    return DownloadHandlerTexture.GetContent(request);
                }
                else if (typeof(T) == typeof(byte[]))
                {
                    return request.downloadHandler.data;
                }
                else
                {
                    DebugLog.Error($"Unsupported data type {typeof(T).Name}");
                    return default;
                }
            }

            #endregion
        }
    }

    #endregion
}
#endif
