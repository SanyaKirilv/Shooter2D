#if UnityExtensions
using System;
using Cysharp.Threading.Tasks;
using Nobi.UiRoundedCorners;
using UnityEngine;

namespace UnityExtensions.UI
{
    public enum EasingType
    {
        Linear,
        EaseInQuad,
        EaseOutQuad,
        EaseInOutQuad,
    }

    /// <summary>
    /// Base component for UI elements that provides common RectTransform operations
    /// with animation capabilities.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class RectComponent : MonoBehaviour
    {
        #region Fields

        private RectTransform _rectTransform;

        #endregion

        #region Unity Methods

#if UNITY_EDITOR

        /// <summary>
        /// Cache the RectTransform reference when validate
        /// </summary>
        protected virtual void OnValidate()
        {
            if (_rectTransform == null)
            {
                _rectTransform = transform as RectTransform;
            }
        }
#endif

        /// <summary>
        /// Cache the RectTransform reference when enabled
        /// </summary>
        protected virtual void OnEnable()
        {
            if (_rectTransform == null)
            {
                _rectTransform = transform as RectTransform;
            }
        }

        protected void RectTransformDimensionsChange()
        {
            if (TryGetComponent(out ImageWithRoundedCorners corners))
            {
                corners.Validate();
                corners.Refresh();
            }

            if (TryGetComponent(out ImageWithIndependentRoundedCorners cornersIndepended))
            {
                cornersIndepended.Validate();
                cornersIndepended.Refresh();
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Direct access to the RectTransform component
        /// </summary>
        public RectTransform RectTransform
        {
            get
            {
                if (_rectTransform == null)
                {
                    _rectTransform = transform as RectTransform;
                }

                return _rectTransform;
            }
        }

        /// <summary>
        /// Gets or sets the size delta of the RectTransform
        /// </summary>
        public Vector2 SizeDelta
        {
            get => RectTransform.sizeDelta;
            set => RectTransform.sizeDelta = value;
        }

        /// <summary>
        /// Gets or sets the world space position
        /// </summary>
        public Vector2 Position
        {
            get => RectTransform.position;
            set => RectTransform.position = value;
        }

        /// <summary>
        /// Gets or sets the anchored position
        /// </summary>
        public Vector2 AnchoredPosition
        {
            get => RectTransform.anchoredPosition;
            set => RectTransform.anchoredPosition = value;
        }

        /// <summary>
        /// Gets or sets the offset from the lower left corner
        /// </summary>
        public Vector2 OffsetMin
        {
            get => RectTransform.offsetMin;
            set => RectTransform.offsetMin = value;
        }

        /// <summary>
        /// Gets or sets the offset from the upper right corner
        /// </summary>
        public Vector2 OffsetMax
        {
            get => RectTransform.offsetMax;
            set => RectTransform.offsetMax = value;
        }

        /// <summary>
        /// Gets or sets the game object's active state
        /// </summary>
        public bool IsActive
        {
            get => gameObject.activeInHierarchy;
            set => gameObject.SetActive(value);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Smoothly moves the RectTransform to target position
        /// </summary>
        /// <param name="target">Target position</param>
        /// <param name="isAnchored">Use anchored position if true, world position if false</param>
        /// <param name="duration">Animation duration in seconds</param>
        /// <param name="easingType">Easing function to use</param>
        public async UniTask MoveLerp(
            Vector2 target,
            bool isAnchored = true,
            float duration = 0.25f,
            EasingType easingType = EasingType.Linear
        )
        {
            await LerpAnimation(
                isAnchored ? AnchoredPosition : Position,
                target,
                duration,
                value =>
                {
                    if (isAnchored)
                    {
                        AnchoredPosition = value;
                    }
                    else
                    {
                        Position = value;
                    }
                },
                easingType
            );
        }

        /// <summary>
        /// Smoothly resizes the RectTransform to target size
        /// </summary>
        /// <param name="target">Target size delta</param>
        /// <param name="duration">Animation duration in seconds</param>
        /// <param name="easingType">Easing function to use</param>
        public async UniTask SizeLerp(
            Vector2 target,
            float duration = 0.25f,
            EasingType easingType = EasingType.Linear
        )
        {
            await LerpAnimation(
                SizeDelta,
                target,
                duration,
                value => SizeDelta = value,
                easingType
            );
        }

        public void ResetObject()
        {
            SafeArea.ResetContent(RectTransform);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Core animation coroutine that handles the interpolation
        /// </summary>
        private async UniTask LerpAnimation(
            Vector2 start,
            Vector2 target,
            float duration,
            Action<Vector2> setValue,
            EasingType easingType = EasingType.Linear
        )
        {
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / duration);
                t = ApplyEasing(t, easingType);
                setValue(Vector2.LerpUnclamped(start, target, t));
                await UniTask.NextFrame();
            }

            setValue(target);
        }

        /// <summary>
        /// Applies easing functions to interpolation value
        /// </summary>
        private float ApplyEasing(float t, EasingType easingType)
        {
            return easingType switch
            {
                EasingType.Linear => t,
                EasingType.EaseInQuad => t * t,
                EasingType.EaseOutQuad => t * (2 - t),
                EasingType.EaseInOutQuad => t < 0.5f ? 2 * t * t : -1 + (4 - 2 * t) * t,
                _ => t,
            };
        }

        #endregion
    }

    /// <summary>
    /// Generic version of RectComponent that inherits from Reference<T>
    /// </summary>
    /// <typeparam name="T">The derived component type</typeparam>
    [RequireComponent(typeof(RectTransform))]
    public class RectComponent<T> : ReferenceBehaviour<T>
        where T : RectComponent<T>
    {
        #region Fields

        private RectTransform _rectTransform;

        #endregion

        #region Unity Methods

#if UNITY_EDITOR

        /// <summary>
        /// Cache the RectTransform reference when validate
        /// </summary>
        protected virtual void OnValidate()
        {
            if (_rectTransform == null)
            {
                _rectTransform = transform as RectTransform;
            }
        }
#endif

        /// <summary>
        /// Cache the RectTransform reference when enabled
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            if (_rectTransform == null)
            {
                _rectTransform = transform as RectTransform;
            }
        }

        protected void RectTransformDimensionsChange()
        {
            if (gameObject.TryGetComponent(out ImageWithRoundedCorners corners))
            {
                corners.Validate();
                corners.Refresh();
            }

            if (
                gameObject.TryGetComponent(out ImageWithIndependentRoundedCorners cornersIndepended)
            )
            {
                cornersIndepended.Validate();
                cornersIndepended.Refresh();
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Direct access to the RectTransform component
        /// </summary>
        public RectTransform RectTransform
        {
            get
            {
                if (_rectTransform == null)
                {
                    _rectTransform = transform as RectTransform;
                }

                return _rectTransform;
            }
        }

        /// <summary>
        /// Gets or sets the size delta of the RectTransform
        /// </summary>
        public Vector2 SizeDelta
        {
            get => RectTransform.sizeDelta;
            set => RectTransform.sizeDelta = value;
        }

        /// <summary>
        /// Gets or sets the world space position
        /// </summary>
        public Vector2 Position
        {
            get => RectTransform.position;
            set => RectTransform.position = value;
        }

        /// <summary>
        /// Gets or sets the anchored position
        /// </summary>
        public Vector2 AnchoredPosition
        {
            get => RectTransform.anchoredPosition;
            set => RectTransform.anchoredPosition = value;
        }

        /// <summary>
        /// Gets or sets the offset from the lower left corner
        /// </summary>
        public Vector2 OffsetMin
        {
            get => RectTransform.offsetMin;
            set => RectTransform.offsetMin = value;
        }

        /// <summary>
        /// Gets or sets the offset from the upper right corner
        /// </summary>
        public Vector2 OffsetMax
        {
            get => RectTransform.offsetMax;
            set => RectTransform.offsetMax = value;
        }

        /// <summary>
        /// Gets or sets the game object's active state
        /// </summary>
        public bool IsActive
        {
            get => gameObject.activeInHierarchy;
            set => gameObject.SetActive(value);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Smoothly moves the RectTransform to target position
        /// </summary>
        /// <param name="target">Target position</param>
        /// <param name="isAnchored">Use anchored position if true, world position if false</param>
        /// <param name="duration">Animation duration in seconds</param>
        /// <param name="easingType">Easing function to use</param>
        public async UniTask MoveLerp(
            Vector2 target,
            bool isAnchored = true,
            float duration = 0.25f,
            EasingType easingType = EasingType.Linear
        )
        {
            await LerpAnimation(
                isAnchored ? AnchoredPosition : Position,
                target,
                duration,
                value =>
                {
                    if (isAnchored)
                    {
                        AnchoredPosition = value;
                    }
                    else
                    {
                        Position = value;
                    }
                },
                easingType
            );
        }

        /// <summary>
        /// Smoothly resizes the RectTransform to target size
        /// </summary>
        /// <param name="target">Target size delta</param>
        /// <param name="duration">Animation duration in seconds</param>
        /// <param name="easingType">Easing function to use</param>
        public async UniTask SizeLerp(
            Vector2 target,
            float duration = 0.25f,
            EasingType easingType = EasingType.Linear
        )
        {
            await LerpAnimation(
                SizeDelta,
                target,
                duration,
                value => SizeDelta = value,
                easingType
            );
        }

        public void ResetObject()
        {
            SafeArea.ResetContent(RectTransform);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Core animation coroutine that handles the interpolation
        /// </summary>
        private async UniTask LerpAnimation(
            Vector2 start,
            Vector2 target,
            float duration,
            Action<Vector2> setValue,
            EasingType easingType = EasingType.Linear
        )
        {
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / duration);
                t = ApplyEasing(t, easingType);
                setValue(Vector2.LerpUnclamped(start, target, t));
                await UniTask.NextFrame();
            }

            setValue(target);
        }

        /// <summary>
        /// Applies easing functions to interpolation value
        /// </summary>
        private float ApplyEasing(float t, EasingType easingType)
        {
            return easingType switch
            {
                EasingType.Linear => t,
                EasingType.EaseInQuad => t * t,
                EasingType.EaseOutQuad => t * (2 - t),
                EasingType.EaseInOutQuad => t < 0.5f ? 2 * t * t : -1 + (4 - 2 * t) * t,
                _ => t,
            };
        }

        #endregion
    }
}
#endif
