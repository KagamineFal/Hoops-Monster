using System.Diagnostics;
using UnityEngine;

public class ToGameplay : MonoBehaviour
{
    // Method ini bakal dipanggil dari SerialController kalau ada pesan dari serial
    public void OnMessageArrived(string message)
    {
        if (message.Contains("Game Starting! Prepare yourself."))
        {
            UnityEngine.Debug.Log("Received 'Game Starting' message, ready to load gameplay scene.");
            // Kalau kamu mau pindah scene di sini juga bisa, tapi biasanya mending di SerialController aja
        }
    }
}
