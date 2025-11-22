using UnityEngine;

public class MerryGoRoundInteraction : MonoBehaviour
{
    [Header("Player Setup")]
    public Transform seatPoint;               // Where player snaps to
    public string playerTag = "Player";

    [Header("Merry-Go-Round Settings")]
    public Transform merryGoRound;            // The rotating platform
    public float rotationSpeed = 60f;         // Speed of rotation when pressing left/right

    [Header("UI Prompt")]
    public GameObject pressXIcon;             // Icon above player's head: "Press X"

    [Header("Input Keys")]
    public KeyCode interactKey = KeyCode.X;

    private bool isSeated = false;
    private bool playerInside = false;
    private Transform player;
    private Rigidbody playerRb;


    void Awake()
    {
        if (pressXIcon)
            pressXIcon.SetActive(false);
    }

    // -----------------------------
    // Trigger Enter
    // -----------------------------
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag))
            return;

        player = other.transform;
        playerInside = true;

        if (pressXIcon)
            pressXIcon.SetActive(true);
    }

    // -----------------------------
    // Trigger Exit
    // -----------------------------
    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(playerTag))
            return;

        playerInside = false;

        if (!isSeated && pressXIcon)
            pressXIcon.SetActive(false);
    }

    // -----------------------------
    // UPDATE � main logic
    // -----------------------------
    void Update()
    {
        // Press X to sit when inside
        if (playerInside && !isSeated && Input.GetKeyDown(interactKey))
        {
            SeatPlayer();
            return;
        }

        // Press X again to get off
        if (isSeated && Input.GetKeyDown(interactKey))
        {
            ReleasePlayer();
            return;
        }

        // Rotate merry-go-round
        if (isSeated)
        {
            HandleRotation();
            KeepPlayerOnSeat();
        }
    }

    // -----------------------------
    // SEAT PLAYER
    // -----------------------------
    void SeatPlayer()
    {
        if (!player) return;

        isSeated = true;

        // Freeze player physics
        if (player.TryGetComponent<Rigidbody>(out playerRb))
        {
            playerRb.isKinematic = true;
            playerRb.linearVelocity = Vector3.zero;
        }

        // Snap player to seat location
        player.position = seatPoint.position;
        player.rotation = Quaternion.Euler(0, merryGoRound.eulerAngles.y, 0);

        // Hide the X prompt
        if (pressXIcon)
            pressXIcon.SetActive(false);

        Debug.Log("Player seated on Merry-Go-Round");
    }

    // -----------------------------
    // RELEASE PLAYER
    // -----------------------------
    void ReleasePlayer()
    {
        isSeated = false;

        if (playerRb != null)
            playerRb.isKinematic = false;

        // Show prompt if still inside zone
        if (playerInside && pressXIcon)
            pressXIcon.SetActive(true);

        Debug.Log("Player released from Merry-Go-Round");
    }

    // -----------------------------
    // ROTATION MECHANIC
    // -----------------------------
    void HandleRotation()
    {
        float input = Input.GetAxis("Horizontal");  // A/D or Left/Right

        if (Mathf.Abs(input) > 0.1f)
        {
            // Rotate platform around Y axis
            merryGoRound.Rotate(Vector3.up, input * rotationSpeed * Time.deltaTime);
        }
    }

    // -----------------------------
    // KEEP PLAYER ON SEAT
    // -----------------------------
    void KeepPlayerOnSeat()
    {
        if (!player) return;

        player.position = seatPoint.position;

        // Keep player always upright
        player.rotation = Quaternion.Euler(0, merryGoRound.eulerAngles.y, 0);
    }
}
