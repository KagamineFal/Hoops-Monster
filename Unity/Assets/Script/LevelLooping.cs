using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLooping : MonoBehaviour
{
    public float sceneDuration = 7f; // Durasi setiap scene

    private Coroutine sceneLoopCoroutine;

    void Start()
    {
        sceneLoopCoroutine = StartCoroutine(SceneLoop());
    }

    IEnumerator SceneLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(sceneDuration);

            int nextSceneIndex = (SceneManager.GetActiveScene().buildIndex + 1) % 2; // hanya 0 dan 1 (scene1 dan scene2)
            SceneManager.LoadScene(nextSceneIndex);
        }
    }

    public void StopSceneLoop()
    {
        if (sceneLoopCoroutine != null)
        {
            StopCoroutine(sceneLoopCoroutine);
            sceneLoopCoroutine = null;
            UnityEngine.Debug.Log("Loop berhenti!");
        }
    }
}
