using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class SisterPatrol : MonoBehaviour
{
    [Header("Patrol Settings")]
    public Transform[] patrolPoints;
    public float WaitTime = 3f;
    public float PatrolSpeed = 3.5f;
    public float interactRange = 3f;
    public float latchDuration = 60f;

    [Header("Latch Settings")]
    [Tooltip("Local offset from the player's transform while latched (x = right, y = up, z = forward).")]
    public Vector3 latchOffsetLocal = new Vector3(1f, 0f, -0.5f);
    [Tooltip("How fast Sister follows the target latch position")]
    public float latchFollowSmooth = 10f;
    [Tooltip("If true, Sister's collider will become a trigger while latched (so she doesn't block the player).")]
    public bool disablePhysicsBlockingWhileLatched = true;
    [Tooltip("Allow player pressing Space to force-unlatch Sister")]
    public bool allowUnlatchWithSpace = true;

    private NavMeshAgent agent;
    private int currentPoint = 0;
    private bool isWaiting = false;
    private bool isLatched = false;

    private Transform player;
    private Collider myCollider;
    private Coroutine latchCoroutine;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent != null) agent.speed = PatrolSpeed;

        myCollider = GetComponent<Collider>();

        if (patrolPoints != null && patrolPoints.Length > 0)
            agent.SetDestination(patrolPoints[0].position);

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;

        // Helpful warning if collider/rigidbody are missing (trigger detection works best with a Rigidbody)
        if (myCollider == null)
            Debug.LogWarning($"SisterPatrol on '{gameObject.name}' has no Collider — add one so triggers work properly.");
    }

    void Update()
    {
        // If latched, follow player with an offset (smooth)
        if (isLatched && player != null)
        {
            Vector3 desired = player.TransformPoint(latchOffsetLocal); // offset in player's local space
            transform.position = Vector3.Lerp(transform.position, desired, Time.deltaTime * latchFollowSmooth);

            // Optional: smooth rotation to roughly match player's facing
            transform.rotation = Quaternion.Lerp(transform.rotation,
                Quaternion.LookRotation(player.forward, Vector3.up),
                Time.deltaTime * latchFollowSmooth);

            // allow player to unlatch with space if enabled
            if (allowUnlatchWithSpace && Input.GetKeyDown(KeyCode.Space))
            {
                ForceUnlatch();
            }

            return; // skip patrol logic while latched
        }

        if (patrolPoints == null || patrolPoints.Length == 0 || isWaiting) return;

        // patrol
        if (!agent.pathPending && agent.remainingDistance < 0.3f)
            StartCoroutine(WaitAtPoint());

        // check for latch input (E) if player in range
        if (player != null && Vector3.Distance(transform.position, player.position) <= interactRange)
        {
            if (Input.GetKeyDown(KeyCode.E) && !isLatched && latchCoroutine == null)
            {
                latchCoroutine = StartCoroutine(LatchOntoPlayerCoroutine());
            }
        }
    }

    IEnumerator WaitAtPoint()
    {
        isWaiting = true;
        if (agent != null) agent.isStopped = true;

        yield return new WaitForSeconds(WaitTime);

        currentPoint = (currentPoint + 1) % patrolPoints.Length;
        if (agent != null) agent.SetDestination(patrolPoints[currentPoint].position);

        if (agent != null) agent.isStopped = false;
        isWaiting = false;
    }

    IEnumerator LatchOntoPlayerCoroutine()
    {
        if (player == null)
        {
            latchCoroutine = null;
            yield break;
        }

        isLatched = true;
        if (agent != null) agent.isStopped = true;

        // Make Sister not physically block the player by making her collider a trigger
        if (disablePhysicsBlockingWhileLatched && myCollider != null)
        {
            myCollider.isTrigger = true;
        }

        float elapsed = 0f;
        while (elapsed < latchDuration && isLatched)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        // auto-unlatch after duration (if still latched)
        ForceUnlatch();
        latchCoroutine = null;
    }

    /// <summary>
    /// Force unlatch immediately (callable from other scripts, or used internally).
    /// </summary>
    public void ForceUnlatch()
    {
        if (!isLatched) return;

        isLatched = false;

        if (disablePhysicsBlockingWhileLatched && myCollider != null)
        {
            myCollider.isTrigger = false;
        }

        if (agent != null)
        {
            agent.isStopped = false;
            // resume to nearest patrol point
            int nearest = GetNearestPatrolPointIndex();
            if (patrolPoints != null && patrolPoints.Length > 0)
                agent.SetDestination(patrolPoints[nearest].position);
        }

        // stop any running latch coroutine
        if (latchCoroutine != null)
        {
            StopCoroutine(latchCoroutine);
            latchCoroutine = null;
        }
    }

    int GetNearestPatrolPointIndex()
    {
        if (patrolPoints == null || patrolPoints.Length == 0) return 0;

        float bestDist = Mathf.Infinity;
        int bestIndex = 0;
        for (int i = 0; i < patrolPoints.Length; i++)
        {
            float d = Vector3.Distance(transform.position, patrolPoints[i].position);
            if (d < bestDist)
            {
                bestDist = d;
                bestIndex = i;
            }
        }
        return bestIndex;
    }

    public bool IsLatchedToPlayer()
    {
        return isLatched;
    }

    void OnDisable()
    {
        // make sure collider is reset if object gets disabled
        if (myCollider != null && disablePhysicsBlockingWhileLatched)
            myCollider.isTrigger = false;
    }
}
