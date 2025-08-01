using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// // Code based on https://www.youtube.com/watch?v=zHSWG05byEc
public class PlayerController : MonoBehaviour
{
    Animator animator;
    Rigidbody2D rb;

    [Header("References")]
    public PlayerMoveStats MoveStats;

    [SerializeField] private Collider2D bodyColl;
    [SerializeField] private Collider2D footColl;

    [SerializeField] private bool isGrounded;

    [Header("Slide-related")]

    [SerializeField] private Collider2D slideColl;

    [SerializeField] private float slideStun = 0.5f;

    [SerializeField] private float stunTimer;

    [Header("Level Interaction")]
    public static float yToDie = -10f;
    public static float DeathTime = 1.5f;
    // misc vars
    private bool bumpedHead;

    private Vector2 moveVel;
    private bool isFacingRight;

    private RaycastHit2D groundHit;
    private RaycastHit2D headHit;

    // Jump Vars
    public float VerticalVelocity { get; private set; }

    private bool isJumping;
    private bool isFastFalling;
    private bool isFalling;
    private float fastFallTime;
    private float fastFallReleaseSpeed;
    private int numberOfJumpsUsed;

    // apex vars
    private float apexPoint;
    private float timePastApexThreshold;
    private bool isPastApexThreshold;

    // jump buffer vars
    private float jumpBufferTimer;
    private bool jumpReleasedDuringBuffer;

    // coyote time vars
    private float coyoteTimer;

    private void Awake()
    {
        isFacingRight = true;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        CountTimers();
        if (stunTimer <= 0)
        {
            JumpChecks();
            animator.SetBool("isStunned", false);
        }
        else
        {
            animator.SetBool("isStunned", true);
            return;
        }

        if (InputManager.SlideIsHeld && stunTimer <= 0)
        {
            animator.SetBool("isSliding", true);
            animator.SetBool("isMoving", false);
            animator.SetBool("isJumping", false);
            slideColl.enabled = true;
            bodyColl.enabled = false;
        }
        else
        {
            animator.SetBool("isSliding", false);
            slideColl.enabled = false;
            bodyColl.enabled = true;

        }

        if (InputManager.SlideWasReleased)
        {
            // Set Stun to True
            animator.SetBool("isMoving", false);
            animator.SetBool("isJumping", false);
            slideColl.enabled = false;
            bodyColl.enabled = true;
            stunTimer = slideStun;
        }

        if (InputManager.Movement != Vector2.zero)
        {
            animator.SetBool("isMoving", true);

        }
        else
        {
            animator.SetBool("isMoving", false);
        }

        animator.SetBool("isJumping", isJumping);
    }

    private void FixedUpdate()
    {
        CollisionChecks();
        Jump();

        if (isGrounded)
        {
            if (!InputManager.SlideIsHeld && stunTimer <= 0)
            {
                Move(MoveStats.GroundAcceleration, MoveStats.GroundDeceleration, InputManager.Movement);
            }

        }
        else
        {
            if (!InputManager.SlideIsHeld && stunTimer <= 0)
            {
                Move(MoveStats.AirAcceleration, MoveStats.AirDeceleration, InputManager.Movement);
            }

        }

        if (transform.position.y < yToDie)
        {
            StartCoroutine(Die());
        }

    }

