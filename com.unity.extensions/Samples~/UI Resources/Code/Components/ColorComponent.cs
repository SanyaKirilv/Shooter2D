#if UnityExtensions
using System;
using UnityEngine;

namespace UnityExtensions.UI
{
    [Serializable]
    public class ColorComponent
    {
        [SerializeField]
        private Color _color = Color.clear;

        [SerializeField]
        private string _name = "None";

        [SerializeField, Range(0, 1f)]
        private float _alpha = 0;

        public Color Color
        {
            get => new(_color.r, _color.g, _color.b, _alpha);
            set
            {
                _color = value;
                _alpha = value.a;
            }
        }

        public float Alpha
        {
            get => _alpha;
            set => _alpha = value;
        }

        public ColorComponent(Color color, float alpha)
        {
            _alpha = alpha;
            Color = color;
        }

        public ColorComponent(Color color)
            : this(color, color.a) { }

        public ColorComponent()
            : this(Color.clear, 0) { }

        public static readonly ColorComponent Clear = new ColorComponent();

        public static implicit operator Color(ColorComponent colorContainer)
        {
            return colorContainer.Color;
        }

        public static implicit operator Color32(ColorComponent colorContainer)
        {
            return colorContainer.Color;
        }

        public static implicit operator string(ColorComponent colorContainer)
        {
            return ColorUtility.ToHtmlStringRGBA(colorContainer.Color);
        }

        public static implicit operator ColorComponent(Color color)
        {
            return new ColorComponent(color);
        }

        public string ToString(ref ColorComponent colorContainer)
        {
            return $"{colorContainer} ({_name})";
        }

        public static Color GetColor(string name, float alpha)
        {
            return Colors.GetColor(name, alpha);
        }
    }
}
#endif
