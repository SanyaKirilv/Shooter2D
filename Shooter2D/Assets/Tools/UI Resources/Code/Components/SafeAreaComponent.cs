#if UnityExtensions
using UnityEngine;

namespace UnityExtensions.UI
{
    /// <summary>
    /// Component that automatically adjusts UI elements to respect device safe areas
    /// (notches, rounded corners, etc.) by applying padding to the RectTransform.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class SafeAreaComponent : RectComponent
    {
        [SerializeField, Tooltip("Whether to apply safe area padding")]
        private bool _include = true;

        /// <summary>
        /// Initializes safe area padding when enabled
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            ApplySafeAreaOffset(true);
        }

        /// <summary>
        /// Removes safe area padding when disabled
        /// </summary>
        protected void OnDisable()
        {
            ApplySafeAreaOffset(false);
        }

        /// <summary>
        /// Applies or removes safe area offset based on device specifications
        /// </summary>
        /// <param name="state">True to apply safe area, false to reset</param>
        private void ApplySafeAreaOffset(bool state)
        {
            if (!_include)
            {
                return;
            }

            int bottomOffset = state ? SafeArea.BottomOffset : 0;
            var topOffset = state ? -SafeArea.TopOffset : 0;

            OffsetMin = new Vector2(0, bottomOffset);
            OffsetMax = new Vector2(0, topOffset);
        }
    }
}
#endif
