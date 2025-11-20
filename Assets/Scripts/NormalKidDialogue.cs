using UnityEngine;

public class NormalKidDialogue : MonoBehaviour
{
    public enum Zone { None, Swing, Slide, Seesaw }

    [Header("References")]
    public DialogueUI ui;
    public TrustMeter trustMeter;
    public NPCProximityReaction proximityReaction;

    [Header("Zone Colliders")]
    public Collider swingZone;
    public Collider slideZone;
    public Collider seesawZone;

    private bool inZone = false;
    private Zone currentZone = Zone.None;

    // ----------------------
    // ZONE DETECTION ONLY
    // ----------------------
    private void OnTriggerEnter(Collider other)
    {
        if (other == swingZone)
        {
            currentZone = Zone.Swing;
            inZone = true;
            Debug.Log("ENTERED SWING ZONE");
            TryTriggerDialogue();
        }

        if (other == slideZone)
        {
            currentZone = Zone.Slide;
            inZone = true;
            Debug.Log("ENTERED SLIDE ZONE");
            TryTriggerDialogue();
        }

        if (other == seesawZone)
        {
            currentZone = Zone.Seesaw;
            inZone = true;
            Debug.Log("ENTERED SEESAW ZONE");
            TryTriggerDialogue();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other == swingZone || other == slideZone || other == seesawZone)
        {
            Debug.Log("EXITED ZONE");
            inZone = false;
            currentZone = Zone.None;
            ui.HideOptions();
        }
    }

    // ----------------------
    // FINAL DIALOGUE LOGIC
    // ----------------------
    void TryTriggerDialogue()
    {
        Debug.Log($"TryTriggerDialogue() called | inZone={inZone} | playerNear={proximityReaction.playerIsNear}");

        if (!proximityReaction.playerIsNear)
        {
            Debug.Log("STOP: Player NOT near kid");
            return;
        }

        if (!inZone)
        {
            Debug.Log("STOP: Player NOT in a zone");
            return;
        }

        Debug.Log("SUCCESS: Player near + in zone → SHOWING OPTIONS");

        switch (currentZone)
        {
            case Zone.Swing:
                TriggerSwingDialogue();
                break;

            case Zone.Slide:
                TriggerSlideDialogue();
                break;

            case Zone.Seesaw:
                TriggerSeesawDialogue();
                break;
        }
    }

    // ----------------------
    // DIALOGUE BRANCH METHODS
    // ----------------------

    void TriggerSwingDialogue()
    {
        Debug.Log("Triggering SWING Dialogue");

        if (trustMeter.trust < 20)
        {
            ui.ShowOptions(
                "Swings look fun! Want to go higher together?", () => { ui.ShowReply("Okay! Let's go higher."); TrustChange(+10); },
                "Can I sit next to you?", () => { ui.ShowReply("Yes, you can sit here."); TrustChange(+5); },
                "Move. I want that swing.", () => { ui.ShowReply("That's not very nice."); TrustChange(-10); }
            );
        }
        else
        {
            ui.ShowOptions(
                "Race you to the sky!", () => { ui.ShowReply("You're on!"); TrustChange(+10); },
                "Nice weather huh?", () => { ui.ShowReply("Yeah, it's great."); TrustChange(+5); },
                "You're swinging weird.", () => { ui.ShowReply("Hmm."); TrustChange(-5); }
            );
        }
    }

    void TriggerSlideDialogue()
    {
        Debug.Log("Triggering SLIDE Dialogue");

        if (trustMeter.trust < 30)
        {
            ui.ShowOptions(
                "Want to slide together?", () => { ui.ShowReply("Let's go."); TrustChange(+10); },
                "Why do you like this slide?", () => { ui.ShowReply("It's smooth."); TrustChange(+5); },
                "You slide like a turtle.", () => { ui.ShowReply("Hey."); TrustChange(-10); }
            );
        }
        else
        {
            ui.ShowOptions(
                "Ready for a race?", () => { ui.ShowReply("Ready."); TrustChange(+10); },
                "You go first!", () => { ui.ShowReply("Okay."); TrustChange(+5); },
                "Just go!", () => { ui.ShowReply("Alright."); TrustChange(-8); }
            );
        }
    }

    void TriggerSeesawDialogue()
    {
        Debug.Log("Triggering SEESAW Dialogue");

        if (trustMeter.trust < 35)
        {
            ui.ShowOptions(
                "Let's balance together.", () => { ui.ShowReply("Okay."); TrustChange(+15); },
                "What if we go super high?", () => { ui.ShowReply("Maybe."); TrustChange(+7); },
                "Get off. I want it.", () => { ui.ShowReply("No."); TrustChange(-15); }
            );
        }
        else
        {
            ui.ShowOptions(
                "We'll move slow, okay?", () => { ui.ShowReply("Yes."); TrustChange(+15); },
                "Do you like gentle play?", () => { ui.ShowReply("I do."); TrustChange(+5); },
                "Let's bounce fast!", () => { ui.ShowReply("Too fast."); TrustChange(-10); }
            );
        }
    }

    void TrustChange(int value)
    {
        trustMeter.ModifyTrust(value);
            ui.HideOptions();
            ui.HideReply();
    }
}
