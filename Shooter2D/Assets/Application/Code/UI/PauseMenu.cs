using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityExtensions.UI;

public class PauseMenu : ViewComponent
{
    #region Fields

    [Header("Buttons")]
    public ButtonElement ResumeButton;
    public ButtonElement RestartButton;
    public ButtonElement CloseButton;

    [Header("Texts")]
    public TextElement ScoreText;
    public TextElement BestScoreText;

    [Header("Other")]
    public LayoutComponent Layout;

    #endregion

    #region Public Methods

    public void ShowMenu(bool isResumable = true)
    {
        Show();

        ResumeButton.IsActive = isResumable;
        Layout.UpdateLayout().Forget();
    }

    #endregion
}
