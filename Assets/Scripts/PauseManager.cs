using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public GameObject pausePanel;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
        }
    }

    public void Pause()
    {
        Time.timeScale = 0;
        pausePanel.SetActive(true);
    }

    public void Resume()
    {
        Time.timeScale = 1;
        pausePanel.SetActive(false);
    }

    public void GoToMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("PlayMenu");
    }
}

