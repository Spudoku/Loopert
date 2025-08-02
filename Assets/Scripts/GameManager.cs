using System.Collections;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject menu;
    [SerializeField] bool isPaused = true;
    public void StartSequence()
    {
        // show tutorial UI
        SetMenu(true);
    }



    private void Update()
    {
        if (InputManager.PauseWasPressed)
        {
            ToggleMenu();
        }

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

    public void ToggleMenu()
    {
        isPaused = !isPaused;
        SetMenu(isPaused);
    }

    private void SetMenu(bool paused)
    {
        menu.SetActive(paused);
    }
}
