using UnityEngine;
using UnityEngine.Splines;

[RequireComponent(typeof(Collider))]
public class PlaySplineForNPCOnTrigger : MonoBehaviour
{
    [Header("Trigger Options")]
    public bool playOnlyOnce = false;
    public bool requireTag = true;
    public string npcTag = "NPC";   // All NPCs must use this tag

    private void Awake()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Ignore player
        if (other.CompareTag("Player"))
            return;

        // If using tags, only accept NPCs
        if (requireTag && !other.CompareTag(npcTag))
            return;

        // Check if the NPC has a SplineAnimate component somewhere
        SplineAnimate spline = other.GetComponent<SplineAnimate>();

        if (spline == null)
        {
            spline = other.GetComponentInChildren<SplineAnimate>();
        }

        if (spline == null)
        {
            Debug.LogWarning($"NPC '{other.name}' has no SplineAnimate component.");
            return;
        }

        // Play Spline Animation
        PlayNPCSpline(spline);
    }

    private void PlayNPCSpline(SplineAnimate spline)
    {
        // prevent replays if required
        if (playOnlyOnce && spline.NormalizedTime > 0f)
            return;

        Debug.Log($"Playing spline animation for NPC: {spline.gameObject.name}");

        spline.enabled = true;
        spline.gameObject.SetActive(true);
        spline.NormalizedTime = 0f;
        spline.Restart(true);
    }
}
