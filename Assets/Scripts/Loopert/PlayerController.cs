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

    #region Movement

    private void Move(float acceleration, float deceleration, Vector2 moveInput)
    {
        if (moveInput != Vector2.zero)
        {
            // check if turn is needed

            Vector2 targetVelocity = new Vector2(moveInput.x, 0) * MoveStats.MaxWalkSpeed;

            // moveVel = 




        }
    }


    #endregion
}
