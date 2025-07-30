using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Animator animator;
    Rigidbody2D rb;

    Collider2D collider;

    [SerializeField] float maxSpeed;
    [SerializeField] float acceleration;
    [SerializeField] float jumpStrength;

    [SerializeField] float gravityScale = 2f;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float horizontal = Input.GetAxis("Horizontal") * acceleration;

        rb.AddForce(new(horizontal, 0), ForceMode2D.Force);
        rb.linearVelocity


    }
}
