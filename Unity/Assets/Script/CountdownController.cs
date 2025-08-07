using UnityEngine;
using UnityEngine.Video;

public class CountdownManager : MonoBehaviour
{
    public VideoPlayer countdownVideo;
    public GameObject canvasGameplay; // Canvas yang berisi score dan timer

    void Start()
    {
        // Pastikan canvas gameplay disembunyikan saat awal
        if (canvasGameplay != null)
            canvasGameplay.SetActive(false);

        // Tambahkan listener untuk event ketika video selesai
        if (countdownVideo != null)
            countdownVideo.loopPointReached += OnCountdownFinished;
    }

    void OnCountdownFinished(VideoPlayer vp)
    {
        // Matikan video player (bisa dengan disable GameObject-nya atau komponen-nya)
        vp.gameObject.SetActive(false);

        // Tampilkan canvas gameplay
        if (canvasGameplay != null)
            canvasGameplay.SetActive(true);
    }
}
