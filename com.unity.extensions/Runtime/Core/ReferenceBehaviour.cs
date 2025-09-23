#if UnityExtensions
using UnityEngine;

namespace UnityExtensions
{
    [DefaultExecutionOrder(-1000)]
    public class ReferenceBehaviour<T> : MonoBehaviour
        where T : ReferenceBehaviour<T>
    {
        #region Fields

        private static T _ref;

        public static T Ref
        {
            get
            {
                _ = ValidateReference();
                return _ref;
            }
        }

        #endregion

        #region Unity Methods

        protected virtual void OnEnable()
        {
            InitializeReference();
        }

        protected virtual void OnDestroy()
        {
            Cleanup();
        }

        #endregion

        #region Private Methods

        private void InitializeReference()
        {
            if (_ref == null)
            {
                _ref = (T)this;
                return;
            }

            if (_ref != this)
            {
                DebugLog.Error($"Detected duplicate of {typeof(T).Name} on {gameObject.name}");
                gameObject.SetActive(false);
            }
        }

        private static bool ValidateReference()
        {
            bool state = _ref != null;
            DebugLog.Assert(
                condition: state,
                positive: null,
                negative: $"Reference of {typeof(T).Name} is null"
            );

            return state;
        }

        private void Cleanup()
        {
            _ref = null;
        }

        #endregion
    }
}
#endif
