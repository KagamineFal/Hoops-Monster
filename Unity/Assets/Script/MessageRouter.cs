using UnityEngine;

public class GameMessageRouter : MonoBehaviour
{
    public TimerController timerController;
    public ScoreManager scoreManager;

    public void OnMessageArrived(string msg)
    {
        UnityEngine.Debug.Log("Pesan diterima: " + msg);
    }

    public void OnGameStart()
    {
        timerController?.OnGameStart();
        scoreManager?.OnGameStart();
    }

    public void OnGameGo()
    {
        timerController?.OnGameGo();
        scoreManager?.OnGameGo();
    }

    public void OnScoreReceived(int score)
    {
        scoreManager?.OnScoreReceived(score);
    }

    public void OnGameEnd(int finalScore)
    {
        UnityEngine.Debug.Log("[GameMessageRouter] OnGameEnd dipanggil dengan skor: " + finalScore); // Tambahkan ini
        scoreManager?.OnGameEnd(finalScore);
        // timerController?.OnGameEnd(); // Baris ini dikomentari jika TimerController belum punya OnGameEnd
    }

    public void OnConnectionEvent(bool isConnected)
    {
        UnityEngine.Debug.Log("Serial connected: " + isConnected);
        scoreManager?.OnConnectionEvent(isConnected);
    }
}