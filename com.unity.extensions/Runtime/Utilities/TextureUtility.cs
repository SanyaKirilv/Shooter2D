#if UnityExtensions
using System;
using UnityEngine;

namespace UnityExtensions
{
    public enum ConversionType
    {
        RGB,
        Grayscale,
    }

    public static class TextureUtility
    {
        public static byte[] TextureToFile(Texture2D texture, bool asPng = true)
        {
            if (texture == null)
            {
                DebugLog.Error("Provided texture is null.");
                return null;
            }

            if (
                !texture.isReadable
                || (
                    texture.format != TextureFormat.RGBA32
                    && texture.format != TextureFormat.ARGB32
                    && texture.format != TextureFormat.RGB24
                )
            )
            {
                texture = ConvertToReadable(texture);
            }

            byte[] data = asPng ? texture.EncodeToPNG() : texture.EncodeToJPG();
            DebugLog.Info($"Texture successfully encoded to {(asPng ? "PNG" : "JPEG")} format.");
            return data;
        }

        public static Texture2D TextureFromFile(byte[] byteData)
        {
            if (byteData?.Length == 0)
            {
                DebugLog.Error("Provided texture data is null or empty.");
                return null;
            }

            Texture2D texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            return texture.LoadImage(byteData) ? ConvertToReadable(texture) : null;
        }

        private static Texture2D ConvertToReadable(Texture2D texture)
        {
            if (
                texture.isReadable
                && (
                    texture.format == TextureFormat.RGBA32
                    || texture.format == TextureFormat.ARGB32
                    || texture.format == TextureFormat.RGB24
                )
            )
            {
                return texture;
            }

            RenderTexture rt = null;
            try
            {
                rt = RenderTexture.GetTemporary(
                    texture.width,
                    texture.height,
                    0,
                    RenderTextureFormat.Default,
                    RenderTextureReadWrite.Linear
                );

                Graphics.Blit(texture, rt);
                RenderTexture previous = RenderTexture.active;
                RenderTexture.active = rt;

                Texture2D readable = new Texture2D(
                    texture.width,
                    texture.height,
                    TextureFormat.RGBA32,
                    false
                );
                readable.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
                readable.Apply();

                RenderTexture.active = previous;
                return readable;
            }
            catch (Exception e)
            {
                DebugLog.Error($"Failed to convert texture - {e.Message}");
                return texture;
            }
            finally
            {
                if (rt != null)
                {
                    RenderTexture.ReleaseTemporary(rt);
                }
                RenderTexture.active = null;
            }
        }

        public static class Conversions
        {
            #region Transform

            private static readonly string TransformShaderName = "TextureTransform";
            private static Shader transformShader;
            private static Material transformMaterial;

            public static void Transform(
                Texture source,
                RenderTexture destination,
                float angle = 0,
                bool flipHorizontal = false,
                bool flipVertical = false
            )
            {
                ValidateInput(source, destination);

                Matrix4x4 rotationMatrix = CreateRotationMatrix(angle);

                if (transformShader == null)
                {
                    transformShader = Shader.Find($"Custom/{TransformShaderName}");
                }
                if (transformMaterial == null)
                {
                    transformMaterial = new Material(transformShader);
                }

                transformMaterial.SetMatrix("_RotationMatrix", rotationMatrix);
                transformMaterial.SetFloat("_FlipHorizontal", flipHorizontal ? 1.0f : 0.0f);
                transformMaterial.SetFloat("_FlipVertical", flipVertical ? 1.0f : 0.0f);

                Graphics.Blit(source, destination, transformMaterial);
            }

            #endregion

            #region Crop

            private static readonly string CropShaderName = "CropResize";
            private static Shader cropShader;
            private static Material cropMaterial;

            public static void CropResize(
                RenderTexture source,
                RenderTexture destination,
                Rect cropRect,
                int targetWidth,
                int targetHeight
            )
            {
                ValidateInput(source, destination);
                ValidateDimensions(targetWidth, targetHeight);

                Vector4 cropParams = CalculateCropParams(source, cropRect);

                if (cropShader == null)
                {
                    cropShader = Shader.Find($"Custom/{CropShaderName}");
                }
                if (cropMaterial == null)
                {
                    cropMaterial = new Material(cropShader);
                }

                cropMaterial.SetVector("_CropRect", cropParams);
                Graphics.Blit(source, destination, cropMaterial);
            }

            #endregion

            #region Resize

            public static Texture2D Resize(Texture2D source, int targetWidth, int targetHeight)
            {
                RenderTexture tempRT = null;
                try
                {
                    ValidateInput(source);
                    ValidateDimensions(targetWidth, targetHeight);

                    if (source.width == targetWidth && source.height == targetHeight)
                    {
                        return source;
                    }

                    tempRT = RenderTexture.GetTemporary(
                        targetWidth,
                        targetHeight,
                        0,
                        RenderTextureFormat.Default,
                        RenderTextureReadWrite.Default
                    );
                    RenderTexture.active = tempRT;

                    Graphics.Blit(source, tempRT);

                    Texture2D resized = new Texture2D(
                        targetWidth,
                        targetHeight,
                        TextureFormat.ARGB32,
                        false
                    );
                    resized.name = source.name;
                    resized.ReadPixels(new Rect(0, 0, targetWidth, targetHeight), 0, 0);
                    resized.Apply();

                    return resized;
                }
                catch (Exception ex)
                {
                    DebugLog.Warning(
                        $"Error occurred during resizing!\n{ex.Message}\n{ex.StackTrace}"
                    );
                    return null;
                }
                finally
                {
                    if (tempRT != null)
                    {
                        RenderTexture.ReleaseTemporary(tempRT);
                    }
                    RenderTexture.active = null;
                }
            }

