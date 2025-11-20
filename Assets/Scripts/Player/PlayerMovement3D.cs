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

    [Header("Wobble Settings")]
    public float wobbleAmount = 0.06f;
    public float wobbleSpeed = 8f;
    public float returnSpeed = 10f;

    [Header("Dust Trail Settings")]
    public ParticleSystem dustParticles;
    public float movementThreshold = 0.2f;   // minimum input strength before tracking movement
    public float dustStartDelay = 0.25f;     // continuous movement required before dust starts
    public float emitRate = 12f;             // dust emission rate

    private float movementTimer = 0f;
    private bool dustActive = false;

    private Vector3 defaultScale;
    private Rigidbody rb;
    private Vector2 moveInput;
    private Vector3 moveDir;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.useGravity = true;

        if (!spriteRenderer)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        defaultScale = spriteRenderer.transform.localScale;

        // Disable dust at start
        if (dustParticles)
        {
            var emission = dustParticles.emission;
            emission.rateOverTime = 0;
        }
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    void FixedUpdate()
    {
        moveDir = new Vector3(moveInput.x, 0f, moveInput.y);
        float speedCheck = moveDir.magnitude;

        if (speedCheck > movementThreshold)
        {
            // Move & wobble
            Vector3 targetPos = rb.position + moveDir.normalized * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(targetPos);

            UpdateSpriteDirection(moveDir);
            ApplyWobbleEffect(true);

            HandleDust(true);
        }
        else
        {
            ApplyWobbleEffect(false);
            HandleDust(false);
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
            if (dir.z > 0.05f)
                spriteRenderer.sprite = backSprite;
            else if (dir.z < -0.05f)
                spriteRenderer.sprite = frontSprite;
        }
    }

    // ----------------------
    // WOBBLE STRETCH
    // ----------------------
    void ApplyWobbleEffect(bool moving)
    {
        if (moving)
        {
            float wobble = Mathf.Sin(Time.time * wobbleSpeed) * wobbleAmount;
            spriteRenderer.transform.localScale = defaultScale + new Vector3(wobble, -wobble, 0f);
        }
        else
        {
            spriteRenderer.transform.localScale =
                Vector3.Lerp(spriteRenderer.transform.localScale, defaultScale, Time.deltaTime * returnSpeed);
        }
    }

    // ----------------------
    // DUST LOGIC
    // ----------------------
    void HandleDust(bool moving)
    {
        if (!dustParticles) return; // no particle assigned

        var emission = dustParticles.emission;

        if (moving)
        {
            movementTimer += Time.deltaTime;

            if (!dustActive && movementTimer >= dustStartDelay)
            {
                dustActive = true;
                emission.rateOverTime = emitRate;
            }
        }
        else
        {
            movementTimer = 0f;

            if (dustActive)
            {
                dustActive = false;
                emission.rateOverTime = 0;
            }
        }
    }
}
