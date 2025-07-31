using UnityEngine;

public class Brickguy : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private GameObject target;

    private Rigidbody2D rb;

    private bool isGrounded;




    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }


    void Update()
    {

    }

    void FixedUpdate()
    {
        // Move towards target
    }
}
