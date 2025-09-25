using System;

public static class UEvent
{
    #region UI

    public static event Action OnPlayGame;
    public static event Action OnStopGame;

    public static void GameState(GameState state)
    {
        GameManager.Ref.GameState = state;
        (state == global::GameState.Game ? OnPlayGame : OnStopGame)?.Invoke();
    }

    public static event Action OnObstacleDestroyed;

    public static void DestroyObstacle() => OnObstacleDestroyed();

    public static event Action OnEnemyDestroyed;

    public static void DestroyEnemy() => OnObstacleDestroyed();

    public static event Action OnPlayerGetDamage;

    public static void PlayerGetDamage() => OnObstacleDestroyed();

    #endregion
}
