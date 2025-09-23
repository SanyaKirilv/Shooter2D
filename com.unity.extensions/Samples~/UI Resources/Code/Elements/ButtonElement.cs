#if UnityExtensions
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UnityExtensions.UI
{
    /// <summary>
    /// Enhanced button component with visual feedback states, click handling,
    /// and pointer event management.
    /// </summary>
    [RequireComponent(typeof(CanvasRenderer), typeof(Image), typeof(Button))]
    public class ButtonElement : GraphicComponent, IPointerDownHandler, IPointerUpHandler
    {
        #region Constants
        private const float NORMAL_STATE_ALPHA = 1f;
        private const float PRESSED_STATE_ALPHA = 0.75f;
        #endregion

        #region Fields
        [SerializeField, Tooltip("Reference to the Button component")]
        private Button _button;

        [SerializeField, Tooltip("Current state of the button")]
        private bool _state;

        [SerializeField, Tooltip("Should the button have visual press feedback?")]
        private bool _hasVisualFeedback = true;
        #endregion

        #region Events
        /// <summary>
        /// UnityEvent that is triggered when the button is clicked
        /// </summary>
        public Button.ButtonClickedEvent OnClick = new Button.ButtonClickedEvent();

        /// <summary>
        /// Action triggered when pointer interacts with the button
        /// </summary>
        public event Action<bool> OnPointerStateChanged;
        #endregion

        #region Properties
        /// <summary>
        /// Direct access to the Button component with lazy initialization
        /// </summary>
        public Button Button => _button ??= GetComponent<Button>();

        /// <summary>
        /// Direct access to the Image component
        /// </summary>
        public Image Image => Graphic.GetComponent<Image>();

        private Color CachedColor => Color.Color;

        /// <summary>
        /// Gets or sets the state of the button
        /// </summary>
        public bool State
        {
            get => _state;
            set
            {
                _state = value;
                ChangeState();
            }
        }
        #endregion

        #region Unity Methods
        protected virtual void Awake()
        {
            InitializeButton();
        }

        protected virtual void Start()
        {
            ChangeState();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Button.onClick.AddListener(HandleClick);
            ChangeState();
        }

        protected void OnDisable()
        {
            Button.onClick.RemoveListener(HandleClick);
            ResetVisualState();
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Removes all listeners from the onClick event
        /// </summary>
        public void RemoveAllListeners()
        {
            OnClick.RemoveAllListeners();
        }
        #endregion

        #region Pointer Handlers
        public void OnPointerDown(PointerEventData eventData)
        {
            if (_hasVisualFeedback)
            {
                SetColor(CachedColor * PRESSED_STATE_ALPHA);
            }
            OnPointerStateChanged?.Invoke(true);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (_hasVisualFeedback)
            {
                SetColor(CachedColor * NORMAL_STATE_ALPHA);
            }
            OnPointerStateChanged?.Invoke(false);
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Handles button click event
        /// </summary>
        protected virtual void HandleClick()
        {
            OnClick?.Invoke();
        }

        /// <summary>
        /// Updates the button's visual state based on interactability
        /// </summary>
        protected virtual void ChangeState() { }

        /// <summary>
        /// Resets the button to its default visual state
        /// </summary>
        protected virtual void ResetVisualState()
        {
            SetColor(Color.Color * NORMAL_STATE_ALPHA);
        }
        #endregion

        #region Private Methods
        private void InitializeButton()
        {
            if (_button == null)
            {
                _button = GetComponent<Button>();
                if (_button == null)
                {
                    Debug.LogError($"Button component missing on {gameObject.name}");
                }
            }
        }
        #endregion
    }
}
#endif
