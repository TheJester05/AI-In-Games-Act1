using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;

    private CharacterController controller;
    private Animator animator;
    private Vector3 velocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(x, 0f, z);

        if (move.magnitude > 1f)
            move.Normalize();

        controller.Move(move * moveSpeed * Time.deltaTime);

        // Rotate toward movement
        if (move != Vector3.zero)
        {
            transform.forward = move;
        }

        // Set animation speed
        animator.SetFloat("Speed", move.magnitude);

        // Ground check
        bool grounded = controller.isGrounded;
        animator.SetBool("IsGrounded", grounded);

        if (grounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Jump
        if (Input.GetButtonDown("Jump") && grounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            animator.SetTrigger("Jump");
        }

        // Gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}