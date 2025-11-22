using UnityEngine;
using UnityEngine.Playables;
using System.Collections;

public class BenchSeatZone : MonoBehaviour
{
    [Header("Seat Settings")]
    public Transform seatPoint;
    public Transform seatPointB;
    public KeyCode interactKey = KeyCode.E;
    public bool npcAutoSit = true;
    public float npcSitDuration = 3f;
    public string npcTag = "NPC";

    private Transform player;
    private PlayerMovement3D movement;
    private Rigidbody playerRb;

    private bool playerInside = false;
    private bool isSeated = false;
    private Coroutine npcSitCo;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            player = other.transform;
            movement = player.GetComponent<PlayerMovement3D>();
            playerRb = player.GetComponent<Rigidbody>();
            return;
        }

        if (npcAutoSit && npcSitCo == null)
        {
            bool isNpcTag = !string.IsNullOrEmpty(npcTag) && other.CompareTag(npcTag);
            bool isNpcComp = other.GetComponent<NPCWaypointPatrol>() != null;
            if (isNpcTag || isNpcComp)
            {
                npcSitCo = StartCoroutine(NPCSitRoutine(other.transform));
            }
        }
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

        Transform target = SelectSeatFor(player);

        if (target != null)
        {
            player.position = target.position;
            player.rotation = target.rotation;
        }

        if (playerRb != null)
        {
            playerRb.linearVelocity = Vector3.zero;
            playerRb.useGravity = false;
        }

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

        if (playerRb != null)
            playerRb.useGravity = true;

        Debug.Log("Player stood up");
    }

    Transform SelectSeatFor(Transform t)
    {
        Transform target = seatPoint;
        if (target == null) target = seatPointB;
        if (t != null && seatPoint != null && seatPointB != null)
        {
            if (isSeated)
                target = seatPointB;
            else
            {
                float dA = Vector3.Distance(t.position, seatPoint.position);
                float dB = Vector3.Distance(t.position, seatPointB.position);
                target = dA <= dB ? seatPoint : seatPointB;
            }
        }
        return target;
    }

    IEnumerator NPCSitRoutine(Transform npc)
    {
        if (npc == null)
        {
            npcSitCo = null;
            yield break;
        }

        var patrol = npc.GetComponent<NPCWaypointPatrol>();
        var rbNpc = npc.GetComponent<Rigidbody>();
        var originalConstraints = rbNpc != null ? rbNpc.constraints : RigidbodyConstraints.None;

        Transform target = SelectSeatFor(npc);
        if (target != null)
        {
            npc.position = target.position;
            npc.rotation = target.rotation;
        }

        if (rbNpc != null)
        {
            rbNpc.linearVelocity = Vector3.zero;
            rbNpc.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
        }
        if (patrol != null)
            patrol.enabled = false;

        yield return new WaitForSeconds(npcSitDuration);

        if (patrol != null)
            patrol.enabled = true;
        if (rbNpc != null)
            rbNpc.constraints = originalConstraints;

        npcSitCo = null;
    }
}
