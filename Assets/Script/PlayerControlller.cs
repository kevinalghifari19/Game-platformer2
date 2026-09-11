using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Horizontal Movement Settings")]
    [SerializeField] private float walkSpeed = 20f;
    [SerializeField] private float jumpForce = 45f;

    [Header("Ground Check Settings")]
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private float groundCheckY = 0.2f;
    [SerializeField] private float groundCheckX = 0.5f;
    [SerializeField] private LayerMask whatIsGround;

    private Rigidbody2D rb;
    private Animator anim;
    private float xAxis;

    // Singleton
    public static PlayerController Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        GetInputs();
        Move();
        Jump();
        Flip();
        UpdateAnimations();
    }

    private void GetInputs()
    {
        xAxis = Input.GetAxisRaw("Horizontal");
    }

    private void Move()
    {
        rb.linearVelocity = new Vector2(walkSpeed * xAxis, rb.linearVelocity.y);

        // Animation
        anim.SetBool("Walking", rb.linearVelocity.x != 0 && Grounded());
    }

    private void Jump()
    {
        // Jump hanya saat di tanah
        if (Input.GetButtonDown("Jump") && Grounded())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        // Variable Jump Height
        if (Input.GetButtonUp("Jump") && rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
        }

        // Animation
        anim.SetBool("Jumping", !Grounded());
    }

    private bool Grounded()
    {
        return Physics2D.Raycast(groundCheckPoint.position, Vector2.down, groundCheckY, whatIsGround)
            || Physics2D.Raycast(groundCheckPoint.position + Vector3.right * groundCheckX, Vector2.down, groundCheckY, whatIsGround)
            || Physics2D.Raycast(groundCheckPoint.position + Vector3.left * groundCheckX, Vector3.down, groundCheckY, whatIsGround);
    }

    private void Flip()
    {
        if (xAxis < 0)
            transform.localScale = new Vector3(-1, transform.localScale.y, 1);
        else if (xAxis > 0)
            transform.localScale = new Vector3(1, transform.localScale.y, 1);
    }

    private void UpdateAnimations()
    {
        // Animasi sudah di-handle di Move() dan Jump()
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheckPoint == null) return;

        Gizmos.color = Color.green;

        Gizmos.DrawLine(groundCheckPoint.position,
            groundCheckPoint.position + Vector3.down * groundCheckY);

        Gizmos.DrawLine(groundCheckPoint.position + Vector3.right * groundCheckX,
            groundCheckPoint.position + Vector3.right * groundCheckX + Vector3.down * groundCheckY);

        Gizmos.DrawLine(groundCheckPoint.position + Vector3.left * groundCheckX,
            groundCheckPoint.position + Vector3.left * groundCheckX + Vector3.down * groundCheckY);
    }
}