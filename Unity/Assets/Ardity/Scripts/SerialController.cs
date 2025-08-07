using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading;
// using System.Diagnostics; // Jika tidak digunakan, bisa dihapus

public class SerialController : MonoBehaviour
{
    private string portName = "COM7"; // Ganti sesuai port ESP32-mu
    private int baudRate = 115200;

    public GameObject messageListener;     // Hubungkan ke GameMessageRouter
    public LevelLooping sceneLooper;       // Script yang looping scene-gameplay
    public BonusRoundManager bonusManager;  // Drag GameObject yang punya BonusRoundManager

    public int reconnectionDelay = 1000;
    public int maxUnreadMessages = 50;

    public const string SERIAL_DEVICE_CONNECTED = "__Connected__";
    public const string SERIAL_DEVICE_DISCONNECTED = "__Disconnected__";

    protected Thread thread;
    protected SerialThreadLines serialThread;

    // private string lastMessage = ""; // Mungkin tidak terlalu efektif jika pesan bisa berulang secara valid
    // private int? lastScore = null; // Mungkin tidak terlalu efektif jika skor bisa sama tapi event berbeda

    void OnEnable()
    {
        serialThread = new SerialThreadLines(portName, baudRate, reconnectionDelay, maxUnreadMessages);
        thread = new Thread(new ThreadStart(serialThread.RunForever));
        thread.Start();
    }

    void OnDisable()
    {
        if (serialThread != null)
        {
            serialThread.RequestStop();
            serialThread = null;
        }
        if (thread != null)
        {
            thread.Join();
            thread = null;
        }
    }

    void Update()
    {
        if (serialThread == null || messageListener == null)
            return;

        string rawMessage = (string)serialThread.ReadMessage();
        if (string.IsNullOrEmpty(rawMessage))
            return;

        string message = rawMessage.Trim();
        UnityEngine.Debug.Log("[SerialController] Received: " + message);

        // 1) Switch scene ke gameplay
        if (message.Contains("Game Starting! Prepare yourself."))
        {
            sceneLooper?.StopSceneLoop();
            SceneManager.LoadScene("gameplay3"); // Pastikan nama scene ini benar
        }
        // 2) Event game start
        else if (message == "START")
        {
            messageListener.SendMessage("OnGameStart");
        }
        // 3) GO! -> mulai sensor
        else if (message == "GO!")
        {
            messageListener.SendMessage("OnGameGo");
        }
        // 4) Score update
        else if (message.StartsWith("Score:"))
        {
            string scoreStr = message.Substring("Score:".Length).Trim();
            if (int.TryParse(scoreStr, out int score))
            {
                messageListener.SendMessage("OnScoreReceived", score);
            }
        }
        // 5) Final Score (Bisa jadi dari game utama atau setelah bonus)
        else if (message.StartsWith("Game Over! Final Score:"))
        {
            string scoreString = message.Substring("Game Over! Final Score:".Length).Trim();
            UnityEngine.Debug.Log("[SerialController] Parsing 'Game Over! Final Score' from: '" + scoreString + "'");
            if (int.TryParse(scoreString, out int finalScoreValue))
            {
                UnityEngine.Debug.Log("[SerialController] Parsed 'Game Over! Final Score': " + finalScoreValue + ". Sending OnGameEnd.");
                messageListener.SendMessage("OnGameEnd", finalScoreValue);
            }
            else
            {
                UnityEngine.Debug.LogError("[SerialController] Failed to parse 'Game Over! Final Score' from message: " + message);
            }
        }
        else if (message.StartsWith("Your Final Score:")) // <-- TAMBAHKAN KONDISI INI
        {
            string scoreString = message.Substring("Your Final Score:".Length).Trim();
            UnityEngine.Debug.Log("[SerialController] Parsing 'Your Final Score' (setelah bonus) from: '" + scoreString + "'");
            if (int.TryParse(scoreString, out int finalScoreValue))
            {
                UnityEngine.Debug.Log("[SerialController] Parsed 'Your Final Score': " + finalScoreValue + ". Sending OnGameEnd.");
                messageListener.SendMessage("OnGameEnd", finalScoreValue); // Mengirim event yang sama dengan skor akhir
            }
            else
            {
                UnityEngine.Debug.LogError("[SerialController] Failed to parse 'Your Final Score' from message: " + message);
            }
        }
        // 6) Bonus Round trigger
        else if (message.Equals("You Hit The Bonus Round!"))
        {
            UnityEngine.Debug.Log(">>> Pesan untuk Bonus Round terdeteksi ('" + message + "'), memanggil BonusRoundManager.PlayBonusRound()");
            bonusManager?.PlayBonusRound();
        }
        // 7) Serial connect/disconnect events
        else if (message == SERIAL_DEVICE_CONNECTED)
        {
            messageListener.SendMessage("OnConnectionEvent", true);
        }
        else if (message == SERIAL_DEVICE_DISCONNECTED)
        {
            messageListener.SendMessage("OnConnectionEvent", false);
        }
        // 8) Pesan bebas (termasuk pesan informatif seperti "You've completed the bonus round!")
        else
        {
            messageListener.SendMessage("OnMessageArrived", message);
        }
    }

    public string ReadSerialMessage()
    {
        return (string)serialThread.ReadMessage();
    }

    public void SendSerialMessage(string message)
    {
        serialThread.SendMessage(message);
    }
}