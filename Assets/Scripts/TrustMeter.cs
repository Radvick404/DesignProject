using UnityEngine;

public class TrustMeter : MonoBehaviour
{
    public int trust = 0;

    // Adjust trust but do not show any UI
    public void ModifyTrust(int amount)
    {
        trust += amount;
        trust = Mathf.Clamp(trust, 0, 100);

        Debug.Log($"[TRUST] New trust value: {trust}");
    }
}
