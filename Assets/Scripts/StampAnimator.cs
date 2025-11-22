using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StampAnimator : MonoBehaviour
{
    public Image stampImage;

    [Header("Stamp Settings")]
    public float dropHeight = 300f;
    public float dropDuration = 0.25f;
    public float squashAmount = 0.8f;
    public float squashDuration = 0.1f;
    public float rotateAngle = 12f;

    private RectTransform rt;
    private Color originalColor;

    void Awake()
    {
        rt = stampImage.GetComponent<RectTransform>();
        originalColor = stampImage.color;

        // Start hidden
        rt.localScale = Vector3.zero;
        stampImage.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
    }

    public void PlayStamp()
    {
        StartCoroutine(StampRoutine());
    }

    IEnumerator StampRoutine()
    {
        // Reset
        rt.localScale = Vector3.zero;
        stampImage.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);

        // Start above
        Vector3 startPos = rt.anchoredPosition + new Vector2(0, dropHeight);
        Vector3 endPos = rt.anchoredPosition;
        rt.anchoredPosition = startPos;

        // Random slight rotation
        float startRot = Random.Range(-rotateAngle, rotateAngle);
        rt.localRotation = Quaternion.Euler(0, 0, startRot);

        // Fade in + drop
        float t = 0f;
        while (t < dropDuration)
        {
            t += Time.deltaTime;
            float lerp = t / dropDuration;

            // Smooth drop
            rt.anchoredPosition = Vector2.Lerp(startPos, endPos, Mathf.SmoothStep(0, 1, lerp));

            // Fade in
            float alpha = Mathf.Lerp(0, 1, lerp);
            stampImage.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

            yield return null;
        }

        // Squash effect
        rt.localScale = new Vector3(1f, squashAmount, 1f);
        yield return new WaitForSeconds(squashDuration);

        // Overshoot back
        rt.localScale = new Vector3(1.05f, 1f, 1f);
        yield return new WaitForSeconds(0.05f);

        // Final stable
        rt.localScale = Vector3.one;
    }
}
