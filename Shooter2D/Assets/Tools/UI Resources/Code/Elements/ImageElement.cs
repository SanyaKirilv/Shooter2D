#if UnityExtensions
using UnityEngine;
using UnityEngine.UI;

namespace UnityExtensions.UI
{
    [RequireComponent(typeof(CanvasRenderer), typeof(Image))]
    public class ImageElement : GraphicComponent
    {
        #region Fields

        public bool Stretch = false;
        private Image _image;

        public Image Image
        {
            get => _image;
        }

        public Sprite Sprite
        {
            get => Image.sprite;
            set => Image.sprite = value;
        }

        #endregion

        #region Unity Methods

        protected override void OnEnable()
        {
            base.OnEnable();
            InitializeImageComponent();
            ApplySafeAreaAdjustments(true);
        }

        protected void OnDisable()
        {
            InitializeImageComponent();
            ApplySafeAreaAdjustments(false);
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            InitializeImageComponent();
        }
#endif

        #endregion

        #region Public Methods

        public void SetSprite(Sprite sprite, bool withColor = true)
        {
            if (Image == null)
            {
                return;
            }

            Image.sprite = sprite;
            if (!withColor)
            {
                Image.color = Color;
            }
        }

        public void SetStretch(bool shouldStretch)
        {
            Stretch = shouldStretch;
            ApplySafeAreaAdjustments(Stretch);
        }

        #endregion

        #region Private Methods

        private void InitializeImageComponent()
        {
            if (Image == null && !TryGetComponent(out _image))
            {
                Debug.LogError($"No Image component attached to {name}!");
            }
        }

        private void ApplySafeAreaAdjustments(bool shouldApply)
        {
            if (!Stretch || Image == null)
            {
                return;
            }

            OffsetMax = new Vector2(0, shouldApply ? SafeArea.TopOffset : 0);
            OffsetMin = new Vector2(0, shouldApply ? -SafeArea.BottomOffset : 0);
        }

        #endregion
    }
}
#endif
