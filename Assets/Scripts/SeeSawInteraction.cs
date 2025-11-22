using UnityEngine;

public class SeeSawInteraction : MonoBehaviour
{
    [Header("Seesaw Settings")]
    public Transform seesawBody;
    public float rotateSpeed = 50f;
    public float minAngle = -25f;
    public float maxAngle = 25f;

    [Header("Seat Points")]
    public Transform seatLeft;
    public Transform seatRight;

    [Header("UI Prompt")]
    public GameObject pressXIcon;

    [Header("Player Settings")]
    public float sitHeightOffset = 0.1f;

    [Header("Detach Delay")]
    public float detachDelay = 0.5f;
    private float nextDetachTime = 0f;

    private bool playerInside = false;
    private bool playerAttached = false;

    private Transform player;
    private Transform currentSeat;

    private MonoBehaviour playerMovementScript;
    private Rigidbody playerRB;

    // ---------------- NPC SETTINGS ----------------
    [Header("NPC Settings")]
    public float npcSitDuration = 3f;
    public string npcTag = "NPC";

    private bool npcAttached = false;
    private Transform npc;
    private Transform npcSeat;
    private MonoBehaviour npcMovementScript;

    // --------------------------------------------------------
    void Start()
    {
        if (pressXIcon)
            pressXIcon.SetActive(false);
    }

    // --------------------------------------------------------
    private void OnTriggerEnter(Collider other)
    {
        // ---------------- PLAYER ENTER ----------------
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            player = other.transform;

            playerMovementScript = player.GetComponent<MonoBehaviour>();
            playerRB = player.GetComponent<Rigidbody>();

            if (pressXIcon)
                pressXIcon.SetActive(true);
        }

        // ---------------- NPC ENTER ----------------
        if (other.CompareTag(npcTag) && !npcAttached)
        {
            npc = other.transform;

            npcMovementScript = npc.GetComponent<MonoBehaviour>();

            // Pick nearest seat
            float distLeft = Vector3.Distance(npc.position, seatLeft.position);
            float distRight = Vector3.Distance(npc.position, seatRight.position);
            npcSeat = (distLeft < distRight) ? seatLeft : seatRight;

            // Block seat if player is already using it
            if (playerAttached && npcSeat == currentSeat)
            {
                Debug.Log("NPC seat blocked by player.");
                return;
            }

            AttachNPC();
        }
    }

    // --------------------------------------------------------
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;

            if (pressXIcon)
                pressXIcon.SetActive(false);
        }
    }

    // --------------------------------------------------------
    private void Update()
    {
        // NPC sitting? Ignore player logic
        if (npcAttached)
            return;

        if (!playerInside)
            return;

        // ------- PLAYER ATTACH -------
        if (!playerAttached && Input.GetKeyDown(KeyCode.X))
        {
            if (pressXIcon)
                pressXIcon.SetActive(false);

            AttachPlayer();
            return;
        }

        // ------- PLAYER DETACH -------
        if (playerAttached && Input.GetKeyDown(KeyCode.X))
        {
            if (Time.time >= nextDetachTime)
            {
                DetachPlayer();

                if (pressXIcon)
                    pressXIcon.SetActive(true);
            }
            return;
        }

        // ------- CONTROL SEESAW -------
        if (playerAttached)
            ControlSeesaw();

        // Keep icon above player
        if (pressXIcon && pressXIcon.activeSelf && player)
            pressXIcon.transform.position = player.position + new Vector3(0, 2f, 0);
    }

    // --------------------------------------------------------
    // PLAYER ATTACH / DETACH
    // --------------------------------------------------------
    void AttachPlayer()
    {
        if (!player) return;

        float distLeft = Vector3.Distance(player.position, seatLeft.position);
        float distRight = Vector3.Distance(player.position, seatRight.position);
        currentSeat = (distLeft < distRight) ? seatLeft : seatRight;

        if (npcAttached && currentSeat == npcSeat)
        {
            Debug.Log("Cannot sit — NPC is on this seat.");
            return;
        }

        playerAttached = true;

        if (playerMovementScript)
            playerMovementScript.enabled = false;

        if (playerRB)
            playerRB.isKinematic = true;

        // Set parent but KEEP world scale (prevents shrinking)
        player.SetParent(currentSeat, true);

        // Adjust position/rotation
        player.position = currentSeat.position + Vector3.up * sitHeightOffset;
        player.rotation = currentSeat.rotation;

        nextDetachTime = Time.time + detachDelay;
    }

    void DetachPlayer()
    {
        playerAttached = false;

        player.SetParent(null, true);

        if (playerMovementScript)
            playerMovementScript.enabled = true;

        if (playerRB)
            playerRB.isKinematic = false;
    }

    // --------------------------------------------------------
    // NPC ATTACH / DETACH
    // --------------------------------------------------------
    void AttachNPC()
    {
        if (!npc) return;

        npcAttached = true;

        if (npcMovementScript)
            npcMovementScript.enabled = false;

        Rigidbody npcRB = npc.GetComponent<Rigidbody>();
        if (npcRB)
            npcRB.isKinematic = true;

        // Safe parent
        npc.SetParent(npcSeat, true);

        npc.position = npcSeat.position + Vector3.up * 0.1f;
        npc.rotation = npcSeat.rotation;

        StartCoroutine(NPCSitTimer());
    }

    System.Collections.IEnumerator NPCSitTimer()
    {
        yield return new WaitForSeconds(npcSitDuration);
        DetachNPC();
    }

    void DetachNPC()
    {
        if (!npc) return;

        npcAttached = false;

        npc.SetParent(null, true);

        if (npcMovementScript)
            npcMovementScript.enabled = true;

        Rigidbody npcRB = npc.GetComponent<Rigidbody>();
        if (npcRB)
            npcRB.isKinematic = false;

        npc = null;
        npcSeat = null;
    }

    // --------------------------------------------------------
    void ControlSeesaw()
    {
        float input = 0f;

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            input = 1f;

        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            input = -1f;

        if (input == 0f) return;

        float z = seesawBody.localEulerAngles.z;
        if (z > 180f) z -= 360f;

        z += input * rotateSpeed * Time.deltaTime;

        z = Mathf.Clamp(z, minAngle, maxAngle);

        seesawBody.localEulerAngles = new Vector3(
            seesawBody.localEulerAngles.x,
            seesawBody.localEulerAngles.y,
            z
        );
    }
}
