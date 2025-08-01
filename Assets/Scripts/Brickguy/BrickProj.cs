using System.Collections;
using UnityEngine;

public class BrickProj : MonoBehaviour
{


    [SerializeField] private Collider2D trigger;
    [SerializeField] private Collider2D mainCollider;

    [SerializeField] private float intangibilityTime = 0.5f;
    public GameObject owner;
    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject != owner)
        {
            StartCoroutine(BrickDie());
        }

    }

    private IEnumerator BrickDie()
    {
        yield return new WaitForSeconds(1f);
        trigger.enabled = false;
        Destroy(gameObject);
    }

    private void Update()
    {
        if (transform.position.y < PlayerController.yToDie)
        {
            StartCoroutine(BrickDie());
        }
    }

    public void Intangibility()
    {
        mainCollider.enabled = false;
        StartCoroutine(TimedIntangibility(intangibilityTime));
    }

    private IEnumerator TimedIntangibility(float time)
    {
        yield return new WaitForSeconds(time);

        mainCollider.enabled = true;
    }
}
