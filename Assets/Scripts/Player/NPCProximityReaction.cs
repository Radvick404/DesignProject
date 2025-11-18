using UnityEngine;

public class NPCProximityReaction : MonoBehaviour
{
    // ⭐ Only one NPC is allowed to control player/NPC state at a time
    public static NPCProximityReaction activeNPC = null;

    public NPCWaypointPatrol patrolScript;

    public SpriteRenderer spriteRenderer;
    public Sprite frontSprite;
    public Sprite backSprite;
    public Sprite sideSprite;
    public Sprite sideSpriteLeft;

    public bool playerIsNear = false;
    public bool uiOpen = false;

    private Transform player;
    private bool paused = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (PlayerRef.isSeated) return;

        player = other.transform;
        playerIsNear = true;
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (!playerIsNear) return;

        // ⭐ ONLY the active NPC can pause or face the player
        if (activeNPC != this)
        {
            ResumeNPC();
            return;
        }

        if (uiOpen)
        {
            PauseNPC();
            FacePlayer();
        }
        else
        {
            ResumeNPC();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerIsNear = false;

        // If THIS NPC was active, release control
        if (activeNPC == this)
            activeNPC = null;

        ResumeNPC();
    }

    // Called by DialogueTrigger
    public void SetUIState(bool state)
    {
        uiOpen = state;

        if (state)
        {
            // ⭐ This NPC becomes the active one
            activeNPC = this;
        }
        else
        {
            // ⭐ Release NPC control
            if (activeNPC == this)
                activeNPC = null;
        }
    }

    void PauseNPC()
    {
        if (!paused)
        {
            patrolScript.enabled = false;
            paused = true;
        }
    }

    void ResumeNPC()
    {
        if (paused)
        {
            patrolScript.enabled = true;
            paused = false;
        }
    }

    void FacePlayer()
    {
        if (!player || !spriteRenderer) return;

        Vector3 dir = player.position - transform.position;
        dir.y = 0;

        bool horizontal = Mathf.Abs(dir.x) > Mathf.Abs(dir.z);

        if (horizontal)
        {
            if (dir.x < 0 && sideSpriteLeft != null)
            {
                spriteRenderer.sprite = sideSpriteLeft;
                spriteRenderer.flipX = false;
            }
            else
            {
                spriteRenderer.sprite = sideSprite;
                spriteRenderer.flipX = dir.x < 0;
            }
        }
        else
        {
            spriteRenderer.sprite = (dir.z > 0) ? backSprite : frontSprite;
        }
    }
}
