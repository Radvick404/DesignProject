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
    public float detachDelay = 0.5f;   // ⭐ Delay before detach allowed
    private float nextDetachTime = 0f;

    private bool playerInside = false;
    private bool playerAttached = false;

    private Transform player;
    private Transform currentSeat;

    private MonoBehaviour playerMovementScript;
    private Rigidbody playerRB;


    private void Start()
    {
        if (pressXIcon)
            pressXIcon.SetActive(false);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInside = true;
        player = other.transform;

        playerMovementScript = player.GetComponent<MonoBehaviour>();
        playerRB = player.GetComponent<Rigidbody>();

        if (pressXIcon)
            pressXIcon.SetActive(true);
    }


    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInside = false;

        if (pressXIcon)
            pressXIcon.SetActive(false);
/*
        if (playerAttached)
            DetachPlayer();*/
    }


    private void Update()
    {
        if (!playerInside)
            return;

        // ------- ATTACH (only if not attached) -------
        if (!playerAttached && Input.GetKeyDown(KeyCode.X))
        {
            if (pressXIcon)
                pressXIcon.SetActive(false);

            AttachPlayer();
            return;
        }

        // ------- DETACH (only if allowed by timer) -------
        if (playerAttached && Input.GetKeyDown(KeyCode.X))
        {
            if (Time.time >= nextDetachTime)   // ⭐ delay check
            {
                DetachPlayer();

                if (pressXIcon)
                    pressXIcon.SetActive(true);
            }

            return;
        }

        if (playerAttached)
            ControlSeesaw();


        // keep X icon above player
        if (pressXIcon && pressXIcon.activeSelf && player)
            pressXIcon.transform.position = player.position + new Vector3(0, 2f, 0);
    }


    void AttachPlayer()
    {
        if (!player) return;

        float distLeft = Vector3.Distance(player.position, seatLeft.position);
        float distRight = Vector3.Distance(player.position, seatRight.position);

        currentSeat = (distLeft < distRight) ? seatLeft : seatRight;

        playerAttached = true;

        if (playerMovementScript)
            playerMovementScript.enabled = false;

        if (playerRB)
            playerRB.isKinematic = true;

        player.position = currentSeat.position + Vector3.up * sitHeightOffset;
        player.rotation = currentSeat.rotation;

        player.SetParent(currentSeat);

        // ⭐ Set delay timer: cannot detach immediately
        nextDetachTime = Time.time + detachDelay;

        Debug.Log("Player attached to seesaw, detach available after delay.");
    }


    void DetachPlayer()
    {
        playerAttached = false;

        player.SetParent(null);

        if (playerMovementScript)
            playerMovementScript.enabled = true;

        if (playerRB)
            playerRB.isKinematic = false;

        Debug.Log("Player detached from seesaw.");
    }


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
