using UnityEngine;
using UnityExtensions;

public class GameManager : ReferenceBehaviour<GameManager>
{
    #region Fields

    [Header("State")]
    public FrameRate FrameRate = FrameRate.FPS60;
    public GameState GameState = GameState.Menu;

    [Header("Score")]
    [SerializeField]
    private int _score;

    [SerializeField]
    private int _bestScore;

    [Header("Player")]
    public Player Player;
    public int InitialHealh;

    [Header("UI")]
    public MenuView MenuView;
    public GameView GameView;

    public int Score
    {
        get => _score;
        set => _score = value;
    }

    public int BestScore
    {
        get => _bestScore = GetBestScore();
        set => _bestScore = SetBestScore(value);
    }

    #endregion

    #region Unity Methods

    protected override void OnEnable()
    {
        base.OnEnable();
        UEvent.OnPlayGame += PlayGame;
        UEvent.OnStopGame += StopGame;
        UEvent.OnEnemyDestroyed += AddScore;
        UEvent.OnObstacleDestroyed += AddScore;
        UEvent.OnPlayerGetDamage += GameView.UpdateView;
    }

    private void OnDisable()
    {
        UEvent.OnPlayGame -= PlayGame;
        UEvent.OnStopGame -= StopGame;
        UEvent.OnEnemyDestroyed -= AddScore;
        UEvent.OnObstacleDestroyed -= AddScore;
        UEvent.OnPlayerGetDamage -= GameView.UpdateView;
    }

    private void Start()
    {
        Application.targetFrameRate = (int)FrameRate;
        UEvent.GameState(GameState.Menu);
    }

    #endregion

    #region Private Methods

    private void PlayGame()
    {
        Score = 0;
        Player.Health = InitialHealh;
        HandleView();
    }

    private void StopGame()
    {
        HandleView();
    }

    private void AddScore()
    {
        Score++;
        if (Score > BestScore)
        {
            BestScore = Score;
        }

        GameView.UpdateView();
    }

    private void HandleView()
    {
        MenuView.IsVisible = GameState == GameState.Menu;
        GameView.IsVisible = GameState == GameState.Game;
    }

    private int GetBestScore()
    {
        if (!PlayerPrefs.HasKey("BestScore"))
        {
            PlayerPrefs.SetInt("BestScore", 0);
        }

        return PlayerPrefs.GetInt("BestScore");
    }

    private int SetBestScore(int value)
    {
        PlayerPrefs.SetInt("BestScore", value);
        return value;
    }

    #endregion
}
