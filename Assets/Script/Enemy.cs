using UnityEngine;
using System.Collections;

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
    
    private bool isAttacking = false;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        
        if (PlayerController.Instance != null)
        {
            player = PlayerController.Instance;
        }
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
        Debug.Log($"[Enemy] Attack called! Player: {player}, Damage: {damage}");
        
        if (player != null)
        {
            player.TakeDamage(damage);
        }
    }

    // GUNAKAN INI UNTUK DAMAGE BERULANG
    protected virtual void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (!isAttacking)
            {
                StartCoroutine(AttackWithCooldown());
            }
        }
    }

    private IEnumerator AttackWithCooldown()
    {
        isAttacking = true;
        Attack();
        yield return new WaitForSeconds(2f); 
        isAttacking = false;
    }

    // OPSIONAL: Hapus atau biarkan OnTriggerEnter2D
    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        // Bisa dihapus atau dibiarkan kosong
    }
}