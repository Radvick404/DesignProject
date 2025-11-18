using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement3D : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    [Header("Sprite Setup")]
    public SpriteRenderer spriteRenderer;
    public Sprite frontSprite;
    public Sprite backSprite;
    public Sprite sideSprite;
    public Sprite sideSpriteLeft;

    private Rigidbody rb;
    private Vector2 moveInput;
    private Vector3 moveDir;
    private string currentDirection = "Front";

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation; // prevent tipping
        rb.useGravity = true;                                 // ✅ gravity on at start

        if (!spriteRenderer)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    // Input System callback
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    void FixedUpdate()
    {
        // --- Movement ---
        moveDir = new Vector3(moveInput.x, 0f, moveInput.y);

        if (moveDir.sqrMagnitude > 0.001f)
        {
            Vector3 targetPos = rb.position + moveDir.normalized * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(targetPos);

            UpdateSpriteDirection(moveDir);
        }
    }

    void UpdateSpriteDirection(Vector3 dir)
    {
        bool horizontal = Mathf.Abs(dir.x) > Mathf.Abs(dir.z);

        if (horizontal)
        {
            currentDirection = "Side";
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
            if (dir.z > 0.05f)
            {
                currentDirection = "Back";
                spriteRenderer.sprite = backSprite;
            }
            else if (dir.z < -0.05f)
            {
                currentDirection = "Front";
                spriteRenderer.sprite = frontSprite;
            }
        }
    }
}
