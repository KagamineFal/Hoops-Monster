using System.Collections.Generic;
using UnityEngine;
using TMPro; // Pastikan menggunakan namespace yang benar untuk TextMeshPro
using System.Linq; // Diperlukan untuk OrderByDescending dan Take

public class leaderboardManager : MonoBehaviour
{
    public TMP_Text score1Text;
    public TMP_Text score2Text;
    public TMP_Text score3Text;

    private List<int> topScores = new List<int>();

    private const string LATEST_SCORE_KEY = "LatestScore"; // Kunci untuk skor terbaru dari game
    private const string TOP_SCORE_KEY_PREFIX = "TopScore"; // Prefix untuk kunci skor teratas (TopScore0, TopScore1, dst.)
    private const int NUMBER_OF_TOP_SCORES = 3; // Jumlah skor teratas yang disimpan

    void Start()
    {
        LoadTopScoresFromPlayerPrefs(); // Muat skor teratas yang sudah ada

        // Cek apakah ada skor baru dari sesi permainan terakhir
        if (PlayerPrefs.HasKey(LATEST_SCORE_KEY))
        {
            int latestScore = PlayerPrefs.GetInt(LATEST_SCORE_KEY);
            UnityEngine.Debug.Log("Skor terbaru ditemukan: " + latestScore);

            AddNewScoreAndSave(latestScore); // Tambah, urutkan, simpan, dan update UI

            PlayerPrefs.DeleteKey(LATEST_SCORE_KEY); // Hapus kunci skor terbaru agar tidak diproses lagi
            PlayerPrefs.Save(); // Simpan penghapusan kunci
        }
        else
        {
            UnityEngine.Debug.Log("Tidak ada skor terbaru ditemukan. Menampilkan skor tersimpan.");
            UpdateUI(); // Jika tidak ada skor baru, cukup update UI dengan skor yang sudah dimuat
        }
    }

    void LoadTopScoresFromPlayerPrefs()
    {
        topScores.Clear();
        for (int i = 0; i < NUMBER_OF_TOP_SCORES; i++)
        {
            // Muat skor, default ke 0 jika kunci tidak ditemukan
            topScores.Add(PlayerPrefs.GetInt(TOP_SCORE_KEY_PREFIX + i, 0));
        }
        // Pastikan daftar selalu diurutkan setelah memuat
        topScores = topScores.OrderByDescending(s => s).ToList(); 
        UnityEngine.Debug.Log("Skor teratas dimuat dari PlayerPrefs: " + string.Join(", ", topScores));
    }

    void SaveTopScoresToPlayerPrefs()
    {
        for (int i = 0; i < NUMBER_OF_TOP_SCORES; i++)
        {
            if (i < topScores.Count)
            {
                PlayerPrefs.SetInt(TOP_SCORE_KEY_PREFIX + i, topScores[i]);
            }
            else
            {
                // Ini seharusnya tidak terjadi jika topScores selalu dijaga memiliki NUMBER_OF_TOP_SCORES elemen
                PlayerPrefs.SetInt(TOP_SCORE_KEY_PREFIX + i, 0); // Simpan 0 jika entah bagaimana list lebih pendek
            }
        }
        PlayerPrefs.Save(); // Penting: simpan perubahan ke disk
        UnityEngine.Debug.Log("Skor teratas disimpan ke PlayerPrefs: " + string.Join(", ", topScores));
    }

    // Method ini akan menangani penambahan skor baru, pengurutan, penyimpanan, dan pembaruan UI
    public void AddNewScoreAndSave(int newScore)
    {
        topScores.Add(newScore);
        // Urutkan skor dari yang terbesar ke terkecil, lalu ambil sejumlah NUMBER_OF_TOP_SCORES
        topScores = topScores.OrderByDescending(s => s).Take(NUMBER_OF_TOP_SCORES).ToList();

        // Pastikan list selalu memiliki NUMBER_OF_TOP_SCORES elemen, isi dengan 0 jika kurang
        while (topScores.Count < NUMBER_OF_TOP_SCORES)
        {
            topScores.Add(0);
        }

        SaveTopScoresToPlayerPrefs();
        UpdateUI();
    }

    private void UpdateUI()
    {
        // Pastikan referensi TMP_Text tidak null sebelum menggunakannya
        if (score1Text != null && topScores.Count > 0) score1Text.text = " " + topScores[0]; else if (score1Text != null) score1Text.text = " 0";
        if (score2Text != null && topScores.Count > 1) score2Text.text = " " + topScores[1]; else if (score2Text != null) score2Text.text = " 0";
        if (score3Text != null && topScores.Count > 2) score3Text.text = " " + topScores[2]; else if (score3Text != null) score3Text.text = " 0";
        UnityEngine.Debug.Log("UI Leaderboard diperbarui.");
    }

    // Fungsi simulasi input untuk pengujian (opsional, bisa dihapus atau dikomentari)
#if UNITY_EDITOR // Hanya aktif di Unity Editor
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // Tekan Space untuk uji coba
        {
            int simulatedScore = UnityEngine.Random.Range(0, 1000);
            UnityEngine.Debug.Log("--- Mensimulasikan skor baru via Spacebar: " + simulatedScore + " ---");
            AddNewScoreAndSave(simulatedScore);
        }

        if (Input.GetKeyDown(KeyCode.C)) // Tekan C untuk membersihkan PlayerPrefs (Hanya untuk Testing)
        {
            UnityEngine.Debug.Log("--- Membersihkan semua PlayerPrefs terkait skor via tombol C ---");
            PlayerPrefs.DeleteKey(LATEST_SCORE_KEY);
            for (int i = 0; i < NUMBER_OF_TOP_SCORES; i++)
            {
                PlayerPrefs.DeleteKey(TOP_SCORE_KEY_PREFIX + i);
            }
            PlayerPrefs.Save();
            LoadTopScoresFromPlayerPrefs(); // Muat ulang (seharusnya menjadi 0 semua)
            UpdateUI(); // Perbarui UI untuk menampilkan skor yang sudah dibersihkan
        }
    }
#endif
}