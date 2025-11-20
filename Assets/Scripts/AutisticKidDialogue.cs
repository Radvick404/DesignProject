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
                "Can I stay here with you?", () => { ui.ShowReply("Yes."); TrustChange(+2); },
                "Can I sit next to you quietly?", () => { ui.ShowReply("Okay."); TrustChange(+2); },
                "HELLOOO!!", () => { ui.ShowReply("Too loud."); TrustChange(-8); }
            );
        }
        else
        {
            ui.ShowOptions(
                "Do you like the sky today?", () => { ui.ShowReply("Yes."); TrustChange(+4); },
                "Do you want space or company?", () => { ui.ShowReply("Space."); TrustChange(+6); },
                "Let's swing super high!", () => { ui.ShowReply("Too high."); TrustChange(-5); }
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
                "Too noisy here?", () => { ui.ShowReply("Yes."); TrustChange(+4); },
                "We can stay at the side.", () => { ui.ShowReply("Thank you."); TrustChange(+5); },
                "Don�t be scared!", () => { ui.ShowReply("I am trying."); TrustChange(-6); }
            );
        }
        else
        {
            ui.ShowOptions(
                "You can go first. I'll wait.", () => { ui.ShowReply("Okay."); TrustChange(+6); },
                "Can I slide after you?", () => { ui.ShowReply("Yes."); TrustChange(+4); },
                "Just go!", () => { ui.ShowReply("Alright."); TrustChange(-8); }
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
                "You can sit. I won't move.", () => { ui.ShowReply("Thank you."); TrustChange(+3); },
                "(Wait silently)", () => { ui.ShowReply("..."); TrustChange(+6); },
                "Let's go up fast!", () => { ui.ShowReply("No."); TrustChange(-7); }
            );
        }
        else
        {
            ui.ShowOptions(
                "We'll move slow, okay?", () => { ui.ShowReply("Yes."); TrustChange(+8); },
                "Do you like gentle play?", () => { ui.ShowReply("Yes."); TrustChange(+5); },
                "Let's bounce high!", () => { ui.ShowReply("Not now."); TrustChange(-6); }
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
