using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
//using static System.Net.Mime.MediaTypeNames;
using System.Diagnostics;

public class ScoreManager : MonoBehaviour
{
    [Header("UI References")]
    public Text scoreText;
    public Image[] popImages;

    [Header("Scene Settings")]
    public string leaderboardSceneName = "leaderboard"; // Nama scene leaderboard

    private int score = 0;
    private bool canAddScore = true;
    private int lastReceivedScore = -1;
    private int currentImageIndex = 0;
    private Coroutine popCoroutine;

    private const string LATEST_SCORE_KEY = "LatestScore";

    void Start()
    {
        UpdateScoreText();
    }

    public void AddScore(int amount)
    {
        if (canAddScore)
        {
            score = amount;
            UpdateScoreText();
            TriggerPopImage();
        }
    }

    public void SetScore(int newScore)
    {
        if (canAddScore && newScore != lastReceivedScore)
        {
            score = newScore;
            lastReceivedScore = newScore;
            UpdateScoreText();
            TriggerPopImage();
        }
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = " " + score.ToString();
        }
    }

    public void AddScoreDirectly()
    {
        AddScore(1);
    }

    public void OnMessageArrived(string message)
    {
        UnityEngine.Debug.Log("Pesan dari Arduino: " + message);
    }

    public void OnConnectionEvent(bool connected)
    {
        UnityEngine.Debug.Log("Status koneksi Arduino: " + (connected ? "Connected" : "Disconnected"));
    }

    public void StopScoring()
    {
        canAddScore = false;
        UnityEngine.Debug.Log("Waktu habis, skor tidak bisa bertambah.");
    }

    public void ResumeScoring()
    {
        canAddScore = true;
        UnityEngine.Debug.Log("Bonus round! Skor bisa bertambah lagi.");
    }

    public void OnScoreReceived(int receivedScore)
    {
        SetScore(receivedScore);
    }

    public void OnGameStart()
    {
        UnityEngine.Debug.Log("Permainan dimulai!");
        ResumeScoring();
        score = 0;
        lastReceivedScore = -1;
        UpdateScoreText();
    }

    public void OnGameGo()
    {
        UnityEngine.Debug.Log("Permainan Go!");
    }

    public void OnGameEnd(int finalScore)
    {
        UnityEngine.Debug.Log("[ScoreManager] OnGameEnd dipanggil. Skor akhir: " + finalScore);
        this.score = finalScore;
        UpdateScoreText();
        StopScoring();

        PlayerPrefs.SetInt(LATEST_SCORE_KEY, this.score);
        PlayerPrefs.Save();
        UnityEngine.Debug.Log("Skor akhir " + this.score + " disimpan ke PlayerPrefs dengan kunci: " + LATEST_SCORE_KEY);

        UnityEngine.Debug.Log("[ScoreManager] Mencoba memuat scene: " + leaderboardSceneName);
        SceneManager.LoadScene(leaderboardSceneName);
    }

    // ================================
    // ANIMASI POP IMAGE DENGAN COROUTINE
    // ================================

    void TriggerPopImage()
    {
        if (popImages == null || popImages.Length == 0) return;

        if (popCoroutine != null)
            StopCoroutine(popCoroutine);

        popCoroutine = StartCoroutine(AnimatePopImage(popImages[currentImageIndex]));
        currentImageIndex = (currentImageIndex + 1) % popImages.Length;
    }

    IEnumerator AnimatePopImage(Image img)
    {
        // Reset semua image
        foreach (var image in popImages)
        {
            image.gameObject.SetActive(false);
            image.transform.localScale = Vector3.one * 0.01f;
        }

        img.gameObject.SetActive(true);

        float timer = 0f;
        float durationUp = 0.25f;
        float durationDown = 0.1f;

        Vector3 startScale = Vector3.one * 0.01f;
        Vector3 overShootScale = Vector3.one * 2.4f; // Besarkan efek pop
        Vector3 endScale = Vector3.one * 0f;

        while (timer < durationUp)
        {
            float t = timer / durationUp;
            img.transform.localScale = Vector3.Lerp(startScale, overShootScale, EaseOutBack(t));
            timer += Time.deltaTime;
            yield return null;
        }

        timer = 0f;
        while (timer < durationDown)
        {
            float t = timer / durationDown;
            img.transform.localScale = Vector3.Lerp(overShootScale, endScale, t);
            timer += Time.deltaTime;
            yield return null;
        }

        img.transform.localScale = endScale;

        yield return new WaitForSeconds(1f);
        img.gameObject.SetActive(false);
    }

    float EaseOutBack(float t)
    {
        float c1 = 1.70158f;
        float c3 = c1 + 1f;
        return 1 + c3 * Mathf.Pow(t - 1, 3) + c1 * Mathf.Pow(t - 1, 2);
    }
}
