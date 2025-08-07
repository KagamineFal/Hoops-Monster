using UnityEngine;
using UnityEngine.SceneManagement;

public class BGMManager : MonoBehaviour
{
    private static BGMManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // BGM tidak dihancurkan saat pindah scene
        }
        else
        {
            Destroy(gameObject); // Hindari duplikat BGM jika scene1 di-reload
        }
    }

    void Update()
    {
        // Hentikan BGM jika masuk scene3 (misalnya index = 2)
        if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            Destroy(gameObject);
        }
    }
}
