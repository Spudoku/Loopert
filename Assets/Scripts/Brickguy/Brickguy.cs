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
    [SerializeField] private float throwWindup = 1f;
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
            StopAllCoroutines();
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
                StartCoroutine(ThrowWindup());
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

    private IEnumerator ThrowWindup()
    {
        throwTimer = throwCooldown + throwWindup;
        yield return new WaitForSeconds(throwWindup);
        Throw();

    }
    private void Throw()
    {
        Debug.Log($"[BrickGuy.Throw] throwing brick!");



        GameObject newBrick = Instantiate(brickPrefab);
        newBrick.transform.position = transform.position;

        BrickProj brickProj = newBrick.GetComponent<BrickProj>();
        brickProj.owner = gameObject;

        brickProj.Intangibility();


        // throw in general direction of target
        Rigidbody2D brickRB = newBrick.GetComponent<Rigidbody2D>();
        Vector2 throwForce = CalculateTrajectory(target.position, throwStrength);

        brickRB.AddForce(throwForce, ForceMode2D.Impulse);

    }

    private Vector2 CalculateTrajectory(Vector2 targetPosition, float force)
    {
        // TODO: factor for trajectory
        float t = 0f;
        return (target.position - transform.position).normalized * force;
    }
}
