using System.Diagnostics;
using UnityEngine;

public class SerialMessageHandler : MonoBehaviour
{
    public ScoreManager scoreManager;

    void OnConnectionEvent(bool connected)
    {
        UnityEngine.Debug.Log("Serial connected: " + connected);
    }

    void OnMessageArrived(string message)
    {
        UnityEngine.Debug.Log("Message received: " + message);

        if (message.StartsWith("Score: "))
        {
            string scoreStr = message.Substring(6);
            if (int.TryParse(scoreStr, out int score))
            {
                // Tambahkan skor ke ScoreManager
                scoreManager.AddScore(score);
            }
        }
    }
}
