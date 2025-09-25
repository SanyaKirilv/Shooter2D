using Cysharp.Threading.Tasks;
using UnityEngine;

public class Obstacle : SpaceObject
{
    #region Field

    private bool IsFrontDirection => this.gameObject.CompareTag("Player");

    #endregion

    #region Unity Methods

    private void Start()
    {
        Move().Forget();
    }

    protected override void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player Laser"))
        {
            TakeDamage();
        }
        else if (other.gameObject.CompareTag("Dead Zone"))
        {
            Destroy(gameObject);
        }
    }

    #endregion

    #region Public Methods

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

    public override void OnDestroyed()
    {
        base.OnDestroyed();
        Destroy(gameObject);
        UEvent.DestroyObstacle();
    }

    #endregion
}
