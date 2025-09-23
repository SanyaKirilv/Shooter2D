#if UnityExtensions
using Nobi.UiRoundedCorners;
using UnityEngine;

namespace UnityExtensions.UI
{
    public static class SafeArea
    {
        private static Vector2 _canvasScale = new Vector2(402, 874);
        private static Vector2 _anchorMin = Vector2.zero;
        private static Vector2 _anchorMax = Vector2.one;
        private static int _topSafeOffset = 0;
        private static int _bottomSafeOffset = 0;

        private static bool _calculated = false;
        private static int _lastScreenWidth = 0;
        private static int _lastScreenHeight = 0;

        public static Vector2 AnchorMin
        {
            get
            {
                EnsureCalculated();
                return _anchorMin;
            }
        }

        public static Vector2 AnchorMax
        {
            get
            {
                EnsureCalculated();
                return _anchorMax;
            }
        }

        public static int TopOffset
        {
            get
            {
                EnsureCalculated();
                return _topSafeOffset;
            }
        }

        public static int BottomOffset
        {
            get
            {
                EnsureCalculated();
                return _bottomSafeOffset;
            }
        }

        public static void Refresh()
        {
            _calculated = false;
            CalculateSafeArea();
        }

        public static async void ResetContent(Transform content)
        {
            if (content == null)
            {
                return;
            }

            if (content.TryGetComponent(out LayoutComponent layout))
            {
                await layout.UpdateLayout();
            }

            if (content.TryGetComponent(out ImageWithRoundedCorners corners))
            {
                corners.Refresh();
            }

            if (content.TryGetComponent(out ImageWithIndependentRoundedCorners cornersIndepended))
            {
                cornersIndepended?.Refresh();
            }
        }

        private static void EnsureCalculated()
        {
            if (!_calculated || ScreenSizeChanged())
            {
                CalculateSafeArea();
            }
        }

        private static bool ScreenSizeChanged()
        {
            return UnityEngine.Screen.width != _lastScreenWidth
                || UnityEngine.Screen.height != _lastScreenHeight;
        }

        private static void CalculateSafeArea()
        {
            Rect safeArea = UnityEngine.Screen.safeArea;
            int screenWidth = UnityEngine.Screen.width;
            int screenHeight = UnityEngine.Screen.height;

            _lastScreenWidth = screenWidth;
            _lastScreenHeight = screenHeight;

            if (screenWidth <= 0 || screenHeight <= 0)
            {
                DebugLog.Warning("Screen dimensions are invalid. Safe area calculation aborted.");
                return;
            }

            _anchorMin = new Vector2(safeArea.xMin / screenWidth, safeArea.yMin / screenHeight);
            _anchorMax = new Vector2(safeArea.xMax / screenWidth, safeArea.yMax / screenHeight);

            _topSafeOffset = Mathf.RoundToInt((1f - _anchorMax.y) * _canvasScale.y);
            _bottomSafeOffset = Mathf.RoundToInt(_anchorMin.y * _canvasScale.y);

            _calculated = true;
        }
    }
}
#endif
