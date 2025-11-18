using UnityEngine;

public class AutisticKidDialogue : MonoBehaviour
{
    public enum Zone { None, Swing, Slide, Seesaw }
    public Zone currentZone;

    [Header("References")]
    public DialogueUI ui;
    public TrustMeter trustMeter;

    private void Start()
    {
        currentZone = Zone.None;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("SwingZone")) TriggerSwingDialogue();
        if (other.CompareTag("SlideZone")) TriggerSlideDialogue();
        if (other.CompareTag("SeesawZone")) TriggerSeesawDialogue();
    }

    // ---------- SWING ----------
    void TriggerSwingDialogue()
    {
        currentZone = Zone.Swing;

        if (trustMeter.trust < 10)
        {
            ui.ShowOptions(
                "Can I stay here with you?", () => TrustChange(+2),
                "Can I sit next to you quietly?", () => TrustChange(+2),
                "HELLOOO!!", () => TrustChange(-8)
            );
        }
        else
        {
            ui.ShowOptions(
                "Do you like the sky today?", () => TrustChange(+4),
                "Do you want space or company?", () => TrustChange(+6),
                "Let's swing super high!", () => TrustChange(-5)
            );
        }
    }

    // ---------- SLIDE ----------
    void TriggerSlideDialogue()
    {
        currentZone = Zone.Slide;

        if (trustMeter.trust < 20)
        {
            ui.ShowOptions(
                "Too noisy here?", () => TrustChange(+4),
                "We can stay at the side.", () => TrustChange(+5),
                "Don’t be scared!", () => TrustChange(-6)
            );
        }
        else
        {
            ui.ShowOptions(
                "You can go first. I'll wait.", () => TrustChange(+6),
                "Can I slide after you?", () => TrustChange(+4),
                "Just go!", () => TrustChange(-8)
            );
        }
    }

    // ---------- SEESAW ----------
    void TriggerSeesawDialogue()
    {
        currentZone = Zone.Seesaw;

        if (trustMeter.trust < 30)
        {
            ui.ShowOptions(
                "You can sit. I won't move.", () => TrustChange(+3),
                "(Wait silently)", () => TrustChange(+6),
                "Let's go up fast!", () => TrustChange(-7)
            );
        }
        else
        {
            ui.ShowOptions(
                "We'll move slow, okay?", () => TrustChange(+8),
                "Do you like gentle play?", () => TrustChange(+5),
                "Let's bounce high!", () => TrustChange(-6)
            );
        }
    }

    // ---------- TRUST HANDLER ----------
    void TrustChange(int value)
    {
        trustMeter.ModifyTrust(value);
        ui.HideOptions();
    }
}
