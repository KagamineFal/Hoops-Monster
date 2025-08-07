using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using static System.Net.Mime.MediaTypeNames;

public class BonusTimerController : MonoBehaviour
{
    public UnityEngine.UI.Text timerText;
    public float bonusDuration = 15f;

    private float timeLeft;
    private bool timerRunning = false;

    public BonusRoundManager bonusRoundManager;

    public void StartBonusRoundTimer()
    {
        timeLeft = bonusDuration;
        timerRunning = true;
        if (timerText != null)
        {
            timerText.gameObject.SetActive(true);
            timerText.text = Mathf.CeilToInt(timeLeft).ToString();
        }
    }

    void Update()
    {
        if (!timerRunning) return;

        timeLeft -= Time.deltaTime;
        if (timerText != null)
            timerText.text = Mathf.CeilToInt(timeLeft).ToString();

        if (timeLeft <= 0f)
        {
            timerRunning = false;
            if (timerText != null)
                timerText.gameObject.SetActive(false);

            // Beritahu BonusRoundManager bahwa bonus round selesai
            if (bonusRoundManager != null)
                bonusRoundManager.OnBonusRoundFinished();
        }
    }
}
