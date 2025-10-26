using UnityEngine;
using UnityEngine.SceneManagement; // ✅ Required for scene loading

public class LevelGoal : MonoBehaviour
{
    [Header("References")]
    public LevelTimer timer; // drag your LevelTimer object here

    private bool playerInside = false;
    private bool sisterInside = false;
    private bool levelCompleted = false; // ensures one-time trigger

    public void OnTriggerStay(Collider other)
    {
        if (other.name == "Player")
        {
            playerInside = true;
            Debug.Log("Player inside");
        }

        if (other.name == "Sister")
        {
            sisterInside = true;
            Debug.Log("Sister inside");
        }

        CheckWinCondition();
    }

    private void CheckWinCondition()
    {
        if (levelCompleted) return; // already triggered

        if (playerInside && sisterInside)
        {
            levelCompleted = true; // prevent multiple triggers
            float timeLeft = timer != null ? timer.GetRemainingTime() : 0f;
            Debug.Log("✅ Level Complete! Time left: " + timeLeft.ToString("F2"));

            // ✅ Load the next scene by build index
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(currentSceneIndex + 1);

            // Alternative: load by name
            // SceneManager.LoadScene("NextSceneName");
        }
    }
}
