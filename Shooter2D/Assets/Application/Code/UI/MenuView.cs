using UnityEngine;
using UnityExtensions.UI;

public class MenuView : ViewComponent
{
    #region Fields

    [Header("Buttons")]
    public ButtonElement PlayButton;

    #endregion

    #region Unity Methods

    protected override void OnEnable()
    {
        base.OnEnable();
        PlayButton.OnClick.AddListener(() => UEvent.GameState(GameState.Game));
    }

    private void OnDisable()
    {
        PlayButton.OnClick.RemoveAllListeners();
    }

    #endregion
}
