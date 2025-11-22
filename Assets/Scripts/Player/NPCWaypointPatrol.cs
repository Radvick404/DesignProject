using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class NPCWaypointPatrol : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 3f;
    public float stopDistance = 1f;
    public float waitDuration = 2f;

    [Header("Waypoints (normal patrol)")]
    public Transform[] waypoints;

    [Header("Final Waypoint Unlock")]
    public Transform finalWaypoint;       // SPECIAL waypoint (locked initially)
    public TrustMeter trustMeter;         // NPC's trust script
    public int requiredTrust = 70;        // Unlock threshold
    private bool finalWaypointUnlocked = false;

    private Rigidbody rb;
    private int currentIndex = 0;
    private Vector3 targetPos;
    private bool isWaiting = false;

    // Failsafe
    private float stuckTimer = 0f;
    public float stuckThreshold = 4f;

    [Header("Optional Sprite")]
    public SpriteRenderer spriteRenderer;
    public Sprite frontSprite;
    public Sprite backSprite;
    public Sprite sideSprite;
    public Sprite sideSpriteLeft;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    void Start()
    {
        if (waypoints.Length == 0)
        {
            Debug.LogWarning($"{name} has no waypoints assigned!");
            enabled = false;
            return;
        }

        targetPos = waypoints[currentIndex].position;
    }

    void FixedUpdate()
    {
        CheckFinalWaypointUnlock();

        if (!isWaiting)
            MoveToWaypoint();

        CheckStuckFailsafe();
    }

    // -------------------------------------------------------------------
    // ⭐ UNLOCK FINAL WAYPOINT WHEN TRUST > REQUIRED TRUST
    // -------------------------------------------------------------------
    void CheckFinalWaypointUnlock()
    {
        if (finalWaypointUnlocked)
            return;

        if (trustMeter != null && trustMeter.trust >= requiredTrust)
        {
            Debug.Log($"{name} unlocked final waypoint! (trust = {trustMeter.trust})");

            // Expand waypoint array dynamically
            Transform[] newArray = new Transform[waypoints.Length + 1];

            for (int i = 0; i < waypoints.Length; i++)
                newArray[i] = waypoints[i];

            newArray[waypoints.Length] = finalWaypoint;

            waypoints = newArray;
            finalWaypointUnlocked = true;
        }
    }

    // -------------------------------------------------------------------
    void MoveToWaypoint()
    {
        Vector3 direction = targetPos - transform.position;
        direction.y = 0f;
        float distance = direction.magnitude;

        if (distance <= stopDistance)
        {
            GoToNextWaypoint();
            return;
        }

        stuckTimer += Time.fixedDeltaTime;

        Vector3 moveDir = direction.normalized;
        Vector3 nextPos = rb.position + moveDir * moveSpeed * Time.fixedDeltaTime;

        if (Vector3.Distance(nextPos, targetPos) < distance)
            rb.MovePosition(nextPos);
        else
            GoToNextWaypoint();

        if (spriteRenderer)
            UpdateSpriteDirection(moveDir);
    }

    // -------------------------------------------------------------------
    void GoToNextWaypoint()
    {
        if (isWaiting) return;

        isWaiting = true;
        stuckTimer = 0f;
        StartCoroutine(WaitAndMoveNext());
    }

    IEnumerator WaitAndMoveNext()
    {
        yield return new WaitForSeconds(waitDuration);

        currentIndex = (currentIndex + 1) % waypoints.Length;
        targetPos = waypoints[currentIndex].position;

        isWaiting = false;
        stuckTimer = 0f;
    }

    // -------------------------------------------------------------------
    void CheckStuckFailsafe()
    {
        if (isWaiting) return;

        if (stuckTimer >= stuckThreshold)
        {
            Debug.LogWarning($"{name} was stuck — forcing next waypoint.");
            GoToNextWaypoint();
        }
    }

    // -------------------------------------------------------------------
    void UpdateSpriteDirection(Vector3 dir)
    {
        bool horizontal = Mathf.Abs(dir.x) > Mathf.Abs(dir.z);

        if (horizontal)
        {
            if (dir.x < 0f && sideSpriteLeft != null)
            {
                spriteRenderer.sprite = sideSpriteLeft;
                spriteRenderer.flipX = false;
            }
            else
            {
                spriteRenderer.sprite = sideSprite;
                spriteRenderer.flipX = dir.x < 0f;
            }
        }
        else
        {
            if (dir.z > 0f)
                spriteRenderer.sprite = backSprite;
            else
                spriteRenderer.sprite = frontSprite;
        }
    }

    // -------------------------------------------------------------------
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        if (waypoints != null)
        {
            for (int i = 0; i < waypoints.Length; i++)
            {
                if (!waypoints[i]) continue;
                Gizmos.DrawSphere(waypoints[i].position, 0.15f);

                if (i < waypoints.Length - 1)
                    Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
            }
        }

        if (finalWaypoint && !finalWaypointUnlocked)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(finalWaypoint.position, 0.2f);
        }
    }
}
