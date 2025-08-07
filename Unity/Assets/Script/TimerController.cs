using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using static System.Net.Mime.MediaTypeNames;
using System.Diagnostics;

public class TimerController : MonoBehaviour
{
    [Header("UI References")]
    public UnityEngine.UI.Text countdownText;   // Teks untuk countdown (3,2,1,GO!)
    public UnityEngine.UI.Text timerText;       // Teks untuk menampilkan waktu permainan

    [Header("Game Settings")]
    public float gameDuration = 30f;

    private float timeLeft;
    private bool gameStarted = false;

    void Start()
    {
        // Pastikan di awal keduanya tersembunyi
        if (countdownText != null) countdownText.gameObject.SetActive(false);
        if (timerText != null) timerText.gameObject.SetActive(false);
    }

    // Dipanggil oleh SerialController saat pesan "START"
    public void OnGameStart()
    {
        StartCoroutine(CountdownRoutine());
    }

    // Kalau masih mau pakai "GO!" via serial
    public void OnGameGo()
    {
        StartGame();
    }

    private IEnumerator CountdownRoutine()
    {
        // Tampilkan countdown, sembunyikan timer
        countdownText.gameObject.SetActive(true);
        timerText.gameObject.SetActive(false);

        // Hitung mundur 3–1
        for (int i = 3; i > 0; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }

        // Tampilkan "GO!" dan tunggu sebentar
        countdownText.text = "GO!";
        yield return new WaitForSeconds(1f);

        // Sembunyikan countdown, mulai game
        countdownText.gameObject.SetActive(false);
        StartGame();
    }

    private void StartGame()
    {
        timeLeft = gameDuration;
        gameStarted = true;

        if (timerText != null)
        {
            timerText.gameObject.SetActive(true);
            timerText.text = Mathf.CeilToInt(timeLeft).ToString();
        }
    }

    void Update()
    {
        if (!gameStarted) return;

        timeLeft -= Time.deltaTime;
        if (timerText != null)
            timerText.text = Mathf.CeilToInt(timeLeft).ToString();

        if (timeLeft <= 0f)
        {
            gameStarted = false;
            if (timerText != null)
                timerText.gameObject.SetActive(false);

            UnityEngine.Debug.Log("Time's up!");
        }
    }
}
