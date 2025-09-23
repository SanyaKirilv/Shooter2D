#if UnityExtensions
using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Nobi.UiRoundedCorners;
using UnityEngine;
using UnityEngine.UI;

namespace UnityExtensions.UI
{
    [System.Serializable]
    public class LayoutData
    {
        public float Height = 0;
        public float Width = 0;
        public bool IsLast = false;

        public LayoutData(Vector2 size, bool isLast)
        {
            Height = size.y;
            Width = size.x;
            IsLast = isLast;
        }

        public LayoutData()
            : this(Vector2.zero, false) { }

        public LayoutData(float width, float height, bool isLast = false)
            : this(new Vector2(width, height), false) { }

        public Vector2 Size => new Vector2(Width, Height);

        public void Reset()
        {
            Width = Height = 0f;
        }
    }

    [System.Serializable]
    public class Padding
    {
        public Vector2 Min = Vector2.zero;
        public Vector2 Max = Vector2.zero;
    }

    public class LayoutComponent : MonoBehaviour
    {
        [SerializeField]
        private LayoutData _layoutData;

        [SerializeField]
        private List<LayoutData> _elementsLayoutData = new List<LayoutData>();

        [SerializeField]
        private bool _isVertical;

        [SerializeField]
        private bool _isHorizontal;

        [SerializeField]
        private float _offset;

        [SerializeField]
        private Padding _padding;

        [SerializeField]
        private Vector2 _minSize = Vector2.zero;

        private RectTransform _rectTransform;
        private ContentSizeFitter _sizeFitterCache;
        private LayoutElement _layoutElementCache;
        private LayoutComponent _layoutComponentCache;
        private ImageWithRoundedCorners _roundedCornersCache;
        private ImageWithIndependentRoundedCorners _independentCornersCache;

        private bool _isUpdating;
        private int _childCountCache;
        private RectTransform[] _childrenBuffer = new RectTransform[32];

        public RectTransform T
        {
            get =>
                _rectTransform =
                    _rectTransform != null ? _rectTransform : transform as RectTransform;
        }

        private void Awake()
        {
            ScheduleUpdate().Forget();
        }

        private void OnTransformChildrenChanged()
        {
            UpdateChildCache();
            ScheduleUpdate().Forget();
        }

        private void UpdateChildCache()
        {
            _childCountCache = transform.childCount;
            if (_childCountCache > _childrenBuffer.Length)
            {
                Array.Resize(
                    ref _childrenBuffer,
                    Math.Max(_childCountCache + 16, _childrenBuffer.Length * 2)
                );
            }

            for (int i = 0; i < _childCountCache; i++)
            {
                _childrenBuffer[i] = transform.GetChild(i) as RectTransform;
            }
        }

        private async UniTaskVoid ScheduleUpdate()
        {
            if (_isUpdating)
            {
                return;
            }

            _isUpdating = true;
            await UpdateLayout();
            _isUpdating = false;
        }

        [ContextMenu("Update")]
        public void UpdateLayoutOption()
        {
            UpdateLayout().Forget();
        }

        public async UniTask UpdateLayout(
            CancellationToken cancellationToken = default,
            Action onComplete = null
        )
        {
            UpdateChildCache();
            _elementsLayoutData.Clear();
            _layoutData.Reset();

            try
            {
                if (_isVertical)
                {
                    await UpdateVertical(cancellationToken);
                }
                else if (_isHorizontal)
                {
                    await UpdateHorizontal(cancellationToken);
                }
                else
                {
                    await UpdateChilds(cancellationToken);
                }

                ApplyFinalSize();
                LayoutRebuilder.ForceRebuildLayoutImmediate(T);
            }
            finally
            {
                onComplete?.Invoke();
            }
        }

