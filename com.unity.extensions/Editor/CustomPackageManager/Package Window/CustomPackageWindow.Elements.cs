#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

public partial class CustomPackageWindow : EditorWindow
{
    public static Vector2 WindowRect = new Vector2(512f, 640f);

    public class LogoRect
    {
        public Texture2D Texture2D = null;

        public float Height = 0;
        public float Width = WindowRect.x;
        public float xOffset = 0;
        public float yOffset = 0;

        public Vector2 ImageSize = Vector2.zero;

        public Color BackgroundColor = Color.clear;

        public Vector2 Offset
        {
            get => new Vector2(_rect.width + xOffset, _rect.height + yOffset);
        }

        private Rect _rect
        {
            get => new Rect(xOffset, yOffset, Width - xOffset * 2, Height);
        }

        private Rect _logoRect
        {
            get
            {
                float offsetX = (Width - ImageSize.x) / 2;
                float offsetY = (_rect.height - ImageSize.y) / 2 + yOffset;
                return new Rect(offsetX, offsetY, ImageSize.x, ImageSize.y);
            }
        }

        public void Draw()
        {
            EditorGUI.DrawRect(_rect, BackgroundColor);
            GUI.DrawTexture(_logoRect, Texture2D, ScaleMode.ScaleToFit);
        }
    }

    public class TextData
    {
        public string Text = string.Empty;

        public float Width = WindowRect.x;
        public float Height = 0;
        public float xOffset = 0;
        public float yOffset = 0;

        public int FontSize = 12;
        public FontStyle FontStyle = FontStyle.Normal;
        public TextAnchor Aligment = TextAnchor.MiddleLeft;
        public Color FontColor = Color.black;
        public bool Wrapped = false;

        public Color BackgroundColor = Color.clear;

        public Vector2 Offset
        {
            get => new Vector2(_rect.width + xOffset, _rect.height + yOffset);
        }

        public float WrappedWidth = 0;
        public float WrappedHeight = 0;

        private Rect _rect
        {
            get
            {
                GUIContent content = new GUIContent(Text);
                Vector2 textSize = _style.CalcSize(content);

                WrappedWidth = Width;
                WrappedHeight = Height;

                if (textSize.x > WrappedWidth)
                {
                    WrappedHeight *= 2;
                }

                if (textSize.x < WrappedWidth)
                {
                    WrappedWidth = textSize.x;
                }
                else
                {
                    WrappedWidth = Width - xOffset * 2;
                }

                return new Rect(xOffset, yOffset, WrappedWidth, WrappedHeight);
            }
        }

        private GUIStyle _style
        {
            get
            {
                GUIStyle style = new GUIStyle();
                style.fontSize = FontSize;
                style.fontStyle = FontStyle;
                style.alignment = Aligment;
                style.normal.textColor = FontColor;
                style.wordWrap = Wrapped;
                return style;
            }
        }

        public void Draw()
        {
            EditorGUI.DrawRect(_rect, BackgroundColor);
            EditorGUI.LabelField(_rect, Text, _style);
        }
    }

    public class ButtonData
    {
        public event Action OnClick;
        public string Text = string.Empty;
        public bool IsAvailable = true;
        public bool IsVisible = true;
        public bool IsLinkButton = true;

        public float Height = 0;
        public float Width = WindowRect.x;
        public float xOffset = 0;
        public float yOffset = 0;

        public Color BackgroundColor = Color.clear;

        public Vector2 Offset
        {
            get => new Vector2(_rect.width + xOffset, _rect.height + yOffset);
        }

        private Rect _rect
        {
            get => new Rect(xOffset, yOffset, Width, Height);
        }

        private GUIStyle _buttonStyle
        {
            get
            {
                GUIStyle style = new GUIStyle(GUI.skin.label);
                GUIStyleState state = new GUIStyleState()
                {
                    textColor = new Color32(77, 126, 255, 255),
                    background = null,
                };

                style.normal = style.hover = style.active = state;
                style.alignment = TextAnchor.MiddleCenter;
                style.fontStyle = FontStyle.Normal;

                return IsLinkButton ? style : new GUIStyle(GUI.skin.button);
            }
        }

        public void Draw()
        {
            EditorGUI.DrawRect(_rect, BackgroundColor);

            if (!IsVisible)
            {
                return;
            }

            EditorGUI.BeginDisabledGroup(!IsAvailable);
            bool state = GUI.Button(_rect, Text, _buttonStyle);

            if (state)
            {
                OnClick?.Invoke();
            }

            EditorGUI.EndDisabledGroup();
        }
    }

    public class PanelData
    {
        public float Height = 0;
        public float Width = WindowRect.x;
        public float xOffset = 0;
        public float yOffset = 0;

        public Color BackgroundColor = Color.clear;

        public Vector2 Offset
        {
            get => new Vector2(_rect.width + xOffset, _rect.height + yOffset);
        }

        private Rect _rect
        {
            get => new Rect(xOffset, yOffset, Width - xOffset * 2, Height);
        }

        public void Draw()
        {
            EditorGUI.DrawRect(_rect, BackgroundColor);
        }
    }

    public class BoxData
    {
        public float Height = 0;
        public float Width = WindowRect.x;
        public float xOffset = 0;
        public float yOffset = 0;

        public Color BackgroundColor = Color.clear;

        public Vector2 Offset
        {
            get => new Vector2(_rect.width + xOffset, _rect.height + yOffset);
        }

        private Rect _rect
        {
            get => new Rect(xOffset, yOffset, Width - xOffset * 2, Height);
        }

        public void Draw()
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.Space(32);
            EditorGUILayout.EndVertical();
        }
    }

    public class MessageData
    {
        public string Text = string.Empty;
        public MessageType Type = MessageType.Info;

        public float Width = WindowRect.x;
        public float xOffset = 0;
        public float yOffset = 0;

        public Color BackgroundColor = Color.clear;

        public Vector2 Offset
        {
            get => new Vector2(_rect.width + xOffset, _rect.height + yOffset);
        }

        private GUIStyle _messageStyle
        {
            get
            {
                GUIStyle style = new GUIStyle(GUI.skin.label);
                GUIStyleState state = new GUIStyleState()
                {
                    textColor = new Color32(77, 126, 255, 255),
                    background = null,
                };

                style.normal = style.hover = style.active = state;
                style.alignment = TextAnchor.MiddleCenter;
                style.fontStyle = FontStyle.Normal;
                style.wordWrap = true;

                return style;
            }
        }

        private Rect _rect
        {
            get
            {
                float height = _messageStyle.CalcSize(new GUIContent(Text)).y + 16;
                return new Rect(xOffset, yOffset, Width - xOffset * 2, height);
            }
        }

        public void Draw()
        {
            EditorGUI.HelpBox(_rect, Text, Type);
        }
    }
}
#endif
