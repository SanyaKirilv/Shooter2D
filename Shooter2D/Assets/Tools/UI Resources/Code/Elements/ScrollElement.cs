#if UnityExtensions
using System;
using UnityEngine;
using UnityEngine.UI;

namespace UnityExtensions.UI
{
    /// <summary>
    /// A UI component that extends ScrollRect functionality with additional utility methods
    /// and event handling for scroll position changes.
    /// </summary>
    [RequireComponent(typeof(ScrollRect))]
    public class ScrollElement : RectComponent
    {
        #region Fields

        private ScrollRect _scrollRect;

        /// <summary>
        /// Event triggered when the scroll position changes
        /// </summary>
        private event Action<Vector2> onValueChanged;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the ScrollRect component with lazy initialization
        /// </summary>
        public ScrollRect ScrollRect => _scrollRect ??= gameObject.GetComponent<ScrollRect>();

        /// <summary>
        /// Gets the content RectTransform of the scroll view
        /// </summary>
        public RectTransform Content => ScrollRect.content;

        /// <summary>
        /// Gets the viewport RectTransform of the scroll view
        /// </summary>
        public RectTransform Viewport => ScrollRect.viewport;

        /// <summary>
        /// Gets or sets the normalized scroll position (0-1 range)
        /// </summary>
        public Vector2 NormalizedPosition
        {
            get => ScrollRect.normalizedPosition;
            set => ScrollRect.normalizedPosition = value;
        }

        #endregion

        #region Unity Methods

        /// <summary>
        /// Initializes the scroll rect event listener when enabled
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            ScrollRect.onValueChanged.AddListener(OnChanged);
        }

        /// <summary>
        /// Removes the scroll rect event listener when disabled
        /// </summary>
        protected void OnDisable()
        {
            ScrollRect.onValueChanged.RemoveListener(OnChanged);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Resets the scroll position to the top and resets content positioning
        /// </summary>
        public void ResetPosition()
        {
            NormalizedPosition = new Vector2(0, 1);
            ResetContent();
        }

        /// <summary>
        /// Resets the content positioning within the scroll view
        /// </summary>
        public void ResetContent()
        {
            SafeArea.ResetContent(Content);
        }

        /// <summary>
        /// Moves the content on value from bottom
        /// </summary>
        public void MovesContent(float value)
        {
            Content.anchoredPosition = new Vector2(Content.anchoredPosition.x, value);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Handles scroll position changes and invokes the onValueChanged event
        /// </summary>
        /// <param name="value">The new normalized scroll position</param>
        protected virtual void OnChanged(Vector2 value)
        {
            onValueChanged?.Invoke(value);
        }

        #endregion
    }
}
#endif
