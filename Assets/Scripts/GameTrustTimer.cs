using UnityEngine;
using TMPro;

public class GameTrustTimer : MonoBehaviour
{
    [Header("Timer Settings")]
    public float totalTime = 60f;
    public TMP_Text timerText;

    [Header("End Results UI")]
    public GameObject resultsPanel;

    [Header("Normal Kids (Assign 2)")]
    public TrustMeter normalKid1;
    public TrustMeter normalKid2;
    public TMP_Text normalKid1ResultText;
    public TMP_Text normalKid2ResultText;

    [Header("Autistic Kids (Assign 2)")]
    public TrustMeter autisticKid1;
    public TrustMeter autisticKid2;
    public TMP_Text autisticKid1ResultText;
    public TMP_Text autisticKid2ResultText;

    [Header("Day/Night – Directional Light")]
    public Light sunLight;
    public Gradient lightColorGradient;
    public AnimationCurve lightIntensityCurve = AnimationCurve.Linear(0, 1, 1, 0);

    [Header("Day/Night – Sun Rotation")]
    public Vector3 sunRiseRotation = new Vector3(0f, 0f, 0f);
    public Vector3 sunSetRotation = new Vector3(180f, 0f, 0f);

    [Header("Skybox Settings")]
    public Material daySkybox;
    public Material nightSkybox;
    [Range(0f, 1f)] public float skyboxBlend = 0f;
    public float skyboxBlendSpeed = 1f;

    [Header("Final Result Text")]
    public TMP_Text finalResultText;

    [Header("Stamp System")]
    public StampAnimator stampAnimator;
    public Sprite acceptSprite;
    public Sprite rejectSprite;

    [TextArea]
    public string lowTrustMessage = "They didn’t feel very comfortable today. Maybe next time you can try gentler interactions!";
    [TextArea]
    public string mediumTrustMessage = "They warmed up a bit and trusted you more. You made a small but meaningful connection!";
    [TextArea]
    public string highTrustMessage = "They felt truly safe and connected with you. You made a big positive impact today!";

    private float startTime;
    private bool timerRunning = true;
    

    void Start()
    {
        startTime = totalTime;

        if (resultsPanel)
            resultsPanel.SetActive(false);

        if (daySkybox && nightSkybox)
            RenderSettings.skybox = daySkybox;
    }

    void Update()
    {
        if (!timerRunning) return;

        totalTime -= Time.deltaTime;
        if (totalTime < 0f) totalTime = 0f;

        if (timerText)
            timerText.text = Mathf.CeilToInt(totalTime).ToString();

        float t = 1f - (totalTime / startTime);

        UpdateSunRotation(t);
        UpdateSunColor(t);
        UpdateSunIntensity(t);
        UpdateSkybox(t);

        if (totalTime <= 0f)
        {
            timerRunning = false;
            ShowResults();
        }
    }

    // ------------------------------
    // DAY-NIGHT FUNCTIONS
    // ------------------------------
    void UpdateSunRotation(float t)
    {
        if (!sunLight) return;

        sunLight.transform.rotation = Quaternion.Euler(
            Mathf.Lerp(sunRiseRotation.x, sunSetRotation.x, t),
            Mathf.Lerp(sunRiseRotation.y, sunSetRotation.y, t),
            Mathf.Lerp(sunRiseRotation.z, sunSetRotation.z, t)
        );
    }

    void UpdateSunColor(float t)
    {
        if (sunLight)
            sunLight.color = lightColorGradient.Evaluate(t);
    }

    void UpdateSunIntensity(float t)
    {
        if (sunLight)
            sunLight.intensity = lightIntensityCurve.Evaluate(t);
    }

    void UpdateSkybox(float t)
    {
        if (!daySkybox || !nightSkybox) return;

        skyboxBlend = t;
        float dayExposure = Mathf.Lerp(1f, 0f, t);
        float nightExposure = Mathf.Lerp(0f, 1f, t);

        daySkybox.SetFloat("_Exposure", dayExposure);
        nightSkybox.SetFloat("_Exposure", nightExposure);

        RenderSettings.skybox = (t > 0.5f) ? nightSkybox : daySkybox;
    }

    // ------------------------------
    // SHOW SCORES
    // ------------------------------
    void ShowResults()
    {
        resultsPanel.SetActive(true);

        // --- Normal Kids ---
        if (normalKid1ResultText)
            normalKid1ResultText.text = "Kid 1 Trust: " + normalKid1.trust;

        if (normalKid2ResultText)
            normalKid2ResultText.text = "Kid 2 Trust: " + normalKid2.trust;

        // --- Autistic Kids ---
        if (autisticKid1ResultText)
            autisticKid1ResultText.text = "Autistic Kid 1 Trust: " + autisticKid1.trust;

        if (autisticKid2ResultText)
            autisticKid2ResultText.text = "Autistic Kid 2 Trust: " + autisticKid2.trust;

        // ------------------------
        // TOTAL AUTISTIC TRUST LOGIC
        // ------------------------
        int autisticTotal = autisticKid1.trust + autisticKid2.trust;

        if (finalResultText)
        {
            if (autisticTotal >= 90)
            {
                finalResultText.text = highTrustMessage;
                stampAnimator.stampImage.sprite = acceptSprite;
                stampAnimator.PlayStamp();
            }
            else if (autisticTotal >= 50)
            {
                finalResultText.text = mediumTrustMessage;
                stampAnimator.stampImage.sprite = rejectSprite;
                stampAnimator.PlayStamp();
            }
            else
            {       finalResultText.text = lowTrustMessage;
            stampAnimator.stampImage.sprite = rejectSprite;
            stampAnimator.PlayStamp();
        }
        }
    }

}
