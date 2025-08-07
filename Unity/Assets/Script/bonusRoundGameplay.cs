using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using System.Diagnostics;

public class bonusRoundGameplay : MonoBehaviour
{
    [Header("Video Player")]
    public VideoPlayer bonusVideo;           // VideoPlayer yang memutar MP4 bonus round

    [Header("Gameplay Canvas")]
    public GameObject canvasGameplay;        // UI gameplay utama

    [Header("Bonus UI")]
    public GameObject bonusUIPanel;          // Panel Canvas yang berisi RawImage bonus
    // (Di dalam bonusUIPanel, letakkan RawImage Anda)

    void Start()
    {
        // 1) Pastikan video & bonus UI tertutup di awal
        if (bonusVideo != null)
            bonusVideo.gameObject.SetActive(false);

        if (bonusUIPanel != null)
            bonusUIPanel.SetActive(false);
    }

    /// <summary>
    /// Panggil ini untuk mulai bonus round:
    /// - sembunyikan gameplay
    /// - play video
    /// </summary>
    public void PlayBonusRound()
    {
        UnityEngine.Debug.Log("PlayBonusRound() called!");

        // sembunyikan gameplay
        if (canvasGameplay != null)
            canvasGameplay.SetActive(false);

        // sembunyikan panel bonus UI sementara
        if (bonusUIPanel != null)
            bonusUIPanel.SetActive(false);

        // play videonya
        if (bonusVideo != null)
        {
            bonusVideo.gameObject.SetActive(true);
            bonusVideo.Play();
            bonusVideo.loopPointReached += OnBonusVideoFinished;
        }
    }

    /// <summary>
    /// Callback ketika bonus video selesai:
    /// - sembunyikan VideoPlayer
    /// - tampilkan gameplay kembali
    /// - tampilkan panel bonus UI dengan RawImage
    /// </summary>
    void OnBonusVideoFinished(VideoPlayer vp)
    {
        UnityEngine.Debug.Log("Bonus video selesai");

        // hide video
        vp.gameObject.SetActive(false);
        vp.loopPointReached -= OnBonusVideoFinished;

        // show gameplay UI
        if (canvasGameplay != null)
            canvasGameplay.SetActive(true);

        // show bonus UI panel (RawImage)
        if (bonusUIPanel != null)
            bonusUIPanel.SetActive(true);
    }
}
