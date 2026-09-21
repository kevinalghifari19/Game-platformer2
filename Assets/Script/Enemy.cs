using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected float health;
    
    [SerializeField] protected float recoilLength;
    [SerializeField] protected float recoilFactor;
    
    [SerializeField] protected float speed;
    protected PlayerController player;
    
    [SerializeField] protected float damage;
    
    protected float recoilTimer;
    protected bool isRecoiling = false;
    protected Rigidbody2D rb;
    
    // Variabel untuk flip direction
    protected bool facingRight = true;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        player = PlayerController.Instance;
    }

    protected virtual void Update()
    {
        if (health <= 0)
        {
            Destroy(gameObject);
            return;
        }

        if (player == null)
        {
            if (PlayerController.Instance != null)
            {
                player = PlayerController.Instance;
            }
        }

        if (isRecoiling)
        {
            if (recoilTimer < recoilLength)
            {
                recoilTimer += Time.deltaTime;
            }
            else
            {
                isRecoiling = false;
                recoilTimer = 0f;
            }
        }
    }

    // Fungsi untuk flip enemy menghadap player
    protected void Flip()
    {
        if (player == null) return;

        // Cek posisi player relatif terhadap enemy
        float distanceToPlayer = player.transform.position.x - transform.position.x;

        // Jika player di kanan dan enemy menghadap kiri, flip ke kanan
        if (distanceToPlayer > 0 && !facingRight)
        {
            FlipDirection();
        }
        // Jika player di kiri dan enemy menghadap kanan, flip ke kiri
        else if (distanceToPlayer < 0 && facingRight)
        {
            FlipDirection();
        }
    }

    // Fungsi untuk mengubah arah hadap
    void FlipDirection()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    public virtual void EnemyHit(float _damageDone, Vector2 _hitDirection, float _hitForce)
    {
        health -= _damageDone;

        if (!isRecoiling)
        {
            rb.AddForce(-_hitForce * recoilFactor * _hitDirection, ForceMode2D.Impulse);
            isRecoiling = true;
            recoilTimer = 0f;
        }
    }

    protected virtual void Attack()
    {
        if (player != null)
        {
            player.TakeDamage(damage);
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Attack();
        }
    }
}