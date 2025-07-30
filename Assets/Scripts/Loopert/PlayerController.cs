using UnityEngine;

// // Code based on https://www.youtube.com/watch?v=zHSWG05byEc
public class PlayerController : MonoBehaviour
{
    Animator animator;
    Rigidbody2D rb;

    [Header("References")]
    public PlayerMoveStats MoveStats;

    [SerializeField] private Collider2D bodyColl;
    [SerializeField] private Collider2D footColl;

    private Vector2 moveVel;
    private bool isFacingRight;

    private RaycastHit2D groundHit;
    private RaycastHit2D headHit;
    private bool isGrounded;
    private bool bumpedHead;

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
    }

    private void Update()
    {
        CountTimers();
        JumpChecks();
    }

    private void FixedUpdate()
    {
        CollisionChecks();
        Jump();

        if (isGrounded)
        {
            Move(MoveStats.GroundAcceleration, MoveStats.GroundDeceleration, InputManager.Movement);
        }
        else
        {
            Move(MoveStats.AirAcceleration, MoveStats.AirDeceleration, InputManager.Movement);
        }
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
        else if (jumpBufferTimer > 0f && isFalling && numberOfJumpsUsed < MoveStats.NumberOfJumpsAllowed)
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
        // if (!isJumping)
        // {

        // }

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
            if ()
            {

            }
            // apex controls

            // gravity on descending
        }


        // jump cut

        // normal gravity while falling

        // clamp fall speed

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

    private void CollisionChecks()
    {
        IsGrounded();
    }
    #endregion

    #region Timers
    private void CountTimers()
    {
        jumpBufferTimer -= Time.deltaTime;

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
