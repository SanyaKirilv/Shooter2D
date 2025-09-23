#if UnityExtensions
using UnityEngine;
using UnityEngine.UI;

namespace UnityExtensions.UI
{
    [RequireComponent(typeof(CanvasRenderer), typeof(Button))]
    public class ToggleImageButtonElement : ButtonElement
    {
        public ImageElement Child;
        public Sprite OnSprite;
        public Sprite OffSprite;

        protected override void ChangeState()
        {
            if (!Child)
            {
                Debug.LogError("No child ImageElement found for InputButtonElement");
                return;
            }

            Child.Sprite = State ? OnSprite : OffSprite;
        }
    }
}
#endif
