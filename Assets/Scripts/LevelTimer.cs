using UnityEngine;
using UnityEngine.UI; // For UI Text

public class LevelTimer : MonoBehaviour
{
    [Header("Timer Settings")]
    public float levelTime = 60f; // ⏱️ set the time limit in seconds from Inspector

    [Header("UI (Optional)")]
    public Text timerText; // Assign a UI Text element in Inspector
    public Color normalColor = Color.white; // Default text color
    public Color timeUpColor = Color.red;   // Text color when time = 0

    private float remainingTime;
    private bool isRunning = true;

    void Start()
    {
        remainingTime = levelTime;

        if (timerText != null)
        {
            timerText.color = normalColor;
        }
    }

    void Update()
    {
        if (!isRunning) return;

        remainingTime -= Time.deltaTime;

        // Clamp to avoid going below zero
        if (remainingTime <= 0f)
        {
            remainingTime = 0f;
            isRunning = false;
            OnTimeUp();
        }

        // Update UI if assigned
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(remainingTime / 60);
            int seconds = Mathf.FloorToInt(remainingTime % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    // 🔴 Triggered when timer runs out
    void OnTimeUp()
    {
        Debug.Log("⏰ Time’s up! Level Over!");

        if (timerText != null)
        {
            timerText.color = timeUpColor;
        }

        // Add your game over logic here, e.g.:
        // SceneManager.LoadScene("GameOver");
    }

    public float GetRemainingTime()
    {
        return remainingTime;
    }

}
