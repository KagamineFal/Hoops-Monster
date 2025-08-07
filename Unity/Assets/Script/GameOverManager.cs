using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public float delayBeforeRestart = 6f;
    public string nextSceneName = "Loop"; // Ganti sesuai nama scene-mu

    void Start()
    {
        Invoke("LoadNextScene", delayBeforeRestart);
    }

    void LoadNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}
