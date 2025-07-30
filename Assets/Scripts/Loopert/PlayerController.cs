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

    private void Awake()
    {
        isFacingRight = true;
        rb = GetComponent<Rigidbody2D>();
    }


    private void FixedUpdate()
    {
        CollisionChecks();

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
}