    private IEnumerator Die()
    {
        // TODO: play sound effect, etc
        animator.speed = 0;
        Rigidbody2D[] rigidBodies = Object.FindObjectsByType<Rigidbody2D>(FindObjectsSortMode.None);

        foreach (Rigidbody2D rb in rigidBodies)
        {
            if (!rb.gameObject.TryGetComponent<BrickProj>(out _))
            {
                rb.constraints = RigidbodyConstraints2D.FreezeAll;
            }

        }
        yield return new WaitForSeconds(DeathTime);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    #region Movement



    private void Move(float acceleration, float deceleration, Vector2 moveInput)
    {
        if (moveInput != Vector2.zero)
        {
            TurnCheck(moveInput);

            Vector2 targetVelocity = new Vector2(moveInput.x, 0) * MoveStats.MaxWalkSpeed;

            moveVel = Vector2.Lerp(moveVel, targetVelocity, acceleration * Time.fixedDeltaTime);
            rb.linearVelocity = new Vector2(moveVel.x, rb.linearVelocity.y);
        }
        else if (moveInput == Vector2.zero)
        {
            moveVel = Vector2.Lerp(moveVel, Vector2.zero, deceleration * Time.fixedDeltaTime);
            rb.linearVelocity = new Vector2(moveVel.x, rb.linearVelocity.y);
        }
    }

    private void TurnCheck(Vector2 moveInput)
    {
        if (isFacingRight && moveInput.x < 0)
        {
            Turn(false);
        }
        else if (!isFacingRight && moveInput.x > 0)
        {
            Turn(true);
        }
    }

    private void Turn(bool turnRight)
    {
        if (turnRight)
        {
            isFacingRight = true;
            transform.Rotate(0f, 180f, 0f);
        }
        else
        {
            isFacingRight = false;
            transform.Rotate(0f, -180f, 0f);
        }
    }


    #endregion

    #region Jump
    private void JumpChecks()
    {
        // when we press the jump button
        if (InputManager.JumpWasPressed)
        {
            jumpBufferTimer = MoveStats.JumpBufferTime;
            jumpReleasedDuringBuffer = false;
        }
        // when jump button released
        if (InputManager.JumpWasReleased)
        {
            if (jumpBufferTimer > 0f)
            {
                jumpReleasedDuringBuffer = true;
            }

            if (isJumping && VerticalVelocity > 0f)
            {
                if (isPastApexThreshold)
                {
                    isPastApexThreshold = false;
                    isFastFalling = true;
                    fastFallTime = MoveStats.TimeForUpwardsCancel;
                    VerticalVelocity = 0f;
                }
                else
                {
                    isFastFalling = true;
                    fastFallReleaseSpeed = VerticalVelocity;
                }
            }
        }
        // initiate jump w/ jump buffering and coyote time
        if (jumpBufferTimer > 0f && !isJumping && (isGrounded || coyoteTimer > 0f))
        {
            InitiateJump(1);

            if (jumpReleasedDuringBuffer)
            {
                isFastFalling = true;
                fastFallReleaseSpeed = VerticalVelocity;
            }
        }
        // double jump
        else if (jumpBufferTimer > 0f && isJumping && numberOfJumpsUsed < MoveStats.NumberOfJumpsAllowed)
        {
            isFastFalling = false;
            InitiateJump(1);
        }
        // handle air jump after coyote time
        else if (jumpBufferTimer > 0f && isFalling && numberOfJumpsUsed < MoveStats.NumberOfJumpsAllowed - 1)
        {
            InitiateJump(MoveStats.NumberOfJumpsAllowed - numberOfJumpsUsed);
            isFastFalling = false;
        }
        // landed
        if ((isJumping || isFalling) && isGrounded && VerticalVelocity <= 0f)
        {
            isJumping = false;
            isFalling = false;
            isFastFalling = false;
            fastFallTime = 0f;
            isPastApexThreshold = false;
            numberOfJumpsUsed = 0;

            VerticalVelocity = Physics2D.gravity.y;
        }
    }

    private void InitiateJump(int _numberOfJumpsUsed)
    {
        isJumping = true;

        jumpBufferTimer = 0f;

        numberOfJumpsUsed += _numberOfJumpsUsed;
        VerticalVelocity = MoveStats.InitialJumpVelocity;
    }

    private void Jump()
    {
        // apply gravity while jumping
        if (isJumping)
        {
            // check for head bump
            if (bumpedHead)
            {
                isFastFalling = true;
            }

            // gravity on ascending
            if (VerticalVelocity >= 0f)
            {
                // apex controls
                apexPoint = Mathf.InverseLerp(MoveStats.InitialJumpVelocity, 0f, VerticalVelocity);

                if (apexPoint > MoveStats.ApexThreshold)
                {

                    if (!isPastApexThreshold)
                    {
                        isPastApexThreshold = true;
                        timePastApexThreshold = 0f;
                    }

                    if (isPastApexThreshold)
                    {
                        timePastApexThreshold += Time.fixedDeltaTime;

                        if (timePastApexThreshold < MoveStats.ApexHangTime)
                        {
                            VerticalVelocity = 0f;
                        }
                        else
                        {
                            VerticalVelocity = -0.01f;
                        }
                    }
                }
                // gravity on ascending but not past apex threshold
                else
                {
                    VerticalVelocity += MoveStats.Gravity * Time.fixedDeltaTime;
                    if (isPastApexThreshold)
                    {
                        isPastApexThreshold = false;
                    }
                }
            }

            // gravity on descending

            else if (!isFastFalling)
            {
                VerticalVelocity += MoveStats.Gravity * MoveStats.GravityOnReleaseMultiplier * Time.fixedDeltaTime;
            }

            else if (VerticalVelocity < 0f)
            {
                if (!isFalling)
                {
                    isFalling = true;
                }
            }

        }
        // jump cut
        if (isFastFalling)
        {
            if (fastFallTime >= MoveStats.TimeForUpwardsCancel)
            {
                VerticalVelocity += MoveStats.Gravity * MoveStats.GravityOnReleaseMultiplier * Time.fixedDeltaTime;
            }
            else if (fastFallTime < MoveStats.TimeForUpwardsCancel)
            {
                VerticalVelocity = Mathf.Lerp(fastFallReleaseSpeed, 0f, (fastFallTime / MoveStats.TimeForUpwardsCancel));
            }

            fastFallTime += Time.deltaTime;
        }
        // normal gravity while falling
        if (!isGrounded && !isJumping)
        {
            if (!isFalling)
            {
                isFalling = true;
            }

            VerticalVelocity += MoveStats.Gravity * Time.fixedDeltaTime;
        }
        // clamp fall speed
        VerticalVelocity = Mathf.Clamp(VerticalVelocity, -MoveStats.MaxFallSpeed, 100f);

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, VerticalVelocity);
    }
    #endregion


