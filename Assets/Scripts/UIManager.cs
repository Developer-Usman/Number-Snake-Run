using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    [SerializeField] private GameObject gameOverScreen, gameFinishScreen;
    void Awake()
    {
        Instance = this;
        Time.timeScale = 1f;
    }
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void GameOver()
    {
        AudioManager.Instance.PlayGameOver();
        StartCoroutine(GameOverDelay());
    }
    IEnumerator GameOverDelay()
    {
        yield return new WaitForSecondsRealtime(1f);
        gameOverScreen.SetActive(true);
        Time.timeScale = 0f;
    }

    public void GameComplete()
    {
        AudioManager.Instance.PlayFinish();
        StartCoroutine(GameCompleteDelay());
    }
    IEnumerator GameCompleteDelay()
    {
        yield return new WaitForSecondsRealtime(1f);
        gameFinishScreen.SetActive(true);
        Time.timeScale = 0f;
    }
}
