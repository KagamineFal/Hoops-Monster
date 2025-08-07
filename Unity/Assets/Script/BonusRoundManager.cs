using System.Diagnostics;
using UnityEngine;
using UnityEngine.Video;

public class BonusRoundManager : MonoBehaviour
{
    public VideoPlayer bonusVideo;
    public GameObject canvasGameplay;
    public GameObject bonusRoundBackgroundCanvas;
    public GameObject gameplayVideoPlayerObject;

    public BonusTimerController bonusTimerController; // Script baru untuk timer bonus round

    void Start()
    {
        if (bonusVideo != null)
            bonusVideo.gameObject.SetActive(false);

        if (bonusRoundBackgroundCanvas != null)
            bonusRoundBackgroundCanvas.SetActive(true);

        if (bonusTimerController != null)
            bonusTimerController.gameObject.SetActive(false);
    }

    public void PlayBonusRound()
    {
        UnityEngine.Debug.Log("PlayBonusRound() called!");

        if (bonusVideo != null)
        {
            bonusVideo.gameObject.SetActive(true);
            bonusVideo.Play();
            bonusVideo.loopPointReached += OnBonusVideoFinished;
        }

        if (canvasGameplay != null)
            canvasGameplay.SetActive(false);

        if (bonusRoundBackgroundCanvas != null)
            bonusRoundBackgroundCanvas.SetActive(false);

        if (bonusTimerController != null)
            bonusTimerController.gameObject.SetActive(false);
    }

    void OnBonusVideoFinished(VideoPlayer vp)
    {
        UnityEngine.Debug.Log("Bonus video selesai");

        if (gameplayVideoPlayerObject != null)
            gameplayVideoPlayerObject.SetActive(false);

        if (bonusRoundBackgroundCanvas != null)
            bonusRoundBackgroundCanvas.SetActive(true);

        if (bonusTimerController != null)
        {
            bonusTimerController.gameObject.SetActive(true);
            bonusTimerController.StartBonusRoundTimer();
        }

        // Jangan aktifkan canvasGameplay dulu di sini, nanti setelah bonus round selesai

        vp.gameObject.SetActive(false);
    }

    public void OnBonusRoundFinished()
    {
        // Dipanggil dari BonusTimerController ketika bonus round selesai

        if (bonusRoundBackgroundCanvas != null)
            bonusRoundBackgroundCanvas.SetActive(false);

        if (canvasGameplay != null)
            canvasGameplay.SetActive(true);
    }
}