            #endregion

            #region Extracting

            public static float[] ExtractData(Texture2D texture, ConversionType type)
            {
                ValidateInput(texture);

                return type switch
                {
                    ConversionType.Grayscale => ExtractGrayscale(texture),
                    ConversionType.RGB => ExtractRGB(texture),
                    _ => throw new NotSupportedException($"Unsupported conversion type: {type}"),
                };
            }

            private static float[] ExtractGrayscale(Texture2D texture)
            {
                Color32[] pixels = texture.GetPixels32();
                float[] grayscale = new float[pixels.Length];

                for (int i = 0; i < pixels.Length; i++)
                {
                    Color32 pixel = pixels[i];
                    grayscale[i] =
                        (0.299f * pixel.r + 0.587f * pixel.g + 0.114f * pixel.b) / 255.0f;
                }

                DebugLog.Info($"Extracted grayscale data from {texture.name}.");
                return grayscale;
            }

            private static float[] ExtractRGB(Texture2D texture)
            {
                Color32[] pixels = texture.GetPixels32();
                float[] rgb = new float[pixels.Length * 3];
                const float scale = 1.0f / 255.0f;

                for (int i = 0; i < pixels.Length; i++)
                {
                    Color32 pixel = pixels[i];
                    rgb[i * 3] = pixel.r * scale;
                    rgb[i * 3 + 1] = pixel.g * scale;
                    rgb[i * 3 + 2] = pixel.b * scale;
                }

                DebugLog.Info($"Extracted RGB data from {texture.name}.");
                return rgb;
            }

            #endregion

            #region Validation

            private static void ValidateName(string fileName)
            {
                if (string.IsNullOrWhiteSpace(fileName))
                {
                    throw new ArgumentException(
                        "File name must not be null or whitespace.",
                        nameof(fileName)
                    );
                }
            }

            private static void ValidateData(object data, string paramName)
            {
                if (
                    data == null
                    || (data is string str && string.IsNullOrWhiteSpace(str))
                    || (data is byte[] bytes && bytes.Length == 0)
                )
                {
                    throw new ArgumentException(
                        $"{paramName} cannot be null, empty, or whitespace.",
                        paramName
                    );
                }
            }

            private static void ValidateInput(object input)
            {
                if (input == null)
                {
                    throw new ArgumentNullException(nameof(input), "Input cannot be null.");
                }
            }

            private static void ValidateInput(object source, object destination)
            {
                ValidateInput(source);
                ValidateInput(destination);
            }

            private static void ValidateDimensions(int width, int height)
            {
                if (width <= 0)
                {
                    throw new ArgumentException("Width must be greater than zero.", nameof(width));
                }

                if (height <= 0)
                {
                    throw new ArgumentException(
                        "Height must be greater than zero.",
                        nameof(height)
                    );
                }
            }

            #endregion

            #region Helpful Methods

            public static Texture2D ConvertToTexture2D(RenderTexture src)
            {
                ValidateInput(src);

                Texture2D converted = new(src.width, src.height, TextureFormat.RGB24, false);
                RenderTexture currentRT = RenderTexture.active;
                try
                {
                    RenderTexture.active = src;
                    converted.ReadPixels(new Rect(0, 0, src.width, src.height), 0, 0);
                    converted.Apply();
                }
                finally
                {
                    RenderTexture.active = currentRT;
                }

                return converted;
            }

            private static Matrix4x4 CreateRotationMatrix(float angle)
            {
                float rad = angle * Mathf.Deg2Rad;
                float cos = Mathf.Cos(rad);
                float sin = Mathf.Sin(rad);

                return new Matrix4x4
                {
                    m00 = cos,
                    m01 = sin,
                    m03 = 0,
                    m10 = -sin,
                    m11 = cos,
                    m13 = 0,
                    m22 = 1,
                    m33 = 1,
                };
            }

            private static Vector4 CalculateCropParams(RenderTexture src, Rect cropRect)
            {
                float xMin = cropRect.xMin / src.width;
                float yMin = cropRect.yMin / src.height;
                float xMax = cropRect.xMax / src.width;
                float yMax = cropRect.yMax / src.height;

                return new Vector4(
                    Mathf.Clamp(xMin, 0, 1),
                    Mathf.Clamp(yMin, 0, 1),
                    Mathf.Clamp(xMax, 0, 1),
                    Mathf.Clamp(yMax, 0, 1)
                );
            }

            #endregion
        }
    }
}
#endif
