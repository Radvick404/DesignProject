
using Unity.Cinemachine;
using UnityEngine;

public class CameraPrioritySwitcher : MonoBehaviour
{
    [Header("References")]
    public CinemachineCamera dialogueCamera;
    public GameObject dialoguePanel;

    [Header("Priority Settings")]
    public int priorityBoost = 20;

    private int defaultPriority;
    private bool lastState = false;

    void Start()
    {
        if (dialogueCamera == null)
        {
            Debug.LogError("CameraPrioritySwitcher: No dialogue camera assigned!");
            return;
        }

        defaultPriority = dialogueCamera.Priority;
    }

    void Update()
    {
        if (dialoguePanel == null)
            return;

        bool active = dialoguePanel.activeInHierarchy;

        // Only react on state changes (prevents unnecessary calls)
        if (active != lastState)
        {
            lastState = active;
            UpdateCameraPriority(active);
        }
    }

    void UpdateCameraPriority(bool panelActive)
    {
        if (dialogueCamera == null) return;

        if (panelActive)
        {
            dialogueCamera.Priority = defaultPriority + priorityBoost;
            // Debug.Log("Dialogue Camera BOOSTED priority");
        }
        else
        {
            dialogueCamera.Priority = defaultPriority;
            // Debug.Log("Dialogue Camera RESET priority");
        }
    }
}
