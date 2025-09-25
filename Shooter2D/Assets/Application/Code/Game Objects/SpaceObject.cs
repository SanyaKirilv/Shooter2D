using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityExtensions;

public abstract class SpaceObject : MonoBehaviour
{
    #region Fields

    public int Health
    {
        get => _health;
        set => _health = value;
    }

    public float Speed
    {
        get => _speed;
        set => _speed = value;
    }

    [SerializeField]
    private int _health;

    [SerializeField]
    private float _speed;

    #endregion

    #region Virtual Methods

    public void SetParameters(int health, float speed)
    {
        Health = health;
        Speed = speed;
    }

    public virtual async UniTaskVoid Move()
    {
        await UniTask.CompletedTask;
    }

    public virtual void TakeDamage()
    {
        Health -= 1;

        if (Health > 0)
        {
            DebugLog.Info($"{name}: takes damage, Health: {Health}");
            return;
        }

        OnDestroyed();
    }

    public virtual void OnDestroyed()
    {
        DebugLog.Info($"{name}: destroyed!");
    }

    protected abstract void OnCollisionEnter(Collision other);

    #endregion
}
