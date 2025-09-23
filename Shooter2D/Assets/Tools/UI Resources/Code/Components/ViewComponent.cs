#if UnityExtensions
using UnityEngine;

namespace UnityExtensions.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class ViewComponent : RectComponent
    {
        #region Fields

        public bool InitOnStart;

        private CanvasGroup _canvasGroup;

        #endregion

        #region Properties

        /// <summary>
        /// Direct access to the CanvasGroup component
        /// </summary>
        public CanvasGroup CanvasGroup => _canvasGroup;

        /// <summary>
        /// Gets or sets the view visibility state
        /// </summary>
        /// <remarks>
        /// Setting this value toggles alpha, interactability and raycast blocking
        /// </remarks>
        public bool IsVisible
        {
            get => CanvasGroup.interactable;
            set => ToggleView(value);
        }

        #endregion

        #region Unity Methods

        /// <summary>
        /// Initializes the CanvasGroup reference and sets initial visibility
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            InitializeCanvasGroup();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Shows the view by enabling CanvasGroup properties
        /// </summary>
        public virtual void Show()
        {
            ToggleView(true);
        }

        /// <summary>
        /// Hides the view by disabling CanvasGroup properties
        /// </summary>
        public virtual void Hide()
        {
            ToggleView(false);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Initializes the CanvasGroup component
        /// </summary>
        private void InitializeCanvasGroup()
        {
            if (_canvasGroup == null && !TryGetComponent(out _canvasGroup))
            {
                Debug.LogError($"No CanvasGroup component attached to {name}!");
                return;
            }

            ToggleView(InitOnStart);
        }

        /// <summary>
        /// Toggles view visibility state
        /// </summary>
        /// <param name="state">True to show, false to hide</param>
        private void ToggleView(bool state)
        {
            if (CanvasGroup == null)
            {
                Debug.LogWarning("CanvasGroup is null. Object may have been destroyed.");
                return;
            }

            CanvasGroup.alpha = state ? 1 : 0;
            CanvasGroup.interactable = state;
            CanvasGroup.blocksRaycasts = state;
        }

        #endregion
    }

    /// <summary>
    /// Generic version of ViewComponent that inherits from RectComponent<T>
    /// </summary>
    /// <typeparam name="T">The derived component type</typeparam>
    [RequireComponent(typeof(CanvasGroup))]
    public class ViewComponent<T> : RectComponent<T>
        where T : ViewComponent<T>
    {
        #region Fields

        public bool InitOnStart; // Should initialize visibility on start

        private CanvasGroup _canvasGroup; // Reference to CanvasGroup component
        #endregion

        #region Properties

        /// <summary>
        /// Direct access to the CanvasGroup component
        /// </summary>
        public CanvasGroup CanvasGroup => _canvasGroup;

        /// <summary>
        /// Gets or sets the view visibility state
        /// </summary>
        /// <remarks>
        /// Setting this value toggles alpha, interactability and raycast blocking
        /// </remarks>
        public bool IsVisible
        {
            get => CanvasGroup.interactable;
            set => ToggleView(value);
        }

        #endregion

        #region Unity Methods

        /// <summary>
        /// Initializes the CanvasGroup reference and sets initial visibility
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            InitializeCanvasGroup();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Shows the view by enabling CanvasGroup properties
        /// </summary>
        public virtual void Show()
        {
            ToggleView(true);
        }

        /// <summary>
        /// Hides the view by disabling CanvasGroup properties
        /// </summary>
        public virtual void Hide()
        {
            ToggleView(false);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Initializes the CanvasGroup component
        /// </summary>
        private void InitializeCanvasGroup()
        {
            if (_canvasGroup == null && !TryGetComponent(out _canvasGroup))
            {
                Debug.LogError($"No CanvasGroup component attached to {name}!");
                return;
            }

            ToggleView(InitOnStart);
        }

        /// <summary>
        /// Toggles view visibility state
        /// </summary>
        /// <param name="state">True to show, false to hide</param>
        private void ToggleView(bool state)
        {
            if (CanvasGroup == null)
            {
                Debug.LogWarning("CanvasGroup is null. Object may have been destroyed.");
                return;
            }

            CanvasGroup.alpha = state ? 1 : 0;
            CanvasGroup.interactable = state;
            CanvasGroup.blocksRaycasts = state;
        }

        #endregion
    }
}
#endif
