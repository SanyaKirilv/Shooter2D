#if UnityExtensions
using TMPro;
using UnityEngine;

namespace UnityExtensions.UI
{
    [RequireComponent(typeof(CanvasRenderer), typeof(TextMeshProUGUI))]
    public class TextElement : GraphicComponent
    {
        #region Fields
        private TextMeshProUGUI _text;

        [SerializeField, TextArea(1, 5)]
        protected string _defaultText = "Sample Text";

        public TextMeshProUGUI Text
        {
            get => _text;
        }

        #endregion

        #region Unity Methods

        protected override void OnEnable()
        {
            base.OnEnable();
            InitializeTextComponent();
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            InitializeTextComponent();
            base.OnValidate();
            SetText();
            Text.color = UnityEngine.Color.black;
        }
#endif

        #endregion

        #region Public Methods

        public override void SetColor(ColorComponent newColor = null)
        {
            Text.text = $"<color=#{(string)(newColor ?? Color)}>{_defaultText}</color>";
        }

        public void SetText(string newText = null)
        {
            Text.text = $"<color=#{(string)Color}>{newText ?? _defaultText}</color>";
        }

        public void SetTextForce(string newText)
        {
            _defaultText = newText;
            SetText();
        }

        #endregion

        #region Private Methods

        private void InitializeTextComponent()
        {
            if (_text == null && !TryGetComponent(out _text))
            {
                Debug.LogError($"No TextMeshProUGUI component attached to {name}!");
            }
        }

        #endregion
    }
}
#endif
