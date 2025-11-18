using UnityEngine;

public class AutisticNPCDialogueTrigger : MonoBehaviour
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

    [Header("Autistic Kid Settings")]
    public float comfortTime = 3f;        // Time player must stay near
    public float anxietyResetTime = 1f;   // Reset timer when leaving
    public float trustMultiplier = 0.5f;  // Slower trust gain

    private bool uiOpen = false;
    private bool interactionLocked = false;
    private string lastZone = "";
    private float comfortTimer = 0f;
    private bool isComfortable = false;

    void Start()
    {
        if (chatBubble)
            chatBubble.SetActive(false);

        proximity.SetUIState(false);  // ALWAYS start in patrol mode
    }

    void Update()
    {
        // -------------------------
        // PLAYER NOT NEAR → RESET
        // -------------------------
        if (!proximity.playerIsNear)
        {
            ResetInteraction();
            proximity.SetUIState(false);
            return;
        }

        // -------------------------
        // ZONE DETECTION
        // -------------------------
        bool inSwing = swingZone.bounds.Contains(player.position);
        bool inSlide = slideZone.bounds.Contains(player.position);
        bool inSeesaw = seesawZone.bounds.Contains(player.position);
        bool inAZone = (inSwing || inSlide || inSeesaw);

        if (!inAZone)
        {
            chatBubble.SetActive(false);
            proximity.SetUIState(false);
            return;
        }

        string currentZone = GetCurrentZoneName(inSwing, inSlide, inSeesaw);

        // -------------------------
        // UNLOCK IF MOVED TO NEW ZONE
        // -------------------------
        if (interactionLocked && currentZone != lastZone)
        {
            interactionLocked = false;
            isComfortable = false;
            comfortTimer = 0f;
        }

        // -------------------------
        // AUTISTIC KID: NEED TIME TO FEEL SAFE
        // -------------------------
        if (!isComfortable)
        {
            comfortTimer += Time.deltaTime;

            if (comfortTimer >= comfortTime)
            {
                isComfortable = true;

                if (!uiOpen && !interactionLocked)
                {
                    chatBubble.SetActive(true);
                    proximity.SetUIState(false); // bubble does NOT pause NPC
                }
            }
            else
            {
                chatBubble.SetActive(false);
            }
        }

        // -------------------------
        // PRESS X TO TALK (only when comfortable)
        // -------------------------
        if (Input.GetKeyDown(KeyCode.X) && isComfortable && !uiOpen && !interactionLocked)
        {
            uiOpen = true;
            chatBubble.SetActive(false);

            proximity.SetUIState(true);  // ⭐ pause NPC

            DialogueTier tier = GetTierForCurrentTrust();
            if (tier == null) return;

            if (inSwing) ShowDialogue(tier.swingDialogue);
            else if (inSlide) ShowDialogue(tier.slideDialogue);
            else if (inSeesaw) ShowDialogue(tier.seesawDialogue);

            lastZone = currentZone;
        }
    }

    // -------------------------
    // GET TRUST TIER
    // -------------------------
    DialogueTier GetTierForCurrentTrust()
    {
        int t = trust.trust;
        foreach (var tier in dialogueTiers)
            if (t >= tier.minTrust && t <= tier.maxTrust)
                return tier;

        return null;
    }

    // -------------------------
    // SHOW DIALOGUE OPTIONS
    // -------------------------
    void ShowDialogue(DialogueSet dlg)
    {
        ui.ShowOptions(
            dlg.option1.text, () => TrustSelect(dlg.option1),
            dlg.option2.text, () => TrustSelect(dlg.option2),
            dlg.option3.text, () => TrustSelect(dlg.option3)
        );
    }

    // -------------------------
    // AUTISTIC TRUST ADJUSTMENT
    // -------------------------
    void TrustSelect(DialogueOption opt)
    {
        int adjusted = Mathf.RoundToInt(opt.trustChange * trustMultiplier);
        trust.ModifyTrust(adjusted);

        ui.HideOptions();
        uiOpen = false;

        interactionLocked = true;
        isComfortable = false;
        comfortTimer = 0f;

        chatBubble.SetActive(false);

        proximity.SetUIState(false);  // ⭐ resume NPC patrol
    }

    // -------------------------
    // WHEN PLAYER LEAVES PROXIMITY
    // -------------------------
    void ResetInteraction()
    {
        comfortTimer += Time.deltaTime;

        if (comfortTimer >= anxietyResetTime)
        {
            isComfortable = false;
            comfortTimer = 0f;
        }

        if (uiOpen)
        {
            ui.HideOptions();
            uiOpen = false;
        }

        chatBubble.SetActive(false);
        proximity.SetUIState(false); // ⭐ resume patrol always
    }

    // -------------------------
    // ZONE NAME HELPER
    // -------------------------
    string GetCurrentZoneName(bool inSwing, bool inSlide, bool inSeesaw)
    {
        if (inSwing) return "swing";
        if (inSlide) return "slide";
        if (inSeesaw) return "seesaw";
        return "";
    }
}
