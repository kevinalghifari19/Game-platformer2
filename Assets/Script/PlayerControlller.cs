using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Horizontal Movement Settings:")]
    [SerializeField] private float walkSpeed = 20f;
    [Space(5)]

    [Header("Vertical Movement Settings:")]
    [SerializeField] private float jumpForce = 45f;
    private int jumpBufferCounter = 0;
    [SerializeField] private int jumpBufferFrames = 8;
    private float coyoteTimeCounter = 0;
    [SerializeField] private float coyoteTime = 0.1f;
    private int airJumpCounter = 0;
    [SerializeField] private int maxAirJumps = 1;
    [Space(5)]

    [Header("Ground Check Settings:")]
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private float groundCheckY = 0.2f;
    [SerializeField] private float groundCheckX = 0.5f;
    [SerializeField] private LayerMask whatIsGround;
    [Space(5)]

    [Header("Dash Settings:")]
    [SerializeField] private float dashSpeed = 70f;
    [SerializeField] private float dashTime = 0.2f;
    [SerializeField] private float dashCooldown = 0.35f;
    [SerializeField] private GameObject dashEffect;
    [Space(5)]

    [Header("Attack Settings:")]
    [SerializeField] private float timeBetweenAttack = 0.5f;
    [SerializeField] private float damage = 10f;
    [SerializeField] private GameObject slashEffect;
    private float timeSinceAttack;
    private bool attack;
    private float yAxis;
    [Space(5)]

    [Header("Attack Area Settings:")]
    [SerializeField] private Transform sideAttackTransform;
    [SerializeField] private Transform upAttackTransform;
    [SerializeField] private Transform downAttackTransform;

    [SerializeField] private Vector2 sideAttackArea;
    [SerializeField] private Vector2 upAttackArea;
    [SerializeField] private Vector2 downAttackArea;

    [SerializeField] private LayerMask attackableLayer;
    [Space(5)]

    [Header("Recoil Settings:")]
    [SerializeField] private int recoilXSteps = 5;
    [SerializeField] private int recoilYSteps = 5;
    [SerializeField] private float recoilXSpeed = 100f;
    [SerializeField] private float recoilYSpeed = 100f;

    private int stepsXRecoiled;
    private int stepsYRecoiled;

    [Header("Health Settings:")]
    [SerializeField] private int health;
    [SerializeField] private int maxHealth = 10;
    [SerializeField] private GameObject bloodSpurt;
    [SerializeField] private float hitFlashSpeed = 10f;
    [SerializeField] private float timeToHeal = 1f;

    [Header("Mana Settings:")]
    [SerializeField] private UnityEngine.UI.Image manaStorage;
    [SerializeField] private float mana = 0.5f;
    [SerializeField] private float manaDrainSpeed = 0.2f;
    [SerializeField] private float manaGain = 0.3f;

    private float healTimer;

    // Health Property
    public int Health 
    { 
        get => health; 
        set 
        {
            if (health != value)
            {
                health = value;
                if (HeartController.Instance != null)
                    HeartController.Instance.UpdateHeartsUI();
            }
        }
    }

    public int MaxHealth 
    { 
        get => maxHealth; 
        set 
        {
            if (maxHealth != value)
            {
                maxHealth = value;
                if (HeartController.Instance != null)
                    HeartController.Instance.UpdateHeartsUI();
            }
        }
    }

    // Mana Property
    public float Mana 
    { 
        get => mana; 
        set 
        {
            if (mana != value)
            {
                mana = Mathf.Clamp(value, 0, 1f); // Clamp antara 0-1
                UpdateManaUI();
            }
        }
    }

    private PlayerStateList pState;
    private Rigidbody2D rb;
    private float xAxis;
    private float gravity;
    private Animator anim;
    private bool canDash = true;
    private bool dashed;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    public static PlayerController Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        pState = GetComponent<PlayerStateList>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        gravity = rb.gravityScale;
        health = maxHealth;
        mana = 0f; // Mulai dengan mana 0
        
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
    }

    void Update()
    {
        GetInputs();
        UpdateJumpVariables();

        if (pState.dashing) return;

        Flip();
        Move();
        Jump();
        StartDash();
        Attack();
        Heal();
    }

    private void FixedUpdate()
    {
        if (pState.dashing) return;
        Recoil();
    }

    void GetInputs()
    {
        xAxis = Input.GetAxisRaw("Horizontal");
        yAxis = Input.GetAxisRaw("Vertical");
        attack = Input.GetButtonDown("Attack");
    }

    void Flip()
    {
        if (xAxis < 0)
        {
            transform.localScale = new Vector2(-1, transform.localScale.y);
            pState.lookingRight = false;
        }
        else if (xAxis > 0)
        {
            transform.localScale = new Vector2(1, transform.localScale.y);
            pState.lookingRight = true;
        }
    }

   void Move()
{
    // BLOKIR movement jika sedang healing
    if (pState.healing) 
    {
        // Stop horizontal movement
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        anim.SetBool("Walking", false);
        return;
    }

    if (pState.recoilingX) return;

    rb.linearVelocity = new Vector2(walkSpeed * xAxis, rb.linearVelocity.y);
    anim.SetBool("Walking", rb.linearVelocity.x != 0 && Grounded());
}

    void StartDash()
    {
        if (Input.GetButtonDown("Dash") && canDash && !dashed)
        {
            StartCoroutine(Dash());
            dashed = true;
        }
        if (Grounded()) dashed = false;
    }

    IEnumerator Dash()
    {
        canDash = false;
        pState.dashing = true;
        anim.SetTrigger("Dashing");
        rb.gravityScale = 0;
        rb.linearVelocity = new Vector2(transform.localScale.x * dashSpeed, 0);

        if (dashEffect != null && Grounded())
        {
            Vector3 spawnPos = new Vector3(
                transform.position.x,
                groundCheckPoint.position.y + 0.3f,
                transform.position.z
            );
            GameObject effect = Instantiate(dashEffect, spawnPos, transform.rotation);
            if (transform.localScale.x < 0)
                effect.transform.localScale = new Vector3(-1, 1, 1);
        }

        yield return new WaitForSeconds(dashTime);
        rb.gravityScale = gravity;
        pState.dashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    void Recoil()
    {
        if (pState.recoilingX)
        {
            if (pState.lookingRight)
                rb.linearVelocity = new Vector2(-recoilXSpeed, 0);
            else
                rb.linearVelocity = new Vector2(recoilXSpeed, 0);

            if (stepsXRecoiled < recoilXSteps)
                stepsXRecoiled++;
            else
                StopRecoilX();
        }

        if (pState.recoilingY)
        {
            rb.gravityScale = 0;
            if (yAxis < 0)
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, recoilYSpeed);
            else
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, -recoilYSpeed);

            if (stepsYRecoiled < recoilYSteps)
                stepsYRecoiled++;
            else
                StopRecoilY();
        }
        else
        {
            rb.gravityScale = gravity;
        }

        if (Grounded()) StopRecoilY();
    }

    void StopRecoilX()
    {
        stepsXRecoiled = 0;
        pState.recoilingX = false;
    }

    void StopRecoilY()
    {
        stepsYRecoiled = 0;
        pState.recoilingY = false;
        rb.gravityScale = gravity;
    }

    void Attack()
    {
        timeSinceAttack += Time.deltaTime;

        if (attack && timeSinceAttack >= timeBetweenAttack)
        {
            timeSinceAttack = 0f;
            anim.SetTrigger("Attacking");

            if (yAxis == 0 || (yAxis < 0 && Grounded()))
            {
                Instantiate(slashEffect, sideAttackTransform.position, transform.rotation);
                Hit(sideAttackTransform, sideAttackArea, ref pState.recoilingX, recoilXSpeed);
            }
            else if (yAxis > 0)
            {
                SlashEffectAtAngle(slashEffect, 80, upAttackTransform);
                Hit(upAttackTransform, upAttackArea, ref pState.recoilingY, recoilYSpeed);
            }
            else if (yAxis < 0 && !Grounded())
            {
                SlashEffectAtAngle(slashEffect, -80, downAttackTransform);
                Hit(downAttackTransform, downAttackArea, ref pState.recoilingY, recoilYSpeed);
            }
        }
    }

    void SlashEffectAtAngle(GameObject prefab, int angle, Transform pos)
    {
        GameObject effect = Instantiate(prefab, pos.position, transform.rotation);
        effect.transform.eulerAngles = new Vector3(0, 0, angle);
        effect.transform.localScale = new Vector2(transform.localScale.x, transform.localScale.y);
    }

    void Hit(Transform attackTransform, Vector2 attackArea, ref bool recoilDir, float recoilStrength)
    {
        Collider2D[] objectsToHit = Physics2D.OverlapBoxAll(
            attackTransform.position, 
            attackArea, 
            0, 
            attackableLayer
        );

        if (objectsToHit.Length > 0)
        {
            for (int i = 0; i < objectsToHit.Length; i++)
            {
                if (objectsToHit[i].GetComponent<Enemy>() != null)
                {
                    Vector2 hitDirection = (transform.position - objectsToHit[i].transform.position).normalized;
                    objectsToHit[i].GetComponent<Enemy>().EnemyHit(damage, hitDirection, recoilStrength);
                    recoilDir = true;
                    
                    // Gain mana
                    Mana += manaGain;
                }
            }
        }
    }

    void UpdateManaUI()
    {
        if (manaStorage != null)
        {
            manaStorage.fillAmount = mana;
        }
    }

    public void TakeDamage(float damage)
    {
        if (!pState.invincible)
        {
            if (bloodSpurt != null)
                Instantiate(bloodSpurt, transform.position, transform.rotation);
            
            StartCoroutine(StopTakingDamage(damage));
        }
    }

    IEnumerator StopTakingDamage(float damage)
    {
        pState.invincible = true;
        Health -= Mathf.RoundToInt(damage);
        
        if (anim != null)
            anim.SetTrigger("TakeDamage");
        
        StartCoroutine(FlashPlayer());
        
        yield return new WaitForSecondsRealtime(2f);
        
        pState.invincible = false;
    }

    IEnumerator FlashPlayer()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
                originalColor = spriteRenderer.color;
        }

        float flashDuration = 2f;
        float flashInterval = 1f / hitFlashSpeed;
        float elapsed = 0f;

        while (elapsed < flashDuration)
        {
            spriteRenderer.color = Color.white;
            yield return new WaitForSecondsRealtime(flashInterval);
            spriteRenderer.color = originalColor;
            yield return new WaitForSecondsRealtime(flashInterval);
            elapsed += flashInterval * 2;
        }

        spriteRenderer.color = originalColor;
    }

    void Heal()
    {
        if (Input.GetButton("Healing") && Health < maxHealth && Grounded() && !pState.dashing && Mana > 0)
        {
            pState.healing = true;
            anim.SetBool("Healing", true);

            healTimer += Time.deltaTime;
            if (healTimer >= timeToHeal)
            {
                Health++;
                healTimer = 0;
            }

            // Drain mana
            Mana -= Time.deltaTime * manaDrainSpeed;
        }
        else
        {
            pState.healing = false;
            anim.SetBool("Healing", false);
            healTimer = 0;
        }
    }

    public bool Grounded()
    {
        return Physics2D.Raycast(groundCheckPoint.position, Vector2.down, groundCheckY, whatIsGround) ||
               Physics2D.Raycast(groundCheckPoint.position + new Vector3(groundCheckX, 0, 0), Vector2.down, groundCheckY, whatIsGround) ||
               Physics2D.Raycast(groundCheckPoint.position + new Vector3(-groundCheckX, 0, 0), Vector2.down, groundCheckY, whatIsGround);
    }

    void Jump()
    {
        if (Input.GetButtonUp("Jump") && rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
            pState.jumping = false;
        }

        if (!pState.jumping)
        {
            if (jumpBufferCounter > 0 && coyoteTimeCounter > 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                pState.jumping = true;
                airJumpCounter = 0;
                coyoteTimeCounter = 0;
                jumpBufferCounter = 0;
            }
            else if (!Grounded() && airJumpCounter < maxAirJumps && Input.GetButtonDown("Jump"))
            {
                pState.jumping = true;
                airJumpCounter++;
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            }
        }

        anim.SetBool("Jumping", !Grounded());
    }

    void UpdateJumpVariables()
    {
        if (Grounded())
        {
            pState.jumping = false;
            coyoteTimeCounter = coyoteTime;
            airJumpCounter = 0;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump"))
            jumpBufferCounter = jumpBufferFrames;
        else
            jumpBufferCounter--;
    }

    private void OnDrawGizmos()
    {
        if (sideAttackTransform == null || upAttackTransform == null || downAttackTransform == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(sideAttackTransform.position, sideAttackArea);
        Gizmos.DrawWireCube(upAttackTransform.position, upAttackArea);
        Gizmos.DrawWireCube(downAttackTransform.position, downAttackArea);
    }
}

