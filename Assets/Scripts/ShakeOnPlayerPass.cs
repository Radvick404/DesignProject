using UnityEngine;

[DisallowMultipleComponent]
public class ShakeOnPlayerPass : MonoBehaviour
{
    public string playerTag = "Player";
    public bool oneShot = false;
    public bool requireCooldown = true;
    public float cooldown = 0.5f;

    public bool useLocalPosition = true;
    public float duration = 0.5f;
    public float magnitude = 0.2f;
    public AnimationCurve falloff = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
    public bool shakeX = true;
    public bool shakeY = true;
    public bool shakeZ = true;

    Coroutine routine;
    float lastShakeTime = -999f;

    void OnTriggerEnter(Collider other)
    {
        if (!string.IsNullOrEmpty(playerTag) && !other.CompareTag(playerTag)) return;
        TryShake();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!string.IsNullOrEmpty(playerTag) && !other.CompareTag(playerTag)) return;
        TryShake();
    }

    public void TriggerShake()
    {
        TryShake();
    }

    void TryShake()
    {
        if (requireCooldown && Time.time - lastShakeTime < cooldown) return;
        lastShakeTime = Time.time;
        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(ShakeRoutine());
    }

    System.Collections.IEnumerator ShakeRoutine()
    {
        float t = 0f;
        Vector3 basePos = useLocalPosition ? transform.localPosition : transform.position;

        while (t < duration)
        {
            float norm = t / duration;
            float amp = magnitude * falloff.Evaluate(norm);

            float ox = shakeX ? Random.Range(-1f, 1f) : 0f;
            float oy = shakeY ? Random.Range(-1f, 1f) : 0f;
            float oz = shakeZ ? Random.Range(-1f, 1f) : 0f;

            Vector3 offset = new Vector3(ox, oy, oz) * amp;

            if (useLocalPosition) transform.localPosition = basePos + offset;
            else transform.position = basePos + offset;

            t += Time.deltaTime;
            yield return null;
        }

        if (useLocalPosition) transform.localPosition = basePos;
        else transform.position = basePos;

        routine = null;

        if (oneShot) enabled = false;
    }
}