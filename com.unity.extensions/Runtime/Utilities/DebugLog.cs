#if UnityExtensions
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UnityExtensions
{
    public static class DebugLog
    {
        #region Fields

        public static InputField btn;

        public static event Action<string> OnLogCalled;

        private static Color InfoLogColor = Color.white;
        private static Color WarningLogColor = Color.yellow;
        private static Color ErrorLogColor = Color.red;
        private static Color ExceptionLogColor = Color.red;

        #endregion

        #region Public Methods

        public static void Info(string message = null)
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                CallLog(LogType.Log, $"{message}", InfoLogColor);
            }
        }

        public static void Warning(string message = null)
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                CallLog(LogType.Warning, $"{message}", WarningLogColor);
            }
        }

        public static void Error(string message = null)
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                CallLog(LogType.Error, $"{message}", ErrorLogColor);
            }
        }

        public static void Exception(Exception exception = null)
        {
            if (exception != null)
            {
                CallLog(LogType.Exception, $"{exception.Message} @ {exception.StackTrace} @ {exception.Source}", ExceptionLogColor);
            }
        }

        public static void Assert(bool condition, string positive, string negative)
        {
            if (condition)
            {
                if (!string.IsNullOrWhiteSpace(positive))
                {
                    Info(positive);
                }

                return;
            }

            Error(negative);
        }

        #endregion

        #region Private Methods

        private static void CallLog(LogType logtype, string message, Color color)
        {
            if (Application.isEditor)
            {
                string log = $"<color=#{ColorUtility.ToHtmlStringRGBA(color)}> {message}</color>";

                switch (logtype)
                {
                    case LogType.Log:
                        Debug.Log(log);
                        break;
                    case LogType.Warning:
                        Debug.LogWarning(log);
                        break;
                    case LogType.Error:
                        Debug.LogError(log);
                        break;
                    case LogType.Exception:
                        Debug.LogError(log);
                        break;
                }

                OnLogCalled?.Invoke(log);
            }
        }

        #endregion
    }
}
#endif
