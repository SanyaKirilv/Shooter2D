using Cysharp.Threading.Tasks;
using UnityEngine;

public class LaserObject : MonoBehaviour
{
    #region Fields

    [Header("Parameters")]
    [SerializeField]
    private float _speed;

    [SerializeField]
    private float _lifetime;

    private bool IsFrontDirection => this.gameObject.CompareTag("Player Laser");

    #endregion

    #region Unity Methods

    private void Awake()
    {
        Move().Forget();
    }

    protected void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Obstacle") || other.gameObject.CompareTag("Enemy Laser"))
        {
            Destroy(gameObject);
        }
    }

    #endregion

    #region Private Methods

    private async UniTaskVoid Move()
    {
        float timer = 0f;
        Vector3 direction = IsFrontDirection ? Vector3.forward : Vector3.back;

        while (timer < _lifetime && this != null)
        {
            transform.position += direction * _speed * Time.deltaTime;

            timer += Time.deltaTime;
            await UniTask.Yield();
        }

        if (this != null)
        {
            Destroy(gameObject);
        }
    }

    #endregion
}
