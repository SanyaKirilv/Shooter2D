#if UnityExtensions
using System.Collections.Generic;
using UnityEngine;

namespace UnityExtensions.UI
{
    public static class Colors
    {
        private const float GoldenRatio = 0.61803398875f;

        public static readonly Dictionary<string, Color> ColorMap = new Dictionary<string, Color>()
        {
            { "None", Color.clear },
            { "Custom", Color.clear },
            { "Black", Color.black },
            { "White", Color.white },
        };

        public static Color GetColor(string colorName, float alpha = 1)
        {
            if (!ColorMap.TryGetValue(colorName, out Color color))
            {
                Debug.LogError($"Color name '{colorName}' not found in the color list.");
                return Color.clear;
            }
            return new Color(color.r, color.g, color.b, alpha);
        }

        public static Color GetUniqueColor(
            int index,
            float saturation = 0.8f,
            float brightness = 0.8f
        )
        {
            float hue = index * GoldenRatio % 1f;
            return Color.HSVToRGB(hue, saturation, brightness);
        }
    }
}
#endif
