using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Animations;

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


    bool isThrowing = false;

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

            if (throwTimer < 0 && !isThrowing)
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
        isThrowing = true;
        throwTimer = throwCooldown + throwWindup;
        yield return new WaitForSeconds(throwWindup);
        Throw();

    }
    private void Throw()
    {
        Vector2 throwForce = CalculateTrajectory(target.position, throwStrength);

        // Can't reach target
        if (throwForce == Vector2.zero)
        {
            Debug.Log($"[BrickGuy.Throw] can't throw brick; no valid launch angle!");
            return;
        }


        Debug.Log($"[BrickGuy.Throw] throwing brick!");



        GameObject newBrick = Instantiate(brickPrefab);
        newBrick.transform.position = transform.position;

        BrickProj brickProj = newBrick.GetComponent<BrickProj>();
        brickProj.owner = gameObject;

        brickProj.Intangibility();


        // throw in general direction of target
        Rigidbody2D brickRB = newBrick.GetComponent<Rigidbody2D>();

        brickRB.AddForce(throwForce, ForceMode2D.Impulse);

        isThrowing = false;
    }

    // trajectory equation
    // returns Vector2.zero if no valid trajectory
    // code based on https://learn.unity.com/tutorial/calculating-trajectories
    private Vector2 CalculateTrajectory(Vector2 targetPosition, float force)
    {

        float deltaX = targetPosition.x - transform.position.x;
        float deltaY = targetPosition.y - transform.position.y;
        Debug.Log($"[Brickguy.CalculateTrajectory] deltaX: {deltaX}; deltaY: {deltaY}");

        if (Mathf.Abs(deltaX) < 0.01f)
        {
            return new Vector2(0, force);
        }

        float vSquared = force * force;
        float g = -Physics.gravity.y;

        // solving for launch angle theta
        float a = 1;
        float b = (-2 * vSquared) / (g * deltaX);
        float c = (2 * vSquared * deltaY + (g * deltaX * deltaX)) / (g * deltaX * deltaX);


        float discriminant = b * b - 4 * a * c;
        // no valid angle
        if (discriminant < 0)
        {
            return Vector2.zero;
        }

        // solving the quadratic

        float tanTheta = (-b + Mathf.Sqrt(discriminant)) / (2 * a);

        // calculate atan2 using the slope
        float theta = Mathf.Atan2(tanTheta * Mathf.Abs(deltaX), Mathf.Abs(deltaX));



        if (deltaX < 0)
        {
            theta += Mathf.PI;
        }

        float vX = force * Mathf.Cos(theta);
        float vY = force * Mathf.Sin(theta);

        return new Vector2(vX, vY);
    }
}
