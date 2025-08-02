using System.Collections;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] AudioSource winJingle;
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(EndSequence());
        }
    }

    public IEnumerator EndSequence()
    {
        Debug.Log($"[Portal.EndSequence] started end sequence!");
        Time.timeScale = 0f;
        winJingle.Play();
        yield return new WaitForSeconds(winJingle.clip.length + 1f);
        Application.Quit();
    }


}
