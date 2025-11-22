using UnityEngine;

public class PlayerRef : MonoBehaviour
{
    public static Transform Instance;
    public static bool isSeated = false;

    void Awake()
    {
        Instance = transform;
    }
}
