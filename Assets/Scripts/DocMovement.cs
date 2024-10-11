using UnityEngine;


public class DocMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float jumpStrenght;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private AudioSource jumpSound, impactSound;
    private Rigidbody2D docBody;
    private Animator animator;
    private CapsuleCollider2D capsuleCollider;
    private bool facingRight = true;
    private float groundSpeed, airSpeed;
    private bool doImpact = false;
    private bool canMove = true;
    public bool CanMove
    {
        get { return canMove; }
        set { canMove = value; }
    }

    void Awake()
    {
        docBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
    }

    void Start()
    {
        groundSpeed = speed;
        airSpeed = speed / 1.5f;
    }

    void Update()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");

        // moving the player left and right
        if (canMove)
        {
            docBody.velocity = new Vector3(horizontalInput * speed, docBody.velocity.y, 0);
            animator.SetBool("walk", horizontalInput != 0); // set parameters for walking animation
        }
        else
        {
            docBody.velocity = new Vector3(0, docBody.velocity.y, 0);
            animator.SetBool("walk", false); // set parameters to stop walking animation
        }

        // making the player sprite face left or right
        if ((horizontalInput > 0.01f && !facingRight) || (horizontalInput < -0.01f && facingRight))
        {
            flip();
        }

        // set parameters for jumping animation
        animator.SetBool("grounded", isGrounded());

        // jump
        if ((Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) && isGrounded())
        {
            jump();
        }

        // reduce speed during air time and manage the impact sound
        if (isGrounded())
        {
            groundImpact();
            doImpact = false;
            speed = groundSpeed;
        }
        else
        {
            doImpact = true;
            speed = airSpeed;
        }
    }

    private void flip()
    {
        facingRight = !facingRight;
        transform.Rotate(0f, 180f, 0f);
    }

    private void jump()
    {
        jumpSound.Play();
        animator.SetTrigger("jump");
        docBody.velocity = Vector2.up * jumpStrenght;
    }

    // check if the player is grounded using raycast with boxes
    private bool isGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(capsuleCollider.bounds.center, capsuleCollider.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
        return raycastHit.collider != null;
    }

    private void groundImpact()
    {
        if (doImpact)
        {
            impactSound.Play();
        }
    }
}
