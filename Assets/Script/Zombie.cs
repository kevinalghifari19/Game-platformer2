using UnityEngine;

public class Zombie : Enemy
{
    [SerializeField] private float gravityScale = 12f;

    protected override void Awake()
    {
        base.Awake();
    }

    void Start()
    {
        rb.gravityScale = gravityScale;
    }

    protected override void Update()
    {
        base.Update();

        if (!isRecoiling && player != null)
        {
            // Chase player secara horizontal
            transform.position = Vector2.MoveTowards(
                transform.position,
                new Vector2(player.transform.position.x, transform.position.y),
                speed * Time.deltaTime
            );
        }
    }

    public override void EnemyHit(float _damageDone, Vector2 _hitDirection, float _hitForce)
    {
        base.EnemyHit(_damageDone, _hitDirection, _hitForce);
    }
}