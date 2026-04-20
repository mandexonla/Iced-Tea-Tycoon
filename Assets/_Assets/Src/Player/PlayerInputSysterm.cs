using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputSysterm : MonoBehaviour
{
    [SerializeField] private float PlayerMoveSpeed = 5f;

    private Rigidbody2D rb;
    private Vector2 PlayerMoveInput;
    private Vector2 playerMoveInput;
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        rb.linearVelocity = PlayerMoveInput * PlayerMoveSpeed;

    }

    public void PlayerMovement(InputAction.CallbackContext context)
    {
        PlayerMoveInput = context.ReadValue<Vector2>();
        playerMoveInput = context.ReadValue<Vector2>();

        animator.SetBool("isWalking", true);

        if (context.canceled)
        {
            animator.SetBool("isWalking", false);
            animator.SetFloat("LastInputX", animator.GetFloat("InputX"));
            animator.SetFloat("LastInputY", animator.GetFloat("InputY"));
        }

        animator.SetFloat("InputX", playerMoveInput.x);
        animator.SetFloat("InputY", playerMoveInput.y);
    }
}