using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider))]
public class PlaySplineOnTrigger : MonoBehaviour
{
    [Header("Spline Animate to Play")]
    public SplineAnimate splineAnimate;

    [Header("UI Prompt")]
    public GameObject pressXIcon;   // assign a floating X sprite above player

    [Header("Options")]
    public bool playOnlyOnce = false;

    private bool hasPlayed = false;
    private bool playerInside = false;
    private Transform player;

    void Awake()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true;

        if (pressXIcon)
            pressXIcon.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        Debug.Log("[PlaySplineOnTrigger] Player entered trigger");

        playerInside = true;
        player = other.transform;

        if (!playOnlyOnce || !hasPlayed)
        {
            if (pressXIcon)
                pressXIcon.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        Debug.Log("[PlaySplineOnTrigger] Player exited trigger");

        playerInside = false;

        if (pressXIcon)
            pressXIcon.SetActive(false);
    }

    void Update()
    {
        if (!playerInside)
            return;

        if (pressXIcon && !pressXIcon.activeSelf)
            return; // icon was hidden (means animation played already)

        // Press X using Unity Input System
        if (Keyboard.current != null && Keyboard.current.xKey.wasPressedThisFrame)
        {
            PlaySpline();
        }
        else if (Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame)
        {
            PlaySpline();
        }
    }

    void PlaySpline()
    {
        if (playOnlyOnce && hasPlayed)
            return;

        Debug.Log("[PlaySplineOnTrigger] Playing spline animation");

        if (pressXIcon)
            pressXIcon.SetActive(false);

        if (splineAnimate == null)
        {
            Debug.LogWarning("SplineAnimate is NOT assigned!");
            return;
        }

        // Make sure it's enabled & reset
        splineAnimate.gameObject.SetActive(true);
        splineAnimate.enabled = true;
        splineAnimate.NormalizedTime = 0f;
        splineAnimate.Restart(true);

        hasPlayed = true;
    }
}