        private async UniTask UpdateVertical(CancellationToken ct)
        {
            _layoutData.Height = _padding.Max.y;

            for (int i = 0; i < _childCountCache; i++)
            {
                if (ct.IsCancellationRequested)
                {
                    break;
                }

                RectTransform child = _childrenBuffer[i];
                if (!child || !child.gameObject.activeInHierarchy)
                {
                    continue;
                }

                await ProcessChildLayout(child, true, IsLastActiveChild(i), ct);
            }

            _layoutData.Height += _padding.Min.y;
        }

        private async UniTask UpdateHorizontal(CancellationToken ct)
        {
            _layoutData.Width = _padding.Min.x;

            for (int i = 0; i < _childCountCache; i++)
            {
                if (ct.IsCancellationRequested)
                {
                    break;
                }

                RectTransform child = _childrenBuffer[i];
                if (!child || !child.gameObject.activeInHierarchy)
                {
                    continue;
                }

                await ProcessChildLayout(child, false, IsLastActiveChild(i), ct);
            }

            _layoutData.Width += _padding.Max.x;
        }

        private bool IsLastActiveChild(int index)
        {
            for (int i = index + 1; i < _childCountCache; i++)
            {
                if (_childrenBuffer[i] && _childrenBuffer[i].gameObject.activeInHierarchy)
                {
                    return false;
                }
            }
            return true;
        }

        private async UniTask ProcessChildLayout(
            RectTransform child,
            bool isVertical,
            bool isLast,
            CancellationToken ct
        )
        {
            _layoutComponentCache = child.GetComponent<LayoutComponent>();
            if (_layoutComponentCache != null)
            {
                await _layoutComponentCache.UpdateLayout(ct);
            }

            _layoutElementCache = child.GetComponent<LayoutElement>();
            if (_layoutElementCache != null && _layoutElementCache.ignoreLayout)
            {
                return;
            }

            _sizeFitterCache = child.GetComponent<ContentSizeFitter>();
            if (_sizeFitterCache != null)
            {
                if (isVertical)
                {
                    _sizeFitterCache.SetLayoutVertical();
                }
                else
                {
                    _sizeFitterCache.SetLayoutHorizontal();
                }
            }

            Vector2 childSize = child.sizeDelta;
            LayoutData childData = new LayoutData(childSize, isLast);
            _elementsLayoutData.Add(childData);

            if (isVertical)
            {
                child.anchoredPosition = new Vector2(child.anchoredPosition.x, -_layoutData.Height);
                _layoutData.Height += childData.Height + (isLast ? 0 : _offset);
            }
            else
            {
                child.anchoredPosition = new Vector2(_layoutData.Width, child.anchoredPosition.y);
                _layoutData.Width += childData.Width + (isLast ? 0 : _offset);
            }
        }

        private async UniTask UpdateChilds(CancellationToken ct)
        {
            for (int i = 0; i < _childCountCache; i++)
            {
                if (ct.IsCancellationRequested)
                {
                    break;
                }

                RectTransform child = _childrenBuffer[i];
                if (!child || !child.gameObject.activeInHierarchy)
                {
                    continue;
                }

                _layoutComponentCache = child.GetComponent<LayoutComponent>();
                if (_layoutComponentCache != null)
                {
                    await _layoutComponentCache.UpdateLayout(ct);
                }

                _roundedCornersCache = child.GetComponent<ImageWithRoundedCorners>();
                if (_roundedCornersCache != null)
                {
                    _roundedCornersCache.Refresh();
                }

                _independentCornersCache = child.GetComponent<ImageWithIndependentRoundedCorners>();
                if (_independentCornersCache != null)
                {
                    _independentCornersCache.Refresh();
                }
            }
        }

        private void ApplyFinalSize()
        {
            Vector2 targetSize = new Vector2(
                Mathf.Max(_layoutData.Width, _minSize.x),
                Mathf.Max(_layoutData.Height, _minSize.y)
            );

            if (_isVertical)
            {
                T.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, targetSize.y);
            }
            else if (_isHorizontal)
            {
                T.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, targetSize.x);
            }
        }
    }
}
#endif