    #region Collision Checks
    private void IsGrounded()
    {
        Vector2 boxCastOrigin = new Vector2(footColl.bounds.center.x, footColl.bounds.min.y);

        Vector2 boxCastSize = new Vector2(footColl.bounds.size.x, MoveStats.GroundDetectionRayLength);

        groundHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, Vector2.down, MoveStats.GroundDetectionRayLength, MoveStats.GroundLayer);


        isGrounded = groundHit.collider != null;

    }

    private void BumpedHead()
    {
        Vector2 boxCastOrigin = new Vector2(footColl.bounds.center.x, bodyColl.bounds.max.y);
        Vector2 boxCastSize = new Vector2(footColl.bounds.size.x * MoveStats.HeadWidth, MoveStats.HeadDetectionRayLength);

        headHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, Vector2.up, MoveStats.HeadDetectionRayLength, MoveStats.GroundLayer);

        if (headHit.collider != null)
        {
            bumpedHead = true;
        }
        else
        {
            bumpedHead = false;
        }
        // If i wanted to implement debug visualization, go to https://youtu.be/zHSWG05byEc?t=1014
    }

    private void CollisionChecks()
    {
        IsGrounded();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"[PlayerController.OnTriggerEnter2D] touching {collision}");
        Brickguy enemy = collision.gameObject.GetComponentInParent<Brickguy>();
        if (enemy != null)
        {
            StartCoroutine(enemy.Die());
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (InputManager.SlideIsHeld && stunTimer <= 0)
        {
            return;
        }
        Brickguy enemy = collision.gameObject.GetComponentInParent<Brickguy>();

        if (enemy != null || collision.gameObject.CompareTag("enemy"))
        {
            StartCoroutine(Die());
        }


    }


    #endregion

    #region Timers
    private void CountTimers()
    {
        jumpBufferTimer -= Time.deltaTime;

        stunTimer -= Time.deltaTime;

        if (!isGrounded)
        {
            coyoteTimer -= Time.deltaTime;
        }
        else
        {
            coyoteTimer = MoveStats.JumpCoyoteTime;
        }
    }
    #endregion
}
