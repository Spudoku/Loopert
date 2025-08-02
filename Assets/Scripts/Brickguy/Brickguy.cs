using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;

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
        Vector2 startPos = (target.position - transform.position).normalized + transform.position;
        Vector2 throwForce = CalculateTrajectory(target.position, startPos, throwStrength);

        // Can't reach target
        if (throwForce == Vector2.zero)
        {
            Debug.Log($"[BrickGuy.Throw] can't throw brick; no valid launch angle!");
            isThrowing = false;
            return;
        }


        Debug.Log($"[BrickGuy.Throw] throwing brick!");



        GameObject newBrick = Instantiate(brickPrefab);
        newBrick.transform.position = startPos;

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
    private Vector2 CalculateTrajectory(Vector2 targetPosition, Vector2 startPosition, float force)
    {
        Vector2 dir = targetPosition - startPosition;

        float y = dir.y;

        dir.y = 0f;

        float x = dir.magnitude;

        float g = -Physics2D.gravity.y;

        float sSqr = force * force;

        float discriminant = (sSqr * sSqr) - g * (g * x * x + 2 * y * sSqr);
        Debug.Log($"[BrickGuy.CalculateTrajectory] discriminant: {discriminant}");

        if (discriminant >= 0f)
        {
            float root = Mathf.Sqrt(discriminant);
            Debug.Log($"[BrickGuy.CalculateTrajectory] root: {root}");
            float tanTheta = (sSqr - root) / 2;

            float theta = Mathf.Atan2(tanTheta, g * x);

            float vX = force * Mathf.Cos(theta);
            float vY = force * Mathf.Sin(theta);
            return new Vector2(vX, vY);
        }
        return Vector2.zero;

    }
}
