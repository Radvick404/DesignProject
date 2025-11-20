using UnityEngine;

public class PlayerSwingInteraction : MonoBehaviour
{
    [Header("Player Setup")]
    public Transform seatPoint;              // exact sitting location
    public string playerTag = "Player";

    [Header("Swing Motion")]
    public Transform swing;                  // the rotating part
    public float swingSpeed = 50f;           // W/S pumping speed
    public float maxRotation = 40f;          // maximum swing angle
    public float returnSpeed = 2f;           // swing stabilization speed

    [Header("Inputs")]
    public KeyCode sitKey = KeyCode.X;
    public KeyCode detachKey = KeyCode.E;

    [Header("UI Prompt")]
    public GameObject pressXIcon;            // icon over player head

    private bool isSeated = false;
    private bool playerInside = false;
    private Transform player;
    private Rigidbody playerRb;
    private float currentAngle = 0f;

    void Awake()
    {
        if (pressXIcon)
            pressXIcon.SetActive(false);
    }

    // ---------------------------------------
    // Trigger Zone Detection
    // ---------------------------------------
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag))
            return;

        player = other.transform;
        playerInside = true;

        // Show X prompt if not seated
        if (!isSeated && pressXIcon)
            pressXIcon.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(playerTag))
            return;

        playerInside = false;

        // Hide prompt only if not seated
        if (!isSeated && pressXIcon)
            pressXIcon.SetActive(false);
    }

    // ---------------------------------------
    // UPDATE LOOP
    // ---------------------------------------
    void Update()
    {
        // If player is inside trigger and not seated → wait for X
        if (playerInside && !isSeated && Input.GetKeyDown(sitKey))
        {
            SeatPlayer();
            return;
        }

        // If seated and E pressed → detach
        if (isSeated && Input.GetKeyDown(detachKey))
        {
            ReleasePlayer();
            return;
        }

        // If seated → pump swing & move seat
        if (isSeated)
        {
            HandleSwingMotion();
            ApplySwingRotation();

            // Keep player glued to seatPoint
            if (player)
                player.position = seatPoint.position;
        }
    }

    // ---------------------------------------
    // SEAT PLAYER
    // ---------------------------------------
    void SeatPlayer()
    {
        if (player == null) return;

        isSeated = true;

        // Lock player movement
        if (player.TryGetComponent<Rigidbody>(out playerRb))
        {
            playerRb.isKinematic = true;
            playerRb.linearVelocity = Vector3.zero;
            playerRb.angularVelocity = Vector3.zero;
        }

        // Snap player to seat
        player.position = seatPoint.position;
        player.rotation = seatPoint.rotation;

        // Hide X prompt
        if (pressXIcon)
            pressXIcon.SetActive(false);

        Debug.Log("Player seated on swing.");
    }

    // ---------------------------------------
    // RELEASE PLAYER
    // ---------------------------------------
    void ReleasePlayer()
    {
        isSeated = false;

        if (playerRb != null)
            playerRb.isKinematic = false;

        // Restore prompt if still inside zone
        if (playerInside && pressXIcon)
            pressXIcon.SetActive(true);

        Debug.Log("Player released from swing.");
    }

    // ---------------------------------------
    // SWING PUMPING
    // ---------------------------------------
    void HandleSwingMotion()
    {
        float pump = Input.GetAxis("Vertical"); // W/S or Arrow keys

        currentAngle += pump * swingSpeed * Time.deltaTime;
        currentAngle = Mathf.Clamp(currentAngle, -maxRotation, maxRotation);

        // Auto-return when no input
        if (Mathf.Abs(pump) < 0.1f)
        {
            currentAngle = Mathf.Lerp(currentAngle, 0f, Time.deltaTime * returnSpeed);
        }
    }

    // ---------------------------------------
    // APPLY ROTATION
    // ---------------------------------------
    void ApplySwingRotation()
    {
        swing.localRotation = Quaternion.Euler(currentAngle, 0f, 0f);
    }
}
