using UnityEngine;


public class DocMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float jumpStrenght;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private AudioSource jumpSound, impactSound;
    private Rigidbody2D docBody;
    private Animator animatior;
    private CapsuleCollider2D capsuleCollider;
    private bool facingRight = true;
    private float groundSpeed, airSpeed;
    private bool doImpact = false;

    private void Awake()
    {
        docBody = GetComponent<Rigidbody2D>();
        animatior = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
    }

    private void Start()
    {
        groundSpeed = speed;
        airSpeed = speed / 1.5f;
    }

    private void Update()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");

        // moving the player left and right
        docBody.velocity = new Vector3(horizontalInput * speed, docBody.velocity.y, 0);


        // making the player sprite face left or right
        if ((horizontalInput > 0.01f && !facingRight) || (horizontalInput < -0.01f && facingRight))
        {
            flip();
        }

        // set parameters for walking and jumping animation
        animatior.SetBool("walk", horizontalInput != 0);
        animatior.SetBool("grounded", isGrounded());

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
        animatior.SetTrigger("jump");
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
