using System.Collections;
using UnityEngine;

public class BrickProj : MonoBehaviour
{


    [SerializeField] private Collider2D trigger;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        StartCoroutine(BrickDie());
    }

    private IEnumerator BrickDie()
    {
        yield return new WaitForSeconds(PlayerController.DeathTime + 0.05f);
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
}
