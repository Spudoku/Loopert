using UnityEngine;

public class Brickguy : MonoBehaviour
{
    [Header("Movement Stats")]
    [SerializeField] private float acceleration;
    [SerializeField] private float maxSpeed;

    [SerializeField] private Transform target;

    [Header("AI")]
    [SerializeField] private float sightDistance = 10f;
    private Rigidbody2D rb;

    private bool isGrounded;

    private Vector2 moveVel;




    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }


    void Update()
    {

    }

    void FixedUpdate()
    {
        if (Vector2.Distance(transform.position, target.position) < sightDistance)
        {
            // Move towards target
            Vector2 dir = target.position - transform.position;

            Vector2 targetVel = new Vector2(Mathf.Sign(dir.x), 0) * maxSpeed;
            moveVel = Vector2.Lerp(moveVel, targetVel, acceleration * Time.fixedDeltaTime);
            rb.linearVelocity = new(moveVel.x, rb.linearVelocity.y);


        }

    }

    public void Die()
    {
        Destroy(gameObject);
    }
}
