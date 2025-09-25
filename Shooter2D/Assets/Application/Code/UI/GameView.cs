using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityExtensions.UI;

public class GameView : ViewComponent
{
    #region Fields

    [Header("Buttons")]
    public ButtonElement CloseButton;
    public ButtonElement ShootButton;

    [Header("Texts")]
    public TextElement ScoreText;
    public TextElement BestScoreText;
    public TextElement HealthText;

    [Header("Other")]
    public LayoutComponent Layout;

    #endregion

    #region Unity Methods

    protected override void OnEnable()
    {
        base.OnEnable();
        CloseButton.OnClick.AddListener(() => UEvent.GameState(GameState.Menu));
        ShootButton.OnClick.AddListener(GameManager.Ref.Player.Shoot);
    }

    private void OnDisable()
    {
        CloseButton.OnClick.RemoveAllListeners();
        ShootButton.OnClick.RemoveAllListeners();
    }

    #endregion

    #region Public Methods

    public void UpdateView()
    {
        ScoreText.SetText($"Ваш счет: {GameManager.Ref.Score}");
        BestScoreText.SetText($"Рекорд: {GameManager.Ref.BestScore}");
        HealthText.SetText(
            $"Жизни: {GameManager.Ref.Player.Health}/{GameManager.Ref.InitialHealh}"
        );
        Layout.UpdateLayout().Forget();
    }

    #endregion
}
