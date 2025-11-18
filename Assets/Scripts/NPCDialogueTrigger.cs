using UnityEngine;

public class NPCDialogueTrigger : MonoBehaviour
{
    [Header("References")]
    public NPCProximityReaction proximity;
    public DialogueUI ui;
    public TrustMeter trust;
    public GameObject chatBubble;
    public Transform player;

    [Header("Zones")]
    public Collider swingZone;
    public Collider slideZone;
    public Collider seesawZone;

    [Header("Trust-based Dialogue Tiers")]
    public DialogueTier[] dialogueTiers;

    private bool uiOpen = false;
    private bool interactionLocked = false;
    private string lastZone = "";

    void Start()
    {
        if (chatBubble)
            chatBubble.SetActive(false);

        // Ensure NPC begins in default (patrolling) state
        proximity.SetUIState(false);
    }

    void Update()
    {
        // -------------------------------
        // PLAYER NOT NEAR → RESET + PATROL
        // -------------------------------
        if (!proximity.playerIsNear)
        {
            ResetInteraction();
            proximity.SetUIState(false);   // NPC resumes walking
            return;
        }

        // -------------------------------
        // ZONE DETECTION
        // -------------------------------
        bool inSwing = swingZone.bounds.Contains(player.position);
        bool inSlide = slideZone.bounds.Contains(player.position);
        bool inSeesaw = seesawZone.bounds.Contains(player.position);

        bool inAZone = inSwing || inSlide || inSeesaw;
        string currentZone = GetCurrentZoneName(inSwing, inSlide, inSeesaw);

        // -------------------------------
        // UNLOCK IF NPC MOVED TO OTHER ZONE
        // -------------------------------
        if (interactionLocked && currentZone != lastZone && currentZone != "")
        {
            interactionLocked = false;
        }

        // -------------------------------
        // SHOW CHAT BUBBLE ONLY WHEN ELIGIBLE
        // -------------------------------
        if (!uiOpen && !interactionLocked && inAZone)
        {
            chatBubble.SetActive(true);
            proximity.SetUIState(false);  // bubble does NOT pause NPC
        }
        else
        {
            chatBubble.SetActive(false);
        }

        // -------------------------------
        // OPEN UI (PRESS X)
        // -------------------------------
        if (Input.GetKeyDown(KeyCode.X) && !uiOpen && !interactionLocked && inAZone)
        {
            uiOpen = true;
            chatBubble.SetActive(false);

            // ⭐ UI is open → pause NPC
            proximity.SetUIState(true);

            DialogueTier tier = GetTierForCurrentTrust();
            if (tier == null)
            {
                Debug.LogError("No matching dialogue tier found!");
                return;
            }

            if (inSwing) ShowDialogue(tier.swingDialogue);
            else if (inSlide) ShowDialogue(tier.slideDialogue);
            else if (inSeesaw) ShowDialogue(tier.seesawDialogue);

            lastZone = currentZone;
        }
    }

    // -----------------------------------------------------
    // TRUST TIER SELECTION
    // -----------------------------------------------------
    DialogueTier GetTierForCurrentTrust()
    {
        int t = trust.trust;
        foreach (var tier in dialogueTiers)
        {
            if (t >= tier.minTrust && t <= tier.maxTrust)
                return tier;
        }
        return null;
    }

    // -----------------------------------------------------
    // LOAD DIALOGUE OPTIONS
    // -----------------------------------------------------
    void ShowDialogue(DialogueSet dlg)
    {
        ui.ShowOptions(
            dlg.option1.text, () => TrustSelect(dlg.option1),
            dlg.option2.text, () => TrustSelect(dlg.option2),
            dlg.option3.text, () => TrustSelect(dlg.option3)
        );
    }

    // -----------------------------------------------------
    // OPTION SELECTED → APPLY TRUST
    // -----------------------------------------------------
    void TrustSelect(DialogueOption opt)
    {
        trust.ModifyTrust(opt.trustChange);

        ui.HideOptions();
        uiOpen = false;

        // Interaction locked in this zone
        interactionLocked = true;

        chatBubble.SetActive(false);

        // ⭐ UI closed → NPC resumes patrol
        proximity.SetUIState(false);
    }

    // -----------------------------------------------------
    // PLAYER EXITED → RESET EVERYTHING
    // -----------------------------------------------------
    void ResetInteraction()
    {
        if (uiOpen)
        {
            ui.HideOptions();
            uiOpen = false;
        }

        chatBubble.SetActive(false);

        interactionLocked = false;
        lastZone = "";

        // Ensure NPC walks again
        proximity.SetUIState(false);
    }

    // -----------------------------------------------------
    // ZONE NAME SELECTOR
    // -----------------------------------------------------
    string GetCurrentZoneName(bool inSwing, bool inSlide, bool inSeesaw)
    {
        if (inSwing) return "swing";
        if (inSlide) return "slide";
        if (inSeesaw) return "seesaw";
        return "";
    }
}
