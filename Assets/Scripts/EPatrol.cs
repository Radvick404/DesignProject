using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EPatrol : MonoBehaviour
{
    [Header("Patrol Settings")]
    public Transform[] patrolPoints;
    public float waitTime = 3f;
    public float patrolSpeed = 3.5f;

    [Header("Latch Settings")]
    public float latchRange = 3f;
    public float followDuration = 5f;
    public float relatchCooldown = 5f; // 🆕 Time before this friend can latch again

    [Header("References")]
    public Transform player;
    public SisterPatrol sisterScript;

    private NavMeshAgent agent;
    private int currentPoint = 0;
    private bool isWaiting = false;
    private bool isPlayerLatched = false;
    private bool isFollowing = false;
    private bool canLatch = true; // 🆕 prevents instant re-latching

    private Transform originalParent; // 🆕 store Player’s original parent

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = patrolSpeed;

        if (patrolPoints.Length > 0)
        {
            agent.SetDestination(patrolPoints[0].position);
        }
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // 🔹 Auto-latch when Player is close enough (only if allowed)
        if (canLatch && !isPlayerLatched && distance <= latchRange)
        {
            LatchPlayer();
        }

        // 🔹 Unlatch only with Space
        if (isPlayerLatched && Input.GetKeyDown(KeyCode.Space))
        {
            UnlatchPlayer();
        }

        // 🔹 If Sister is latched to Player, follow Player
        if (sisterScript != null && sisterScript.IsLatchedToPlayer() && !isFollowing)
        {
            StartCoroutine(FollowPlayerForTime());
        }

        // 🔹 Normal Patrol
        if (!isPlayerLatched && !isFollowing && !isWaiting && patrolPoints.Length > 0)
        {
            if (!agent.pathPending && agent.remainingDistance < 0.3f)
            {
                StartCoroutine(WaitAtPoint());
            }
        }
    }

    IEnumerator WaitAtPoint()
    {
        isWaiting = true;
        agent.isStopped = true;

        yield return new WaitForSeconds(waitTime);

        currentPoint = (currentPoint + 1) % patrolPoints.Length;
        agent.SetDestination(patrolPoints[currentPoint].position);

        agent.isStopped = false;
        isWaiting = false;
    }

    void LatchPlayer()
    {
        isPlayerLatched = true;

        // 🔹 Save original parent
        originalParent = player.parent;

        // 🔹 Parent player to this NPC
        player.SetParent(transform);

        // 🔹 Place Player slightly in front (or wherever you want)
        player.localPosition = Vector3.forward * 1f;

        // 🔹 Disable player movement
        PlayerMove pm = player.GetComponent<PlayerMove>();
        if (pm != null) pm.canMove = false;

        // 🔹 Freeze physics so it doesn’t fight the parent movement
        Rigidbody prb = player.GetComponent<Rigidbody>();
        if (prb != null) prb.isKinematic = true;
    }

    void UnlatchPlayer()
    {
        isPlayerLatched = false;

        // 🔹 Restore parent
        player.SetParent(originalParent);

        // 🔹 Re-enable player movement
        PlayerMove pm = player.GetComponent<PlayerMove>();
        if (pm != null) pm.canMove = true;

        // 🔹 Restore physics
        Rigidbody prb = player.GetComponent<Rigidbody>();
        if (prb != null) prb.isKinematic = false;

        // 🔹 Start relatch cooldown
        StartCoroutine(RelatchCooldown());

        // Resume patrol
        if (patrolPoints.Length > 0)
        {
            agent.SetDestination(patrolPoints[currentPoint].position);
        }
    }

    IEnumerator RelatchCooldown()
    {
        canLatch = false;
        yield return new WaitForSeconds(relatchCooldown);
        canLatch = true;
    }

    IEnumerator FollowPlayerForTime()
    {
        isFollowing = true;
        agent.isStopped = false;

        float elapsed = 0f;
        while (elapsed < followDuration && sisterScript.IsLatchedToPlayer())
        {
            if (player != null)
            {
                agent.SetDestination(player.position);
            }
            elapsed += Time.deltaTime;
            yield return null;
        }

        isFollowing = false;

        // Resume patrol
        if (patrolPoints.Length > 0)
        {
            agent.SetDestination(patrolPoints[currentPoint].position);
        }
    }
}
