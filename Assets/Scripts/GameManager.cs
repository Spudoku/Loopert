using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static void StartSequence()
    {
        // show tutorial UI
    }

    public static IEnumerator EndSequence()
    {
        Debug.Log($"[Portal.EndSequence] started end sequence!");
        yield return null;
        Application.Quit();
    }
}
