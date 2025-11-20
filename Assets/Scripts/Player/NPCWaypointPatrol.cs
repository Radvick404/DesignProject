using UnityEngine;

using System.Collections;



[RequireComponent(typeof(Rigidbody))]

public class NPCWaypointPatrol : MonoBehaviour

{

    [Header("Movement Settings")]

    public float moveSpeed = 3f;

    public float stopDistance = 1f;   // ★ Increased for stability

    public float waitDuration = 2f;



    [Header("Waypoints (in order)")]

    public Transform[] waypoints;



    private Rigidbody rb;

    private int currentIndex = 0;

    private Vector3 targetPos;

    private bool isWaiting = false;



    // ★ Failsafe timers

    private float stuckTimer = 0f;

    public float stuckThreshold = 4f; // seconds allowed before forcing next WP



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

        // rb.useGravity = true;

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

        if (!isWaiting)

            MoveToWaypoint();



        CheckStuckFailsafe();

    }



    void MoveToWaypoint()

    {

        Vector3 direction = targetPos - transform.position;

        direction.y = 0f;

        float distance = direction.magnitude;



        // ★ Reached waypoint? Use forgiving radius

        if (distance <= stopDistance)

        {

            GoToNextWaypoint();

            return;

        }



        stuckTimer += Time.fixedDeltaTime;



        // ★ Move toward waypoint without overshooting

        Vector3 moveDir = direction.normalized;

        Vector3 nextPos = rb.position + moveDir * moveSpeed * Time.fixedDeltaTime;



        if (Vector3.Distance(nextPos, targetPos) < distance)

            rb.MovePosition(nextPos);

        else

            GoToNextWaypoint();



        // Sprite update

        if (spriteRenderer)

            UpdateSpriteDirection(moveDir);

    }



    // ★ Handles moving to next waypoint safely

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



    // ★ Failsafe: if NPC stuck too long, force to next WP

    void CheckStuckFailsafe()

    {

        if (isWaiting) return;



        if (stuckTimer >= stuckThreshold)

        {

            Debug.LogWarning($"{name} was stuck — forcing next waypoint.");

            GoToNextWaypoint();

        }

    }



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



    void OnDrawGizmosSelected()

    {

        Gizmos.color = Color.yellow;

        if (waypoints == null || waypoints.Length == 0) return;

        for (int i = 0; i < waypoints.Length; i++)

        {

            if (waypoints[i] == null) continue;

            Gizmos.DrawSphere(waypoints[i].position, 0.15f);

            Transform next = waypoints[(i + 1) % waypoints.Length];

            if (next) Gizmos.DrawLine(waypoints[i].position, next.position);

        }

    }

}