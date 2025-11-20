using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PanelTimer : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("The GameObject (Panel) to enable/disable.")]
    public GameObject panelObject;

    [Header("Settings")]
    [Tooltip("The duration (in seconds) the panel stays active.")]
    public float displayDuration = 3f;

    private void Start()
    {
        // 1. Ensure the panel starts disabled, then enable it for the first time
        if (panelObject != null)
        {
            panelObject.SetActive(false);
            EnablePanel();
        }
        else
        {
            Debug.LogError("Panel Object is not assigned in the Inspector!");
        }
    }

    /// <summary>
    /// Public method to call when the UI Button is clicked.
    /// </summary>
    public void OnButtonClickEnablePanel()
    {
        EnablePanel();
    }

    /// <summary>
    /// Starts the coroutine to enable the panel and disable it after a delay.
    /// </summary>
    private void EnablePanel()
    {
        // Stop any currently running coroutine to avoid conflicts
        StopAllCoroutines();

        // Start the new coroutine
        StartCoroutine(ShowPanelForDuration());
    }

    /// <summary>
    /// Coroutine to handle the panel's timing.
    /// </summary>
    private IEnumerator ShowPanelForDuration()
    {
        // Step 1: Enable the panel
        panelObject.SetActive(true);

        // Step 2: Wait for the specified duration
        yield return new WaitForSeconds(displayDuration);

        // Step 3: Disable the panel
        panelObject.SetActive(false);
    }
}