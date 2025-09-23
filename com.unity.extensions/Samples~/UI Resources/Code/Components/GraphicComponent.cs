#if UnityExtensions
using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace UnityExtensions.UI
{
    [RequireComponent(typeof(Graphic))]
    public class GraphicComponent : RectComponent
    {
        #region Fields
        [SerializeField]
        private Graphic _graphic;

        [SerializeField]
        private ColorComponent _color = new ColorComponent(UnityEngine.Color.clear, 0);

        public Graphic Graphic
        {
            get => _graphic;
        }

        public ColorComponent Color
        {
            get => _color;
            set
            {
                _color = value;
                SetColor(Color);
            }
        }

        #endregion

        #region Unity Methods

        protected override void OnEnable()
        {
            base.OnEnable();
            InitializeGraphic();
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            InitializeGraphic();

            if (!Application.isPlaying)
            {
                SetColor(Color);
            }
        }
#endif

        #endregion

        #region Public Methods

        public virtual void SetColor(ColorComponent newColor = null)
        {
            Graphic.color = newColor ?? Color;
        }

        public void SetColorForce(ColorComponent newColor)
        {
            Color = newColor ?? Color;
            SetColor(Color);
        }

        public async UniTask SetColorLerp(ColorComponent target, float duration = 0.25f)
        {
            await LerpColor(Graphic.color, target, duration, color => Graphic.color = color);
        }

        #endregion

        #region Private Methods

        private void InitializeGraphic()
        {
            if (_graphic == null && !TryGetComponent(out _graphic))
            {
                Debug.LogError($"No Graphic component attached to {name}!");
            }
        }

        private async UniTask LerpColor(
            Color start,
            Color target,
            float duration,
            Action<Color> setColor
        )
        {
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / duration);
                setColor(UnityEngine.Color.LerpUnclamped(start, target, t));
                await UniTask.Yield();
            }

            setColor(target);
        }

        #endregion
    }

    [RequireComponent(typeof(Graphic))]
    public class GraphicComponent<T> : RectComponent<T>
        where T : GraphicComponent<T>
    {
        #region Fields

        private Graphic _graphic;
        private ColorComponent _color = new ColorComponent(UnityEngine.Color.clear, 0);

        public Graphic Graphic
        {
            get => _graphic;
        }

        public ColorComponent Color
        {
            get => _color;
            set
            {
                _color = value;
                SetColor(Color);
            }
        }

        #endregion

        #region Unity Methods

        protected override void OnEnable()
        {
            base.OnEnable();
            InitializeGraphic();
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            InitializeGraphic();

            if (!Application.isPlaying)
            {
                SetColor(Color);
            }
        }
#endif

        #endregion

        #region Public Methods

        public virtual void SetColor(ColorComponent newColor = null)
        {
            Graphic.color = newColor ?? Color;
        }

        public void SetColorForce(ColorComponent newColor)
        {
            Color = newColor ?? Color;
            SetColor(Color);
        }

        public async UniTask SetColorLerp(ColorComponent target, float duration = 0.25f)
        {
            await LerpColor(Graphic.color, target, duration, color => Graphic.color = color);
        }

        #endregion

        #region Private Methods

        private void InitializeGraphic()
        {
            if (_graphic == null && !TryGetComponent(out _graphic))
            {
                Debug.LogError($"No Graphic component attached to {name}!");
            }
        }

        private async UniTask LerpColor(
            Color start,
            Color target,
            float duration,
            Action<Color> setColor
        )
        {
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / duration);
                setColor(UnityEngine.Color.LerpUnclamped(start, target, t));
                await UniTask.Yield();
            }

            setColor(target);
        }

        #endregion
    }
}
#endif
