using UnityEngine;

public class Player : SpaceObject
{
    #region Field

    [SerializeField]
    private LaserObject _laser;

    #endregion

    #region Unity Methods

    protected override void OnCollisionEnter(Collision other)
    {
        if (
            other.gameObject.CompareTag("Obstacle")
            || other.gameObject.CompareTag("Enemy")
            || other.gameObject.CompareTag("Enemy Laser")
        )
        {
            TakeDamage();
            UEvent.PlayerGetDamage();
            Destroy(other.gameObject);
        }
    }

    #endregion

    #region Public Methods

    public override void OnDestroyed()
    {
        base.OnDestroyed();
        UEvent.GameState(GameState.Game);
    }

    public void Shoot()
    {
        Instantiate(_laser, transform.position, Quaternion.identity);
    }

    #endregion
}
