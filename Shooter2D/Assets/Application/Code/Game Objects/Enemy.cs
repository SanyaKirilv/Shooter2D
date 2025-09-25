using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Enemy : SpaceObject
{
    #region Field

    public int ShootDelay
    {
        get => _shootDelay;
        set => _shootDelay = value;
    }

    [SerializeField]
    private int _shootDelay;

    [SerializeField]
    private LaserObject _laser;

    private bool IsFrontDirection => this.gameObject.CompareTag("Player");

    #endregion

    #region Unity Methods

    private void Start()
    {
        Move().Forget();
        Shoot().Forget();
    }

    protected override void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player Laser"))
        {
            TakeDamage();
            Destroy(other.gameObject);
        }
        else if (other.gameObject.CompareTag("Dead Zone"))
        {
            Destroy(gameObject);
        }
    }

    #endregion

    #region Public Methods

    public void SetParameters(int health, float speed, int shootDelay)
    {
        SetParameters(health, speed);
        ShootDelay = shootDelay;
    }

    public override async UniTaskVoid Move()
    {
        float timer = 0f;
        Vector3 direction = IsFrontDirection ? Vector3.forward : Vector3.back;

        while (this != null)
        {
            transform.localPosition += direction * Speed * Time.deltaTime;

            timer += Time.deltaTime;
            await UniTask.Yield();
        }
    }

    public async UniTaskVoid Shoot()
    {
        while (this != null)
        {
            Instantiate(_laser, transform.position, Quaternion.identity);
            await UniTask.Delay(TimeSpan.FromSeconds(ShootDelay));
        }
    }

    public override void OnDestroyed()
    {
        base.OnDestroyed();
        Destroy(gameObject);
        UEvent.DestroyEnemy();
    }

    #endregion
}
