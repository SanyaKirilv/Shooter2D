#if UnityExtensions
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UnityExtensions.UI
{
    public enum InputFieldType
    {
        Base,
        Name,
        Password,
    }

    [RequireComponent(typeof(CanvasRenderer), typeof(TMP_InputField))]
    public class InputElement : GraphicComponent
    {
        #region Fields
        public InputFieldType Type = InputFieldType.Base;
        public TextElement Label;

        private TMP_InputField _TMP_InputField;
        private TextElement _placeholder;

        [SerializeField]
        private ToggleImageButtonElement _button = null;

        [SerializeField]
        private bool _hide = true;

        public TMP_InputField InputField
        {
            get => _TMP_InputField;
        }

        public Graphic TargetGraphic => InputField.image;

        public TextElement Placeholder
        {
            get => _placeholder;
        }

        public ToggleImageButtonElement Button
        {
            get => _button;
        }

        public bool Hide
        {
            get => _hide;
            set
            {
                _hide = value;
                ChangeState();
            }
        }

        #endregion

        #region Unity Methods

        protected override void OnEnable()
        {
            base.OnEnable();
            InitializeInputComponent();
            InputField.onValueChanged.AddListener(HandleValueChanged);
            Button.OnClick.AddListener(HandleButtonClick);
        }

        protected void OnDisable()
        {
            InputField.onValueChanged.RemoveListener(HandleValueChanged);
            Button.OnClick.RemoveListener(HandleButtonClick);
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            InitializeInputComponent();

            if (!Application.isPlaying)
            {
                SetColor(Color);
            }
        }
#endif

        #endregion

        #region Public Methods

        public void SetWarning(bool state = true)
        {
            Color? color = state ? ColorComponent.GetColor("Red", 96) : null;

            Placeholder?.SetColor(color);
            Label?.SetColor(color);
        }

        public void Clear()
        {
            InputField.text = string.Empty;
            if (Button != null)
            {
                Button.IsActive = false;
            }

            HandleValueChanged();
        }

        #endregion

        #region Protected Methods

        protected virtual void ChangeState() { }

        protected virtual void HandleValueChanged(string args = null)
        {
            bool isEmpty = string.IsNullOrWhiteSpace(InputField.text);

            if (Type != InputFieldType.Base)
            {
                Button.IsActive = Type == InputFieldType.Password;

                if (!isEmpty)
                {
                    Label.SetText();
                }

                if (Label != null)
                {
                    Label.IsActive = !isEmpty;
                }

                AdjustViewport(isEmpty ? 0 : -9);
            }
            else
            {
                Button.IsActive = !isEmpty;
            }

            Placeholder.SetText();
        }

        protected virtual void HandleButtonClick()
        {
            if (Type == InputFieldType.Base)
            {
                Clear();
            }
            else if (Type == InputFieldType.Password)
            {
                TogglePasswordVisibility();
            }
        }

        #endregion

        #region Private Methods

        private void AdjustViewport(int yOffset)
        {
            InputField.textViewport.anchoredPosition = new Vector2(0, yOffset);
        }

        private void TogglePasswordVisibility()
        {
            Hide = !Hide;
            UpdateContentType();
        }

        private void UpdateContentType()
        {
            InputField.contentType = Hide
                ? TMP_InputField.ContentType.Password
                : TMP_InputField.ContentType.Standard;

            if (Button != null)
            {
                Button.State = !Hide;
            }

            InputField.ForceLabelUpdate();
        }

        private void InitializeInputComponent()
        {
            if (_TMP_InputField == null && !TryGetComponent(out _TMP_InputField))
            {
                Debug.LogError($"No TMP_InputField component attached to {name}!");
            }

            if (_placeholder == null && !InputField.placeholder.TryGetComponent(out _placeholder))
            {
                Debug.LogError($"No TextElement component attached to {name}!");
            }

            if (_button == null && !transform.GetChild(2).TryGetComponent(out _button))
            {
                Debug.LogError($"No ToggleImageButtonElement component attached to {name}!");
            }
        }

        #endregion
    }
}
#endif
