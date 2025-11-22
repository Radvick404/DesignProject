using UnityEngine;

public class TrustMeter : MonoBehaviour
{
    public int trust = 0;
    public AudioClip trustIncreaseSFX;
    public AudioClip trustDecreaseSFX;

    // Adjust trust but do not show any UI
    public void ModifyTrust(int amount)
    {
        int oldTrust = trust;
        trust += amount;

        // Clamp trust value if you have max/min
        // trust = Mathf.Clamp(trust, 0, maxTrust);

        // Detect increase
        if (trust > oldTrust)
        {
            if (trustIncreaseSFX)
                AudioManager.Instance.PlaySFX(trustIncreaseSFX);
        }
        // Detect decrease
        else if (trust < oldTrust)
        {
            if (trustDecreaseSFX)
                AudioManager.Instance.PlaySFX(trustDecreaseSFX);
        }

        Debug.Log("Trust now = " + trust);
    }

}
