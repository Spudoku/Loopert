using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Brickguy : MonoBehaviour
{
    [Header("Movement Stats")]
    [SerializeField] private float acceleration;
    [SerializeField] private float maxSpeed;

    [SerializeField] private Transform target;

    [Header("AI")]
    [SerializeField] private float sightDistance = 10f;

    [Header("Brick-Throwing")]
    [SerializeField] private GameObject brickPrefab;
    [SerializeField] private float throwStrength;
    [SerializeField] private float throwCooldown;
    [SerializeField] private AudioClip throwSFX;

    private float throwTimer;

    private Rigidbody2D rb;

    private bool isGrounded;

    private Vector2 moveVel;




    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }


    private void Update()
    {
        if (transform.position.y < PlayerController.yToDie)
        {
            StartCoroutine(Die());
        }

        throwTimer -= Time.deltaTime;
    }

    void FixedUpdate()
    {
        if (Vector2.Distance(transform.position, target.position) < sightDistance)
        {

            if (throwTimer < 0)
            {
                Throw();
            }
            else
            {
                // Move towards target
                Vector2 dir = target.position - transform.position;

                Vector2 targetVel = new Vector2(Mathf.Sign(dir.x), 0) * maxSpeed;
                moveVel = Vector2.Lerp(moveVel, targetVel, acceleration * Time.fixedDeltaTime);
                rb.linearVelocity = new(moveVel.x, rb.linearVelocity.y);
            }



        }

    }

    public IEnumerator Die()
    {
        yield return null;
        Destroy(gameObject);
    }

    private void Throw()
    {
        throwTimer = throwCooldown;
    }
}
