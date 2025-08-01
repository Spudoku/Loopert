using System.Collections;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private static GameObject menu;
    static bool isPaused = true;
    public static void StartSequence()
    {
        // show tutorial UI
        SetMenu(true);
    }

    public static void ToggleMenu()
    {
        isPaused = !isPaused;
        SetMenu(isPaused);
    }

    private static void SetMenu(bool paused)
    {
        menu.SetActive(paused);
    }

    private void Update()
    {
        if (isPaused)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }

    public static IEnumerator EndSequence()
    {
        Debug.Log($"[Portal.EndSequence] started end sequence!");
        yield return null;
        Application.Quit();
    }
}
