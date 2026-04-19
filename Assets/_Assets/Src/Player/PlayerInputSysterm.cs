using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputSysterm : MonoBehaviour
{
    [SerializeField] private float playerMoveSpeed = 5f;
    [SerializeField] private string gameplayActionMapName = "Player";

    private Rigidbody2D rb;
    private Vector2 playerMoveInput;
    private Animator animator;
    private PlayerInput playerInput;
    private bool playingFootstep = false;
    private bool isInputLocked = false;
    private Coroutine deferredInputActivationRoutine;

    public float footstepSpeed = 0.5f;

    private void Awake()
    {
        CacheComponents();

        // Smooth out collision response against tile edges and moving colliders.
        if (rb != null)
        {
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }
    }

    private void OnEnable()
    {
        CacheComponents();
        isInputLocked = false;
        playerMoveInput = Vector2.zero;
        BeginDeferredInputActivation();
    }

    private void Start()
    {
        BeginDeferredInputActivation();
    }

    private void OnDisable()
    {
        if (deferredInputActivationRoutine != null)
        {
            StopCoroutine(deferredInputActivationRoutine);
            deferredInputActivationRoutine = null;
        }
    }

    private void Update()
    {
        //if (PauseController.IsGamePaused || isInputLocked)
        //{
        //    if (rb != null)
        //    {
        //        rb.linearVelocity = Vector2.zero;
        //    }

        //    if (animator != null)
        //    {
        //        animator.SetBool("isWalking", false);
        //    }
        //    StopFootsteps();
        //    return;
        //}

        bool isMoving = playerMoveInput.sqrMagnitude > 0.0001f;
        if (animator != null)
        {
            animator.SetBool("isWalking", isMoving);
        }

        //if (isMoving && !playingFootstep)
        //{
        //    StartFootsteps();
        //}
        //else if (!isMoving)
        //{
        //    StopFootsteps();
        //}
    }

    private void FixedUpdate()
    {
        if (rb == null)
        {
            return;
        }

        //if (PauseController.IsGamePaused || isInputLocked)
        //{
        //    rb.linearVelocity = Vector2.zero;
        //    return;
        //}

        rb.linearVelocity = Vector2.ClampMagnitude(playerMoveInput, 1f) * playerMoveSpeed;
    }

    public void PlayerMovement(InputAction.CallbackContext context)
    {
        CacheComponents();

        if (isInputLocked)
        {
            return;
        }

        playerMoveInput = context.ReadValue<Vector2>();

        if (animator == null)
        {
            return;
        }

        if (context.canceled)
        {
            animator.SetBool("isWalking", false);
            animator.SetFloat("LastInputX", animator.GetFloat("InputX"));
            animator.SetFloat("LastInputY", animator.GetFloat("InputY"));
        }

        animator.SetFloat("InputX", playerMoveInput.x);
        animator.SetFloat("InputY", playerMoveInput.y);
    }

    public void SetInputLocked(bool locked)
    {
        CacheComponents();
        isInputLocked = locked;
        playerMoveInput = Vector2.zero;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        if (animator != null)
        {
            animator.SetBool("isWalking", false);
            animator.SetFloat("LastInputX", animator.GetFloat("InputX"));
            animator.SetFloat("LastInputY", animator.GetFloat("InputY"));
        }

        //StopFootsteps();

        if (playerInput != null)
        {
            if (locked)
            {
                playerInput.DeactivateInput();
            }
            else
            {
                playerInput.ActivateInput();

                if (!string.IsNullOrWhiteSpace(gameplayActionMapName))
                {
                    playerInput.SwitchCurrentActionMap(gameplayActionMapName);
                }
            }
        }
    }

    private void CacheComponents()
    {
        rb ??= GetComponent<Rigidbody2D>();
        animator ??= GetComponent<Animator>();
        playerInput ??= GetComponent<PlayerInput>();
    }

    private void BeginDeferredInputActivation()
    {
        if (!isActiveAndEnabled)
        {
            return;
        }

        if (deferredInputActivationRoutine != null)
        {
            StopCoroutine(deferredInputActivationRoutine);
        }

        deferredInputActivationRoutine = StartCoroutine(EnsureGameplayInputReady());
    }

    private IEnumerator EnsureGameplayInputReady()
    {
        yield return null;
        ActivateGameplayInput();

        yield return null;
        ActivateGameplayInput();

        deferredInputActivationRoutine = null;
    }

    private void ActivateGameplayInput()
    {
        CacheComponents();

        if (playerInput == null || !playerInput.isActiveAndEnabled)
        {
            return;
        }

        playerInput.ActivateInput();

        if (!string.IsNullOrWhiteSpace(gameplayActionMapName))
        {
            playerInput.SwitchCurrentActionMap(gameplayActionMapName);
        }
    }

    //void StartFootsteps()
    //{
    //    playingFootstep = true;
    //    InvokeRepeating(nameof(PlayFootstep), 0f, footstepSpeed);
    //}

    //void StopFootsteps()
    //{
    //    playingFootstep = false;
    //    CancelInvoke(nameof(PlayFootstep));
    //}

    //void PlayFootstep()
    //{
    //    SoundEffectManager.Play("Footstep", true);
    //}
}