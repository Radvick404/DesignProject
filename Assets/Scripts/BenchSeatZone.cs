using UnityEngine;
using UnityEngine.Playables;

public class BenchSeatZone : MonoBehaviour
{
    [Header("Seat Settings")]
    public Transform seatPoint;
    public KeyCode interactKey = KeyCode.E;

    private Transform player;
    private PlayerMovement3D movement;
    private Rigidbody playerRb;

    private bool playerInside = false;
    private bool isSeated = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInside = true;
        player = other.transform;
        movement = player.GetComponent<PlayerMovement3D>();
        playerRb = player.GetComponent<Rigidbody>();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (!isSeated)
        {
            playerInside = false;
            player = null;
            movement = null;
            playerRb = null;
        }
    }

    private void Update()
    {
        if (!playerInside || player == null) return;

        // Sit
        if (!isSeated && Input.GetKeyDown(interactKey))
        {
            SitPlayer();
        }
        // Stand
        else if (isSeated && Input.GetKeyDown(interactKey))
        {
            StandPlayer();
        }
    }

    private void SitPlayer()
    {
        isSeated = true;
        PlayerRef.isSeated = true;     // ⭐ Set global seated state

        player.position = seatPoint.position;
        player.rotation = seatPoint.rotation;

        if (playerRb != null)
            playerRb.linearVelocity = Vector3.zero;

        if (movement != null)
            movement.enabled = false;

        Debug.Log("Player sat on bench");
    }

    private void StandPlayer()
    {
        isSeated = false;
        PlayerRef.isSeated = false;    // ⭐ Clear global seated state

        if (movement != null)
            movement.enabled = true;

        Debug.Log("Player stood up");
    }
}
