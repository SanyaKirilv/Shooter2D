using UnityEngine;
using UnityExtensions;

public class PlayerManager : ReferenceBehaviour<PlayerManager>
{
    #region Fields

    public Player Player;

    [Header("Move Settings X Position")]
    [SerializeField]
    private float _minXPosition = -5f;

    [SerializeField]
    private float _maxXPosition = 5f;

    [Header("Move Settings Drag")]
    [SerializeField]
    private float _dragSensitivity = 0.01f;

    private Vector2 _lastTouchPosition;
    private Vector3 _startPosition;

    #endregion

    #region Unity Methods

    private void Start()
    {
        _startPosition = Player.transform.position;
    }

    private void Update()
    {
        if (GameManager.Ref.GameState == GameState.Game)
        {
            HandleDrag();
        }
        else
        {
            Player.transform.position = _startPosition;
        }
    }

    #endregion

    #region Drag Logic

    private void HandleDrag()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                _lastTouchPosition = touch.position;
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                Vector2 delta = touch.position - _lastTouchPosition;
                _lastTouchPosition = touch.position;

                MoveHorizontal(delta.x);
            }
        }
    }

    private void MoveHorizontal(float deltaX)
    {
        Vector3 pos = Player.transform.position;
        pos.x += deltaX * _dragSensitivity;
        pos.x = Mathf.Clamp(pos.x, _minXPosition, _maxXPosition);
        Player.transform.position = pos;
    }

    #endregion
}
